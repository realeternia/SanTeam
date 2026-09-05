using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

public class SkillDefFeedback : Skill
{
    public SkillDefFeedback(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        DoFeedback(attacker, damType, damage);
    }

    public override void OnBeDoSkillDamage(Chess caster, SkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        if(isFeedback)
            return;
        DoFeedback(caster, checkSkillCfg.Attr, damage);
    }    

    private void DoFeedback(Chess attacker, string damType, int damage)
    {
        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(damType))
            return;

        if (skillCfg.Range > 0)
        {
            // 配置表重生成后已无 RangeOut 列：Range 表示反弹生效范围（范围内才触发，对应旧表 RangeOut=false 语义，刺甲/明镜均适用）
            var isInRange = WorldManager.Instance.CheckInRange(owner.transform.position, attacker.transform.position, skillCfg.Range);
            if (!isInRange)
                return;
        }

        if (CheckBurst(attacker))
        {
            var damageBack = (int)(damage * skillCfg.Strength);
            attacker.OnSkillDamaged(owner, skillId, damageBack, true);
            EffectManager.PlaySkillEffect(attacker, skillCfg.HitEffect);

            WorldManager.Instance.AddBattleText("反" + damageBack.ToString(), attacker.transform.position, new UnityEngine.Vector2(0, 150), new UnityEngine.Color(0.65f, 0.31f, 0), 3);
        }
    }

}
