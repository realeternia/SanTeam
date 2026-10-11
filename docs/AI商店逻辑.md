# AI 商店逻辑

> 说明：本文描述 AI 玩家在**商店阶段**（选牌/买卡）的全部决策逻辑，包含商店模式分流、买经验、买卡评分链、品质偏好、羁绊加成、卖卡换卡。
> 代码位置：`Assets/Resources/Scripts/PlayerAI.cs`、`Assets/Resources/Scripts/Shop/*.cs`、`Assets/Resources/Scripts/CardShopManager.cs`、`Assets/Resources/Scripts/Combat/CombatConst.cs`
> 验证工具：`Tools/选牌模拟器`（零修改链接游戏源码的无头模拟器，30 回合冒烟可复现全部行为）

---

## 1. 相关文件

| 文件 | 职责 |
|---|---|
| `CardShopManager.cs` | 商店宿主：预制体/容器/布局、按 `GameRoundConfig.ShopType` 选模式、协程驱动、买卡入口 `BuyOffer`、开场收尾 |
| `Shop/ShopMode.cs` | 商店模式抽象基类 `ShopMode`（`Begin/GetOffers/OnHumanPass/OnHumanRefresh/Drive/End`，内置 `IsPanelBlocked()`）+ `ShopModeType` 枚举 + 统一报价 DTO `ShopOffer` |
| `Shop/SharedShopMode.cs` | 共享轮换模式（`ShopType=0`）：共享卡位、售出倒计时、相邻刷新 |
| `Shop/IndependentShopMode.cs` | 独立买卡模式（`ShopType=1`）：人类 5 格私有商店（`SLOT_COUNT=5`），AI 数据店无 UI |
| `PlayerAI.cs` | AI 决策核心：`ShopBegin`（买经验）、`AiCheckBuyCard`（买卡评分）、`FindWeakCard`（卖卡） |
| `Combat/CombatConst.cs` | 全部机制常量（倍率、上限、阈值），禁止在业务代码硬编码 |
| `Configs/PlayerConfig_s.cs` | 玩家性格与 AI 参数（同卡倍率、喜好、聪明度、经验系数等） |

---

## 2. 商店两种模式与 AI 参与方式

模式由 `GameRoundConfig.ShopType` 决定：`0=共享轮换`、`1=独立买卡`（14 个 PVE 回合用共享，86 个 PVP 回合用独立）。

### 2.1 共享轮换（`SharedShopMode`）

- 循环按积分低到高的顺序取当前行动玩家（`GetTurnPid()`），若为 AI 则调一次 `PlayerAI.AiCheckBuyCard(playerInfo, host.Era, offers)`：
  - 返回 `true` → 该玩家本次行动结束，`AfterAct()` → `NextTurn()` 轮转到下一个玩家
  - 返回 `false` → 进入刷新循环：只要"刷新次数 < `PlayerAI.GetShopRefreshLimit(玩家, AiSharedMaxRefreshPerShop)`"且 `PlayerAI.ShouldAiRefresh` 通过，就 `RefreshSharedBoard`（花 2 金重掷 6 张未售出卡，与人类按钮同一实现）后重估；**仍买不动**才 `playerPassed[pid] = true`、`passedPlayers++`、`SetRoundOver(true)`
  - 刷新上限 = `AiSharedMaxRefreshPerShop(=1)` + （金币充裕时）`RichExtraRefreshPerShop(=2)`，即"钱多"时本阶段最多刷 3 次
  - `passedPlayers >= 8` 时请求结束商店（`RequestEnd()`）
- 人类刷新不受"每次最多 1 次"限制（只要有钱可反复刷）。
- 被动换卡：某卡被买走后其相邻卡倒计时 -1、所有已售出卡倒计时 -1，归零换新（`SOLD_REMAIN_ROUNDS = 5`）。

### 2.2 独立买卡（`IndependentShopMode`）

- 人类（pid0）先操作自己的 5 格私有商店，结束后 AI 依次处理：
  ```
  if (AiCheckBuyCard(...)) continue;                          // 买得动就继续买
  if (refreshes < PlayerAI.GetShopRefreshLimit(player,        // 基础 ≤2 次；金币充裕再加 2
                  AiMaxShopRefreshPerShop)
      && PlayerAI.ShouldAiRefresh(player, offers))            // 收益评估 + 买得起
  { RefreshShopFor(pid); refreshes++; continue; }              // 花 2 金重掷全部 5 格
  break;
  ```
- AI 刷新门槛：次数上限 = `PlayerAI.GetShopRefreshLimit(玩家, AiMaxShopRefreshPerShop)` = `AiMaxShopRefreshPerShop(=2)` + （金币充裕时）`RichExtraRefreshPerShop(=2)`，即"钱多"时本商店最多刷 4 次；单价 `RefreshGoldCost = 2` 金，且 `ShouldAiRefresh` 通过。

### 2.3 AI 刷新判定 `PlayerAI.ShouldAiRefresh(playerInfo, offers)`

共享/独立共用。**金币充裕（`PlayerAI.IsRichGold`：`gold >= RichGoldThreshold(=15)`）时放开门槛1**，只要买得起就允许刷新；金币不充裕时**两道门槛同时满足才刷新**：

1. **收益评估：当前"可用卡"为 0** —— `PlayerAI.FilterOffers(玩家, offers).Count == 0`（金币充裕时此条失效，有可用卡也可刷）。
   "可用卡" = 买得起 + 英雄卡 + 非满级同卡 + 通过淘汰过滤，即买卡评分实际会考虑的候选（`FilterOffers` 与买卡评分**共用同一过滤链**，口径不分叉）。
   **关键前提**：刷新出的新卡与旧卡同源、品质概率相同 → **刷新不会提高品质期望**；所以"收益"只来自把"结构性无效"的一批卡（全满级同卡 / 全被淘汰过滤 / 全买不起）整体换掉。若已有可用卡，买不动属评分/换卡/存钱问题，刷新收益低，不刷。
