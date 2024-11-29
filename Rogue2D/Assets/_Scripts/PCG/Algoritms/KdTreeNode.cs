using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KdTreeNode
{
    public Vector2Int Point { get; set; }
    public KdTreeNode Left { get; set; }
    public KdTreeNode Right { get; set; }

    public KdTreeNode(Vector2Int point)
    {
        Point = point;
    }
}
