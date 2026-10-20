using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 典韦·天降恶来：从天而降，从施法点正上方加速砸落（参考吕布·魔神的下落段）。
/// 施法点：Range 内敌人英雄 ≥2 时，先取距自身最远者为第一目标，再取其 Area 内距第一目标最远者为第二目标，取两者中点；
/// 仅有 1 个目标时，落点取在距该目标 SingleTargetDistance（&lt;10）的位置。
/// 落地对 Area 范围内最多 TargetCount 个敌人（由近到远）造成 GetSkillDamage 伤害，并给自身挂「甲」Buff 提升 StrengthBuff1-armor 点护甲，持续 BuffTime 秒。
/// </summary>
public class SkillAidSkyFall : Skill
{
    /// <summary>下落起始高度（世界单位）</summary>
    private const float FallHeight = 25f;
    /// <summary>下落耗时（秒），位移按 t² 加速（等效匀加速自由落体）</summary>
    private const float FallDuration = 0.55f;
    /// <summary>位移采样步长（秒）</summary>
    private const float StepTime = 0.025f;
    /// <summary>单目标时的落点距离（要求 &lt;10 米）</summary>
    private const float SingleTargetDistance = 9f;

    public SkillAidSkyFall(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;

        // Range 内敌人英雄
        var targets = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Range, owner.side)
            .FindAll(u => u != null && u.hp > 0 && u.isHero);
        if (targets.Count == 0)
            return false;

        // 第一目标：距自身最远的敌人英雄
        var primary = FindFarthest(targets, owner.transform.position);
        if (!CheckBurst(primary))
            return false;

        var castPos = CalcCastPos(primary);
        owner.noMoveCount++;
        owner.StartCoroutine(DropDown(castPos, primary));
        return true;
    }

    /// <summary>计算施法点：双目标取两者中点，单目标落在距其不到 10 的位置</summary>
    private Vector3 CalcCastPos(Chess primary)
    {
        var groundY = owner.baseY + owner.heightOffset;
        var primaryPos = primary.transform.position;

        // 第二目标：第一目标 Area 内距其最远的敌人英雄；没有则视为单目标
        var second = FindFarthestInArea(primary, primaryPos);
        if (second == null)
        {
            // 仅一个目标：从自身一侧落到距目标 SingleTargetDistance 处
            var dir = owner.transform.position - primaryPos;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f)
                dir = owner.transform.forward;
            var single = primaryPos + dir.normalized * SingleTargetDistance;
            single.y = groundY;
            return single;
        }

        // 两者中点作为施法点
        var mid = (primaryPos + second.transform.position) * 0.5f;
        mid.y = groundY;
        return mid;
    }

    /// <summary>取第一目标 Area 内、距第一目标最远的其他敌人英雄；没有则返回 null</summary>
    private Chess FindFarthestInArea(Chess primary, Vector3 primaryPos)
    {
        Chess farthest = null;
        var best = -1f;
        var units = WorldManager.Instance.GetEnemyInRange(primaryPos, skillCfg.Area, owner.side);
        foreach (var u in units)
        {
            if (u == null || u == primary || u.hp <= 0 || !u.isHero)
                continue;
            var d = FlatDistance(u.transform.position, primaryPos);
            if (d > best)
            {
                best = d;
                farthest = u;
            }
        }
        return farthest;
    }

    private static Chess FindFarthest(List<Chess> units, Vector3 from)
    {
        Chess farthest = null;
        var best = -1f;
        foreach (var u in units)
        {
            var d = FlatDistance(u.transform.position, from);
            if (d > best)
            {
                best = d;
                farthest = u;
            }
        }
        return farthest;
    }

    private static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    // 从天而降：正上方固定水平位置，按 t² 加速下落，落地结算范围伤害与自身护甲
    IEnumerator DropDown(Vector3 castPos, Chess primary)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        owner.LockTarget(primary);

        // 位移期间由协程接管位置，锁住寻路避免被互斥力/寻路拉走
        var groundY = owner.baseY + owner.heightOffset;
        castPos.y = groundY;
        owner.transform.position = new Vector3(castPos.x, groundY + FallHeight, castPos.z);

        var elapsed = 0f;
        while (elapsed < FallDuration)
        {
            if (owner == null)
                yield break;
            if (owner.hp <= 0)
            {
                owner.noMoveCount--;
                yield break;
            }

            var t = Mathf.Clamp01(elapsed / FallDuration);
            var pos = castPos;
            pos.y = groundY + FallHeight * (1f - t * t); // t² 缓动 = 匀加速下落
            owner.transform.position = pos;
            elapsed += StepTime;
            yield return new WaitForSeconds(StepTime);
        }

        if (owner == null)
            yield break;
        owner.transform.position = castPos;
        owner.noMoveCount--;

        // 落地：范围特效 + 范围伤害
        PlayAreaEffect(castPos);
        var skillDamage = GetSkillDamage();
        if (skillDamage > 0)
        {
            // 按落点由近到远排序，最多命中 TargetCount 个目标（0 表示不限）
            var targets = WorldManager.Instance.GetEnemyInRange(castPos, skillCfg.Area, owner.side)
                .FindAll(u => u != null && u.hp > 0);
            targets.Sort((a, b) => FlatDistance(a.transform.position, castPos)
                .CompareTo(FlatDistance(b.transform.position, castPos)));
            var maxTargets = skillCfg.TargetCount > 0 ? Mathf.Min(skillCfg.TargetCount, targets.Count) : targets.Count;
            for (var i = 0; i < maxTargets; i++)
            {
                var u = targets[i];
                u.OnSkillDamaged(owner, id, skillDamage);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(u, skillCfg.HitEffect);
            }
        }

        // 落地自身增防：按 BuffId 配置施加护甲 Buff
        foreach (var buffId in GetSkillBuffIds())
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
    }
}
