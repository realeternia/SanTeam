using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 贾诩·乱阵：战斗开始给本侧近战士兵附加普攻眩晕 Buff(BuffHitStun，按 Strength2[0] 概率眩晕)；
/// 每次主动释放对目标造成法术伤害并使其混乱(攻击最近的己方单位)。
/// Buff 短名由 SkillConfig.BuffId 以逗号分隔配置：第 1 个=承伤眩晕Buff(威)，
/// 第 2 个=眩晕效果Buff(乱，供 BuffHitStun 普攻命中时施加)，第 3 个=主动释放控制Buff(惑=混乱)。
/// </summary>
public class SkillSoldierStun : Skill
{
    public SkillSoldierStun(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        // 开局给本侧近战士兵附加普攻眩晕 Buff(BuffHitStun)：带该 Buff 的士兵普攻按 Strength2[0](百分数) 概率眩晕目标
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 1)
        {
            GameLog.Error($"乱阵.BattleBegin: 未配置承伤眩晕Buff BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }
        int carrierBuffId = buffIds[0];
        int count = 0;
        foreach (var s in WorldManager.Instance.GetUnitsMySide(owner.side).Where(x => !x.isHero && x.attackRange < CombatConst.MeleeRange))
        {
            if (s.GetBuff(carrierBuffId) != null)
                continue; // 防重复附加
            BuffManager.AddBuff(s, owner, skillCfg.Id, carrierBuffId, 999f);
            count++;
        }
        GameLog.Debug($"乱阵 技能id={id} 等级={Level} 给本侧近战士兵附加普攻眩晕Buff {count} 名");
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        // BuffId 逗号分隔：buffIds[2]=主动释放控制Buff(惑=混乱)
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 3)
        {
            GameLog.Error($"乱阵.CheckAidSkill: BuffId 需配置承伤/眩晕/混乱三个buff（如\"威,乱,惑\"）技能id={id}");
            return false;
        }
        int confuseBuffId = buffIds[2];

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        target.OnSkillDamaged(owner, skillId, GetSkillDamage());
        BuffManager.AddBuff(target, owner, id, confuseBuffId, skillCfg.BuffTime);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}