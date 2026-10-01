using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    private const string VolumeKey = "MasterVolume";
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        // Make sure the slider is configured correctly
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }

    private void Start()
    {
        // Load the saved volume (default is full volume)
        float saved = PlayerPrefs.GetFloat(VolumeKey, 1f);

        slider.SetValueWithoutNotify(saved);
        ApplyVolume(saved);

        // Update the volume whenever the slider moves
        slider.onValueChanged.AddListener(ApplyVolume);
    }

    private void ApplyVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(ApplyVolume);
        PlayerPrefs.Save();
    }
}
