using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerLayerData
{
    [SerializeField] public LayerMask Damageable = -1;


    public bool ConstrainsLayer(LayerMask layerMask, int layer)
    {
        return (1 << layer & layerMask) != 0;
    }
}