using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -6);   // relative to target's heading (yaw only)
    public float pitch = 0f;                         // 0 = looking straight forward, small positive = slight downward tilt
    public float positionSmooth = 8f;
    public float rotationSmooth = 8f;

    void LateUpdate()
    {
        if (!target) return;

        // Only use the target's left/right heading, ignore its tilt and roll
        Quaternion yaw = Quaternion.Euler(0, target.eulerAngles.y, 0);

        Vector3 desiredPos = target.position + yaw * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos,
                                          positionSmooth * Time.deltaTime);

        Quaternion desiredRot = yaw * Quaternion.Euler(pitch, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot,
                                              rotationSmooth * Time.deltaTime);
    }
}