2. **买得起：支付刷新费后剩余金币 ≥ `AiRefreshMinRemainGold(=6)`** —— `gold - RefreshGoldCost >= 6`，避免刷完没钱买。

---

## 3. AI 每回合执行顺序

`CardShopManager.ShopBegin()` 内，**先淘汰清仓与买经验、后生成商店**（升级会影响本回合商店品质）：

```
1. ShopCfg = GameRoundConfig.GetConfig(min(100, year + 1))；按 ShopCfg.RoundGold 发本回合金币
2. 各玩家 boughtThisShopCards.Clear()（重置"当回合已买入卡"，保证当回合买入的新卡当回合不卖）
3. SortPlayersByScore()（按积分排序，决定共享模式的行动顺序）
4. 各玩家 AutoCombineItems / AutoEquipItems / AutoUseItems（三方法内部只对 AI 生效）
5. year++（此后 year == ShopCfg.Id）；era = 0；BeginEraCommon()（OnEra 重置、记录开局金币、落后补金）
6. Mode = CreateMode(ShopCfg.ShopType)；Mode.Bind(this)
7. for pid 1..7: if (isAI) PlayerAI.ShopBegin(playerInfo)   // 淘汰清仓 + 买经验
8. Mode.Begin()                                             // 生成商店（独立模式 AI 数据店按自己等级 roll）
9. RefreshRateText()
10. StartCoroutine(Mode.Drive())                            // 驱动买卡
```

pid0 为人类，不参与 AI 逻辑（淘汰清仓与买经验都只对 AI 生效）。

---

## 3.5 淘汰清仓：`PlayerAI.EliminateOutdatedCards(PlayerInfo)`

**目的**：到达配置的淘汰回合后，AI 不再囤"低品质垃圾卡"，只专注主力阵容的升星与补强。

- 淘汰回合：`PlayerConfig.EliminateQuality1` / `EliminateQuality2` 为**基准值**，实际回合 = 基准 ± `round((1-smart) × EliminateRoundMaxOffset(=5))`
  - `smart = Intelligence / 99`：越聪明偏移越小（±1 以内），越笨偏移越大（最多 ±5）
  - 偏移方向按 `pid` 固定（偶数提前、奇数延后），一局内稳定不抖动
- 淘汰线 `GetEliminateMaxQuality()` 取已生效档位的较大值：到品质1回合 → 1（淘汰品质1卡）；到品质2回合 → 2（淘汰品质2及以下卡）
- **逐回合渐进卖出**：`EliminateOutdatedCards` 把"非主力上阵"且品质 ≤ 淘汰线的候选按**品质升序**排序，**每回合最多卖 `EliminateMaxSellPerRound(=1)` 张**，避免单回合清空导致阵容断档（在 `ShopBegin` 最前面执行，与 `nextSkip` 无关）
- **不再购买**：`AiCheckBuyCard` 里剔除"非主力上阵且品质 ≤ 淘汰线"的候选（见 5.1）
- 与喜好无关：即使该卡属于 `LikeJob` / `LikeForce` / `LikeFriend` 也一视同仁
- 不卖：主力上阵卡、主公（`IsKingHero`）、初始卡、当回合买入的新卡、**聚焦目标成员**、**正在成组的好友组成员**（同组已拥有 ≥ 2 张）
- 品质口径：`HeroConfig.Quality`（1~4）。品质是**卡自身固定属性**，与是否拥有/张数无关——因此未拥有的品质1/2卡同样被剔除，而品质3/4卡天然高于淘汰线，后期仍可买入补阵
- `基准值 = 0` 表示该玩家不启用该档淘汰（旺仔、怪物为 0）

---

## 4. 买经验：`PlayerAI.ShopBegin(PlayerInfo)`

```
round     = GameManager.Instance.year
expected  = round × ((BattleWinExp + BattleLoseExp) / 2 + playerConfig.ExpCheckOff)
                 = round × (2.5 + ExpCheckOff)
actual    = playerInfo.GetTotalExp()
if (actual >= expected) return;                      // 不落后就不买
prob      = min(0.7, (expected - actual) / expected) // 缺口越大触发概率越高，封顶 70%
if (SysRandom.Value >= prob) return;                 // 未命中
rate      = IsRichGold ? RichExpBuyGoldRate(0.35) : NormalExpBuyGoldRate(0.2) // 金币充裕时预算更高
budget    = floor(gold × rate)
times     = budget / ExpBuyGoldCost                  // 4 金 = 4 经验
if (gold >= ExpBuyGoldCost) times = max(1, times);   // 保底至少买 1 次
循环 playerInfo.BuyExp() 直到次数用尽或失败
```

要点：

- `GetTotalExp()` = 升到当前等级累计消耗的等级需求经验之和 + 当前 `exp`（`PlayerInfo.cs`）。
- `ExpCheckOff` 为 **float**，与 round 挂钩，仅正值，范围 0.2~1，弱的角色偏 0.2（见第 9 节数据表）。
- **预算比例按金币充裕程度分级**：金币充裕（`gold >= RichGoldThreshold(=15)`）时用 `RichExpBuyGoldRate(=0.35)`，否则 `NormalExpBuyGoldRate(=0.2)`——"钱多"时多买经验。
- **保底至少 1 次**：命中且金币 ≥ 4 时至少买 1 次。否则 `floor(金币×比例/4)` 常为 0，会出现"触发很多次但实际买得很少"（实测 48 次触发里 33 次空手）。
- 期望口径：系数 1 → 每回合期望 3.5 经验 → 30 回合缺 30 经验 → 约 7 次购买（30×1/4）。

