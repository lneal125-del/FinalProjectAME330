using UnityEngine;

public class FollowPlayerLight : MonoBehaviour
{
    public Transform target;               // your player character
    public bool copyRotation = false;      // also copy the player's turning
    public float smoothTime = 0f;          // 0 = exact copy, 0.1 to 0.3 = slight lag

    private Vector3 offset;                // gap between light and player at start
    private Quaternion startRotation;
    private Quaternion playerStartRotation;
    private Vector3 velocity;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("FollowPlayerLight: assign the player to 'target'.");
            enabled = false;
            return;
        }

        offset = transform.position - target.position;   // remembers your chosen start spot
        startRotation = transform.rotation;
        playerStartRotation = target.rotation;
    }

    void LateUpdate()
    {
        Vector3 goal = target.position + offset;

        if (smoothTime <= 0f)
            transform.position = goal;
        else
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);

        if (copyRotation)
            transform.rotation = (target.rotation * Quaternion.Inverse(playerStartRotation)) * startRotation;
    }
}