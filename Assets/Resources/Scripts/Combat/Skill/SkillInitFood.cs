using System;
using CommonConfig;
using UnityEngine;

public class SkillInitFood : Skill
{
    public SkillInitFood(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

        var addon = Math.Max(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillAttrRate, skillCfg.StrengthInt);        
        owner.GetPlayerInfo().AddFood((int)addon);
        owner.PlayerAnim(skillCfg.Action);

        WorldManager.Instance.AddBattleText(addon.ToString() + "粮食", owner.transform.position, new UnityEngine.Vector2(0, 60), new Color(1, 0.8f, 0), 2);
    }
}