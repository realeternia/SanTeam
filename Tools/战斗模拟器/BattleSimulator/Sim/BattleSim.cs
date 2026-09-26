// ============================================================
// 战斗模拟器 · 战斗驱动 —— BattleSim
// 装配 GameManager/WorldManager，复刻 WorldManager.BattleBegin 流程，
// 由外部主循环驱动：Time.Advance → CoroutineRunner.Step → 所有单位 LogicUpdate。
// ============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

public class BattleSim
{
    public WorldManager World { get; private set; }
    public GameManager Game { get; private set; }

    // 布阵常量：前排近战士兵 pos0-4、弩手 pos5-7、弓手 pos8-9、英雄 pos10+
    private const int MeleeSoldierId = 500001;
    private const int CrossbowSoldierId = 500004;   // 弩手（短射程高攻击）
    private const int ArcherSoldierId = 500003;      // 弓手（长射程）
    private const int FrontMeleeCount = 5;
    private const int FrontCrossCount = 3;
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
        public int side;        // 受击方阵营（1/2）
        public string heroName; // 受击单位名（供日志显示）
        public string attackerName; // 攻击方名（供"谁攻击谁"日志显示）
        public int attackerSide;    // 攻击方阵营（1/2），日志按此方着色
        public string skillName;    // 来源技能名（null=普攻），供日志区分技能/普攻伤害
        public int shieldAbsorb;    // 本次被护盾吸收的伤害量（>0=有盾抵挡），日志区分"盾降低"与"实际受伤"
    }

    public readonly List<HitFxData> HitFx = new List<HitFxData>();
    private readonly Dictionary<Skill, float> _skillMpBefore = new Dictionary<Skill, float>();

    // 当前活跃战斗实例：订阅 Chess.OnDamageDealt 使用静态活跃实例，避免多实例事件累积
    private static BattleSim _active;
    static BattleSim()
    {
        Chess.OnDamageDealt += (a, v, d, sk) => { if (_active != null) _active.RecordDamage(a, v, d, sk); };
    }

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
        _active = this;
    }

    // 由 Chess.OnDamageDealt 事件驱动：每次伤害精确一次，生成飘字/日志数据（不靠血量差分，避免治疗/护盾/多段伤害漏记）
    private void RecordDamage(Chess attacker, Chess victim, int damage, int skillId)
    {
        if (victim == null || attacker == null || damage <= 0)
            return;
        float dirZ = attacker.side == 1 ? 1f : -1f;

        string victimName = "士兵#" + (victim.isHero ? victim.heroId : victim.soldierId);
        if (victim.isHero && victim.heroId > 0 && HeroConfig.HasConfig(victim.heroId))
            victimName = HeroConfig.GetConfig(victim.heroId).Name;

        string atkName = "士兵#" + (attacker.isHero ? attacker.heroId : attacker.soldierId);
        if (attacker.isHero && attacker.heroId > 0 && HeroConfig.HasConfig(attacker.heroId))
            atkName = HeroConfig.GetConfig(attacker.heroId).Name;

        string skillName = null;
        if (skillId > 0)
        {
            var sk = SkillConfig.GetConfig(skillId);
            if (sk != null) skillName = sk.Name;
        }

        HitFx.Add(new HitFxData
        {
            id = victim.id,
            pos = victim.transform.position,
            damage = damage,
            time = Time.time,
            dirZ = dirZ,
            side = victim.side,
            heroName = victimName,
            attackerName = atkName,
            attackerSide = attacker.side,
            skillName = skillName,
            shieldAbsorb = victim.lastShieldAbsorb
        });
        if (HitFx.Count > 300)
            HitFx.RemoveRange(0, 150);
    }

    // 设置双方上阵英雄（含等级），并按固定布阵填 battleCards（士兵 + 英雄）
    // soldierCount：每侧小兵数量（0=纯英雄；优先近战士兵，超出 5 个补远程）
    public void Setup(List<int> heroesA, List<int> heroesB, int soldierCount = 0)
    {
        Setup(ToLeveled(heroesA), ToLeveled(heroesB), soldierCount);
    }

    // 带等级的上阵（heroId, level：1~5）；等级写进 cards 并经 GetBattleCardList → CheckInitAttr(lv) 生效
    public void Setup(List<(int id, int lv)> heroesA, List<(int id, int lv)> heroesB, int soldierCount = 0)
    {
        _heroCountA = heroesA != null ? heroesA.Count : 0;
        _heroCountB = heroesB != null ? heroesB.Count : 0;
        _soldierCount = soldierCount;

        FillBattleCards(Game.players[0], heroesA, soldierCount);
        FillBattleCards(Game.players[1], heroesB, soldierCount);
    }

    // 把纯 id 列表统一转成等级1（旧调用路径）
    private static List<(int id, int lv)> ToLeveled(List<int> ids)
    {
        if (ids == null)
            return null;
        return ids.Select(id => (id, 1)).ToList();
    }

    // 按 25 格布阵填 battleCards：前排近战士兵、后排远程士兵、英雄放第10格起
    private void FillBattleCards(PlayerInfo p, List<(int id, int lv)> heroes, int soldierCount)
    {
        var cards = p.battleCards;
        Array.Clear(cards, 0, cards.Length);

        int melee = Math.Min(soldierCount, FrontMeleeCount);
        int cross = Math.Min(Math.Max(0, soldierCount - FrontMeleeCount), FrontCrossCount);
        int archer = Math.Max(0, soldierCount - FrontMeleeCount - FrontCrossCount);
        for (int i = 0; i < melee && i < cards.Length; i++)
            cards[i] = MeleeSoldierId;
        for (int i = 0; i < cross && FrontMeleeCount + i < cards.Length; i++)
            cards[FrontMeleeCount + i] = CrossbowSoldierId;
        for (int i = 0; i < archer && FrontMeleeCount + FrontCrossCount + i < cards.Length; i++)
            cards[FrontMeleeCount + FrontCrossCount + i] = ArcherSoldierId;

        if (heroes != null)
        {
            for (int i = 0; i < heroes.Count && HeroStartPos + i < cards.Length; i++)
            {
                int heroId = heroes[i].id;
                if (heroId <= 0 || !ConfigManager.IsHeroCard(heroId))
                {
                    GameLog.Warn("BattleSim.Setup: 无效英雄 heroId=" + heroId + "，跳过");
                    continue;
                }
                cards[HeroStartPos + i] = heroId;
                // 卡片经验存等级（1~5）：GetBattleCardList.Item2 会作为 lv 传入 CheckInitAttr 生效
                p.cards[heroId] = Math.Max(1, Math.Min(5, heroes[i].lv));
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
        _skillMpBefore.Clear();
    }

    // 单步推进：固定步长 dt=0.05s；伤害事件由 Chess.OnDamageDealt 驱动生成飘字/日志
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

        // 技能施放检测：耗蓝技能发动时 skill.mp 会从满值(MpCost)清空到 0，据此捕获并记录技能名
        foreach (var c in World.chessList)
        {
            if (c == null || c.hp <= 0)
                continue;
            foreach (var sk in c.skills)
            {
                if (sk == null)
                    continue;
                float prevMp;
                if (!_skillMpBefore.TryGetValue(sk, out prevMp))
                {
                    _skillMpBefore[sk] = sk.mp;
                    continue;
                }
                if (prevMp > 1f && sk.mp <= 1f)
                {
                    var cfg = SkillConfig.GetConfig(sk.skillId);
                    var name = cfg != null ? cfg.Name : null;
                    if (!string.IsNullOrEmpty(name))
                    {
                        var heroCfg = HeroConfig.GetConfig(c.isHero ? c.heroId : c.soldierId);
                        World.SkillFx.Add(new WorldManager.SkillFxData
                        {
                            pos = c.transform.position,
                            name = name,
                            time = Time.time,
                            side = c.side,
                            heroName = heroCfg != null ? heroCfg.Name : ("#" + c.heroId)
                        });
                        if (World.SkillFx.Count > 200)
                            World.SkillFx.RemoveRange(0, 50);
                    }
                }
                _skillMpBefore[sk] = sk.mp;
            }
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
