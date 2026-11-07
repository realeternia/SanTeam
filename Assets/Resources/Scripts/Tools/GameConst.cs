/// <summary>
/// 玩法常量：非战斗的通用玩法规则（许愿/好友金币/布阵/等级经验/卡上限/买经验/士兵等级）统一在此维护
/// 战斗相关的技能/Buff Id 与机制数值仍放 CombatConst，AI 策略数值放 AIConst
/// </summary>
public static class GameConst
{
    // ---- 剧本(Scenario) ----
    /// <summary>新游戏的默认剧本Id（1 群雄割据；老存档为0，英雄池取剧本1+2并集）</summary>
    public const int DefaultScenarioId = 1;
    /// <summary>剧本配置缺失时的兜底开始年份</summary>
    public const int DefaultScenarioStartYear = 184;

    // ---- 许愿(wish)机制 ----
    /// <summary>每名玩家的许愿槽位数量（对应背包 loveBtn 的 3 个按钮）</summary>
    public const int WishSlotCount = 3;
    /// <summary>各许愿槽位解锁的回合（第 10/25/40 回合可分别选择一个英雄）</summary>
    public static readonly int[] WishUnlockRounds = { 10, 25, 40 };
    /// <summary>许愿英雄在商店的出卡权重倍率（正常为1，许愿为3倍）</summary>
    public const int WishHeroWeightMultiplier = 3;

    // ---- 好友·每回合金币(济) ----
    /// <summary>好友·每回合金币技能缩写（技能Id按 Lv 取 SkillConfig 2010091~2010095；Dumb技能，金币在回合发钱时结算，不在战斗内生效）</summary>
    public const string FriendGoldSkillSname = "济";
    /// <summary>好友·每回合金币技能的每人金币数（每名上阵同组英雄+1金）</summary>
    public const int FriendGoldPerMember = 1;
    /// <summary>好友·每回合金币技能的最少上阵人数（1人不成团，默认2人起生效）</summary>
    public const int FriendGoldMinCount = 2;

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
    /// <summary>近战小兵所在行(布阵图第0行，前排)</summary>
    public const int SoldierMeleeRow = 0;
    /// <summary>远程小兵所在行(布阵图第4行，后排)</summary>
    public const int SoldierRangedRow = 4;
    /// <summary>
    /// 小兵按数量(1~5人)在5列行内摆放的列号(0基，从左到右)：
    /// 1人居中(第3列)；2人占2、4列；3人占1、3、5列；4人占1~4列；5人满列
    /// </summary>
    public static readonly int[][] SoldierColumnsByCount =
    {
        new[] { 2 },
        new[] { 1, 3 },
        new[] { 0, 2, 4 },
        new[] { 0, 1, 2, 3 },
        new[] { 0, 1, 2, 3, 4 },
    };
    /// <summary>英雄自动布阵占用的格(中间3x3区域，最多9格)</summary>
    public static readonly int[] HeroCells = { 6, 7, 8, 11, 12, 13, 16, 17, 18 };

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
    /// <summary>玩家初始生命值：每轮战斗失败按 GameRoundConfig.DamageCount 扣除，生命≤0 淘汰移出游戏</summary>
    public const int PlayerInitLife = 100;

    // ---- 买经验（预留：金铲铲4金币买4经验，1金币=1经验；UI后续接入） ----
    /// <summary>购买经验所需金币</summary>
    public const int ExpBuyGoldCost = 4;
    /// <summary>购买一次获得的经验</summary>
    public const int ExpBuyAmount = 4;

    // ---- 士兵等级体系 ----
    /// <summary>士兵最高等级(30级，每级成长由 SoldierConfig.AtkP/HpP 决定；数量由玩家等级决定)</summary>
    public const int SoldierMaxLevel = 30;
    /// <summary>士兵额外升级(每级)所需金币</summary>
    public const int SodLvupGoldCost = 6;
}
