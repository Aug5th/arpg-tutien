using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageImpactListener : MonoBehaviour
{
    [Header("VFX Settings")]
    public GameObject hitVfxPrefab;
    
    public float surfaceOffset = 0.1f;

    private Health health;
    private Collider2D ownCollider;
    private Transform playerTransform;

    private void Awake()
    {
        health = GetComponent<Health>();
        ownCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void OnEnable()
    {
        if (health != null) health.OnDamageTaken += PlayHitEffect;
    }

    private void OnDisable()
    {
        if (health != null) health.OnDamageTaken -= PlayHitEffect;
    }

    private void PlayHitEffect()
    {
        PlayHitEffectLogic();
    }

    private void PlayHitEffectLogic()
    {
        if (hitVfxPrefab == null || playerTransform == null) return;

        Vector2 spawnPos = ownCollider.ClosestPoint(playerTransform.position);

        GameObject vfxInstance = Instantiate(hitVfxPrefab, spawnPos, Quaternion.identity);
        Destroy(vfxInstance, 1.0f);
    }
}