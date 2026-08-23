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
    public Camera uiCamera;
    public bool isDebug = true; //自动判定的，不要改
    public GameObject Units;
    public int gridCellSize = 3; // 每个格子的实际大小(米)

    private Dictionary<int, List<Vector2Int>> occupiedGrids = new Dictionary<int, List<Vector2Int>>(); // 所有被占据的格子，键为chess.id

    public bool showDebugCube = false;
    private Dictionary<Vector2Int, GameObject> debugGridCubes = new Dictionary<Vector2Int, GameObject>(); // 格子与调试cube的映射

    private List<Chess> chessList = new List<Chess>(); // 所有棋子
    private int[] killMark = new int[8];
    private int[] deathOrder = new int[8]; // 记录各阵营的死亡顺序，0表示未死亡
    private int deathCount = 0; // 记录已死亡的阵营数量

    private bool gameFinish = false;
    private bool hasWin;
    private MapConfig mapConfig;
   
    public HeroInfoGroup heroInfoGroup;
    public Button buttonRestart;
    public TMP_Text textRestart;
    public Button buttonInfo;
    public GameObject BattleResultPanel;
    public GameObject BattleResultCellPrefab; // 用于显示玩家战斗结果的单元格预制体
    public GameObject BattleResultHeroCellPrefab; // 用于显示玩家战斗结果的单元格预制体
    private List<GameObject> battleResultCells = new List<GameObject>(); // 维护创建的结果单元格列表

    public GameObject HudNode;
    public GameObject BattleTextNode;
    private int idCounter = 100;

    void Start()
    {
        Instance = this;

        buttonRestart.onClick.AddListener(BattleEnd);
        buttonInfo.onClick.AddListener(ShowBattleResult);

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
        var roll = UnityEngine.Random.Range(0, 2);
        BGMPlayer.Instance.PlaySound(roll == 0 ? "BGMs/weifeng" : "BGMs/pozhu");

        var newMapId = 1;
        gameFinish = false;
        if (GameManager.Instance.year >= 5)
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

        killMark = new int[8];
        deathOrder = new int[8];
        deathCount = 0;
        BattleStatManager.Clear();

        // 通知所有玩家开始战斗
        foreach (var player in GameManager.Instance.players)
            player.OnBattleBegin();

        BattleResultPanel.gameObject.SetActive(false);
        SpawnUnitsInRegions();

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.CheckAddSkill(chess);

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.BattleBegin(chess);
        StartCoroutine(GameUpdate());
    }

    public void BattleEnd()
    {
        // 销毁所有结果单元格
        foreach (GameObject cell in battleResultCells)
        {
            if (cell != null)
                Destroy(cell);
        }
        battleResultCells.Clear();
        
        foreach (Transform child in Units.transform)
        {
            Destroy(child.gameObject);
        }
        chessList.Clear();

        foreach (Transform cell in HudNode.transform)
        {
            Destroy(cell.gameObject);
        }

        PanelManager.Instance.ShowShop();
        CardShopManager.Instance.ShopBegin();
    }

    public void ShowBattleResult()
    {
        var top10 = BattleStatManager.GetTop10();
        buttonInfo.gameObject.SetActive(false);
        // 获取RectTransform组件并设置宽度
        RectTransform battleResultRect = BattleResultPanel.GetComponent<RectTransform>();
        battleResultRect.sizeDelta = new Vector2(battleResultRect.sizeDelta.x + 800, battleResultRect.sizeDelta.y);
        for (int i = 0; i < top10.Count; i++)
        {
            var battleStat = top10[i];
            var cell = Instantiate(BattleResultHeroCellPrefab, BattleResultPanel.transform);
            cell.GetComponent<BattleResultHeroCellControl>().SetData(battleStat, i + 1);

            // 设置位置，每个单元格垂直偏移50
            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(302 + 700, -120 - i * 50); // 起始位置向下100，每个单元格间距50

            battleResultCells.Add(cell);
        }
    }

    private int[] GetMatch()
    {
        var battleIndex = GameManager.Instance.year;
        // 两两组合搭配方案
        if (battleIndex % 7 == 0)
            return new int[] { 0, 7, 1, 6, 2, 5, 3, 4 }; 
        else if (battleIndex % 7 == 1)
            return new int[] { 0, 1, 7, 4, 5, 3, 6, 2 }; 
        else if (battleIndex % 7 == 2)
            return new int[] { 0, 2, 1, 7, 3, 6, 4, 5 };
        else if (battleIndex % 7 == 3)
            return new int[] { 0, 3, 7, 5, 6, 4, 1, 2 };
        else if (battleIndex % 7 == 4)
            return new int[] { 0, 4, 2, 7, 3, 1, 5, 6 }; 
        else if (battleIndex % 7 == 5)
            return new int[] { 0, 5, 7, 6, 1, 4, 2, 3 }; 
        else
            return new int[] { 0, 6, 3, 7, 4, 2, 5, 1 }; 
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
            for (int i = 0; i < 8; i++)
            {
                GameManager.Instance.GetPlayer(match[i]).battleSide = i + 1;
            }
            var p = GameManager.Instance.GetPlayer(match[0]);
            for (int i = 0; i < mapConfig.RegionSide1.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, i, mapConfig.RegionSide1[i].transform.position, 1, p.imgPath);
            // 在RegionSide2生成单位 (阵营2)
            p = GameManager.Instance.GetPlayer(match[1]);
            for (int i = 0; i < mapConfig.RegionSide2.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, i, mapConfig.RegionSide2[i].transform.position, 2, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[2]);
            for (int i = 0; i < mapConfig.RegionSide3.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, i, mapConfig.RegionSide3[i].transform.position, 3, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[3]);
            for (int i = 0; i < mapConfig.RegionSide4.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, i, mapConfig.RegionSide4[i].transform.position, 4, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[4]);
            for (int i = 0; i < mapConfig.RegionSide5.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, i, mapConfig.RegionSide5[i].transform.position, 5, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[5]);
            for (int i = 0; i < mapConfig.RegionSide6.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, i, mapConfig.RegionSide6[i].transform.position, 6, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[6]);
            for (int i = 0; i < mapConfig.RegionSide7.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, i, mapConfig.RegionSide7[i].transform.position, 7, p.imgPath);
            p = GameManager.Instance.GetPlayer(match[7]);
            for (int i = 0; i < mapConfig.RegionSide8.Length; i++)
                SpawnUnitsForRegion(p, i < 3 ? 500001 : 500002, i, mapConfig.RegionSide8[i].transform.position, 8, p.imgPath);

            var cards = GameManager.Instance.GetPlayer(match[0]).GetBattleCardList();
            var cardsFriend = cards.Where(a => a != null).Select(a => a.Item1).ToList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide1.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[0]), i, mapConfig.RegionHeroSide1[i], cards[i], cardsFriend, 1);
            cards = GameManager.Instance.GetPlayer(match[1]).GetBattleCardList();
            cardsFriend = cards.Where(a => a != null).Select(a => a.Item1).ToList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide2.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[1]), i, mapConfig.RegionHeroSide2[i], cards[i], cardsFriend, 2);
            cards = GameManager.Instance.GetPlayer(match[2]).GetBattleCardList();
            cardsFriend = cards.Where(a => a != null).Select(a => a.Item1).ToList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide3.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[2]), i, mapConfig.RegionHeroSide3[i], cards[i], cardsFriend, 3);
            cards = GameManager.Instance.GetPlayer(match[3]).GetBattleCardList();
            cardsFriend = cards.Where(a => a != null).Select(a => a.Item1).ToList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide4.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[3]), i, mapConfig.RegionHeroSide4[i], cards[i], cardsFriend, 4);
            cards = GameManager.Instance.GetPlayer(match[4]).GetBattleCardList();
            cardsFriend = cards.Where(a => a != null).Select(a => a.Item1).ToList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide5.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[4]), i, mapConfig.RegionHeroSide5[i], cards[i], cardsFriend, 5);
            cards = GameManager.Instance.GetPlayer(match[5]).GetBattleCardList();
            cardsFriend = cards.Where(a => a != null).Select(a => a.Item1).ToList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide6.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[5]), i, mapConfig.RegionHeroSide6[i], cards[i], cardsFriend, 6);
            cards = GameManager.Instance.GetPlayer(match[6]).GetBattleCardList();
            cardsFriend = cards.Where(a => a != null).Select(a => a.Item1).ToList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide7.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[6]), i, mapConfig.RegionHeroSide7[i], cards[i], cardsFriend, 7);
            cards = GameManager.Instance.GetPlayer(match[7]).GetBattleCardList();
            cardsFriend = cards.Where(a => a != null).Select(a => a.Item1).ToList();
            for (int i = 0; i < cards.Count && i < mapConfig.RegionHeroSide8.Length; i++)
                if (cards[i] != null)
                    SpawnHerosForRegion(GameManager.Instance.GetPlayer(match[7]), i, mapConfig.RegionHeroSide8[i], cards[i], cardsFriend, 8);                    

            CreateCastleHUD(GameManager.Instance.GetPlayer(match[0]), mapConfig.RegionHeroSide1[4]);
            CreateCastleHUD(GameManager.Instance.GetPlayer(match[1]), mapConfig.RegionHeroSide2[4]);
            CreateCastleHUD(GameManager.Instance.GetPlayer(match[2]), mapConfig.RegionHeroSide3[4]);
            CreateCastleHUD(GameManager.Instance.GetPlayer(match[3]), mapConfig.RegionHeroSide4[4]);
            CreateCastleHUD(GameManager.Instance.GetPlayer(match[4]), mapConfig.RegionHeroSide5[4]);
            CreateCastleHUD(GameManager.Instance.GetPlayer(match[5]), mapConfig.RegionHeroSide6[4]);
            CreateCastleHUD(GameManager.Instance.GetPlayer(match[6]), mapConfig.RegionHeroSide7[4]);
            CreateCastleHUD(GameManager.Instance.GetPlayer(match[7]), mapConfig.RegionHeroSide8[4]);
        }
        else
        {
            GameManager.Instance.GetPlayer(0).banCount = 1;
            GameManager.Instance.GetPlayer(1).banCount = 2;
            //   SpawnHerosForRegion(GameManager.Instance.GetPlayer(0), mapConfig.RegionHeroSide1[4], new System.Tuple<int, int>(101008, 1), 1);
            var heroList = new List<int> { 103007 };
            for (int i = 0; i < heroList.Count; i++)
                SpawnHerosForRegion(GameManager.Instance.GetPlayer(0), i, mapConfig.RegionHeroSide1[i], new System.Tuple<int, int>(heroList[i], 1), heroList, 1);

            heroList = new List<int> { 101020,101020 };
            for (int i = 0; i < heroList.Count; i++)
                SpawnHerosForRegion(GameManager.Instance.GetPlayer(1), i, mapConfig.RegionHeroSide2[i], new System.Tuple<int, int>(heroList[i], 1), heroList, 2);

        }

    }

    public Chess SpawnUnitsForRegion(PlayerInfo p, int soldierId, int posId, UnityEngine.Vector3 spawnPos, int side, string imgPath)
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
            chessComponent.isFakeHero = soldierConfig.Model == "UnitHero";

            chessComponent.hitEffect = soldierConfig.HitEffect;
            chessComponent.soldierId = soldierId;
            chessComponent.playerId = p.pid;
            chessComponent.Init(p.pid, posId, p.lineColor);
        }
        else
        {
            Debug.LogError("Chess component not found on UnitBing prefab");
        }
        chessList.Add(chessComponent);

        idCounter++;

        return chessComponent;
    }

    private Chess SpawnHerosForRegion(PlayerInfo p, int posId, GameObject spawnPoint, System.Tuple<int, int> heroData, List<int> friendIds, int side)
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
                chessComponent.missileHight = heroConfig.MissileHight;

                if (side <= 2)
                {
                    var heroInfo = heroInfoGroup.AddHero(side, (int)heroConfig.Id, heroData.Item2);
                    chessComponent.heroInfo = heroInfo;
                }
                chessComponent.playerId = p.pid;
                chessComponent.CheckInitAttr(p, heroData.Item2, friendIds);
                chessComponent.Init(p.pid, posId, p.lineColor);
                // 可以在这里设置其他必要的初始化参数
            }
            else
            {
                Debug.LogError("Chess component not found on UnitBing prefab");
            }
            chessList.Add(chessComponent);
            idCounter++;

            return chessComponent;
        }
        return null;
    }


    // 创建血条HUD
    private void CreateCastleHUD(PlayerInfo p, GameObject castleSpawn)
    {
        // 查找或创建Canvas
        var canvas = FindObjectOfType<Canvas>();

        // 加载Hud预制体
        GameObject hudPrefab = Resources.Load<GameObject>("Prefabs/HudCastle");

        // 实例化HUD对象
        GameObject hudObj = Instantiate(hudPrefab, HudNode.transform);
        hudObj.name = "CastleHUD";

        // 获取ChessHUD组件
        var hud = hudObj.GetComponent<CastleHUD>();
        if (hud == null)
        {
            Debug.LogError("CastleHUD component not found on Hud.prefab");
            return;
        }

        // 初始化血条显示
        hud.Init(p, castleSpawn);
        p.castleHUD = hud;
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
            // 每个回合结束，玩家消耗食物
            foreach (var player in GameManager.Instance.players)
            {
                player.RoundFoodCost();
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
                            cellControl.SetData(player, i + 1, killMark[playerId]);
                        }
                    }

                    // 添加到维护列表
                    battleResultCells.Add(cell);
                }
            }
            buttonInfo.gameObject.SetActive(true);
            // 获取RectTransform组件并设置宽度
            RectTransform battleResultRect = BattleResultPanel.GetComponent<RectTransform>();
            battleResultRect.sizeDelta = new Vector2(650, battleResultRect.sizeDelta.y);
            BattleResultPanel.gameObject.SetActive(true);
        }
    }


    public void CreateAttackMissile(Chess sourceChess, Chess targetChess, string effectName)
    {
        // 首先加载导弹预制体
        Missile missilePrefab = Resources.Load<Missile>("Prefabs/MissileCom");
        
        // 实例化导弹
        var missile = Instantiate<Missile>(missilePrefab, sourceChess.transform.position, Quaternion.identity, Units.transform);
        missile.Init(sourceChess, 1, effectName);
        missile.MoveToTarget(targetChess, sourceChess.missileSpeed, sourceChess.missileHight);
    }

    public void CreateSpellMissile(Chess sourceChess, Chess targetChess, Vector3 startPos, int skillId, int damage, string effectName)
    {
        // 首先加载导弹预制体
        Missile missilePrefab = Resources.Load<Missile>("Prefabs/MissileCom");
        
        // 实例化导弹
        var missile = Instantiate<Missile>(missilePrefab, startPos, Quaternion.identity, Units.transform);
        missile.Init(sourceChess, 1, effectName);
        missile.SetSkillInfo(skillId, damage);
        missile.MoveToTarget(targetChess, Mathf.Max(sourceChess.missileSpeed, 14), sourceChess.missileHight);
    }    

    public void CreateSpellMissile(Chess sourceChess, Vector3 targetPos, float time, float speed, float size, int skillId, int damage, string effectName)
    {
        // 首先加载导弹预制体
        Missile missilePrefab = Resources.Load<Missile>("Prefabs/MissileCom");
        
        // 实例化导弹
        var missile = Instantiate<Missile>(missilePrefab, sourceChess.transform.position, Quaternion.identity, Units.transform);
        missile.Init(sourceChess, size, effectName);
        missile.SetSkillInfo(skillId, damage);
        missile.MoveToDirection(targetPos, time, speed);
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
    public bool TryLockGridPositions(Chess unit, Vector3 targetPosition, out List<Vector2Int> requiredGrids)
    {
        // 获取单位包围盒
        var collider = unit.GetComponent<Collider>();

        // 使用GetOccupiedGrids方法获取需要锁定的格子列表
        requiredGrids = GetOccupiedGrids(targetPosition, collider);
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
        return true;
    }

    public void DoLockGridPositions(Chess unit, List<Vector2Int> requiredGrids)
    {
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
    }

    public void ForceLockGridPositions(Chess unit, Vector3 targetPosition)
    {
        // 获取单位包围盒
        var collider = unit.GetComponent<Collider>();

        // 使用GetOccupiedGrids方法获取需要锁定的格子列表
        List<Vector2Int> requiredGrids = GetOccupiedGrids(targetPosition, collider);
        List<Vector2Int> toRemoves = new List<Vector2Int>();

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
                            toRemoves.Add(occupiedGrid);
                    }
                }
            }
        }

        ReleaseGridPositions(unit);
        requiredGrids.RemoveAll(x => toRemoves.Contains(x));
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
    }    

    public bool MoveTo(Chess unit, Vector3 targetPosition, bool isForce = false)
    {
        if (isForce)
        {
            ForceLockGridPositions(unit, targetPosition);
            unit.transform.position = targetPosition;

            return true;
        }
        else
        { 
            if(TryLockGridPositions(unit, targetPosition, out List<Vector2Int> requiredGrids))
            {
                DoLockGridPositions(unit, requiredGrids);
                unit.transform.position = targetPosition;
                return true;
            }
            return false;
        }

    }

    // 获取指定位置和碰撞体占据的所有格子
    public List<Vector2Int> GetOccupiedGrids(Vector3 position, Collider collider)
    {
        List<Vector2Int> occupiedGrids = new List<Vector2Int>();

        // 获取碰撞体边界
        Vector3 boundsSize = collider.bounds.size;
        Vector3 halfBounds = boundsSize / 3f;

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
        // 将oid散列到RGB值中
        int hash = oid * oid * 31 + oid * 3779; // 对哈希值进行位运算打散，避免值为1时不被打散的问题
        float r = Mathf.Abs((float)(hash & 0xFF) / 255f);
        float g = Mathf.Abs((float)((hash >> 8) & 0xFF) / 255f);
        float b = Mathf.Abs((float)((hash >> 16) & 0xFF) / 255f);
        cube.GetComponent<Renderer>().material.color = new Color(r, g, b);
        cube.name = "GridCube_" + hash;
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
            bool isTeam1 = a == 1 || a == 3 || a == 5 || a == 7;
            bool isTeam2 = a == 2 || a == 4 || a == 6 || a == 8;
            bool targetIsTeam1 = b == 1 || b == 3 || b == 5 || b == 7;
            bool targetIsTeam2 = b == 2 || b == 4 || b == 6 || b == 8;

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
            bool[] sideHasUnits = new bool[8];
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
            if (aliveSideCount == 4)
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
                    if (chessComponent.side == 1 || chessComponent.side == 3 || chessComponent.side == 5 || chessComponent.side == 7)
                    {
                        team1HasUnits = true;
                    }
                    else if (chessComponent.side == 2 || chessComponent.side == 4 || chessComponent.side == 6 || chessComponent.side == 8)
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
                    bool isTeam1 = playerSide == 1 || playerSide == 3 || playerSide == 5 || playerSide == 7;
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
            bool[] sideHasUnits = new bool[8];
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
            if (dieSideIndex >= 0 && dieSideIndex < 8)
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
                int[] scores = new int[8];
                for (int i = 0; i < scores.Length; i++)
                {
                    if (i + 1 == winnerSide)
                        scores[i] = 10; // 胜利者得10分
                    else if (deathOrder[i] == 7)
                        scores[i] = 7;
                    else if (deathOrder[i] == 6)
                        scores[i] = 5;
                    else if (deathOrder[i] == 5)
                        scores[i] = 5;
                    else if (deathOrder[i] == 4)
                        scores[i] = 3;
                    else if (deathOrder[i] == 3)
                        scores[i] = 2;
                    else if (deathOrder[i] == 2)
                        scores[i] = 1;
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

    public float GetRange(Vector3 pos1, Vector3 pos2)
    {
        Vector2Int pos1a = WorldManager.Instance.WorldToGridPosition(pos1, true);
        Vector2Int pos2a = WorldManager.Instance.WorldToGridPosition(pos2, true);

        return Vector2Int.Distance(pos1a, pos2a);
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

    public List<Chess> GetUnitsMySide(int mySide)
    {
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                if (chessComponent.side == mySide)
                    unitsInRange.Add(chessComponent);
            }
        }
        return unitsInRange;
    }

    public Chess FindByHeroIdAndSide(int heroId, int side)
    {
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                if(chessComponent.isHero && chessComponent.heroId == heroId && chessComponent.side == side)
                    return chessComponent;
            }
        }   
        return null;
    }

    public List<Chess> GetUnitsMySidePosType(int mySide, int pos, bool isHero, int selectType)
    {
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                if (chessComponent.side == mySide && chessComponent.isHero == isHero)
                {
                    if(selectType == 1 && pos / 3 == chessComponent.pos / 3)
                        unitsInRange.Add(chessComponent);
                    else if(selectType == 2 && ((pos % 3) == (chessComponent.pos % 3)))
                        unitsInRange.Add(chessComponent);
                    else if(selectType == 3)
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
        RectTransform rectTransform = battleText.GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPos = TransformWorldToScreen(worldPos + new UnityEngine.Vector3(5, 0, 5), parentCanvas);

        rectTransform.anchoredPosition = screenPos;

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
            float moveY = speed.y * timeDiff * scaleY / 80;

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

    public Vector2 TransformWorldToScreen(Vector3 worldPosition, RectTransform canvas)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas,
            screenPosition,
            uiCamera,
            out localPosition
        );

        return localPosition;
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
