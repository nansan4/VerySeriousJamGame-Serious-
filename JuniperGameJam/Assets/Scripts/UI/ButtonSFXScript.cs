using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(AudioSource))]

public class ButtonSFXScript : MonoBehaviour, IPointerEnterHandler
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;
    private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickClip);

        audioSource.volume = 0.75f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverClip();
    }

    private void PlayHoverClip()
    {
        if (hoverClip != null)
            audioSource.PlayOneShot(hoverClip);
    }

    private void PlayClickClip()
    {
        if (clickClip != null)
            audioSource.PlayOneShot(clickClip);
    }
}