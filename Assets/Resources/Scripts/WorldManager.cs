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
    private int[] currentMatch = null; // 本场战斗的1-8号位随机分配结果
    private bool isPveRound = false; // 本场是否为PVE（打怪物拿掉落）
    private List<System.Tuple<int, int>> pveMonsterSpawns = null; // PVE怪物布阵（怪物id, 布阵格0~24：0=左上，24=右下）
    private int pveFightCount = 1; // PVE仅玩家0参战（1号位打2号位怪物），AI不打怪
    private List<Tuple<GameObject, PlayerInfo, int>> bagDrops = new List<Tuple<GameObject, PlayerInfo, int>>(); // 怪物掉落包（BagDrop模型，获得玩家，首个掉落物id用于显示图标）
   
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
        var roll = SysRandom.Range(0, 2);
        BGMPlayer.Instance.PlaySound(roll == 0 ? "BGMs/weifeng" : "BGMs/pozhu");

        // 从回合配置表读取本回合可能刷的地图，随机选一张
        gameFinish = false;
        currentMatch = null; // 重置号位分配，本场重新随机
        var roundCfg = GameRoundConfig.GetConfig(Math.Min(100, GameManager.Instance.year));
        isPveRound = roundCfg.RoundType == 1;
        pveMonsterSpawns = ParsePveMonsterSpawns(roundCfg.SoldierList);
        if (isPveRound)
            GameLog.Debug($"[PVE] 回合{GameManager.Instance.year} {roundCfg.Name} 怪物布阵: {(string.IsNullOrEmpty(roundCfg.SoldierList) ? "(空!请在GameRoundConfig填SoldierList)" : roundCfg.SoldierList)} 解析到{pveMonsterSpawns.Count}只");
        var mapIds = roundCfg.MapIds;
        var newMapId = mapIds[SysRandom.Range(0, mapIds.Length)];
        if (mapConfig == null || newMapId != mapConfig.Mapid)
        {
            // 打印加载耗时
            var startTime = Time.realtimeSinceStartup;
            var mapNode = Resources.Load<GameObject>("Maps/Map" + newMapId);
            if (mapConfig != null)
                Destroy(mapConfig.gameObject);

            GameObject cell = Instantiate(mapNode, gameObject.transform.parent);
            mapConfig = cell.GetComponent<MapConfig>();
            var endTime = Time.realtimeSinceStartup;
            GameLog.Debug("加载地图耗时：" + (endTime - startTime) + "秒");
        }

        killMark = new int[8];
        deathOrder = new int[8];
        deathCount = 0;
        // 清理上一场残留的掉落包
        foreach (var drop in bagDrops)
        {
            if (drop.Item1 != null)
                Destroy(drop.Item1);
        }
        bagDrops.Clear();
        BattleStatManager.Clear();

        // 通知所有玩家开始战斗
        foreach (var player in GameManager.Instance.players)
            player.OnBattleBegin();

        BattleResultPanel.gameObject.SetActive(false);
        SpawnUnitsInRegions();

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.CheckAddSkill(chess);

        // 兵种连锁：按同职业英雄数量施加职业被动属性加成（自身加成+全队加成，不走技能系统）
        JobLinkManager.ApplyJobLinks();

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.BattleBegin(chess);

        // 默认护盾机制：同阵营英雄数量达到档位后直接获得护盾
        FactionShieldManager.ApplyFactionShields();

        // 连线(武将关系)：计算好友属性加成并创建连线特效
        FriendLineManager.ApplyFriendLines();

        // 好友连锁·特殊：在场好友数量提升关联(助益)技能等级（默认无技能，每多一个+1级）
        FriendLineManager.ApplyFriendSpecialSkills();

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
        // 每场战斗随机生成一次1-8号位分配，战斗期间（结算/结果显示）复用同一结果
        if (currentMatch == null)
        {
            currentMatch = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            for (int i = currentMatch.Length - 1; i > 0; i--)
            {
                int j = SysRandom.Range(0, i + 1);
                int temp = currentMatch[i];
                currentMatch[i] = currentMatch[j];
                currentMatch[j] = temp;
            }
        }
        return currentMatch;
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
              //  GameLog.Debug("Lock " + gridPos + " for wall");
            }
        }
        occupiedGrids[300001] = unitGrids;

        if (!isDebug)
        {
            int[] match = GetMatch();
            if (isPveRound)
            {
                // PVE：仅显示玩家0打怪局，AI不打怪
                // 玩家0固定占1号位，怪物刷2号位；其余玩家无战斗单位
                // 其他玩家（AI）战斗结束一起随机获得装备
                for (int i = 0; i < match.Length; i++)
                {
                    if (match[i] == 0)
                    {
                        int tmp = match[0];
                        match[0] = match[i];
                        match[i] = tmp;
                        break;
                    }
                }
                for (int i = 0; i < 8; i++)
                    GameManager.Instance.GetPlayer(match[i]).battleSide = i == 0 ? 1 : 0;

                if (mapConfig.SideCenters != null && mapConfig.SideCenters.Length >= 2 && mapConfig.SideCenters[0] != null)
                {
                    var playerCenter = mapConfig.SideCenters[0];
                    var monsterCenter = mapConfig.SideCenters[1];
                    var p = GameManager.Instance.GetPlayer(match[0]);

                    SpawnSoldiersForSide(p, playerCenter, 1);
                    SpawnHerosForSide(p, playerCenter, p.GetBattleCardList(), 1);
                    SpawnGongSummonForSide(p, playerCenter, 1);
                    CreateCastleHUD(p, playerCenter);

                    if (monsterCenter != null)
                        SpawnMonstersForSide(monsterCenter, 2);
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    GameManager.Instance.GetPlayer(match[i]).battleSide = i + 1;
                }
                for (int side = 1; side <= 8; side++)
                {
                    var p = GameManager.Instance.GetPlayer(match[side - 1]);
                    if (mapConfig.SideCenters == null || side - 1 >= mapConfig.SideCenters.Length || mapConfig.SideCenters[side - 1] == null)
                        continue;
                    var center = mapConfig.SideCenters[side - 1];
                    SpawnSoldiersForSide(p, center, side);
                    SpawnHerosForSide(p, center, p.GetBattleCardList(), side);
                    SpawnGongSummonForSide(p, center, side);
                    CreateCastleHUD(p, center);
                }
            }
        }
        else
        {
            GameManager.Instance.GetPlayer(0).banCount = 1;
            GameManager.Instance.GetPlayer(1).banCount = 2;
            var center1 = mapConfig.SideCenters != null && mapConfig.SideCenters.Length > 0 ? mapConfig.SideCenters[0] : null;
            var center2 = mapConfig.SideCenters != null && mapConfig.SideCenters.Length > 1 ? mapConfig.SideCenters[1] : null;

            var heroList = new List<int> { 103007 };
            for (int i = 0; i < heroList.Count && center1 != null; i++)
                SpawnHerosForRegion(GameManager.Instance.GetPlayer(0), i, GetFormationCellPos(center1, i), new System.Tuple<int, int>(heroList[i], 1), 1);

            heroList = new List<int> { 101020, 101020 };
            for (int i = 0; i < heroList.Count && center2 != null; i++)
                SpawnHerosForRegion(GameManager.Instance.GetPlayer(1), i, GetFormationCellPos(center2, i), new System.Tuple<int, int>(heroList[i], 1), 2);
        }

    }

    // 计算布阵图中第pos格(0~24)的世界坐标：row=pos/5 从上到下，col=pos%5 从左到右
    // 布阵图最上面一行(row0)朝向 center 的 forward 方向，即布阵图正上方在地图中的朝向(Y旋转决定)
    private Vector3 GetFormationCellPos(Transform center, int pos)
    {
        int row = pos / CombatConst.FormationGridSize;
        int col = pos % CombatConst.FormationGridSize;
        Vector3 forward = Vector3.ProjectOnPlane(center.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(center.right, Vector3.up).normalized;
        float half = (CombatConst.FormationGridSize - 1) * 0.5f;
        return center.position + forward * (row - half) * mapConfig.FormationCellSize + right * (col - half) * mapConfig.FormationCellSize;
    }

    // 生成一个势力的5个小兵：按布阵面板中小兵摆放位置生成(近战500001/远程500002)
    private void SpawnSoldiersForSide(PlayerInfo p, Transform center, int side)
    {
        for (int i = 0; i < p.battleCards.Length; i++)
        {
            if (p.battleCards[i] == 500001 || p.battleCards[i] == 500002)
                SpawnUnitsForRegion(p, p.battleCards[i], i, GetFormationCellPos(center, i), side, p.imgPath);
        }
    }

    // 生成一个势力的英雄：按布阵面板英雄摆放位置生成（索引=格子，跳过非英雄格）
    private void SpawnHerosForSide(PlayerInfo p, Transform center, List<System.Tuple<int, int>> cards, int side)
    {
        for (int i = 0; i < cards.Count && i < CombatConst.FormationCellCount; i++)
        {
            if (cards[i] == null || !ConfigManager.IsHeroCard(cards[i].Item1))
                continue;
            SpawnHerosForRegion(p, i, GetFormationCellPos(center, i), cards[i], side);
        }
    }

    // 工·机巧召唤：在创建小兵后按本侧上阵的"工"职业英雄数(1~5)定档，
    // 写死在该侧布阵空格上刷召唤单位（近战前排/远程后排，按单位射程划分随机空格）。
    private void SpawnGongSummonForSide(PlayerInfo p, Transform center, int side)
    {
        var units = GetUnitsMySide(side);
        int gongCount = 0;
        foreach (var u in units)
        {
            if (u == null || !u.isHero || u.hp <= 0) continue;
            var heroCfg = HeroConfig.GetConfig(u.heroId);
            var jobCfg = ConfigManager.GetJobConfig(heroCfg.Job);
            if (jobCfg != null && jobCfg.SkillId == "工") gongCount++;
        }
        if (gongCount <= 0)
            return;
        var lv = Mathf.Min(gongCount, 5);

        // 已占用布阵格（士兵/英雄），召唤单位落在空格上
        var occupied = new HashSet<int>();
        foreach (var u in units)
            if (u != null && u.pos >= 0 && u.pos < CombatConst.FormationCellCount)
                occupied.Add(u.pos);

        var spawnList = BuildGongSummonList(lv);
        var img = p != null ? p.imgPath : "";
        foreach (var item in spawnList)
        {
            var cfg = SoldierConfig.GetConfig(item.Item1);
            // row0=最贴敌方(前)，row4=最靠己方(后)：远程站后排(row3~4)，近战站前排(row0~1)，中间row2应急
            int rowMin, rowMax;
            if (cfg.Range >= 30) { rowMin = 3; rowMax = 4; }
            else { rowMin = 0; rowMax = 1; }

            for (var c = 0; c < item.Item2; c++)
            {
                int pos;
                if (!TryPickFreePos(occupied, rowMin, rowMax, out pos)
                    && !TryPickFreePos(occupied, 0, CombatConst.FormationGridSize - 1, out pos))
                    continue;
                occupied.Add(pos);
                SpawnUnitsForRegion(p, item.Item1, pos, GetFormationCellPos(center, pos), side, img);
            }
        }
    }

    // 在指定行范围内挑选一个未被占用的随机布阵格(pos=row*5+col)，范围无空位返回false
    private bool TryPickFreePos(HashSet<int> occupied, int rowMin, int rowMax, out int pos)
    {
        var size = CombatConst.FormationGridSize;
        var candidates = new List<int>();
        for (var row = rowMin; row <= rowMax; row++)
            for (var col = 0; col < size; col++)
            {
                var gridPos = row * size + col;
                if (!occupied.Contains(gridPos))
                    candidates.Add(gridPos);
            }
        if (candidates.Count == 0)
        {
            pos = -1;
            return false;
        }
        pos = candidates[SysRandom.Range(0, candidates.Count)];
        return true;
    }

    // 工·机巧各档位(工英雄数1~5)召唤单位硬编码：木牛流马=肉盾(近战前排)，喷火兽=远程火DPS(后排)，辅助=加buff(近战)
    private List<System.Tuple<int, int>> BuildGongSummonList(int lv)
    {
        var list = new List<System.Tuple<int, int>>();
        switch (lv)
        {
            case 1:
                list.Add(System.Tuple.Create(502001, 1)); // 木牛流马lv1
                break;
            case 2:
                list.Add(System.Tuple.Create(502002, 1)); // 木牛流马lv2
                break;
            case 3:
                list.Add(System.Tuple.Create(502002, 1)); // 木牛流马lv2
                list.Add(System.Tuple.Create(502003, 1)); // 喷火兽lv1
                break;
            case 4:
                list.Add(System.Tuple.Create(502002, 1)); // 双木牛流马lv2
                list.Add(System.Tuple.Create(502005, 1)); // 喷火兽lv2
                break;
            case 5:
                list.Add(System.Tuple.Create(502002, 1)); // 双木牛流马lv2
                list.Add(System.Tuple.Create(502005, 1)); // 喷火兽lv2
                list.Add(System.Tuple.Create(502004, 1)); // 辅助
                break;
        }
        return list;
    }

    public Chess SpawnUnitsForRegion(PlayerInfo p, int soldierId, int posId, UnityEngine.Vector3 spawnPos, int side, string imgPath)
    {
        var soldierConfig = SoldierConfig.GetConfig(soldierId);
        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/Battles/" + soldierConfig.Model);

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
            chessComponent.attackRate = soldierConfig.AtkSpeed / 30f; // 攻速值→每秒攻击次数（30=1次/秒；攻速20=1.5秒/次，15=2秒/次）
            chessComponent.missileSpeed = soldierConfig.MissileSpeed;
            chessComponent.missileHight = soldierConfig.MissileHight;
            chessComponent.armor = soldierConfig.Armor;
            chessComponent.magicRes = soldierConfig.MagicRes;
            chessComponent.isFakeHero = soldierConfig.Model == "UnitHero";

            chessComponent.hitEffect = soldierConfig.HitEffect;
            chessComponent.soldierId = soldierId;
            chessComponent.playerId = p.pid;
            chessComponent.Init(p.pid, posId, p.lineColor);
        }
        else
        {
            GameLog.Error("Chess component not found on UnitBing prefab");
        }
        chessList.Add(chessComponent);

        idCounter++;

        return chessComponent;
    }

    // 解析PVE怪物布阵配置"怪物id;布阵格|怪物id;布阵格"（布阵格0~24：0=左上，24=右下）
    private List<System.Tuple<int, int>> ParsePveMonsterSpawns(string cfg)
    {
        var result = new List<System.Tuple<int, int>>();
        if (string.IsNullOrEmpty(cfg))
            return result;
        foreach (var seg in cfg.Split('|'))
        {
            var parts = seg.Split(';');
            if (parts.Length != 2)
                continue;
            int id, pos;
            if (!int.TryParse(parts[0].Trim(), out id) || !int.TryParse(parts[1].Trim(), out pos))
                continue;
            if (pos < 0 || pos >= CombatConst.FormationCellCount)
                continue;
            result.Add(System.Tuple.Create(id, pos));
        }
        return result;
    }

    // PVE：按回合配置的怪物布阵在怪物号位生成（每只在配置指定的5x5布阵格上）
    private void SpawnMonstersForSide(Transform center, int side)
    {
        if (pveMonsterSpawns == null)
            return;
        foreach (var spawn in pveMonsterSpawns)
            SpawnMonsterForRegion(spawn.Item1, spawn.Item2, GetFormationCellPos(center, spawn.Item2), side);
    }

    // 生成一只PVE怪物：归属虚拟玩家999(PlayerConfig"怪物"，无PlayerInfo实体，不参与PVP匹配)，贴图用SoldierConfig.Img，死亡时按Drops掉落给配对玩家
    private Chess SpawnMonsterForRegion(int soldierId, int posId, UnityEngine.Vector3 spawnPos, int side)
    {
        var soldierConfig = SoldierConfig.GetConfig(soldierId);
        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/Battles/" + soldierConfig.Model);

        GameObject unitInstance = Instantiate(unitPrefab, spawnPos, Quaternion.identity, Units.transform);
        unitInstance.name = $"UnitMonster_{side}_{idCounter}";
        unitInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        Chess chessComponent = unitInstance.GetComponent<Chess>();
        if (chessComponent != null)
        {
            chessComponent.id = idCounter;
            chessComponent.isHero = false;
            chessComponent.side = side;
            // 怪物模型不显示玩家头像，显示SoldierConfig.Img配置的贴图
            chessComponent.chessName = soldierConfig.Img != null ? soldierConfig.Img : "";
            chessComponent.maxHp = soldierConfig.Hp;
            chessComponent.moveSpeed = soldierConfig.MoveSpeed;
            chessComponent.attackRange = soldierConfig.Range;
            chessComponent.attackDamage = soldierConfig.Atk;
            chessComponent.attackRate = soldierConfig.AtkSpeed / 30f;
            chessComponent.missileSpeed = soldierConfig.MissileSpeed;
            chessComponent.missileHight = soldierConfig.MissileHight;
            chessComponent.armor = soldierConfig.Armor;
            chessComponent.magicRes = soldierConfig.MagicRes;
            chessComponent.isFakeHero = soldierConfig.Model == "UnitHero";
            chessComponent.hitEffect = soldierConfig.HitEffect;
            chessComponent.soldierId = soldierId;
            chessComponent.playerId = PlayerBook.MonsterPlayerId;
            chessComponent.Init(PlayerBook.MonsterPlayerId, posId, Color.red);
        }
        else
        {
            GameLog.Error("Chess component not found on monster prefab");
        }
        chessList.Add(chessComponent);

        idCounter++;

        return chessComponent;
    }

    private Chess SpawnHerosForRegion(PlayerInfo p, int posId, UnityEngine.Vector3 spawnPos, System.Tuple<int, int> heroData, int side)
    {
        var heroConfig = HeroConfig.GetConfig(heroData.Item1);
        GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/Battles/UnitHero");

        // 实例化单位
        GameObject unitInstance = Instantiate(heroPrefab, spawnPos, Quaternion.identity, Units.transform);
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
            chessComponent.CheckInitAttr(p, heroData.Item2);
            chessComponent.Init(p.pid, posId, p.lineColor);
            // 可以在这里设置其他必要的初始化参数
        }
        else
        {
            GameLog.Error("Chess component not found on UnitBing prefab");
        }
        chessList.Add(chessComponent);
        idCounter++;

        return chessComponent;
    }


    // 创建血条HUD
    private void CreateCastleHUD(PlayerInfo p, Transform center)
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
            GameLog.Error("CastleHUD component not found on Hud.prefab");
            return;
        }

        // 初始化血条显示
        hud.Init(p, center);
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
        }

        {
            // 怪物掉落包变成卡牌飞向玩家头像，全部到达后等2秒再出结算界面
            if (bagDrops.Count > 0)
            {
                yield return MoveBagDropsToPlayers();
                yield return new WaitForSeconds(2f);
            }

            // PVE：未参战玩家没有战斗过程，战斗结束一起随机获得装备
            if (isPveRound)
                GivePveIdleDrops();

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

    // 战斗结束：怪物掉落包变成卡牌飞向获得玩家的头像
    private IEnumerator MoveBagDropsToPlayers()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;
        var movingCardPrefab = Resources.Load<GameObject>("Prefabs/MovingCard");

        var cards = new List<GameObject>();
        var starts = new List<Vector2>();
        var targets = new List<Vector2>();

        foreach (var drop in bagDrops)
        {
            var bag = drop.Item1;
            var player = drop.Item2;
            if (bag == null || player == null)
                continue;

            // 起点：掉落包世界坐标 → Canvas局部坐标
            Vector2 startLocalPos = TransformWorldToScreen(bag.transform.position, canvasRect);

            // 终点：玩家头像屏幕坐标 → Canvas局部坐标
            Vector2 targetScreenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, player.playerImage.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPoint, uiCamera, out Vector2 targetLocalPos);

            // 在掉落包位置生成飞行卡牌，贴图用首个掉落物图标
            var card = Instantiate(movingCardPrefab, canvas.transform, false);
            var img = card.GetComponent<Image>();
            if (ItemConfig.HasConfig(drop.Item3))
                img.sprite = Resources.Load<Sprite>("ItemPic/" + ItemConfig.GetConfig(drop.Item3).Icon);
            card.GetComponent<RectTransform>().anchoredPosition = startLocalPos;

            cards.Add(card);
            starts.Add(startLocalPos);
            targets.Add(targetLocalPos);

            // 销毁3D掉落包模型
            Destroy(bag);
        }
        bagDrops.Clear();

        // 卡牌飞向玩家头像（逐渐缩小到50%）
        float duration = 0.8f;
        float elapsedTime = 0;
        while (elapsedTime < duration && cards.Count > 0)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] == null)
                    continue;
                cards[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(starts[i], targets[i], t);
                cards[i].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(100, 140) * (1f - 0.5f * t);
            }
            yield return null;
        }

        foreach (var card in cards)
        {
            if (card != null)
                Destroy(card);
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
        // GameLog.Debug($"id:{unit.id} requiredGrids: Target Position = {targetPosition}, Collider Size = {collider.bounds.size}");
        // string gridPositions = string.Join(", ", requiredGrids);
        // GameLog.Debug($"Grids: {gridPositions}");

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
                         //   GameLog.Debug("Grid " + gridPos + " is already occupied by unit: " + entry.Key);
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
         //   GameLog.Debug("Lock " + gridPos + " for unit: " + unit.id);
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
         //   GameLog.Debug("Lock " + gridPos + " for unit: " + unit.id);
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
          //  GameLog.Debug("Released all grids for unit: " + unit.id);
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
        if (isPveRound)
        {
            // PVE：只有配对的(玩家奇数号位,怪物偶数号位)互为敌人，各对之间互不干扰
            return a != b && (a + 1) / 2 == (b + 1) / 2;
        }
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

    // PVE战斗结算：怪物死亡按配置掉落给配对玩家；所有(玩家,怪物)配对分出胜负后结束战斗
    private void HandlePveUnitDying(Chess dieUnit, int killerPlayerId)
    {
        int[] match = GetMatch();

        // 怪物死亡（归属虚拟玩家999且偶数号位）：掉落给配对的奇数号位玩家（2号怪→1号玩家）
        if (dieUnit.playerId == PlayerBook.MonsterPlayerId && dieUnit.side % 2 == 0)
        {
            int pairIdx = dieUnit.side / 2 - 1;
            if (pairIdx >= 0 && pairIdx < pveFightCount)
            {
                var p = GameManager.Instance.GetPlayer(match[pairIdx]);
                var drops = SoldierConfig.GetConfig(dieUnit.soldierId).RollDrops();
                foreach (var itemId in drops)
                {
                    p.AddItemCard(itemId);
                    var itemName = ItemConfig.HasConfig(itemId) ? ItemConfig.GetConfig(itemId).Name : itemId.ToString();
                    AddBattleText("掉落:" + itemName, dieUnit.transform.position, new UnityEngine.Vector2(0, 40), Color.yellow, 3);
                    GameLog.Debug($"[PVE] 玩家{p.pid} 击杀怪物{dieUnit.soldierId} 掉落 {itemName}({itemId})");
                }

                // 有掉落时在怪物位置创建掉落包模型，战斗结束时变成卡牌飞向玩家头像
                if (drops.Count > 0)
                {
                    var bagPrefab = Resources.Load<GameObject>("Prefabs/Battles/BagDrop");
                    var bag = Instantiate(bagPrefab, dieUnit.transform.position, Quaternion.identity, Units.transform);
                    bagDrops.Add(Tuple.Create(bag, p, drops[0]));
                }
            }
        }

        // 统计各阵营存活情况
        bool[] sideHasUnits = new bool[8];
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                int sideIndex = chessComponent.side - 1;
                if (sideIndex >= 0 && sideIndex < 8)
                    sideHasUnits[sideIndex] = true;
            }
        }

        // 每组(玩家=奇数位,怪物=偶数位)需分出胜负：一方全灭
        for (int i = 0; i < pveFightCount; i++)
        {
            if (sideHasUnits[i * 2] && sideHasUnits[i * 2 + 1])
                return; // 还有配对未分出胜负，战斗继续
        }

        // 全部配对结束：结算参战玩家（玩家存活=击杀全部怪物=胜利），PVE不加分都是0分
        for (int i = 0; i < pveFightCount; i++)
        {
            bool isWin = sideHasUnits[i * 2];
            GameManager.Instance.GetPlayer(match[i]).onBattleResult(isWin, 0);
        }

        gameFinish = true;
        int player0Idx = Array.IndexOf(match, 0);
        hasWin = player0Idx >= pveFightCount || sideHasUnits[player0Idx * 2];
    }

    // PVE：未参战玩家没有战斗过程，战斗结束一起随机获得装备（模拟一轮怪物掉落）
    private void GivePveIdleDrops()
    {
        if (pveMonsterSpawns == null || pveMonsterSpawns.Count == 0)
            return;
        int[] match = GetMatch();
        for (int i = pveFightCount; i < match.Length; i++)
        {
            var p = GameManager.Instance.GetPlayer(match[i]);
            foreach (var spawn in pveMonsterSpawns)
            {
                foreach (var itemId in SoldierConfig.GetConfig(spawn.Item1).RollDrops())
                {
                    p.AddItemCard(itemId);
                    var itemName = ItemConfig.HasConfig(itemId) ? ItemConfig.GetConfig(itemId).Name : itemId.ToString();
                    GameLog.Debug($"[PVE] 未参战玩家{p.pid} 随机获得 {itemName}({itemId})");
                }
            }
        }
    }

    public void OnUnitDying(Chess dieUnit, int killerPlayerId)
    {
        if(killerPlayerId >= 0 && killerPlayerId < killMark.Length && dieUnit.isHero)
            killMark[killerPlayerId]++;
        // 从chessList中移除死亡单位
        chessList.Remove(dieUnit);

        gameFinish = false;
        hasWin = false;
        if (isPveRound)
        {
            HandlePveUnitDying(dieUnit, killerPlayerId);
            return;
        }
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

            GameLog.Debug($"id:{dieUnit.id} dieUnit.side:{dieUnit.side} 存活阵营数:{aliveSideCount}");
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

            GameLog.Debug($"id:{dieUnit.id} dieUnit.side:{dieUnit.side} 团队1存活:{team1HasUnits} 团队2存活:{team2HasUnits}");
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
                    GameLog.Debug($"阵营 {dieUnit.side} 被消灭，死亡顺序: {deathCount}");
                }
            }

            GameLog.Debug($"id:{dieUnit.id} dieUnit.side:{dieUnit.side} 存活阵营数:{aliveSideCount}");

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
        if(limit < 0)
            return;
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
                    if(selectType == 1 && pos / CombatConst.FormationGridSize == chessComponent.pos / CombatConst.FormationGridSize)
                        unitsInRange.Add(chessComponent);
                    else if(selectType == 2 && ((pos % CombatConst.FormationGridSize) == (chessComponent.pos % CombatConst.FormationGridSize)))
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

        GameLog.Debug($"screenWidth:{screenWidth} screenHeight:{screenHeight}");

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
