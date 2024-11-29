using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Breakable : MonoBehaviour
{
    [SerializeField] private float deathTime = 0.5f;
    [SerializeField] private bool instaHitBoxOff = false;
    [SerializeField] private List<LootSpawner> itemsToDrop;
    [Header("Animator Parameter Names")]
    [SerializeField] private string breakParameterName = "isBreaking";
    [Header("Sound Effects")]
    [SerializeField] public AudioSource breakSound;


    private bool alreadyBreak = false;
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //test
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Break();
    //}

    public void Break()
    {
        if (!alreadyBreak)
        {
            alreadyBreak = true;

            breakSound.Play();
            animator.SetBool(breakParameterName, true);
            if (instaHitBoxOff)
            {
                GetComponent<Collider2D>().enabled = false;
                DungeonEventManager.SendNeedReScan(0);
            }

            StartCoroutine(DestroyAndDrop());
        }
    }
    IEnumerator DestroyAndDrop()
    {
        yield return new WaitForSeconds(deathTime);

        foreach (var item in itemsToDrop)
        {
            if (item.spawnChance * 10 >= Random.Range(1, 1001))
            {
                int spawnCount = Random.Range(item.minCount, item.maxCount + 1);
                for (int i = 0; i < spawnCount; i++)
                {
                    int offsetX = (int)(item.rndOffsetSpawnPos.x * 10);
                    int offsetY = (int)(item.rndOffsetSpawnPos.y * 10);
                    Vector3 spawnPos = transform.position + new Vector3(Random.Range(-offsetX, offsetX + 1), Random.Range(-offsetY, offsetY + 1)) / 10;
                    Instantiate(item.prefab, spawnPos, Quaternion.identity);
                }
            }
        }

        Destroy(gameObject, deathTime);
    }

    private void OnDestroy()
    {
        DungeonEventManager.SendNeedReScan(0);
    }
}
