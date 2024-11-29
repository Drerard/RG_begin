using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour, IAttacker, IDamageable
{
    #region FIELDS
    [SerializeField] private float maxHealthPoint;

    private float healthPoint;
    public float HealthPoint
    {
        get
        {
            return healthPoint;
        }
        private set
        {
            healthPoint = Mathf.Clamp(value, 0, maxHealthPoint);

            if (healthPoint <= 0 && !alreadyDead)
            {
                Die();
            }

            UIEventManager.SendHPChanged(HealthPoint, maxHealthPoint);
        }
    }

    [SerializeField] private float baseDamageMultiplier = 1;
    [SerializeField] private bool isInvulnerable;
    [Space(5)]
    [SerializeField] private Transform weaponFXPoint;


    private bool alreadyDead = false;

    private DamageColorEffect dmgEffect;
    private Player player;
    private Weapon currentWeapon;
    private GameObject currentWeaponFX;
    #endregion

    #region MONO
    private void Awake()
    {
        player = GetComponent<Player>();
        dmgEffect = GetComponentInChildren<DamageColorEffect>();
    }

    private void Start()
    {
        HealthPoint = maxHealthPoint;
        //player.UIData.playerInterface.ChangeHPText(HealthPoint, maxHealthPoint);
    }
    #endregion

    #region MAIN
    public void Dash(float duration)
    {
        StartCoroutine(TempInvulnerable(duration));
    }
    IEnumerator TempInvulnerable(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    public void NormalAttack()
    {
        currentWeaponFX.GetComponent<ParticleSystem>().Play();

        Collider2D[] hits = currentWeapon.NormalAttack(player.LayerData.Damageable, weaponFXPoint.position);
        List<Collider2D> uniqueHits = CollectColliders(hits);

        foreach (var hit in uniqueHits)
        {
            if (hit.TryGetComponent(out Breakable breakable))
            {
                breakable.Break();
            }

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                DealDamage(target);
            }
        }
    }

    public void DealDamage(IDamageable damageableObj)
    {
        float hitDamage = currentWeapon.baseDamage * baseDamageMultiplier;
        damageableObj.RecieveDamage(hitDamage);
    }

    public void RecieveDamage(float damage)
    {
        if (!IsInvulnerable() && !alreadyDead)
        {
            dmgEffect?.StartEffect();
            player.recieveDmgSound.Play();
            HealthPoint -= damage;
            Debug.Log(HealthPoint + "/" + maxHealthPoint + " hp");
        }
    }

    public void RecieveHeal(float heal)
    {
        if(heal >= 0)
        {
            HealthPoint += heal;
        }
    }

    public void Die()
    {
        alreadyDead = true;
        UIEventManager.SendDied();
        GetComponent<Rigidbody2D>().simulated = false;
    }
    #endregion

    #region Reusable Methods
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    public bool IsDead()
    {
        return alreadyDead;
    }

    public float GetMaxHP()
    {
        return maxHealthPoint;
    }

    public float GetNormalAttackCD()
    {
        return currentWeapon.normalAttackCD;
    }

    public void SetCurrentWeapon(Weapon weapon)
    {
        Destroy(currentWeaponFX);
        currentWeapon = weapon;
        currentWeaponFX = Instantiate(weapon.weaponFX, weaponFXPoint);
    }

    private List<Collider2D> CollectColliders(Collider2D[] hits)
    {
        List<int> uniqueHitsKey = new List<int>();
        List<Collider2D> uniqueHits = new List<Collider2D>();

        for (int i = 0; i < hits.Length; i++)
        {
            if (!uniqueHitsKey.Contains(hits[i].gameObject.GetInstanceID()))
            {
                uniqueHitsKey.Add(hits[i].gameObject.GetInstanceID());
                uniqueHits.Add(hits[i]);
            }
        }

        return uniqueHits;
    }
    #endregion
}
