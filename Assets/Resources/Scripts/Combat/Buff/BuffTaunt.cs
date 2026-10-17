/// <summary>
/// 嘲讽（负面 Buff，短名"嘲"，BuffConfig 301010）：本身不产生属性效果，只作状态标记与索敌依据——
/// Chess.FindTarget 判断被挂者身上是否存在该 Buff，存在则强制把嘲讽者(BuffTaunt.caster)作为索敌目标，
/// 直到 Buff 到期/被驱散(净化)或嘲讽者阵亡。施加入口为 Skill.TauntEnemies（各嘲讽类技能复用）。
/// </summary>
public class BuffTaunt : Buff
{
    public BuffTaunt(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }
}
