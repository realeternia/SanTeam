/// <summary>
/// 嘲讽标记（BuffTaunt）：仅作视觉/状态标记，本身不产生属性效果。
/// 嘲讽的索敌效果不再由 Chess.FindTarget 读取，而是由嘲讽类技能在释放瞬间调用 Skill.TauntEnemies
/// 把范围内敌人的当前目标直接改为嘲讽者（详见 Skill.cs）。
/// </summary>
public class BuffTaunt : Buff
{
    public BuffTaunt(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }
}