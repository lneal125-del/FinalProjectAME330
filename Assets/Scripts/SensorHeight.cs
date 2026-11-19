using UnityEngine;

public class SensorHeight : MonoBehaviour
{
    [Header("References")]
    public GetArduinoData arduino;        // leave empty to auto-find
    public PathFollower follower;         // drag your player here, or leave empty to auto-find

    [Header("Height")]
    public float minHeight = 0.5f;        // height at pie3Val = 0
    public float maxHeight = 5f;          // height at pie3Val = 1023
    public float smoothTime = 0f;         // 0 = exact direct mapping, 0.05 to 0.15 = softens sensor jitter

    [Header("Debug (read only)")]
    public float currentHeight;

    private float velocity;

    void Start()
    {
        if (arduino == null)
            arduino = FindFirstObjectByType<GetArduinoData>();
        if (follower == null)
            follower = FindFirstObjectByType<PathFollower>();

        if (arduino == null || follower == null)
        {
            Debug.LogWarning("SensorHeight: missing GetArduinoData or PathFollower in the scene.");
            enabled = false;
            return;
        }

        currentHeight = minHeight;
    }

    void Update()
    {
        float t = Mathf.Clamp01(arduino.pie3Val / 1023f);
        float target = Mathf.Lerp(minHeight, maxHeight, t);

        currentHeight = smoothTime <= 0f
            ? target
            : Mathf.SmoothDamp(currentHeight, target, ref velocity, smoothTime);

        follower.heightAboveTerrain = currentHeight;
    }
}
