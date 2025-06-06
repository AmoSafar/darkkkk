using UnityEngine;
using UnityEngine.UI;

public class HealthBarUIManager : MonoBehaviour
{
    public Slider player1Slider;
    public Slider player2Slider;

    private Health player1Health;
    private Health player2Health;

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            PlayerIdentifier identifier = player.GetComponent<PlayerIdentifier>();
            Health health = player.GetComponent<Health>();

            if (identifier != null && health != null)
            {
                if (identifier.isPlayerOne)
                    player1Health = health;
                else
                    player2Health = health;
            }
        }

        UpdateAllUI();
    }

    void Update()
    {
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        if (player1Health != null)
        {
            player1Slider.value = player1Health.currentHealth / player1Health.maxHealth;
        }

        if (player2Health != null)
        {
            player2Slider.value = player2Health.currentHealth / player2Health.maxHealth;
        }
    }
}
