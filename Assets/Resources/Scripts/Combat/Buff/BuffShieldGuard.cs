using CommonConfig;

/// <summary>
/// 龙胆 Buff（短名"胆"，BuffConfig 300034）：赵云·龙胆 套盾期间同步附加的状态（与护盾同时长）。
/// 携带者攻击力提升 StrengthBuff1[0] 点，且普攻命中时额外造成 Strength2[1]（随法强成长）的法术伤害，
/// 让"套盾"由纯防御转为攻守兼备；Buff 移除时攻击力原样加回。
/// </summary>
public class BuffShieldGuard : Buff
{
    private int atkDiff;

    public BuffShieldGuard(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        atkDiff = (int)skillCfg.StrengthBuff1[0];
        chess.atk += atkDiff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.atk -= atkDiff;
        base.OnRemove(chess);
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (defender == null || defender.hp <= 0)
            return;
        if (skillCfg.Strength2 == null || skillCfg.Strength2.Length < 2)
            return;
        var extra = (int)(skillCfg.Strength2[1] * (100 + owner.GetAttr("ap")) / 100f);
        if (extra > 0)
            defender.OnSkillDamaged(owner, skillCfg.Id, extra);
    }
}