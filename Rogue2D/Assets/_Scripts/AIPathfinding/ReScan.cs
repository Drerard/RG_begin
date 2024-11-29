using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ReScan : MonoBehaviour
{
    [SerializeField] private Vector3 scanStartPos = Vector3.zero;
    [SerializeField] private Vector3 scanSize = Vector3.zero;

    private void Awake()
    {
        DungeonEventManager.OnReScanNeeded.AddListener(StartReScan);
    }

    private void StartReScan(float time)
    {
        StartCoroutine(DelayStartReScan(time));
    }
    IEnumerator DelayStartReScan(float time)
    {
        yield return new WaitForSeconds(time);
        AstarPath.active.UpdateGraphs(new Bounds(scanStartPos, scanSize));
    }
}
