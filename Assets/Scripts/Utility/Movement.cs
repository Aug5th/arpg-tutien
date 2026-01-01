using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;

    private Rigidbody2D rb;
    private UnitStats stats;
    
    // Movement State
    private Vector3 targetPosition;
    private Transform followTarget;
    private bool hasTarget = false;
    private float currentStopDistance;
    private bool isMoving;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<UnitStats>();
        targetPosition = rb.position;
        
        // Ensure 2D physics settings
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void OnEnable()
    {
        // Only listen to Stats, NOT Input
        if (stats != null)
        {
            stats.OnStatsChanged += UpdateStats;
            UpdateStats();
        }
    }

    void OnDisable()
    {
        if (stats != null)
            stats.OnStatsChanged -= UpdateStats;
    }

    void UpdateStats()
    {
        if (stats == null) return;
        moveSpeed = stats.MoveSpeed;
    }

    void FixedUpdate()
    {
        HandleMovementLogic();
    }

    private void HandleMovementLogic()
    {
        if (!hasTarget) return;

        // 1. Update target position if following a Transform (Dynamic target)
        if (followTarget != null)
        {
            targetPosition = followTarget.position;
        }

        // 2. Calculate Distance
        Vector2 currentPos = rb.position;
        Vector2 targetPos2D = new Vector2(targetPosition.x, targetPosition.y);
        Vector2 dir = targetPos2D - currentPos;
        float distance = dir.magnitude;

        // 3. Check Stop Condition
        if (distance <= currentStopDistance)
        {
            if (isMoving) StopMoving();
            return;
        }

        // 4. Move
        isMoving = true;
        Vector2 moveDir = dir.normalized;
        float step = moveSpeed * Time.fixedDeltaTime;

        if (step >= distance)
        {
            rb.MovePosition(targetPos2D); // Snap to target
        }
        else
        {
            rb.MovePosition(currentPos + moveDir * step);
        }
    }

    // --- Public API for Controllers (Player or AI) ---

    /// <summary>
    /// Moves to a static world position.
    /// </summary>
    public void MoveTo(Vector2 worldPos)
    {
        followTarget = null;
        targetPosition = worldPos;
        currentStopDistance = stopDistance; // Reset to default small distance
        hasTarget = true;
        isMoving = true;
    }

    /// <summary>
    /// Follows a dynamic target (Transform) until within specific range.
    /// </summary>
    public void MoveToTarget(Transform target, float stopDist)
    {
        if (target == null) return;
        
        followTarget = target;
        currentStopDistance = stopDist; // Use custom stop distance (e.g., Attack Range)
        hasTarget = true;
        isMoving = true;
        
        // Quick check: if already close, stop immediately to prevent jitters
        if(Vector2.Distance(rb.position, target.position) <= stopDist)
        {
             isMoving = false;
        }
    }

    public void StopMoving()
    {
        hasTarget = false;
        followTarget = null;
        isMoving = false;
        rb.velocity = Vector2.zero;
    }

    public bool IsMoving => isMoving;
}