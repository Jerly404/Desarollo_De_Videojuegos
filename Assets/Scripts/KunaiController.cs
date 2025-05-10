using UnityEngine;

public class KunaiController : MonoBehaviour
{
    private string direccion = "Derecha";
    private PlayerController playerController;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        Vector2 vel = rb.linearVelocity;
        vel.x = (direccion == "Derecha") ? 15f : -15f;
        rb.linearVelocity = vel;
        sr.flipX = (direccion == "Izquierda");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            // Notificar al PlayerController
            if (playerController != null)
                playerController.IncrementarContadorEnemigos();

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    public void SetDirection(string dir)
    {
        direccion = dir;
    }

    public void SetPlayerController(PlayerController controller)
    {
        playerController = controller;
    }
}
