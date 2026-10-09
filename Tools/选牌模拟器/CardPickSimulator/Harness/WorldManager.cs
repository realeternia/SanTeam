// ============================================================
// 战斗模拟器 · Harness —— WorldManager 无头替身
// 复刻游戏战斗层所依赖的全部对外 API（签名与原版 WorldManager 一致），
// 2 玩家对战（TeamMode2 语义：任意不同 side 即敌人）。
// 墙/地图/城堡/怪物/PVE 均不实现；主循环由 Sim 驱动，不启动 GameUpdate 协程。
// ============================================================
using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    public int gridCellSize = 3;
    public GameObject Units;
    public GameObject HudNode;
    public GameObject BattleTextNode;
    public int idCounter;

    // UI 摄像机（CardViewControl 飞卡动画引用）
    public Camera uiCamera;

    // 选牌模拟器：跳过真正的战斗生成/开战，仅把 BattleBegin 当作"本商店回合结束"的信号。
    // Combat 层源码仍需 WorldManager 的完整方法集才能编译，故保留其余实现。
    public static bool SkipBattle = false;
    public static int BattleBeginCount = 0;

    public List<Chess> chessList = new List<Chess>();
    public bool gameFinish;
    public bool hasWin;
    public bool isDraw;   // 同刻双方全灭导致的平局（与打到步数上限的平局区分）
    private bool[] sideBattleOver = new bool[8]; // 各阵营战斗是否已结束（无存活敌方）

    // 羁绊开关（默认全开，仅批量模拟器按需在开战前关闭；不影响战斗逻辑本身）
    public static bool EnableJobLinks = true;        // 职业连锁
    public static bool EnableFriendLines = true;     // 好友连线（默认连线 + 特殊连锁）
    public static bool EnableFactionShields = true;  // 国家护盾

    // 飘字数据（供 GDI+ 主窗体绘制，无特效）
    public class BattleTextData
    {
        public string text;
        public Vector3 worldPos;
        public Color color;
        public float expireTime;
    }
    public List<BattleTextData> battleTexts = new List<BattleTextData>();

    // 技能施放飘字数据（供 GDI+ 主窗体绘制技能名与右侧日志，无特效）
    public class SkillFxData
    {
        public Vector3 pos;
        public string name;
        public float time;
        public int side;
        public string heroName;
    }
    public readonly List<SkillFxData> SkillFx = new List<SkillFxData>();

    private Transform _center1;
    private Transform _center2;

    // 建立两侧布阵中心：side1 (0,0,-30) 朝 +z，side2 (0,0,30) 朝 -z（互为镜像）
    public void SetupCenters()
    {
        if (Units == null) Units = new GameObject("Units");
        if (HudNode == null) HudNode = new GameObject("HudNode");
        if (BattleTextNode == null) BattleTextNode = new GameObject("BattleTextNode");

        var go1 = new GameObject("Side1Center", new Vector3(0f, 0f, -30f), Quaternion.identity);
        _center1 = go1.transform;
        var go2 = new GameObject("Side2Center", new Vector3(0f, 0f, 30f), Quaternion.Euler(0f, 180f, 0f));
        _center2 = go2.transform;
    }

    // ================= 布阵与单位生成 =================

    // 按布阵面板生成两侧单位：士兵（battleCards 中 500001/500002）+ 英雄
    public void SpawnUnitsInRegions()
    {
        var players = GameManager.Instance.players;
        if (players == null || players.Length == 0)
        {
            GameLog.Error("WorldManager.SpawnUnitsInRegions: players 为空");
            return;
        }
        for (int i = 0; i < players.Length; i++)
            players[i].battleSide = i + 1;

        for (int side = 1; side <= players.Length; side++)
        {
            var p = players[side - 1];
            var center = side == 1 ? _center1 : _center2;
            SpawnSoldiersForSide(p, center, side);
            SpawnHerosForSide(p, center, p.GetBattleCardList(), side);
            SpawnGongSummonForSide(p, center, side);
        }
    }

    // 计算布阵图中第pos格(0~24)的世界坐标：row=pos/5 从上到下，col=pos%5 从左到右
    private Vector3 GetFormationCellPos(Transform center, int pos)
    {
        int row = pos / CombatConst.FormationGridSize;
        int col = pos % CombatConst.FormationGridSize;
        Vector3 forward = Vector3.ProjectOnPlane(center.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(center.right, Vector3.up).normalized;
        float half = (CombatConst.FormationGridSize - 1) * 0.5f;
        return center.position + forward * (row - half) * 13f + right * (col - half) * 13f;
    }

    // 返回本侧布阵格 pos 按"离敌方由近到远"（前→后）排序：
    // 用两侧 center 的世界坐标算距离动态判定前后（本 harness 中 center.forward 指向敌方，
    // 即 row 越大越靠前；与游戏内 row0=最前的约定相反，故不写死 row 方向）。
    public List<int> GetFormationPosOrderFrontToBack(int side)
    {
        var result = new List<int>();
        for (int pos = 0; pos < CombatConst.FormationCellCount; pos++)
            result.Add(pos);

        var myCenter = side == 1 ? _center1 : _center2;
        var foeCenter = side == 1 ? _center2 : _center1;
        if (myCenter == null || foeCenter == null)
            return result;

        result.Sort((x, y) =>
        {
            float dx = Vector3.Distance(GetFormationCellPos(myCenter, x), foeCenter.position);
            float dy = Vector3.Distance(GetFormationCellPos(myCenter, y), foeCenter.position);
            int c = dx.CompareTo(dy);
            return c != 0 ? c : x.CompareTo(y);   // 同距按 pos 保证确定性
        });
        return result;
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
    private void SpawnHerosForSide(PlayerInfo p, Transform center, List<Tuple<int, int>> cards, int side)
    {
        for (int i = 0; i < cards.Count && i < CombatConst.FormationCellCount; i++)
        {
            if (cards[i] == null || !ConfigManager.IsHeroCard(cards[i].Item1))
                continue;
            SpawnHerosForRegion(p, i, GetFormationCellPos(center, i), cards[i], side);
        }
    }

    // 工·机巧召唤（复刻游戏源码 WorldManager.SpawnGongSummonForSide）：
    // 按本侧上阵"工"职业英雄数(1~5)定档，在布阵空格刷召唤单位（近战前排/远程后排）。
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

        var occupied = new HashSet<int>();
        foreach (var u in units)
            if (u != null && u.pos >= 0 && u.pos < CombatConst.FormationCellCount)
                occupied.Add(u.pos);

        var spawnList = BuildGongSummonList(lv);
        var img = p != null ? p.imgPath : "";
        foreach (var item in spawnList)
        {
            var cfg = SoldierConfig.GetConfig(item.Item1);
            // 近战(木牛流马/辅助)站前排row0~1，远程(喷火兽)站后排row3~4
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

    // 工·机巧各档位(工英雄数1~5)召唤单位：木牛流马=肉盾(近战前排)，喷火兽=远程火DPS(后排)，辅助=加buff(近战)
    private static List<Tuple<int, int>> BuildGongSummonList(int lv)
    {
        var list = new List<Tuple<int, int>>();
        switch (lv)
        {
            case 1:
                list.Add(Tuple.Create(502001, 1)); // 木牛流马lv1
                break;
            case 2:
                list.Add(Tuple.Create(502002, 1)); // 木牛流马lv2
                break;
            case 3:
                list.Add(Tuple.Create(502002, 1)); // 木牛流马lv2
                list.Add(Tuple.Create(502003, 1)); // 喷火兽lv1
                break;
            case 4:
                list.Add(Tuple.Create(502002, 1)); // 木牛流马lv2
                list.Add(Tuple.Create(502005, 1)); // 喷火兽lv2
                break;
            case 5:
                list.Add(Tuple.Create(502002, 1)); // 木牛流马lv2
                list.Add(Tuple.Create(502005, 1)); // 喷火兽lv2
                list.Add(Tuple.Create(502004, 1)); // 机巧辅助
                break;
        }
        return list;
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

    // 生成士兵单位：从 SoldierConfig 取值逐字段赋值后 Init
    public Chess SpawnUnitsForRegion(PlayerInfo p, int soldierId, int posId, Vector3 spawnPos, int side, string imgPath)
    {
        var soldierConfig = SoldierConfig.GetConfig(soldierId);
        if (soldierConfig == null)
        {
            GameLog.Error("WorldManager.SpawnUnitsForRegion: 缺少 SoldierConfig id=" + soldierId);
            return null;
        }
        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/Battles/" + soldierConfig.Model);
        GameObject unitInstance = UnityEngine.Object.Instantiate(unitPrefab, spawnPos, Quaternion.identity, Units.transform);
        unitInstance.name = "UnitBing_" + side + "_" + idCounter;
        unitInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        var chessComponent = unitInstance.GetComponent<Chess>();
        if (chessComponent == null)
            chessComponent = unitInstance.AddComponent<Chess>();

        chessComponent.id = idCounter;
        chessComponent.isHero = false;
        chessComponent.side = side;
        chessComponent.chessName = imgPath;
        chessComponent.maxHp = soldierConfig.Hp;
        chessComponent.moveSpeed = soldierConfig.MoveSpeed;
        chessComponent.attackRange = soldierConfig.Range;
        chessComponent.atk = soldierConfig.Atk;
        chessComponent.attackSpeed = soldierConfig.AtkSpeed / 30f; // 攻速值→每秒攻击次数
        chessComponent.missileSpeed = soldierConfig.MissileSpeed;
        chessComponent.missileHight = soldierConfig.MissileHight;
        chessComponent.armor = soldierConfig.Armor;
        chessComponent.magicRes = soldierConfig.MagicRes;
        chessComponent.isFakeHero = soldierConfig.Model == "UnitHero";
        chessComponent.hitEffect = soldierConfig.HitEffect;
        chessComponent.soldierId = soldierId;
        chessComponent.playerId = p != null ? p.pid : 0;
        chessComponent.Init(p != null ? p.pid : 0, posId, p != null ? p.lineColor : Color.red);

        chessList.Add(chessComponent);
        idCounter++;
        return chessComponent;
    }

    // 生成英雄单位：职业基准导弹参数 + 面板初始化
    private Chess SpawnHerosForRegion(PlayerInfo p, int posId, Vector3 spawnPos, Tuple<int, int> heroData, int side)
    {
        var heroConfig = HeroConfig.GetConfig(heroData.Item1);
        if (heroConfig == null)
        {
            GameLog.Error("WorldManager.SpawnHerosForRegion: 缺少 HeroConfig id=" + heroData.Item1);
            return null;
        }
        GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/Battles/UnitHero");
        GameObject unitInstance = UnityEngine.Object.Instantiate(heroPrefab, spawnPos, Quaternion.identity, Units.transform);
        unitInstance.name = "Hero_" + side + "_" + idCounter;
        unitInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        var chessComponent = unitInstance.GetComponent<Chess>();
        if (chessComponent == null)
            chessComponent = unitInstance.AddComponent<Chess>();

        chessComponent.id = idCounter;
        chessComponent.isHero = true;
        chessComponent.heroId = (int)heroConfig.Id;
        chessComponent.side = side;
        chessComponent.chessName = heroConfig.Icon;
        // 导弹速度/高度、命中特效按职业基准从 JobConfig 取
        var jobCfg = ConfigManager.GetJobConfig(heroConfig.Job);
        chessComponent.hitEffect = jobCfg != null ? jobCfg.HitEffect : "";
        chessComponent.missileSpeed = jobCfg != null ? jobCfg.MissileSpeed : 0;
        chessComponent.missileHight = jobCfg != null ? jobCfg.MissileHight : 0f;

        if (side <= 2)
            chessComponent.heroInfo = new HeroInfo();
        chessComponent.playerId = p != null ? p.pid : 0;
        chessComponent.CheckInitAttr(p, heroData.Item2);
        chessComponent.Init(p != null ? p.pid : 0, posId, p != null ? p.lineColor : Color.red);

        chessList.Add(chessComponent);
        idCounter++;
        return chessComponent;
    }

    // ================= 战斗开始 =================

    // 复刻 WorldManager.BattleBegin 的后处理顺序（主循环由 Sim 驱动）
    public void BattleBegin()
    {
        BattleBeginCount++;

        // 选牌模拟器：不开战，仅作为"本商店回合结束"的信号返回
        if (SkipBattle)
        {
            gameFinish = true;
            hasWin = false;
            isDraw = false;
            return;
        }

        gameFinish = false;
        hasWin = false;
        isDraw = false;
        sideBattleOver = new bool[8]; // 各阵营战斗结束状态重置
        idCounter = 100;
        chessList.Clear();
        battleTexts.Clear();
        FatigueManager.Reset(); // 疲劳计时本场重新开始

        var players = GameManager.Instance.players;
        if (players == null)
        {
            GameLog.Error("WorldManager.BattleBegin: GameManager.Instance.players 为空");
            return;
        }
        // 复刻游戏启动时 GameManager 调用的 InitFriend：构建好友关系/特殊连锁字典（friendRdData 在 harness 为空，仅落地静态 HeroFriendConfig）
        ConfigManager.InitFriend();
        for (int i = 0; i < players.Length; i++)
            players[i].OnBattleBegin();

        SpawnUnitsInRegions();

        if (EnableJobLinks)
            JobLinkManager.ApplyJobLinks();
        if (EnableFriendLines)
            FriendLineManager.ApplyFriendSpecialSkills();
        if (EnableFactionShields)
            FactionShieldManager.ApplyFactionShields();

        foreach (var chess in chessList.ToArray())
        {
            if (chess != null)
                SkillManager.BattleBegin(chess);
        }
    }

    // ================= 查询/距离 =================

    // TeamMode2：任意不同 side 即敌人（1v1 对等）
    public bool IsEnemy(int a, int b)
    {
        return a != b;
    }

    // 世界坐标转格子坐标（格子间距 gridCellSize=3）
    public Vector2Int WorldToGridPosition(Vector3 worldPosition, bool FloorToInt)
    {
        int x, z;
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

    public bool CheckInRange(Vector3 pos1, Vector3 pos2, float range)
    {
        return GetRange(pos1, pos2) <= range;
    }

    public float GetRange(Vector3 pos1, Vector3 pos2)
    {
        Vector2Int pos1a = WorldToGridPosition(pos1, true);
        Vector2Int pos2a = WorldToGridPosition(pos2, true);
        return Vector2Int.Distance(pos1a, pos2a);
    }

    // 获取指定范围内的敌方存活单位(排除影子)
    public List<Chess> GetEnemyInRange(Vector3 wPos, float range, int mySide)
    {
        Vector2Int center = WorldToGridPosition(wPos, true);
        var unitsInRange = new List<Chess>();
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

    // 获取所有敌方存活英雄：优先返回与自身配对的敌方英雄（1v1 下即全部敌方英雄）
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

    // 获取指定范围内所有带指定 SummonTag 的召唤物（不分敌我）
    public List<Chess> GetUnitsInRangeByTag(Vector3 wPos, float range, string summonTag)
    {
        var result = new List<Chess>();
        if (string.IsNullOrEmpty(summonTag))
            return result;
        Vector2Int center = WorldToGridPosition(wPos, true);
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

    // 获取指定范围内的所有存活单位(不分敌我，排除影子)：供单位间距松弛等全邻居场景
    public List<Chess> GetUnitsInRangeAll(Vector3 wPos, float range)
    {
        Vector2Int center = WorldToGridPosition(wPos, true);
        var unitsInRange = new List<Chess>();
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

    // 随机保留 limit 个（用 SysRandom 保证可复现）
    public void RandomSelect(List<Chess> unitsInRange, int limit)
    {
        if (limit < 0)
            return;
        while (unitsInRange.Count > limit)
        {
            int indexToRemove = SysRandom.Range(0, unitsInRange.Count);
            unitsInRange.RemoveAt(indexToRemove);
        }
    }

    public List<Chess> GetMySideInRange(Vector3 wPos, float range, int mySide)
    {
        Vector2Int center = WorldToGridPosition(wPos, true);
        var unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                Vector2Int chessPos = WorldToGridPosition(chessComponent.transform.position, true);
                if (range == 0 || Vector2Int.Distance(center, chessPos) <= range)
                {
                    if (chessComponent.side == mySide)
                        unitsInRange.Add(chessComponent);
                }
            }
        }
        return unitsInRange;
    }

    public List<Chess> GetUnitsMySide(int mySide)
    {
        var unitsInRange = new List<Chess>();
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

    /// <summary>该阵营的战斗是否已结束：整体战斗已结束，或该方已无存活敌方（冻结：不回血回蓝、不吃疲劳）</summary>
    public bool IsSideBattleOver(int side)
    {
        if (gameFinish)
            return true;
        if (side < 1 || side > sideBattleOver.Length)
            return false;
        return sideBattleOver[side - 1];
    }

    // 重算各阵营战斗结束状态（单位阵亡移除后调用）：某阵营已无存活敌方即视为战斗结束
    private void RecomputeSideBattleOver()
    {
        for (int side = 1; side <= sideBattleOver.Length; side++)
        {
            bool hasEnemy = false;
            foreach (var chessComponent in chessList)
            {
                if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow
                    && IsEnemy(side, chessComponent.side))
                {
                    hasEnemy = true;
                    break;
                }
            }
            sideBattleOver[side - 1] = !hasEnemy;
        }
    }

    public Chess FindByHeroIdAndSide(int heroId, int side)
    {
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow
                && chessComponent.isHero && chessComponent.heroId == heroId && chessComponent.side == side)
                return chessComponent;
        }
        return null;
    }

    // ================= 导弹 =================

    public void CreateAttackMissile(Chess sourceChess, Chess targetChess, string effectName)
    {
        Missile missilePrefab = Resources.Load<Missile>("Prefabs/MissileCom");
        var missile = UnityEngine.Object.Instantiate<Missile>(missilePrefab, sourceChess.transform.position, Quaternion.identity, Units.transform);
        missile.Init(sourceChess, 1, effectName);
        missile.MoveToTarget(targetChess, sourceChess.missileSpeed, sourceChess.missileHight);
    }

    public void CreateSpellMissile(Chess sourceChess, Chess targetChess, Vector3 startPos, int skillId, int damage, string effectName)
    {
        Missile missilePrefab = Resources.Load<Missile>("Prefabs/MissileCom");
        var missile = UnityEngine.Object.Instantiate<Missile>(missilePrefab, startPos, Quaternion.identity, Units.transform);
        missile.Init(sourceChess, 1, effectName);
        missile.SetSkillInfo(skillId, damage);
        missile.MoveToTarget(targetChess, Mathf.Max(sourceChess.missileSpeed, 14), sourceChess.missileHight);
    }

    public void CreateSpellMissile(Chess sourceChess, Vector3 targetPos, float time, float speed, float size, int skillId, int damage, string effectName, Func<Chess, float> perTargetDamageMulti = null, Action<Chess> perTargetOnHit = null)
    {
        Missile missilePrefab = Resources.Load<Missile>("Prefabs/MissileCom");
        var missile = UnityEngine.Object.Instantiate<Missile>(missilePrefab, sourceChess.transform.position, Quaternion.identity, Units.transform);
        missile.Init(sourceChess, size, effectName);
        missile.SetSkillInfo(skillId, damage);
        missile.perTargetDamageMulti = perTargetDamageMulti;
        missile.perTargetOnHit = perTargetOnHit;
        missile.MoveToDirection(targetPos, time, speed);
    }

    // ================= 移动/阻挡 =================

    public bool MoveTo(Chess unit, Vector3 targetPosition, bool isForce = false)
    {
        unit.transform.position = targetPosition;
        return true;
    }

    // 无墙：直线通畅
    public Vector3? FindMoveWaypoint(Vector3 from, Vector3 to)
    {
        return null;
    }

    public bool CheckPositionBlocked(Chess unit, Vector3 position)
    {
        return false;
    }

    public bool TryLockGridPositions(Chess unit, Vector3 targetPosition, out List<Vector2Int> requiredGrids)
    {
        requiredGrids = new List<Vector2Int>();
        return true;
    }

    public bool IsPathBlockedGrid(Vector2Int grid)
    {
        return false;
    }

    // 获取指定位置和碰撞体占据的所有格子（无墙，结果不影响阻挡判定）
    public List<Vector2Int> GetOccupiedGrids(Vector3 position, Collider collider, bool clampUnit = false)
    {
        var result = new List<Vector2Int>();
        Vector3 boundsSize = collider != null ? collider.bounds.size : new Vector3(6f, 6f, 6f);
        Vector3 halfBounds = boundsSize / 3f;
        if (clampUnit)
        {
            halfBounds = new Vector3(
                Mathf.Min(halfBounds.x, gridCellSize * 2f / 3f),
                Mathf.Min(halfBounds.y, gridCellSize * 2f / 3f),
                Mathf.Min(halfBounds.z, gridCellSize * 2f / 3f));
        }
        for (float x = position.x - halfBounds.x; x <= position.x + halfBounds.x + 0.01f; x += gridCellSize)
        {
            for (float z = position.z - halfBounds.z; z <= position.z + halfBounds.z + 0.01f; z += gridCellSize)
                result.Add(WorldToGridPosition(new Vector3(x, position.y, z), true));
        }
        return result;
    }

    // ================= 战斗文本/屏幕 =================

    // 飘字收集到列表供 GDI+ 绘制（无特效）
    public void AddBattleText(string text, Vector3 worldPos, Vector2 speed, Color color, int duration)
    {
        battleTexts.Add(new BattleTextData
        {
            text = text,
            worldPos = worldPos,
            color = color,
            expireTime = Time.time + duration
        });
    }

    public Vector2 TransformWorldToScreen(Vector3 worldPosition, RectTransform canvas)
    {
        return Vector2.zero;
    }

    // ================= 死亡判定 =================

    // TeamMode2：移除死亡单位，统计存活阵营；只剩一个阵营(或同刻双方全灭)时结束战斗
    public void OnUnitDying(Chess dieUnit, int killerPlayerId)
    {
        chessList.Remove(dieUnit);
        RecomputeSideBattleOver(); // 单位阵亡后重算各阵营"战斗是否已结束"（无存活敌方即结束）

        bool[] sideHasUnits = new bool[8];
        int aliveSideCount = 0;
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                int sideIndex = chessComponent.side - 1;
                if (sideIndex >= 0 && sideIndex < sideHasUnits.Length && !sideHasUnits[sideIndex])
                {
                    sideHasUnits[sideIndex] = true;
                    aliveSideCount++;
                }
            }
        }

        GameLog.Debug("id:" + dieUnit.id + " dieUnit.side:" + dieUnit.side + " 存活阵营数:" + aliveSideCount);

        // 结束判定用 <=1（而非 ==1）：同一 tick 内双方最后单位同时死亡(例如疲劳同时击杀双方)时存活阵营数为 0，
        // 此时也必须结束（平局）。另外不再每次死亡都把 gameFinish 重置为 false，
        // 否则同 tick 的后一次死亡会把前一次已判定的结束标记抹掉，战斗永不结束（白跑到步数上限、界面看起来双方都傻了）。
        if (aliveSideCount <= 1)
        {
            if (aliveSideCount == 1)
            {
                int winnerSide = -1;
                for (int i = 0; i < sideHasUnits.Length; i++)
                {
                    if (sideHasUnits[i])
                    {
                        winnerSide = i + 1;
                        break;
                    }
                }
                var players = GameManager.Instance.players;
                if (players != null)
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        int playerSide = i + 1;
                        bool isWin = playerSide == winnerSide;
                        players[i].onBattleResult(isWin, isWin ? 10 : 0);
                    }
                }
                hasWin = sideHasUnits[0];
            }
            else
            {
                isDraw = true; // 同刻双方全灭：平局
            }
            gameFinish = true;
        }
    }

    // ================= 清理 =================

    public void Reset()
    {
        gameFinish = false;
        hasWin = false;
        isDraw = false;
        sideBattleOver = new bool[8]; // 各阵营战斗结束状态重置
        chessList.Clear();
        battleTexts.Clear();
        SkillFx.Clear();
        LiveRegistry.Clear();
    }
}
