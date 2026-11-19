using UnityEngine;
using System.Reflection;

public class SensorSkipWait : MonoBehaviour
{
    [Header("References")]
    public GetArduinoData arduino;        // leave empty to auto-find
    public PathFollower follower;         // drag your player here, or leave empty to auto-find
    public AudioSource audioSource;       // your shared AudioSource

    [Header("Wait")]
    public float defaultWait = 2f;        // wait at each waypoint
    public int skipThreshold = 100;       // pie1Val rising above this ends the wait
    public int rearmThreshold = 50;       // must fall below this before another skip can trigger

    [Header("Sound")]
    public AudioClip skipClip;            // played when the threshold is passed
    [Range(0f, 1f)] public float clipVolume = 1f;
    public bool playOnlyWhileWaiting = false;  // true = sound only plays if the player is actually waiting

    private FieldInfo waitTimerField;
    private bool armed = true;

    void Start()
    {
        if (arduino == null)
            arduino = FindFirstObjectByType<GetArduinoData>();
        if (follower == null)
            follower = FindFirstObjectByType<PathFollower>();

        if (arduino == null || follower == null)
        {
            Debug.LogWarning("SensorSkipWait: missing GetArduinoData or PathFollower in the scene.");
            enabled = false;
            return;
        }

        waitTimerField = typeof(PathFollower).GetField(
            "waitTimer", BindingFlags.Instance | BindingFlags.NonPublic);

        if (waitTimerField == null)
        {
            Debug.LogWarning("SensorSkipWait: couldn't find 'waitTimer' in PathFollower (was it renamed?).");
            enabled = false;
            return;
        }

        if (audioSource == null && skipClip != null)
        {
            Debug.LogWarning("SensorSkipWait: no shared audioSource assigned, creating one on this object.");
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        follower.waitAtWaypoint = defaultWait;
    }

    void Update()
    {
        int v = arduino.pie1Val;

        if (armed && v > skipThreshold)
        {
            float timer = (float)waitTimerField.GetValue(follower);
            bool waiting = timer > 0f;

            if (waiting)
                waitTimerField.SetValue(follower, 0.0001f);   // PathFollower finishes the wait on its next Update

            if (skipClip != null && audioSource != null && (waiting || !playOnlyWhileWaiting))
                audioSource.PlayOneShot(skipClip, clipVolume);

            armed = false;
        }
        else if (!armed && v < rearmThreshold)
        {
            armed = true;
        }
    }
}