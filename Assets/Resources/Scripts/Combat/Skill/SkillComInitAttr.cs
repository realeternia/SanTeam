using UnityEngine;
using CommonConfig;

/// <summary>
/// 才干（ScriptName = "SkillComInitAttr"）：非模板技能，无 MP、无 CD、无任何事件钩子。
/// 开局 BattleBegin 时一次性给自身加固定初始属性，配置写在 Strength2：
/// "atk:10,armor:5,magicRes:5,hp:120"（Lv1 约为职业基准的 20%，每级按 1~5 倍递增，5 级封顶）。
/// 只用于 1 费英雄，属性为固定值，不参与装备/羁绊的百分比放大。
/// </summary>
public class SkillComInitAttr : Skill
{
    /// <summary>防止 BattleBegin 被重复调用时属性叠层</summary>
    private bool applied;

    public SkillComInitAttr(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        if (applied || owner == null || skillCfg == null)
            return;
        applied = true;

        var atk = (int)GetStr2("atk");
        var armor = (int)GetStr2("armor");
        var magicRes = (int)GetStr2("magicRes");
        var hp = (int)GetStr2("hp");

        owner.atk += atk;
        owner.armor += armor;
        owner.magicRes += magicRes;
        owner.maxHp += hp;
        owner.hp += hp;

        GameLog.Debug($"才干加成 技能id={id} 等级={Level} 攻击+{atk} 护甲+{armor} 法防+{magicRes} 生命+{hp} → 生命={owner.hp}/{owner.maxHp}");
    }
}
