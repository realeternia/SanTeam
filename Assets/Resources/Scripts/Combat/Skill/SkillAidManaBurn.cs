using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹叡 · 压制（法力燃烧）：在自身 skillCfg.Area 半径内挑选“当前法力值最高”的敌方单位，
/// 燃烧其 skillCfg.Strength2 键 "mpBurn" 点法力（只对设置了 MpCost 的技能生效）；
/// 再把实际燃烧量 × 键 "mpShareRate"（默认 0.5）转移给 skillCfg.Range 半径内最近的一名友方，
/// 该友方所有 MpCost&gt;0 的技能各加同量法力并各自裁剪到上限；身边无友方时转移给自己。
/// </summary>
public class SkillAidManaBurn : Skill
{
    public SkillAidManaBurn(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        // 目标：按 SelectOnly / SelectFirst 选（典型 enemy;inFight + highestMp，最高 = 各蓝耗技能当前法力之和最大）
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, false, 1);
        var target = picked.Count > 0 ? picked[0] : null;
        // 无可燃烧法力时不发动，避免空耗 MP
        if (target == null || GetCurrentMp(target) <= 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var burned = BurnMpToTarget(target, GetStr2("mpBurn"));
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        if (burned <= 0)
            return true;

        var share = Mathf.RoundToInt(burned * GetStr2("mpShareRate", 0.5f));
        if (share <= 0)
            return true;

        // 法力转移对象：次要场景固定 "ally;notSelf;inFight + nearest"，范围内无友军则给自己
        var searchRange = skillCfg.Range > 0f ? skillCfg.Range : skillCfg.Area;
        var receiverPick = SelectTargetsWithRules(owner.transform.position, searchRange, true, 1,
            "ally;notSelf;inFight", "nearest");
        var receiver = receiverPick.Count > 0 ? receiverPick[0] : owner;
        foreach (var skill in receiver.skills)
        {
            if (skill.skillCfg.MpCost > 0 && !skill.IsMpFull())
                skill.AddMp(share);
        }

        GameLog.Info("SkillAidManaBurn: 燃烧 " + target.chessName + " " + burned + " 点法力，转移 " + share + " 点给 " + receiver.chessName);
        return true;
    }

    /// <summary>统计单位所有 MpCost&gt;0 技能当前的 MP 总量（用于“法力值最高”的目标筛选）</summary>
    private int GetCurrentMp(Chess chess)
    {
        var total = 0;
        foreach (var skill in chess.skills)
        {
            if (skill.skillCfg.MpCost > 0)
                total += (int)skill.mp;
        }
        return total;
    }
}
