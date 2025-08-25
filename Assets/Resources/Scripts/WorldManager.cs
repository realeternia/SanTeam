using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    public bool isDebug = true; //自动判定的，不要改
    public GameObject Units;
    public int gridCellSize = 3; // 每个格子的实际大小(米)

    public int battleIndex = 0; //第几场

    private Dictionary<int, List<Vector2Int>> occupiedGrids = new Dictionary<int, List<Vector2Int>>(); // 所有被占据的格子，键为chess.id

    private bool showDebugCube = false;
    private Dictionary<Vector2Int, GameObject> debugGridCubes = new Dictionary<Vector2Int, GameObject>(); // 格子与调试cube的映射

    private List<Chess> chessList = new List<Chess>(); // 所有棋子
    private int[] killMark = new int[6];
    private int[] deathOrder = new int[6]; // 记录各阵营的死亡顺序，0表示未死亡
    private int deathCount = 0; // 记录已死亡的阵营数量

    bool gameFinish = false;
    bool hasWin;
    private MapConfig mapConfig;
   
    public HeroInfoGroup heroInfoGroup;
    public Button buttonRestart;
    public TMP_Text textRestart;
    public GameObject BattleResultPanel;
    public GameObject BattleResultCellPrefab; // 用于显示玩家战斗结果的单元格预制体
    private List<GameObject> battleResultCells = new List<GameObject>(); // 维护创建的结果单元格列表

    public GameObject HudNode;
    public GameObject BattleTextNode;
    private int idCounter = 100;

    void Start()
    {
        Instance = this;

        buttonRestart.onClick.AddListener(BattleEnd);

        StartCoroutine(DebugBattleBeginCheck());
    }

    IEnumerator DebugBattleBeginCheck()
    {      
        // 延迟2秒
        yield return new WaitForSeconds(2f);
        ConfigManager.Init();             
        if(isDebug)
        {
            BattleBegin();
        }
    }

    public void BattleBegin()
    {
        var newMapId = 1;
        gameFinish = false;
        if (battleIndex >= 5)
            newMapId = UnityEngine.Random.Range(1, 5);
        if (mapConfig == null || newMapId != mapConfig.Mapid)
        {
            // 打印加载耗时
            var startTime = Time.realtimeSinceStartup;
            var mapNode = Resources.Load<GameObject>("Prefabs/Map" + newMapId);
            if (mapConfig != null)
                Destroy(mapConfig.gameObject);

            GameObject cell = Instantiate(mapNode, gameObject.transform.parent);
            mapConfig = cell.GetComponent<MapConfig>();
            var endTime = Time.realtimeSinceStartup;
            Debug.Log("加载地图耗时：" + (endTime - startTime) + "秒");
        }

        battleIndex++;
        killMark = new int[6];
        deathOrder = new int[6];
        deathCount = 0;

        BattleResultPanel.gameObject.SetActive(false);
        SpawnUnitsInRegions();

        // 初始化技能
        foreach (var chess in chessList)
            SkillManager.BattleBegin(chess);
        StartCoroutine(GameUpdate());
    }

    public void BattleEnd()
    {
        // 销毁所有结果单元格
        foreach (GameObject cell in battleResultCells)
        {
            if (cell != null)
            {
                Destroy(cell);
            }
        }
        battleResultCells.Clear();
        
        foreach (Transform child in Units.transform)
        {
            Destroy(child.gameObject);
        }
        chessList.Clear();

        PanelManager.Instance.ShowShop();
        CardShopManager.Instance.ShopBegin();
    }

    private int[] GetMatch()
    {
        // 两两组合搭配方案
        if(battleIndex % 5 == 0)
            return new int[] { 0, 1, 2, 3, 4, 5 };
        else if(battleIndex % 5 == 1)
            return new int[] { 0, 2, 1, 4, 5, 3 };
        else if(battleIndex % 5 == 2)
            return new int[] { 0, 3, 1, 5, 4, 2 };
        else if(battleIndex % 5 == 3)
            return new int[] { 0, 4, 1, 3, 5, 2 };
        else
            return new int[] { 0, 5, 1, 2, 3, 4 };
    }

    private void SpawnUnitsInRegions()
    {
        // 清空之前的单位
        foreach (Transform child in Units.transform)
        {
            Destroy(child.gameObject);
        }
        occupiedGrids.Clear();
        heroInfoGroup.Reset();

        List<Vector2Int> unitGrids = new List<Vector2Int>();
        // 生成墙
        for (int i = 0; i < mapConfig.WallNode.transform.childCount; i++)
        {
            var wallNodeCell = mapConfig.WallNode.transform.GetChild(i);
            // 使用GetOccupiedGrids方法获取需要锁定的格子列表
            List<Vector2Int> requiredGrids = GetOccupiedGrids(wallNodeCell.transform.position, wallNodeCell.GetComponent<Collider>());
            // 锁定新格子

            foreach (var gridPos in requiredGrids)
            {
                unitGrids.Add(gridPos);
                CreateDebugCube(300001, gridPos);
              //  UnityEngine.Debug.Log("Lock " + gridPos + " for wall");
            }
        }
        occupiedGrids[300001] = unitGrids;

        if (!isDebug)
        {
            int[] match = GetMatch();
            // 在RegionSide1生成单位 (阵营1)
            var p = GameManager.Instance.GetPlayer(match[0]);
            for(int i = 0; i < mapConfig.RegionSide1.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, mapConfig.RegionSide1[i].transform.position, 1, p.imgPath);
            // 在RegionSide2生成单位 (阵营2)
            p = GameManager.Instance.GetPlayer(match[1]);
            for(int i = 0; i < mapConfig.RegionSide2.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, mapConfig.RegionSide2[i].transform.position, 2, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[2]);
            for(int i = 0; i < mapConfig.RegionSide3.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, mapConfig.RegionSide3[i].transform.position, 3, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[3]);
            for(int i = 0; i < mapConfig.RegionSide4.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, mapConfig.RegionSide4[i].transform.position, 4, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[4]);
            for(int i = 0; i < mapConfig.RegionSide5.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, mapConfig.RegionSide5[i].transform.position, 5, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[5]);
            for(int i = 0; i < mapConfig.RegionSide6.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, mapConfig.RegionSide6[i].transform.position, 6, p.imgPath);

            var cards = GameManager.Instance.GetPlayer(match[0]).GetBattleCardList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide1.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[0]), mapConfig.RegionHeroSide1[i], cards[i], 1);
            cards = GameManager.Instance.GetPlayer(match[1]).GetBattleCardList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide2.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[1]), mapConfig.RegionHeroSide2[i], cards[i], 2);
            cards = GameManager.Instance.GetPlayer(match[2]).GetBattleCardList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide3.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[2]), mapConfig.RegionHeroSide3[i], cards[i], 3);
            cards = GameManager.Instance.GetPlayer(match[3]).GetBattleCardList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide4.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[3]), mapConfig.RegionHeroSide4[i], cards[i], 4);
            cards = GameManager.Instance.GetPlayer(match[4]).GetBattleCardList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide5.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[4]), mapConfig.RegionHeroSide5[i], cards[i], 5);
            cards = GameManager.Instance.GetPlayer(match[5]).GetBattleCardList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide6.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[5]), mapConfig.RegionHeroSide6[i], cards[i], 6);
        }
        else
        {
            
            SpawnHerosForRegion(GameManager.Instance.GetPlayer(0), mapConfig.RegionHeroSide1[4], new System.Tuple<int, int>(101017, 1), 1);
            //SpawnHerosForRegion(GameManager.Instance.GetPlayer(0), mapConfig.RegionHeroSide1[3], new System.Tuple<int, int>(100001, 1), 1);
         //   SpawnHerosForRegion(GameManager.Instance.GetPlayer(0), mapConfig.RegionHeroSide1[0], new System.Tuple<int, int>(104002, 1), 1);

            SpawnHerosForRegion(GameManager.Instance.GetPlayer(1), mapConfig.RegionHeroSide2[0], new System.Tuple<int, int>(103037, 1), 2);
            SpawnHerosForRegion(GameManager.Instance.GetPlayer(1), mapConfig.RegionHeroSide2[1], new System.Tuple<int, int>(103037, 1), 2);
        //    SpawnHerosForRegion(GameManager.Instance.GetPlayer(1), mapConfig.RegionHeroSide2[1], new System.Tuple<int, int>(101005, 1), 2);
        }


    }

    public Chess SpawnUnitsForRegion(PlayerInfo p, int soldierId, UnityEngine.Vector3 spawnPos, int side, string imgPath)
    {
        var soldierConfig = SoldierConfig.GetConfig(soldierId);
        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/" + soldierConfig.Model);

        // 实例化单位
        GameObject unitInstance = Instantiate(unitPrefab, spawnPos, Quaternion.identity, Units.transform);

        unitInstance.name = $"UnitBing_{side}_{idCounter}";
        unitInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        // 获取并初始化Chess组件
        Chess chessComponent = unitInstance.GetComponent<Chess>();
        if (chessComponent != null)
        {
            chessComponent.id = idCounter;
            chessComponent.isHero = false;
            chessComponent.side = side;
            chessComponent.chessName = imgPath;
            chessComponent.maxHp = soldierConfig.Hp;
            chessComponent.moveSpeed = soldierConfig.MoveSpeed;
            chessComponent.attackRange = soldierConfig.Range;
            chessComponent.attackDamage = soldierConfig.Atk;

            chessComponent.hitEffect = soldierConfig.HitEffect;

            chessComponent.Init(p.pid, p.lineColor);
        }
        else
        {
            Debug.LogError("Chess component not found on UnitBing prefab");
        }
        chessList.Add(chessComponent);

        idCounter++;

        return chessComponent;
    }

    private void SpawnHerosForRegion(PlayerInfo p, GameObject spawnPoint, System.Tuple<int, int> heroData, int side)
    {
        var heroConfig = HeroConfig.GetConfig(heroData.Item1);
        GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/UnitHero");
        if (spawnPoint != null)
        {
            // 实例化单位
            GameObject unitInstance = Instantiate(heroPrefab, spawnPoint.transform.position, Quaternion.identity, Units.transform);
            unitInstance.name = $"Hero_{side}_{idCounter}";
            unitInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            // 获取并初始化Chess组件
            Chess chessComponent = unitInstance.GetComponent<Chess>();
            if (chessComponent != null)
            {
                chessComponent.id = idCounter;
                chessComponent.isHero = true;
                chessComponent.heroId = (int)heroConfig.Id;
                chessComponent.side = side;
                chessComponent.chessName = heroConfig.Icon;
                chessComponent.hitEffect = heroConfig.HitEffect;
                chessComponent.missileSpeed = heroConfig.MissileSpeed;

                if (side <= 2)
                {
                    var heroInfo = heroInfoGroup.AddHero(side, (int)heroConfig.Id, heroData.Item2);
                    chessComponent.heroInfo = heroInfo;
                }
                chessComponent.UpdateLevel(p, heroData.Item2);

                chessComponent.Init(p.pid, p.lineColor);
                // 可以在这里设置其他必要的初始化参数
            }
            else
            {
                Debug.LogError("Chess component not found on UnitBing prefab");
            }
            chessList.Add(chessComponent);
            idCounter++;
        }
    }

    private IEnumerator GameUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        while (!gameFinish)
        {
            yield return new WaitForSeconds(0.05f);
            foreach (var chess in chessList.ToArray())
            {
                if (chess != null && chess.hp > 0)
                    chess.LogicUpdate(0.05f);
            }
        }

        {
            if (hasWin)
                textRestart.text = "你获胜了!!!";
            else
                textRestart.text = "你输了!!!";

            // 销毁之前的结果单元格
            foreach (GameObject cell in battleResultCells)
            {
                if (cell != null)
                {
                    Destroy(cell);
                }
            }
            battleResultCells.Clear();

            // 为每个玩家创建结果单元格
            if (BattleResultCellPrefab != null)
            {
                int[] match = GetMatch();
                // 根据玩家的 mark 进行排序
                var sortedPlayers = match
                    .Select(id => new { Id = id, Mark = GameManager.Instance.GetPlayer(id)?.mark ?? 0 })
                    .OrderByDescending(p => p.Mark)
                    .Select(p => p.Id)
                    .ToArray();
                for (int i = 0; i < sortedPlayers.Length; i++)
                {
                    int playerId = sortedPlayers[i];
                    // 创建结果单元格
                    GameObject cell = Instantiate(BattleResultCellPrefab, BattleResultPanel.transform);

                    // 设置位置，每个单元格垂直偏移50
                    RectTransform rectTransform = cell.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.anchoredPosition = new Vector2(302, -120 - i * 50); // 起始位置向下100，每个单元格间距50
                    }

                    // 获取并设置单元格数据
                    BattleResultCellControl cellControl = cell.GetComponent<BattleResultCellControl>();
                    if (cellControl != null)
                    {
                        var player = GameManager.Instance.GetPlayer(playerId);
                        if (player != null)
                        {
                            // 设置玩家信息
                            cellControl.playerName.text = player.playerNameText.text;

                            cellControl.playerRank.text = (i + 1).ToString(); // 假设按match顺序排列
                            cellControl.playerMark.text = $"<color=white>{player.mark}</color> (<color=green>+{killMark[playerId]}</color>)";


                            cellControl.playerIcon.sprite = player.playerImage.sprite;

                        }
                    }

                    // 添加到维护列表
                    battleResultCells.Add(cell);
                }
            }
            BattleResultPanel.gameObject.SetActive(true);
        }
    }


    public void CreateMissile(Chess sourceChess, Chess targetChess, string effectName)
    {
        // 首先加载导弹预制体
        Missile missilePrefab = Resources.Load<Missile>("Prefabs/MissileCom");
        
        // 实例化导弹
        var missile = Instantiate<Missile>(missilePrefab, sourceChess.transform.position, Quaternion.identity, Units.transform);
        missile.Init(sourceChess, targetChess, effectName);

    }


    // 世界坐标转格子坐标
    public Vector2Int WorldToGridPosition(Vector3 worldPosition, bool FloorToInt)
    {
        int x = 0;
        int z = 0;
        if (FloorToInt)
        {
            x = Mathf.FloorToInt(worldPosition.x / gridCellSize) * gridCellSize;
            z = Mathf.FloorToInt(worldPosition.z / gridCellSize) * gridCellSize;
        }
        else
        {
            x = Mathf.CeilToInt(worldPosition.x / gridCellSize) * gridCellSize;
            z = Mathf.CeilToInt(worldPosition.z / gridCellSize) * gridCellSize;
        }
        return new Vector2Int(x, z);
    }

    // 尝试锁定目标位置的格子
    public bool TryLockGridPositions(Chess unit, Vector3 targetPosition)
    {
        // 获取单位包围盒
        var collider = unit.GetComponent<Collider>();
        if (collider == null)
        {
            UnityEngine.Debug.LogError("Unit missing collider for grid calculation");
            return false;
        }

        // 使用GetOccupiedGrids方法获取需要锁定的格子列表
         List<Vector2Int> requiredGrids = GetOccupiedGrids(targetPosition, collider);
        // UnityEngine.Debug.Log($"id:{unit.id} requiredGrids: Target Position = {targetPosition}, Collider Size = {collider.bounds.size}");
        // string gridPositions = string.Join(", ", requiredGrids);
        // UnityEngine.Debug.Log($"Grids: {gridPositions}");

        // 检查所有格子是否可用
        foreach (var gridPos in requiredGrids)
        {
            foreach (var entry in occupiedGrids)
            {
                if (entry.Key != unit.id)
                {
                    foreach (var occupiedGrid in entry.Value)
                    {
                        if (occupiedGrid.x == gridPos.x && occupiedGrid.y == gridPos.y)
                        {
                         //   UnityEngine.Debug.Log("Grid " + gridPos + " is already occupied by unit: " + entry.Key);
                            return false; // 格子不可用
                        }
                    }
                }
            }
        }

        ReleaseGridPositions(unit);
        // 锁定新格子
        List<Vector2Int> unitGrids = new List<Vector2Int>();
        foreach (var gridPos in requiredGrids)
        {
            unitGrids.Add(gridPos);
            CreateDebugCube(unit.id, gridPos);
         //   UnityEngine.Debug.Log("Lock " + gridPos + " for unit: " + unit.id);
        }

        // 存储单位占据的格子
        occupiedGrids[unit.id] = unitGrids;

        return true;
    }

    // 获取指定位置和碰撞体占据的所有格子
    public List<Vector2Int> GetOccupiedGrids(Vector3 position, Collider collider)
    {
        List<Vector2Int> occupiedGrids = new List<Vector2Int>();
        
        if (collider == null)
        {
            UnityEngine.Debug.LogError("Collider is null for grid calculation");
            return occupiedGrids;
        }

        // 获取碰撞体边界
        Vector3 boundsSize = collider.bounds.size;
        Vector3 halfBounds = boundsSize / 2f;

        // 计算边界的最小和最大世界坐标
        Vector3 minWorldPos = position - halfBounds;
        Vector3 maxWorldPos = position + halfBounds;

        // 将世界坐标转换为格子坐标
        Vector2Int minGridPos = WorldToGridPosition(minWorldPos, true);
        Vector2Int maxGridPos = WorldToGridPosition(maxWorldPos, false);

        // 遍历从最小到最大格子坐标的所有格子
        for (int x = minGridPos.x; x <= maxGridPos.x; x+= gridCellSize)
        {
            for (int z = minGridPos.y; z <= maxGridPos.y; z+= gridCellSize)
            {
                Vector2Int currentGrid = new Vector2Int(x, z);
                occupiedGrids.Add(currentGrid);
            }
        }
        
        return occupiedGrids;
    }

    // 释放指定位置的格子
    // 释放指定单位占据的格子
    public void ReleaseGridPositions(Chess unit)
    {
        if (unit == null)
        {
            UnityEngine.Debug.LogError("Unit is null for grid release");
            return;
        }

        // 检查单位是否有占据的格子
        if (occupiedGrids.ContainsKey(unit.id))
        {
            // 删除该单位占据的所有格子的调试cube
            foreach (var gridPos in occupiedGrids[unit.id])
            {
                DestroyDebugCube(gridPos);
            }
            occupiedGrids[unit.id].Clear();
          //  UnityEngine.Debug.Log("Released all grids for unit: " + unit.id);
        }
    }

    // 创建调试用的cube
    private void CreateDebugCube(int oid, Vector2Int gridPos)
    {
        if(!showDebugCube)
            return;

        if (debugGridCubes.ContainsKey(gridPos))
            return; // 已存在则不再创建

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(gridPos.x, 0.5f, gridPos.y);
        cube.transform.localScale = new Vector3(gridCellSize * 0.9f, 1f, gridCellSize * 0.9f);
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.name = "GridCube_" + oid;
        cube.transform.parent = Units.transform;
        cube.transform.localPosition += new Vector3(0, 10f, 0);

        debugGridCubes[gridPos] = cube;
    }

    // 销毁调试用的cube
    private void DestroyDebugCube(Vector2Int gridPos)
    {
        if(!showDebugCube)
            return;

        if (debugGridCubes.TryGetValue(gridPos, out GameObject cube))
        {
            Destroy(cube);
            debugGridCubes.Remove(gridPos);
        }
    }

    public bool IsEnemy(int a, int b)
    {
        if (mapConfig.TeamMode == 1)
        {
            // 阵营1、3、4为一个阵营，阵营2、5、6为另一个阵营
            bool isTeam1 = a == 1 || a == 3 || a == 5;
            bool isTeam2 = a == 2 || a == 4 || a == 6;
            bool targetIsTeam1 = b == 1 || b == 3 || b == 5;
            bool targetIsTeam2 = b == 2 || b == 4 || b == 6;

            // 不同阵营之间是敌人
            return (isTeam1 && targetIsTeam2) || (isTeam2 && targetIsTeam1);
        }
        else if (mapConfig.TeamMode == 0)
        {
            return a != b && (a + 1) / 2 == (b + 1) / 2;
        }
        else
        {
            return a != b;
        }
    }

    public void OnUnitDying(Chess dieUnit, int killerPlayerId)
    {
        if(killerPlayerId >= 0 && dieUnit.isHero)
            killMark[killerPlayerId]++;
        // 从chessList中移除死亡单位
        chessList.Remove(dieUnit);

        gameFinish = false;
        hasWin = false;
        if (mapConfig.TeamMode == 0)
        {
            // 检查所有阵营是否还有存活单位
            // 创建一个数组来统计每个阵营是否有存活单位，数组索引对应阵营编号减1
            bool[] sideHasUnits = new bool[6];
            int aliveSideCount = 0;

            foreach (var chessComponent in chessList)
            {
                if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
                {
                    int sideIndex = chessComponent.side - 1;
                    if (sideIndex >= 0 && sideIndex < sideHasUnits.Length)
                    {
                        if (!sideHasUnits[sideIndex])
                        {
                            sideHasUnits[sideIndex] = true;
                            aliveSideCount++;
                        }
                    }
                }
            }

            UnityEngine.Debug.Log($"id:{dieUnit.id} dieUnit.side:{dieUnit.side} 存活阵营数:{aliveSideCount}");
            // 如果只剩一个阵营有存活单位，显示重启按钮
            if (aliveSideCount == 3)
            {
                int[] match = GetMatch();
                for (int i = 0; i < match.Length; i++)
                {
                    if (sideHasUnits[i])
                        killMark[match[i]] = 10;
                    else
                        killMark[match[i]] = Math.Min(5, killMark[match[i]]);

                    GameManager.Instance.GetPlayer(match[i]).onBattleResult(sideHasUnits[i], killMark[match[i]]);
                }
                gameFinish = true;
                hasWin = sideHasUnits[0];
            }
        }
        else if (mapConfig.TeamMode == 1)
        {
            // 团队模式逻辑：检查两个阵营是否还有存活单位
            bool team1HasUnits = false; // 阵营1、3、4
            bool team2HasUnits = false; // 阵营2、5、6

            foreach (var chessComponent in chessList)
            {
                if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
                {
                    if (chessComponent.side == 1 || chessComponent.side == 3 || chessComponent.side == 5)
                    {
                        team1HasUnits = true;
                    }
                    else if (chessComponent.side == 2 || chessComponent.side == 4 || chessComponent.side == 6)
                    {
                        team2HasUnits = true;
                    }
                }
            }

            UnityEngine.Debug.Log($"id:{dieUnit.id} dieUnit.side:{dieUnit.side} 团队1存活:{team1HasUnits} 团队2存活:{team2HasUnits}");
            // 如果一个阵营被全灭，另一个阵营获胜
            if (!team1HasUnits || !team2HasUnits)
            {
                // 通知玩家战斗结果
                int[] match = GetMatch();
                for (int i = 0; i < match.Length; i++)
                {
                    int playerSide = i + 1; // 假设match索引对应阵营1-6
                    bool isTeam1 = playerSide == 1 || playerSide == 3 || playerSide == 5;
                    bool isWinner = (isTeam1 && team1HasUnits) || (!isTeam1 && team2HasUnits);

                    if (isWinner)
                        killMark[match[i]] = 10;
                    else
                        killMark[match[i]] = Math.Min(5, killMark[match[i]]);

                    GameManager.Instance.GetPlayer(match[i]).onBattleResult(isWinner, killMark[match[i]]);
                }
                gameFinish = true;
                hasWin = team1HasUnits;
            }
        }
       else if (mapConfig.TeamMode == 2)
        {
            // 检查所有阵营是否还有存活单位
            // 创建一个数组来统计每个阵营是否有存活单位，数组索引对应阵营编号减1
            bool[] sideHasUnits = new bool[6];
            int aliveSideCount = 0;

            foreach (var chessComponent in chessList)
            {
                if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
                {
                    int sideIndex = chessComponent.side - 1;
                    if (sideIndex >= 0 && sideIndex < sideHasUnits.Length)
                    {
                        if (!sideHasUnits[sideIndex])
                        {
                            sideHasUnits[sideIndex] = true;
                            aliveSideCount++;
                        }
                    }
                }
            }

            // 记录死亡顺序
            int dieSideIndex = dieUnit.side - 1;
            if (dieSideIndex >= 0 && dieSideIndex < 6)
            {
                // 检查该阵营是否刚刚被消灭
                if (sideHasUnits[dieSideIndex] == false && deathOrder[dieSideIndex] == 0)
                {
                    deathCount++;

                    deathOrder[dieSideIndex] = deathCount;
                    UnityEngine.Debug.Log($"阵营 {dieUnit.side} 被消灭，死亡顺序: {deathCount}");
                }
            }

            UnityEngine.Debug.Log($"id:{dieUnit.id} dieUnit.side:{dieUnit.side} 存活阵营数:{aliveSideCount}");

            // 如果只剩一个阵营有存活单位，计算分数并结束游戏
            if (aliveSideCount == 1)
            {
                int winnerSide = -1;
                for (int i = 0; i < sideHasUnits.Length; i++)
                {
                    if (sideHasUnits[i])
                    {
                        winnerSide = i + 1; // 阵营编号从1开始
                        break;
                    }
                }

                // 计算分数：存活的阵营得1分，死亡顺序越晚分数越高
                int[] scores = new int[6];
                for (int i = 0; i < scores.Length; i++)
                {
                    if (i + 1 == winnerSide)
                        scores[i] = 10; // 胜利者得1分
                    else if (deathOrder[i] == 5)
                        scores[i] = 8;
                    else if (deathOrder[i] == 4)
                        scores[i] = 6;
                    else if (deathOrder[i] == 3)
                        scores[i] = 4;
                    else if (deathOrder[i] == 2)
                        scores[i] = 3;
                    else if (deathOrder[i] == 1)
                        scores[i] = 2;
                }

                // 通知玩家战斗结果
                int[] match = GetMatch();
                for (int i = 0; i < match.Length; i++)
                {
                    int playerId = match[i];
                    int playerSide = i + 1; // 假设match索引对应阵营1-6
                    bool isWinner = (playerSide == winnerSide);

                    killMark[playerId] = scores[i];
                    GameManager.Instance.GetPlayer(playerId).onBattleResult(isWinner, killMark[playerId]);
                }

                gameFinish = true;
                hasWin = sideHasUnits[0]; // 假设阵营1是玩家阵营
            }
        }
    }

    public bool CheckInRange(Vector3 pos1, Vector3 pos2, float range)
    {
        Vector2Int pos1a = WorldManager.Instance.WorldToGridPosition(pos1, true);
        Vector2Int pos2a = WorldManager.Instance.WorldToGridPosition(pos2, true);

        return Vector2Int.Distance(pos1a, pos2a) <= range;
    }


    public List<Chess> GetUnitsInRange(Vector3 wPos, float range, int mySide, bool findEnemy)
    {
        Vector2Int center = WorldManager.Instance.WorldToGridPosition(wPos, true);
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                Vector2Int chessPos = WorldToGridPosition(chessComponent.transform.position, true);
                if (Vector2Int.Distance(center, chessPos) <= range || range == 0)
                {
                    if(findEnemy)
                    {
                        if(IsEnemy(chessComponent.side, mySide))
                            unitsInRange.Add(chessComponent);
                    }
                    else
                    {
                        if(!IsEnemy(chessComponent.side, mySide)) 
                            unitsInRange.Add(chessComponent);
                    }
                }
            }
        }

        return unitsInRange;
    }

    public void RandomSelect(List<Chess> unitsInRange, int limit)
    {
        if(unitsInRange.Count > limit)
        {
            System.Random random = new System.Random();
            while (unitsInRange.Count > limit)
            {
                int indexToRemove = random.Next(0, unitsInRange.Count);
                unitsInRange.RemoveAt(indexToRemove);
            }
        }
    }

    public List<Chess> GetUnitsMySide(Vector3 wPos, float range, int mySide)
    {
        Vector2Int center = WorldManager.Instance.WorldToGridPosition(wPos, true);
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                Vector2Int chessPos = WorldToGridPosition(chessComponent.transform.position, true);
                if (range == 0 || Vector2Int.Distance(center, chessPos) <= range)
                {
                    if(chessComponent.side == mySide)
                        unitsInRange.Add(chessComponent);
                }
            }
        }
        return unitsInRange;
    }

    public void AddBattleText(string text, UnityEngine.Vector3 worldPos, UnityEngine.Vector2 speed, Color color, int duration)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/BattleTxt");
        var battleText = Instantiate(prefab, BattleTextNode.transform);
        
        // 将世界坐标转换为屏幕坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        
        battleText.transform.position = screenPos;

        var textCtr = battleText.transform.GetChild(0).GetComponent<TMP_Text>();
        textCtr.color = color;
        textCtr.text = text;
        Destroy(battleText, duration);

        //如果speed不为0，开一个协程移动文本
        if(speed != UnityEngine.Vector2.zero)
        {
            StartCoroutine(MoveText(battleText, speed, duration));
        }
    }

    // 战斗文本移动协程
    private IEnumerator MoveText(GameObject battleText, UnityEngine.Vector2 speed, int duration)
    {
        //获得屏幕分辨率
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        UnityEngine.Debug.Log($"screenWidth:{screenWidth} screenHeight:{screenHeight}");

        // 假设设计分辨率为 1920x1080，可根据实际项目修改
        const float designWidth = 2048f;
        const float designHeight = 1536f;
        // 根据当前屏幕分辨率计算缩放比例
        float scaleX = (float)screenWidth / designWidth;
        float scaleY = (float)screenHeight / designHeight;

        float startTime = Time.time;
        float endTime = startTime + duration;
        RectTransform rectTransform = battleText.GetComponent<RectTransform>();
        var lastTime = Time.time;

        while (Time.time < endTime)
        {
            // 考虑分辨率和缩放因素计算移动距离
            var timeDiff = Time.time - lastTime;
            lastTime = Time.time;

            float moveX = speed.x * timeDiff * scaleX;
            float moveY = speed.y * timeDiff * scaleY;

            if (rectTransform == null)
            {
                Destroy(battleText);
                yield break;
            }

            // 更新位置
            rectTransform.Translate(new Vector3(moveX, moveY, 0));

            yield return new WaitForSeconds(0.05f); // 使用 yield return null 在下一帧继续执行，保证流畅移动

        }
    }


    // 管理器销毁时释放所有格子
    private void OnDestroy()
    {
        occupiedGrids.Clear();

        // 销毁所有调试cube
        foreach (var cube in debugGridCubes.Values)
        {
            Destroy(cube);
        }
        debugGridCubes.Clear();
    }
}
