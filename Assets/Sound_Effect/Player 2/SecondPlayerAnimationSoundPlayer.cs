using UnityEngine;

public class SecondPlayerAnimationSoundPlayer : MonoBehaviour
{
    [Header("References for Second Player")]
    public Animator secondPlayerAnimator;
    public AudioSource secondPlayerAudioSource;

    [Header("Sound Clips for Second Player")]
    public AudioClip secondPlayerJumpClip;
    public AudioClip secondPlayerAttackClip;
    public AudioClip secondPlayerDamageClip; // صدای دمیج

    private int lastStateHash = 0;

    // نام دقیق state های انیمیشن شما در Animator پلیر دوم
    private int jumpHash = Animator.StringToHash("Jump");
    private int attackHash = Animator.StringToHash("Attack");
    private int damageHash = Animator.StringToHash("Damage"); // فرض کردم انیمیشن دمیج به همین اسم هست

    private void Start()
    {
        if (secondPlayerAnimator == null)
            secondPlayerAnimator = GetComponent<Animator>();

        if (secondPlayerAudioSource == null)
            secondPlayerAudioSource = GetComponent<AudioSource>();

        if (secondPlayerAudioSource == null)
            secondPlayerAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        AnimatorStateInfo state = secondPlayerAnimator.GetCurrentAnimatorStateInfo(0);
        int currentHash = state.shortNameHash;

        if (currentHash != lastStateHash)
        {
            if (currentHash == jumpHash)
            {
                PlaySound(secondPlayerJumpClip);
            }
            else if (currentHash == attackHash)
            {
                PlaySound(secondPlayerAttackClip);
            }
            else if (currentHash == damageHash)
            {
                PlaySound(secondPlayerDamageClip);
            }

            lastStateHash = currentHash;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        secondPlayerAudioSource.clip = clip;
        secondPlayerAudioSource.Play();
    }

    // اگر می‌خوای با Animation Event هم صدا بزنی، این متدها رو اضافه کن:
    public void PlayJumpSound() => PlaySound(secondPlayerJumpClip);
    public void PlayAttackSound() => PlaySound(secondPlayerAttackClip);
    public void PlayDamageSound() => PlaySound(secondPlayerDamageClip);
}
