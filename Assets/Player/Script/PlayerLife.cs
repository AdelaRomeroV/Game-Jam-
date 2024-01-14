using UnityEngine;

public class PlayerLife : MonoBehaviour
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
        if(currentLife <= 0 )
        {
            Destroy(gameObject);
        }
    }
}
