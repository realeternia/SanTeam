using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 焰衣/雷衣：挂在自身身上的持续伤害光环。存在期间每秒对自身 skillCfg.Area 半径内至多
/// skillCfg.TargetCount 名敌人造成 Skill.GetSkillDamage(skillCfg, 施法者) × StrengthBuff1 key "damageShare"
/// （伤害分摊，未配置时按 100%；通常按 BuffTime 配置为 1/持续秒数，如 5 秒=0.2），并播放 skillCfg.HitEffect。
/// Strength2 配 "hitBuff:短名,hitBuffTime:秒" 时，命中目标额外施加该减益 Buff（如护甲降低）。
/// 持续时间由 SkillComBuffSingle 施放时的 BuffTime 控制；技能侧配 TargetType=noStack 保证不会重叠多层。
/// </summary>
public class BuffFireCloak : Buff
{
    private Coroutine tickCoroutine;
    private float damageShare = 1f; // 伤害分摊：技能伤害按该比例分摊到每跳，未配置时按 100%
    private int hitBuffId;          // 命中额外施加的减益 Buff（Strength2 key "hitBuff"，0=不施加）
    private float hitBuffTime;      // 减益持续时间（Strength2 key "hitBuffTime"）

    public BuffFireCloak(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        if (skillCfg == null)
        {
            GameLog.Error("BuffFireCloak: 找不到技能配置 buffId=" + id);
            return;
        }
        damageShare = GetStrBuff("damageShare", 1f);
        InitHitBuff();
        tickCoroutine = chess.StartCoroutine(TickDamage(chess));
    }

    /// <summary>
    /// 解析命中附带减益：Strength2 的 hitBuff（Buff 短名）与 hitBuffTime（秒），未配置则忽略。
    /// </summary>
    private void InitHitBuff()
    {
        var hitBuffName = GetSlotStr(skillCfg.Strength2, "hitBuff");
        if (string.IsNullOrEmpty(hitBuffName))
            return;

        var hitCfg = BuffConfig.GetConfigByNameS(hitBuffName);
        if (hitCfg == null)
        {
            GameLog.Error("BuffFireCloak: 未找到命中减益 Buff 短名 " + hitBuffName + " 技能id=" + skillCfg.Id);
            return;
        }
        hitBuffId = hitCfg.Id;
        hitBuffTime = ConfigManager.GetSlotValue(skillCfg.Strength2, "hitBuffTime", 0f);
    }

    public override void OnRemove(Chess chess)
    {
        if (tickCoroutine != null)
        {
            chess.StopCoroutine(tickCoroutine);
            tickCoroutine = null;
        }
        base.OnRemove(chess);
    }

    private IEnumerator TickDamage(Chess chess)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (chess == null || chess.hp <= 0)
                yield break;

            var attrOwner = caster != null ? caster : chess;
            var dmg = (int)(Skill.GetSkillDamage(skillCfg, attrOwner) * damageShare);
            if (dmg <= 0)
                continue;

            // 限定最大伤害数（TargetCount，<=0 不限），并沿用技能的 SelectOnly / SelectFirst 选取规则
            var enemies = TargetSelector.Select(chess, chess.transform.position, skillCfg.Area, false,
                skillCfg.SelectOnly, skillCfg.SelectFirst, skillCfg.TargetCount);
            foreach (var e in enemies)
            {
                e.OnSkillDamaged(attrOwner, skillCfg.Id, dmg);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(e, skillCfg.HitEffect);
                // 减益不叠加：已有同 Id Buff 时等其过期再补，避免每秒刷出多层（破为共存模式）
                if (hitBuffId != 0 && hitBuffTime > 0 && !e.HasBuff(hitBuffId))
                    BuffManager.AddBuff(e, attrOwner, skillCfg.Id, hitBuffId, hitBuffTime);
            }
        }
    }

    /// <summary>
    /// 从 "k:v,k:v" 槽位字符串中取字符串值，未配置返回空串。
    /// </summary>
    private static string GetSlotStr(string slots, string key)
    {
        if (string.IsNullOrEmpty(slots))
            return "";
        foreach (var seg in slots.Split(new[] { ',', '，', ';', '；' }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            var idx = seg.IndexOf(':');
            if (idx <= 0)
                continue;
            if (seg.Substring(0, idx).Trim() == key)
                return seg.Substring(idx + 1).Trim();
        }
        return "";
    }
}
