using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSword.asset", menuName = "Weapons/DefaultSword", order = 51)]
public class DefaultSword : Weapon
{
    public override Collider2D[] NormalAttack(LayerMask targetLayer, Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, attackRange, targetLayer);
        return hits;
    }
}
