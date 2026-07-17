using UnityEngine;
using System.Collections;

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
    public float attackRate = 0.5f;
    private float nextAttackTime = 0f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false; 
    private float nextDashTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;
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

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackRate;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            StartCoroutine(PerformDash());
            nextDashTime = Time.time + dashCooldown;
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        animator.SetTrigger("Dash");

        float originalGravity = Rigidbody2D.gravityScale;
        Rigidbody2D.gravityScale = 0;
        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        Rigidbody2D.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);
        Rigidbody2D.gravityScale = originalGravity;
        Rigidbody2D.linearVelocity = new Vector2(0f, 0f);
        isDashing = false;
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        Rigidbody2D.linearVelocity = new Vector2(input * speed, Rigidbody2D.linearVelocity.y);
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftBound.position.x + boundOffset, rightBound.position.x - boundOffset);
        transform.position = pos;
    }
}
