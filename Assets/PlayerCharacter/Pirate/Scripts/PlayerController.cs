using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocityX = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.linearVelocityX = 5;
            sr.flipX = false;

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.linearVelocityX = -5;
            sr.flipX = true;
        }
        
    }
}
