using System;

/// <summary>
/// 兼资（出将入相·兼资，短名"兼"）：攻击叠层。每层 AddStack() 提升 StrengthBuff1 语义 key "atk" 点攻击、"ap" 点法术强度；
/// 持续攻击刷新 4 秒窗口，停手时层数清零（BuffTime 到期自动移除）。OnRemove 按最终 stack 总量还原。
/// </summary>
public class BuffStackBuf : Buff
{
    public int stack;

    public BuffStackBuf(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        stack = 0;
    }

    // 叠层：每层提升攻击与法术强度，并刷新持续窗口
    public void AddStack()
    {
        stack++;
        if (owner != null)
        {
            owner.atk += (int)GetStrBuff("atk");
            owner.ap += (int)GetStrBuff("ap");
        }
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.atk -= (int)GetStrBuff("atk") * stack;
        chess.ap -= (int)GetStrBuff("ap") * stack;
    }
}