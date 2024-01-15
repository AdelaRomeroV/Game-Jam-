using System;
using UnityEngine;

public class playerMov : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb;
    Animator animator;
    PlayerLife life;


    [Header("Layer")]
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] LayerMask WallLayer;


    [Header("Particulas")]
    [SerializeField] ParticleSystem JumpEffect;


    [Header("Movement Variables")]
    [SerializeField] float Speed;
    [NonSerialized] public float Horizontal;
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

    [SerializeField] public Transform wallCheck;
    [SerializeField] float wallRadius;
    public bool isWalled() => Physics2D.OverlapCircle(wallCheck.position, wallRadius, WallLayer);


    [Header("Animaciones")]
    float horizontal_ANIM;
    string JUMP_ANIM = "PlayerJump";



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundSize);
        Gizmos.DrawWireSphere(wallCheck.position, wallRadius);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        life = GetComponent<PlayerLife>();
    }

    void Animations()
    {
        if(Horizontal != 0) horizontal_ANIM = 1; else horizontal_ANIM = 0;
        animator.SetFloat("Horizontal", horizontal_ANIM);
        animator.SetBool("onGround", onGround());
    }

    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (life.isAlive)
        {
            if (!isWallSliding && !isWallJumping)
            {
                Flip();
            }

            Jump();
            WallSlide();
            WallJump();

            Animations();
        }
    }

    private void FixedUpdate()
    {
        if (life.isAlive)
        {
            Movement();
        }
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
            animator.Play(JUMP_ANIM);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Instantiate(JumpEffect, transform.position, JumpEffect.transform.rotation);
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
            animator.Play(JUMP_ANIM);

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
