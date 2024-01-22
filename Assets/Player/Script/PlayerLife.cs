using System;
using System.Collections;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] playerMov mov;
    [SerializeField] PlayerCombat combat;
    string PLAYER_DEATH = "PlayerDeath";

    [Header("Life Variables")]
    public int maxlife;
    public int currentLife;
    [NonSerialized] public bool isAlive = true;
    public bool canBeHitted = true;
    bool hitted;

    private void Awake()
    {
        currentLife = maxlife;
    }

    private void Update()
    {
        animator.SetBool("Damage", hitted);

        if(currentLife <= 0 )
        {
            isAlive = false;
            mov.rb.velocity = Vector2.zero;
            animator.Play(PLAYER_DEATH);
        }
    }

    public void DamagePlayer(Transform EnemyPos)
    {
        StartCoroutine(RecieveDamage(EnemyPos));
        StartCoroutine(CanGetDamage());
    }

    IEnumerator RecieveDamage(Transform enemyPos)
    {
        Vector2 attackDir = transform.position - enemyPos.position;

        mov.canMove = false;
        combat.canAttack = false;
        hitted = true;

        currentLife--;
        mov.rb.AddForce(new Vector2(attackDir.x, 2) * 5,ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f);

        hitted = false;
        mov.canMove = true;
        combat.canAttack = true;
    }

    IEnumerator CanGetDamage()
    {
        canBeHitted = false;

        yield return new WaitForSeconds(0.8f);

        canBeHitted = true;
    }
}
