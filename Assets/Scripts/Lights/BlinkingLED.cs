using UnityEngine;
using System.Collections;

// Automatically adds a Light component if the object doesn't have one
[RequireComponent(typeof(Light))]
public class BlinkingLED : MonoBehaviour
{
    [Header("Blink Settings")]
    [Tooltip("How long the LED stays ON (in seconds)")]
    public float timeOn = 0.5f;

    [Tooltip("How long the LED stays OFF (in seconds)")]
    public float timeOff = 0.5f;

    private Light ledLight;

    void Start()
    {
        // Grab the light component attached to this GameObject
        ledLight = GetComponent<Light>();
        
        // Start the blinking loop
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        // This loop runs infinitely while the GameObject is active
        while (true)
        {
            ledLight.enabled = true;
            yield return new WaitForSeconds(timeOn);
            
            ledLight.enabled = false;
            yield return new WaitForSeconds(timeOff);
        }
    }
}