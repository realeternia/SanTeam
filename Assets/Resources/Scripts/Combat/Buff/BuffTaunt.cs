/// <summary>
/// 嘲讽（BuffTaunt）：持有者成为敌对单位强制优先攻击的目标。
/// 本身不产生属性效果，仅作为标记。索敌逻辑在 Chess.FindTarget 中读取该标记，把带嘲讽的存活单位作为最高优先级锁定目标。
/// </summary>
public class BuffTaunt : Buff
{
    public BuffTaunt(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }
}