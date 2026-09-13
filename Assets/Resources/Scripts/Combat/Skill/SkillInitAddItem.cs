using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 战斗开始获得道具技能：BattleBegin 时按配置发动概率(Rate)判定，
/// 命中则给所属玩家1个配置列 ItemId 指定的道具（0=不发放）。对应技能：仁者无敌(2010076~2010080)。
/// </summary>
public class SkillInitAddItem : Skill
{
    public SkillInitAddItem(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        if (skillCfg.ItemId <= 0)
            return;
        // 发动概率：Rate=0 恒发放；0<Rate<1 按概率判定
        if (skillCfg.Rate > 0 && SysRandom.Value >= skillCfg.Rate)
            return;
        var player = owner.GetPlayerInfo();
        if (player == null)
        {
            GameLog.Warn("InitAddItem 技能所属单位无玩家，无法发放道具 技能id=" + skillCfg.Id);
            return;
        }
        player.AddItemCard(skillCfg.ItemId);
        GameLog.Debug(string.Format("战斗开始发放道具：玩家{0} 获得道具{1}，技能id={2}", player.pid, skillCfg.ItemId, skillCfg.Id));
    }
}
