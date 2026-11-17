using System.Collections;
using UnityEngine;

/// <summary>
/// 焰衣（孙坚·破阵）：挂在自身身上的持续伤害光环。存在期间每秒对自身 skillCfg.Area 半径内的所有敌人
/// 造成 Skill.GetSkillDamage(skillCfg, 施法者) × StrengthBuff1 key "damageRate"（伤害%，默认 100%）的技能伤害，
/// 并弹伤害飘字。持续时间由 SkillComBuffSingle 施放时的 BuffTime 控制；技能侧配 TargetType=noStack 保证不会重叠多层。
/// </summary>
public class BuffFireCloak : Buff
{
    private Coroutine tickCoroutine;
    private float damageRate = 1f; // 伤害%：对标技能伤害的倍率，未配置时按 100%

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
        damageRate = GetStrBuff("damageRate", 1f);
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
            var dmg = (int)(Skill.GetSkillDamage(skillCfg, attrOwner) * damageRate);
            if (dmg <= 0)
                continue;

            var enemies = WorldManager.Instance.GetEnemyInRange(chess.transform.position, skillCfg.Area, chess.side);
            foreach (var e in enemies)
            {
                if (e == null || e.hp <= 0)
                    continue;
                e.OnSkillDamaged(attrOwner, skillCfg.Id, dmg);
                WorldManager.Instance.AddBattleText("-" + dmg, e.transform.position, new Vector2(0, 60), SysColor.Battle.DamageColor, 2);
            }
        }
    }
}
