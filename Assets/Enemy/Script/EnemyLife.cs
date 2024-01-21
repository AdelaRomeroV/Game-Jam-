using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Script References")]
    public EnemyAI Ai;
    public Animator anim;

    [Header("Life Variables")]
    public int maxlife;
    public int currentLife;

    private void Awake()
    {
        currentLife = maxlife;
    }

    private void Update()
    {
        anim.SetInteger("life", currentLife);

        if (currentLife <= 0)
        {
            Ai.enabled = false;
        }
    }

    public void RecieveDamage()
    {
        currentLife--;
    }
}
