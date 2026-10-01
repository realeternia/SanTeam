using CommonConfig;
using UnityEngine;

/// <summary>
/// 周泰·不屈：为自身附加 Strength2[0]×(100+法强)/100 的护盾（随法强成长），并回复 Strength2[1]% 最大生命，肉盾续命。
/// 同时移除当前敌方目标身上的吸收型护盾（破盾），压制对方续航。
/// </summary>
public class SkillAidUnbreakable : Skill
{
    public SkillAidUnbreakable(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 给自己套盾（护盾量 = Strength2[0] × (100+法强)/100，随法强成长）
        int shieldId = BuffConfig.GetConfigByNameS("盾").Id;
        BuffManager.AddBuff(owner, owner, id, shieldId, skillCfg.BuffTime);
        var sh = owner.GetBuff(shieldId) as BuffShield;
        if (sh != null)
            sh.SetHp(GetSkillShield(0));

        // 移除当前敌方目标身上的吸收型护盾（破盾）
        var enemy = owner.targetChess;
        if (enemy != null && enemy.hp > 0)
        {
            var enemyShield = enemy.GetBuff(CombatConst.ShieldBuffId) as BuffShield;
            if (enemyShield != null)
            {
                enemyShield.SetHp(0);
                BuffManager.RemoveBuff(enemy, CombatConst.ShieldBuffId);
            }
        }

        // 回复生命（护盾之外的实体回复）
        int heal = (int)(owner.maxHp * skillCfg.Strength2[1]);
        if (heal > 0)
            owner.HealTarget(owner, id, heal, true);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}