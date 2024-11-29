using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour, IAttacker
{
    [SerializeField] private float baseDamage = 1;
    [SerializeField] private float damageDealCD = 0.5f;
    [SerializeField] private float upDuration = 1;
    [SerializeField] private float downDuration = 1;
    [Header("Animator Parameter Names")]
    [SerializeField] private string upParameterName = "isUp";
    [SerializeField] private string downParameterName = "isDown";


    private float dmgDealTimer = 0;
    private bool shouldUp = false;
    private bool shouldDown = false;

    private Animator animator;
    private Collider2D hitTrigger;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        hitTrigger = GetComponent<Collider2D>();
    }

    private void Start()
    {
        shouldUp = true;
    }

    private void Update()
    {
        if(dmgDealTimer > 0)
        {
            dmgDealTimer -= Time.deltaTime;
        }

        if (shouldUp)
        {
            shouldUp = false;
            StartCoroutine(SpikeUp());
        }
        if (shouldDown)
        {
            shouldDown = false;
            StartCoroutine(SpikeDown());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (dmgDealTimer <= 0)
        {
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                dmgDealTimer = damageDealCD;
                DealDamage(damageable);
            }
        }
    }

    public void DealDamage(IDamageable damageableObj)
    {
        damageableObj.RecieveDamage(baseDamage);
    }

    private IEnumerator SpikeUp()
    {
        animator.SetBool(upParameterName, true);
        animator.SetBool(downParameterName, false);
        hitTrigger.enabled = true;

        yield return new WaitForSeconds(upDuration);
        shouldDown = true;
    }
    private IEnumerator SpikeDown()
    {
        animator.SetBool(upParameterName, false);
        animator.SetBool(downParameterName, true);
        hitTrigger.enabled = false;

        yield return new WaitForSeconds(downDuration);
        shouldUp = true;
    }
}
