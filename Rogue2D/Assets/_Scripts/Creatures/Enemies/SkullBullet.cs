using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10;
    [SerializeField] private float speed = 20;
    [SerializeField] private LayerMask obstacles = 0;

    private Vector2 normalizedDirection = Vector2.zero;
    private float damage = 0;
    private LayerMask damageable = 0;


    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)normalizedDirection * speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ConstrainsLayer(obstacles, collision.gameObject.layer))
        {
            if (collision.TryGetComponent(out Breakable breakable))
            {
                breakable.Break();
            }
            Destroy(gameObject);
            return;
        }

        if (ConstrainsLayer(damageable, collision.gameObject.layer))
        {
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                damageable.RecieveDamage(damage);
            }
            if (collision.TryGetComponent(out Breakable breakable))
            {
                breakable.Break();
            }
            Destroy(gameObject);
            return;
        }
    }


    public void SetTargetParam(Vector2 direction, LayerMask Damageable, float damage)
    {
        normalizedDirection = direction;
        this.damageable = Damageable;
        this.damage = damage;
    }

    public bool ConstrainsLayer(LayerMask layerMask, int layer)
    {
        return (1 << layer & layerMask) != 0;
    }
}
