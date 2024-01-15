using System;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [Header("Components")]
    Animator animator;
    string PLAYER_DEATH = "PlayerDeath";

    [Header("Life Variables")]
    public int maxlife;
    public int currentLife;
    [NonSerialized] public bool isAlive = true; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentLife = maxlife;
    }

    private void Update()
    {
        if(currentLife <= 0 )
        {
            isAlive = false;
            animator.Play(PLAYER_DEATH);
        }
    }
}