---

## 5. 买卡：`PlayerAI.AiCheckBuyCard(PlayerInfo, era, List<ShopOffer>)`

### 5.1 候选过滤（不满足直接跳过）

| 过滤 | 条件 |
|---|---|
| 跳过标记 | `playerInfo.nextSkip == true` |
| 报价为空 | `offers` 为 null 或空 |
| 买不起 | 剔除 `gold < price` 的卡；全被剔除则 `return false` |
| 非英雄卡 | 商店不卖道具，只保留 `isHero` |
| 满级同卡 | 已有该卡且 `IsHeroCardMaxLevel`（Lv5）→ 剔除（买了纯亏金币） |
| 软上限拒买 | 非强卡（`Quality != 4`）**新卡**且 `英雄卡数 >= 上阵格 + Cardherolimit` 时，按超出张数概率拒买：`rand(0,100) < (超出张数) × AiOverLimitRejectPerCard(=35)` |
| 卖旧买新价格门槛 | 非强卡新卡超限且未被拒买时：无可卖弱卡 → 跳过；`新卡价格 < 弱卡价格` → 跳过；`year > 8 且 新卡价格 < 弱卡价格 + (year-8)` → 跳过 |
| 淘汰期 | 已到淘汰回合（见 3.5）且该卡**非主力上阵**、品质 ≤ 淘汰线 → 剔除（不再买；喜好职业/势力/好友组也不例外）。品质是卡自身属性，未拥有的品质1/2卡同样被剔除，品质3/4卡不受影响。**豁免**：`IsFocusMember`（聚焦目标成员）不受淘汰过滤——即使低品质，聚焦套卡成员仍可继续买 |

### 5.2 已移除的旧机制

> **变更记录**
> 1. 原先"低等级优先（`1+0.15×(4-同卡张数)`）""非上阵卡惩罚（`×0.7` / `×max(0.5-(year-8)×0.05+张数×0.1, 0.1)`）""价格偏好 `×1+buyBias`"三处已删除。`GetBuyCostBias` 方法一并删除；`AccumulatedCostBias` 现在只被卖卡侧 `GetSellCostBias` 使用。
> 2. **AI 卖卡次数上限 `AiMaxSellPerShop(=3)` 已删除**（常量、`PlayerInfo.aiShopSellCount` 字段、`CardShopManager` 中的重置都一并移除），改为"**当回合买入的新卡当回合不卖**"：`PlayerInfo.boughtThisShopCards` 记录本商店阶段买入的英雄卡，`FindWeakCard` 与淘汰清仓都跳过它们。
> 3. 原先"`weakHeroCard` 若已有 ≥2 张且 `rand(0,100) < 40 + 张数×20 - year×10` 就置空（不准卖）"的随机保护已删除，改为按**卡堆总价**评估卖卡意向（见 6.3）。

### 5.3 完整评分链

```
score = 1
 ├ 已有该卡  → × SameCardRate                                ← 仅保留同卡倍率
 ├ 英雄卡：
 │   ├ × GetQualityBias(品质, year, playerInfo)               ← 品质偏好（见 5.4）
 │   ├ × (价格 / CardPriceBiasBase)^CardPriceBiasExponent      ← 价格偏好（见 5.4.1）
 │   ├ 已有该卡 → × (1 + StarUpBias(0.8) × GetStarUpGain)      ← 升星导向（见 5.4.2）
 │   ├ LikeForce 命中 → × (1 + ForceBias(0.4)  × GetForceGain)
 │   ├ LikeJob   命中 → × (1 + JobBias        × GetJobGain)   ← JobBias = 1.3 或 0.8（见 5.5）
 │   ├ LikeFriend 命中 → × (1 + FriendBias     × GetFriendGain)← FriendBias = 1.8 或 1.1
 │   ├ 命中聚焦目标 → × (1 + FocusBias(1.2) × GetFocusGain)     ← 派生的好友组/职业聚焦（见 5.7），不要求命中 Like
 │   └ × (1 + BalanceFactor × GetBalanceGain × smart)          ← 远近搭配，缺哪类补哪类
 ├ 非已有卡 → × OwnTooMuchCardRate ^ (拥有该卡的玩家人数)        ← 热门卡惩罚
 └ × (1 + Rand(-1,1) × (1-smart) × 0.5)                       ← 聪明度抖动（越低越易手滑）
```

选卡与加权随机：

```
if (scoredCards.Count >= 5 && max(score) < 1.6f)
    按【价格】取前 3 张，分别 ×1.6 / ×1.4 / ×1.2   // 全员低分时向高价卡倾斜

排序降序 → pickPool = smart>=0.7 ? 3 : (smart>=0.4 ? 4 : 6) → 取前 pickPool 张
按 score 归一化后加权随机抽 1 张
```

- `smart = clamp(Intelligence, 1, 99) / 99`：越高，候选池越窄（只在最优几张里挑）、抖动越小。
- 若无卡可评（`scoredCards` 为空）：金币低于 `开局金币 × (0.2 + clamp(Futurerate,0,1) × max(0, futureBias) × 0.15)` 时 `return false`（跳过，存钱等下一回合品质更好）；否则所有候选给 1 分参与随机。
- `futureBias = clamp((下回合 Q3+Q4 概率 - 本回合 Q3+Q4 概率) / 20, -1, 1)`。

### 5.4 品质偏好系数 `GetQualityBias`

**口径：哪个品质在本回合越容易出，AI 就越倾向买这个品质，系数范围 0.5~2.0。**

- 概率源与真实刷牌完全一致：

