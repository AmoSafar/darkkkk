using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // مقدار اولیه
        musicSlider.value = SoundManager.Instance.MusicVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;

        // اتصال رویدادها
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMusicVolume(float value)
    {
        SoundManager.Instance.MusicVolume = value;
    }

    private void SetSFXVolume(float value)
    {
        SoundManager.Instance.SFXVolume = value;
    }
}
