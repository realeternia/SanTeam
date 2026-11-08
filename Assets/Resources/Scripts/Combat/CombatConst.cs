/// <summary>
/// 战斗常量：战斗相关的技能/Buff Id 与机制数值统一在此维护
/// </summary>
public static class CombatConst
{
    // ---- BuffId ----
    /// <summary>护盾(BuffShield)</summary>
    public const int ShieldBuffId = 300001;
    /// <summary>嘲讽(BuffTaunt，负面效果)：被挂者强制把嘲讽者(BuffTaunt.caster)作为索敌目标，索敌判定见 Chess.FindTarget</summary>
    public const int TauntBuffId = 301010;
    /// <summary>眩晕(BuffNoAction，负面效果，短名"乱")：无法行动；威震(BuffVengefulStance)普攻触发眩晕时使用</summary>
    public const int StunBuffId = 301001;
    /// <summary>陷阵(BuffNoMove，负面效果，短名"停")：无法移动；张昭·筑垒把敌方英雄拖回我方出生点后用它定身数秒</summary>
    public const int BuffNoMoveId = 301005;
    /// <summary>隐匿(BuffStealth，正面效果，短名"隐")：嘲讽的反效果，敌方索敌(Chess.FindTarget)与单体技能锁定不到；
    /// 甘宁·百骑劫营每次释放给自己挂 5 秒，并切半透明材质</summary>
    public const int StealthBuffId = 300030;

    // ---- Buff 叠加模式(BuffConfig.StackMode) ----
    /// <summary>1 共存：同 Id 允许多个实例并存，数值各自生效（如减甲、减攻）</summary>
    public const int BuffStackCoexist = 1;
    /// <summary>2 数值融合：保留单个实例，数值合并到同一 Buff（如护盾）</summary>
    public const int BuffStackMerge = 2;
    /// <summary>3 时间延长：保留单个实例与原数值，延长时长并按被控期间第 N 次延长衰减（0.9/0.8/…）</summary>
    public const int BuffStackExtend = 3;
    /// <summary>4 刷新：单实例，时间重置并用新实例顶掉旧实例（数值/施法者/配置全部换新）</summary>
    public const int BuffStackRefresh = 4;
    /// <summary>时间延长模式：每次延长的衰减步长（第1次×0.9、第2次×0.8…）</summary>
    public const float BuffStackExtendDecayStep = 0.1f;
    /// <summary>时间延长模式：单次延长的最低系数</summary>
    public const float BuffStackExtendMinFactor = 0.1f;

    // ---- 伤害标签(SkillConfig.HurtTag) ----
    /// <summary>AntiShield：绕过护盾直接打血（破盾类技能，BuffShield 判定该标签不吸收）</summary>
    public const string AntiShieldHurtTag = "AntiShield";
    /// <summary>LockChain：连锁(BuffLock)传递的伤害标签，避免锁链伤害二次扩散成循环放大</summary>
    public const string LockChainHurtTag = "LockChain";

    /// <summary>
    /// 是否派生伤害：连锁(LockChain)/破盾(AntiShield)这类由其他伤害二次派生的伤害。
    /// 派生伤害不得再生出派生伤害（连锁/生命链接/破盾），否则两种机制会互相触发形成无限递归
    /// </summary>
    public static bool IsDerivedHurtTag(string hurtTag)
    {
        return hurtTag == LockChainHurtTag || hurtTag == AntiShieldHurtTag;
    }

    // ---- 伤害类型(SkillConfig.DamageType) ----
    /// <summary>0=法术：ap 成长，受魔抗减免</summary>
    public const int DamageTypeMagic = 0;
    /// <summary>1=物理：atk 成长，受护甲减免</summary>
    public const int DamageTypeAttack = 1;
    /// <summary>2=真实：无视抗性，护盾不吸收</summary>
    public const int DamageTypeReal = 2;

    // ---- 技能初始MP ----
    /// <summary>战斗开始时技能MP是否直接拉满（默认关闭；开启则所有技能从满MP开局，关闭则按常规从0开始充能）</summary>
    public static bool FullMpAtBattleBegin = false;

