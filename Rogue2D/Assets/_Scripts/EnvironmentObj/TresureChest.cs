using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TresureChest : MonoBehaviour
{
    [SerializeField] private float openTime = 0;
    [SerializeField] private List<LootSpawner> itemsToDrop;
    [Header("Animator Parameter Names")]
    [SerializeField] private string breakParameterName = "isOpens";


    private bool alreadyOpen = false;
    public Animator animator { get; private set; }


    private void Awake()
    {
        animator = GetComponent<Animator>();
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

        StartCoroutine(Drop());
    }

    IEnumerator Drop()
    {
        yield return new WaitForSeconds(openTime);
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
    }
}
