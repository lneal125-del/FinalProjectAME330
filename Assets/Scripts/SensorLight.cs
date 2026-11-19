using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(HDAdditionalLightData))]
public class SensorLight : MonoBehaviour
{
    public GetArduinoData arduino;            // leave empty to auto-find
    public int threshold = 300;
    public float onIntensity = 20000000f;
    public UnityEngine.Rendering.LightUnit unit = UnityEngine.Rendering.LightUnit.Lumen;  // match the unit you use in the Inspector
    public float holdTime = 3f;               // stays on this long after dropping below threshold
    public float fadeTime = 2f;               // then fades to 0 over this long

    private HDAdditionalLightData hd;
    private float timeBelow;                  // seconds since the value was last above threshold

    void Start()
    {
        hd = GetComponent<HDAdditionalLightData>();

        if (arduino == null)
            arduino = FindFirstObjectByType<GetArduinoData>();

        if (arduino == null)
        {
            Debug.LogWarning("SensorLight: no GetArduinoData found in the scene.");
            enabled = false;
            return;
        }

        timeBelow = holdTime + fadeTime;      // start fully faded out, so the light begins off
        hd.SetIntensity(0f, unit);
    }

    void Update()
    {
        if (arduino.pie6Val > threshold)
            timeBelow = 0f;                   // above threshold: full on, timer reset
        else
            timeBelow += Time.deltaTime;

        float intensity;
        if (timeBelow <= holdTime)
        {
            intensity = onIntensity;          // on, or still in the 3 second hold
        }
        else
        {
            float t = Mathf.Clamp01((timeBelow - holdTime) / fadeTime);
            intensity = Mathf.Lerp(onIntensity, 0f, t);   // 2 second fade
        }

        hd.SetIntensity(intensity, unit);
    }
}