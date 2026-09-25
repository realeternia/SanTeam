// ============================================================
// 战斗模拟器 · 战斗驱动 —— BattleSim
// 装配 GameManager/WorldManager，复刻 WorldManager.BattleBegin 流程，
// 由外部主循环驱动：Time.Advance → CoroutineRunner.Step → 所有单位 LogicUpdate。
// ============================================================
using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class BattleSim
{
    public WorldManager World { get; private set; }
    public GameManager Game { get; private set; }

    // 布阵常量：前排近战士兵 pos0-4、后排远程士兵 pos5-9、英雄 pos10+
    private const int MeleeSoldierId = 500001;
    private const int RangedSoldierId = 500002;
    private const int FrontMeleeCount = 5;
    private const int FrontRangedCount = 5;
    private const int HeroStartPos = 10;

    public bool IsFinished { get { return World != null && World.gameFinish; } }
    public bool HasWin { get { return World != null && World.hasWin; } }
    public float BattleTime { get { return Time.time; } }

    private int _heroCountA;
    private int _heroCountB;
    private int _soldierCount;

    // 受击事件（供 GUI 绘制飘血与受击临时图形）。由血量差分检测生成，不改游戏源码。
    public class HitFxData
    {
        public int id;          // 受击单位 id
        public Vector3 pos;     // 受击时间点的世界坐标
        public int damage;      // 本次掉血
        public float time;      // Time.time 命中时刻
        public float dirZ;      // 攻击方向（朝向攻击方，屏幕 x 偏移系数：+1 向右 / -1 向左）
    }

    public readonly List<HitFxData> HitFx = new List<HitFxData>();
    private readonly Dictionary<int, float> _hpBefore = new Dictionary<int, float>();

    // 构造：创建 GameManager（2 玩家）与 WorldManager
    public BattleSim()
    {
        Game = new GameManager();
        GameManager.Instance = Game;
        Game.players = new PlayerInfo[2];
        // pid 0 = 甲（side1），pid 1 = 乙（side2）；PlayerConfig 用 1 旺仔 / 2 布布
        Game.players[0] = new PlayerInfo();
        Game.players[0].Init(0, 1);
        Game.players[1] = new PlayerInfo();
        Game.players[1].Init(1, 2);

        World = new WorldManager();
        WorldManager.Instance = World;
        World.SetupCenters();
    }

    // 设置双方上阵英雄，并按固定布阵填 battleCards（士兵 + 英雄）
    // soldierCount：每侧小兵数量（0=纯英雄；优先近战士兵，超出 5 个补远程）
    public void Setup(List<int> heroesA, List<int> heroesB, int soldierCount = 0)
    {
        _heroCountA = heroesA != null ? heroesA.Count : 0;
        _heroCountB = heroesB != null ? heroesB.Count : 0;
        _soldierCount = soldierCount;

        FillBattleCards(Game.players[0], heroesA, soldierCount);
        FillBattleCards(Game.players[1], heroesB, soldierCount);
    }

    // 按 25 格布阵填 battleCards：前排近战士兵、后排远程士兵、英雄放第10格起
    private void FillBattleCards(PlayerInfo p, List<int> heroes, int soldierCount)
    {
        var cards = p.battleCards;
        Array.Clear(cards, 0, cards.Length);

        int melee = Math.Min(soldierCount, FrontMeleeCount);
        int ranged = Math.Max(0, soldierCount - FrontMeleeCount);
        for (int i = 0; i < melee && i < cards.Length; i++)
            cards[i] = MeleeSoldierId;
        for (int i = 0; i < ranged && FrontMeleeCount + i < cards.Length; i++)
            cards[FrontMeleeCount + i] = RangedSoldierId;

        if (heroes != null)
        {
            for (int i = 0; i < heroes.Count && HeroStartPos + i < cards.Length; i++)
            {
                int heroId = heroes[i];
                if (heroId <= 0 || !ConfigManager.IsHeroCard(heroId))
                {
                    GameLog.Warn("BattleSim.Setup: 无效英雄 heroId=" + heroId + "，跳过");
                    continue;
                }
                cards[HeroStartPos + i] = heroId;
                // 卡片经验：默认 1（1 星）；已有卡则累加
                if (!p.cards.ContainsKey(heroId))
                    p.cards[heroId] = 1;
            }
        }
    }

    // 开始一场新战斗：重置随机种子/时间/协程/统计 → BattleBegin
    public void Start(int seed)
    {
        SysRandom.Seed(seed);
        Time.Reset();
        CoroutineRunner.Clear();
        BattleStatManager.Clear();
        World.Reset();
        World.BattleBegin();
        HitFx.Clear();
        _hpBefore.Clear();
    }

    // 单步推进：固定步长 dt=0.05s；通过血量差分检测命中事件
    public void Step(float dt = 0.05f)
    {
        Time.Advance(dt);
        CoroutineRunner.Step();

        // 记录起始血量，执行逻辑更新
        foreach (var chess in World.chessList.ToArray())
        {
            if (chess != null && chess.hp > 0)
                chess.LogicUpdate(dt);
        }

        // 近战英雄贴身逼近：射程(17格)偏大导致近战隔空站桩输出，这里让近战英雄继续朝最近敌方逼近到近身距离再停
        const float MeleeCloseDist = 10f;   // 近战贴身判定距离
        foreach (var c in World.chessList)
        {
            if (c == null || c.hp <= 0 || !c.isHero || c.attackRange <= 0 || c.attackRange >= 20)
                continue;
            Chess target = null;
            float best = float.MaxValue;
            foreach (var o in World.chessList)
            {
                if (o == null || o == c || o.hp <= 0 || o.side == c.side)
                    continue;
                float dx = c.transform.position.x - o.transform.position.x;
                float dz = c.transform.position.z - o.transform.position.z;
                float d = dx * dx + dz * dz;
                if (d < best) { best = d; target = o; }
            }
            if (target == null)
                continue;
            float dist = (float)Math.Sqrt(best);
            if (dist > MeleeCloseDist)
            {
                Vector3 dir = target.transform.position - c.transform.position;
                dir.y = 0f;
                float mag = (float)Math.Sqrt(dir.x * dir.x + dir.z * dir.z);
                if (mag < 0.0001f)
                    continue;
                dir.x /= mag;
                dir.z /= mag;
                float step = Math.Min(1f, dist - MeleeCloseDist);
                c.transform.position += dir * step;
            }
        }

        // 血量差分检测：比上一步少 → 这次步内被命中一次（受击/飘血）
        foreach (var c in World.chessList)
        {
            if (c == null)
                continue;
            if (c.hp <= 0)
            {
                _hpBefore.Remove(c.id);
                continue;
            }
            float prev;
            if (_hpBefore.TryGetValue(c.id, out prev) && prev > c.hp)
            {
                // 攻击方向推断：甲(side1)在左向右攻、乙(side2)在右向左攻；受击者的攻击方在对面
                float dirZ = (c.side == 1) ? 1f : -1f;
                HitFx.Add(new HitFxData
                {
                    id = c.id,
                    pos = c.transform.position,
                    damage = (int)(prev - c.hp),
                    time = Time.time,
                    dirZ = dirZ
                });
                if (HitFx.Count > 300)
                    HitFx.RemoveRange(0, 150);
            }
            _hpBefore[c.id] = c.hp;
        }
    }

    // 回合结果摘要
    public string GetResultSummary()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== 战斗结果 ===");
        sb.AppendLine("战斗时长: " + BattleTime.ToString("F1") + "s");
        sb.AppendLine("胜负: " + (HasWin ? "甲（side1）胜" : "乙（side2）胜"));

        int surviveA = 0, surviveB = 0;
        foreach (var c in World.chessList)
        {
            if (c == null || c.hp <= 0)
                continue;
            if (c.side == 1) surviveA++;
            else if (c.side == 2) surviveB++;
        }
        sb.AppendLine("存活: 甲 " + surviveA + " / 乙 " + surviveB);

        sb.AppendLine("=== 伤害统计 Top10 ===");
        int rank = 1;
        foreach (var stat in BattleStatManager.GetTop10())
        {
            var heroName = "";
            if (stat.heroId >= 500000)
                heroName = "士兵" + stat.heroId;
            else
            {
                var cfg = HeroConfig.GetConfig(stat.heroId);
                heroName = cfg != null ? cfg.Name : "hero" + stat.heroId;
            }
            sb.AppendLine(rank + ". P" + stat.playerId + " " + heroName
                + " 总伤:" + ((int)stat.damage)
                + " 攻击:" + stat.attackCount + "次 技能:" + ((int)stat.skillDamage)
                + " 对英雄:" + ((int)stat.heroDamage));
            rank++;
        }
        return sb.ToString();
    }
}
