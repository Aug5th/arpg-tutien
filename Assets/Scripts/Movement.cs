using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.05f;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool hasTarget = false;

    // new: optional follow target (e.g., enemy)
    private Transform followTarget;
    private float currentStopDistance;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = rb.position;
        currentStopDistance = stopDistance;
    }

    void OnEnable()
    {
        MouseInput.OnRightClick += HandleMouseRightClick;
    }

    void OnDisable()
    {
        MouseInput.OnRightClick -= HandleMouseRightClick;
    }

    void HandleMouseRightClick(Vector2 worldPosition)
    {
        // stop following any target, move to a position
        followTarget = null;
        currentStopDistance = stopDistance;
        targetPosition = worldPosition;
        hasTarget = true;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (!hasTarget)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // if following a transform, update targetPosition from it
        if (followTarget != null)
        {
            // if followTarget was destroyed, Unity's == null will be true
            if (followTarget == null)
            {
                followTarget = null;
                hasTarget = false;
                rb.velocity = Vector2.zero;
                return;
            }

            targetPosition = followTarget.position;
        }

        Vector2 currentPos = rb.position;
        Vector2 dir = targetPosition - currentPos;
        float distance = dir.magnitude;

        // If following a moving target, do NOT clear hasTarget when within stop distance.
        // Instead stop movement but keep following so if the target moves away we'll resume.
        if (followTarget != null)
        {
            if (distance <= currentStopDistance)
            {
                rb.velocity = Vector2.zero;
                return; // keep hasTarget true so following continues
            }
        }
        else
        {
            if (distance <= currentStopDistance)
            {
                hasTarget = false;
                rb.velocity = Vector2.zero;
                return;
            }
        }

        Vector2 moveDir = dir.normalized;
        Vector2 newPos = currentPos + moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    public void MoveTo(Vector2 worldPos)
    {
        followTarget = null;
        currentStopDistance = stopDistance;
        targetPosition = worldPos;
        hasTarget = true;
    }

    // new: follow a transform until within stopDistance (used for attacking enemies)
    public void MoveToTarget(Transform target, float stopDist)
    {
        if (target == null) return;
        followTarget = target;
        currentStopDistance = Mathf.Max(0.01f, stopDist);
        hasTarget = true;
    }

    public void StopMoving()
    {
        followTarget = null;
        hasTarget = false;
        rb.velocity = Vector2.zero;
    }
}