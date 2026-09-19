using System;

[System.Serializable]
public class AttrInfo
{
    public int Ap;    // 法术强度（原智力 Inte）
    public int Atk;   // 攻击（原统王 Lead；无双强度已并入此处）
    public int Hp;
    // 装备扩展属性（金铲铲式基础组件）：护甲/魔抗为实际数值，攻速/暴击为比例（0.1=+10%）
    public int Armor;        // 护甲
    public int MagicRes;     // 魔抗
    public float AttackSpeedRate; // 攻速比例加成（如 0.1=+10%，乘算 attackSpeed）
    public float CritRate;   // 暴击率（比例，如 0.15=+15%）
    public float MpRegen;    // 法力回复/秒
    public float HpRegen;    // 生命回复/秒

    public int Total
    {
        get { return Ap + Atk; }
    }

    public void AddAttr(AttrInfo attr)
    {
        Ap += attr.Ap;
        Atk += attr.Atk;
        Hp += attr.Hp;
        Armor += attr.Armor;
        MagicRes += attr.MagicRes;
        AttackSpeedRate += attr.AttackSpeedRate;
        CritRate += attr.CritRate;
        MpRegen += attr.MpRegen;
        HpRegen += attr.HpRegen;
    }

}