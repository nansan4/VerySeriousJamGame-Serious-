using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] collisionClips;
    [SerializeField] private AudioSource collisionAudioSource;

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
}
