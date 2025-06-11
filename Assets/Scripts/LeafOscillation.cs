using UnityEngine;

[ExecuteAlways]
public class LeafOscillation : MonoBehaviour
{
    [Header("Oscillation Parameters")]
    [Tooltip("Minimum Z rotation (degrees)")]
    public float minZ = -30f;
    [Tooltip("Maximum Z rotation (degrees)")]
    public float maxZ = 30f;
    [Tooltip("Speed of oscillation (cycles per second)")]
    public float speed = 1f;

    private Vector3 pivotWorld;        // world-space point at bottom-left
    private float lastAngle = 0f;      // to track incremental rotation

    void Start()
    {
        // Compute bottom-left corner in world space
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // bounds are in world space already
            Bounds b = sr.bounds;
            float minX = b.center.x - b.extents.x;
            float minY = b.center.y - b.extents.y;
            pivotWorld = new Vector3(minX, minY, b.center.z);
        }
        else
        {
            // fallback: use transform position
            pivotWorld = transform.position;
            Debug.LogWarning("No SpriteRenderer found; using transform.position as pivot.");
        }

        // initialize lastAngle to current
        lastAngle = transform.eulerAngles.z;
    }

    void Update()
    {
        // 0→1→0 curve
        float t = Mathf.PingPong(Time.time * speed, 1f);
        // map to desired angle
        float targetAngle = Mathf.Lerp(minZ, maxZ, t);

        // compute how much to rotate this frame
        float deltaAngle = targetAngle - lastAngle;

        // rotate around pivot in world space
        transform.RotateAround(pivotWorld, Vector3.forward, deltaAngle);

        // store for next frame
        lastAngle = targetAngle;
    }
}