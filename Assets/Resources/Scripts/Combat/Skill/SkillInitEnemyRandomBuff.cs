using System.Collections.Generic;
using CommonConfig;

/// <summary>
/// 通用·开局对随机敌方英雄施加 Buff（ScriptName = "InitEnemyRandomBuff"）：
/// 战斗开始时，在敌方存活英雄中随机选 1 名，对其施加 SkillConfig.BuffId 指定的 Buff，
/// 持续 BuffTime 秒；Buff 强度由技能行 Strength 提供（由具体 Buff 实现解释，如 301003「伤」= 受到伤害增加比例）。
/// 使用示例：权奸当道·弄权跋扈（缩写「奸」，2010096~2010100，BuffId="伤"，Strength=30%~70%，BuffTime=10~20s）。
/// 好友组内每个成员各持一份该技能，各自独立触发一次；重复命中同一目标时同 id Buff 只刷新时间、数值不叠加。
/// </summary>
public class SkillInitEnemyRandomBuff : Skill
{
    public SkillInitEnemyRandomBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error($"开局随机敌方Buff技能缺少Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }

        // 敌方存活英雄中随机选一名（range=0 表示全场，参考偷袭技能的随机选敌写法）
        var candidates = new List<Chess>();
        foreach (var chess in WorldManager.Instance.GetUnitsInRange(owner.transform.position, 0, owner.side, true))
        {
            if (chess.isHero && chess.hp > 0)
                candidates.Add(chess);
        }
        if (candidates.Count == 0)
            return;

        var target = candidates[SysRandom.Range(0, candidates.Count)];
        BuffManager.AddBuff(target, owner, id, buffCfg.Id, skillCfg.BuffTime);
        GameLog.Debug($"开局随机敌方Buff 技能id={id} 等级={Level} Buff={buffCfg.NameS} 目标英雄={target.heroId} 强度={skillCfg.Strength * 100:0}% 持续={skillCfg.BuffTime}s");
    }
}
