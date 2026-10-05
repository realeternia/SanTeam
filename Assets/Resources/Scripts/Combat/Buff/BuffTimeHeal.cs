using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 持续回血Buff（BuffConfig.NameS="愈"）：每秒回复一定生命值。
/// 每跳回血量优先取技能行 StrengthBuff1[0]（固定值，不随法强成长，如宝刀未老）；
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
        // 优先取技能行 StrengthBuff1[0]（固定每秒回血值，不随法强成长）；
        // 未配置时回退到统一治疗公式 Skill.GetSkillHeal（随法强成长）；属性主体取施加者 caster
        if (skillCfg.StrengthBuff1 != null && skillCfg.StrengthBuff1.Length > 0 && skillCfg.StrengthBuff1[0] > 0f)
            healPerTick = (int)skillCfg.StrengthBuff1[0];
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
