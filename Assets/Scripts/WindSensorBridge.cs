using UnityEngine;

public class WindSensorBridge : MonoBehaviour
{
    public GetArduinoData arduino;       // leave empty to auto-find
    public GlobalWindSpring windSpring;  // leave empty to auto-find

    void Start()
    {
        if (arduino == null) arduino = FindFirstObjectByType<GetArduinoData>();
        if (windSpring == null) windSpring = FindFirstObjectByType<GlobalWindSpring>();

        if (arduino == null || windSpring == null)
        {
            Debug.LogWarning("WindSensorBridge: missing GetArduinoData or GlobalWindSpring in the scene.");
            enabled = false;
        }
    }

    void Update()
    {
        windSpring.input = Mathf.Clamp01(arduino.pie5Val / 1023f);
    }
}