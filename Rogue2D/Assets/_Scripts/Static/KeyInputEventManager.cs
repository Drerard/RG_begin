using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyInputEventManager
{
    public static UnityEvent interactionEvent = new UnityEvent();

    public static UnityEvent escapeEvent = new UnityEvent();

    public static UnityEvent<Player> pickUpEvent = new UnityEvent<Player>();


    public static void StartInteractionEvent()
    {
        interactionEvent.Invoke();
    }

    public static void StartEscapePressedEvent()
    {
        escapeEvent.Invoke();
    }

    public static void StartPickUpEvent(Player player)
    {
        pickUpEvent.Invoke(player);
    }
}
