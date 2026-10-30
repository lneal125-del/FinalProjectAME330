using UnityEngine;

public class TreeSway : MonoBehaviour
{
    // Reference to the GetArduinoData script
    public GetArduinoData sensorData;
    // Reference the potentiometer value
    public int pie2Val;

    [Header("Audio")]
    public AudioSource audioSource;    // Audio Source on the tree
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
            pie2Val = sensorData.pie2Val;

            // Map the potentiometer value to a force (0 to 20)
            float forceAmount = Mathf.Lerp(0f, 20f, pie2Val / 1023f);

            // Apply a constant force along the z-axis
            rb.AddForce(0f, 0f, forceAmount);

            // Play the sound once when the value crosses the threshold
            if (pie2Val >= triggerThreshold && !hasTriggered)
            {
                audioSource.PlayOneShot(soundClip);
                hasTriggered = true;
            }
            else if (pie2Val < resetThreshold)
            {
                hasTriggered = false;
            }
        }
    }
}