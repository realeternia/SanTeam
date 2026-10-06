using CommonConfig;
using UnityEngine;

/// <summary>
/// 典韦·舍身救主：瞬移至最近受伤(默认1s内受击)的友军英雄"身前"，对落点 /area 范围内敌人造成 /damagestrength 法术伤害，
/// 并给自身挂「甲」Buff 提升 /strengthbuff1-1 点护甲，持续 /bufftime 秒。
/// "身前"方向 = 该友军指向它最近一次伤害来源的位置（来源已阵亡仍用快照位置），使典韦挡在友军与攻击者之间。
/// </summary>
public class SkillAidSacrificeGuard : Skill
{
    /// <summary>落点相对友军朝向伤害来源方向的前置距离(米)：站在友军与攻击者之间起挡刀作用</summary>
    public const float FrontOffset = 9f;

    public SkillAidSacrificeGuard(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;

        // 选最近受伤的友军英雄（多个时取最近一次受伤者）
        var protect = FindRecentlyDamagedAlly();
        if (protect == null)
            return false;
        if (!CheckBurst(protect))
            return false;

        var landPos = CalcFrontPosition(protect);

        owner.PlayerAnim(skillCfg.Action);
        owner.MoveTo(landPos, true);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

        // 自身增防：按 BuffId 配置施加护甲 Buff
        foreach (var buffId in GetSkillBuffIds())
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);

        // 落点范围伤害
        PlayAreaEffect(landPos);
        var skillDamage = GetSkillDamage();
        if (skillDamage > 0)
        {
            var units = WorldManager.Instance.GetEnemyInRange(landPos, skillCfg.Area, owner.side);
            foreach (var u in units)
            {
                if (u == null || u.hp <= 0)
                    continue;
                u.OnSkillDamaged(owner, id, skillDamage);
            }
        }

        return true;
    }

    /// <summary>取 Range 内最近受伤的友军英雄（排除自身），多个时取最近一次受伤者；无则返回 null</summary>
    private Chess FindRecentlyDamagedAlly()
    {
        var allies = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Range, owner.side);
        Chess protect = null;
        foreach (var a in allies)
        {
            if (a == null || a == owner || a.hp <= 0 || !a.isHero || !a.IsRecentlyDamaged())
                continue;
            if (protect == null || a.lastDamagedTime > protect.lastDamagedTime)
                protect = a;
        }
        return protect;
    }

    /// <summary>计算友军"身前"落点：友军位置 + 指向最近伤害来源的方向 × FrontOffset</summary>
    private Vector3 CalcFrontPosition(Chess protect)
    {
        var dir = protect.lastDamagedSourcePos - protect.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            // 伤害来源与友军几乎同点：退化为自身指向友军的方向
            dir = protect.transform.position - owner.transform.position;
            dir.y = 0f;
        }
        if (dir.sqrMagnitude < 0.0001f)
            dir = owner.transform.forward;
        return protect.transform.position + dir.normalized * FrontOffset;
    }
}
