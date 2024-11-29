using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableWeapon : MonoBehaviour
{
    [SerializeField] private float spawnTime = 0;
    [SerializeField] private Weapon weapon;
    [Space(5)]
    [SerializeField] private SpriteRenderer text_sprite;


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
        player.inventory.PutWeapon(weapon);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            text_sprite.enabled = true;
            KeyInputEventManager.pickUpEvent.AddListener(PickUp);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            text_sprite.enabled = false;
            KeyInputEventManager.pickUpEvent.RemoveListener(PickUp);
        }
    }

    private void ClearThisObj()
    {
        Destroy(gameObject);
    }
}
