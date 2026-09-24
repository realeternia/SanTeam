using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 持续回血Buff（BuffConfig.NameS="愈"）：每秒回复一定生命值。
/// 每秒回血量 = 最大生命 × Strength + StrengthInt（固定值）。
/// 用于邓艾·偷渡阴平：残血时获得快速回血buff，背水一战逐步回血。
/// </summary>
public class BuffTimeHeal : Buff
{
    private Coroutine healCoroutine;

    public BuffTimeHeal(int id, int skillId, Chess caster, Chess target, float lastTime)
        : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
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

            if (chess.hp < chess.maxHp)
            {
                int heal = Mathf.CeilToInt(chess.maxHp * skillCfg.Strength) + skillCfg.StrengthInt;
                if (heal > 0)
                {
                    // 持续回血不算治疗，不吃治疗加成/不触发治疗扩散（isHeal=false）
                    (caster ?? chess).HealTarget(chess, skillCfg.Id, heal, false);
                    WorldManager.Instance.AddBattleText("+" + heal.ToString(), chess.transform.position, new Vector2(0, 60), SysColor.Battle.HealColor, 2);
                }
            }
        }
    }
}
