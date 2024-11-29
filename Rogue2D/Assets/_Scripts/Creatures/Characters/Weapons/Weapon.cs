using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    public string weaponName;
    public Sprite sprite;
    [Space(5)]
    public float baseDamage;
    public float attackRange;
    public float normalAttackCD;
    [Space(5)]
    public GameObject weaponFX;

    public abstract Collider2D[] NormalAttack(LayerMask targetLayer, Vector2 position);
}
