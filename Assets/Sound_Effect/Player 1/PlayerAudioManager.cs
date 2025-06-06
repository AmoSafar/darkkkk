using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip damageClip;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            Debug.LogError("AudioSource component missing!");
    }

    // متدی که در Animation Event برای صدای پرش صدا زده می‌شود
    public void PlayJumpSound()
    {
        PlaySound(jumpClip);
    }

    // متدی که در Animation Event برای صدای تیراندازی صدا زده می‌شود
    public void PlayShootSound()
    {
        PlaySound(shootClip);
    }

    // متدی که در Animation Event برای صدای دمیج صدا زده می‌شود
    public void PlayDamageSound()
    {
        PlaySound(damageClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.PlayOneShot(clip);
    }
}
