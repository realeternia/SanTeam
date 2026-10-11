# AI 选牌：好友/职业聚焦 + 好友收益平档 + 淘汰渐进

## Context（为什么改）

50 回合无头模拟（[CardPickSim_20261011_080312.txt](file:///d:/U3dPrj/SanTeam/Tools/选牌模拟器/CardPickSimulator/Logs/CardPickSim_20261011_080312.txt)）暴露两个问题：

1. **好友羁绊几乎不成团**：8 名玩家 × 各自喜好好友组，最终只有 1 例成团（钱多·王佐Lv3）。根因：评分逐卡贪心、无"锁定目标组"的长期规划；好友收益梯度陡（`friendCount==1` 只给 0.3、`≥2` 才 0.8）、权重低（`FriendBias 0.8/1.5`）易被 `SameCardRate(1.5)`、`GetQualityBias(最高2.0)` 盖过。
2. **有效金币低 / 淘汰断档**：P1 期末现金 123、阵容价值仅 104，50 回合刷新烧 138 金、卖卡折扣 209 金；且 [EliminateOutdatedCards](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/PlayerAI.cs#L115-L148) 一回合把所有 ≤ 淘汰线的非上阵卡卖光，造成阵容断档。

用户诉求（已确认口径）：
- 用 **Q4 核心卡单张** 触发"聚焦"；聚焦对象为 **好友组或职业**，国家不聚焦；**名额共 2 个**（2 个好友组，或 1 好友 + 1 职业）。
- **拉平好友"差一人"档**；**提升好友/职业倍率**，使其不被同卡率/品质偏好盖过。
- **淘汰期逐回合渐进，每回合最多卖 1 张**，不出现单回合阵容断档。
- 国家、职业作为**辅助选择**参与评分。

## 方案

### 1. 聚焦机制（好友组 / 职业，派生式、无状态）

**核心思路：不新增存档字段、不做显式重置**——聚焦目标每回合从"当前已拥有的 Q4 核心卡"**派生**，卖掉核心卡即自动释放，天然跨回合稳定。

- 新增 `PlayerAI.DeriveFocusTargets(PlayerInfo) -> List<FocusTarget>`（`FocusTarget{ bool isJob; int friendGroupId; string jobNameS; }`）：
  - 遍历玩家拥有的英雄卡（≤15 张），取 `HeroConfig.Quality == CombatConst.FocusCoreQuality(=4)` 者为"核心卡"。
  - **好友候选**：每张核心卡所属各好友组（复用 [ConfigManager.GetHeroFriendInfo](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Configs/ConfigManager.cs#L237-L244)）→ 统计该组"已拥有成员数 / 已拥有Q4数"；Q4数 ≥ 1 入候选。
  - **职业候选**：每张核心卡的 `HeroConfig.Job` → 统计"已拥有同职业数 / Q4数"；Q4数 ≥ 1 入候选。
  - 排序：`Q4数 desc → 已拥有成员数 desc → 档位进度 desc`，取前 `CombatConst.FocusMaxTargets(=2)`。好友/职业**共用名额**，靠排序自然产生"2好友"或"1好友+1职业"。
  - 复用 [ConfigManager.GetFriendLevel](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Configs/ConfigManager.cs#L228-L235)、[HeroFriendConfig](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Configs/HeroFriendConfig_s.cs)、`ConfigManager.GetJobConfig`、`GetTierLevel`。
- 把 `focusTargets` 放进 [AiContext](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/PlayerAI.cs#L649-L672)，在 [BuildContext](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/PlayerAI.cs#L675-L691) 内一并构建，供买卡评分与卖卡/淘汰保护共用同一快照。
- **买卡评分**（[AiCheckBuyCard 评分链](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/PlayerAI.cs#L304-L322)）：候选卡命中任一聚焦目标（好友组 ∈ focus，或职业 == focus）时，追加 `score *= 1 + CombatConst.FocusBias * gain`，其中 `gain` 复用 `GetFriendGain` / `GetJobGain`。**不要求** `LikeFriend`/`LikeJob` 命中（聚焦是"发现"出来的，不是偏好）。
- **保护**：`FindWeakCard` 与淘汰清仓跳过聚焦目标成员。

> 触发窗口：默认不限回合（由"是否拥有 Q4 核心卡"驱动）。若需只在前期触发，加 `CombatConst.FocusTriggerMaxRound`（默认 0=不限）预留开关。

### 2. 拉平好友"差一人"档 + 提升权重

- [GetFriendGain](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/PlayerAI.cs#L778-L830)：`friendCount == 1` 的收益 `0.3 → CombatConst.FriendGainOneShort(=0.7)`（贴近成团档 0.8），让"第 2 人"真正有吸引力。
- 常量提升（`Combat/CombatConst.cs`，避免被 `SameCardRate 1.5` / `GetQualityBias 2.0` 盖过）：

  | 常量 | 现值 | 建议值 |
  |---|---|---|
  | `FriendBias` | 0.8 | 1.1 |
  | `FriendBiasTop` | 1.5 | 1.8 |
  | `JobChainBias` | 0.6 | 0.8 |
  | `JobChainBiasTop` | 1.0 | 1.3 |
  | 新增 `FocusBias` | — | 1.2 |
  | 新增 `FocusCoreQuality` | — | 4 |
  | 新增 `FocusMaxTargets` | — | 2 |
  | 新增 `FriendGainOneShort` | — | 0.7 |
  | 新增 `EliminateMaxSellPerRound` | — | 1 |

### 3. 淘汰期逐回合渐进

- 改 [EliminateOutdatedCards](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/PlayerAI.cs#L115-L148)：先按"品质升序 + 非聚焦 + 非成组成员"排序，然后**每回合最多卖 `EliminateMaxSellPerRound(=1)` 张**。
- 跳过：聚焦目标成员、"正在成组"成员（同组已拥有 ≥ 2）、主力/主公/初始卡/当回合买入（保持现有保护）。
- 效果：淘汰在多个回合内摊开，不出现单回合清空、阵容断档。

### 4. 国家/职业的"辅助"定位

- 国家（`LikeForce + GetForceGain`）保持不变、不聚焦——仅作辅助评分。
- 职业：保留 `LikeJob` 路径 + 新增"职业聚焦"路径（见 1），倍率提升（见 2）。

## 影响文件

| 文件 | 改动 |
|---|---|
| [PlayerAI.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/PlayerAI.cs) | 核心：新增 `DeriveFocusTargets`/`FocusTarget`；`AiContext` 加聚焦目标；评分链加聚焦倍率；`GetFriendGain` 平档；`EliminateOutdatedCards` 渐进 + 保护；`FindWeakCard` 保护聚焦成员 |
| [CombatConst.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/CombatConst.cs) | 上表常量新增/调整 |
| [Docs/AI商店逻辑.md](file:///d:/U3dPrj/SanTeam/Docs/AI商店逻辑.md) | 同步聚焦机制、平档、淘汰渐进、常量表 |

无需改动 `PlayerInfo` / 存档（派生式无状态）；无需新增 `.cs` 文件（符合工程规范）。

## 验证

1. 编译：`dotnet build "CardPickSimulator.csproj" -c Debug -v m --nologo`（cwd = `Tools/选牌模拟器/CardPickSimulator`）→ 0 错误。
2. 50 回合冒烟：复用现有 smoke 脚本（stdout 重定向到 output.log）。
3. 观察项（对比改前 080312 日志）：
   - **好友成团数上升**：结算区 `羁绊(Lv≥2)` 中出现的 `好友·xxx` 条目数应明显 > 1。
   - **淘汰渐进**：grep output.log 的 `淘汰清仓 卖出` —— 同一玩家同一回合最多 1 条。
   - **有效金币**：对照 `上阵价值 / 刷新+升级消耗 / 卖卡损失`，不应恶化。
   - 无异常/无回归（`FINISHED ok=True`）。
4. 若出现聚焦"抖动"（相邻回合来回切换目标）或某组垄断，按需微调 `FocusBias` / 排序权重。