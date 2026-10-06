/// <summary>
/// 叛逃 Buff：携带者不受控制，持续朝初始(出生)位置后退，且移动速度减半。
/// fleeCount&gt;0 由 Chess.MoveAndFight 识别（不索敌/不攻击，改为朝 spawnPosition 后退）；
/// 添加时打断当前引导施法，移除时还原移动速度。
/// </summary>
public class BuffDefect : Buff
{
    // 本 buff 造成的移速削减量，移除时原样加回
    private float moveSpeedDiff;

    public BuffDefect(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        owner.fleeCount++;
        // 叛逃会打断目标当前的持续施法(引导)
        owner.BreakCasting();
        // 移动速度减半（保留 DefectMoveSpeedRate 比例）
        moveSpeedDiff = chess.moveSpeed * (1f - CombatConst.DefectMoveSpeedRate);
        chess.moveSpeed -= moveSpeedDiff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.moveSpeed += moveSpeedDiff;
        owner.fleeCount--;
        base.OnRemove(chess);
    }
}
