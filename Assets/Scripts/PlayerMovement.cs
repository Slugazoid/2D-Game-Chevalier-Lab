using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D;
    public float speed;
    public float input;
    public float jumpForce;

    // Update is called once per frame
    void Update()
    {
        input = Input.GetAxisRaw("Horizontal");

        if (Input.GetButton("Jump"))
        {
            Rigidbody2D.linearVelocity = Vector2.up * jumpForce;
        }
    }

    void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(input * speed, Rigidbody2D.linearVelocity.y);
    }
}
