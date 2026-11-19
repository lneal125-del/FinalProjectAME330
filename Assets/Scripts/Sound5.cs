using UnityEngine;

public class Sound5 : MonoBehaviour
{
    [Header("References")]
    public GetArduinoData arduino;        // leave empty to auto-find
    public AudioSource audioSource;       // your shared AudioSource

    [Header("Trigger")]
    public int threshold = 100;           // pie2Val rising above this plays the clip
    public int rearmThreshold = 50;       // must fall below this before it can play again

    [Header("Sound")]
    public AudioClip clip;                // the sound to play
    [Range(0f, 1f)] public float clipVolume = 1f;

    private bool armed = true;

    void Start()
    {
        if (arduino == null)
            arduino = FindFirstObjectByType<GetArduinoData>();

        if (arduino == null || clip == null)
        {
            Debug.LogWarning("Sound5: missing GetArduinoData or clip.");
            enabled = false;
            return;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("Sound5: no shared audioSource assigned, creating one on this object.");
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    void Update()
    {
        int v = arduino.pie5Val;

        if (armed && v > threshold)
        {
            audioSource.PlayOneShot(clip, clipVolume);
            armed = false;
        }
        else if (!armed && v < rearmThreshold)
        {
            armed = true;
        }
    }
}