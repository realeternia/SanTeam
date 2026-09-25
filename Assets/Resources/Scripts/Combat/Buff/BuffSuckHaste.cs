using System;

/// <summary>
/// 汲血快攻：攻击时按造成伤害的 Strength 比例吸血，并按 Strength2 提升攻速。
/// 用于离间（貂蝉）给单名友军的吸血+攻速组合 buff。
/// </summary>
public class BuffSuckHaste : Buff
{
    private float attackSpeedRateDiff;

    public BuffSuckHaste(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        attackSpeedRateDiff = skillCfg.Strength2;
        chess.attackSpeedRate += attackSpeedRateDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.attackSpeedRate -= attackSpeedRateDiff;
    }

    public override void OnAttack(Chess defender, int damage)
    {
        owner.HealTarget(owner, skillCfg.Id, (int)(damage * skillCfg.Strength), false); // 吸血不算治疗，不吃治疗加成
    }
}