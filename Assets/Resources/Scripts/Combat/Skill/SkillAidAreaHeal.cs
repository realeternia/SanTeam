using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 天书（ScriptName = "AidAreaHeal"）：辅助技能，在自身位置展开治疗领域（参考魔兽争霸3 丛林守护者的"宁静"）。
/// 领域存在 SummonTime 秒，每 SummonHitInterval 秒为领域内全部友方英雄回复一次生命，
/// 治疗量走独立治疗公式 GetSkillHeal()。由 SkillManager.CheckAidSkill 自动循环施放；领域召唤物用 SummonTag 区分。
/// 用于左慈：遁甲天书撒豆成兵、起死回生，术法所至，众军自愈。
/// </summary>
public class SkillAidAreaHeal : Skill
{
    public SkillAidAreaHeal(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        var center = owner.transform.position;
        var magicStub = SummonMagicField(center, out var summonTime);
        EffectManager.PlayPosSkillEffect(magicStub, center, skillCfg.Area, skillCfg.AreaEffect, summonTime);

        owner.StartCasting(id, AreaHeal(center, summonTime));
        return true;
    }

    private IEnumerator AreaHeal(Vector3 center, float duration)
    {
        var interval = Mathf.Max(0.1f, skillCfg.SummonHitInterval);
        var term = Mathf.Max(1, Mathf.FloorToInt(duration / interval));

        for (var i = 0; i < term; i++)
        {
            yield return new WaitForSeconds(interval);

            // 被打断/死亡：castingSkillId 被 BreakCasting 置0，结束治疗领域
            if (owner == null || owner.hp <= 0 || owner.castingSkillId != id)
                yield break;

            // 引导期间每跳持续播放施法动作
            owner.PlayerAnim(skillCfg.Action);

            // 领域内全部友方英雄（含自身），每跳回复一次（独立治疗公式）
            var units = WorldManager.Instance.GetMySideInRange(center, skillCfg.Area, owner.side)
                .FindAll(x => x.isHero && x.hp < x.maxHp);

            var heal = GetSkillHeal();
            foreach (var unit in units)
            {
                if (heal > 0)
                    owner.HealTarget(unit, skillId, heal, true);
                EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
            }

            GameLog.Debug($"天书 技能id={id} 等级={Level} 第{i + 1}跳 每跳治疗={heal} 目标数={units.Count}");
        }
    }
}