| 模式 | 概率源 | 对应刷牌代码 |
|---|---|---|
| 共享轮换（`ShopType=0`） | `GameRoundConfig.GetConfig(year)` | `CardShopManager.GetRandomShopHeroId(ShopCfg)` → `HeroSelectionTool.GetRandomHeroIdByQuality` |
| 独立买卡（`ShopType=1`） | `PlayerLevelConfig.GetConfig(该玩家 level)` | `CardShopManager.GetRandomShopHeroIdByLevel(player.level)` → `IndependentShopMode.RollOffer` |

- 品质1概率 = `100 - Quality2Rate - Quality3Rate - Quality4Rate`。
- 映射：4 个品质概率做 min-max 归一，**概率最高 → `QualityBiasMax = 2.0`，概率最低 → `QualityBiasMin = 0.5`**，中间线性插值。
- **品质4 下限**：Q4 的结果再取 `max(bias, QualityBias4Floor(=1.0))` —— 低等级阶段 Q4 概率最低会被压到 0.5，设下限避免史诗卡被过度压制（阵容价值 = 价格×星级，Q4 为最高价）。
- 各品质概率相同（`max <= min`）或配置缺失 → 返回 1.0（不加不减）。

实测（模拟器日志 `Logs/game_*.txt`）：

| 场景 | Q1/Q2/Q3/Q4 概率 | 品质1 | 品质2 | 品质3 | 品质4 |
|---|---|---|---|---|---|
| PVE 第 1 回合（共享，Q2=Q3=Q4=0） | 100/0/0/0 | 2.00 | 0.50 | 0.50 | 0.50 |
| 玩家 Lv2（独立，Q2=15） | 85/15/0/0 | 2.00 | 0.76 | 0.50 | 0.50 |
| 玩家 Lv3（独立，Q2=25,Q3=10） | 65/25/10/0 | 2.00 | 1.08 | 0.73 | 0.50 |

> 注意：PVE 共享回合的 `GameRoundConfig` 品质概率目前全是 0（即 100% 品质1），此时所有卡都拿 2.0，该系数在 PVE 回合等价于无区分；PVP 独立回合才真正起作用。
> 另注意：品质4（强卡）在低等级概率最低，归一后本会被压到 0.5，现由 `QualityBias4Floor(=1.0)` 抬到 1.0，不再与"强卡超限豁免"对冲。

### 5.4.1 价格偏好（`score ×= (价格 / CardPriceBiasBase)^CardPriceBiasExponent`）

**口径：阵容价值 = 价格 × 星级；星级受出卡随机性限制（多数卡只能到 2 星），价格是更可控的价值杠杆**，故按 `(价格/6)^2` 放大高价卡权重（Q4 史诗 = 最高价）。

- 常量：`CardPriceBiasBase = 6`、`CardPriceBiasExponent = 2`（6 金卡系数 = 1.0，2 金卡 ≈ 0.11，8 金卡 ≈ 1.78）。

### 5.4.2 升星导向 `GetStarUpGain`（仅重复卡）

**口径：重复卡按"距下一星的卡数"越近收益越高，把资源集中到核心卡升星**（升星是乘法收益）。

```
已有该卡(未满级) → gain = 近度 × (上阵主力 ? 1.0 : StarUpBenchFactor(0.4))
近度 = 距下一星还差 1 张 ? 1.0 : 差 2 张 ? 0.6 : 差 3 张 ? 0.35 : 0.15
score ×= 1 + StarUpBias(0.8) × gain
以上阵格内的卡为准（`GetStrongCardList`），只有上阵卡的星级直接计入阵容价值；已满级 gain=0
```

- 常量：`StarUpBias = 0.8`、`StarUpBenchFactor = 0.4`。

### 5.5 羁绊（Like）加成

命中判定（三个都只在英雄卡上生效）：

| 路 | 命中条件 | 判定函数 |
|---|---|---|
| `LikeForce` | `HeroConfig.Side` ∈ `playerConfig.LikeForce` | `LikeHasForce` |
| `LikeJob` | `HeroConfig.Job`(= `JobConfig.NameS`) 映射回 `JobConfig.Id` 后 ∈ `LikeJob` | `LikeHasJob` |
| `LikeFriend` | 该英雄所属任一 `HeroFriendConfig.Id` ∈ `LikeFriend` | `LikeHasFriend` |

收益函数（**均以"是否新增上阵"为前提**）：

| 函数 | 口径 | 输出范围 |
|---|---|---|
| `GetForceGain` | `Quality==4` 直接 `1.0`；否则走 `GetSideGain` | 0 ~ 1.0（Q4） / 0 ~ 2.0（非Q4，达成 Lv5 档） |
| `GetSideGain` | 上阵同阵营人数推进 `FactionShieldCounts={2,3,4,5,6}` 档位：达成档 `1+0.25×(档-1)`；未达档 `count/nextNeed×0.4`；主公且同阵营≥1人 `+0.6` | 0 ~ 2.6 |
| `GetJobGain` | 上阵同职业人数 `0.3×(人数+1)`，满 5 人不再鼓励 | 0 ~ 1.5 |
| `GetFriendGain` | 与阵容内英雄连线：成团 `0.8+0.25×(档-1)`（档位 `FriendLineCounts={2,3,4,5,6}`）；差一人 `FriendGainOneShort=0.7`；特殊连锁组 `0.5+0.2×min(上阵成员,3)`；每回合金币组（`FriendGoldSkillSname="济"`）`+0.25`；普通好友组 `+0.3` | 0 ~ 2.9 |

**倍率与"上阵最多那一路"提升**：

