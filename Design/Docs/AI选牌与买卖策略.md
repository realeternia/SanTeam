# AI 选牌与买卖策略（PlayerAI）

> 整理日期：2026-10-27（按当前 [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) 与 [CombatConst.cs](../../Assets/Resources/Scripts/Combat/CombatConst.cs) 重写）
> 范围：点赞（like）、商店开场、买卡评分、卖卡/淘汰、刷新决策、聚焦机制
> 相关文档：[职业与兵种连锁配置](./职业与兵种连锁配置.md)、[好友羁绊与武将分组](./好友羁绊与武将分组.md)、[done 商店双模式抽象方案](./done%20商店双模式抽象方案.md)

---

## 一、调用链路

| 时机 | 入口 | 作用 |
| --- | --- | --- |
| 选牌界面（like 阶段） | `PlayerAI.CheckLike` | 每个 AI 点赞 `likeCount` 张（默认 2 张），只从 5~10 金、非主公、未被点过的卡里随机挑 |
| 商店阶段开始（AI） | `PlayerAI.ShopBegin` | ① 淘汰清仓 ② 经验检查买经验 |
| AI 回合买卡 | `PlayerAI.AiCheckBuyCard` | 对 `ShopOffer` 列表评分，加权随机选一张，失败换下一张 |
| 需要腾位时 | `PlayerAI.FindWeakCard` | 按"阵容价值"找最该卖的卡 |
| 买不动时 | `PlayerAI.ShouldAiRefresh` | 判断是否值得花 2 金刷新 |

两种商店模式（共享轮换 / 独立买卡）共用同一套 `ShopOffer` 报价与评分代码，见 [done 商店双模式抽象方案](./done%20商店双模式抽象方案.md)。

---

## 二、商店开场：淘汰 + 买经验

### 2.1 淘汰清仓 `EliminateOutdatedCards`

- **淘汰线**：`PlayerConfig.EliminateQuality1/2` 为基准回合，实际回合 = `基准 ± round((1-smart)×EliminateRoundMaxOffset)`，`smart=Intelligence/99`，偏移方向按 `pid` 奇偶固定（一局内稳定）。到达后：品质 ≤ 淘汰线的卡不再买入，手上的逐步卖出。
- **每回合最多卖 `EliminateMaxSellPerRound(=1)` 张**，按品质升序（先淘汰最低品质），避免单回合清空导致阵容断档。
- **不卖**：当前主力上阵、主公、初始卡、当回合买入、品质高于淘汰线、聚焦目标成员、正在成组的好友组成员（该组已拥有 ≥2 张）。

### 2.2 买经验

- 期望总经验 = `round × ((BattleWinExp + BattleLoseExp)/2 + ExpCheckOff)`（基准胜 3 / 负 2，即 2.5/回合）。
- 触发概率 = `min(0.7, 缺口 / 期望)`，缺口越大越可能买。
- 预算 = 金币 × 20%（金币充裕时 35%，`RichGoldThreshold=15`），`ExpBuyGoldCost=4` 金/次；金币够时至少买 1 次。

---

## 三、买卡评分

### 3.1 候选过滤 `FilterOffers`

保留：买得起 + 英雄卡 + 非"已满级(Lv5)同卡" + 通过淘汰过滤。
淘汰期过滤例外：**聚焦目标成员豁免**（低品质聚焦卡仍可继续买，避免"攒一半被淘汰线掐断"）。

### 3.2 评分链（按代码顺序相乘）

