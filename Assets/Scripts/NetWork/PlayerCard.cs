using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private GameObject visuals;
    [SerializeField] private Sprite[] characters;
    [SerializeField] private Image characterIconImage;
    [SerializeField] private TMP_Text playerNameText;

    public void DisableDisplay()
    {
        visuals.SetActive(false);
    }

    public void UpdateDisplay(CharacterSelectState state)
    {
        if(state.characterID == -1){
            characterIconImage.enabled = false;
        } else {
            characterIconImage.sprite = characters[state.characterID];
            characterIconImage.enabled = true;
            playerNameText.text = "Player " + state.clientID;
        }

        visuals.SetActive(true);
    }
}