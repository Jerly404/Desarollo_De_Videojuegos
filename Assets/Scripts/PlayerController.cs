using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 10f;
    public float fuerzaSalto = 12.5f;

    public GameObject kunaiPrefab;
    public int kunaisDisponibles = 5;

    public Transform groundCheck;
    public LayerMask groundLayer;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    private bool isGrounded = true;
    private string direccion = "Derecha";
    private bool puedeMoverseVerticalMente = false;
    private float defaultGravityScale = 1f;
    private bool puedeSaltar = true;
    private bool puedeLanzarKunai = true;

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

    private Text enemigosMuertosText;
    private int enemigosEliminados = 0;

    void Start()
    {
        Debug.Log("Iniciando PlayerController");

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        enemigosMuertosText = GameObject.Find("EnemigoMuertosText").GetComponent<Text>();

        defaultGravityScale = rb.gravityScale;

        enemigosMuertosText.text = "Enemigos 2.0";
    }

    void Update()
    {
        SetupMoverseHorizontal();
        SetupMoverseVertical();
        SetupSalto();
        SetUpLanzarKunai();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            ZombieController zombie = collision.gameObject.GetComponent<ZombieController>();
            Debug.Log($"Colisión con Enemigo: {zombie.puntosVida}");
            Destroy(collision.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"Trigger con: {other.gameObject.name}");
        if (other.gameObject.name == "Muro")
        {
            puedeMoverseVerticalMente = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Trigger con: {other.gameObject.name}");
        if (other.gameObject.name == "Muro")
        {
            puedeMoverseVerticalMente = false;
            rb.gravityScale = defaultGravityScale;
        }
    }

    void SetupMoverseHorizontal()
    {
        Debug.Log($"isGrounded: {isGrounded}, {rb.linearVelocity.y}");
        if (isGrounded && rb.linearVelocity.y == 0)
        {
            animator.SetInteger("Estado", 0);
        }

        Vector2 velocity = rb.linearVelocity;
        velocity.x = 0;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            velocity.x = velocidad;
            sr.flipX = false;
            direccion = "Derecha";
            if (isGrounded && rb.linearVelocity.y == 0)
                animator.SetInteger("Estado", 1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocity.x = -velocidad;
            sr.flipX = true;
            direccion = "Izquierda";
            if (isGrounded && rb.linearVelocity.y == 0)
                animator.SetInteger("Estado", 1);
        }

        rb.linearVelocity = velocity;
    }

    void SetupMoverseVertical()
    {
        if (!puedeMoverseVerticalMente) return;
        rb.gravityScale = 0;
        Vector2 velocity = rb.linearVelocity;
        velocity.y = 0;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            velocity.y = velocidad;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            velocity.y = -velocidad;
        }

        rb.linearVelocity = velocity;
    }

    void SetupSalto()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            if (rb.linearVelocity.y > 5f)
                animator.SetInteger("Estado", 3);
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }
    }

    void SetUpLanzarKunai()
    {
        if (!puedeLanzarKunai || kunaisDisponibles <= 0) return;

        if (Input.GetKeyUp(KeyCode.K))
        {
            GameObject kunai = Instantiate(kunaiPrefab, transform.position, Quaternion.Euler(0, 0, -90));
            kunai.GetComponent<KunaiController>().SetDirection(direccion);
            kunai.GetComponent<KunaiController>().SetPlayerController(this); // IMPORTANTE
            kunaisDisponibles -= 1;
        }
    }

    public void IncrementarContadorEnemigos()
    {
        enemigosEliminados++;
        enemigosMuertosText.text = $"Enemigos 2.{enemigosEliminados}";
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
