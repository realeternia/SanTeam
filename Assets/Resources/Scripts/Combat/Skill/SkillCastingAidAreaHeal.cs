using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 胡笳（ScriptName = "SkillCastingAidAreaHeal"）：辅助技能，持续施法，在自身位置展开治疗领域（参考魔兽争霸3 丛林守护者的"宁静"）。
/// 领域存在 SummonTime 秒，每 SummonHitInterval 秒为领域内友方单位（英雄与小兵）回复一次生命：
/// 按生命缺口（maxHp-hp）从大到小排序，最多治疗 TargetCount 个（0=不限）。
/// 治疗量走独立治疗公式 GetSkillHeal()。由 SkillManager.CheckAidSkill 自动循环施放；领域召唤物用 SummonTag 区分。
/// 用于蔡文姬：胡笳十八拍，闻者忘忧，众军自愈。
/// </summary>
public class SkillCastingAidAreaHeal : Skill
{
    public SkillCastingAidAreaHeal(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var center = owner.transform.position;
        SummonMagicField(center, out var summonTime);
        PlayAreaEffect(center, summonTime);

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

            // 领域内友方单位（英雄与小兵，含自身；排除领域召唤物本身），按生命缺口从大到小排序
            var units = WorldManager.Instance.GetMySideInRange(center, skillCfg.Area, owner.side)
                .FindAll(x => x.hp < x.maxHp && x.SummonTag != skillCfg.SummonTag);
            units.Sort((a, b) => (b.maxHp - b.hp).CompareTo(a.maxHp - a.hp));

            // 目标数上限：只回复生命缺口最大的前 TargetCount 个（0/负数表示不限）
            if (skillCfg.TargetCount > 0 && units.Count > skillCfg.TargetCount)
                units.RemoveRange(skillCfg.TargetCount, units.Count - skillCfg.TargetCount);

            var heal = GetSkillHeal();
            foreach (var unit in units)
            {
                if (heal > 0)
                    owner.HealTarget(unit, skillId, heal, true);
                EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
            }

            GameLog.Debug($"胡笳 技能id={id} 等级={Level} 第{i + 1}跳 每跳治疗={heal} 目标数={units.Count}");
        }
    }
}
