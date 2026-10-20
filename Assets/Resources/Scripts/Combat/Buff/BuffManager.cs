using System.Collections.Generic;
using System.Reflection;
using CommonConfig;

public static class BuffManager
{
    /// <summary>
    /// ScriptName → Buff 脚本类构造器缓存：AddBuff 按配置反射创建对应 Buff 子类，缓存避免重复反射查找。
    /// </summary>
    private static readonly Dictionary<string, ConstructorInfo> ctorCache = new Dictionary<string, ConstructorInfo>();

    public static void AddBuff(Chess target, Chess caster, int skillId, int buffId, float time)
    {
        SkillManager.OnAddBuff(target, caster, ref buffId, skillId, ref time);

        if(time == 0) //有的技能会先填0，等待buff
            return;

        // buff时长调整由职业技能事件钩子实现（SkillManager.OnAddBuff → 扇/琴的 ModifyBuffTime 技能，见 JobLinkManager）
        var buffCfg = BuffConfig.GetConfig(buffId);

        GameLog.Debug("AddBuff buffId=" + buffId.ToString() + " skillId=" + skillId.ToString() + " time=" + time.ToString());

        var ctor = GetBuffConstructor(buffCfg.ScriptName);
        if (ctor == null)
        {
            GameLog.Error("Buff not found " + buffCfg.ScriptName);
            return;
        }

        var buff = (Buff)ctor.Invoke(new object[] { buffId, skillId, caster, target, time });
        target.AddBuff(buff, caster, time);
    }

    /// <summary>
    /// 按 ScriptName 反射取 Buff 脚本类的 (int, int, Chess, Chess, float) 构造器（约定 ScriptName 即类名，如 "BuffShield"）。
    /// 未带 Buff 前缀时自动补全（兼容历史配置）；找不到类或构造器时记录日志并返回 null。
    /// </summary>
    private static ConstructorInfo GetBuffConstructor(string scriptName)
    {
        if (string.IsNullOrEmpty(scriptName))
            return null;
        if (ctorCache.TryGetValue(scriptName, out var cached))
            return cached;

        var typeName = scriptName.StartsWith("Buff") ? scriptName : "Buff" + scriptName;
        var type = typeof(Buff).Assembly.GetType(typeName);
        ConstructorInfo ctor = null;
        if (type != null && typeof(Buff).IsAssignableFrom(type))
            ctor = type.GetConstructor(new[] { typeof(int), typeof(int), typeof(Chess), typeof(Chess), typeof(float) });
        if (ctor == null)
            GameLog.Error("BuffManager: 未找到Buff脚本类 " + typeName + "（ScriptName=" + scriptName + "）");

        ctorCache[scriptName] = ctor;
        return ctor;
    }

    /// <summary>
    /// 按实例移除 Buff：共存模式(StackMode=1)下同 Id 允许多个实例，过期与刷新顶掉必须按实例删
    /// </summary>
    public static void RemoveBuff(Chess chess, Buff buff)
    {
        if (chess == null || buff == null)
            return;
        if (!chess.buffs.Remove(buff))
            return;
        buff.OnRemove(chess);
        // buff 移除事件分发：参考技能体系，经 Chess 虚方法派发到各技能（如天人守城护盾破爆炸）
        chess.OnBuffRemoved(buff);
    }

    /// <summary>
    /// 按 BuffId 移除该 Id 的全部实例（驱散、技能主动移除用）
    /// </summary>
    public static void RemoveBuff(Chess chess, int buffId)
    {
        if (chess == null)
            return;
        var toRemove = chess.buffs.FindAll(b => b.id == buffId);
        foreach (var buff in toRemove)
            RemoveBuff(chess, buff);
    }

    /// <summary>
    /// 驱散负面 Buff：移除目标身上所有"负面(IsPositive=false) 且 可驱散(CanDispel=true)"的 Buff，返回实际驱散数量。
    /// 英雄招牌类不可驱散 Buff（CanDispel=false，如连锁/叛逃/混乱）会被保留。
    /// </summary>
    public static int DispelNegative(Chess chess)
    {
        if (chess == null)
        {
            GameLog.Warn("DispelNegative 目标为空，跳过驱散");
            return 0;
        }

        // 先收集待移除的 buff 实例，再统一移除，避免遍历中改动 buffs 列表
        var toRemove = new List<Buff>();
        foreach (var buff in chess.buffs)
        {
            if (buff.buffCfg != null && !buff.buffCfg.IsPositive && buff.buffCfg.CanDispel)
                toRemove.Add(buff);
        }
        foreach (var buff in toRemove)
            RemoveBuff(chess, buff);
        return toRemove.Count;
    }

    /// <summary>
    /// 驱散正面 Buff：移除目标身上所有"正面(IsPositive=true) 且 可驱散(CanDispel=true)"的 Buff，返回实际驱散数量。
    /// 英雄招牌类不可驱散 Buff（CanDispel=false，如据守/复活/威震/狂暴等）会被保留。
    /// </summary>
    public static int DispelPositive(Chess chess)
    {
        if (chess == null)
        {
            GameLog.Warn("DispelPositive 目标为空，跳过驱散");
            return 0;
        }

        // 先收集待移除的 buff 实例，再统一移除，避免遍历中改动 buffs 列表
        var toRemove = new List<Buff>();
        foreach (var buff in chess.buffs)
        {
            if (buff.buffCfg != null && buff.buffCfg.IsPositive && buff.buffCfg.CanDispel)
                toRemove.Add(buff);
        }
        foreach (var buff in toRemove)
            RemoveBuff(chess, buff);
        return toRemove.Count;
    }

}
