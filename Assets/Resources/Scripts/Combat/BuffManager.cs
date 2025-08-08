using CommonConfig;

public static class BuffManager
{
    public static void AddBuff(Chess chess, Chess caster, int buffId, float time, int strength = 0)

    {
        var buffCfg = BuffConfig.GetConfig((uint)buffId);

        Buff buff = null;
        switch (buffCfg.ScriptName)
        {
            case "BuffShield":
                buff = new BuffShield(buffId, chess, time, strength);
                break;

        }

        if (buff == null)
            return;
        
        foreach(var item in chess.buffs)
        {
            if(item.id == buffId)
            {
                item.Refresh(caster, time, strength);
                return;
            }
        }

        chess.buffs.Add(buff);
        buff.OnAdd(chess, caster);

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