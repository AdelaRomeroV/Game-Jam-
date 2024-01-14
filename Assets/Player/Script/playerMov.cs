using System;
using UnityEditor;
using UnityEngine;

public class playerMov : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb;


    [Header("Layer")]
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] LayerMask WallLayer;


    [Header("Movement Variables")]
    [SerializeField] float Speed;
    float Horizontal;
    bool isFacingRight = true;


    [Header("Jump Variables")]
    [SerializeField] float jumpForce;
    bool jumpInput => Input.GetButtonDown("Jump") && onGround();


    [Header("WallSlide Variables")]
    [SerializeField] float wallSlideSpeed;
    [NonSerialized] public bool isWallSliding;


    [Header("WallJump Variables")]
    [SerializeField] float wallJumpingTime = 0.2f;
    [SerializeField] float wallJumpingDuration = 0.4f;
    [SerializeField] Vector2 wallJumpingPower = new Vector2(8, 16);
    [NonSerialized] public bool isWallJumping;
    public float wallJumpingDirection;
    float wallJumpingCounter;


    [Header("Collisions Variables")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 groundSize;
    public bool onGround() => Physics2D.OverlapBox(groundCheck.position, groundSize, 0, GroundLayer);

    [SerializeField] Transform wallCheck;
    [SerializeField] float wallRadius;
    public bool isWalled() => Physics2D.OverlapCircle(wallCheck.position, wallRadius, WallLayer);


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundSize);
        Gizmos.DrawWireSphere(wallCheck.position, wallRadius);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (!isWallSliding && !isWallJumping)
        {
            Flip();
        }

        Jump();
        WallSlide();
        WallJump();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(Horizontal * Speed * Time.deltaTime * 100, rb.velocity.y);
        }
    }

    void Flip()
    {
        if(isFacingRight && Horizontal < 0 || !isFacingRight && Horizontal > 0)
        {
            isFacingRight = !isFacingRight;
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void Jump()
    {
        if (jumpInput)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void WallSlide()
    {
        if (isWalled() && !onGround() && Horizontal != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingCounter = wallJumpingTime;

            if (isWalled())
                wallJumpingDirection = -transform.localScale.x;

            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 escala = transform.localScale;
                escala.x *= -1;
                transform.localScale = escala;
            }

            Invoke(nameof(StopWallJump), wallJumpingDuration);

        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }
}
