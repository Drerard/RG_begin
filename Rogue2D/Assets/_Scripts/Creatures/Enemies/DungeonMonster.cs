using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonMonster : MonoBehaviour, IAttacker, IDamageable
{
    [SerializeField] public LayerMask Damageable = -1;

    public abstract float HealthPoint { get; protected set; }

    [SerializeField] protected bool isActive = false;
    protected Room room;


    public abstract void DealDamage(IDamageable damageableObj);
    public virtual void Die()
    {
        room?.SendEnemyKilled();
    }
    public abstract float GetMaxHP();
    public abstract bool IsInvulnerable();
    public abstract void RecieveDamage(float damage);
    public abstract void RecieveHeal(float heal);

    public void SetOwnRoom(Room room)
    {
        this.room = room;
    }
    public void Activate()
    {
        isActive = true;
    }
}
