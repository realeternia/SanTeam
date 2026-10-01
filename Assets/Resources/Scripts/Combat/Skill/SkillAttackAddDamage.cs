using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackAddDamage : Skill
{
    public SkillAttackAddDamage(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BeforeCalDamage(Chess target, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        if(!string.IsNullOrEmpty(skillCfg.BuffId) && !target.HasBuff(BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id))
            return;

        if(CheckBurst(target))
        {
            owner.PlayerAnim(skillCfg.Action);

            damageBase += (int)skillCfg.DamageStrength;
            // Strength2 为压缩数组（只存非零值）：空数组表示仅加固定伤害、无额外倍率（如道具"追风/骁勇/贯日"）
            if (skillCfg.Strength2.Length > 0 && skillCfg.Strength2[0] > 0)
                damageMulti += skillCfg.Strength2[0];
            effect = skillCfg.HitEffect;
        }
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if(isBurst)
            WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
    }

}
