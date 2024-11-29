using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    public void DealDamage(IDamageable damageableObj);
}
