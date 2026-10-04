using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BallGravityController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camara;

    [Header("Movimiento")]
    public float sensibilidadMouse = 0.1f;
    public float inclinacionMaxima = 20f;

    [Header("Física")]
    public float fuerzaGravedad = 80f;
    public float velocidadRetorno = 8f;
    public float velocidadMaxima = 15f;

    public float inclinacionX;
    public float inclinacionZ;

    [Header("Stun")]
    public bool stun = false;
    public float duracionStun = 1.0f;

    [Header("Caída")]
    public float velocidadCaidaMaxima = 100f;

    [HideInInspector]
    public bool cayendoEnHoyo = false;

    [Header("Viento")]
    public float fuerzaViento = 80f;

    private bool dentroDelViento = false;
    private Vector3 direccionViento;

    [HideInInspector]
    public bool vivo = true;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
    }

    void Update()
    {
        // ==========================
        // Muerte por caída
        // ==========================

        if (vivo && transform.position.y < GameManager.Instance.NivelActual.CaidaMuerte)
        {
            vivo = false;
        }

        // ==========================
        // STUN
        // ==========================

        if (stun)
        {
            // Mientras está aturdida, la inclinación
            // permanece completamente neutral.
            inclinacionX = 0f;
            inclinacionZ = 0f;

            return;
        }

        // ==========================
        // Movimiento del mouse
        // ==========================

        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            inclinacionZ += delta.x * sensibilidadMouse;
            inclinacionX += delta.y * sensibilidadMouse;

            inclinacionX = Mathf.Clamp(
                inclinacionX,
                -inclinacionMaxima,
                inclinacionMaxima
            );

            inclinacionZ = Mathf.Clamp(
                inclinacionZ,
                -inclinacionMaxima,
                inclinacionMaxima
            );
        }
        else
        {
            inclinacionX = Mathf.Lerp(
                inclinacionX,
                0f,
                velocidadRetorno * Time.deltaTime
            );

            inclinacionZ = Mathf.Lerp(
                inclinacionZ,
                0f,
                velocidadRetorno * Time.deltaTime
            );
        }
    }

    void FixedUpdate()
    {
        // ==========================
        // Muerte
        // ==========================

        if (!vivo)
        {
            Respawn();
            return;
        }

        // ==========================
        // Caída en hoyo
        // ==========================

        if (cayendoEnHoyo)
        {
            Vector3 v = rb.linearVelocity;

            rb.linearVelocity = new Vector3(
                Mathf.Lerp(v.x, 0f, 8f * Time.fixedDeltaTime),
                v.y,
                Mathf.Lerp(v.z, 0f, 8f * Time.fixedDeltaTime)
            );

            return;
        }

        // ==========================
        // VIENTO
        // ==========================

        if (dentroDelViento)
        {
            rb.AddForce(
                direccionViento * fuerzaViento,
                ForceMode.Acceleration
            );
        }

        // ==========================
        // STUN
        // ==========================

        if (stun)
        {
            // No permitimos el movimiento controlado
            // por el jugador.
            return;
        }

        // ==========================
        // Movimiento respecto a cámara
        // ==========================

        Vector3 adelante = camara.forward;
        Vector3 derecha = camara.right;

        adelante.y = 0;
        derecha.y = 0;

        adelante.Normalize();
        derecha.Normalize();

        Vector3 direccion =
            adelante * inclinacionX +
            derecha * inclinacionZ;

        if (direccion.sqrMagnitude > 1f)
            direccion.Normalize();

        rb.AddForce(
            direccion * fuerzaGravedad,
            ForceMode.Acceleration
        );

        // ==========================
        // Limitar velocidad
        // ==========================

        Vector3 velocidad = rb.linearVelocity;

        Vector3 horizontal = new Vector3(
            velocidad.x,
            0f,
            velocidad.z
        );

        if (horizontal.magnitude > velocidadMaxima)
        {
            horizontal = horizontal.normalized * velocidadMaxima;

            rb.linearVelocity = new Vector3(
                horizontal.x,
                velocidad.y,
                horizontal.z
            );
        }
    }

    // =====================================================
    // VIENTO
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Wind"))
            return;

        dentroDelViento = true;

        direccionViento = other.transform.right;

        ActivarStun();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Wind"))
            return;

        dentroDelViento = true;

        direccionViento = -other.transform.right;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Wind"))
            return;

        dentroDelViento = false;
    }

    // =====================================================
    // STUN
    // =====================================================

    public void ActivarStun()
    {
        if (stun)
            return;

        stun = true;

        // Eliminar completamente la inercia anterior
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Neutralizar control
        inclinacionX = 0f;
        inclinacionZ = 0f;

        StartCoroutine(FinalizarStun());
    }

    IEnumerator FinalizarStun()
    {
        yield return new WaitForSeconds(duracionStun);

        stun = false;
    }

    // =====================================================
    // RESPAWN
    // =====================================================

    void Respawn()
    {
        GameManager.Instance.ReiniciarBola();
    }
}