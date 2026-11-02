/// <summary>
/// AI 常量：AI 选牌/评分/买卖/刷新/许愿/锁卡等策略相关数值统一在此维护（调整 AI 强弱优先改这里）
/// </summary>
public static class AIConst
{
    // ---- 许愿(like) ----
    /// <summary>AI 许愿品质4的概率(%)，其余情况许愿品质3</summary>
    public const int AiWishQuality4Rate = 75;

    // ---- 卖卡意向 ----
    /// <summary>卖卡意向倍率区间：按"同卡堆总价(单价×张数)÷本家最高总价"归一后线性映射，总价越高越不舍得卖</summary>
    public const float SellValueMultiplierMin = 1f;
    public const float SellValueMultiplierMax = 3f;

    // ---- 淘汰(品质1/2清仓) ----
    /// <summary>淘汰回合的最大偏移量：实际淘汰回合 = 配置基准 ± round((1-smart)×本值)，smart 越高偏移越小</summary>
    public const int EliminateRoundMaxOffset = 5;
    /// <summary>淘汰清仓每回合最多卖出的张数：逐回合渐进剔卡，避免单回合清空导致阵容断档</summary>
    public const int EliminateMaxSellPerRound = 1;

    // ---- 商店刷新 / 金币预算 ----
    /// <summary>"金币充裕"判定阈值：金币 >= 本值视为"钱多"，允许刷新与加大买经验预算（通用规则，不改配置类）</summary>
    public const int RichGoldThreshold = 12;
    /// <summary>买经验的金币预算比例：默认取 Normal，金币充裕("钱多")时取 Rich</summary>
    public const float NormalExpBuyGoldRate = 0.3f;
    public const float RichExpBuyGoldRate = 0.35f;

    // ---- 锁卡 ----
    /// <summary>锁卡门槛：买不起的卡中评分最高的那张，分数达到第二名的该倍数才锁定（品质3/4已在用的上阵卡不受此限制）</summary>
    public const float AiLockScoreRatio = 2f;
    /// <summary>每个商店阶段最多锁定的卡位数（独立买卡商店私有5格）；锁定时长为1回合，下回合结束自动解锁</summary>
    public const int AiLockMaxPerShop = 1;

    // ---- 英雄卡数量上限 ----
    /// <summary>软上限溢出时，非强卡新卡每多囤1张的拒买概率(%)（锁卡判定同样复用该上限配置）</summary>
    public const int AiOverLimitRejectPerCard = 35;

    // ---- 羁绊信任倍率 ----
    /// <summary>羁绊信任固定倍率（每档）：friend(2-6)好友连线加成最大、job(0-4)职业次之、force(0-2)强卡/国家护盾加成最小</summary>
    public const float FriendBias = 1.3f;
    public const float JobChainBias = 1.0f;
    public const float ForceBias = 0.6f;
    /// <summary>羁绊信任提升倍率：候选卡命中 Like 且该路是当前上阵人数最多的一路时替换上述基础倍率（friend 1.3→2.2、job 1.0→1.6）</summary>
    public const float FriendBiasTop = 2.2f;
    public const float JobChainBiasTop = 1.6f;
    /// <summary>好友"差一人"档收益：仅 1 名同组上阵时给的收益，拉平到接近成团档(0.8)以鼓励补齐第 2 人</summary>
    public const float FriendGainOneShort = 0.7f;
    /// <summary>好友连线好友数量档位(2/3/4/5/6，供好友收益评估取档)</summary>
    public static readonly int[] FriendLineCounts = { 2, 3, 4, 5, 6 };

    // ---- 聚焦(好友组/职业) ----
    /// <summary>聚焦目标核心卡品质门槛：拥有该品质的核心卡即触发对所属好友组/职业的聚焦</summary>
    public const int FocusCoreQuality = 4;
    /// <summary>聚焦目标名额上限（好友组与职业共用，通常 2 好友或 1 好友+1 职业）</summary>
    public const int FocusMaxTargets = 3;
    /// <summary>聚焦信任倍率：候选卡命中任一派生聚焦目标(好友组/职业)时追加的固定倍率（不要求命中 Like）</summary>
    public const float FocusBias = 1.2f;

    // ---- 评分倍率 ----
    /// <summary>品质偏好系数区间：按本回合商店出卡概率线性插值，最高概率品质取上限、最低取下限</summary>
    public const float QualityBiasMax = 2f;
    public const float QualityBiasMin = 0.5f;
    /// <summary>许愿偏好倍率：候选卡是"当前玩家自己许愿(点赞收藏池)的牌"时，拿取意愿乘本倍率</summary>
    public const float WishCardBias = 2f;
    /// <summary>品质4强卡的品质偏好下限：避免低等级阶段 Q4 概率最低被判到 QualityBiasMin，抬高史诗卡获取（阵容价值=价格×星级）</summary>
    public const float QualityBias4Floor = 1f;
    /// <summary>升星导向信任倍率：重复卡按"距下一星卡数"（越近越高）加权，把资源集中到核心卡升星（阵容价值=价格×星级，升星是乘法收益）</summary>
    public const float StarUpBias = 1.0f;
    /// <summary>升星导向中"预备席卡"的权重系数（上阵主力=1.0，预备席=本值；上阵卡的星级才直接计入阵容价值）</summary>
    public const float StarUpBenchFactor = 0.4f;
    /// <summary>价格偏好基准价与指数：score ×= (卡价/本基准)^本指数。阵容价值=价格×星级，星级受出卡随机性限制（多数卡只能到2星），价格是更可控的价值杠杆</summary>
    public const float CardPriceBiasBase = 6f;
    public const float CardPriceBiasExponent = 2f;
    /// <summary>热门卡线性惩罚：每多一名存活对手持有该卡，评分折损 = 本值 × PlayerConfig.OwnTooMuchCardRate（该值越大越回避热门卡）</summary>
    public const float HotCardPenaltyPerPlayer = 0.12f;
    /// <summary>热门卡惩罚下限：无论多少对手持有，评分最多折损到本值，避免热门核心卡被整体挤出候选池</summary>
    public const float HotCardPenaltyFloor = 0.55f;
}
