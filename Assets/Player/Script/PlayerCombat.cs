using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    playerMov mov;

    [Header("Layers")]
    [SerializeField] LayerMask EnemyLayer;

    [Header("Check Collision")]
    [SerializeField] Transform AttackCheck;
    [SerializeField] float AttackRadius;
    bool canAttack = true;

    public bool hit;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(AttackCheck.position.x + 0.5f * transform.localScale.x, AttackCheck.position.y), AttackRadius);
    }

    private void Awake()
    {
        mov = GetComponent<playerMov>();
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack && mov.onGround() && !mov.isWallSliding && !mov.isWallJumping)
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(new Vector2(AttackCheck.position.x + 0.5f * transform.localScale.x, AttackCheck.position.y), AttackRadius, EnemyLayer);

        foreach (Collider2D hit in hitEnemies)
        {
            Debug.Log("Enemig");
        }
    }
}
