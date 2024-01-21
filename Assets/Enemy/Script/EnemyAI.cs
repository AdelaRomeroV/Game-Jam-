using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Script References:")]
    PlayerLife m_PlayerLife;
    EnemyLife m_EnemyLife;
    Rigidbody2D rb;
    GameObject m_Target;
    Animator m_anim;

    [Header("Movement Values:")]
    [SerializeField] float Speed;
    [SerializeField] float flipTime;
    public float currentTime;
    public int Xdirection = 1;
    bool flip = true;

    [Header("Attack Variables")]
    [SerializeField] float AtkRadius;
    [SerializeField] float AtkDelay;
    bool isAttacking;
    bool canAttack = true;

    [Header("Collision Variables")]
    [SerializeField] LayerMask Player;
    [SerializeField] LayerMask Ground;
    [SerializeField] Vector2 wallCheck;
    [SerializeField] Vector2 groundCheck;
    [SerializeField] Transform AtkTransform;
    bool PlayerInArea() => Physics2D.OverlapCircle(AtkTransform.position, AtkRadius, Player);

    private void Awake()
    {
        m_PlayerLife = GameObject.Find("Player").GetComponent<PlayerLife>();
        m_EnemyLife = GetComponent<EnemyLife>();
        rb = GetComponent<Rigidbody2D>();
        m_Target = GameObject.Find("Player");
        m_anim = GetComponent<Animator>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //wallcheck
        Gizmos.DrawWireCube(new Vector2(transform.position.x + 0.5f * Xdirection, transform.position.y), wallCheck);
        //groundcheck
        Gizmos.DrawWireCube(new Vector2(transform.position.x + 0.55f * Xdirection, transform.position.y - 0.5f), groundCheck);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AtkTransform.position, AtkRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, m_Target.transform.position) > 1.5f)
        {
            if (!isAttacking)
            {
                Walk();

                m_anim.SetBool("Walking", true);
            }
        }
        else
        {
            if (canAttack && Attack() != null) StartCoroutine(Attack());

            m_anim.SetBool("Walking", false);

            if (flip)
            {
                Vector2 dir = transform.position - m_Target.transform.position;
                if (dir.x > 0)
                {
                    Xdirection = -1;
                    Vector2 scale = new Vector2(Xdirection, transform.localScale.y);
                    transform.localScale = scale;
                }
                else if (dir.x < 0)
                {
                    Xdirection = 1;
                    Vector2 scale = new Vector2(Xdirection, transform.localScale.y);
                    transform.localScale = scale;
                }
            }
        }
    }

    void Walk()
    {
        currentTime += Time.deltaTime;
        rb.velocity = new Vector2(Xdirection * Speed * Time.deltaTime * 10, rb.velocity.y);

        Collider2D checkWall = Physics2D.OverlapBox(new Vector2(transform.position.x + 0.5f * Xdirection, transform.position.y), wallCheck, 0, Ground);
        Collider2D checkGround = Physics2D.OverlapBox(new Vector2(transform.position.x + 0.5f * Xdirection, transform.position.y - 0.5f), groundCheck, 0, Ground);

        if (currentTime > flipTime || checkWall || !checkGround)
        {
            Xdirection *= -1;
            Vector2 scale = new Vector2(Xdirection, transform.localScale.y);
            transform.localScale = scale;
            currentTime = 0;
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;
        rb.velocity = Vector2.zero;

        m_anim.SetBool("Attacking", isAttacking);
        flip = false;


        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        m_anim.SetBool("Attacking", isAttacking);


        yield return new WaitForSeconds(AtkDelay);

        canAttack = true;
    }

    public void ConfirmAttack()
    {
        if (PlayerInArea() && m_PlayerLife.isAlive)
        {
            Debug.Log("Hitted");

            if (m_PlayerLife.canBeHitted)
                m_PlayerLife.DamagePlayer(transform);
        }
    }

    public void canFlip()
    {
        flip = true;
    }
}