| 路 | 基础倍率 | 提升后 | 提升条件 |
|---|---|---|---|
| Force | `ForceBias = 0.4` | 不变 | — |
| Job | `JobChainBias = 0.8` | `JobChainBiasTop = 1.3` | 候选卡职业 = `LikeJob` 中**上阵人数最多**的那一路（`IsTopLikeJob`） |
| Friend | `FriendBias = 1.1` | `FriendBiasTop = 1.8` | 候选卡所属好友组 = `LikeFriend` 中**上阵成员最多**的那一组（`IsTopLikeFriend`） |

单路最大倍率（按基础倍率）：Force `×2.04`、Job `×2.2`、Friend `×4.29`。
若 Job/Friend 命中"上阵最多那一路"（用提升倍率 1.3 / 1.8），则 Job 上限 `×2.95`、Friend 上限 `×6.22`。
（倍率上调目的：让好友/职业羁绊收益不被 `SameCardRate(1.5)`、`GetQualityBias(最高 2.0)` 盖过。）

### 5.6 "已有 / 上阵"的交互（重要）

- `isNewHero = !strongList.Contains(cardId)`，`strongList = playerInfo.GetStrongCardList(GetSlotCount())` 即**当前自动上阵阵容**。
- 候选卡**已在 `strongList`** 时：`GetJobGain = 0`、`GetFriendGain = 0`、`GetSideGain = 0`（仅 `Quality==4` 的 `GetForceGain` 仍给 1.0）。
  → **LikeJob / LikeFriend 对已上阵卡完全失效，LikeForce 仅对 Q4 生效**，因为再买只是升星、不新增羁绊人数。
- 候选卡**已有但在预备席**时：Like 加成全额生效，且与 `SameCardRate` 叠乘（布布/钱多 `SameCardRate = 5` 时同卡极易被反复选中）。

### 5.7 聚焦机制（好友组 / 职业）

**目的**：解决"好友羁绊几乎不成团"——逐卡贪心评分没有长期规划。聚焦把"我打算攒哪一组"显式化。

**派生式、无状态**：不写存档、不做显式重置。聚焦目标每回合从**当前已拥有的核心卡**重新派生，卖掉核心卡即自动释放，跨回合天然稳定。

- 核心卡 = 已拥有且 `HeroConfig.Quality == FocusCoreQuality(=4)` 的卡（**单张即触发**）。
- 候选：
  - **好友组候选**：每张核心卡所属各好友组（`ConfigManager.GetHeroFriendInfo`），统计该组"已拥有成员数 / 已拥有Q4数"，Q4数 ≥ 1 入候选。
  - **职业候选**：每张核心卡的 `HeroConfig.Job`，统计"已拥有同职业数 / Q4数"，Q4数 ≥ 1 入候选。
- 排序：`Q4数 desc → 已拥有成员数 desc → 档位进度 desc`（同分好友优先于职业），取前 `FocusMaxTargets(=2)`。
  - 好友与职业**共用名额**：靠排序自然产生"2 个好友组"或"1 好友 + 1 职业"。
- 生效方式：`BuildContext` 内一次性派生并存进 `AiContext.focusTargets`，供买卡评分与卖卡/淘汰保护共用同一快照。
- 买卡评分：候选卡命中任一聚焦目标（属于聚焦好友组 或 职业 == 聚焦职业）→ `score *= 1 + FocusBias(1.2) × GetFocusGain`。
  - `GetFocusGain` 复用 `GetFriendGain` / `GetJobGain`；好友侧只算一次（同卡可能属多个聚焦组），避免重复累加。
  - **不要求命中 `LikeFriend` / `LikeJob`**：聚焦是 AI 从已拥有的 Q4 核心卡"发现"出来的，只对有核心卡的玩家生效。
- 保护：`FindWeakCard` 与淘汰清仓都跳过聚焦目标成员。
- 国家不聚焦，仅作辅助评分（`LikeForce + GetForceBias`）；职业既有 `LikeJob` 路径、也有聚焦路径。

---

## 6. 卖卡 / 换卡

### 6.1 卖旧买新（在 `AiCheckBuyCard` 内）

触发条件全部满足才卖：

```
新卡是英雄 && 英雄卡数 >= 上阵格 + Cardherolimit && 不是同卡 && weakHeroCard != null
&& (新卡品质 != 4 || 已达硬上限 PlayerMaxHeroCards)
```

卖的对象是 `FindWeakCard(playerInfo)` 选出的"阵容价值最低"的卡。**已无次数上限**——防止零成本换卡改由"当回合买入的新卡当回合不卖"承担。

### 6.2 淘汰清仓（在 `PlayerAI.ShopBegin` 内，见 3.5）

到达淘汰回合后，把"非主力上阵"且品质 ≤ 淘汰线的英雄卡**逐回合渐进卖出**（不看喜好、不看价格，按品质升序，每回合最多 1 张）。品质是卡自身固定属性，与张数无关。不卖：主力上阵、主公、初始卡、当回合买入的新卡、聚焦目标成员、正在成组的好友组成员。

### 6.3 `FindWeakCard` 的卖卡意向

```
阵容价值 = 价格 × (1 + 阵营贡献×ForceBias + 职业贡献×JobChainBias + 好友贡献×FriendBias)
阵营贡献 = 参与同阵营护盾 且 同阵营≥2人 ? min(同阵营-1, 5)/5 : 0
职业贡献 = 同职业≥2人 ? min(同职业-1, 5)/5 : 0
好友贡献 = min(连线好友数, 5)/5

总价倍率 = 1 + 2 × (本卡堆总价 ÷ 本家最高卡堆总价)          ← 总价=单价×张数，范围 1~3 倍
最终价值 = 阵容价值 × 总价倍率 × (1 + GetSellCostBias)
```

