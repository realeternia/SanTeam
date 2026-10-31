using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    public Camera uiCamera;
    public GameObject Units;
    public int gridCellSize = 3; // 每个格子的实际大小(米)

    private HashSet<Vector2Int> wallGrids = new HashSet<Vector2Int>(); // 墙占用的格子(静态障碍，单位不可入，踩墙校验用)
    private HashSet<Vector2Int> pathBlockedGrids = new HashSet<Vector2Int>(); // 墙膨胀2格后的寻路障碍(短程寻路/直线通畅判定用)

    private List<Chess> chessList = new List<Chess>(); // 所有棋子
    private int[] killMark = new int[8];
    private int[] deathOrder = new int[8]; // 记录各阵营的死亡顺序，0表示未死亡
    private int deathCount = 0; // 记录已死亡的阵营数量
    private bool[] sideBattleOver = new bool[8]; // 各阵营战斗是否已结束（无存活敌方：如地图规则0 独立配对中对手先被全灭）
    private bool[] sideDefeated = new bool[8]; // 各阵营是否已战败（本场参战过且已无存活单位），战败表现只处理一次
    private bool[] sideHadUnits = new bool[8]; // 各阵营本场是否参战过（避免从未参战的空阵营被判战败）
    // 城堡被消融替换前的原始材质（Renderer -> 原始材质数组），下一场开局恢复
    private readonly Dictionary<Renderer, Material[]> castleOriginalMats = new Dictionary<Renderer, Material[]>();
    // 本场被战败消融隐藏的城堡，下一场开局重新显示
    private readonly HashSet<GameObject> hiddenCastles = new HashSet<GameObject>();

    private bool gameFinish = false;
    private bool hasWin;
    private int battleVersion = 0; // 每场战斗自增，用于让上一场的城堡消融协程在新一场开始时自动失效
    private MapInfo mapInfo;
    private int[] currentMatch = null; // 本场战斗的1-8号位随机分配结果
    private bool isPveRound = false; // 本场是否为PVE（打怪物拿掉落）
    private List<System.Tuple<int, int>> pveMonsterSpawns = null; // PVE怪物布阵（怪物id, 布阵格0~24：0=左上，24=右下）
    private int pveFightCount = 1; // PVE仅玩家0参战（1号位打2号位怪物），AI不打怪
    private int oddPlayerSide = 0; // 单数存活玩家时随机选中的玩家号位（0=本场没有）
    private int oddMonsterSide = 0; // 与该玩家对应的野怪号位（rule0为敌方，rule1为友方助战）
    private List<Tuple<GameObject, PlayerInfo, int>> bagDrops = new List<Tuple<GameObject, PlayerInfo, int>>(); // 怪物掉落包（BagDrop模型，获得玩家，首个掉落物id用于显示图标）

    // 调试阵容·开局Buff：记录"调试单位 → 配置条目"，在 BattleBegin 全部技能结算后统一施加（见 ApplyDebugBuffs）
    private List<Tuple<Chess, GameManager.DebugHeroEntry>> debugBuffTargets = new List<Tuple<Chess, GameManager.DebugHeroEntry>>();
   
    public HeroInfoGroup heroInfoGroup;
    public BattleDefeatTip battleDefeatTip;
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

        buttonRestart.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            BattleEnd();
        });
        buttonInfo.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            ShowBattleResult();
        });

        StartCoroutine(DebugBattleBeginCheck());
    }

    IEnumerator DebugBattleBeginCheck()
    {
        // 判定统一走 GameManager：配置了调试阵容才直接开战，否则走正常的选牌→商店→战斗流程
        if (GameManager.Instance == null || !GameManager.Instance.HasDebugLineup())
            yield break;

        // 延迟2秒等场景/配置就绪
        yield return new WaitForSeconds(2f);
        ConfigManager.Init();
        // 调试阵容跳过了选牌/商店流程，year 不会被商店阶段累加（GameManager.year 初始为0），
        // 这里兜底为第1回合，否则 BattleBegin 里 GameRoundConfig.GetConfig(0) 会取不到配置
        if (GameManager.Instance.year <= 0)
        {
            GameManager.Instance.year = 1;
            GameLog.Info("调试阵容：跳过选牌/商店流程，回合数兜底为第1回合");
        }
        BattleBegin();
    }

    public void BattleBegin()
    {
        // 战斗阶段 BGM：battle1~battle4 随机一首
        BGMPlayer.Instance.PlaySound("BGMs/battle" + (SysRandom.Range(0, 4) + 1));

        // 从回合配置表读取本回合可刷地图权重，按权重随机选一张
        gameFinish = false;
        hasWin = false; // 本场重置胜负标记（阵亡处理里不再重置，见 OnUnitDying）
        battleVersion++;
        currentMatch = null; // 重置号位分配，本场重新随机
        var roundCfg = GameRoundConfig.GetConfig(Math.Min(100, GameManager.Instance.year));
        isPveRound = roundCfg.RoundType == 1;
        pveMonsterSpawns = ParsePveMonsterSpawns(roundCfg.SoldierList);
        if (isPveRound)
            GameLog.Debug($"[PVE] 回合{GameManager.Instance.year} {roundCfg.Name} 怪物布阵: {(string.IsNullOrEmpty(roundCfg.SoldierList) ? "(空!请在GameRoundConfig填SoldierList)" : roundCfg.SoldierList)} 解析到{pveMonsterSpawns.Count}只");
        // 调试阵容（测试战斗）固定使用 MapConfig 中 IsTestDefault 标记的地图（多个 true 取第一个），未标记时按权重随机
        MapConfig testMapCfg = null;
        if (GameManager.Instance.HasDebugLineup())
        {
            foreach (var cfg in MapConfig.ConfigList)
            {
                if (cfg.IsTestDefault)
                {
                    testMapCfg = cfg;
                    break;
                }
            }
        }
        var newMapId = testMapCfg != null ? testMapCfg.Id : RollMapId(roundCfg.MapWeights);
        if (mapInfo == null || newMapId != mapInfo.Mapid)
        {
            // 打印加载耗时
            var startTime = Time.realtimeSinceStartup;
            var mapNode = Resources.Load<GameObject>("Maps/" + MapConfig.GetConfig(newMapId).Prefab);
            if (mapInfo != null)
                Destroy(mapInfo.gameObject);

            GameObject cell = Instantiate(mapNode, gameObject.transform.parent);
            mapInfo = cell.GetComponent<MapInfo>();
            var endTime = Time.realtimeSinceStartup;
            GameLog.Debug("加载地图耗时：" + (endTime - startTime) + "秒");
        }

        killMark = new int[8];
        deathOrder = new int[8];
        deathCount = 0;
        sideBattleOver = new bool[8]; // 各阵营战斗结束状态本场重置
        sideDefeated = new bool[8];
        sideHadUnits = new bool[8];
        // 同地图复用时不会重新加载地图，需恢复上一场被消融移除的城堡
        ResetCastles();
        // 清理上一场残留的掉落包
        foreach (var drop in bagDrops)
        {
            if (drop.Item1 != null)
                Destroy(drop.Item1);
        }
        bagDrops.Clear();
        BattleStatManager.Clear();
        FatigueManager.Reset(); // 疲劳计时本场重新开始

        // 通知所有玩家开始战斗
        foreach (var player in GameManager.Instance.players)
        {
            player.lastDamage = 0;
            player.OnBattleBegin();
        }

        BattleResultPanel.gameObject.SetActive(false);
        SpawnUnitsInRegions();
        // 记录本场参战阵营（后续战败判定只对参战过且单位全灭的阵营生效）
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                int sideIndex = chessComponent.side - 1;
                if (sideIndex >= 0 && sideIndex < sideHadUnits.Length)
                    sideHadUnits[sideIndex] = true;
            }
        }
        GameLog.Debug("[BattleInit] spawn done");

        // 存活玩家≤1（其余已淘汰）：没有可匹配的对手，直接判定该玩家获胜，避免无敌人导致战斗永不结束
        if (!isPveRound && GameManager.Instance.GetAlivePlayerIds().Count <= 1)
            ResolveBattleWithoutOpponent();

        // 兵种连锁：按同职业英雄数量施加职业被动属性加成（自身加成+全队加成，不走技能系统）
        JobLinkManager.ApplyJobLinks();
        GameLog.Debug("[BattleInit] jobLink done");

        // 好友连锁·特殊：在场好友数量提升关联(助益)技能等级并创建连线特效（每多一个+1级）
        FriendLineManager.ApplyFriendSpecialSkills();
        GameLog.Debug("[BattleInit] friendSpecial done");

        // 国家护盾：同阵营英雄数量达到档位后授予对应等级的国家护盾技能（效果由技能在 BattleBegin 施加）
        FactionShieldManager.ApplyFactionShields();
        GameLog.Debug("[BattleInit] factionShield done");

        // 战斗开始技能：必须等在好友特殊技能授予之后，否则 BattleBegin 型技能（如偷袭/智勇）取不到技能
        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.BattleBegin(chess);
        GameLog.Debug("[BattleInit] skills done");

        // 调试阵容·开局Buff：等技能结算完毕后统一挂载（时长从开战时刻起算）
        ApplyDebugBuffs();

        StartCoroutine(GameUpdate());
    }

    // 调试阵容·开局Buff：给窗口里配了Buff的调试武将按条目挂载Buff（时长取条目配置，默认10秒）。
    // Buff 的效果数值统一读 SkillConfig，这里用零值调试配置行（CombatConst.DebugBuffSkillId），
    // 只用于验证Buff的挂载/表现/时长，不产生数值影响。
    private void ApplyDebugBuffs()
    {
        if (debugBuffTargets.Count == 0)
            return;

        // 零值调试技能配置行：数值槽走 key-value 串，留空即可（各 Buff 脚本按固定 key 读取，缺 key 得 0）
        SkillConfig.Assign(CombatConst.DebugBuffSkillId, new SkillConfig
        {
            Id = CombatConst.DebugBuffSkillId,
            Name = "调试Buff",
            Sname = "调试Buff",
            Strength2 = "",
            StrengthBuff1 = "",
        });

        foreach (var target in debugBuffTargets)
        {
            var chess = target.Item1;
            var hero = target.Item2;
            if (chess == null || hero == null || chess.hp <= 0)
                continue;
            if (!BuffConfig.HasConfig(hero.buffId))
            {
                GameLog.Error($"调试阵容：Buff配置不存在 id={hero.buffId}");
                continue;
            }
            float time = hero.buffTime > 0f ? hero.buffTime : CombatConst.DebugBuffDefaultTime;
            GameLog.Info($"调试阵容：给 {chess.chessName}(id{chess.id}) 挂载Buff {hero.buffId}，时长{time}秒");
            BuffManager.AddBuff(chess, chess, CombatConst.DebugBuffSkillId, hero.buffId, time);
        }
        debugBuffTargets.Clear();
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

        // 人类玩家生命耗尽：游戏结束，回到商店界面并停住（不再开启新回合商店/新战斗），并弹出结算面板
        if (GameManager.Instance.GetPlayer(0).isEliminated)
        {
            CardShopManager.Instance.EndGame();
            PanelManager.Instance.ShowGameResult();
            return;
        }

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
        if (currentMatch != null)
            return currentMatch;

        currentMatch = new int[8];
        for (int i = 0; i < currentMatch.Length; i++)
            currentMatch[i] = -1; // -1=该号位无玩家（已淘汰/空位/野怪专用）
        oddPlayerSide = 0;
        oddMonsterSide = 0;

        var alive = GameManager.Instance.GetAlivePlayerIds();
        int n = alive.Count;
        if (n == 0)
        {
            GameLog.Error("战斗匹配失败：已无存活玩家");
            return currentMatch;
        }

        // 单数存活玩家（3人及以上）：随机挑一名排名第3及以后的玩家（生命值排名，生命高者为前列），
        // rule0让其对面刷野怪、rule1为其本方刷野怪助战
        int oddPid = -1;
        if (!isPveRound && n >= 3 && n % 2 == 1)
        {
            var byLife = alive.OrderByDescending(pid => GameManager.Instance.GetPlayer(pid).mark).ToList();
            var candidates = byLife.Skip(2).ToList();
            oddPid = candidates.Count > 0
                ? candidates[SysRandom.Range(0, candidates.Count)]
                : byLife[byLife.Count - 1];
        }

        int rule = mapInfo != null ? MapConfig.GetConfig(mapInfo.Mapid).Rule : 0;
        if (oddPid >= 0 && rule == 1)
            AssignTeamSlotsWithMonster(alive, oddPid);
        else if (oddPid >= 0)
            AssignPairSlotsWithMonster(alive, oddPid);
        else
            AssignSequentialSlots(alive);

        GameLog.Debug($"[匹配] 存活{n}人 rule={rule} 选中玩家={oddPid} 野怪号位={oddMonsterSide} 号位=[{string.Join(",", currentMatch)}]");
        return currentMatch;
    }

    // 普通匹配：存活玩家随机填号位（人类固定1号位）
    private void AssignSequentialSlots(List<int> alive)
    {
        var order = new List<int>(alive);
        ShufflePids(order);
        int zeroIndex = order.IndexOf(0);
        if (zeroIndex > 0)
        {
            int tmp = order[0];
            order[0] = order[zeroIndex];
            order[zeroIndex] = tmp;
        }
        for (int i = 0; i < order.Count; i++)
            currentMatch[i] = order[i];
    }

    // rule0独立配对 + 单数玩家：玩家从1号位起连续占位（存活玩家用前面的初始点），
    // 被选中玩家占最后一个玩家号位、野怪刷在其对面号位（两个号位同属最后一组配对）
    private void AssignPairSlotsWithMonster(List<int> alive, int oddPid)
    {
        int numSides = alive.Count + 1; // 玩家 + 野怪总数（偶数）
        int specialSlot = numSides - 2; // 被选中玩家的0-based号位（最后一个玩家位）
        currentMatch[specialSlot] = oddPid;
        oddPlayerSide = specialSlot + 1;
        oddMonsterSide = numSides;      // 野怪占最后一号位，与 specialSlot 相邻成对

        var others = alive.Where(pid => pid != oddPid).ToList();
        ShufflePids(others);

        // 人类(pid0)若不是被选中玩家，固定使用第1号位
        if (others.Contains(0))
        {
            others.Remove(0);
            currentMatch[0] = 0;
        }

        // 剩余玩家按1号位起顺序填未被占用的号位（号位连续，不留空洞）
        int idx = 0;
        foreach (var pid in others)
        {
            while (idx < numSides && currentMatch[idx] >= 0)
                idx++;
            if (idx >= numSides)
            {
                GameLog.Error($"配对匹配号位不足：玩家{pid}未能分配号位");
                break;
            }
            currentMatch[idx] = pid;
            idx++;
        }
    }

    // rule1团队 + 单数玩家：玩家+助战野怪总数(numSides)从1号位起连续占位（先用前面的初始点），
    // 按号位奇偶分两队（1/3/5/7 与 2/4/6/8），被选中玩家与其助战野怪同队且占用最后两个同队号位
    private void AssignTeamSlotsWithMonster(List<int> alive, int oddPid)
    {
        int numSides = alive.Count + 1;          // 玩家 + 野怪总数（单数玩家+1后必为偶数）
        int monsterSlot = numSides - 1;          // 0-based 最后一个号位
        int specialSlot = numSides - 3;          // 与野怪奇偶相同、同队的前一个号位（被选中玩家）

        currentMatch[specialSlot] = oddPid;
        currentMatch[monsterSlot] = -1;          // 野怪不占玩家槽位，生成时按 oddMonsterSide 单独刷
        oddPlayerSide = specialSlot + 1;
        oddMonsterSide = monsterSlot + 1;

        var others = alive.Where(pid => pid != oddPid).ToList();
        ShufflePids(others);

        // 人类(pid0)若不是被选中玩家，固定使用第1号位
        if (others.Contains(0))
        {
            others.Remove(0);
            currentMatch[0] = 0;
        }

        // 剩余玩家按1号位起顺序填未被占用的号位（号位连续，不留空洞）
        int idx = 0;
        foreach (var pid in others)
        {
            while (idx < numSides && currentMatch[idx] >= 0)
                idx++;
            if (idx >= numSides)
            {
                GameLog.Error($"团队匹配号位不足：玩家{pid}未能分配号位");
                break;
            }
            currentMatch[idx] = pid;
            idx++;
        }
    }

    private void ShufflePids(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = SysRandom.Range(0, i + 1);
            int tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    private void SpawnUnitsInRegions()
    {
        // 清空之前的单位
        foreach (Transform child in Units.transform)
        {
            Destroy(child.gameObject);
        }
        wallGrids.Clear();
        pathBlockedGrids.Clear();
        heroInfoGroup.Reset();

        // 生成墙
        for (int i = 0; i < mapInfo.WallNode.transform.childCount; i++)
        {
            var wallNodeCell = mapInfo.WallNode.transform.GetChild(i);
            // 使用GetOccupiedGrids方法获取需要锁定的格子列表
            List<Vector2Int> requiredGrids = GetOccupiedGrids(wallNodeCell.transform.position, wallNodeCell.GetComponent<Collider>());
            // 锁定新格子

            foreach (var gridPos in requiredGrids)
            {
                wallGrids.Add(gridPos);
                // 膨胀2格(约6米，超过单位半宽4.5米)作为寻路障碍，保证单位按路走不擦墙
                for (int dx = -gridCellSize * 2; dx <= gridCellSize * 2; dx += gridCellSize)
                {
                    for (int dz = -gridCellSize * 2; dz <= gridCellSize * 2; dz += gridCellSize)
                    {
                        pathBlockedGrids.Add(new Vector2Int(gridPos.x + dx, gridPos.y + dz));
                    }
                }
              //  GameLog.Debug("Lock " + gridPos + " for wall");
            }
        }

        // 本场调试开局Buff目标清空重收（仅调试阵容分支会重新填充，见 SpawnDebugHeroUnit）
        debugBuffTargets.Clear();
        if (!GameManager.Instance.HasDebugLineup())
        {
            // 非调试阵容：关闭调试用的满MP开关，避免上一次调试开局的静态值残留影响正常对局
            CombatConst.FullMpAtBattleBegin = false;
            int[] match = GetMatch();
            // 本场所有玩家先清空参战号位（淘汰/未参战玩家保持0，供胜负判定用）
            foreach (var player in GameManager.Instance.players)
                player.battleSide = 0;

            if (isPveRound)
            {
                // PVE：仅显示玩家0打怪局，AI不打怪
                // 玩家0固定占1号位（GetMatch已保证match[0]=玩家0），怪物刷2号位；其余玩家无战斗单位
                // 其他玩家（AI）战斗结束一起随机获得装备
                for (int i = 0; i < 8; i++)
                {
                    if (match[i] >= 0)
                        GameManager.Instance.GetPlayer(match[i]).battleSide = i == 0 ? 1 : 0;
                }

                if (mapInfo.SideCenters != null && mapInfo.SideCenters.Length >= 2 && mapInfo.SideCenters[0] != null)
                {
                    var playerCenter = mapInfo.SideCenters[0];
                    var monsterCenter = mapInfo.SideCenters[1];

                    if (match[0] < 0)
                    {
                        GameLog.Error("PVE战斗：1号位没有可用玩家，跳过玩家侧生成");
                    }
                    else
                    {
                        var p = GameManager.Instance.GetPlayer(match[0]);

                        var cards = p.GetBattleCardList(); // AI 侧会据此重算英雄/小兵布阵，需先于小兵生成
                        SpawnSoldiersForSide(p, playerCenter, 1);
                        SpawnHerosForSide(p, playerCenter, cards, 1);
                        SpawnGongSummonForSide(p, playerCenter, 1);
                        GameLog.Debug("[BattleInit] gongSummon done");
                        CreateCastleHUD(p, playerCenter, 1);
                        GameLog.Debug("[BattleInit] castleHud done");
                    }

                    if (monsterCenter != null)
                        SpawnMonstersForSide(monsterCenter, 2);
                    GameLog.Debug("[BattleInit] monsters done");
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    if (match[i] >= 0)
                        GameManager.Instance.GetPlayer(match[i]).battleSide = i + 1;
                }
                for (int side = 1; side <= 8; side++)
                {
                    int pid = match[side - 1];
                    if (pid < 0)
                        continue;
                    if (mapInfo.SideCenters == null || side - 1 >= mapInfo.SideCenters.Length || mapInfo.SideCenters[side - 1] == null)
                        continue;
                    var p = GameManager.Instance.GetPlayer(pid);
                    var center = mapInfo.SideCenters[side - 1];
                    var cards = p.GetBattleCardList(); // AI 侧会据此重算英雄/小兵布阵，需先于小兵生成
                    SpawnSoldiersForSide(p, center, side);
                    SpawnHerosForSide(p, center, cards, side);
                    SpawnGongSummonForSide(p, center, side);
                    CreateCastleHUD(p, center, side);
                }

                // 单数存活玩家：在 oddMonsterSide 刷一组野怪（rule0为对手、rule1为助战，由号位奇偶决定敌我）
                if (oddMonsterSide >= 1)
                {
                    if (pveMonsterSpawns == null || pveMonsterSpawns.Count == 0)
                    {
                        GameLog.Error($"单数玩家匹配：回合{GameManager.Instance.year} SoldierList 为空，无法刷野怪");
                    }
                    else if (mapInfo.SideCenters == null || oddMonsterSide - 1 >= mapInfo.SideCenters.Length || mapInfo.SideCenters[oddMonsterSide - 1] == null)
                    {
                        GameLog.Error($"单数玩家匹配：{oddMonsterSide}号位没有地图中心点，无法刷野怪");
                    }
                    else
                    {
                        SpawnMonstersForSide(mapInfo.SideCenters[oddMonsterSide - 1], oddMonsterSide);
                        GameLog.Debug($"[匹配] 单数玩家{oddPlayerSide}号位，野怪刷{oddMonsterSide}号位");
                    }
                }
            }
        }
        else
        {
            // 调试阵容：玩家0占1号位、玩家1占2号位，武将取自 GameManager 配置的调试列表
            // 开局满MP开关由调试阵容窗口的「开局技能MP拉满」控制（默认勾选）
            CombatConst.FullMpAtBattleBegin = GameManager.Instance.debugFullMp;
            // 调试阵容同步参战号位（胜负判定依赖 player.battleSide）
            foreach (var player in GameManager.Instance.players)
                player.battleSide = 0;
            GameManager.Instance.GetPlayer(0).battleSide = 1;
            GameManager.Instance.GetPlayer(1).battleSide = 2;
            GameManager.Instance.GetPlayer(0).likeCount = 2;
            GameManager.Instance.GetPlayer(1).likeCount = 2;
            var center1 = mapInfo.SideCenters != null && mapInfo.SideCenters.Length > 0 ? mapInfo.SideCenters[0] : null;
            var center2 = mapInfo.SideCenters != null && mapInfo.SideCenters.Length > 1 ? mapInfo.SideCenters[1] : null;

            SpawnDebugHeroes(GameManager.Instance.GetPlayer(0), center1, GameManager.Instance.debugHeroesSide1, 1);
            SpawnDebugHeroes(GameManager.Instance.GetPlayer(1), center2, GameManager.Instance.debugHeroesSide2, 2);
        }

    }

    // 调试阵容：按近战/远程分前后排摆入中间英雄格(HeroCells)——近战从顶排往下填(前排)，远程从底排往上填(后排)；
    // 超出9格的武将再按布阵格顺序补空位。等级取配置值(1~MaxHeroCardLevel)
    private void SpawnDebugHeroes(PlayerInfo p, Transform center, List<GameManager.DebugHeroEntry> heroes, int side)
    {
        if (center == null || heroes == null)
            return;

        var melee = new List<GameManager.DebugHeroEntry>();
        var ranged = new List<GameManager.DebugHeroEntry>();
        foreach (var hero in heroes)
        {
            if (hero == null)
                continue;
            int heroId = hero.heroId;
            if (heroId <= 0 || !ConfigManager.IsHeroCard(heroId) || !HeroConfig.HasConfig(heroId))
            {
                GameLog.Error($"调试阵容非法武将ID: {heroId}(side{side})");
                continue;
            }
            if (HeroSelectionTool.IsRangedHero(HeroConfig.GetConfig(heroId)))
                ranged.Add(hero);
            else
                melee.Add(hero);
        }

        var heroCells = CombatConst.HeroCells;
        var occupied = new HashSet<int>();
        // 近战：HeroCells 顶排往下填(前排)
        int mi = 0;
        for (int ci = 0; ci < heroCells.Length && mi < melee.Count; ci++, mi++)
        {
            SpawnDebugHeroUnit(p, center, heroCells[ci], melee[mi], side);
            occupied.Add(heroCells[ci]);
        }
        // 远程：HeroCells 底排往上填(后排)
        int ri = 0;
        for (int ci = heroCells.Length - 1; ci >= 0 && ri < ranged.Count; ci--)
        {
            if (occupied.Contains(heroCells[ci]))
                continue;
            SpawnDebugHeroUnit(p, center, heroCells[ci], ranged[ri], side);
            occupied.Add(heroCells[ci]);
            ri++;
        }
        // 溢出：剩余武将按布阵格顺序补空位
        for (int pos = 0; pos < CombatConst.FormationCellCount && (mi < melee.Count || ri < ranged.Count); pos++)
        {
            if (occupied.Contains(pos))
                continue;
            SpawnDebugHeroUnit(p, center, pos, mi < melee.Count ? melee[mi++] : ranged[ri++], side);
            occupied.Add(pos);
        }
    }

    // 生成一个调试武将到指定布阵格(等级取配置值并夹在 1~MaxHeroCardLevel)
    private void SpawnDebugHeroUnit(PlayerInfo p, Transform center, int pos, GameManager.DebugHeroEntry hero, int side)
    {
        int level = Mathf.Clamp(hero.level, 1, HeroSelectionTool.MaxHeroCardLevel);
        var chess = SpawnHerosForRegion(p, pos, GetFormationCellPos(center, pos), new System.Tuple<int, int>(hero.heroId, level), side);
        // 调试开局Buff：先记录待挂载条目，等 BattleBegin 全部技能结算后再施加（见 ApplyDebugBuffs）
        if (chess != null && hero.buffId > 0)
            debugBuffTargets.Add(new Tuple<Chess, GameManager.DebugHeroEntry>(chess, hero));
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
        return center.position + forward * (row - half) * mapInfo.FormationCellSize + right * (col - half) * mapInfo.FormationCellSize;
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
            if (jobCfg != null && jobCfg.NameS == "工") gongCount++;
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
            chessComponent.atk = soldierConfig.Atk;
            chessComponent.attackSpeed = soldierConfig.AtkSpeed / 30f; // 攻速值→每秒攻击次数（30=1次/秒；攻速20=1.5秒/次，15=2秒/次）
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

    // 解析回合可刷地图权重"地图英文名:权重,地图英文名:权重"（与 SkillConfig 数值槽 k:v、ItemConfig 掉落权重同一套解析口径：
    // 逗号分隔、键值冒号、相对权重、≤0 不参与），按权重随机返回一个地图Id；无有效条目时记录错误并回退地图1。
    private int RollMapId(string cfg)
    {
        var entries = new List<KeyValuePair<int, int>>();
        int totalWeight = 0;
        if (!string.IsNullOrEmpty(cfg))
        {
            foreach (var seg in cfg.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var s = seg.Trim();
                int i = s.IndexOf(':');
                if (i <= 0)
                    continue;
                var nameE = s.Substring(0, i).Trim();
                int weight;
                if (!int.TryParse(s.Substring(i + 1).Trim(), out weight) || weight <= 0)
                    continue;
                var mapCfg = MapConfig.GetConfigByNameE(nameE);
                entries.Add(new KeyValuePair<int, int>(mapCfg.Id, weight));
                totalWeight += weight;
            }
        }
        if (totalWeight <= 0)
        {
            GameLog.Error($"GameRoundConfig.MapWeights 无有效地图权重（{cfg}），回退地图1");
            return 1;
        }
        int roll = SysRandom.Range(0, totalWeight);
        foreach (var entry in entries)
        {
            roll -= entry.Value;
            if (roll < 0)
                return entry.Key;
        }
        return entries[entries.Count - 1].Key;
    }

    // PVE：按回合配置的怪物布阵在怪物号位生成（每只在配置指定的5x5布阵格上）
    private void SpawnMonstersForSide(Transform center, int side)
    {
        if (pveMonsterSpawns == null)
            return;
        foreach (var spawn in pveMonsterSpawns)
            SpawnMonsterForRegion(spawn.Item1, spawn.Item2, GetFormationCellPos(center, spawn.Item2), side);
    }

    // 生成一只PVE怪物：归属虚拟玩家999(PlayerConfig"怪物"，无PlayerInfo实体，不参与PVP匹配)，贴图用SoldierConfig.Img，死亡时按IsDrop从道具掉落池随机掉落给配对玩家
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
            chessComponent.atk = soldierConfig.Atk;
            chessComponent.attackSpeed = soldierConfig.AtkSpeed / 30f;
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
            // 导弹速度/高度、命中特效按职业基准从 JobConfig 取（HeroConfig 不再配置）
            var jobCfg = ConfigManager.GetJobConfig(heroConfig.Job);
            chessComponent.hitEffect = jobCfg != null ? jobCfg.HitEffect : "";
            chessComponent.missileSpeed = jobCfg != null ? jobCfg.MissileSpeed : 0;
            chessComponent.missileHight = jobCfg != null ? jobCfg.MissileHight : 0f;

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


    // 创建城堡HUD（位置跟随阵营城堡）
    private void CreateCastleHUD(PlayerInfo p, Transform center, int side)
    {
        // 加载Hud预制体
        GameObject hudPrefab = Resources.Load<GameObject>("Prefabs/HudCastle");

        // 实例化HUD对象
        GameObject hudObj = Instantiate(hudPrefab, HudNode.transform);
        hudObj.name = "CastleHUD";

        // 获取CastleHUD组件
        var hud = hudObj.GetComponent<CastleHUD>();
        if (hud == null)
        {
            GameLog.Error("CastleHUD component not found on Hud.prefab");
            return;
        }

        // 初始化血条显示：HUD锚定该阵营的城堡
        GameObject castle = GetCastle(side);
        hud.Init(p, castle != null ? castle.transform : null, center, side);
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
            // 疲劳结算：超时后每间隔对全体英雄施加递增真实伤害
            FatigueManager.Tick(0.05f);
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

            // 战斗结束按胜负播放音效
            GameManager.Instance.PlaySound(hasWin ? "Sounds/win" : "Sounds/loss");

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
                // 根据玩家的 mark（生命值）排序（-1号位为空/已淘汰，不显示）
                var sortedPlayers = match
                    .Where(id => id >= 0)
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
                img.sprite = ItemIconLoader.Load(ItemConfig.GetConfig(drop.Item3).Icon);
            card.GetComponent<RectTransform>().anchoredPosition = startLocalPos;

            cards.Add(card);
            targets.Add(targetLocalPos);

            // 销毁3D掉落包模型
            Destroy(bag);
        }
        bagDrops.Clear();

        // 卡牌飞向玩家头像（DoTween 加速曲线：起步慢、越飞越快，同时逐渐缩小到50%）
        const float duration = 0.8f;
        const float shrinkScale = 0.5f;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null)
                continue;

            RectTransform cardRect = cards[i].GetComponent<RectTransform>();
            cardRect.DOAnchorPos(targets[i], duration).SetEase(Ease.InQuad);
            cardRect.DOSizeDelta(cardRect.sizeDelta * shrinkScale, duration).SetEase(Ease.InQuad);
        }

        yield return new WaitForSeconds(duration);

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

    public void CreateSpellMissile(Chess sourceChess, Vector3 targetPos, float time, float speed, float size, int skillId, int damage, string effectName, System.Func<Chess, float> perTargetDamageMulti = null, System.Action<Chess> perTargetOnHit = null)
    {
        // 首先加载导弹预制体
        Missile missilePrefab = Resources.Load<Missile>("Prefabs/MissileCom");
        
        // 实例化导弹
        var missile = Instantiate<Missile>(missilePrefab, sourceChess.transform.position, Quaternion.identity, Units.transform);
        missile.Init(sourceChess, size, effectName);
        missile.SetSkillInfo(skillId, damage);
        missile.perTargetDamageMulti = perTargetDamageMulti;
        missile.perTargetOnHit = perTargetOnHit;
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

    // 检查目标位置是否可用(不会被墙阻挡)：单位间不再锁格子，由互斥力控制间距；冲刺/镜像类技能落点校验用
    public bool TryLockGridPositions(Chess unit, Vector3 targetPosition, out List<Vector2Int> requiredGrids)
    {
        // 获取单位包围盒
        var collider = unit.GetComponent<Collider>();

        // 使用GetOccupiedGrids方法获取需要锁定的格子列表
        requiredGrids = GetOccupiedGrids(targetPosition, collider, true);

        // 只要落点不踩进墙就算可用
        foreach (var gridPos in requiredGrids)
        {
            if (wallGrids.Contains(gridPos))
                return false;
        }
        return true;
    }

    public bool MoveTo(Chess unit, Vector3 targetPosition, bool isForce = false)
    {
        // 单位移动已改为转向制(短程寻路+互斥力)，不再锁格子；技能强制位移直接落位
        unit.transform.position = targetPosition;
        return true;
    }

    // 获取指定位置和碰撞体占据的所有格子
    // clampUnit=true 用于单位移动/技能落点：阻挡半径上限2格(6m)再除3，即最多占3x3格(9m)，
    // 避免英雄(碰撞体10m)占5x5格(15m)比布阵格间距(13m)还大，导致两单位稍近就互卡无法移动；墙等静态障碍不收缩
    public List<Vector2Int> GetOccupiedGrids(Vector3 position, Collider collider, bool clampUnit = false)
    {
        List<Vector2Int> occupiedGrids = new List<Vector2Int>();

        // 获取碰撞体边界
        Vector3 boundsSize = collider.bounds.size;
        Vector3 halfBounds = boundsSize / 3f;
        if (clampUnit)
        {
            halfBounds = new Vector3(
                Mathf.Min(halfBounds.x, gridCellSize * 2f / 3f),
                Mathf.Min(halfBounds.y, gridCellSize * 2f / 3f),
                Mathf.Min(halfBounds.z, gridCellSize * 2f / 3f));
        }

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

    // ---- 短程寻路(单位转向制移动用：墙为唯一障碍，单位间用互斥力控制间距) ----

    /// <summary>格子是否为寻路障碍(墙膨胀2格)</summary>
    public bool IsPathBlockedGrid(Vector2Int grid)
    {
        return pathBlockedGrids.Contains(grid);
    }

    /// <summary>校验某位置的单位阻挡范围是否会踩进墙(静态障碍)，寻路失效时的兜底</summary>
    public bool CheckPositionBlocked(Chess unit, Vector3 position)
    {
        var collider = unit.GetComponent<Collider>();
        if (collider == null)
            return false;
        foreach (var gridPos in GetOccupiedGrids(position, collider, true))
        {
            if (wallGrids.Contains(gridPos))
                return true;
        }
        return false;
    }

    /// <summary>短程寻路：从 from 到 to 绕过墙取下一个途经点(格子中心)；直线通畅返回null(直接朝目标走)</summary>
    /// <remarks>目标太远时在搜索预算内取"最接近目标的一步"返回，保证持续朝目标推进；被墙围死返回null</remarks>
    public Vector3? FindMoveWaypoint(Vector3 from, Vector3 to)
    {
        int cell = gridCellSize;
        Vector2Int start = WorldToGridPosition(from, true);
        Vector2Int goal = WorldToGridPosition(to, true);
        if (start == goal)
            return null;
        // 直线通畅直接直行
        if (IsLineClear(start, goal))
            return null;

        // BFS(4邻域)：预算内优先到达目标；到不了则记录最接近目标的可达格子作为推进方向
        Queue<(Vector2Int node, int depth)> queue = new Queue<(Vector2Int, int)>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int> { [start] = start };
        queue.Enqueue((start, 0));

        Vector2Int bestNode = start;
        int bestDist = ManhattanDist(start, goal);

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();
            if (node == goal)
            {
                bestNode = node;
                break;
            }
            if (depth >= CombatConst.MovePathMaxDepth)
                continue; // 超出搜索深度不再扩展
            TryNeighbor(queue, parent, node, node.x + cell, node.y, depth, goal, ref bestNode, ref bestDist);
            TryNeighbor(queue, parent, node, node.x - cell, node.y, depth, goal, ref bestNode, ref bestDist);
            TryNeighbor(queue, parent, node, node.x, node.y + cell, depth, goal, ref bestNode, ref bestDist);
            TryNeighbor(queue, parent, node, node.x, node.y - cell, depth, goal, ref bestNode, ref bestDist);
        }

        if (bestNode == start)
            return null; // 一步都走不了(被墙围死)

        // 回溯到起点，返回起点后的第一个格子中心
        Vector2Int cur = bestNode;
        while (parent[cur] != start)
        {
            cur = parent[cur];
            if (cur == start)
                return null;
        }
        return new Vector3(cur.x, 0f, cur.y);
    }

    // BFS 尝试扩展一个邻格
    private void TryNeighbor(Queue<(Vector2Int, int)> queue, Dictionary<Vector2Int, Vector2Int> parent, Vector2Int node,
        int nx, int nz, int depth, Vector2Int goal, ref Vector2Int bestNode, ref int bestDist)
    {
        Vector2Int nb = new Vector2Int(nx, nz);
        if (parent.ContainsKey(nb) || IsPathBlockedGrid(nb))
            return;
        parent[nb] = node;
        int d = ManhattanDist(nb, goal);
        if (d < bestDist)
        {
            bestDist = d;
            bestNode = nb;
        }
        queue.Enqueue((nb, depth + 1));
    }

    // 起点到终点连线经过的格子是否都通畅(无寻路障碍)
    private bool IsLineClear(Vector2Int from, Vector2Int to)
    {
        int x0 = from.x, y0 = from.y;
        int dx = Mathf.Abs(to.x - x0), dy = Mathf.Abs(to.y - y0);
        int sx = x0 < to.x ? gridCellSize : -gridCellSize;
        int sy = y0 < to.y ? gridCellSize : -gridCellSize;
        int err = dx - dy;
        while (true)
        {
            if (IsPathBlockedGrid(new Vector2Int(x0, y0)))
                return false;
            if (x0 == to.x && y0 == to.y)
                return true;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }

    // 格子间曼哈顿距离(米)
    private int ManhattanDist(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    /// <summary>当前战斗地图规则（MapConfig.Rule）：0=1v1配对 1=两大阵营 2=混战；无地图时为 -1</summary>
    public int MapRule
    {
        get
        {
            if (mapInfo == null)
                return -1;
            var cfg = MapConfig.GetConfig(mapInfo.Mapid);
            return cfg != null ? cfg.Rule : -1;
        }
    }

    public bool IsEnemy(int a, int b)
    {
        if (isPveRound)
        {
            // PVE：只有配对的(玩家奇数号位,怪物偶数号位)互为敌人，各对之间互不干扰
            return a != b && (a + 1) / 2 == (b + 1) / 2;
        }
        if (MapConfig.GetConfig(mapInfo.Mapid).Rule == 1)
        {
            // 阵营1、3、4为一个阵营，阵营2、5、6为另一个阵营
            bool isTeam1 = a == 1 || a == 3 || a == 5 || a == 7;
            bool isTeam2 = a == 2 || a == 4 || a == 6 || a == 8;
            bool targetIsTeam1 = b == 1 || b == 3 || b == 5 || b == 7;
            bool targetIsTeam2 = b == 2 || b == 4 || b == 6 || b == 8;

            // 不同阵营之间是敌人
            return (isTeam1 && targetIsTeam2) || (isTeam2 && targetIsTeam1);
        }
        else if (MapConfig.GetConfig(mapInfo.Mapid).Rule == 0)
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
                int pid = match[pairIdx];
                if (pid < 0)
                {
                    GameLog.Warn($"PVE怪物死亡：{pairIdx + 1}号配对没有玩家，跳过掉落");
                }
                else
                {
                    var p = GameManager.Instance.GetPlayer(pid);
                    var drops = new List<int>();
                    if (SoldierConfig.GetConfig(dieUnit.soldierId).IsDrop)
                    {
                        int dropItemId = RollDropItem(GameManager.Instance.year);
                        if (dropItemId > 0)
                            drops.Add(dropItemId);
                    }
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
            if (match[i] < 0)
                continue;
            bool isWin = sideHasUnits[i * 2];
            GameManager.Instance.GetPlayer(match[i]).onBattleResult(isWin, 0);
        }

        gameFinish = true;
        // 人类玩家胜负按其实际参战号位判定（淘汰玩家 battleSide=0 记为失败）
        int p0Side = GameManager.Instance.GetPlayer(0).battleSide;
        hasWin = p0Side >= 1 && sideHasUnits[p0Side - 1];
    }

    // PVE：从 ItemConfig 掉落池加权随机一个道具，返回 0 表示无可用掉落。
    // 掉落池 = DropWeight > 0 且 DropStartRound <= 当前回合 的道具；权重为相对权重（30 的概率是 10 的 3 倍）。
    // 掉落解析属战斗/掉落业务逻辑，不放配置类（配置类仅保留数据定义与基础方法）
    public static int RollDropItem(int round)
    {
        var entries = new List<KeyValuePair<int, int>>();
        int totalWeight = 0;
        foreach (var itemCfg in ItemConfig.ConfigList)
        {
            if (itemCfg.DropWeight <= 0 || itemCfg.DropStartRound > round)
                continue;
            entries.Add(new KeyValuePair<int, int>(itemCfg.Id, itemCfg.DropWeight));
            totalWeight += itemCfg.DropWeight;
        }
        if (entries.Count == 0)
            return 0;
        int roll = SysRandom.Range(0, totalWeight);
        int acc = 0;
        foreach (var entry in entries)
        {
            acc += entry.Value;
            if (roll < acc)
                return entry.Key;
        }
        return 0;
    }

    // PVE：未参战玩家没有战斗过程，战斗结束一起随机获得装备（模拟一轮怪物掉落）
    private void GivePveIdleDrops()
    {
        if (pveMonsterSpawns == null || pveMonsterSpawns.Count == 0)
            return;
        int[] match = GetMatch();
        for (int i = pveFightCount; i < match.Length; i++)
        {
            if (match[i] < 0)
                continue;
            var p = GameManager.Instance.GetPlayer(match[i]);
            foreach (var spawn in pveMonsterSpawns)
            {
                if (!SoldierConfig.GetConfig(spawn.Item1).IsDrop)
                    continue;
                int itemId = RollDropItem(GameManager.Instance.year);
                if (itemId <= 0)
                    continue;
                p.AddItemCard(itemId);
                var itemName = ItemConfig.HasConfig(itemId) ? ItemConfig.GetConfig(itemId).Name : itemId.ToString();
                GameLog.Debug($"[PVE] 未参战玩家{p.pid} 随机获得 {itemName}({itemId})");
            }
        }
    }

    // 存活玩家≤1：无对手可匹配，直接给存活玩家判定胜利（不产生战斗过程，但仍然走正常结算/扣血流程）
    private void ResolveBattleWithoutOpponent()
    {
        int[] match = GetMatch();
        int p0Side = GameManager.Instance.GetPlayer(0).battleSide;
        for (int i = 0; i < match.Length; i++)
        {
            int pid = match[i];
            if (pid < 0)
                continue;
            killMark[pid] = 10;
            GameManager.Instance.GetPlayer(pid).onBattleResult(true, 10);
        }
        hasWin = p0Side >= 1;
        gameFinish = true;
        GameLog.Info($"匹配无对手（存活玩家≤1），直接判定存活玩家获胜，共{GameManager.Instance.GetAlivePlayerIds().Count}人");
    }

    public void OnUnitDying(Chess dieUnit, int killerPlayerId)
    {
        if(killerPlayerId >= 0 && killerPlayerId < killMark.Length && dieUnit.isHero)
            killMark[killerPlayerId]++;
        // 从chessList中移除死亡单位
        chessList.Remove(dieUnit);

        // 本场已判定结束：同一 tick 内其余单位阵亡不再重复结算（避免重复扣血/重复计分）
        if (gameFinish)
            return;

        // 注意：这里绝不能重置 gameFinish/hasWin。同一 tick 内先后有多个单位阵亡时，
        // 后一次死亡会把前一次已判定的结束标记抹掉；而该次死亡后的存活情况可能不再满足结束条件，
        // 导致战斗永不结束、主循环 while(!gameFinish) 空转卡死（全灭后无任何日志，界面看起来永远不动）。
        // gameFinish/hasWin 仅在本场 BattleBegin 时重置。
        RecomputeSideBattleOver(killerPlayerId); // 单位阵亡后重算各阵营"战斗是否已结束"（无存活敌方即结束）
        if (isPveRound)
        {
            HandlePveUnitDying(dieUnit, killerPlayerId);
            return;
        }
        if (MapConfig.GetConfig(mapInfo.Mapid).Rule == 0)
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
            // 地图规则0：独立配对（1-2/3-4/5-6/7-8），每个配对分出胜负即该配对结束。
            // "所有配对都已分出胜负" ⇔ 存活阵营之间不存在任何互为敌人的组合。
            // 不能写死 aliveSideCount==4：参战阵营数不足 8（如模拟/调试战斗只有 2 方）、或某配对双方同刻同灭时
            // 该条件永远不满足，战斗永不结束 → 主循环空转卡死。
            bool anyFightLeft = false;
            for (int a = 1; a <= sideHasUnits.Length && !anyFightLeft; a++)
            {
                if (!sideHasUnits[a - 1])
                    continue;
                for (int b = a + 1; b <= sideHasUnits.Length; b++)
                {
                    if (sideHasUnits[b - 1] && IsEnemy(a, b))
                    {
                        anyFightLeft = true;
                        break;
                    }
                }
            }
            if (!anyFightLeft)
            {
                int[] match = GetMatch();
                for (int i = 0; i < match.Length; i++)
                {
                    int pid = match[i];
                    if (pid < 0)
                        continue;
                    if (sideHasUnits[i])
                        killMark[pid] = 10;
                    else
                        killMark[pid] = Math.Min(5, killMark[pid]);

                    GameManager.Instance.GetPlayer(pid).onBattleResult(sideHasUnits[i], killMark[pid]);
                }
                gameFinish = true;
                // 人类玩家的胜负按其实际参战号位判定（人类可能已淘汰，battleSide=0 时记为失败）
                int p0Side = GameManager.Instance.GetPlayer(0).battleSide;
                hasWin = p0Side >= 1 && sideHasUnits[p0Side - 1];
            }
        }
        else if (MapConfig.GetConfig(mapInfo.Mapid).Rule == 1)
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
                    int pid = match[i];
                    if (pid < 0)
                        continue;
                    int playerSide = i + 1; // 假设match索引对应阵营1-6
                    bool isTeam1 = playerSide == 1 || playerSide == 3 || playerSide == 5 || playerSide == 7;
                    bool isWinner = (isTeam1 && team1HasUnits) || (!isTeam1 && team2HasUnits);

                    if (isWinner)
                        killMark[pid] = 10;
                    else
                        killMark[pid] = Math.Min(5, killMark[pid]);

                    GameManager.Instance.GetPlayer(pid).onBattleResult(isWinner, killMark[pid]);
                }
                gameFinish = true;
                // 人类玩家胜负按其实际参战号位所在的队伍判定（淘汰玩家 battleSide=0 记为失败）
                int p0Side = GameManager.Instance.GetPlayer(0).battleSide;
                bool p0Team1 = p0Side == 1 || p0Side == 3 || p0Side == 5 || p0Side == 7;
                hasWin = p0Side >= 1 && (p0Team1 ? team1HasUnits : team2HasUnits);
            }
        }
       else if (MapConfig.GetConfig(mapInfo.Mapid).Rule == 2)
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

            // 只剩一个阵营有存活单位（或同刻双方全灭，aliveSideCount==0）即结束，计算分数
            if (aliveSideCount <= 1)
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
                    if (playerId < 0)
                        continue;
                    int playerSide = i + 1; // 假设match索引对应阵营1-6
                    bool isWinner = (playerSide == winnerSide);

                    killMark[playerId] = scores[i];

                    // 混战地图（Rule=2）按被淘汰顺序扣血：前4被淘汰扣全额，第5~6扣一半，第7扣三分之一（向上取整）
                    int damage = -1;
                    if (!isWinner)
                    {
                        int baseDamage = 0;
                        int round = Mathf.Clamp(GameManager.Instance.year, 1, 100);
                        if (GameRoundConfig.HasConfig(round))
                            baseDamage = GameRoundConfig.GetConfig(round).DamageCount;
                        else
                            GameLog.Error($"混战扣血：GameRoundConfig 缺少回合{round}配置，扣血为0");

                        int order = deathOrder[i];
                        if (order <= 4)
                            damage = baseDamage;
                        else if (order <= 6)
                            damage = Mathf.CeilToInt(baseDamage / 2f);
                        else
                            damage = Mathf.CeilToInt(baseDamage / 3f);
                    }

                    GameManager.Instance.GetPlayer(playerId).onBattleResult(isWinner, killMark[playerId], damage);
                }

                gameFinish = true;
                // 人类玩家胜负按其实际参战号位判定（淘汰玩家 battleSide=0 记为失败）
                int p0Side = GameManager.Instance.GetPlayer(0).battleSide;
                hasWin = p0Side >= 1 && sideHasUnits[p0Side - 1];
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


    /// <summary>
    /// 获取指定范围内的敌方存活单位(排除影子)
    /// </summary>
    public List<Chess> GetEnemyInRange(Vector3 wPos, float range, int mySide)
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
                    if (IsEnemy(chessComponent.side, mySide))
                        unitsInRange.Add(chessComponent);
                }
            }
        }

        return unitsInRange;
    }

    /// <summary>
    /// 获取所有敌方存活英雄：优先返回与自身配对((a+1)/2 == (b+1)/2)的敌方英雄，
    /// 若不存在配对敌人则返回全部敌方英雄
    /// </summary>
    public List<Chess> GetAllEnemys(int mySide)
    {
        var result = new List<Chess>();
        var paired = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow && chessComponent.isHero)
            {
                if (!IsEnemy(chessComponent.side, mySide))
                    continue;
                result.Add(chessComponent);
                if ((mySide + 1) / 2 == (chessComponent.side + 1) / 2)
                    paired.Add(chessComponent);
            }
        }
        return paired.Count > 0 ? paired : result;
    }

    /// <summary>
    /// 获取指定范围内所有带指定 SummonTag 的召唤物（不分敌我，按召唤物标签识别，如"火"/"雷"）
    /// </summary>
    public List<Chess> GetUnitsInRangeByTag(Vector3 wPos, float range, string summonTag)
    {
        var result = new List<Chess>();
        if (string.IsNullOrEmpty(summonTag))
            return result;

        Vector2Int center = WorldManager.Instance.WorldToGridPosition(wPos, true);
        foreach (var chessComponent in chessList)
        {
            if (chessComponent == null || chessComponent.hp <= 0 || chessComponent.isShadow || chessComponent.SummonTag != summonTag)
                continue;

            Vector2Int chessPos = WorldToGridPosition(chessComponent.transform.position, true);
            if (Vector2Int.Distance(center, chessPos) <= range || range == 0)
                result.Add(chessComponent);
        }

        return result;
    }

    /// <summary>
    /// 获取指定范围内的所有存活单位(不分敌我，排除影子)，供单位间距松弛等需要全邻居的场景
    /// </summary>
    public List<Chess> GetUnitsInRangeAll(Vector3 wPos, float range)
    {
        Vector2Int center = WorldToGridPosition(wPos, true);
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent == null || chessComponent.hp <= 0 || chessComponent.isShadow)
                continue;

            Vector2Int chessPos = WorldToGridPosition(chessComponent.transform.position, true);
            if (range == 0 || Vector2Int.Distance(center, chessPos) <= range)
                unitsInRange.Add(chessComponent);
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

    /// <summary>
    /// 获取指定范围内的同侧(同 battleSide)存活单位(排除影子)
    /// </summary>
    public List<Chess> GetMySideInRange(Vector3 wPos, float range, int mySide)
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

    /// <summary>
    /// 该阵营的战斗是否已结束：整体战斗已结束，或该方已无存活敌方
    /// （如 地图规则0 的四个独立配对中对手先被全灭、其他配对仍在打）。
    /// 战斗结束的一方不再回血/回蓝，也不再承受疲劳伤害（冻结该方）。
    /// </summary>
    public bool IsSideBattleOver(int side)
    {
        if (gameFinish)
            return true;
        if (side < 1 || side > sideBattleOver.Length)
            return false;
        return sideBattleOver[side - 1];
    }

    // 重算各阵营战斗结束状态（单位阵亡移除后调用）：某阵营已无存活敌方即视为战斗结束；
    // 本场参战过且已无存活单位的阵营视为战败，触发城堡燃烧/消融与HUD置灰（每方只处理一次）
    private void RecomputeSideBattleOver(int killerPlayerId)
    {
        for (int side = 1; side <= sideBattleOver.Length; side++)
        {
            bool hasEnemy = false;
            bool hasSelf = false;
            foreach (var chessComponent in chessList)
            {
                if (chessComponent == null || chessComponent.hp <= 0 || chessComponent.isShadow)
                    continue;
                if (chessComponent.side == side)
                    hasSelf = true;
                if (IsEnemy(side, chessComponent.side))
                    hasEnemy = true;
                if (hasSelf && hasEnemy)
                    break;
            }
            sideBattleOver[side - 1] = !hasEnemy;

            if (sideHadUnits[side - 1] && !hasSelf && !sideDefeated[side - 1])
            {
                sideDefeated[side - 1] = true;
                HandleSideDefeated(side, killerPlayerId);
            }
        }
    }

    // 某阵营的全部（存活）单位血量合计与血量上限合计：城堡HUD血条用
    public void GetSideHpSum(int side, out int curHp, out int maxHp)
    {
        curHp = 0;
        maxHp = 0;
        foreach (var chessComponent in chessList)
        {
            if (chessComponent == null || chessComponent.isShadow || chessComponent.side != side)
                continue;
            maxHp += chessComponent.maxHp;
            curHp += Mathf.Max(0, chessComponent.hp);
        }
    }

    // 某阵营战败：城堡处播放火焰特效，延迟后城堡消融移除；该阵营玩家的HUD显示失败并置灰
    private void HandleSideDefeated(int side, int killerPlayerId)
    {
        GameLog.Debug($"阵营 {side} 战败，城堡开始燃烧");
        GameObject castle = GetCastle(side);
        if (castle != null)
        {
            EffectManager.PlayPosSkillEffect(castle.transform.position, 0f, CastleHUD.CastleBurnEffect, CastleHUD.CastleBurnEffectTime);
            StartCoroutine(DissolveCastleCoroutine(castle));
        }

        PlayerInfo player = FindPlayerBySide(side);
        if (player != null && player.castleHUD != null)
            player.castleHUD.SetDefeated();

        ShowDefeatTip(side, killerPlayerId);
    }

    // 一方全灭时增加一组击败提示：胜者=最后一击击杀者（无击杀归属时取该方任一存活敌方），败者=被消灭的阵营
    private void ShowDefeatTip(int defeatedSide, int killerPlayerId)
    {
        if (battleDefeatTip == null)
            return;

        // 败者：被消灭阵营对应的玩家；怪物阵营（PVE/单数匹配野怪）没有 PlayerInfo，用虚拟玩家999
        PlayerInfo loser = FindPlayerBySide(defeatedSide);
        int loserPid;
        if (loser != null)
            loserPid = loser.pid;
        else if (IsMonsterSide(defeatedSide))
            loserPid = PlayerBook.MonsterPlayerId;
        else
            loserPid = -1;

        // 胜者：优先取击杀者；无归属（如疲劳/持续伤害致死）时取任一存活敌方
        int winnerPid = killerPlayerId;
        if (winnerPid < 0)
            winnerPid = FindAliveEnemyPid(defeatedSide);

        Sprite winIcon = LoadPlayerIcon(winnerPid);
        Sprite loseIcon = LoadPlayerIcon(loserPid);
        if (winIcon == null && loseIcon == null)
        {
            GameLog.Warn($"击败提示跳过：阵营{defeatedSide}取不到胜/败者头像（胜者pid={winnerPid}，败者pid={loserPid}）");
            return;
        }

        battleDefeatTip.AddDefeatTip(winIcon, loseIcon);
    }

    // 取某阵营的存活敌方玩家pid（PVE怪物阵营用虚拟玩家999）
    private int FindAliveEnemyPid(int side)
    {
        foreach (var chessComponent in chessList)
        {
            if (chessComponent == null || chessComponent.hp <= 0 || chessComponent.isShadow)
                continue;
            if (!IsEnemy(side, chessComponent.side))
                continue;

            PlayerInfo player = FindPlayerBySide(chessComponent.side);
            if (player != null)
                return player.pid;
            if (IsMonsterSide(chessComponent.side))
                return PlayerBook.MonsterPlayerId;
        }
        return -1;
    }

    // 判断某号位是否为怪物阵营（PVE怪物号位 / 单数匹配刷出的野怪号位）
    private bool IsMonsterSide(int side)
    {
        if (oddMonsterSide > 0 && side == oddMonsterSide)
            return true;
        return isPveRound && side % 2 == 0;
    }

    // 按玩家pid取头像：普通玩家走 PlayerInfo.imgPath，虚拟玩家（PVE怪物）走 PlayerConfig
    private Sprite LoadPlayerIcon(int pid)
    {
        if (pid < 0 || GameManager.Instance == null)
            return null;

        if (pid < GameManager.Instance.players.Length)
        {
            PlayerInfo player = GameManager.Instance.GetPlayer(pid);
            return player != null ? Resources.Load<Sprite>(player.imgPath) : null;
        }

        if (!PlayerConfig.HasConfig(pid))
            return null;

        string imgpath = PlayerConfig.GetConfig(pid).Imgpath;
        return string.IsNullOrEmpty(imgpath) ? null : Resources.Load<Sprite>("Textures/" + imgpath);
    }

    // 城堡战败表现：延迟 CastleBurnDelay 后把渲染器切到消融材质，按时间推进消融量，结束后隐藏城堡
    private IEnumerator DissolveCastleCoroutine(GameObject castle)
    {
        int version = battleVersion;
        yield return new WaitForSeconds(CastleHUD.CastleBurnDelay);
        if (castle == null || version != battleVersion)
            yield break;

        var shader = Resources.Load<Shader>("Shaders/CastleDissolve");
        if (shader == null)
        {
            GameLog.Error("城堡消融失败：未找到 Resources/Shaders/CastleDissolve 着色器，直接移除城堡");
            castle.SetActive(false);
            yield break;
        }

        var renderers = castle.GetComponentsInChildren<Renderer>();
        var dissolveMats = new List<Material>();
        foreach (var rend in renderers)
        {
            if (rend == null)
                continue;

            var srcMats = rend.sharedMaterials;
            if (!castleOriginalMats.ContainsKey(rend))
                castleOriginalMats[rend] = srcMats;

            var mats = new Material[srcMats.Length];
            for (int i = 0; i < srcMats.Length; i++)
            {
                var src = srcMats[i];
                var mat = new Material(shader);
                if (src != null)
                {
                    if (src.HasProperty("_MainTex") && mat.HasProperty("_MainTex"))
                        mat.SetTexture("_MainTex", src.GetTexture("_MainTex"));
                    if (src.HasProperty("_BumpMap") && mat.HasProperty("_BumpMap"))
                        mat.SetTexture("_BumpMap", src.GetTexture("_BumpMap"));
                    if (src.HasProperty("_Color") && mat.HasProperty("_Color"))
                        mat.SetColor("_Color", src.GetColor("_Color"));
                }
                mats[i] = mat;
                dissolveMats.Add(mat);
            }
            rend.sharedMaterials = mats;
        }

        float time = 0f;
        while (time < CastleHUD.CastleDissolveTime && castle != null && version == battleVersion)
        {
            time += Time.deltaTime;
            float amount = Mathf.Clamp01(time / CastleHUD.CastleDissolveTime);
            foreach (var mat in dissolveMats)
            {
                if (mat != null)
                    mat.SetFloat("_DissolveAmount", amount);
            }
            yield return null;
        }

        if (castle != null && version == battleVersion)
        {
            castle.SetActive(false);
            hiddenCastles.Add(castle);
        }
        foreach (var mat in dissolveMats)
        {
            if (mat != null)
                Destroy(mat);
        }
    }

    // 新一场开始前恢复上一场被消融移除的城堡（同地图复用不重新加载地图）
    private void ResetCastles()
    {
        foreach (var kv in castleOriginalMats)
        {
            if (kv.Key != null)
                kv.Key.sharedMaterials = kv.Value;
        }
        castleOriginalMats.Clear();

        foreach (var castle in hiddenCastles)
        {
            if (castle != null)
                castle.SetActive(true);
        }
        hiddenCastles.Clear();
    }

    // 取阵营对应城堡（Castles 顺序与 SideCenters/阵营编号一致）
    private GameObject GetCastle(int side)
    {
        if (mapInfo == null || mapInfo.Castles == null)
            return null;
        int index = side - 1;
        if (index < 0 || index >= mapInfo.Castles.Length)
            return null;
        return mapInfo.Castles[index];
    }

    // 按阵营号取玩家（battleSide 与阵营编号一致）
    private PlayerInfo FindPlayerBySide(int side)
    {
        var players = GameManager.Instance != null ? GameManager.Instance.players : null;
        if (players == null)
            return null;
        foreach (var player in players)
        {
            if (player != null && player.battleSide == side)
                return player;
        }
        return null;
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

}
