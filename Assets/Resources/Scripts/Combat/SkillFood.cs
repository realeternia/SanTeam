using CommonConfig;
using UnityEngine;

public class SkillFood : Skill
{
    public SkillFood(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

        var addon = skillCfg.StrengthInt;
        owner.GetPlayerInfo().AddFood(addon);

        WorldManager.Instance.AddBattleText(addon.ToString() + "粮食", owner.transform.position, new UnityEngine.Vector2(0, 60), new Color(1, 0.8f, 0), 2);
    }
}