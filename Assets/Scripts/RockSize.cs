using UnityEngine;

public class RockSize : MonoBehaviour
{
    // Reference to the GetArduinoData script
    public GetArduinoData sensorData;
    // Reference the potentiometer value
    public int pie1Val;

    [Header("Audio")]
    public AudioSource audioSource;   // Audio Source on the rock
    public AudioClip soundClip;       // your .wav file
    public int triggerThreshold = 800; // sensor value that triggers the sound
    public int resetThreshold = 700;   // must drop below this before it can trigger again

    private bool hasTriggered = false;

    void Update()
    {
        if (sensorData != null)
        {
            pie1Val = sensorData.pie1Val;

            // Map the potentiometer value to a scale multiplier (1x to 5x)
            float scaleFactor = Mathf.Lerp(1f, 5f, pie1Val / 1023f);
            transform.localScale = Vector3.one * scaleFactor;

            // Play the sound once when the value crosses the threshold
            if (pie1Val >= triggerThreshold && !hasTriggered)
            {
                audioSource.PlayOneShot(soundClip);
                hasTriggered = true;
            }
            else if (pie1Val < resetThreshold)
            {
                hasTriggered = false;
            }
        }
    }
}
