using System.Collections;
using UnityEngine;

public class BuffTimeDamage : Buff
{
    private float damage;

    public BuffTimeDamage(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    private Coroutine damageCoroutine;

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        // 与 Skill.GetSkillDamage 保持一致：魔法 = Strength × (100 + ap × (1+Strength2)) / 100；物理/真实 = Strength + atk × (1+Strength2)
        damage = skillCfg.DamageType == CombatConst.DamageTypeMagic
            ? skillCfg.Strength * (100 + caster.GetAttr("ap") * (1 + skillCfg.Strength2)) / 100
            : skillCfg.Strength + caster.GetAttr("atk") * (1 + skillCfg.Strength2);
        
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