| 步骤 | 条件 | 倍率 |
| --- | --- | --- |
| 同卡 | 已拥有该卡 | `× SameCardRate`（配置值） |
| 软上限 | 非强卡新卡、卡数 ≥ 上阵格+`Cardherolimit` | 每超 1 张 35% 概率直接拒买；未拒则须卖弱卡换入，且新卡价格不能低于弱卡（`year>8` 时还要高出 `year-8` 金） |
| 品质偏好 | 所有英雄卡 | 按当前商店实际出卡概率线性插值：最高概率品质 `2.0`、最低 `0.5`；Q4 下限 `1.0` |
| 价格偏好 | 所有英雄卡 | `(价格 / 6)^2`（`CardPriceBiasBase=6`、`CardPriceBiasExponent=2`） |
| 许愿 | 该卡在本玩家点赞收藏池 | `× 2` |
| 升星导向 | 已有同卡且未满级 | `× (1 + 0.8 × 升星接近度)`；接近度按距下一级还差 1/2/3/更远 = 1.0/0.6/0.35/0.15，预备席再 ×0.4 |
| 阵营（force） | 命中 `LikeForce` | `× (1 + 0.4 × forceGain)`；Q4 强卡 gain=1，否则按同阵营护盾档位（2/3/4/5/6 人）推进程度计，主公同阵营已有英雄 +0.6 |
| 职业（job） | 命中 `LikeJob` | `× (1 + bias × jobGain)`；bias 平常 0.8，该职业是 Like 中上阵人数最多的一路时 1.3；`jobGain = 0.3 × (当前人数+1)`，满 5 人为 0 |
| 好友（friend） | 命中 `LikeFriend` | `× (1 + bias × friendGain)`；bias 平常 1.1、最成组时 1.8；已成团 `0.8 + 0.25×(档位-1)`，只差 1 人 `0.7`；特殊连锁组 `0.5 + 0.2×min(在场其他成员,3)`，金币组 `0.25`，普通组 `0.3` |
| 聚焦 | 命中派生聚焦目标 | `× (1 + 1.2 × gain)`，gain 复用好友/职业收益，**不要求命中 Like** |
| 远近搭配 | 上阵 ≥3 人 | `× (1 + BalanceFactor × gain × smart)`，gain 为近战/远程差额归一（-1~1） |
| 热门惩罚 | 新卡（非同卡） | `× OwnTooMuchCardRate^(拥有该卡的玩家数)` |
| 聪明度抖动 | 全部 | `× (1 + rand(-1,1) × (1-smart) × 0.5)` |

补充规则：

- 候选 ≥5 且最高分 <1.6 时，按价格取前 3 名分别 ×1.6 / ×1.4 / ×1.2（避免"全场都低分"时选到廉价废卡）。
- 候选池按聪明度收窄：`smart≥0.7` 取前 3、`≥0.4` 取前 4、否则前 6；再按分数加权随机抽一张。
- 抽中的卡买不进（满包无弱卡可卖等）时，从池里移除后继续尝试下一张，全部失败才跳过该玩家。
- 无任何可用卡时，只有金币低于开局金币的 `20% + Futurerate×futureBias×15%` 才允许跳过，否则买一张兜底。

---

## 四、聚焦机制（好友组 / 职业）

- **派生式无状态**：每回合从"当前拥有的 Q4 核心卡"重新派生，不写存档、不显式重置；卖掉核心卡即自动释放。
- 候选 = 核心卡所属的**好友组**与**职业**（各要求该组/职业已拥有 Q4 ≥1）。
- 排序：`Q4数 desc → 已拥有成员数 desc → 档位进度 desc → 好友优先于职业`，取前 `FocusMaxTargets(=2)` 个（名额共用，通常是 2 好友或 1 好友+1 职业）。
- 命中聚焦的卡在评分中追加 `FocusBias=1.2` 收益；`FindWeakCard` 与淘汰清仓都跳过聚焦成员。

---

## 五、卖卡 `FindWeakCard`

