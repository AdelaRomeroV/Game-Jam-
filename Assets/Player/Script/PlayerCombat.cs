using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    playerMov mov;
    PlayerLife life;
    [SerializeField] Transform scaledObject;

    [Header("Particles")]
    [SerializeField] ParticleSystem DamageParticles;

    [Header("Layers")]
    [SerializeField] LayerMask EnemyLayer;

    [Header("Check Collision")]
    [SerializeField] Transform AttackCheck;
    [SerializeField] float AttackRadius;
    bool canAttack = true;

    public bool hitting;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(AttackCheck.position.x + 0.5f * scaledObject.transform.localScale.x, AttackCheck.position.y), AttackRadius);
    }

    private void Awake()
    {
        mov = GetComponent<playerMov>();
        life = GetComponent<PlayerLife>();
    }

    private void Update()
    {
        if(life.isAlive)
        {
            mov.animator.SetBool("isAttacking", hitting);

            if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack && /*mov.onGround() &&*/ !mov.isWallSliding && !mov.isWallJumping)
            {
                StartCoroutine(Attack());
            }
        } 
    }

    IEnumerator Attack()
    {
        hitting = true;
        mov.canJump = false;
        canAttack = false;

        yield return new WaitForSeconds(0.2f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(new Vector2(AttackCheck.position.x + 0.5f * transform.localScale.x, AttackCheck.position.y), AttackRadius, EnemyLayer);
        foreach (Collider2D hit in hitEnemies)
        {
            ParticleSystem parti = Instantiate(DamageParticles, hit.transform.position, DamageParticles.transform.rotation);
            Vector2 orientacion = mov.scaledObject.transform.localScale;
            parti.transform.localScale = orientacion;

            hit.GetComponent<EnemyLife>().RecieveDamage();
        }

        mov.canJump = true;
        hitting = false;

        yield return new WaitForSeconds(0.2f);
        canAttack = true;
    }
}
