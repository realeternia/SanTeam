using CommonConfig;

public static class BuffManager
{
    public static void AddBuff(Chess target, Chess caster, int skillId, int buffId, float time)
    {
        var buffCfg = BuffConfig.GetConfig(buffId);
        SkillManager.OnAddBuff(target, caster, buffCfg, skillId, ref time);

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
            case "BuffLock":
                buff = new BuffLock(buffId, skillId, caster, target, time);
                break;
            case "BuffSuck":
                buff = new BuffSuck(buffId, skillId, caster, target, time);
                break;
            case "BuffDamagedAddRate":
                buff = new BuffDamagedAddRate(buffId, skillId, caster, target, time);
                break;
            case "BuffDamageAddRate":
                buff = new BuffDamageAddRate(buffId, skillId, caster, target, time);
                break;

        }

        if (buff == null)
        {
            UnityEngine.Debug.LogError("Buff not found");
            return;
        }

        foreach(var item in target.buffs)
        {
            if(item.id == buffId)
            {
                item.Refresh(caster, time);
                return;
            }
        }

        target.buffs.Add(buff);
        buff.OnAdd(target, caster);

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