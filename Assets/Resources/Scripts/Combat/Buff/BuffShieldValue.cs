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
        // 减伤盾（短名"硬"）：恒定按 StrengthBuff1 语义 key "shieldReduce" 减免，不再做攻守属性对比
        // （挂在伤害计算阶段，普攻与技能伤害统一生效）
        var rate = GetStrBuff("shieldReduce");
        if (rate <= 0f)
        {
            GameLog.Warn($"减伤盾技能缺少减伤率配置: 技能id={skillCfg.Id} BuffId={skillCfg.BuffId}");
            return;
        }
        damageMulti -= rate;
        WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
    }
}