using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Automatically moves the player along a list of waypoints.
/// - If a NavMeshAgent is present, it uses the NavMesh (obstacle avoidance, slopes, etc.).
/// - Otherwise it moves the transform directly toward each waypoint.
/// Call StartPath() / StopPath() from other scripts, or tick "Play On Start".
/// </summary>
public class PathFollower : MonoBehaviour
{
    [Header("Path")]
    [Tooltip("Drag empty GameObjects here, in order.")]
    public Transform[] waypoints;
    public bool loop = false;
    public bool playOnStart = true;

    [Header("Movement")]
    public float speed = 4f;
    public float turnSpeed = 8f;
    [Tooltip("How close counts as 'reached' a waypoint.")]
    public float arriveDistance = 0.3f;
    public float waitAtWaypoint = 0f;

    NavMeshAgent agent;
    int index;
    bool running;
    float waitTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = speed;
    }

    void Start()
    {
        if (playOnStart) StartPath();
    }

    public void StartPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        index = 0;
        running = true;
        GoToCurrent();
    }

    public void StopPath()
    {
        running = false;
        if (agent != null && agent.isOnNavMesh) agent.ResetPath();
    }

    void Update()
    {
        if (!running) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f) Advance();
            return;
        }

        Vector3 target = waypoints[index].position;

        if (agent != null)
        {
            // NavMesh mode
            if (!agent.pathPending && agent.remainingDistance <= arriveDistance)
                OnReached();
        }
        else
        {
            // Direct mode
            Vector3 flat = target - transform.position;
            flat.y = 0f;

            if (flat.magnitude <= arriveDistance)
            {
                OnReached();
                return;
            }

            Vector3 dir = flat.normalized;
            transform.position += dir * speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                turnSpeed * Time.deltaTime);
        }
    }

    void OnReached()
    {
        if (waitAtWaypoint > 0f) waitTimer = waitAtWaypoint;
        else Advance();
    }

    void Advance()
    {
        index++;
        if (index >= waypoints.Length)
        {
            if (loop) index = 0;
            else { StopPath(); return; }
        }
        GoToCurrent();
    }

    void GoToCurrent()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.SetDestination(waypoints[index].position);
    }

    // Draw the path in the Scene view
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            int next = (i + 1) % waypoints.Length;
            if ((i < waypoints.Length - 1 || loop) && waypoints[next] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
        }
    }
}
