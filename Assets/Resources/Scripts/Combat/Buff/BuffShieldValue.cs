using System;
using UnityEngine;

public class BuffShieldValue : Buff
{
    public BuffShieldValue(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        // 减伤盾：恒定按 StrengthBuff1[0] 减免，不再做攻守属性对比（挂在伤害计算阶段，普攻与技能伤害统一生效）
        // 约定：DamageStrength 为伤害基值（仅伤害/治疗/DOT 使用），组合 Buff（如「硬,愈」）中本段占用第 1 个数值槽 [0]
        var values = skillCfg.StrengthBuff1;
        if (values == null || values.Length < 1)
        {
            GameLog.Warn($"减伤盾技能缺少减伤率配置: 技能id={skillCfg.Id} BuffId={skillCfg.BuffId}");
            return;
        }
        var rate = values[0];
        damageMulti -= rate;
        WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
    }
}