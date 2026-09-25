using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 贾诩·乱阵：战斗开始给本侧近战士兵附加普攻眩晕 Buff(BuffHitStun，按 StrengthInt 概率眩晕)；
/// 每次主动释放对目标造成法术伤害并眩晕。
/// </summary>
public class SkillSoldierStun : Skill
{
    /// <summary>眩晕效果 Buff 短名(乱)：主动技命中目标、以及 BuffHitStun 普攻命中目标时施加的眩晕 Buff</summary>
    public const string StunBuffNameS = "乱";

    public SkillSoldierStun(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        // 开局给本侧近战士兵附加普攻眩晕 Buff(BuffHitStun，由乱阵 BuffId="威" 配置承伤 Buff)：带该 Buff 的士兵普攻按 StrengthInt(百分数) 概率眩晕目标
        var carrierCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (carrierCfg == null)
        {
            GameLog.Error("乱阵.BattleBegin: 未配置承伤眩晕Buff " + skillCfg.BuffId);
            return;
        }
        int count = 0;
        foreach (var s in WorldManager.Instance.GetUnitsMySide(owner.side).Where(x => !x.isHero && x.attackRange < CombatConst.MeleeRange))
        {
            if (s.GetBuff(carrierCfg.Id) != null)
                continue; // 防重复附加
            BuffManager.AddBuff(s, owner, skillCfg.Id, carrierCfg.Id, 999f);
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
        owner.PlayerAnim(skillCfg.Action);

        target.OnSkillDamaged(owner, skillId, GetSkillDamage());
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS(StunBuffNameS).Id, skillCfg.BuffTime);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}