    // ---- 法力获取（金铲铲式：普攻与受击获取，mpRegen 为百分比加成） ----
    /// <summary>每次普攻命中为攻击者提供的基础法力（未计 mpRegen 加成）</summary>
    public const float MpPerAttack = 10f;
    /// <summary>受到伤害时按伤害量获得法力的比例（0.05=每点伤害回 0.05 点法力，护盾吸收部分同样计入）</summary>
    public const float MpPerDamageRate = 0.05f;
    /// <summary>单次受击最多获得的法力（防止一次高伤害直接回满）</summary>
    public const float MpPerDamageCap = 20f;

    // ---- 调试阵容·开局Buff ----
    /// <summary>
    /// 调试阵容给武将挂Buff时使用的临时技能配置Id：Buff 的效果数值统一取自 SkillConfig，
    /// 该配置行数值全为0（见 WorldManager.ApplyDebugBuffs），仅保证 Buff 能正常构造，不产生数值影响
    /// </summary>
    public const int DebugBuffSkillId = 999999;
    /// <summary>调试阵容挂Buff的默认时长(秒)：条目未填时长时使用</summary>
    public const float DebugBuffDefaultTime = 10f;

    // ---- 国家护盾机制(同阵营连线，数值参考金铲铲神盾使羁绊) ----
    /// <summary>同阵营英雄数量档位(2/3/4/5/6，对应国家护盾技能 Lv1~5)</summary>
    public static readonly int[] FactionShieldCounts = { 2, 3, 4, 5, 6 };
    /// <summary>国家护盾技能缩写（技能Id按 Lv 取 SkillConfig 2000001~2000005，护盾=最大生命×Strength2[shield]）</summary>
    public const string FactionShieldSkillSname = "国";

    // ---- 抗性减伤公式（参考金铲铲：实际伤害 = 原伤害 × 100/(100+抗性)） ----
    /// <summary>抗性减伤基准值（减伤% = 抗性/(抗性+基准值)，如50点抗性≈减伤33%）</summary>
    public const float ResistBase = 100f;

    /// <summary>按抗性计算伤害系数（护甲/魔抗通用，抗性越高受到伤害越低）</summary>
    public static float ResistMultiplier(int resist)
    {
        return ResistBase / (ResistBase + resist);
    }

    // ---- 移动避障(转向制：互斥力+短程寻路，不再锁单位格子) ----
    /// <summary>敌方(非目标)单位分离推力权重(0~1，线性衰减，顺滑擦身而过)</summary>
    public const float MoveSeparationForce = 1.0f;
    /// <summary>同阵营单位恒定推力(必须>1才能压住前进意图，防止跟屁股堆叠黏在一起)</summary>
    public const float MoveSeparationAllyForce = 1.4f;
    /// <summary>短程寻路重规划间隔(秒)：目标远距离移动过程中环境变化大，定期重算</summary>
    public const float MoveReplanInterval = 0.5f;
    /// <summary>单位侧向错位权重(拥挤时叠加横向分量打散同向队列；独行时不偏移)</summary>
    public const float MoveLaneBias = 0.4f;
    /// <summary>"叛逃"移动速度保留比例：叛逃期间移动速度乘以本值(0.5=减半)</summary>
    public const float DefectMoveSpeedRate = 0.5f;
    /// <summary>短程寻路最大搜索深度(格)，超过预算取最接近目标的一步继续推进</summary>
    public const int MovePathMaxDepth = 6;

    // ---- 单位间距松弛(位置级硬约束：射程内/被位移后的单位同样互斥，防止堆叠) ----
    /// <summary>英雄实体尺寸(米)：对应 UnitHero 预制体水平缩放10；无头模拟器无预制体缩放时以此为准</summary>
    public const float HeroEntitySize = 10f;
    /// <summary>单位占位半径兜底值(米)：取不到实际模型尺寸时使用</summary>
    public const float DefaultUnitRadius = 3f;
    /// <summary>单位间距余量(米)：最小中心距 = 两单位占位半径之和 + 该余量</summary>
    public const float UnitSpacingPadding = 1.5f;
    /// <summary>单位间距邻居查询半径(米)：需覆盖最大单位直径+余量(英雄直径10米)，供邻居扫描</summary>
    public const float UnitSpacingQueryMeters = 15f;
    /// <summary>单位间距松弛每 tick 单次最大位移(米)：逐步推开，避免瞬移抖动</summary>
    public const float UnitSpacingMaxStep = 0.5f;

