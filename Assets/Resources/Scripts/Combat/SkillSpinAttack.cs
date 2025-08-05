using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillSpinAttack : Skill
{
    public SkillSpinAttack(int id) : base(id)
    {
    }

    public override void OnAttack(Chess attacker, Chess defender, int damage)
    {
        Debug.Log("SkillSpinAttack");
        Vector2Int centerPos = WorldManager.Instance.WorldToGridPosition(attacker.transform.position, true);
        var unitsInRange = WorldManager.Instance.GetUnitsInRange(centerPos, 20, attacker.side, true);
        foreach(var unit in unitsInRange)
        {
            if(unit == defender)
                continue;
            unit.hp -= damage;
        }

        var hitPrefab = Resources.Load<GameObject>("Prefabs/" + skillCfg.HitEffect);

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, attacker.transform.position, hitPrefab.transform.rotation);
            // 设置特效的父对象为目标单位，使其跟随目标移动
            hitEffect.transform.parent = attacker.transform;
            hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);
            // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
            UnityEngine.Object.Destroy(hitEffect, 1f);



    }
}
