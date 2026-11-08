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

        var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x != null && x.IsInFight());
        if (enemies.Count == 0)
            return false;

        // 范围内当前法力值最高的敌人（各 MpCost>0 技能的当前 MP 之和）
        Chess target = null;
        var highest = 0;
        foreach (var e in enemies)
        {
            var mp = GetCurrentMp(e);
            if (mp > highest)
            {
                highest = mp;
                target = e;
            }
        }
        if (target == null)
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

        var receiver = FindNearestAlly();
        if (receiver == null)
            receiver = owner;
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

    /// <summary>取 skillCfg.Range（未配置时退回 Area）半径内距离最近的友方单位（不含自身）</summary>
    private Chess FindNearestAlly()
    {
        var searchRange = skillCfg.Range > 0f ? skillCfg.Range : skillCfg.Area;
        var allies = WorldManager.Instance.GetMySideInRange(owner.transform.position, searchRange, owner.side)
            .FindAll(x => x != null && x != owner && x.IsInFight());

        Chess nearest = null;
        var minDist = float.MaxValue;
        foreach (var a in allies)
        {
            var d = Vector3.Distance(owner.transform.position, a.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = a;
            }
        }
        return nearest;
    }
}
