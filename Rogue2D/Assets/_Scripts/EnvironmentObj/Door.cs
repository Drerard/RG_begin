using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform doorFront;
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;
    [Space(5)]
    [SerializeField] private bool isKeyDoor = false;

    [Header("Animator Parameter Names")]
    [SerializeField] private string openParameterName = "isOpens";
    [SerializeField] private string closeParameterName = "isCloses";


    public Animator animator { get; private set; }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (!isKeyDoor)
        {
            DungeonEventManager.OnRoomCleared.AddListener(OpenDoor);
            DungeonEventManager.OnRoomEntered.AddListener(CloseDoor);
            OpenDoor();
        }
    }

    public void SetSide(Vector2Int direction)
    {
        if (direction.y != 0)
        {
            doorFront.gameObject.SetActive(true);
            doorLeft.gameObject.SetActive(false);
            doorRight.gameObject.SetActive(false);
        }
        else if (direction.x == 1)
        {
            doorFront.gameObject.SetActive(false);
            doorLeft.gameObject.SetActive(true);
            doorRight.gameObject.SetActive(false);
        }
        else
        {
            doorFront.gameObject.SetActive(false);
            doorLeft.gameObject.SetActive(false);
            doorRight.gameObject.SetActive(true);
        }
    }

    private void OpenDoor()
    {
        GetComponent<Collider2D>().enabled = false;

        animator.SetBool(closeParameterName, false);
        animator.SetBool(openParameterName, true);
    }
    private void CloseDoor()
    {
        GetComponent<Collider2D>().enabled = true;

        animator.SetBool(openParameterName, false);
        animator.SetBool(closeParameterName, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isKeyDoor)
        {
            if(collision.TryGetComponent(out Player player))
            {
                if(player.inventory.GetSimpleItemCount(PlayerInventory.SimpleItems.GoldKey) > 0)
                {
                    player.inventory.TakeSimpleItem(PlayerInventory.SimpleItems.GoldKey, 1);
                    OpenDoor();
                }
            }
        }
    }
}
