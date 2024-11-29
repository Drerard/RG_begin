using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonEventManager
{
    public static UnityEvent OnDangeonGenerate = new UnityEvent();

    public static UnityEvent OnRoomCleared = new UnityEvent();
    public static UnityEvent OnRoomEntered = new UnityEvent();

    public static UnityEvent<float> OnReScanNeeded = new UnityEvent<float>();


    public static void SendStartGenerateDungeon()
    {
        OnDangeonGenerate.Invoke();
    }

    public static void SendRoomCleared()
    {
        OnRoomCleared.Invoke();
    }
    public static void SendRoomEntered()
    {
        OnRoomEntered.Invoke();
    }

    public static void SendNeedReScan(float time)
    {
        OnReScanNeeded.Invoke(time);
    }
}
