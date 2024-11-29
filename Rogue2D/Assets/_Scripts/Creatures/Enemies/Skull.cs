using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : DungeonMonster
{
    #region FIELDS
    [SerializeField] private float maxHealthPoint;

    private float healthPoint;
    public override float HealthPoint
    {
        get
        {
            return healthPoint;
        }
        protected set
        {
            healthPoint = Mathf.Clamp(value, 0, maxHealthPoint);

            if (healthPoint <= 0 && !alreadyDead)
            {
                Die();
            }
        }
    }

    [SerializeField] private GameObject bulletPrefab;
    [Space(5)]
    [SerializeField] private float baseDamage = 1;
    [SerializeField] private float shootCD = 2;
    [SerializeField] private float seekRadius = 5;
    [Space(5)]
    [SerializeField] private float deathTime = 1;
    [Space(5)]
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private bool onGizmos = false;
    [Header("Animator Parameter Names")]
    [SerializeField] private string idleParameterName = "isIdle";
    [SerializeField] private string deathParameterName = "isDeath";
    [Header("Sound Effects")]
    [SerializeField] public AudioSource recieveDmgSound;


    private float shootTimer = 0;
    private bool alreadyDead = false;

    private Animator animator;
    private DamageColorEffect dmgEffect;
    private DeathColorEffect deathEffect;
    private Transform target;
    #endregion

    #region MONO
    private void Awake()
    {
        animator = GetComponent<Animator>();
        dmgEffect = GetComponentInChildren<DamageColorEffect>();
        deathEffect = GetComponentInChildren<DeathColorEffect>();
    }

    private void Start()
    {
        HealthPoint = maxHealthPoint;

        animator.SetBool(idleParameterName, true);
        animator.SetBool(deathParameterName, false);
    }

    private void Update()
    {
        if (isActive && !alreadyDead)
        {
            if (shootTimer <= 0)
            {
                Seek();
            }
            else
            {
                shootTimer -= Time.deltaTime;
            }
        }
    }
    #endregion
    #region MAIN
    private void Seek()
    {
        Vector2 pos = transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, seekRadius, Damageable);
        if(colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if(collider.TryGetComponent(out IDamageable damageable))
                {
                    shootTimer = shootCD;
                    target = collider.transform;
                    DealDamage(damageable);
                    break;
                }
            }
        }
    }

    public override void DealDamage(IDamageable damageableObj)
    {
        if (!alreadyDead)
        {
            Vector2 direction = target.position - transform.position;
            SkullBullet skullBullet = Instantiate(bulletPrefab, transform.position, transform.rotation).GetComponent<SkullBullet>();
            skullBullet.SetTargetParam(direction.normalized, Damageable, baseDamage);
        }
    }

    public override void RecieveDamage(float damage)
    {
        if (!isInvulnerable && !alreadyDead)
        {
            dmgEffect?.StartEffect();
            recieveDmgSound.Play();
            HealthPoint -= damage;
            Debug.Log(HealthPoint + "/" + maxHealthPoint + " hp");
        }
    }

    public override void RecieveHeal(float heal)
    {
        if (heal >= 0)
        {
            HealthPoint += heal;
        }
    }

    public override void Die()
    {
        base.Die();
        alreadyDead = true;
        GetComponent<Collider2D>().enabled = false;

        animator.SetBool(idleParameterName, false);
        animator.SetBool(deathParameterName, true);
        deathEffect?.StartEffect();

        Destroy(gameObject, deathTime);
    }

    public override float GetMaxHP()
    {
        return maxHealthPoint;
    }

    public override bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (onGizmos)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, seekRadius);
            //Gizmos.color = Color.red;
            //Gizmos.DrawLine(A, B);
        }
    }
}
