using UnityEngine;
using UnityEngine.U2D.Animation;

public class WindFX : MonoBehaviour
{
    public enum Direction { None, Left, Right }

    [System.Serializable]
    public class PlantLayer
    {
        public string name = "Layer";
        public SpriteSkin[] spriteSkins;

        [Header("Bend")]
        [Tooltip("Grados de inclinación por hueso (se acumulan a lo largo de la cadena).")]
        public float bendPerBone = 8f;
        [Tooltip("Cuánto más se dobla la punta respecto de la base (1 = igual en todos los huesos).")]
        public float tipMultiplier = 1.5f;
        [Tooltip("Velocidad con la que la planta llega a la inclinación.")]
        public float bendSpeed = 3f;
        [Tooltip("Velocidad con la que la planta vuelve al reposo al soltar la tecla.")]
        public float releaseSpeed = 1.5f;

        [Header("Wind sway")]
        [Tooltip("Oscilación extra en grados por hueso mientras sopla el viento.")]
        public float swayAmount = 2f;
        public float swaySpeed = 2f;

        [Header("Idle")]
        [Tooltip("Oscilación en grados por hueso cuando no hay viento.")]
        public float idleAmount = 1.5f;
        public float idleSpeed = 0.25f;

        [System.NonSerialized] public Plant[] plants;
        [System.NonSerialized] public float currentBend;
    }

    public class Plant
    {
        public Transform[] bones;
        public Quaternion[] restRotations;
        public float seed;
    }

    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform target;
        [Tooltip("Unidades por segundo con viento a pleno. Negativo invierte el sentido.")]
        public float speed = 1f;
        [Tooltip("Al llegar a x <= -limit o x >= limit (posición local) vuelve a x = 0.")]
        public float limit = 10f;
    }

    [SerializeField] GameObject windLeft;
    [SerializeField] GameObject windRight;
    [SerializeField] KeyCode leftKey = KeyCode.Q;
    [SerializeField] KeyCode rightKey = KeyCode.W;
    [SerializeField] PlantLayer[] layers;

    [Header("Parallax")]
    [SerializeField] ParallaxLayer[] parallax;
    [Tooltip("Qué tan rápido el parallax acelera / frena al empezar o dejar de soplar.")]
    [SerializeField] float parallaxAcceleration = 2f;
    float parallaxWind;

    // Campos viejos (antes había una sola capa). Se migran automáticamente a layers[0].
    [SerializeField, HideInInspector] SpriteSkin[] spriteSkin;
    [SerializeField, HideInInspector] float bendPerBone = 8f;
    [SerializeField, HideInInspector] float tipMultiplier = 1.5f;
    [SerializeField, HideInInspector] float bendSpeed = 3f;
    [SerializeField, HideInInspector] float releaseSpeed = 1.5f;
    [SerializeField, HideInInspector] float swayAmount = 2f;
    [SerializeField, HideInInspector] float swaySpeed = 2f;
    [SerializeField, HideInInspector] float idleAmount = 1.5f;
    [SerializeField, HideInInspector] float idleSpeed = 0.25f;

    Direction currentDirection = Direction.None;
    float targetBend;   // -1 izquierda, 1 derecha, 0 idle

    void OnValidate() => MigrateLegacy();

    void MigrateLegacy()
    {
        if (spriteSkin == null || spriteSkin.Length == 0) return;
        if (layers != null && layers.Length > 0) return;

        layers = new[]
        {
            new PlantLayer
            {
                name = "Plantas",
                spriteSkins = spriteSkin,
                bendPerBone = bendPerBone,
                tipMultiplier = tipMultiplier,
                bendSpeed = bendSpeed,
                releaseSpeed = releaseSpeed,
                swayAmount = swayAmount,
                swaySpeed = swaySpeed,
                idleAmount = idleAmount,
                idleSpeed = idleSpeed,
            }
        };
        spriteSkin = null;
    }

    void Start()
    {
        MigrateLegacy();
        layers ??= new PlantLayer[0];

        foreach (PlantLayer layer in layers)
        {
            SpriteSkin[] skins = layer.spriteSkins ?? new SpriteSkin[0];
            layer.plants = new Plant[skins.Length];
            for (int i = 0; i < skins.Length; i++)
            {
                if (skins[i] == null) continue;
                Transform[] bones = skins[i].boneTransforms;
                Quaternion[] rest = new Quaternion[bones.Length];
                for (int b = 0; b < bones.Length; b++)
                    rest[b] = bones[b].localRotation;
                layer.plants[i] = new Plant { bones = bones, restRotations = rest, seed = Random.value * 100f };
            }
        }
        Wind(Direction.None);
    }

    void Update()
    {
        Direction direction = Direction.None;
        if (Input.GetKey(leftKey))
            direction = Direction.Left;
        else if (Input.GetKey(rightKey))
            direction = Direction.Right;

        if (direction != currentDirection)
            Wind(direction);
    }

    void LateUpdate()
    {
        foreach (PlantLayer layer in layers)
            AnimateLayer(layer);
        AnimateParallax();
    }

    void AnimateParallax()
    {
        if (parallax == null) return;

        parallaxWind = Mathf.Lerp(parallaxWind, targetBend, 1f - Mathf.Exp(-parallaxAcceleration * Time.deltaTime));

        foreach (ParallaxLayer p in parallax)
        {
            if (p == null || p.target == null) continue;
            Vector3 pos = p.target.localPosition;
            pos.x += parallaxWind * p.speed * Time.deltaTime;
            if (pos.x <= -p.limit || pos.x >= p.limit)
                pos.x = 0f;
            p.target.localPosition = pos;
        }
    }

    void AnimateLayer(PlantLayer layer)
    {
        if (layer.plants == null) return;

        float speed = targetBend == 0f ? layer.releaseSpeed : layer.bendSpeed;
        layer.currentBend = Mathf.Lerp(layer.currentBend, targetBend, 1f - Mathf.Exp(-speed * Time.deltaTime));
        float windStrength = Mathf.Abs(layer.currentBend);

        foreach (Plant plant in layer.plants)
        {
            if (plant == null) continue;
            int count = plant.bones.Length;

            float windSway = (Mathf.PerlinNoise(plant.seed, Time.time * layer.swaySpeed) - 0.5f) * 2f * layer.swayAmount * windStrength;
            float idleSway = (Mathf.PerlinNoise(plant.seed + 50f, Time.time * layer.idleSpeed) - 0.5f) * 2f * layer.idleAmount * (1f - windStrength);

            for (int b = 0; b < count; b++)
            {
                float t = count > 1 ? (float)b / (count - 1) : 1f; // 0 base, 1 punta
                float weight = Mathf.Lerp(1f, layer.tipMultiplier, t);
                // Derecha = rotación negativa en Z (sentido horario)
                float angle = -(layer.currentBend * layer.bendPerBone + windSway + idleSway) * weight;
                plant.bones[b].localRotation = plant.restRotations[b] * Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    public void Wind(Direction direction)
    {
        currentDirection = direction;
        if (windLeft != null) windLeft.SetActive(direction == Direction.Left);
        if (windRight != null) windRight.SetActive(direction == Direction.Right);
        targetBend = direction == Direction.Left ? -1f : direction == Direction.Right ? 1f : 0f;
    }
}
