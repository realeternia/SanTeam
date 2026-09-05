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
    /// <summary>主公技(王/帅)：所在同阵营护盾效果加倍（技能表 帅 的5级行，Id=原Id*10+级）</summary>
    public const int MasterShieldSkillId = 2000015;
    /// <summary>仁德：给与我方前排士兵护盾（技能表 仁 的5级行）</summary>
    public const int SoldierShieldSkillId = 2090035;
    /// <summary>连锁：锁定目标并传递伤害（技能表 锁 的5级行）</summary>
    public const int LockSkillId = 2030025;

    // ---- 默认护盾机制(同阵营连线，数值参考金铲铲神盾使羁绊) ----
    /// <summary>同阵营英雄数量档位(3/5/7/9)</summary>
    public static readonly int[] FactionShieldCounts = { 3, 5, 7, 9 };
    /// <summary>各档位对应的护盾(生命值百分比)</summary>
    public static readonly float[] FactionShieldRates = { 0.18f, 0.24f, 0.30f, 0.36f };
    /// <summary>护盾持续时间(整场战斗)</summary>
    public const float FactionShieldTime = 999f;
    /// <summary>主公技：护盾加倍倍率</summary>
    public const float MasterShieldDouble = 2f;

    /// <summary>主公(帅/王)在场：全队护盾额外加成（百分比）</summary>
    public const float KingShieldBonusRate = 0.1f;

    // ---- 抗性减伤公式（参考金铲铲：实际伤害 = 原伤害 × 100/(100+抗性)） ----
    /// <summary>抗性减伤基准值（减伤% = 抗性/(抗性+基准值)，如50点抗性≈减伤33%）</summary>
    public const float ResistBase = 100f;

    /// <summary>按抗性计算伤害系数（护甲/魔抗通用，抗性越高受到伤害越低）</summary>
    public static float ResistMultiplier(int resist)
    {
        return ResistBase / (ResistBase + resist);
    }

    // ---- 其他 ----
    /// <summary>近战/远程士兵射程判定阈值</summary>
    public const float MeleeRange = 30f;

    // ---- 连线(武将关系) ----
    /// <summary>连线好友数量档位(2/3/4/5/6/7)</summary>
    public static readonly int[] FriendLineCounts = { 2, 3, 4, 5, 6, 7 };
    /// <summary>各档位对应的攻击强化(百分比)</summary>
    public static readonly float[] FriendLineAtkRates = { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f, 0.30f };

    // ---- 兵种连锁 ----
    /// <summary>兵种默认技能起始等级（默认兵种技能1级，每多一个同兵种英雄+1级）</summary>
    public const int JobLinkBaseLevel = 1;

    // ---- 好友连锁·特殊 ----
    /// <summary>好友特殊(关联助益)技能起始等级（默认没有该技能=0级，每多一个好友+1级）</summary>
    public const int FriendSpecialBaseLevel = 0;

    // ---- 布阵图(5x5) ----
    // 布阵图坐标(索引 0~24, 行优先)：
    //   行0: 0  1  2  3  4      兵  兵  兵  兵  兵
    //   行1: 5  6  7  8  9      x  H  H  H  x
    //   行2: 10 11 12 13 14     x  H  H  H  x
    //   行3: 15 16 17 18 19     x  H  H  H  x
    //   行4: 20 21 22 23 24     弓  x  弓  x  弓
    /// <summary>布阵图边长(5x5)</summary>
    public const int FormationGridSize = 5;
    /// <summary>布阵图总格数</summary>
    public const int FormationCellCount = FormationGridSize * FormationGridSize; // 25
    /// <summary>近战小兵占用的布阵格(布阵图第0行全部5格)</summary>
    public static readonly int[] SoldierMeleeCells = { 0, 1, 2, 3, 4 };
    /// <summary>远程小兵占用的布阵格(布阵图第4行第1、3、5格)</summary>
    public static readonly int[] SoldierRangedCells = { 20, 22, 24 };
    /// <summary>英雄自动布阵占用的格(中间3x3区域，最多9格)</summary>
    public static readonly int[] HeroCells = { 6, 7, 8, 11, 12, 13, 16, 17, 18 };

    /// <summary>判断布阵格是否被小兵占用(小兵格不可布阵英雄)</summary>
    public static bool IsSoldierCell(int pos)
    {
        return System.Array.IndexOf(SoldierMeleeCells, pos) >= 0 || System.Array.IndexOf(SoldierRangedCells, pos) >= 0;
    }

    // ---- 玩家等级体系（参考金铲铲，节奏放慢一倍） ----
    /// <summary>玩家最高等级（10级后9个上阵格全解锁）</summary>
    public const int PlayerMaxLevel = 10;
    /// <summary>布阵图总格数(5x5)，上阵上限由 PlayerLevelConfig.SlotCount 控制(最多9)</summary>
    public const int PlayerMaxSlot = FormationCellCount;
    /// <summary>战斗获胜获得经验（参考金铲铲每回合2经验，节奏放慢一倍后胜利才给满）</summary>
    public const int BattleWinExp = 2;
    /// <summary>战斗失败获得经验（失败给一点）</summary>
    public const int BattleLoseExp = 1;
    // ---- 买经验（预留：金铲铲4金币买4经验，1金币=1经验；UI后续接入） ----
    /// <summary>购买经验所需金币</summary>
    public const int ExpBuyGoldCost = 4;
    /// <summary>购买一次获得的经验</summary>
    public const int ExpBuyAmount = 4;

    // ---- 士兵等级体系 ----
    /// <summary>士兵最高等级(30级，只提升士兵攻防加成；数量由玩家等级决定)</summary>
    public const int SoldierMaxLevel = 30;
    /// <summary>士兵升级所需金币</summary>
    public const int SodLvupGoldCost = 5;
    /// <summary>士兵最大数量：步兵5、弓兵3（10级玩家达成）</summary>
    public const int SoldierMaxMeleeCount = 5;
    public const int SoldierMaxRangedCount = 3;
}
