using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 10f;
    public float fuerzaSalto = 12.5f;

    [Header("Kunai")]
    public GameObject kunaiPrefab;
    public int kunaisDisponibles = 5;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Texto Enemigos")]
    private Text enemigosMuertosText;
    private int enemigosEliminados = 0;

    // Componentes
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    // Estados internos
    private bool isGrounded = true;
    private string direccion = "Derecha";
    private bool puedeMoverseVerticalmente = false;
    private float defaultGravityScale;

    // Parámetros de salto avanzado
    [Header("Parámetros de salto")]
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        defaultGravityScale = rb.gravityScale;

        // Inicializar texto
        enemigosMuertosText = GameObject.Find("EnemigoMuertosText").GetComponent<Text>();
        enemigosEliminados = 0;
        enemigosMuertosText.text = $"Enemigos 2:{enemigosEliminados}";
    }

    void Update()
    {
        SetupMoverseHorizontal();
        SetupMoverseVertical();
        SetupSalto();
        SetupLanzarKunai();
    }

    void SetupMoverseHorizontal()
    {
        // Animaciones
        if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            animator.SetInteger("Estado", 0);

        Vector2 velocity = rb.linearVelocity;
        velocity.x = 0f;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            velocity.x = velocidad;
            sr.flipX = false;
            direccion = "Derecha";
            if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
                animator.SetInteger("Estado", 1);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocity.x = -velocidad;
            sr.flipX = true;
            direccion = "Izquierda";
            if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
                animator.SetInteger("Estado", 1);
        }

        rb.linearVelocity = velocity;
    }

    void SetupMoverseVertical()
    {
        if (!puedeMoverseVerticalmente) return;

        rb.gravityScale = 0f;
        Vector2 velocity = rb.linearVelocity;
        velocity.y = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) velocity.y = velocidad;
        if (Input.GetKey(KeyCode.DownArrow)) velocity.y = -velocidad;

        rb.linearVelocity = velocity;
    }

    void SetupSalto()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
        }

        // Ajuste de gravedad para saltos más suaves
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
    }

    void SetupLanzarKunai()
    {
        if (kunaisDisponibles <= 0) return;

        if (Input.GetKeyUp(KeyCode.K))
        {
            GameObject kunai = Instantiate(kunaiPrefab, transform.position, Quaternion.identity);
            KunaiController kc = kunai.GetComponent<KunaiController>();
            kc.SetDirection(direccion);
            kc.SetPlayerController(this);
            kunaisDisponibles--;
        }
    }

    // Método para actualizar el contador y el texto
    public void IncrementarContadorEnemigos()
    {
        enemigosEliminados++;
        enemigosMuertosText.text = $"Enemigos 2:{enemigosEliminados}";
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            // También contar muertes por colisión cuerpo a cuerpo
            IncrementarContadorEnemigos();
            Destroy(collision.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Muro")
            puedeMoverseVerticalmente = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Muro")
        {
            puedeMoverseVerticalmente = false;
            rb.gravityScale = defaultGravityScale;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
}