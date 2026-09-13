using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 破甲：物理伤害无视目标一定比例护甲（10%~50%，随技能等级提升，Strength=0.1~0.5）。
/// 通过 GetArmorDelta(攻击方=无视对方护甲) 在伤害结算时按加法叠加折算实际护甲。
/// </summary>
public class SkillAttackArmorPierce : Skill
{
    public SkillAttackArmorPierce(int id, Chess unit) : base(id, unit)
    {
    }

    public override float GetArmorDelta(bool isAttackerSide)
    {
        // 破甲只作用于攻击方视角：无视对方护甲（增量=-Strength，护甲×1+delta，0=完全无视）
        if (!isAttackerSide)
            return 0f;
        return Mathf.Max(-1f, -skillCfg.Strength);
    }
}