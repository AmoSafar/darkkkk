using UnityEngine;
using UnityEngine.UI;

public class HealthBar1 : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        totalHealthBar.fillAmount = 1f; // کل نوار پره
    }

    private void Update()
    {
        float fillPercent = playerHealth.currentHealth / playerHealth.maxHealth;
        currentHealthBar.fillAmount = fillPercent;
    }
}
