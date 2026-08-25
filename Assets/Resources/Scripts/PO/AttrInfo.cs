using System;

[System.Serializable]
public class AttrInfo
{
    public int Might; // 无双强度（原武力 Str）
    public int Ap;    // 法术强度（原智力 Inte）
    public int Atk;   // 攻击（原统帅 Lead）
    public int Hp;

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
    }

}