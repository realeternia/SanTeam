/// <summary>
/// 战斗常量：战斗相关的技能/Buff Id 与机制数值统一在此维护
/// </summary>
public static class CombatConst
{
    // ---- BuffId ----
    /// <summary>护盾(BuffShield)</summary>
    public const int ShieldBuffId = 300001;
    /// <summary>减伤盾(BuffShieldValue)</summary>
    public const int ShieldValueBuffId = 300002;
    /// <summary>连锁(BuffLock)</summary>
    public const int LockBuffId = 301002;

    // ---- 技能Id ----
    /// <summary>主公技(王/帅)：所在同阵营护盾效果加倍</summary>
    public const int MasterShieldSkillId = 200001;
    /// <summary>仁德：给与我方前排士兵护盾</summary>
    public const int SoldierShieldSkillId = 209003;
    /// <summary>连锁：锁定目标并传递伤害</summary>
    public const int LockSkillId = 203002;

    // ---- 默认护盾机制(同阵营连线，数值参考金铲铲神盾使羁绊) ----
    /// <summary>同阵营英雄数量档位(3/5/7/9)</summary>
    public static readonly int[] FactionShieldCounts = { 3, 5, 7, 9 };
    /// <summary>各档位对应的护盾(生命值百分比)</summary>
    public static readonly float[] FactionShieldRates = { 0.18f, 0.24f, 0.30f, 0.36f };
    /// <summary>护盾持续时间(整场战斗)</summary>
    public const float FactionShieldTime = 999f;
    /// <summary>主公技：护盾加倍倍率</summary>
    public const float MasterShieldDouble = 2f;

    // ---- 其他 ----
    /// <summary>近战/远程士兵射程判定阈值</summary>
    public const float MeleeRange = 30f;

    // ---- 连线(武将关系) ----
    /// <summary>连线好友数量档位(2/3/4/5/6/7)</summary>
    public static readonly int[] FriendLineCounts = { 2, 3, 4, 5, 6, 7 };
    /// <summary>各档位对应的攻击强化(百分比)</summary>
    public static readonly float[] FriendLineAtkRates = { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f, 0.30f };
}