- **卡堆总价越高 → 倍率越高 → 价值越高 → 越不容易被卖**（升序取最小者售卖）。同一玩家内以总价最高的卡堆为基数归一，"低的分级"体现在 1~3 倍的线性带宽上。
- 跳过卖出的卡：当回合买入的新卡、3 级及以上高星卡、`卡数≥2 且 ≥2级`的升星中卡、初始卡、主公、聚焦目标成员。
- 卖卡侧 `GetSellCostBias` 仍使用 `AccumulatedCostBias`（相对目标价的低价卡优先卖）。
- 卖卡退款：`totalValue <= 5 ? totalValue : totalValue - (totalValue-1)/5`。
- **口径差异（已知）**：`FindWeakCard` / `GetLineupValue` 对**已上阵的喜好卡仍全额计 Like 加成**（不看 `isNewHero`），所以这类卡难被卖；但买卡评分里对已上阵卡又不给 Like 加成。买入难、卖出更难，是当前唯一不自洽处。

---

## 7. 关键常量（`CombatConst.cs` / `CardShopManager.cs`）

| 常量 | 值 | 含义 |
|---|---|---|
| `ForceBias` | 0.4 | LikeForce 信任倍率 |
| `JobChainBias` / `JobChainBiasTop` | 0.8 / 1.3 | LikeJob 信任倍率 / 上阵最多那一路 |
| `FriendBias` / `FriendBiasTop` | 1.1 / 1.8 | LikeFriend 信任倍率 / 上阵最多那一组 |
| `FriendGainOneShort` | 0.7 | 好友"差一人"档收益（拉平到接近成团档 0.8） |
| `FocusBias` | 1.2 | 聚焦目标（好友组/职业）信任倍率，不要求命中 Like |
| `FocusCoreQuality` | 4 | 触发聚焦的核心卡品质门槛 |
| `FocusMaxTargets` | 2 | 聚焦目标名额上限（好友组与职业共用） |
| `QualityBiasMin` / `QualityBiasMax` | 0.5 / 2.0 | 品质偏好系数区间 |
| `QualityBias4Floor` | 1.0 | 品质4 品质偏好的下限（低等级 Q4 概率最低时抬到 1.0，避免史诗卡被过度压制） |
| `CardPriceBiasBase` / `CardPriceBiasExponent` | 6 / 2 | 价格偏好：`score ×= (价格/6)^2`（阵容价值=价格×星级，价格是更可控的杠杆） |
| `StarUpBias` / `StarUpBenchFactor` | 0.8 / 0.4 | 升星导向信任倍率 / 预备席权重（上阵主力=1.0，预备席=本值） |
| `FactionShieldCounts` | {2,3,4,5,6} | 同阵营护盾档位 |
| `FriendLineCounts` | {2,3,4,5,6} | 好友连线档位 |
| `AiOverLimitRejectPerCard` | 35 | 超限新卡每多囤 1 张的拒买概率(%) |
| `SellValueMultiplierMin` / `SellValueMultiplierMax` | 1 / 3 | 卖卡意向倍率区间（按卡堆总价占比归一） |
| `EliminateRoundMaxOffset` | 5 | 淘汰回合最大偏移量（实际 = 基准 ± round((1-smart)×此值)） |
| `EliminateMaxSellPerRound` | 1 | 淘汰清仓每回合最多卖出张数（逐回合渐进，避免断档） |
| `AiMaxShopRefreshPerShop` | 2 | 独立模式下 AI 每商店基础刷新次数（叠加 `RichExtraRefreshPerShop`） |
| `AiSharedMaxRefreshPerShop` | 1 | 共享模式下 AI 每个商店阶段基础刷新次数（叠加 `RichExtraRefreshPerShop`） |
| `AiRefreshMinRemainGold` | 6 | AI 刷新后须保留的最低金币（`gold - RefreshGoldCost >= 此值` 才允许刷） |
| `RichGoldThreshold` | 15 | "金币充裕"阈值（`gold >= 此值` 时放开刷新门槛1、用高经验预算） |
| `RichExtraRefreshPerShop` | 2 | "钱多"时每个商店阶段额外允许的刷新次数（叠加上两个上限） |
| `NormalExpBuyGoldRate` / `RichExpBuyGoldRate` | 0.2 / 0.35 | 买经验预算占金币比例（默认 / 金币充裕） |
| `PlayerMaxHeroCards` | 15 | 背包英雄卡硬上限（15 种，重复卡不占位） |
| `BattleWinExp` / `BattleLoseExp` | 3 / 2 | 战斗经验（期望 2.5/回合） |
| `ExpBuyGoldCost` / `ExpBuyAmount` | 4 / 4 | 买经验单价 / 每次获得经验 |
| `PlayerMaxLevel` | 10 | 玩家最高等级 |
| `LikeCardRefreshRate` | 5 | 刷牌时从收藏池出卡的概率(%) |
| `CardShopManager.RefreshGoldCost` | 2 | 刷新单价 |
| `CardShopManager.SOLD_REMAIN_ROUNDS` | 5 | 已售出卡重新出现的回合数 |

---

## 8. 玩家性格参数（`PlayerConfig_s.cs`）

buy/sell 评分与淘汰清仓用到的列（`ExpCheckOff` 为 float，其余含义见第 3.5/5 节；末两列为淘汰品质的基准回合，实际回合还要按 smart 偏移）：

