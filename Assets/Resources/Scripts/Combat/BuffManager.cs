using CommonConfig;

public static class BuffManager
{
    public static void AddBuff(Chess target, Chess caster, int skillId, int buffId, float time)
    {
        var buffCfg = BuffConfig.GetConfig(buffId);

        Buff buff = null;
        switch (buffCfg.ScriptName)
        {
            case "BuffShield":
                buff = new BuffShield(buffId, skillId, target, time);
                break;
            case "BuffShieldRate":
                buff = new BuffShieldRate(buffId, skillId, target, time);                
                break;
            case "BuffNoAction":
                buff = new BuffNoAction(buffId, skillId, target, time);
                break;


        }

        if (buff == null)
            return;
        
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