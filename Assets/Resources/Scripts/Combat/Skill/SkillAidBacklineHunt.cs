using CommonConfig;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 徐晃·疾袭：远程射击优先打后排（射程内找出距离自身最远的敌方目标）
/// </summary>
public class SkillAidBacklineHunt : Skill
{
    public SkillAidBacklineHunt(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 疾袭：优先射击后排敌人
    /// </summary>
    public override bool CheckAidSkill()
    {
        var enemies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, true);
        Chess back = null;
        float md = -1f;
        if (enemies != null)
        {
            foreach (var e in enemies)
            {
                float d = UnityEngine.Vector3.Distance(owner.transform.position, e.transform.position);
                if (d > md)
                {
                    md = d;
                    back = e;
                }
            }
        }

        if (back == null || back.hp <= 0)
            return false;
        if (!CheckBurst(back))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        int dmg = GetSkillDamage() + (int)(GetSkillDamage() * skillCfg.Strength);
        WorldManager.Instance.CreateSpellMissile(owner, back, owner.transform.position, id, dmg, owner.hitEffect);
        return true;
    }
}
