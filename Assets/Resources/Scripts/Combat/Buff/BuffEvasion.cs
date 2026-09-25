using UnityEngine;

/// <summary>
/// 闪避 Buff：按 Strength2 概率完全闪避即将受到的伤害（触及时伤害倍率归零），
/// 触发时在受击位置播放"闪避"提示飘字。
/// </summary>
public class BuffEvasion : Buff
{
    public BuffEvasion(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        if (SysRandom.Value < skillCfg.Strength2)
        {
            damageMulti = 0;
            WorldManager.Instance.AddBattleText("闪避", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
        }
    }
}