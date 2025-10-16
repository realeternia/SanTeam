using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitAround : Skill
{
    public SkillHitAround(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti,ref int damageReal,  ref string effect)
    {
        if (CheckBurst(defender))
            effect = "";
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (isBurst)
        {
            owner.PlayerAnim(skillCfg.Action);

            var startPos = owner.transform.position;
            var targetPos = defender.transform.position;

            //创建一个hitEffect
            var hitEffect = EffectManager.PlaySkillEffect(defender, skillCfg.HitEffect);
            hitEffect.transform.forward = (targetPos - startPos).normalized;

            var unitsInRange = WorldManager.Instance.GetUnitsInRange(startPos, skillCfg.Range, owner.side, true);
            unitsInRange.Remove(defender);
            
            // 筛选startPos到targetPos方向，左右各60°开角内的单位
            if (unitsInRange.Count > 0)
            {
                Vector3 direction = (targetPos - startPos).normalized;
                List<Chess> filteredUnits = new List<Chess>();
                
                foreach (var unit in unitsInRange)
                {
                    Vector3 unitDirection = (unit.transform.position - startPos).normalized;
                    float angle = Vector3.Angle(direction, unitDirection);
                    
                    // 检查是否在左右各60°开角内（总共120°扇形）
                    if (angle <= 90f)
                    {
                        filteredUnits.Add(unit);
                    }
                }
                
                if (filteredUnits.Count > 0)
                {
                    WorldManager.Instance.RandomSelect(filteredUnits, skillCfg.TargetCount);
                    var damage2 = (int)(damage * skillCfg.SkillDamageRate);
                    foreach(var unit in filteredUnits)
                        unit.OnSkillDamaged(owner, skillId, damage2);
                }
            }
        }
    }

}
