using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Slime : DungeonMonster
{
    private enum CharState
    {
        Idle,
        Run,
        Death
    }
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

    [SerializeField] private float baseDamage = 1;
    [SerializeField] private float attackCD = 1;
    [SerializeField] private float seekRadius = 5;
    [SerializeField] private float moveSpeed = 5;
    [Space(5)]
    [SerializeField] private float deathTime = 1;
    [Space(5)]
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private bool onGizmos = false;
    [Header("Sound Effects")]
    [SerializeField] public AudioSource recieveDmgSound;


    private float attackTimer = 0;
    private bool alreadyDead = false;

    private AIPath path;
    private Animator animator;
    private DamageColorEffect dmgEffect;
    private DeathColorEffect deathEffect;
    private Transform target = null;

    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }
    #endregion

    #region MONO
    private void Awake()
    {
        path = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        dmgEffect = GetComponentInChildren<DamageColorEffect>();
        deathEffect = GetComponentInChildren<DeathColorEffect>();
    }

    private void Start()
    {
        path.canMove = false;
        path.maxSpeed = moveSpeed;

        HealthPoint = maxHealthPoint;
        State = CharState.Idle;

        //isActive = true;//DEBUG
    }

    private void Update()
    {
        if (isActive && !alreadyDead)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }

            if (target == null)
            {
                Seek();
            }
            else
            {
                path.destination = target.position;
                path.canMove = true;
                State = CharState.Run;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (attackTimer <= 0)
        {
            if (ConstrainsLayer(Damageable, collision.gameObject.layer))
            {
                if (collision.TryGetComponent(out IDamageable damageable))
                {
                    attackTimer = attackCD;
                    DealDamage(damageable);
                }
            }
        }
    }
    #endregion
    #region MAIN
    private void Seek()
    {
        Vector2 pos = transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, seekRadius, Damageable);
        if (colliders.Length > 0)
        {
            target = colliders[0].transform;
        }
    }

    public override void DealDamage(IDamageable damageableObj)
    {
        if (!alreadyDead)
            damageableObj.RecieveDamage(baseDamage);
    }

    public override void RecieveDamage(float damage)
    {
        dmgEffect?.StartEffect();
        recieveDmgSound.Play();
        HealthPoint -= damage;
        Debug.Log(HealthPoint + "/" + maxHealthPoint + " hp");
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
        path.destination = transform.position;
        path.canMove = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        State = CharState.Death;
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

    public bool ConstrainsLayer(LayerMask layerMask, int layer)
    {
        return (1 << layer & layerMask) != 0;
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (onGizmos)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, seekRadius);
        }
    }
}
