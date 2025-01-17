using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2.5f;      // Normal movement speed
    public float dashSpeed = 8f;        // Speed during the dash
    public float dashDuration = 0.2f;   // Duration of the dash in seconds
    public float dashCooldown = 1f;     // Cooldown after dashing in seconds

    private Rigidbody2D rb;
    private Vector2 movement;

    public Animator animator;

    private bool isFacingRight = true;
    private bool isDashing = false;
    private bool canMove = true;        // Controls whether the player can move
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;

    // Define the movement boundaries
    public float leftLimit;
    public float rightLimit;
    public float topLimit;
    public float bottomLimit;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Only allow movement input if the player can move
        if (canMove)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            float speed = movement.sqrMagnitude;
            animator.SetFloat("Speed", speed);

            // Check for dash input (Shift key)
            bool dashInput = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);

            if (dashInput && cooldownTimer <= 0f && !isDashing)
            {
                // Start the dash
                isDashing = true;
                dashTimer = dashDuration;
                cooldownTimer = dashCooldown;
            }

            if (isDashing)
            {
                dashTimer -= Time.deltaTime;

                // Stop the dash when the duration is over
                if (dashTimer <= 0f)
                {
                    isDashing = false;
                }
            }

            // Handle cooldown timer
            if (!isDashing && cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }

            // Flip the player when moving left or right
            if (movement.x > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (movement.x < 0 && isFacingRight)
            {
                Flip();
            }
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            // Adjust speed: If dashing, apply the dash speed; otherwise, use normal move speed
            float currentSpeed = isDashing ? dashSpeed : moveSpeed;

            Vector2 targetPosition = rb.position + movement * currentSpeed * Time.fixedDeltaTime;

            // Clamp the player's position within the boundaries
            targetPosition.x = Mathf.Clamp(targetPosition.x, leftLimit, rightLimit);
            targetPosition.y = Mathf.Clamp(targetPosition.y, bottomLimit, topLimit);

            rb.MovePosition(targetPosition);
        }
        else
        {
            // Stop any residual velocity when movement is disabled
            rb.velocity = Vector2.zero;
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    public void StopMovement()
    {
        canMove = false;

        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
    }


    public void AllowMovement()
    {
        canMove = true;
    }
}
