using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIEventManager : MonoBehaviour
{
    public static UnityEvent<float, float> OnHPChange = new UnityEvent<float, float>();
    public static UnityEvent<float> OnCoinChange = new UnityEvent<float>();
    public static UnityEvent<float> OnKeyChange = new UnityEvent<float>();

    public static UnityEvent OnDeath = new UnityEvent();


    public static void SendHPChanged(float currentHP, float maxHP)
    {
        OnHPChange.Invoke(currentHP, maxHP);
    }

    public static void SendCoinChanged(float count)
    {
        OnCoinChange.Invoke(count);
    }

    public static void SendKeyChanged(float count)
    {
        OnKeyChange.Invoke(count);
    }

    public static void SendDied()
    {
        OnDeath.Invoke();
    }
}
