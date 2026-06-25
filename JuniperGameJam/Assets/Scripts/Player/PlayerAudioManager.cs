using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] collisionClips;
    [SerializeField] private AudioSource collisionAudioSource;
    [Header("Package SFX/Vars")]
    [SerializeField] private AudioClip deliveredClip;
    [SerializeField] private AudioClip failedClip;
    [SerializeField] private AudioSource packageAudioSource;
    [Header("Timer SFX/Vars")]
    [SerializeField] private AudioClip timerWarningClip;
    [SerializeField] private AudioSource timerAudioSource;

    public static PlayerAudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayRandomCollisionSFX()
    {
        int randFX = Random.Range(0, collisionClips.Length - 1);
        collisionAudioSource.PlayOneShot(collisionClips[randFX]);
    }

    public void PlayDeliveredSFX()
    {
        packageAudioSource.PlayOneShot(deliveredClip);
    }

    public void PlayFailedSFX()
    {
        packageAudioSource.PlayOneShot(failedClip);
    }

    public void PlayTimerWarningSFX()
    {
        timerAudioSource.PlayOneShot(timerWarningClip);
    }
}
