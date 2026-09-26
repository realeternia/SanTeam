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
            case "BuffTimeHeal":
                buff = new BuffTimeHeal(buffId, skillId, caster, target, time);
                break;
            case "BuffAtkAdd":
                buff = new BuffAtkAdd(buffId, skillId, caster, target, time);
                break;
            case "BuffSuckHaste":
                buff = new BuffSuckHaste(buffId, skillId, caster, target, time);
                break;
            case "BuffHasteMoveSpeed":
                buff = new BuffHasteMoveSpeed(buffId, skillId, caster, target, time);
                break;
            case "BuffBuildFort":
                buff = new BuffBuildFort(buffId, skillId, caster, target, time);
                break;
            case "BuffHitStun":
                buff = new BuffHitStun(buffId, skillId, caster, target, time);
                break;
            case "BuffAtkDown":
                buff = new BuffAtkDown(buffId, skillId, caster, target, time);
                break;
            case "BuffHealDown":
                buff = new BuffHealDown(buffId, skillId, caster, target, time);
                break;
            case "BuffArmorDown":
                buff = new BuffArmorDown(buffId, skillId, caster, target, time);
                break;
            case "BuffSlowDown":
                buff = new BuffSlowDown(buffId, skillId, caster, target, time);
                break;
            case "BuffEvasion":
                buff = new BuffEvasion(buffId, skillId, caster, target, time);
                break;
            case "BuffMultiShot":
                buff = new BuffMultiShot(buffId, skillId, caster, target, time);
                break;
            case "BuffNextAttacksMult":
                buff = new BuffNextAttacksMult(buffId, skillId, caster, target, time);
                break;
            case "BuffLifeStealAndDamage":
                buff = new BuffLifeStealAndDamage(buffId, skillId, caster, target, time);
                break;
            case "BuffFrenzy":
                buff = new BuffFrenzy(buffId, skillId, caster, target, time);
                break;

        }

        if (buff == null)
        {
            GameLog.Error("Buff not found");
            return;
        }

        target.AddBuff(buff, caster, time);
    }

    public static void RemoveBuff(Chess chess, int buffId)
    {
        for(int i = 0; i < chess.buffs.Count; i++)
        {
            if(chess.buffs[i].id == buffId)
            {
                var buff = chess.buffs[i];
                buff.OnRemove(chess);
                // buff 移除事件分发：参考技能体系，经 Chess 虚方法派发到各技能（如天人守城护盾破爆炸）
                chess.OnBuffRemoved(buff);
                chess.buffs.RemoveAt(i);
                break;
            }
        }
    }

}