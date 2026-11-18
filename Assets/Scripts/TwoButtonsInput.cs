using UnityEngine;

// PROVISORIO: el mueble tiene solo 2 botones (llegan como Z y X).
// Con 1 / 2 / 3 se elige que controlan:
//   1 -> Platforms (character 1 y 2)
//   2 -> Trees (character 1 y 2)
//   3 -> Viento (izquierda / derecha)
// Reasigna los keyCode en runtime: el modo activo recibe Z/X y el resto queda en KeyCode.None.
public class TwoButtonsInput : MonoBehaviour
{
    public enum Mode { Platforms = 1, Trees = 2, Wind = 3 }

    [SerializeField] PlatformsGame.Platforms platforms;
    [SerializeField] TreesGame.Trees trees;
    [SerializeField] WindFX wind;

    [SerializeField] KeyCode button1 = KeyCode.Z;
    [SerializeField] KeyCode button2 = KeyCode.X;
    [SerializeField] Mode startMode = Mode.Platforms;

    [Header("Mensaje en pantalla")]
    [SerializeField] float messageDuration = 2f;
    [SerializeField] int fontSize = 60;

    Mode mode;
    string message;
    float messageUntil;
    GUIStyle style;

    void Start()
    {
        if (platforms == null) platforms = FindFirstObjectByType<PlatformsGame.Platforms>();
        if (trees == null) trees = FindFirstObjectByType<TreesGame.Trees>();
        if (wind == null) wind = FindFirstObjectByType<WindFX>();

        SetMode(startMode, showMessage: false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) SetMode(Mode.Platforms);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) SetMode(Mode.Trees);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) SetMode(Mode.Wind);
    }

    void SetMode(Mode newMode, bool showMessage = true)
    {
        mode = newMode;

        if (platforms != null && platforms.characters != null)
            for (int i = 0; i < platforms.characters.Length; i++)
                if (platforms.characters[i] != null)
                    platforms.characters[i].keyCode = mode == Mode.Platforms ? KeyFor(i) : KeyCode.None;

        if (trees != null && trees.characters != null)
            for (int i = 0; i < trees.characters.Length; i++)
                if (trees.characters[i] != null)
                    trees.characters[i].keyCode = mode == Mode.Trees ? KeyFor(i) : KeyCode.None;

        if (wind != null)
        {
            if (mode == Mode.Wind) wind.SetKeys(button1, button2);
            else wind.SetKeys(KeyCode.None, KeyCode.None);
        }

        if (showMessage)
        {
            message = mode switch
            {
                Mode.Platforms => "Controlando: PLATAFORMAS\nZ = personaje 1   X = personaje 2",
                Mode.Trees => "Controlando: ÁRBOLES\nZ = personaje 1   X = personaje 2",
                _ => "Controlando: VIENTO\nZ = izquierda   X = derecha",
            };
            messageUntil = Time.time + messageDuration;
        }
        Debug.Log("TwoButtonsInput: modo " + mode);
    }

    // character 0 -> boton 1 (Z), character 1 -> boton 2 (X), el resto sin tecla
    KeyCode KeyFor(int index)
    {
        if (index == 0) return button1;
        if (index == 1) return button2;
        return KeyCode.None;
    }

    void OnGUI()
    {
        if (Time.time > messageUntil || string.IsNullOrEmpty(message)) return;

        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
            };
        }
        style.fontSize = fontSize;

        Rect r = new Rect(0, 0, Screen.width, Screen.height);
        // sombra para que se lea sobre cualquier fondo
        style.normal.textColor = Color.black;
        GUI.Label(new Rect(r.x + 3, r.y + 3, r.width, r.height), message, style);
        style.normal.textColor = Color.white;
        GUI.Label(r, message, style);
    }
}
