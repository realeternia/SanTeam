using CommonConfig;

/// <summary>
/// 通用·开局对随机敌方英雄施加 Buff（ScriptName = "InitEnemyRandomBuff"）：
/// 战斗开始时，在敌方存活英雄中随机选 1 名，对其施加 SkillConfig.BuffId 指定的 Buff，
/// 持续 BuffTime 秒；Buff 强度由技能行 Strength2[1] 提供（由具体 Buff 实现解释，如 301003「伤」= 受到伤害增加比例）。
/// 使用示例：权奸（缩写「奸」，2010096~2010100，BuffId="伤"，Strength2[1]=30%~70%，BuffTime=10~20s）。
/// 随机范围为未持有该 Buff 的敌方英雄，优先近战，近战全部挂满后才轮到远程；若全部已持有则本次不施加。
/// 好友组内每个成员各持一份该技能，各自独立触发一次。
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

        // 敌方存活英雄中随机选一名未持有该 Buff 者（优先配对敌人，无配对敌人时取全部敌方英雄）：
        // 近战优先（射程<=20 判为近战，与 JobLinkManager/弩羁绊规则一致），近战全部挂满后才轮到远程
        var unbuffed = WorldManager.Instance.GetAllEnemys(owner.side)
            .FindAll(x => !x.HasBuff(buffCfg.Id));
        var melee = unbuffed.FindAll(x => HeroSelectionTool.IsMeleeHero(HeroConfig.GetConfig(x.heroId)));
        var candidates = melee.Count > 0
            ? melee
            : unbuffed.FindAll(x => HeroSelectionTool.IsRangedHero(HeroConfig.GetConfig(x.heroId)));
        if (candidates.Count == 0)
            return;

        var target = candidates[SysRandom.Range(0, candidates.Count)];
        BuffManager.AddBuff(target, owner, id, buffCfg.Id, skillCfg.BuffTime);
        GameLog.Debug($"开局随机敌方Buff 技能id={id} 等级={Level} Buff={buffCfg.NameS} 目标英雄={target.heroId} 强度={skillCfg.StrengthBuff1[0] * 100:0}% 持续={skillCfg.BuffTime}s");
    }
}
