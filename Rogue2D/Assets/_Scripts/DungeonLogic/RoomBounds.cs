using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBounds : MonoBehaviour
{
    private Room room;


    public void SetOwnRoom(Room room)
    {
        this.room = room;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Player player))
        {
            GetComponent<Collider2D>().enabled = false;
            room?.SendPlayerEntered(player.transform);
        }
    }
}