| Id | Name | SameCardRate | Cardherolimit | Futurerate | LikeJob | LikeForce | LikeFriend | OwnTooMuchCardRate | BalanceFactor | Intelligence | AccumulatedCostBias | ExpCheckOff | 淘汰品质1 | 淘汰品质2 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 旺仔 | 0 | 0 | 0 | — | — | — | 0 | 0 | 0 | 0 | 0 | 0(不启用) | 0(不启用) |
| 2 | 布布 | 5 | 2 | 0.6 | 101 | 1,2 | 7,12,8,28 | 0.7 | 0.4 | 75 | 0.95 | 0.50 | 14 | 28 |
| 3 | 翔阳 | 3 | 2 | 0.5 | 101,102 | 2,3 | 9,13,29,22,26 | 0.7 | 0.5 | 60 | 1.00 | 0.60 | 10 | 23 |
| 4 | 屁屁 | 3 | 1 | 0.7 | 201,202,403,301 | — | 10,23,19,27 | 0.7 | 0.5 | 60 | 0.90 | 0.65 | 10 | 22 |
| 5 | 八戒 | 3 | 2 | 0.28 | — | 10,4 | 14,26 | 0.7 | 0.6 | 50 | 1.10 | 0.35 | 11 | 24 |
| 6 | 艾沙 | 3 | 1 | 0.2 | 401,503 | 2 | 5,15,21,32,4 | 0.7 | 0.8 | 55 | 0.85 | 0.50 | 11 | 24 |
| 7 | 大雄 | 3 | 1 | 0.3 | 601,602,603 | 1,2 | 1,13,14 | 0.5 | 0.3 | 15 | 0.60 | 0.20 | 14 | 28 |
| 8 | 巴爸 | 3 | 2 | 0.28 | 301,302 | 1 | 11,25,28,18,7 | 0.85 | 0.6 | 65 | 1.05 | 0.50 | 9 | 21 |
| 9 | 巴妈 | 3 | 2 | 0.35 | 501 | 3 | 15,18,21,32,5,19 | 0.5 | 0.5 | 45 | 0.88 | 0.45 | 12 | 25 |
| 10 | 朱迪 | 3 | 1 | 0.3 | 203 | 2,6 | 2,9,22 | 0.5 | 0.4 | 30 | 0.70 | 0.30 | 13 | 27 |
| 11 | 光头 | 3 | 1 | 0.3 | 602,603 | 3,4 | 3,12,29 | 0.5 | 0.3 | 18 | 0.65 | 0.20 | 14 | 28 |
| 12 | 可霏 | 3 | 1 | 0.4 | 502,701 | 6,3 | 4,7,15,32,21 | 0.45 | 0.5 | 40 | 0.75 | 0.40 | 12 | 26 |
| 100 | 魔童 | 3 | 2 | 0.5 | 102 | 4,10 | 13,14,31,9 | 0.9 | 0.5 | 50 | 0.75 | 0.60 | 11 | 24 |
| 101 | 钱多 | 5 | 3 | 0.525 | 701,402 | — | 17,24,12,20,7 | 0.7 | 0.4 | 80 | 0.80 | 0.90 | 8 | 20 |
| 102 | 黄眉 | 3 | 2 | 0.5 | 401,402,403 | 3,5 | 20,16,31,6 | 0.85 | 0.6 | 90 | 1.20 | 0.85 | 8 | 20 |
| 103 | 无量 | 3 | 2 | 0.5 | 403,401 | 3,2 | 26,9,29,5,13 | 0.9 | 0.6 | 85 | 1.15 | 0.90 | 8 | 20 |
| 104 | 大虎 | 3 | 2 | 0.5 | 402,403,401,503 | — | 10,27,11,25,23 | 0.9 | 0.5 | 90 | 0.70 | 0.75 | 8 | 20 |
| 999 | 怪物 | 0 | 0 | 0 | — | — | — | 0 | 0 | 0 | 0 | 0 | 0(不启用) | 0(不启用) |

> 基准值填法：聪明/激进的玩家偏早（钱多/黄眉/无量/大虎 8/20），稳健或爱攒同卡的偏晚（布布 14/28，其 `SameCardRate=5` 要攒同卡升星）；旺仔与怪物为 0 表示不启用。

> 选牌阶段的玩家分配走 `PlayerBook.GetRandomN`：pid0 = 旺仔，pid1~7 从 id∈[2,900] 中随机抽（4 个 `CanPlay=true` + 3 个 `CanPlay=false`），排除 999 怪物。

---

## 9. 调参指引 / 已知影响

| 想调整 | 改哪里 |
|---|---|
| 品质偏好的强弱 | `CombatConst.QualityBiasMin/Max`（当前 0.5 / 2.0） |
| 品质4 是否设下限 | `CombatConst.QualityBias4Floor`（当前 1.0，设 0 即关闭下限） |
| 价格偏好强度 | `CombatConst.CardPriceBiasBase / CardPriceBiasExponent`（当前 6 / 2，指数设 0 即关闭） |
| 升星导向强度/开关 | `CombatConst.StarUpBias`（当前 0.8，设 0 即关闭） / `StarUpBenchFactor`（预备席权重，当前 0.4） |
| 品质偏好的概率源 | `PlayerAI.GetQualityBias` 内分支 |
| Like 三路的权重 | `CombatConst.ForceBias / JobChainBias / FriendBias` 及 `*Top` |
| "上阵最多那一路"的判定 | `PlayerAI.IsTopLikeJob / IsTopLikeFriend` |
| 好友"差一人"档收益 | `CombatConst.FriendGainOneShort`（当前 0.7） |
| 聚焦机制开关/强度 | `CombatConst.FocusCoreQuality`(核心卡品质门槛) / `FocusMaxTargets`(名额) / `FocusBias`(倍率)；派生见 `PlayerAI.DeriveFocusTargets` |
| 淘汰渐进节奏 | `CombatConst.EliminateMaxSellPerRound`（当前每回合 1 张） |
| AI 刷新次数 / 门槛 | `CombatConst.AiMaxShopRefreshPerShop`(独立2) / `AiSharedMaxRefreshPerShop`(共享1) / `AiRefreshMinRemainGold`(6)；判定 `PlayerAI.ShouldAiRefresh`；上限由 `PlayerAI.GetShopRefreshLimit` 叠 `RichExtraRefreshPerShop`(钱多+2) |
| "钱多"判定阈值 / 加成 | `CombatConst.RichGoldThreshold`(15) / `RichExtraRefreshPerShop`(2) / `RichExpBuyGoldRate`(0.35) |
| 淘汰回合早晚 | `PlayerConfig.EliminateQuality1 / EliminateQuality2`（基准回合，0=不启用） |
| 淘汰回合的 smart 偏移幅度 | `CombatConst.EliminateRoundMaxOffset`（当前 5）；偏移方向见 `PlayerAI.GetEliminateRound` |
| 卖卡意向的总价曲线 | `CombatConst.SellValueMultiplierMin / SellValueMultiplierMax`（当前 1~3 倍） |
| 超限囤卡拒买概率 | `CombatConst.AiOverLimitRejectPerCard` |
| AI 买经验积极度 | `PlayerConfig.ExpCheckOff`（0.2~1）、`PlayerAI.ShopBegin` 内的 70% 封顶与预算比例（默认 `NormalExpBuyGoldRate=0.2`，金币充裕 `RichExpBuyGoldRate=0.35`） |
| 候选池宽窄 / 抖动 | `PlayerConfig.Intelligence`（越高池越窄、抖动越小） |

