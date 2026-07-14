using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D;
    public Animator animator;
    public float speed;
    public float input;
    public float jumpForce;
    public Transform leftBound;
    public Transform rightBound;
    public float boundOffset = 0.5f;

    // Update is called once per frame
    void Update()
    {
        input = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(input));
        Vector3 currentScale = transform.localScale;

        if (input > 0)
        {
            currentScale.x = Mathf.Abs(currentScale.x);
        }
        else if (input < 0)
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
        }

        transform.localScale = currentScale;

        if (Input.GetButtonDown("Jump"))
        {
            Rigidbody2D.linearVelocity = Vector2.up * jumpForce;
            animator.SetBool("isJumping", true);
        }

        if (Mathf.Abs(Rigidbody2D.linearVelocity.y) < 0.01f)
        {
            animator.SetBool("isJumping", false);
        }
    }

    void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(input * speed, Rigidbody2D.linearVelocity.y);
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftBound.position.x + boundOffset, rightBound.position.x - boundOffset);
        transform.position = pos;
    }
}