- **保护**：当回合买入、卡级 ≥3、已有 ≥2 张且卡级 ≥2、初始卡、主公、聚焦目标成员。
- **阵容价值** = `价格 × (1 + 阵营贡献×0.4 + 职业贡献×0.8 + 好友贡献×1.1)`（仅命中对应 Like 时计入）。
- **卡堆倍率**：按"该卡总价 ÷ 本家最高卡堆总价"线性映射到 `1~3` 倍（总价越高越不舍得卖）。
- **卖卡倾向 `GetSellCostBias`**：目标费 `target = round(AccumulatedCostBias × sqrt(year))`
  - 价格 ≤ target：`-0.6 - (target-价格)×0.25`
  - 价格 = target+1：`-0.25`
  - 更高：`+0.2`
- **远近惩罚**：堆得多的那一类（近战/远程）价值 ×`(1 - min(差额,3)×0.1)`。
- 按价值升序取最低的一张卖出。

---

## 六、刷新决策

- **值得刷新**：当前 `FilterOffers` 为空（一张能买/该买的卡都没有）时刷新；若已有可用卡，只有"金币充裕"（≥15）才刷新。
- **买得起**：支付 2 金刷新费后剩余 ≥ `AiRefreshMinRemainGold(=6)`。
- **次数上限**：独立买卡 `AiMaxShopRefreshPerShop(=2)`，共享轮换 `AiSharedMaxRefreshPerShop(=1)`；金币充裕时各 +`RichExtraRefreshPerShop(=2)`（统一走 `GetShopRefreshLimit`）。

---

## 七、PlayerConfig 关键参数

| 字段 | 作用 |
| --- | --- |
| `Intelligence` | 聪明度 1~99；影响淘汰回合偏移、评价抖动、候选池宽度、远近因子生效程度 |
| `SameCardRate` | 同卡（升星）倍率 |
| `Cardherolimit` | 英雄卡上限额外数（上阵格 + 本值） |
| `Futurerate` | 看未来强度（影响跳过阈值） |
| `LikeJob` / `LikeForce` / `LikeFriend` | 偏好职业 Id / 阵营 side / 好友组 Id |
| `OwnTooMuchCardRate` | 热门卡惩罚底数 |
| `BalanceFactor` | 近战/远程搭配权重 |
| `AccumulatedCostBias` | 目标费增长系数（`sqrt(year)` 映射） |
| `ExpCheckOff` | 经验期望偏移（期望 = round×(2.5+本值)） |
| `EliminateQuality1/2` | 淘汰品质 1 / 品质 2 卡的基准回合（0=不淘汰） |
| `InitCards` | 初始卡（不卖） |

---

## 八、调试与日志

- `AiCheckBuyCard` 会输出每张候选卡的 `分数 / 价格 / 品质 / 品质系数` 以及最终选中卡；买卡失败会记录"买卡失败跳过"。
- `FindWeakCard` 输出卖卡候选的 `等级 / 数量 / 价格 / 价值 / sellBias`。
- 淘汰清仓逐条记录"卖出 XX 品质N 淘汰线M 回合K"。
- 均为 `GameLog.Debug`，排查 AI 行为时直接搜这些关键字。

---

## 九、代码索引

| 内容 | 位置 |
| --- | --- |
| 点赞 | [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) `CheckLike` |
| 商店开场/淘汰/买经验 | [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) `ShopBegin` / `EliminateOutdatedCards` / `GetEliminateMaxQuality` |
| 候选过滤 | [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) `FilterOffers` |
| 买卡评分与购买 | [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) `AiCheckBuyCard` |
| 聚焦 | [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) `DeriveFocusTargets` / `GetFocusGain` / `IsFocusMember` |
| 卖卡 | [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) `FindWeakCard` / `GetLineupValue` / `GetSellCostBias` |
| 刷新判定 | [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) `ShouldAiRefresh` / `GetShopRefreshLimit` |
| 全部 AI 常量 | [CombatConst.cs](../../Assets/Resources/Scripts/Combat/CombatConst.cs) `AI / 淘汰 / 聚焦 / 羁绊信任` 段 |
| 玩家 AI 参数 | [PlayerConfig_s.cs](../../Assets/Resources/Scripts/Configs/PlayerConfig_s.cs) |
