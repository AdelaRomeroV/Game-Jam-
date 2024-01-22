using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] PlayerLife p_life;

    [Header("Fill Variables")]
    [SerializeField] Image bar;
    [Range(0,100)] float fill;
    float maxValue;

    float target;
    float current;

    private void Awake()
    {
        p_life = GameObject.Find("Player").GetComponent<PlayerLife>();

        fill = 100;
        target = fill;
        current = target;
        maxValue = p_life.maxlife;
    }

    private void Update()
    {
        current = Mathf.Lerp(current, target, Time.deltaTime * 20);
        fill = current;

        bar.fillAmount = fill/100;
    }

    public void SubstractLife()
    {
        target = current - 100 / maxValue;
    }

    public void PlusLife(int times)
    {
        target = current + (100 / maxValue) * times;
    }
}
