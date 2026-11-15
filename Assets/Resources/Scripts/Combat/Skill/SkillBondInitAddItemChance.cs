using CommonConfig;

/// <summary>
/// 战斗开始按概率获得道具技能（ScriptName = "SkillBondInitAddItemChance"）：BattleBegin 时按 skillCfg.Rate 判定，
/// 命中则给所属玩家发放 skillCfg.ItemId 指定的道具（0=不发放）。
/// 上一局战斗失败时，发动概率 ×LoseRateBonus（+50%，越挫越勇）。
/// 概率可超过 100%：先保底发放 1 个，超出 100% 的部分继续判定，命中可再获得 1 个。
/// 当前无技能引用（原文采用此脚本，已改为 SkillBondInitAttrChange 的全队加魔抗），保留供后续概率赠道具类羁绊复用。
/// </summary>
public class SkillBondInitAddItemChance : Skill
{
    /// <summary>上一局战斗失败时发动概率的倍率（+50%）</summary>
    private const float LoseRateBonus = 1.5f;

    public SkillBondInitAddItemChance(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        if (skillCfg.ItemId <= 0)
        {
            GameLog.Error($"战斗开始获得道具技能缺少道具配置: 技能id={id}");
            return;
        }

        var player = owner.GetPlayerInfo();
        if (player == null)
        {
            GameLog.Warn("InitAddItemChance 技能所属单位无玩家，无法发放道具 技能id=" + skillCfg.Id);
            return;
        }

        // 发动概率：上一局失败时提升（+50%）
        float rate = skillCfg.Rate;
        if (player.lastBattleLose)
            rate *= LoseRateBonus;

        // 概率超过100%：先保底1个，超出部分再判定是否额外获得1个
        int count = 0;
        if (rate >= 1f)
        {
            count = 1;
            rate -= 1f;
        }
        if (SysRandom.Value < rate)
            count++;

        if (count <= 0)
            return;

        for (int i = 0; i < count; i++)
            player.AddItemCard(skillCfg.ItemId);

        GameLog.Debug($"战斗开始发放道具：玩家{player.pid} 获得{count}个道具{skillCfg.ItemId}，技能id={skillCfg.Id} 概率={(player.lastBattleLose ? skillCfg.Rate * LoseRateBonus : skillCfg.Rate):P0}");
    }
}
