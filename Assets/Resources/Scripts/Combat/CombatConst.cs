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

    // ---- 伤害标签(SkillConfig.HurtTag) ----
    /// <summary>AntiShield：绕过护盾直接打血（破盾类技能，BuffShield 判定该标签不吸收）</summary>
    public const string AntiShieldHurtTag = "AntiShield";
    /// <summary>LockChain：连锁(BuffLock)传递的伤害标签，避免锁链伤害二次扩散成循环放大</summary>
    public const string LockChainHurtTag = "LockChain";

    // ---- 伤害类型(SkillConfig.DamageType) ----
    /// <summary>0=法术：ap 成长，受魔抗减免</summary>
    public const int DamageTypeMagic = 0;
    /// <summary>1=物理：atk 成长，受护甲减免</summary>
    public const int DamageTypeAttack = 1;
    /// <summary>2=真实：无视抗性，护盾不吸收</summary>
    public const int DamageTypeReal = 2;

    // ---- 技能Id ----
    /// <summary>仁德：给与我方前排士兵护盾（技能表 仁 的5级行）</summary>
    public const int SoldierShieldSkillId = 2090035;
    /// <summary>连锁：锁定目标并传递伤害（技能表 锁 的5级行）</summary>
    public const int LockSkillId = 2030025;

    // ---- 国家护盾机制(同阵营连线，数值参考金铲铲神盾使羁绊) ----
    /// <summary>同阵营英雄数量档位(2/3/4/5/6，对应国家护盾技能 Lv1~5)</summary>
    public static readonly int[] FactionShieldCounts = { 2, 3, 4, 5, 6 };
    /// <summary>国家护盾技能缩写（技能Id按 Lv 取 SkillConfig 2000001~2000005，护盾=最大生命×Strength）</summary>
    public const string FactionShieldSkillSname = "国";

    /// <summary>主公(王/王)上阵：同阵营护盾额外加成（百分比，国家护盾技能内结算）</summary>
    public const float KingShieldBonusRate = 0.1f;

    // ---- 抗性减伤公式（参考金铲铲：实际伤害 = 原伤害 × 100/(100+抗性)） ----
    /// <summary>抗性减伤基准值（减伤% = 抗性/(抗性+基准值)，如50点抗性≈减伤33%）</summary>
    public const float ResistBase = 100f;

    /// <summary>按抗性计算伤害系数（护甲/魔抗通用，抗性越高受到伤害越低）</summary>
    public static float ResistMultiplier(int resist)
    {
        return ResistBase / (ResistBase + resist);
    }

    // ---- 移动避障(转向制：互斥力+短程寻路，不再锁单位格子) ----
    /// <summary>单位间距小于该值(米)时触发分离推力，防止贴脸卡住</summary>
    public const float MoveSeparationDist = 9f;
    /// <summary>敌方(非目标)单位分离推力权重(0~1，线性衰减，顺滑擦身而过)</summary>
    public const float MoveSeparationForce = 1.0f;
    /// <summary>同阵营单位恒定推力(必须>1才能压住前进意图，防止跟屁股堆叠黏在一起)</summary>
    public const float MoveSeparationAllyForce = 1.4f;
    /// <summary>短程寻路重规划间隔(秒)：目标远距离移动过程中环境变化大，定期重算</summary>
    public const float MoveReplanInterval = 0.5f;
    /// <summary>单位侧向错位权重(拥挤时叠加横向分量打散同向队列；独行时不偏移)</summary>
    public const float MoveLaneBias = 0.4f;
    /// <summary>短程寻路最大搜索深度(格)，超过预算取最接近目标的一步继续推进</summary>
    public const int MovePathMaxDepth = 6;

    // ---- 技能召唤物(士兵Id) ----
    /// <summary>法术场(501001)：技能场/火攻场/火墙的召唤物载体（类型用 SummonTag 区分，如"火"/"雷"）</summary>
    public const int SoldierMagicField = 501001;
    /// <summary>影子(501002)：分兵/影技能召唤的分身</summary>
    public const int SoldierShadow = 501002;

    // ---- 其他 ----
    /// <summary>近战/远程士兵射程判定阈值</summary>
    public const float MeleeRange = 30f;

    // ---- 连线(武将关系) ----
    /// <summary>连线好友数量档位(2/3/4/5/6，对应连线技能 Lv1~5)</summary>
    public static readonly int[] FriendLineCounts = { 2, 3, 4, 5, 6 };
    /// <summary>连线(默认连接)技能缩写（技能Id按 Lv 取 SkillConfig 2000006~2000010，攻击强化比例配在 LinkSelf）</summary>
    public const string FriendLineSkillSname = "友";
    /// <summary>好友·每回合金币技能缩写（技能Id按 Lv 取 SkillConfig 2010091~2010095；Dumb技能，金币在回合发钱时结算，不在战斗内生效）</summary>
    public const string FriendGoldSkillSname = "济";
    /// <summary>好友·每回合金币技能的每人金币数（每名上阵同组英雄+1金）</summary>
    public const int FriendGoldPerMember = 1;
    /// <summary>好友·每回合金币技能的最少上阵人数（1人不成团，默认2人起生效）</summary>
    public const int FriendGoldMinCount = 2;

    // ---- 战斗开始获得道具（InitAddItemChance） ----
    /// <summary>上一局战斗失败时，战斗开始获得道具技能的发动概率倍率（+50%）</summary>
    public const float InitAddItemLoseRateBonus = 1.5f;

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
    /// <summary>战斗获胜获得经验（胜利给3，失败给2）</summary>
    public const int BattleWinExp = 3;
    /// <summary>战斗失败获得经验（失败给2）</summary>
    public const int BattleLoseExp = 2;
    /// <summary>背包英雄卡上限（最多持有15种不同英雄，重复卡计入经验不占位）</summary>
    public const int PlayerMaxHeroCards = 15;
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
