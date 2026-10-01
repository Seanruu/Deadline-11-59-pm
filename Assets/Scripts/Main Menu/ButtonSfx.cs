using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound; // optional
    [Range(0f, 1f)][SerializeField] private float volume = 1f;

    private AudioSource source;
    private Button button;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f; // 2D sound, so it's not affected by distance

        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Skip the sound if the button is disabled
        if (button != null && !button.interactable) return;

        if (hoverSound != null)
            source.PlayOneShot(hoverSound, volume);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        if (clickSound != null)
            source.PlayOneShot(clickSound, volume);
    }
}