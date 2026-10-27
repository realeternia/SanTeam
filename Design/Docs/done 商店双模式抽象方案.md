# 商店双模式抽象（已完成）

> 整理日期：2026-10-27（按当前实现重写，去掉迁移前的行号快照）
> 状态：已实施。`CardShopManager` 作为宿主（UI/布局/卡池/回合驱动），两种模式只是策略实现
> 相关：[AI选牌与买卖策略](./AI选牌与买卖策略.md)

---

## 一、总体结构

```
CardShopManager (MonoBehaviour, 宿主/门面)
 ├─ ShopCfg / Era / TurnOrder / cardViews / cardPool / prefab / passBtn / mySelect
 ├─ CreateCardView / ReplaceCardAt / LayoutCards / ClearCards
 ├─ GetRandomShopHeroId(shopCfg, pid)   ← 按品质概率 + 玩家 like（收藏卡）
 ├─ RefreshGoldCost = 2
 ├─ ShopMode Mode  →  current is SharedShopMode / IndependentShopMode
 └─ 生命周期对 Mode 做纯转发：Begin / GetActingPlayer / GetOffers / OnBought /
    OnHumanBought / OnHumanPass / OnHumanRefresh / Drive
```

- `ShopModeType { Shared, Independent }`；`ShopMode` 基类提供 `Type`、`IsPanelBlocked()`、`ShouldBotStop()`。
- 宿主不知道也无需知道具体模式：人类点击、AI 驱动、回合推进都由 `Mode` 决定。
- 外部只通过 `Begin() / Drive() / GetOffers(pid) / GetActingPlayer() / OnBought()` 交互。

## 二、两种模式的差异

| 维度 | 共享轮换 `SharedShopMode` | 独立买卡 `IndependentShopMode` |
| --- | --- | --- |
| 卡池 | 全场一份，开局 15 张不重复（`TOTAL_HERO_CARDS=15`），5 列布局 | 每人一份，各 5 张（按该玩家 like 生成） |
| 行动顺序 | `TurnOrder`（积分低→高）轮流，一人一次 | 无顺序，各买各的，同时进行 |
| 购买后 | 卡保持售出并显示倒计时 `SOLD_REMAIN_ROUNDS`；相邻卡倒计时 -1，归零立即刷新；其他已售卡倒计时 -1 | 买走即从该玩家面板移除，不保留售出态 |
| 跳过 | `passBtn`「跳过」，跳过后本商店阶段不再轮到他 | 关闭面板即结束，无 pass 概念 |
| 人类刷新 | 支付 `RefreshGoldCost=2` 立即刷新 6 张（不足全刷），不结束回合，可多次 | 同价，重开自己的 5 张面板 |
| AI 刷新上限 | `AiSharedMaxRefreshPerShop(=1)` | `AiMaxShopRefreshPerShop(=2)` |
| 金币充裕加刷 | `+RichExtraRefreshPerShop(=2)`（`PlayerAI.GetShopRefreshLimit`） | 同 |
| 结束条件 | 8 人全部 pass | 所有玩家买完/放弃 |
| AI 节奏 | `Drive()` 每步随机 0.3~0.5s 思考、0.5~0.8s 停顿；有面板打开时暂停 | `Drive()` 逐人执行，同样受面板阻塞保护 |

## 三、公共契约（新增模式只需实现这些）

- `Begin()`：清空并初始化自己的卡面，重置状态，开回合。
- `GetOffers(pid)`：返回该玩家可见的可买 `ShopOffer` 列表（AI 与人类共用）。
- `OnBought(offer, player)`：AI/统一入口购买；`OnHumanBought(player, offer)`：人类购买后的收尾（推进回合）。
- `OnHumanPass()` / `OnHumanRefresh()`：人类操作入口，内部做金币与状态校验。
- `Drive()`：AI 驱动协程，`IsPanelBlocked()` 为真时暂停，`ShouldBotStop()` 为真时中止。
- 商店结束调用 `host.RequestEnd()`。

## 四、共享商店细节

- 首次生成用 `TurnOrder[0]`（积分最低者）的 like 判定；生成时遇到重复卡重抽，最多 `15×20` 次，卡池不足则生成全部不重复卡后结束。
- 刷新单张卡：`RefreshCard(ctr, pid)` 按当前品质概率重新随机，并可能按 `LikeCardRefreshRate` 替换为该玩家的收藏卡（物品卡不参与刷新）。
- 相邻判定：英雄卡按 3 列 × 5 行网格取上下左右；物品卡取左右相邻物品。
- 面板阻塞：背包/排行/查看玩家等面板打开时不驱动 AI，避免玩家看不到过程。

## 五、独立模式细节

- 每名玩家固定 5 张；买卡后即时补位或直接移除（按模式实现），不产生共享棋盘上的售出倒计时。
- AI 无 pass，买不动就结束自己的阶段；人类直接关面板。
- `GetOffers(pid)` 只返回该 pid 自己的卡，天然支持"每个 AI 看到不同商店"。

## 六、验证要点

- 共享店：人类买卡后相邻卡倒计时正确 -1、归零刷新；刷新 6 张扣 2 金；8 人全 pass 后进战斗。
- 独立店：任意玩家买卡不影响他人面板；AI 刷新次数受上限约束。
- 两模式下 `PlayerAI.AiCheckBuyCard` 均能拿到正确的 `owner/price/heroId` 报价。
- 面板打开时 AI 停止驱动，关闭后继续。

## 七、代码索引

| 内容 | 位置 |
| --- | --- |
| 宿主/门面 | [CardShopManager.cs](../../Assets/Resources/Scripts/CardShopManager.cs) |
| 模式基类、枚举与报价结构 | [Shop/ShopMode.cs](../../Assets/Resources/Scripts/Shop/ShopMode.cs)（`ShopModeType` / `ShopOffer` / `ShopMode`） |
| 共享轮换 | [Shop/SharedShopMode.cs](../../Assets/Resources/Scripts/Shop/SharedShopMode.cs) |
| 独立买卡 | [Shop/IndependentShopMode.cs](../../Assets/Resources/Scripts/Shop/IndependentShopMode.cs) |
| AI 买卡/刷新 | [PlayerAI.cs](../../Assets/Resources/Scripts/PlayerAI.cs) |
