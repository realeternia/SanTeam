using System.Collections;
using UnityEngine;

/// <summary>
/// 焰衣（孙坚·破阵）：挂在自身身上的持续伤害光环。存在期间每秒对自身 skillCfg.Area 半径内至多
/// skillCfg.TargetCount 名敌人造成 Skill.GetSkillDamage(skillCfg, 施法者) × StrengthBuff1 key "damageShare"
/// （伤害分摊，未配置时按 100%；焰衣按 BuffTime 配置为 1/持续秒数，如 5 秒=0.2），并对命中目标播放 skillCfg.HitEffect。
/// 持续时间由 SkillComBuffSingle 施放时的 BuffTime 控制；技能侧配 TargetType=noStack 保证不会重叠多层。
/// </summary>
public class BuffFireCloak : Buff
{
    private Coroutine tickCoroutine;
    private float damageShare = 1f; // 伤害分摊：技能伤害按该比例分摊到每跳，未配置时按 100%

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
        tickCoroutine = chess.StartCoroutine(TickDamage(chess));
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
            }
        }
    }
}
