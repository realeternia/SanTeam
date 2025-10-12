using System;
using CommonConfig;
using UnityEngine;

public class SkillInitGold : Skill
{
    public SkillInitGold(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

        var goldAdd = Math.Max(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillAttrRate, skillCfg.StrengthInt);
        owner.GetPlayerInfo().AddGold(goldAdd);

        WorldManager.Instance.AddBattleText(goldAdd.ToString() + "金", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.yellow, 2);
    }
}