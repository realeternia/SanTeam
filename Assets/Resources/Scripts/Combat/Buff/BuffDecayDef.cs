using System;

/// <summary>
/// 望族（名门望族·门阀）：开局双防强化，随时间衰减。OnAdd 增加 skillCfg.StrengthBuff1[0] 点护甲、skillCfg.StrengthBuff1[1] 点魔抗；
/// DecayOnce() 每跳减去初始加成 1/5（共5跳，15秒归零）；OnRemove 还原剩余加成。
/// 衰减节奏由施放技能 SkillInitDecayDef 的协程驱动。
/// </summary>
public class BuffDecayDef : Buff
{
    private float armorBonus;
    private float magicResBonus;
    private float armorStep;
    private float magicResStep;
    private int jumpsRemain = 5;

    public BuffDecayDef(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        armorBonus = skillCfg.StrengthBuff1[0];
        magicResBonus = skillCfg.StrengthBuff1[1];
        armorStep = armorBonus / jumpsRemain;
        magicResStep = magicResBonus / jumpsRemain;
        chess.armor += (int)armorBonus;
        chess.magicRes += (int)magicResBonus;
    }

    // 每跳衰减 1/5 初始加成（由技能协程每3秒调用一次，共5跳）
    public void DecayOnce()
    {
        if (jumpsRemain <= 0)
            return;
        armorBonus -= armorStep;
        magicResBonus -= magicResStep;
        jumpsRemain--;
        if (owner != null)
        {
            owner.armor -= (int)armorStep;
            owner.magicRes -= (int)magicResStep;
        }
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.armor -= (int)armorBonus;
        chess.magicRes -= (int)magicResBonus;
    }
}