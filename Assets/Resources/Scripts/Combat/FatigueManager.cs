// ============================================================
// 战斗 · 疲劳机制 —— FatigueManager
// 战斗超时加压：开战后 FatigueStartTime 秒起，每 FatigueInterval 秒对全体英雄结算一次疲劳伤害，
// 数值按 FatigueBaseDamage 线性递增(100/200/300…)。真实伤害：直接扣血，无视抗性/护盾/减伤/闪避等一切结算。
// 由战斗主循环驱动（真机 WorldManager.GameUpdate / 模拟器 BattleSim.Step 每 tick 调 Tick），开战前 Reset。
// ============================================================
using System.Collections.Generic;

/// <summary>战斗疲劳：超时后按固定间隔对全体英雄施加递增的真实伤害（拖时间会被逐渐耗死）</summary>
public static class FatigueManager
{
    private static float elapsed;   // 本场已进行的战斗时长(秒)
    private static float nextTime;  // 下一次疲劳结算的时间点(秒)
    private static int count;       // 本场已结算次数(决定伤害 100/200/300…)

    /// <summary>开战前重置（真机 WorldManager.BattleBegin / 模拟器 BattleBegin 调用）</summary>
    public static void Reset()
    {
        elapsed = 0f;
        count = 0;
        nextTime = CombatConst.FatigueStartTime;
    }

    /// <summary>每个战斗 tick 调用一次（dt=战斗步长），内部按秒累计并在到点后结算</summary>
    public static void Tick(float dt)
    {
        if (CombatConst.FatigueInterval <= 0f)
            return;
        elapsed += dt;
        // 一帧可能跨过多个结算点，逐个补齐（如步长较大或首次触发时）
        while (elapsed >= nextTime)
        {
            ApplyFatigue();
            nextTime += CombatConst.FatigueInterval;
        }
    }

    // 对全体英雄结算一次疲劳伤害（真实伤害，直接扣血）
    private static void ApplyFatigue()
    {
        count++;
        int damage = CombatConst.FatigueBaseDamage + (count - 1) * CombatConst.FatigueDamageGrowth;

        var game = GameManager.Instance;
        var world = WorldManager.Instance;
        if (game == null || game.players == null || world == null)
        {
            GameLog.Warn("FatigueManager: GameManager/WorldManager 未就绪，跳过疲劳结算");
            return;
        }

        // 按参战方逐侧取存活英雄（与 FactionShieldManager 同口径：GetUnitsMySide 已过滤死亡/影子）；
        // side 去重，避免多玩家共用同一 battleSide 时重复结算
        var handledSides = new HashSet<int>();
        foreach (var player in game.players)
        {
            if (player == null || !handledSides.Add(player.battleSide))
                continue;
            foreach (var unit in world.GetUnitsMySide(player.battleSide))
            {
                if (unit == null || !unit.isHero)
                    continue;
                unit.TakeFatigueDamage(damage);
            }
        }
    }
}