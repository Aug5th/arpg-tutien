using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Health))]
public class Enemy : MonoBehaviour
{
    public int attackDamage = 5;
    Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    // thêm logic AI/đi lại/attack vào đây; health và healthbar đã tách ra
}