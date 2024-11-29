using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject loadingUI;

    private Dungeon dungeon;
    private RoomBasedDungeonGenerator roomBasedDungeonGenerator;
    private BaseLogicGenerator baseLogicGenerator;
    private EnvironmentGenerator environmentGenerator;
    private EnemyGenerator enemyGenerator;


    private void Awake()
    {
        dungeon = GetComponent<Dungeon>();
        roomBasedDungeonGenerator = GetComponentInChildren<RoomBasedDungeonGenerator>();
        baseLogicGenerator = GetComponentInChildren<BaseLogicGenerator>();
        environmentGenerator = GetComponentInChildren<EnvironmentGenerator>();
        enemyGenerator = GetComponentInChildren<EnemyGenerator>();

        DungeonEventManager.OnDangeonGenerate.AddListener(GenerateAndStartDungeon);
    }

    private void Start()
    {
        GenerateAndStartDungeon();
    }

    public void GenerateAndStartDungeon()
    {
        loadingUI.SetActive(true);
        player.SetActive(false);

        ClearAllDungeon();

        roomBasedDungeonGenerator.GenerateDungeon();
        baseLogicGenerator.GenerateLogic();
        environmentGenerator.GenerateEnvironment();
        enemyGenerator.GenerateEnemy();

        DungeonEventManager.SendNeedReScan(1f);

        StartCoroutine(DelayUIAndPlayerSpawn(1.5f));
    }
    IEnumerator DelayUIAndPlayerSpawn(float time)
    {
        player.transform.position = new Vector3(dungeon.startRoom.center.x + 0.5f, dungeon.startRoom.center.y, 0);
        yield return new WaitForSeconds(time);
        player.SetActive(true);
        loadingUI.SetActive(false);
    }

    public void ClearAllDungeon()
    {
        dungeon?.ClearAllObjects();
    }
}
