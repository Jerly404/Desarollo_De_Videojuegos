using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocityX = 0;
        animator.SetInteger("Estado", 0);
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.linearVelocityX = 5;
            sr.flipX = false;
            animator.SetInteger("Estado", 1);

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.linearVelocityX = -5;
            sr.flipX = true;
            animator.SetInteger("Estado", 1);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocityY = 12.5f;
        }        
    }
}
