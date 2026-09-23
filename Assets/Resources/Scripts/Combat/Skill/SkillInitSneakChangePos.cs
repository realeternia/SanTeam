using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 偷袭：战斗开始时，对敌方随机1名英雄造成 Strength×(100 + ap×(1+Strength2))/100 的魔法伤害（DamageType=法术 受魔抗减免），
/// 并有 Rate 概率与其交换位置。每个敌方英雄整场只可能被交换一次（sneakSwapped 标记，防止多名偷袭者重复交换同一目标）。
/// 对应技能：偷袭高手组特殊连锁「偷」（2010071~2010075）。
/// </summary>
public class SkillInitSneakChangePos : Skill
{
    public SkillInitSneakChangePos(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        // 敌方存活且未被交换过的英雄中随机选一名作为偷袭目标（优先配对敌人，无配对敌人时取全部敌方英雄）
        var enemyHeroes = new List<Chess>();
        foreach (var chess in WorldManager.Instance.GetAllEnemys(owner.side))
        {
            if (chess.sneakSwapped == false)
                enemyHeroes.Add(chess);
        }
        if (enemyHeroes.Count == 0)
            return;

        var target = enemyHeroes[SysRandom.Range(0, enemyHeroes.Count)];

        // 按统一技能伤害公式计算魔法伤害
        var damage = GetSkillDamage();
        target.OnSkillDamaged(owner, skillId, damage);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        WorldManager.Instance.AddBattleText(damage + "!", target.transform.position, new Vector2(0, 60), Color.red, 3);

        if (SysRandom.Value >= skillCfg.Rate)
            return;

        target.sneakSwapped = true;
        var ownerPos = owner.transform.position;
        var targetPos = target.transform.position;
        owner.MoveTo(targetPos, true);
        target.MoveTo(ownerPos, true);
        WorldManager.Instance.AddBattleText("偷袭换位!", target.transform.position, new Vector2(0, 80), Color.yellow, 3);
    }
}
