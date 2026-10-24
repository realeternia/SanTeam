/// <summary>
/// 隐匿（正面 Buff，短名"隐"，BuffConfig 300030）：嘲讽的反效果，本身不产生属性效果，只作状态标记——
/// Chess.FindTarget 收集目标时会跳过身上带该 Buff 的敌方单位，因此敌方普攻与单体技能(取 targetChess)都锁定不到；
/// 单位渲染层由 BuffConfig.Material(BuffStealth 半透明材质)切换成半透明。
/// 施加入口为甘宁·百骑劫营（每次释放给自己挂 5 秒）。
/// </summary>
public class BuffStealth : Buff
{
    public BuffStealth(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }
}
