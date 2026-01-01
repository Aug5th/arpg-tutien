using UnityEngine;
using System;

[RequireComponent(typeof(UnitStats))]
public class Combat : MonoBehaviour
{
    [Header("Combat Configuration")]
    public int damage = 10;
    public float attackSpeed = 1f;
    public float attackRange = 1.5f;
    
    [Header("Targeting")]
    [Tooltip("Layers that this unit can attack (e.g., Player attacks Enemy, Enemy attacks Player)")]
    public LayerMask validTargetLayers;

    // Current target references
    private IDamagable currentTarget;
    private UnitStats stats;
    private float lastAttackTime = -100f; // Allow immediate first attack

    public bool HasTarget => currentTarget != null;

    // Event hooks for Animation or Audio
    public event Action OnAttackPerformed;

    void Awake()
    {
        stats = GetComponent<UnitStats>();
    }

    void OnEnable()
    {
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
        // Sync stats from UnitStats system
        damage = stats.Attack;
        attackSpeed = Mathf.Max(0.1f, stats.GetStat(StatType.AttackSpeed));
    }

    void Update()
    {
        // Combat Loop: Only active if we have a target
        if (currentTarget != null)
        {
            if (currentTarget.IsDead)
            {
                CancelTarget();
                return;
            }

            // Check distance
            float dist = Vector2.Distance(transform.position, currentTarget.Transform.position);

            // If within range, engage combat
            if (dist <= attackRange)
            {
                // Rotate towards target (Visual only)
                RotateTowards(currentTarget.Transform.position);

                // Handle Attack Cooldown
                if (Time.time >= lastAttackTime + (1f / attackSpeed))
                {
                    PerformAttack();
                    lastAttackTime = Time.time;
                }
            }
            // Note: If out of range, this script does NOTHING. 
            // Movement is handled by an external controller (PlayerController or AI).
        }
    }

    public void SetTarget(IDamagable target)
    {
        if (target == null) return;

        // Validate layer (prevent Friendly Fire)
        if (IsValidTarget(target.Transform.gameObject))
        {
            currentTarget = target;
        }
    }

    public void CancelTarget()
    {
        currentTarget = null;
    }

    private void PerformAttack()
    {
        if (currentTarget == null) return;

        // Apply damage
        currentTarget.TakeDamage(damage);
        
        // Trigger visual/audio events
        OnAttackPerformed?.Invoke();
    }

    private bool IsValidTarget(GameObject obj)
    {
        // Check if object layer is within the valid mask
        return (validTargetLayers.value & (1 << obj.layer)) > 0;
    }

    private void RotateTowards(Vector3 targetPos)
    {
        Vector2 dir = targetPos - transform.position;
        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle -= 90f; // Adjust based on sprite orientation
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 20f);
        }
    }

    void OnDrawGizmos()
    {
        // Visualize Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}