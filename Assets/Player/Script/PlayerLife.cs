using System;
using System.Collections;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] playerMov mov;
    [SerializeField] PlayerCombat combat;
    [SerializeField] LifeBar ui_lifeBar;

    [Header("Life Variables")]
    public int maxlife;
    public int currentLife;
    [NonSerialized] public bool isAlive = true;
    public bool canBeHitted = true;
    bool hitted;

    string PLAYER_DEATH = "PlayerDeath";

    private void Awake()
    {
        ui_lifeBar = GameObject.Find("Fill").GetComponent<LifeBar>();

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
        ui_lifeBar.SubstractLife();

        mov.rb.AddForce(new Vector2(attackDir.x, 2) * 5,ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f);

        hitted = false;
        mov.canMove = true;
        combat.canAttack = true;
    }

    IEnumerator CanGetDamage()
    {
        canBeHitted = false;

        yield return new WaitForSeconds(1);

        canBeHitted = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cure"))
        {
            if(currentLife < maxlife)
            {
                currentLife++;
                ui_lifeBar.PlusLife(1);
                Destroy(collision.gameObject);
            }
        }
    }
}
