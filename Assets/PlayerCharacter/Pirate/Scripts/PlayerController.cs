using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocityX = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.linearVelocityX = 5;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.linearVelocityX = -5;
        }
        
    }
}
