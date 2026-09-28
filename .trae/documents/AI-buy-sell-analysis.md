# AI 买/卖卡逻辑分析（PlayerAI）

本文档分析了仓库中 PlayerAI 的买卡/卖卡决策逻辑（文件：Assets/Resources/Scripts/PlayerAI.cs）以及 PlayerConfig 中用于调节的相关参数（Assets/Resources/Scripts/Configs/PlayerConfig_s.cs）。最后列出当前实现中可能不合理或可改进的点，并给出建议。

---

## 1 概览

核心入口：
- 购买决策：`PlayerAI.AiCheckBuyCard(PlayerInfo playerInfo, int era)`
- 卖卡决策（找最弱卡）：`PlayerAI.FindWeakCard(PlayerInfo playerInfo)`

整体思路：对商店中所有可购买且负担得起的卡片计算一个评分（score），按权重随机选一张买。评分由若干子项相乘/累加产生（同卡加权、强度分、羁绊收益、远近平衡、已有人数惩罚、随机抖动等）。为了实现“随回合更偏好高费卡”的行为，新增了配置字段 `PlayerConfig.AccumulatedCostBias`，并在评分里通过 `GetBuyCostBias` / `GetSellCostBias` 对分数/卖出优先级进行调整，目标费通过映射计算为：

  target ≈ round(AccumulatedCostBias * sqrt(year))

---

## 2 购买决策（步骤拆解）

1. 早期检查：若 playerInfo.nextSkip 为 true 或没有可买卡或买不起任何卡则直接返回 false。 
2. 计算 weakHeroCard（当卡位已满且存在可卖卡时，用来判断是否允许以卖旧换新）
3. 获取 strongList（当前自动上阵的最强几张卡）并构建 `AiContext`（记录阵营/职业/远近统计）
4. 计算 `futureBias`（下一回合高品质卡概率变化）与 `smart`（聪明度归一化）
5. 遍历 `affordableCards`，对每张卡计算 `score`：
   - 基础 score = 1
   - 如果已有相同卡：score *= sameCardRate，并基于已有张数/回合再调整
   - 对英雄卡：
     - 若阵容位满且没有可换卡则跳过
     - 根据 `PowerFactor` × GetPowerMetric（品质/面板/升星）、`SideFactor`×GetSideGain、`JobFactor`×GetJobGain、`FriendFactor`×GetFriendGain、`BalanceFactor`×GetBalanceGain 依次乘权
   - 对道具卡：按 itemCfg.Effect 给分
   - 若没人拥有这张卡：根据 `OwnTooMuchCardRate` 对热门卡进行惩罚（人数越多得分越低）
   - 加入“聪明度抖动”（随机乘子）
   - 新增：`score *= 1f + GetBuyCostBias(price, year, cfg)`，按价钱和回合给额外加成/惩罚
6. 若 `scoredCards` 为空（全部被 filter 掉），会在满足最低 gold 的情况下把所有可负担卡加入候选并用 default score
7. 若评分都较低且候选 >=5，会把按价格*count 排名前 3 的卡额外提升系数（鼓励买贵卡作为优先）
8. 依据聪明度缩小候选池（3/4/6）并按权重随机抽取
9. 若选择的卡需要“卖旧买新”，会在买之前调用 `playerInfo.SellCard(weakHeroCard.Item1)`（并受 `CombatConst.AiMaxSellPerShop` 限制）
10. 确定购买数量 `finalBuyCount`：基于 `playerInfo.gold * 2f / 3f / price * saveMood` 计算并 clamp 到 [1, count]

---

## 3 卖卡决策（FindWeakCard）

FindWeakCard 的流程：
- 遍历玩家现有卡（仅英雄卡）：过滤掉非英雄、已升至 4 级以上、初始卡、主公卡
- 对每张候选卡计算 `GetLineupValue(ctx, heroCfg)`：价格 × (1 + 阵营/职业/好友贡献)
- 新增调整：`value *= 1f + GetSellCostBias(price, year, cfg)`，对低于“目标费”的卡大幅降低 value
- 对价值升序排序，返回最小的那张作为卖卡目标

注意：卖卡次数受 `playerInfo.aiShopSellCount < CombatConst.AiMaxSellPerShop` 约束。

---

## 4 关键参数与子函数说明

- PlayerConfig 中的主要参数会影响评分：sameCardRate, Cardherolimit, Futurerate, Findmasterrate, FriendFactor, OwnTooMuchCardRate, SideFactor, JobFactor, PowerFactor, BalanceFactor, Intelligence, AccumulatedCostBias
- GetPowerMetric(heroCfg, ownCount)：合成品质/面板/升星为强度分
- GetSideGain / GetJobGain / GetFriendGain / GetBalanceGain：基于当前阵容统计返回不同类型羁绊收益
- GetFutureBias(year)：利用 `GameRoundConfig` 比较当前/下一回合高品质卡概率差
- GetBuyCostBias / GetSellCostBias：新增函数，当前实现采用 sqrt(year) 映射，并返回几个离散区间的增益/惩罚数值

---

## 5 当前实现可能存在的问题（不合理或值得优化的地方）

下面按优先级列出观测到的潜在问题与改进建议：

1) 买卡偏好权重可能过强/非线性突兀
- 实现细节：`score *= 1f + GetBuyCostBias(...)`。当 `GetBuyCostBias` 返回 1.0 (exact target) 时，score 会翻倍，这在某些情况下可能压倒其他合理评分项（比如羁绊或强度分）。
- 建议：改为 additive 或对 bias 做限制（如 cap 到 [ -0.5, +0.6 ]），或用平滑函数（如高斯衰减或 sigmoid 基于 |price - target|）来给予更连续、可调的加权。