已知影响（改动后需留意）：

1. **PVE 共享回合品质系数无区分**：`GameRoundConfig` 的 Q2/Q3/Q4 全为 0，所有卡都拿 2.0。
2. **品质4 强卡由下限抬到 1.0**：低等级时 Q4 概率最低，归一后本会压到 0.5，现由 `QualityBias4Floor(=1.0)` 抬到 1.0，不再与"强卡超限豁免"对冲，实际会增加 Q4 购买。
3. **移除预备席 `×0.7` 惩罚后**，`SameCardRate` 高的玩家（布布、钱多 = 5）更倾向反复囤预备席同卡。
4. **买入/卖出口径不一致**：已上阵喜好卡买入时不给 Like 加成，但卖出时受 Like 加成保护（难被卖）。
5. **保底买经验**保证"该买就能买"，但实际购买次数仍受金币限制，会略低于理论值 `系数 × 回合 / 4`。
6. **淘汰口径为品质（不是卡等级）**：不再买的是"品质 ≤ 淘汰线"的卡；卖出改为**逐回合渐进**（按品质升序、每回合最多 `EliminateMaxSellPerRound` 张），不再整堆一次清空。品质是卡自身固定属性、与张数无关；品质3/4 卡（含**未拥有**的新英雄）天然高于淘汰线，始终不受淘汰影响，后期仍可买入补阵。聚焦目标成员与正在成组的好友组成员会被保护，不被淘汰。
7. **聚焦依赖 Q4 核心卡**：只有买到过品质4 卡的玩家才会派生聚焦目标；无 Q4 卡的玩家不受影响（走原 Like 评分）。聚焦与 Like 可叠乘，聚焦不要求命中 Like。
8. **淘汰后 AI 卡种收窄但仍可进强卡**：品质1/2 的卡在各自淘汰回合后不再囤，AI 持有的卡种类会少于人类；但已不再是"只剩 4~7 种"——品质3/4 新卡始终可买。
9. **`AiMaxSellPerShop` 已删除**：同回合换卡次数不再受限，防线改为"当回合买入的新卡当回合不卖"（`PlayerInfo.boughtThisShopCards`）。
10. **买卡失败不再整回合跳过**：`AiCheckBuyCard` 按分数加权随机在一批候选里轮询——某张买入失败（如背包满卖不出位）只"跳过该张、继续试下一张"，全部试完才返回 `false`。避免一次失败就收工导致金币花不出去；满包仍是最主要瓶颈（可卖弱卡少时无位可腾）。
11. **聚焦套卡豁免淘汰**：`FilterOffers` 的淘汰过滤对 `IsFocusMember`（聚焦目标成员）放行，聚焦套卡里的低品质卡即使过了淘汰回合也仍可继续买；`FindWeakCard` / `EliminateOutdatedCards` 同样不卖聚焦成员。
12. **"钱多"通用规则**：`gold >= RichGoldThreshold(=15)` 时（a）放开刷新门槛1（有可用卡也可刷）+ 每商店额外 2 次刷新上限；（b）买经验预算比例从 0.2 提到 0.35。对全部 AI 生效（非某一角色专属）。
13. **价格偏好 / 升星导向 / 品质4下限（本组改动）**：30 种子（50 回合、8 AI、指标 = 全体上阵价值合计）A/B 实测，相对基线 `1013.0 ± 29.2`，本组为 `1021.4 ± 28.3`（**+0.8%，t≈1.1，不显著**）。原因：买卡评分只决定"买哪张"、不决定"买不买"，AI 本就每轮尽量买（约 4.8 张/人/回合），故评分重排对阵容价值影响微弱。保留本组为设计取向（更偏高价 / Q4 / 升星），不追求量化收益。
14. **买卡纪律（已试并回退）**：曾加"库越满、最高候选分越不达标就越不买"的门槛（让 AI 攒钱 / 多刷找强卡）。30 种子实测 `960.3 ± 27.5`（**-5.2%，显著**）——AI 的高频买入是"载重"：少买 → 少重复卡升星、少羁绊成员 → 阵容价值反跌；且多出的刷新（798→864 次）白耗金币。已回退（常量 `AiBuyScoreBarMin/Max` 与评分链门槛一并移除）。**结论：提升阵容价值不能"少买"，方向应在获取量 / 升格（上阵格随等级涨）侧。**
