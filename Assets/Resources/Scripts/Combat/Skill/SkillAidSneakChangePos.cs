using CommonConfig;
using UnityEngine;

/// <summary>
/// 偷袭（主动辅助技能：「偷」2010071~2010075）：CD 10 秒，攻击目标存在时按 Rate 概率发动，
/// 锁定自身技能射程(skillCfg.Range)内距离最远的敌方单位，造成 GetSkillDamage 伤害并与其交换位置。
/// 目标需处于交战（Chess.IsInFight / IsRecentlyDamaged）才换位，避免刚落到敌方后排就被集火；
/// 每个敌方单位整场只可能被交换一次（sneakSwapped 标记，防止多名偷袭者重复交换同一目标）。
/// </summary>
public class SkillAidSneakChangePos : Skill
{
    public SkillAidSneakChangePos(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner == null || owner.hp <= 0)
            return false;

        var target = FarthestEnemyInRange();
        if (target == null)
            return false;

        // 目标未进入交战：此时跳过去会被敌方后排直接集火，本次不发动（后续检查会继续重试）
        if (!target.IsInFight() && !target.IsRecentlyDamaged())
            return false;

        if (!CheckBurst(target))
            return false;

        PlaySelfEffect();

        // 按统一技能伤害公式结算伤害
        var damage = GetSkillDamage();
        if (damage > 0)
        {
            target.OnSkillDamaged(owner, skillId, damage);
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
            WorldManager.Instance.AddBattleText(damage + "!", target.transform.position, new Vector2(0, 60), Color.red, 3);
        }

        // 与目标交换位置
        target.sneakSwapped = true;
        var ownerPos = owner.transform.position;
        var targetPos = target.transform.position;
        owner.MoveTo(targetPos, true);
        target.MoveTo(ownerPos, true);
        WorldManager.Instance.AddBattleText("偷袭换位!", target.transform.position, new Vector2(0, 80), Color.yellow, 3);
        return true;
    }

    // 技能射程内距离自身最远、且未被换过位的存活敌方单位
    private Chess FarthestEnemyInRange()
    {
        Chess best = null;
        float bestDist = -1f;
        foreach (var e in WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Range, owner.side))
        {
            if (e == null || e.hp <= 0 || e.sneakSwapped)
                continue;

            float d = WorldManager.Instance.GetRange(owner.transform.position, e.transform.position);
            if (d > bestDist)
            {
                bestDist = d;
                best = e;
            }
        }
        return best;
    }
}
