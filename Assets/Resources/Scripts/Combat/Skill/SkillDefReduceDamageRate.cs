using CommonConfig;
using UnityEngine;

public class SkillDefReduceDamageRate : Skill
{
    public SkillDefReduceDamageRate(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BeforeCalDamaged(Chess caster, SkillConfig checkSkillCfg, ref int damage, string hurtTag, bool isFeedback)
    {
        if (isFeedback)
            return;

        // 触发条件(TriggerCondition，如 hprate<50)满足且发动概率命中时，按技能档位减免伤害
        if (CheckBurst(caster))
        {
            WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            damage = (int)(damage * (1 - skillCfg.Strength));
        }
    }
}