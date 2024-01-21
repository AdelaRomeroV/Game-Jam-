using System;
using System.Collections;
using UnityEngine;

public class playerMov : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public GameObject scaledObject;
    [HideInInspector] public Animator animator;
    [SerializeField] PlayerLife life;
    [SerializeField] GrappleGun gun;
    [SerializeField] GrappleRope rope;


    [Header("Layer")]
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] LayerMask WallLayer;
    [SerializeField] LayerMask LedgeLayer;


    [Header("Particulas")]
    [SerializeField] ParticleSystem JumpEffect;
    [SerializeField] ParticleSystem WallJumpEffect;


    [Header("Movement Variables")]
    [SerializeField] float Speed;
    [NonSerialized] public float Horizontal;
    [NonSerialized] public float vertical;
    [HideInInspector] public bool canMove = true;
    float actualGravity;
    bool isFacingRight = true;
    bool canFlip = true;


    [Header("Jump Variables")]
    [SerializeField] float jumpForce;
    [HideInInspector] public bool canJump = true;
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

    [Header("LedgeGrab Collision Variables")]
    [SerializeField] Vector2 LedgeCollidersSize;
    [NonSerialized] public bool isGrabing;
    [NonSerialized] public bool ledgeRising;
    Vector2 currentSize;
    bool GreenCollider() => Physics2D.OverlapBox(new Vector2(wallCheck.position.x, wallCheck.position.y + greenOffset), LedgeCollidersSize, 0, LedgeLayer);
    [SerializeField] float greenOffset;
    bool redCollider() => Physics2D.OverlapBox(new Vector2(wallCheck.position.x, wallCheck.position.y + redOffset), LedgeCollidersSize, 0, LedgeLayer);
    [SerializeField] float redOffset;


    [Header("Animaciones")]
    float horizontal_ANIM;
    string JUMP_ANIM = "PlayerJump";


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundSize);
        Gizmos.DrawWireSphere(wallCheck.position, wallRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(wallCheck.position.x, wallCheck.position.y + greenOffset), LedgeCollidersSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(wallCheck.position.x, wallCheck.position.y + redOffset), LedgeCollidersSize);
    }

    private void Awake()
    {
        animator = scaledObject.GetComponent<Animator>();
        actualGravity = rb.gravityScale;
        currentSize = LedgeCollidersSize;
    }

    void Animations()
    {
        if(Horizontal != 0) horizontal_ANIM = 1; else horizontal_ANIM = 0;
        animator.SetFloat("Horizontal", horizontal_ANIM);
        animator.SetBool("onGround", onGround());
        animator.SetBool("isWalled", isWallSliding || isGrabing);
        animator.SetBool("isWallJumping", isWallJumping);
    }

    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");


        if (life.isAlive)
        {
            if (!isWallSliding && !isWallJumping)
            {
                Flip();
            }

            Jump();
            WallSlide();
            WallJump();
            LedgeGrab();

            Animations();
        }
    }

    private void FixedUpdate()
    {
        if (life.isAlive && canMove)
        {
            Movement();
        }
    }

    void Movement()
    {
        if (!isWallJumping && canMove)
        {
            rb.velocity = new Vector2(Horizontal * Speed * Time.deltaTime * 100, rb.velocity.y);
        }
    }

    void Flip()
    {
        if (canFlip)
        {
            if(isFacingRight && Horizontal < 0 || !isFacingRight && Horizontal > 0)
            {
                isFacingRight = !isFacingRight;
                Vector2 scale = scaledObject.transform.localScale;
                scale.x *= -1;
                scaledObject.transform.localScale = scale;
            }
        }
    }

    void Jump()
    {
        if (jumpInput && canJump)
        {
            animator.Play(JUMP_ANIM);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Instantiate(JumpEffect, transform.position, JumpEffect.transform.rotation);
        }
    }

    private void WallSlide()
    {
        if (isWalled() && !onGround() && Horizontal != 0 && !isGrabing)
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
                wallJumpingDirection = -scaledObject.transform.localScale.x;

            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0)
        {
            animator.Play(JUMP_ANIM);

            ParticleSystem parti = Instantiate(WallJumpEffect, wallCheck.transform.position, WallJumpEffect.transform.rotation);
            Vector2 orientacion = scaledObject.transform.localScale;
            parti.transform.localScale = orientacion;

            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0;

            if (scaledObject.transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 escala = scaledObject.transform.localScale;
                escala.x *= -1;
                scaledObject.transform.localScale = escala;
            }

            Invoke(nameof(StopWallJump), wallJumpingDuration);

        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    void LedgeGrab()
    {
        if (LedgeCollidersSize == new Vector2(0f, 0f)) isGrabing = false;
        else if (GreenCollider() && !redCollider() && !isGrabing && !onGround() && !gun.grappleRope.enabled) isGrabing = true;

        if (isGrabing)
        {
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
            canMove = false;
            canFlip = false;

            if (vertical > 0 && ledgeRise() != null || Input.GetButtonDown("Jump"))
            {
                StartCoroutine(ledgeRise());
            }
            else if (vertical < 0 && ledgeFall() != null)
            {
                StartCoroutine(ledgeFall());
            }
        }
    }

    private IEnumerator ledgeRise()
    {
        ledgeRising = true;
        isGrabing = false;
        canMove = false;
        canFlip = false;

        rb.gravityScale = actualGravity;

        transform.position = new Vector2(scaledObject.transform.position.x + scaledObject.transform.localScale.x * 1f, scaledObject.transform.position.y + 1f);
        rb.velocity = Vector2.zero;

        yield return null;

        ledgeRising = false;
        canFlip = true;
        canMove = true;
    }

    public IEnumerator ledgeFall()
    {
        canMove = true;
        canFlip = true;
        isGrabing = false;
        LedgeCollidersSize = Vector2.zero;

        rb.gravityScale = actualGravity;

        yield return new WaitForSeconds(1f);

        LedgeCollidersSize = currentSize;
    }
}
