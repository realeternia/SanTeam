using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackedShadow : Skill
{
    private int count;
    public SkillAttackedShadow(int id, Chess unit) : base(id, unit)
    {
        count = skillCfg.DoCount;
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if (count > 0 && CheckBurst(attacker))
        {
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 randomPosition = owner.transform.position + new Vector3(randomDir.x, 0, randomDir.y) * skillCfg.Range;
            var shadowUnit = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501002, -1, randomPosition, owner.side, HeroConfig.GetConfig(owner.heroId).Icon);
            shadowUnit.attackDamage = (int)(owner.attackDamage * skillCfg.SkillDamageRate);
            shadowUnit.maxHp = (int)(owner.maxHp * skillCfg.SkillAttrRate);
            shadowUnit.hp = (int)(shadowUnit.maxHp * owner.HpRate);
            shadowUnit.material.SetFloat("_SecondTexSize", 2f);
            shadowUnit.material.SetTexture("_SecondTex", Resources.Load<Texture>("SkillPic/" + skillCfg.Icon));
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
            EffectManager.PlaySkillEffect(shadowUnit, skillCfg.HitEffect);

            owner.PlayerAnim(skillCfg.Action);

            count--;
        }
    }

}
