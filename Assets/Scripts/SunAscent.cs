using UnityEngine;

public class SunAscent : MonoBehaviour
{
    // Reference to the GetArduinoData script
    public GetArduinoData sensorData;
    // Reference the potentiometer value
    public int pie3Val;

    [Header("Audio")]
    public AudioSource audioSource;    // Audio Source on the sun
    public AudioClip soundClip;        // your .wav file
    public int triggerThreshold = 800; // sensor value that triggers the sound
    public int resetThreshold = 700;   // must drop below this before it can trigger again

    private Rigidbody rb;
    private bool hasTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (sensorData != null)
        {
            pie3Val = sensorData.pie3Val;

            // Map the potentiometer value to a force (0 to 3)
            float forceAmount = Mathf.Lerp(0f, 3f, pie3Val / 1023f);

            // Apply a constant force along the y-axis
            rb.AddForce(0f, forceAmount, 0f);

            // Play the sound once when the value crosses the threshold
            if (pie3Val >= triggerThreshold && !hasTriggered)
            {
                audioSource.PlayOneShot(soundClip);
                hasTriggered = true;
            }
            else if (pie3Val < resetThreshold)
            {
                hasTriggered = false;
            }
        }
    }
}