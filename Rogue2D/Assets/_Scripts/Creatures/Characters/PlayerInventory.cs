using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory: MonoBehaviour
{
    public enum SimpleItems
    {
        Coin,
        GoldKey
    }

    [SerializeField] private Weapon startWeapon;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    private Dictionary<SimpleItems, int> simpleItemsStorage = new Dictionary<SimpleItems, int>()
    {
        {SimpleItems.Coin, 0},
        {SimpleItems.GoldKey, 0}
    };
    private Weapon currentWeapon;

    private PlayerCombatSystem playerCS;
    private Player player;


    private void Awake()
    {
        playerCS = GetComponent<PlayerCombatSystem>();
        player = GetComponent<Player>();

        currentWeapon = startWeapon;
        weaponSpriteRenderer.sprite = currentWeapon.sprite;

        playerCS.SetCurrentWeapon(currentWeapon);
    }


    public void PutWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        weaponSpriteRenderer.sprite = currentWeapon.sprite;

        playerCS.SetCurrentWeapon(currentWeapon);
    }
    
    public bool PutSimpleItem(SimpleItems item, int count)
    {
        if (count >= 0)
        {
            simpleItemsStorage[item] += count;
            SendChanges(item);
            PickUpSoundEffect(item);
            return true;
        }
        return false;
    }
    public bool TakeSimpleItem(SimpleItems item, int count)
    {
        int itemCount = simpleItemsStorage[item];
        if(itemCount >= count)
        {
            simpleItemsStorage[item] -= count;
            SendChanges(item);
            return true;
        }
        return false;
    }
    public int GetSimpleItemCount(SimpleItems item)
    {
        return simpleItemsStorage[item];
    }

    private void SendChanges(SimpleItems item)
    {
        if (item == SimpleItems.Coin)
            UIEventManager.SendCoinChanged(GetSimpleItemCount(item));
        if (item == SimpleItems.GoldKey)
            UIEventManager.SendKeyChanged(GetSimpleItemCount(item));
    }
    private void PickUpSoundEffect(SimpleItems item)
    {
        if (item == SimpleItems.Coin)
            player.upCoinSound.Play();
        if (item == SimpleItems.GoldKey)
            player.upKeySound.Play();
    }
}
