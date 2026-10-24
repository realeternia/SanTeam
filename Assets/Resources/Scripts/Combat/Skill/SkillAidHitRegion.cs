using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 雷电阵（张角·落雷 / 张郃·震碎 共用，ScriptName = "SkillAidHitRegion"）：
/// 顺发——在目标位置召唤雷电阵（法术场可视化）并播放一次范围特效（特效本身持续时间较长），
/// 之后每 SummonHitInterval 秒对范围内敌人结算一次法术伤害，持续 SummonTime 秒；
/// 施法期间自身可自由移动/普攻，施法者死亡则结束。
/// 张角·落雷原地不动；张郃·震碎在 Strength2 配了 moveDist 时，法阵会在持续期间沿"施法者→落点"方向缓慢前移该总距离（逐跳均分，踩墙则停）。
/// 仿法正·明断，由 SkillManager.CheckAidSkill 自动循环施放。
/// </summary>
public class SkillAidHitRegion : Skill
{
    public SkillAidHitRegion(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var targetPos = target.transform.position;

        var field = SummonMagicField(targetPos, out var summonTime);

        // 范围特效一次性播放（该特效自身持续时间长，覆盖整个法阵持续时间）
        var areaFx = PlayAreaEffect(targetPos, summonTime);

        // 顺发（不进入引导）：放完法阵即可继续移动/普攻，由协程独立维持伤害
        owner.StartCoroutine(DelayDamage(targetPos, summonTime, field, areaFx));
        return true;
    }

    IEnumerator DelayDamage(Vector3 targetPos, float summonTime, Chess field, GameObject areaFx)
    {
        var term = (int)Math.Floor(summonTime / skillCfg.SummonHitInterval);

        // 法阵前移：Strength2 的 moveDist 为整段持续时间前进的总距离，逐跳均分；未配置时原地不动（张角·落雷）
        var moveStep = GetForwardStep(targetPos, term);
        var curPos = targetPos;

        // 记录召唤物/范围特效相对法阵中心的偏移（范围特效带落地 Y 偏移），移动时按偏移整体平移
        var fieldOffset = field != null ? field.transform.position - targetPos : Vector3.zero;
        var fxOffset = areaFx != null ? areaFx.transform.position - targetPos : Vector3.zero;

        for (int i = 0; i < term; i++)
        {
            // 施法者死亡则结束法阵
            if (owner == null || owner.hp <= 0)
                yield break;

            var unitsInRange = WorldManager.Instance.GetEnemyInRange(curPos, skillCfg.Area, owner.side);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage = GetSkillDamage(); // 固定系数 + 比例系数×关联属性
                foreach (var unit in unitsInRange)
                {
                    if (damage > 0)
                    {
                        unit.OnSkillDamaged(owner, skillId, damage);
                        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
                    }
                }
            }

            // 法阵在下一跳之前的整个时间窗内匀速前移（视觉连续、缓慢移动），而非每跳瞬移；
            // 落点踩墙则本跳原地不动，仍按 SummonHitInterval 等待
            if (moveStep != Vector3.zero)
            {
                var toPos = curPos + moveStep;
                toPos.y = curPos.y;
                bool canMove = field == null || !WorldManager.Instance.CheckPositionBlocked(field, toPos);
                if (canMove)
                {
                    var fromPos = curPos;
                    float elapsed = 0f;
                    while (elapsed < skillCfg.SummonHitInterval)
                    {
                        if (owner == null || owner.hp <= 0)
                            yield break;
                        elapsed += Time.deltaTime;
                        curPos = Vector3.Lerp(fromPos, toPos, Mathf.Clamp01(elapsed / skillCfg.SummonHitInterval));
                        SetFieldPos(curPos, field, areaFx, fieldOffset, fxOffset);
                        yield return null;
                    }
                    curPos = toPos;
                    SetFieldPos(curPos, field, areaFx, fieldOffset, fxOffset);
                    continue; // 本跳时间窗已用完，直接进入下一跳
                }
            }

            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }

    /// <summary>把召唤的法术场与范围特效按各自偏移整体平移到法阵中心 pos</summary>
    private void SetFieldPos(Vector3 pos, Chess field, GameObject areaFx, Vector3 fieldOffset, Vector3 fxOffset)
    {
        if (field != null)
            field.transform.position = pos + fieldOffset;
        if (areaFx != null)
            areaFx.transform.position = pos + fxOffset;
    }

    /// <summary>法阵每跳前进的位移向量：总距离取 Strength2 的 moveDist，沿"施法者→落点"水平方向均分到 term 跳；未配置返回零向量</summary>
    private Vector3 GetForwardStep(Vector3 targetPos, int term)
    {
        var moveDist = GetStr2("moveDist");
        if (moveDist <= 0f || term <= 0)
            return Vector3.zero;

        var dir = targetPos - owner.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = owner.transform.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        return dir.normalized * (moveDist / term);
    }
}
