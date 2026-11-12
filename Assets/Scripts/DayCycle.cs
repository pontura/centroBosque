using UnityEngine;

public class DayCycle : MonoBehaviour
{
    [SerializeField] Animator animator;
    [Tooltip("Nombre del estado a controlar. Vacío = el estado actual del layer.")]
    [SerializeField] string stateName;
    [SerializeField] int layer = 0;

    [Tooltip("Cuántos ciclos completos de la animación avanza por segundo con valor 1.")]
    [SerializeField] float cyclesPerSecond = 0.25f;
    [Tooltip("Si llega al final vuelve al principio (y viceversa). Si no, se frena en los extremos.")]
    [SerializeField] bool loop = true;

    [Header("Mouse (emulación)")]
    [SerializeField] bool useMouse = true;
    [Tooltip("Cuánto movimiento del mouse equivale a 1. Fracción de la altura de pantalla por frame.")]
    [SerializeField] float mouseSensitivity = 50f;

    int stateHash;
    float normalizedTime;
    Vector3 lastMousePosition;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();

        animator.Update(0f);
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
        stateHash = string.IsNullOrEmpty(stateName) ? info.fullPathHash : Animator.StringToHash(stateName);
        normalizedTime = loop ? Mathf.Repeat(info.normalizedTime, 1f) : Mathf.Clamp01(info.normalizedTime);

        animator.speed = 0f; // el tiempo lo controlamos nosotros
        animator.Play(stateHash, layer, normalizedTime);

        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (!useMouse) return;

        if (Input.GetMouseButtonDown(0))
            lastMousePosition = Input.mousePosition; // evita un salto al apretar

        if (!Input.GetMouseButton(0)) return;

        Vector3 mouse = Input.mousePosition;
        float deltaY = (mouse.y - lastMousePosition.y) / Screen.height;
        lastMousePosition = mouse;

        Move(Mathf.Clamp(deltaY * mouseSensitivity, -1f, 1f));
    }

    /// <summary>
    /// Avanza (positivo) o retrocede (negativo) en la línea de tiempo de la animación.
    /// value entre -1 y 1; 0 = quieto.
    /// </summary>
    public void Move(float value)
    {
        value = Mathf.Clamp(value, -1f, 1f);
        if (value == 0f) return;

        normalizedTime += value * cyclesPerSecond * Time.deltaTime;
        normalizedTime = loop ? Mathf.Repeat(normalizedTime, 1f) : Mathf.Clamp01(normalizedTime);

        animator.Play(stateHash, layer, normalizedTime);
    }
}
