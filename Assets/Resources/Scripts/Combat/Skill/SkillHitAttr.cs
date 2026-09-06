using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitAttr : Skill
{
    public SkillHitAttr(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            // 无双已并入攻击：随机强化在 法强(ap)/攻击(atk) 之间二选一
            var attr = SysRandom.Range(0, 2) == 0 ? "ap" : "atk";
            owner.AddAttr(attr, skillCfg.StrengthInt);
            owner.PlayerAnim(skillCfg.Action);
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        }
    }

}
