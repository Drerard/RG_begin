using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private float deathTime = 0;
    [SerializeField] private float openTime = 0;
    [SerializeField] private LootSpawner goldKey;
    [SerializeField] private List<LootSpawner> itemsToDrop;
    [Header("Animator Parameter Names")]
    [SerializeField] private string breakParameterName = "isOpens";


    private bool alreadyOpen = false;
    public Animator animator { get; private set; }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PutKey()
    {
        itemsToDrop.Add(goldKey);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!alreadyOpen)
        {
            Player player = collider.GetComponent<Player>();
            if (player != null)
            {
                alreadyOpen = true;
                Open();
            }
        }
    }

    private void Open()
    {
        animator.SetBool(breakParameterName, true);

        StartCoroutine(DestroyAndDrop());
    }

    IEnumerator DestroyAndDrop()
    {
        yield return new WaitForSeconds(openTime);
        GetComponent<Collider2D>().enabled = false;
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

        yield return new WaitForSeconds(deathTime >= openTime ? deathTime - openTime : 0);
        Destroy(gameObject, deathTime);
    }

    private void OnDestroy()
    {
        DungeonEventManager.SendNeedReScan(0);
    }
}
