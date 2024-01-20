using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyAI : MonoBehaviour
{
    [Header("Script References:")]
    PlayerLife m_PlayerLife;
    EnemyLife m_EnemyLife;
    Rigidbody2D rb;
    GameObject m_Target;

    [Header("Movement Values:")]
    [SerializeField] float Speed;
    [SerializeField] float flipTime;
    public float currentTime;
    public int Xdirection = 1;

    [Header("Attack Variables")]
    [SerializeField] float AtkRadius;
    [SerializeField] float AtkDelay;
    public bool isAttacking;
    public bool canAttack;
    public bool Hitting;

    [Header("Collision Variables")]
    [SerializeField] LayerMask Player;
    [SerializeField] LayerMask Ground;
    [SerializeField] Vector2 wallCheck;
    [SerializeField] Vector2 groundCheck;
    [SerializeField] Transform AtkTransform;

    private void Awake()
    {
        m_PlayerLife = GameObject.Find("Player").GetComponent<PlayerLife>();
        m_EnemyLife = GetComponent<EnemyLife>();
        rb = GetComponent<Rigidbody2D>();
        m_Target = GameObject.Find("Player");

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
    }

    private void Update()
    {
        if(!isAttacking) Walk();

        if(Vector2.Distance(transform.position, m_Target.transform.position) < 2f)
        {
            if (canAttack && Attack() != null) StartCoroutine(Attack());
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
        Debug.Log("Detected");
        isAttacking = true;
        canAttack = false;

        yield return new WaitForSeconds(0.2f);

        Collider2D hit = Physics2D.OverlapCircle(AtkTransform.position, AtkRadius, Player);

        if(hit != null)
        {
            Debug.Log("Hitted");
            m_PlayerLife.currentLife--;
        }
    }
}
