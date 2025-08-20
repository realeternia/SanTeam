using CommonConfig;
using UnityEngine;

public class SkillGold : Skill
{
    public SkillGold(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        UnityEngine.Debug.Log("SkillGold BattleBegin");

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

        var goldAdd = (int)(skillCfg.Strength);
        owner.GetPlayerInfo().AddGold(goldAdd);

        WorldManager.Instance.AddBattleText(goldAdd.ToString() + "金", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.yellow, 2);
    }
}