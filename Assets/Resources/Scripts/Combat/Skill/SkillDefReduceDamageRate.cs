using CommonConfig;
using UnityEngine;

public class SkillDefReduceDamageRate : Skill
{
    public SkillDefReduceDamageRate(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BeforeCalDamaged(Chess caster, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        if (isFeedback)
            return;

        // 触发条件(TriggerCondition，如 hprate<50)满足且发动概率命中时，按技能档位减免伤害（等效伤害 ×(1-Strength)）
        if (CheckBurst(caster))
        {
            WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            damageMulti *= 1 - skillCfg.Strength;
        }
    }
}