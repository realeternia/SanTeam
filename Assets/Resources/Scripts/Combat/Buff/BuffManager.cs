using CommonConfig;

public static class BuffManager
{
    public static void AddBuff(Chess target, Chess caster, int skillId, int buffId, float time)
    {
        SkillManager.OnAddBuff(target, caster, ref buffId, skillId, ref time);

        if(time == 0) //有的技能会先填0，等待buff
            return;

        // buff时长调整由职业技能事件钩子实现（SkillManager.OnAddBuff → 扇/琴的 ModifyBuffTime 技能，见 JobLinkManager）
        var buffCfg = BuffConfig.GetConfig(buffId);

        GameLog.Debug("AddBuff buffId=" + buffId.ToString() + " skillId=" + skillId.ToString() + " time=" + time.ToString());

        Buff buff = null;
        switch (buffCfg.ScriptName)
        {
            case "BuffShield":
                buff = new BuffShield(buffId, skillId, caster, target, time);
                break;
            case "BuffShieldValue":
                buff = new BuffShieldValue(buffId, skillId, caster, target, time);
                break;
            case "BuffCoolDown":
                buff = new BuffCoolDown(buffId, skillId, caster, target, time);
                break; 
            case "BuffNoAction":
                buff = new BuffNoAction(buffId, skillId, caster, target, time);
                break;
            case "BuffNoMove":
                buff = new BuffNoMove(buffId, skillId, caster, target, time);
                break;
            case "BuffLock":
                buff = new BuffLock(buffId, skillId, caster, target, time);
                break;
            case "BuffSuck":
                buff = new BuffSuck(buffId, skillId, caster, target, time);
                break;
            case "BuffDamageAddRate":
                buff = new BuffDamageAddRate(buffId, skillId, caster, target, time);
                break;                
            case "BuffDamagedAddRate":
                buff = new BuffDamagedAddRate(buffId, skillId, caster, target, time);
                break;
            case "BuffSpeedDown":
                buff = new BuffSpeedDown(buffId, skillId, caster, target, time);
                break;
            case "BuffTimeDamage":
                buff = new BuffTimeDamage(buffId, skillId, caster, target, time);
                break;

        }

        if (buff == null)
        {
            GameLog.Error("Buff not found");
            return;
        }

        target.AddBuff(buff, caster, time);
    }

    /// <summary>
    /// 直接按指定护盾值添加护盾(默认同阵营护盾机制使用)
    /// </summary>
    public static void AddShield(Chess target, Chess caster, int hp, float time)
    {
        var buff = new BuffShield(CombatConst.ShieldBuffId, CombatConst.MasterShieldSkillId, caster, target, time);
        target.AddBuff(buff, caster, time);
        buff.SetHp(hp);
    }

    public static void RemoveBuff(Chess chess, int buffId)
    {
        for(int i = 0; i < chess.buffs.Count; i++)
        {
            if(chess.buffs[i].id == buffId)
            {
                chess.buffs[i].OnRemove(chess);
                chess.buffs.RemoveAt(i);
                break;
            }
        }
    }

}