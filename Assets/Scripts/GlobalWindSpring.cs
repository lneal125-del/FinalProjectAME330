using UnityEngine;

public class GlobalWindSpring : MonoBehaviour
{
    [Range(0f, 1f)] public float input;
    public float stiffness = 20f;
    public float damping = 4f;

    static readonly int WindStrengthID = Shader.PropertyToID("_WindStrength");
    float value, velocity;

    void Update()
    {
        float accel = stiffness * (input - value) - damping * velocity;
        velocity += accel * Time.deltaTime;
        value += velocity * Time.deltaTime;
        Shader.SetGlobalFloat(WindStrengthID, value);
    }
}