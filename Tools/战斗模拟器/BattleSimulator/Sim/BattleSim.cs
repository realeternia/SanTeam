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
    public void Setup(List<int> heroesA, List<int> heroesB)
    {
        _heroCountA = heroesA != null ? heroesA.Count : 0;
        _heroCountB = heroesB != null ? heroesB.Count : 0;

        FillBattleCards(Game.players[0], heroesA);
        FillBattleCards(Game.players[1], heroesB);
    }

    // 按 25 格布阵填 battleCards：前排近战士兵、后排远程士兵、英雄放第10格起
    private void FillBattleCards(PlayerInfo p, List<int> heroes)
    {
        var cards = p.battleCards;
        Array.Clear(cards, 0, cards.Length);

        for (int i = 0; i < FrontMeleeCount && i < cards.Length; i++)
            cards[i] = MeleeSoldierId;
        for (int i = 0; i < FrontRangedCount && FrontMeleeCount + i < cards.Length; i++)
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
    }

    // 单步推进：固定步长 dt=0.05s
    public void Step(float dt = 0.05f)
    {
        Time.Advance(dt);
        CoroutineRunner.Step();

        foreach (var chess in World.chessList.ToArray())
        {
            if (chess != null && chess.hp > 0)
                chess.LogicUpdate(dt);
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
