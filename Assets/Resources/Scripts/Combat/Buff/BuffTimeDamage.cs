using System.Collections;
using UnityEngine;

/// <summary>
/// 持续伤害 Buff（如"败"溃败，短名"败"）：每 1 秒对携带者结算一次技能伤害。
/// 默认跳伤取施加技能的 StrengthBuff1 语义 key "dot"（绝对值，不走伤害公式、不随属性成长）；
/// 需要按实时数值结算的技能（如旋风斩流血）在首次 tick 前用 SetDamage 覆写。
/// </summary>
public class BuffTimeDamage : Buff
{
    private float damage;

    public BuffTimeDamage(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    private Coroutine damageCoroutine;

    /// <summary>
    /// 覆写每跳伤害（供技能按实时数值计算后写入，如旋风斩按命中伤害比例挂流血）：
    /// 覆盖 OnAdd 时按 StrengthBuff1 语义 key "dot" 取的默认跳伤，须在首次 tick(1 秒)前调用。
    /// </summary>
    public void SetDamage(int value)
    {
        damage = value;
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        // 默认跳伤取施加技能的 StrengthBuff1 语义 key "dot"（绝对值，不走伤害公式、不随属性成长；需要实时数值的技能用 SetDamage 覆写）
        damage = GetStrBuff("dot");

        // 启动伤害协程
        damageCoroutine = chess.StartCoroutine(DamageOverTime(chess, caster));
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        
        // 停止伤害协程
        if (damageCoroutine != null)
        {
            chess.StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    // 协程：每1秒造成伤害
    private IEnumerator DamageOverTime(Chess chess, Chess caster)
    {
        while (true)
        {
            // 等待1秒
            yield return new WaitForSeconds(1f);
            
            // 检查目标是否还存活
            if (chess.hp > 0)
            {
                // 造成Skill类型的伤害
                chess.OnSkillDamaged(caster, skillCfg.Id, (int)damage);
                WorldManager.Instance.AddBattleText("-" + ((int)damage).ToString(), chess.transform.position, new UnityEngine.Vector2(0, 60), SysColor.Battle.DamageColor, 2);
            }
            else
            {
                yield break;
            }
        }
    }

}