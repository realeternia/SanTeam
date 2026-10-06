using UnityEngine;

/// <summary>
/// 闪避 Buff：按 StrengthBuff1 最后一位（压缩数组末尾）作为概率完全闪避即将受到的伤害（触及时伤害倍率归零），
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
        // StrengthBuff1 为压缩数组：取最后一个元素作为闪避概率（空数组=不闪避）
        if (skillCfg.StrengthBuff1 == null || skillCfg.StrengthBuff1.Length == 0)
            return;

        var evadeChance = skillCfg.StrengthBuff1[skillCfg.StrengthBuff1.Length - 1];
        if (SysRandom.Value < evadeChance)
        {
            damageMulti = 0;
            WorldManager.Instance.AddBattleText("闪避", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
        }
    }
}
