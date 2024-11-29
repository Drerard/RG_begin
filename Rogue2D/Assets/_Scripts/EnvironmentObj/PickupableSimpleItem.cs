using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupableSimpleItem : MonoBehaviour
{
    [SerializeField] private float spawnTime = 0;
    [SerializeField] private PlayerInventory.SimpleItems itemType;


    private void Awake()
    {
        GetComponent<Collider2D>().enabled = false;

        DungeonEventManager.OnDangeonGenerate.AddListener(ClearThisObj);
    }

    void Start()
    {
        StartCoroutine(StartInteraction());
    }
    IEnumerator StartInteraction()
    {
        yield return new WaitForSeconds(spawnTime);
        GetComponent<Collider2D>().enabled = true;
    }

    private void PickUp(Player player)
    {
        player.inventory.PutSimpleItem(itemType, 1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Player player = collider.GetComponent<Player>();
        if (player != null)
        {
            PickUp(player);
        }
    }

    private void ClearThisObj()
    {
        Destroy(gameObject);
    }
}
