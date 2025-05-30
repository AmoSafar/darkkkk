using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        if (totalHealthBar != null)
            totalHealthBar.fillAmount = 1f;
    }

    private void Update()
    {
        if (enemyHealth == null) return;
        if (enemyHealth.MaxHealth <= 0) return;

        float fillPercent = (float)enemyHealth.CurrentHealth / enemyHealth.MaxHealth;
        fillPercent = Mathf.Clamp01(fillPercent);

        if (currentHealthBar != null)
            currentHealthBar.fillAmount = fillPercent;
    }
}
