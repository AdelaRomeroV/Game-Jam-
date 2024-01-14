using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Life Variables")]
    public int maxlife;
    public int currentLife;

    private void Awake()
    {
        currentLife = maxlife;
    }

    private void Update()
    {
        if (currentLife <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void RecieveDamage()
    {
        currentLife--;
    }
}
