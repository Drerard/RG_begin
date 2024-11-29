using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float HealthPoint { get; }

    public void RecieveDamage(float damage);
    public void RecieveHeal(float heal);
    public void Die();

    public bool IsInvulnerable();

    public float GetMaxHP();
}
