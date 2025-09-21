using System;

[System.Serializable]
public class AttrInfo
{
    public int Str;
    public int Inte;
    public int Lead;
    public int Hp;

    public int Total
    {
        get { return Str + Inte + Lead; }
    }

    public void AddAttr(AttrInfo attr)
    {
        Str += attr.Str;
        Inte += attr.Inte;
        Lead += attr.Lead;
        Hp += attr.Hp;
    }

}