using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [Header("Health")]
    public int maxHealth = 50;
    public int currentHealth = 0;

    public bool IsDead { get; private set; } = false;
    public Transform Transform => this.transform;

    public event Action<float> OnHealthChanged; // 0..1

    void Awake()
    {
        if (maxHealth <= 0) maxHealth = 1;
        if (currentHealth <= 0) currentHealth = maxHealth;
        OnHealthChanged?.Invoke(1f);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        FloatingText.Create(transform.position, "-" + amount.ToString(), Color.red, 1.0f, 0.35f);
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        float pct = (float)currentHealth / maxHealth;
        OnHealthChanged?.Invoke(pct);

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnHealthChanged?.Invoke(0f);
        // default: destroy object (you can override by listening OnHealthChanged or subclass)
        Destroy(gameObject);
    }
}