using UnityEditor;
using UnityEngine;

public class playerMov : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb;

    [Header("Layer")]
    [SerializeField] LayerMask GroundLayer;

    [Header("Movement Variables")]
    [SerializeField] float Speed;
    float Horizontal;
    bool isFacingRight = true;

    [Header("Jump Variables")]
    [SerializeField] float jumpForce;
    bool jumpInput => Input.GetButtonDown("Jump") && onGround();

    [Header("Collisions Variables")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 groundSize;
    bool onGround() => Physics2D.OverlapBox(groundCheck.position, groundSize, 0, GroundLayer);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundSize);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        Flip();
        Jump();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        rb.velocity = new Vector2 (Horizontal * Speed * Time.deltaTime * 100, rb.velocity.y);
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
}
