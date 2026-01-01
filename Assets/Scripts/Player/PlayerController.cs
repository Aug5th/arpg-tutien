using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Combat))]
public class PlayerController : MonoBehaviour
{
    [Header("Auto Targeting")]
    public float autoDetectRadius = 4f;
    public float scanInterval = 0.5f; // Optimize: Don't scan every frame

    private Movement movement;
    private Combat combat;
    private float nextScanTime;

    void Awake()
    {
        movement = GetComponent<Movement>();
        combat = GetComponent<Combat>();
    }

    void OnEnable()
    {
        MouseInput.OnRightClick += HandleMoveInput;
        MouseInput.OnRightClickTarget += HandleAttackInput;
    }

    void OnDisable()
    {
        MouseInput.OnRightClick -= HandleMoveInput;
        MouseInput.OnRightClickTarget -= HandleAttackInput;
    }

    void Update()
    {
        // Auto-acquire logic:
        // Only scan if:
        // 1. We are not moving (Idle)
        // 2. We don't have a target yet
        // 3. Scan interval has passed (Performance optimization)
        if (!movement.IsMoving && !combat.HasTarget)
        {
            if (Time.time >= nextScanTime)
            {
                TryAutoAcquireTarget();
                nextScanTime = Time.time + scanInterval;
            }
        }
    }

    private void TryAutoAcquireTarget()
    {
        // Use the LayerMask defined in Combat to find valid targets
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, autoDetectRadius, combat.validTargetLayers);

        if (hits.Length == 0) return;

        IDamagable nearestTarget = null;
        float shortestDistance = float.MaxValue;

        foreach (var col in hits)
        {
            // Get IDamagable component (check both object and parent/rigidbody)
            IDamagable target = col.GetComponent<IDamagable>();
            if (target == null && col.attachedRigidbody != null)
                target = col.attachedRigidbody.GetComponent<IDamagable>();

            if (target != null && !target.IsDead)
            {
                float dist = Vector2.Distance(transform.position, target.Transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearestTarget = target;
                }
            }
        }

        // If a target is found, engage combat just like a right-click
        if (nearestTarget != null)
        {
            HandleAttackInput(nearestTarget);
        }
    }

    private void HandleMoveInput(Vector2 pos)
    {
        combat.CancelTarget();
        movement.MoveTo(pos);
    }

    private void HandleAttackInput(IDamagable target)
    {
        if (target == null || target.IsDead) return;

        combat.SetTarget(target);

        float stopDistance = Mathf.Max(0.5f, combat.attackRange - 0.1f);
        movement.MoveToTarget(target.Transform, stopDistance);
    }

    // Visualize the auto-detect range
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, autoDetectRadius);
    }
}