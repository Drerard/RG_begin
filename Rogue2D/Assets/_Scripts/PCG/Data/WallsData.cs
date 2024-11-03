using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class WallsData
{
    [SerializeField] public string wallName = "None";
    [SerializeField] public TileBase Tile;
    [SerializeField] public string existenceMask = "00000000"; //should existence = 1, else = 0
    [SerializeField] public string absenceMask = "11111111"; //should absence = 1, else = 0

    private int? cachedExistenceMaskInt = null;
    private int? cachedAbsenceMaskInt = null;

    public int existenceMaskInt
    {
        get
        {
            if(cachedExistenceMaskInt == null)
            {
                cachedExistenceMaskInt = Convert.ToInt32(existenceMask, 2);
            }
            return cachedExistenceMaskInt.Value;
        }
    }
    public int absenceMaskInt
    {
        get
        {
            if (cachedAbsenceMaskInt == null)
            {
                cachedAbsenceMaskInt = Convert.ToInt32(absenceMask, 2);
            }
            return cachedAbsenceMaskInt.Value;
        }
    }
}
