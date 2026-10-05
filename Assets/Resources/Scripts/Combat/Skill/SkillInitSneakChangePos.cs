using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 偷袭：战斗开始1秒后，对敌方射程&gt;20（远程）且护甲最低的1名英雄造成 DamageStrength×(100 + ap)/100 的魔法伤害（走统一公式 GetSkillDamage，DamageType=法术 受魔抗减免），
/// 并有 Rate 概率与其交换位置。每个敌方英雄整场只可能被交换一次（sneakSwapped 标记，防止多名偷袭者重复交换同一目标）。
/// 对应技能：偷袭高手组特殊连锁「偷」（2010071~2010075）。
/// </summary>
public class SkillInitSneakChangePos : Skill
{
    /// <summary>偷袭发动延迟(秒)：战斗开始后延迟1秒再结算，避免开场瞬间换位</summary>
    private const float SneakDelay = 1f;

    /// <summary>远程判定阈值：射程大于该值的英雄视为远程（近战为17）</summary>
    private const float RangedThreshold = 20f;

    public SkillInitSneakChangePos(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.StartCoroutine(SneakAfterDelay());
    }

    private IEnumerator SneakAfterDelay()
    {
        yield return new WaitForSeconds(SneakDelay);
        if (owner == null || owner.hp <= 0)
            yield break;

        // 敌方存活且未被交换过的远程英雄（射程>20）中，选护甲最低者作为偷袭目标
        Chess target = null;
        foreach (var chess in WorldManager.Instance.GetAllEnemys(owner.side))
        {
            if (chess.sneakSwapped || chess.attackRange <= RangedThreshold)
                continue;
            if (target == null || chess.armor < target.armor)
                target = chess;
        }
        if (target == null)
            yield break;

        // 按统一技能伤害公式计算魔法伤害
        var damage = GetSkillDamage();
        target.OnSkillDamaged(owner, skillId, damage);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        WorldManager.Instance.AddBattleText(damage + "!", target.transform.position, new Vector2(0, 60), Color.red, 3);

        if (SysRandom.Value >= skillCfg.Rate)
            yield break;

        target.sneakSwapped = true;
        var ownerPos = owner.transform.position;
        var targetPos = target.transform.position;
        owner.MoveTo(targetPos, true);
        target.MoveTo(ownerPos, true);
        WorldManager.Instance.AddBattleText("偷袭换位!", target.transform.position, new Vector2(0, 80), Color.yellow, 3);
    }
}
