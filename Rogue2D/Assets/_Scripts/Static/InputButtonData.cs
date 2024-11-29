using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class InputButtonData
{
    public static bool KeyLock = false;

    [field: SerializeField] public static string Horizontal { get; private set; } = "Horizontal";
    [field: SerializeField] public static string Vertical { get; private set; } = "Vertical";
    [field: SerializeField] public static string Fire1 { get; private set; } = "Fire1";
    [field: SerializeField] public static string Fire2 { get; private set; } = "Fire2";
    [field: SerializeField] public static string Fire3 { get; private set; } = "Fire3";
    [field: SerializeField] public static string Jump { get; private set; } = "Jump";
    [field: SerializeField] public static string MouseX { get; private set; } = "Mouse X";
    [field: SerializeField] public static string MouseY { get; private set; } = "Mouse Y";
    [field: SerializeField] public static string MouseScrollWheel { get; private set; } = "Mouse ScrollWheel";
    [field: SerializeField] public static string Submit { get; private set; } = "Submit";
    [field: SerializeField] public static string Cancel { get; private set; } = "Cancel";
    [field: SerializeField] public static string PickUp { get; private set; } = "PickUp";
    
}
