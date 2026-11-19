using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HorizonLight : MonoBehaviour
{
    [Header("References")]
    public GetArduinoData arduino;        // leave empty to auto-find
    public Light Horizon;                     // drag directional light here

    [Header("Intensity (Lux)")]
    public float minIntensity = 0f;       // at pie2Val = 0
    public float maxIntensity = 30000f;   // at pie2Val = 1023
    public float smoothTime = 0f;         // 0 = exact direct mapping, 0.05 to 0.15 = softens sensor jitter

    [Header("Debug (read only)")]
    public float currentIntensity;

    private HDAdditionalLightData hd;
    private float velocity;

    void Start()
    {
        if (arduino == null)
            arduino = FindFirstObjectByType<GetArduinoData>();

        if (Horizon == null)
        {
            // fall back to the first directional light in the scene
            foreach (Light l in FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (l.type == LightType.Directional) { Horizon = l; break; }
            }
        }

        if (arduino == null || Horizon == null)
        {
            Debug.LogWarning("HorizonLight: missing GetArduinoData or a directional light.");
            enabled = false;
            return;
        }

        hd = Horizon.GetComponent<HDAdditionalLightData>();
        if (hd == null)
        {
            Debug.LogWarning("HorizonLight: the light has no HDAdditionalLightData component.");
            enabled = false;
            return;
        }

        currentIntensity = minIntensity;
    }

    void Update()
    {
        float t = Mathf.Clamp01(arduino.pie2Val / 1023f);
        float target = Mathf.Lerp(minIntensity, maxIntensity, t);

        currentIntensity = smoothTime <= 0f
            ? target
            : Mathf.SmoothDamp(currentIntensity, target, ref velocity, smoothTime);

        hd.SetIntensity(currentIntensity, UnityEngine.Rendering.LightUnit.Lux);
    }
}