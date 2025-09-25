using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackedShadow : Skill
{
    private int count = 3;
    public SkillAttackedShadow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if (count > 0 && CheckBurst(Math.Min(skillCfg.Rate, count * 0.1f)))
        {
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 randomPosition = owner.transform.position + new Vector3(randomDir.x, 0, randomDir.y) * skillCfg.Range;
            var shadowUnit = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501002, -1, randomPosition, owner.side, HeroConfig.GetConfig(owner.heroId).Icon);
            shadowUnit.attackDamage = (int)(owner.attackDamage * skillCfg.Strength);
            shadowUnit.maxHp = (int)(owner.maxHp * skillCfg.Strength);
            shadowUnit.hp = shadowUnit.maxHp;
            shadowUnit.material.SetFloat("_SecondTexSize", 2f);
            shadowUnit.material.SetTexture("_SecondTex", Resources.Load<Texture>("SkillPic/" + skillCfg.Icon));
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
            EffectManager.PlaySkillEffect(shadowUnit, skillCfg.HitEffect);

            count--;
        }
    }

}
