using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KdTree
{
    private KdTreeNode root;

    public KdTree(List<Vector2Int> points)
    {
        root = BuildKdTree(points, depth: 0);
    }

    private KdTreeNode BuildKdTree(List<Vector2Int> points, int depth)
    {
        if (points.Count == 0) return null;

        //split axis: 0 = X, 1 = Y
        int axis = depth % 2;

        //sort by selected axis
        points = points.OrderBy(p => axis == 0 ? p.x : p.y).ToList();
        int medianIndex = points.Count / 2;

        KdTreeNode node = new KdTreeNode(points[medianIndex]);
        node.Left = BuildKdTree(points.Take(medianIndex).ToList(), depth + 1);
        node.Right = BuildKdTree(points.Skip(medianIndex + 1).ToList(), depth + 1);

        return node;
    }

    public Vector2Int FindNearestNeighbor(Vector2Int target)
    {
        return FindNearest(root, target, depth: 0).Point;
    }

    private (Vector2Int Point, float Distance) FindNearest(KdTreeNode node, Vector2Int target, int depth)
    {
        if (node == null) return (new Vector2Int(int.MaxValue, int.MaxValue), float.MaxValue);

        int axis = depth % 2;
        float distanceToNode = Vector2Int.Distance(target, node.Point);
        (Vector2Int bestPoint, float bestDistance) = (node.Point, distanceToNode);

        KdTreeNode nextBranch = axis == 0 ? (target.x < node.Point.x ? node.Left : node.Right) : (target.y < node.Point.y ? node.Left : node.Right);
        KdTreeNode oppositeBranch = nextBranch == node.Left ? node.Right : node.Left;

        var (candidatePoint, candidateDistance) = FindNearest(nextBranch, target, depth + 1);
        if (candidateDistance < bestDistance)
        {
            bestPoint = candidatePoint;
            bestDistance = candidateDistance;
        }

        float axisDistance = axis == 0 ? Math.Abs(target.x - node.Point.x) : Math.Abs(target.y - node.Point.y);
        if (axisDistance < bestDistance)
        {
            (candidatePoint, candidateDistance) = FindNearest(oppositeBranch, target, depth + 1);
            if (candidateDistance < bestDistance)
            {
                bestPoint = candidatePoint;
                bestDistance = candidateDistance;
            }
        }

        return (bestPoint, bestDistance);
    }
}
