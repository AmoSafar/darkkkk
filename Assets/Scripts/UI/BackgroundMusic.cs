using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    [SerializeField] private AudioClip backgroundMusic;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = SoundManager.Instance.MusicVolume;
        audioSource.Play();
    }

    private void Update()
    {
        // بروزرسانی ولوم موسیقی
        audioSource.volume = SoundManager.Instance.MusicVolume;
    }
}
