using UnityEngine;
using UnityEngine.AI;

public class SpeedBoost : MonoBehaviour
{
    [Header("References")]
    public GetArduinoData arduino;        // leave empty to auto-find
    public PathFollower follower;         // drag your player here, or leave empty to auto-find

    [Header("Speed")]
    public float baseSpeed = 4f;
    public float maxSpeed = 20f;
    public float boostPerFullPluck = 5f;  // a 1023 pluck adds this much
    public float decayTime = 5f;          // seconds for a full boost to drain to 0

    [Header("Pluck detection")]
    public int triggerThreshold = 40;     // value must exceed this to start tracking a spike
    public int rearmThreshold = 20;       // value must fall below this before another pluck counts
    public int dropMargin = 15;           // how far below the peak counts as "falling"

    [Header("Debug (read only)")]
    public float boost;
    public float lastPluckPeak;

    private NavMeshAgent agent;

    private enum State { Idle, Tracking, WaitingRearm }
    private State state = State.Idle;
    private int peak;

    void Start()
    {
        if (arduino == null)
            arduino = FindFirstObjectByType<GetArduinoData>();
        if (follower == null)
            follower = FindFirstObjectByType<PathFollower>();

        if (arduino == null || follower == null)
        {
            Debug.LogWarning("PluckSpeedBoost: missing GetArduinoData or PathFollower in the scene.");
            enabled = false;
            return;
        }

        agent = follower.GetComponent<NavMeshAgent>();
        ApplySpeed();
    }

    void Update()
    {
        int v = arduino.pie4Val;

        switch (state)
        {
            case State.Idle:
                if (v > triggerThreshold)
                {
                    peak = v;
                    state = State.Tracking;
                }
                break;

            case State.Tracking:
                if (v > peak)
                {
                    peak = v;                       // still rising
                }
                else if (v < peak - dropMargin)     // spike has started falling: commit the pluck
                {
                    AddPluck(peak);
                    state = (v < rearmThreshold) ? State.Idle : State.WaitingRearm;
                }
                break;

            case State.WaitingRearm:
                if (v < rearmThreshold)
                    state = State.Idle;
                break;
        }

        // Linear decay: a full boost drains in decayTime seconds
        float maxBoost = maxSpeed - baseSpeed;
        float decayRate = maxBoost / Mathf.Max(0.01f, decayTime);
        boost = Mathf.Max(0f, boost - decayRate * Time.deltaTime);

        ApplySpeed();
    }

    void AddPluck(int peakValue)
    {
        lastPluckPeak = peakValue;
        float add = Mathf.Clamp01(peakValue / 1023f) * boostPerFullPluck;
        boost = Mathf.Min(boost + add, maxSpeed - baseSpeed);
    }

    void ApplySpeed()
    {
        float s = Mathf.Min(baseSpeed + boost, maxSpeed);
        follower.speed = s;
        if (agent != null) agent.speed = s;   // NavMesh mode doesn't read `speed` after Awake
    }
}