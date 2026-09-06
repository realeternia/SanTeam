using System;

[System.Serializable]
public class AttrInfo
{
    public int Might; // 无双强度（原武力 Str）
    public int Ap;    // 法术强度（原智力 Inte）
    public int Atk;   // 攻击（原统王 Lead）
    public int Hp;
    // 装备扩展属性（金铲铲式基础组件）：护甲/魔抗为实际数值，攻速/暴击为比例（0.1=+10%）
    public int Armor;        // 护甲
    public int MagicRes;     // 魔抗
    public float AttackRate; // 攻速加成（比例，如 0.1=+10%）
    public float CritRate;   // 暴击率（比例，如 0.15=+15%）
    public float MpRegen;    // 法力回复/秒

    public int Total
    {
        get { return Might + Ap + Atk; }
    }

    public void AddAttr(AttrInfo attr)
    {
        Might += attr.Might;
        Ap += attr.Ap;
        Atk += attr.Atk;
        Hp += attr.Hp;
        Armor += attr.Armor;
        MagicRes += attr.MagicRes;
        AttackRate += attr.AttackRate;
        CritRate += attr.CritRate;
        MpRegen += attr.MpRegen;
    }

}