2) target 计算与参数表达的歧义
- 目前使用 target = round(AccumulatedCostBias * sqrt(year))。这能达到你要求的 10→3, 30→5, 60→8 的大致效果，但：
  - 当 year 很大或 small bias 时，四舍五入可能导致跳跃（比如从 3 直接跳到 5），不够平滑
  - AccumulatedCostBias 含义“越大越快”合理，但命名与配置解释可以更清晰（比如直接存储 `GrowthRate`，并在注释中写示例）
- 建议：用带上限/下限且连续的映射（e.g. target = clamp(round(bias * year^alpha), min, max) 或 使用线性+logistic 混合）并记录/打印 target 以便调参。

3) 卖卡惩罚力度偏重且未考虑多维因素
- `GetSellCostBias` 对低于 target 的卡直接返回较大负值，使其价值被迅速拉低进而优先卖出。若 target 偏高（例如 AI 希望 6 费），所有 1–3 费卡都会被大幅惩罚，可能导致：
  - 过早卖掉本能有价值的低费羁绊关键卡或重复卡（尤其是当低费卡是队伍关键或将被升到更高星时）
- 建议：把 sell bias 设计为“倾向但非硬性”，考虑卡的升级潜力（卡片剩余经验/已有 copy），以及是否参与关键羁绊/主公/初始卡等，降低误删风险。

4) finalBuyCount 的买入数量策略可能导致极端囤货
- 目前：finalBuyCount = clamp(round(playerInfo.gold * 2/3 / price * saveMood), 1, selectedCard.count)
- 这会在高金量下买很多同卡（尤其是 price 低时），可能让 AI 在一轮花光大部分金币，影响后续决策。
- 建议：引入最大单次购买上限（基于卡池稀有度或常量），或者以“留下一定基础运营金”为目标（例如保留 10~20 金作为常备金）。可以改成基于预期收益（EV）估算要买的份数。

5) top3 价格奖励可能鼓励买贵卡但忽略性价比
- 代码中当所有分数较低时，会把按 price*count 排名前 3 的卡额外乘系数（1.6/1.4/1.2），这直接鼓励买更贵卡。
- 问题：贵卡不一定适合当前羁绊/阵容，强行提升会产生不合理选择。
- 建议：把 price 作为因素之一而不是简单奖励；应结合羁绊/强度/当前需求再决定是否加权。

6) “聪明度抖动”与随机性
- 低聪明度增加抖动会使 AI 更随机，这是合理的。但过大的随机振幅可能掩盖策略性选择，导致 AI 在关键时点做出明显糟糕选择。
- 建议：限定随机幅度或把随机化从乘法改为更轻微的加性噪声。

7) 没有把“商店中卡牌供给特性”纳入考量
- 当前仅依据单张卡评分，没有考虑同一商店中同类卡（如多个相同卡）的存在或整套卡池中可达的连锁机会。
- 建议：考虑商店整体视角，例如若商店内有两张同一英雄卡，AI 更应该优先购买以确保升星，或者若商店有多张互补的羁绊卡也应额外提升优先级。

8) 没有明确考虑“当前金量 vs 保存以待更好回合”的长期价值计算
- 虽然有 `Futurerate` 和 `futureBias` 的考虑，但它只对是否 skip 有影响和 finalBuyCount 的 saveMood，整体仍缺少对“用金币换现在战力 vs 存钱等待更高品质卡”的期望值比较。
- 建议：引入简单的 EV 估算（基于 GameRoundConfig 的未来品质概率）来决定是否买、买多少或选择 skip。

9) 代码可测性与调试信息不足
- 当前只在选卡时打印 `scoredCards` 等，但没有系统化地输出 target 值、GetBuyCostBias/GetSellCostBias 的中间结果，导致在调整 bias 时难以观察实际效果。
- 建议：增加可选的调试开关，打印每回合 target、bias 值、最终选卡与卖卡理由，便于数据驱动的调参。

---

## 6 建议的改进工作项（优先级排序）

1. 将 `GetBuyCostBias` / `GetSellCostBias` 改为连续平滑函数（高斯或 sigmoid），并对输出做上下 cap（例如 [-0.5, +0.6]）。
2. 为 `finalBuyCount` 添加最大购买限制和/或保留最低流动资金阈值；或改为基于简单 EV 的购买份数估算。
3. 在卖卡判断中加入“是否为关键羁绊/升级潜力/已有多张”的保护逻辑，避免过早卖掉未来可升星或阵容关键卡。
4. 添加更多调试输出（target、每张卡的 buyBias/sellBias、最终 chosen card 和 sell reason），并在 UI 或日志中做汇总以便 A/B 调参。
5. 考虑商店级别的组合评估（若同一卡两张/三张出现在本轮商店，应提升其优先级）。
6. 根据线上或模拟器跑的数据微调各 AI 的 `AccumulatedCostBias`（目前用 sqrt 映射是一种合理默认，但可以做更细粒度调整）。

---

## 7 参考（重要代码位置）

- 购买主逻辑：Assets/Resources/Scripts/PlayerAI.cs::AiCheckBuyCard
- 卖卡判断：Assets/Resources/Scripts/PlayerAI.cs::FindWeakCard
- 配置参数：Assets/Resources/Scripts/Configs/PlayerConfig_s.cs (新增 `AccumulatedCostBias` 字段和 Load 中的默认值)

---

如果你愿意，我可以把上面优先级最高的改动（1-3）做成一个小 PR：
- 把 buy/sell bias 改为高斯衰减并限制幅度
- 在卖卡里加入“保护即将升星/关键羁绊卡”的判断
- 为 finalBuyCount 设置上限并保留最低流动金

另外可以在模拟器（Tools/战斗模拟器）中做一批对战日志来量化不同 bias 值对 AI 行为与胜率的影响，帮助调参。