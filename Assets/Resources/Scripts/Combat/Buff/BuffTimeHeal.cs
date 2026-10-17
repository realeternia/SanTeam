using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 持续回血Buff（BuffConfig.NameS="愈"）：每秒回复一定生命值。
/// 每跳回血量优先取技能行 StrengthBuff1 的固定值（不随法强成长，如益壮；单独「愈」读 [0]，
/// 与其它 Buff 组合时按组合顺序顺延，如「硬,愈」读 [1]）；
/// 未配置 StrengthBuff1 时回退到统一治疗公式 Skill.GetSkillHeal（HealStrength × (100+法强)/100，如邓艾·偷渡阴平），首次施放时快照。
/// </summary>
public class BuffTimeHeal : Buff
{
    private Coroutine healCoroutine;
    private int healPerTick;

    public BuffTimeHeal(int id, int skillId, Chess caster, Chess target, float lastTime)
        : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        // 优先取技能行 StrengthBuff1 的固定每秒回血值（不随法强成长）：
        // 单独「愈」读 [0]；与其它 Buff 组合时按组合顺序顺延（如「硬,愈」：[0]=减伤率，回血值取 [1]）
        int slot = Skill.GetSkillBuffIds(skillCfg).Count > 1 ? 1 : 0;
        if (skillCfg.StrengthBuff1 != null && skillCfg.StrengthBuff1.Length > slot && skillCfg.StrengthBuff1[slot] > 0f)
            healPerTick = (int)skillCfg.StrengthBuff1[slot];
        else
            healPerTick = Skill.GetSkillHeal(skillCfg, caster ?? chess);
        healCoroutine = chess.StartCoroutine(HealOverTime(chess));
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        if (healCoroutine != null)
        {
            chess.StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
    }

    private IEnumerator HealOverTime(Chess chess)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (chess == null || chess.hp <= 0)
                yield break;

            if (chess.hp < chess.maxHp && healPerTick > 0)
            {
                // 持续回血不算治疗，不吃治疗加成/不触发治疗扩散（isHeal=false）
                (caster ?? chess).HealTarget(chess, skillCfg.Id, healPerTick, false);
                WorldManager.Instance.AddBattleText("+" + healPerTick.ToString(), chess.transform.position, new Vector2(0, 60), SysColor.Battle.HealColor, 2);
            }
        }
    }
}