    // ---- 技能召唤物(士兵Id) ----
    /// <summary>法术场(501001)：技能场/火攻场/火墙的召唤物载体（类型用 SummonTag 区分，如"火"/"雷"）</summary>
    public const int SoldierMagicField = 501001;
    /// <summary>影子(501002)：分兵/影技能召唤的分身</summary>
    public const int SoldierShadow = 501002;

    // ---- 范围特效 ----
    /// <summary>范围特效缩放基准半径：EffectConfig.Scale 对应的半径，实际缩放 = Scale × Area / 本值</summary>
    public const float AreaEffectBaseRadius = 15f;
    /// <summary>挂在单位身上的特效（命中/技能/施法/buff）额外抬升高度：在 EffectConfig.OffsetY 基础上统一再上抬的世界高度；
    /// 地面范围特效(PlayPosSkillEffect)不适用，保持贴地</summary>
    public const float UnitEffectExtraOffsetY = 4f;

    // ---- 卡牌属性成长 ----
    /// <summary>星级成长倍率：每升1星 ×本值（乘方式，Lv1=×1.0、Lv2=×1.55、Lv3=×1.55²…），作用于 Atk/Ap/Hp</summary>
    public const float StarGrowthPerStar = 1.55f;
    /// <summary>品质系数表（按品质 Q1~Q4 直接取总值，非复利）：Q1=×1.0、Q2=×1.4、Q3=×1.8、Q4=×2.2，仅作用于 Atk/Hp</summary>
    public static readonly float[] QualityAttrFactors = { 1f, 1.4f, 1.8f, 2.2f };

    // ---- 英雄专属技能数值修正 ----
    /// <summary>技能品质系数表（按品质 Q1~Q4 直接取总值，非复利）：作用于英雄专属技能 DamageStrength/HealStrength，Q1=×1.0、Q2=×1.4、Q3=×1.8、Q4=×2.2</summary>
    public static readonly float[] QualitySkillFactors = { 1f, 1.6f, 2.2f, 3f };
    /// <summary>技能MP消耗修正基准：MpCost=本值时系数=1.0，伤害/治疗按 MpCost/本值 等比例放大或缩小（MP消耗越高、释放越慢，单次数值越高）</summary>
    public const float SkillMpStandard = 100f;
    /// <summary>技能MP消耗修正系数下限（MpCost很低时的最小倍率）</summary>
    public const float SkillMpFactorMin = 0.2f;
    /// <summary>技能MP消耗修正系数上限（MpCost很高时的最大倍率）</summary>
    public const float SkillMpFactorMax = 2f;

    // ---- 其他 ----
    /// <summary>近战/远程士兵射程判定阈值</summary>
    public const float MeleeRange = 25f;

    // ---- 好友连锁·特殊 ----
    /// <summary>好友特殊(关联助益)技能起始等级（默认没有该技能=0级，每多一个好友+1级）</summary>
    public const int FriendSpecialBaseLevel = 0;
    /// <summary>代友道具的 ItemConfig.Effect 标记：装备后视为指代羁绊组在场1人，参与人数计算、连线与关联技能授予</summary>
    public const string FriendProxyItemEffect = "friendrep";

    // ---- 疲劳机制(战斗超时加压：拖时间会被逐渐磨到残血，但不致死，最低扣到 1 血) ----
    /// <summary>疲劳开始时间(秒)：开战后经过该时长开始结算疲劳</summary>
    public const float FatigueStartTime = 30f;
    /// <summary>疲劳结算间隔(秒)</summary>
    public const float FatigueInterval = 5f;
    /// <summary>首次疲劳伤害</summary>
    public const int FatigueBaseDamage = 100;
    /// <summary>每次疲劳伤害的线性增量(100/200/300…)</summary>
    public const int FatigueDamageGrowth = 100;
}
