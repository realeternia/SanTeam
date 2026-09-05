# 连锁机制与玩家等级体系方案

> 日期：2026-08-28（2026-08-29 更新：技能等级字段落地，好友连锁·特殊 / 兵种连锁 / 玩家等级体系(基础) 已实现）
> 状态：三大连锁机制与玩家等级体系（基础）已实施（见文末 TODO 清单）
> 目标：整理三大连锁机制（国家连锁 / 好友连锁 / 兵种连锁）与金铲铲式玩家等级体系，明确哪些已实现、哪些待实现。

---

## 一、总览

| 机制 | 类型 | 触发方式 | 效果 | 状态 |
| --- | --- | --- | --- | --- |
| 国家连锁 | 阵营羁绊 | 同阵营英雄数达标（3/5/7/9） | 全阵营英雄获得护盾；君主（帅）在场护盾加倍 | ✅ 已实现 |
| 好友连锁·普通 | 武将关系 | 场上好友数量达标（2~7） | 连线特效 + 攻击强化 | ✅ 已实现 |
| 好友连锁·特殊 | 武将关系 | 配置了关联技能（HeroFriendConfig.SkillId>0）的关系在场 | 激活关联技能（按关系配置颜色拉线、不加攻击属性），好友越多技能等级越高 | ✅ 已实现 |
| 兵种连锁 | 职业羁绊 | 场上同兵种（Job）英雄数量 | 兵种技能等级 = 同兵种英雄数（默认1级，每多一个+1级） | ✅ 已实现 |
| 玩家等级体系 | 成长系统 | 战斗获胜/失败获得经验，升级解锁上阵格子 | 升级解锁上阵格子（10级后9格全解锁）、预留买经验 | ✅ 已实现（基础） |

- 连锁数值统一维护在 [CombatConst.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/CombatConst.cs)，新机制沿用该约定。

---

## 二、国家连锁（已实现）

> 实现：`Combat/FactionShieldManager.cs`（`ApplyFactionShields`，战斗开始时结算）

### 1. 触发与效果
- 统计本侧场上**同阵营（HeroConfig.Side）**英雄数量。
- 达到档位后，该阵营所有英雄按**最大生命值百分比**获得护盾，持续整场战斗（`FactionShieldTime = 999`）。

| 同阵营英雄数 | 护盾（% 最大生命） |
| --- | --- |
| ≥ 3 | 18% |
| ≥ 5 | 24% |
| ≥ 7 | 30% |
| ≥ 9 | 36% |

### 2. 君主强化
- 主公技（王/帅，技能 id `200001`，职业 `shuai`）：所在阵营存在携带主公技的英雄时，**该阵营护盾效果加倍**（`MasterShieldDouble = 2`）。
- 护盾走 `BuffManager.AddShield`（BuffId `300001` 护盾），不吃减伤、独立结算。

### 3. 配置位置
- 档位 / 倍率 / 时长：`CombatConst.FactionShieldCounts / FactionShieldRates / FactionShieldTime / MasterShieldSkillId / MasterShieldDouble`
- 主公技属于职业默认技能（`JobConfig` 中 `shuai` 的 `SkillId = 200001`，由 `ConfigManager.PostModify` 注入每个 `shuai` 英雄）。

---

## 三、好友连锁

> 好友关系配置：`HeroFriendConfig`（目前 108 组，如「桃源结义」「五虎上将」，Level 支援级别 1~3，最多 5 人）。
> 查询接口：`ConfigManager.GetFriendLevel(heroId, friendId)`（双向配对，返回支援级别，无关系返回 0）。

### 1. 普通：连线 + 属性（已实现）

> 实现：`Combat/FriendLineManager.cs`（`ApplyFriendLines`，战斗开始时结算）

- 每个英雄统计场上与其有好友关系的其他英雄数量。
- 按档位获得**攻击强化**，并生成连线特效（`Prefabs/LaserLine`，`GlowBeamController`）。

| 连线好友数 | 攻击强化 |
| --- | --- |
| ≥ 2 | +5% |
| ≥ 3 | +10% |
| ≥ 4 | +15% |
| ≥ 5 | +20% |
| ≥ 6 | +25% |
| ≥ 7 | +30% |

- 配置位置：`CombatConst.FriendLineCounts / FriendLineAtkRates`；战斗属性在 `Chess.ApplyFriendAtkBonus / RefreshHeroAttr` 中生效，好友死亡时实时重算（`Chess.OnFriendDie`）。

### 2. 特殊：关联技能（已实现）

> 实现：`Combat/FriendLineManager.cs`（`ApplyFriendSpecialSkills`，战斗开始时与普通连锁一并结算）
> 配置：`HeroFriendConfig` 新增 **SkillId（关联技能）** 与 **LineColor（连线颜色）** 两列（xlsx 列 6/7）。

- **判定**：某对好友之间的关系配置了 `SkillId > 0` → 特殊连锁；未配置（=0）→ 普通连锁（加属性）。
- **效果**：特殊连锁**不加攻击属性**，改为**激活关联技能**；**仍然拉线**，线颜色取 `HeroFriendConfig.LineColor`（HTML 色值，如 `#FF0000`；未配置时回退玩家默认线色）。
- **技能等级**：默认 0 级（无技能），该关系组在场成员每多一个 **+1 级**（等级 = 该关系组在场成员数，不含自己）。
- **技能来源**：技能在 `SkillConfig` 中新建即可，战斗开始时通过 `Chess.AddSkill` 动态授予；技能尚未配置时仅告警跳过，不影响其它逻辑。
- **查询接口**：`ConfigManager.GetFriendSkillId(heroId, friendId)`（>0 为特殊连线）、`ConfigManager.GetFriendLineColor(heroId, friendId)`。
- 等级字段：`Skill.Level`（默认取配置 Lv=5），由 `Skill.SetLevel()` 修正；**当前仅记录等级、不缩放技能数值**，数值联动后续接入。

---

## 四、兵种连锁（已实现）

> 实现：`Combat/JobLinkManager.cs`（`ApplyJobLinks`，战斗开始时结算）
> 配置：`CombatConst.JobLinkBaseLevel = 1`（默认兵种技能起始等级）

- 规则：统计本侧场上**相同兵种（HeroConfig.Job / JobConfig）**英雄数量，**兵种技能等级 = 同兵种英雄数**（默认 1 级，每多一个 +1 级，即 `JobLinkBaseLevel + 同兵种数 - 1`）。
- 兵种技能即职业默认技能：`JobConfig.SkillId`（如 帅→200001、马→200005、弓→200007、士→200004、扇→200002、鼓→200016、刀→200003），由 `ConfigManager.PostModify` 注入每个英雄。
- 等级字段：`Skill.Level`（默认取配置 Lv=5），由 `Skill.SetLevel()` 修正；**当前仅记录等级、不缩放技能数值**，数值联动后续接入。
- 设计说明：早期档位方案（≥2 → Lv2、≥4 → Lv3 及对应倍率）已改为**线性规则**「每多一个 +1 级」，档位/倍率预留 `CombatConst` 常量位，后续如需回退可在此扩展。

---

## 五、玩家等级体系（参考金铲铲，已实现）

> 实现：`PlayerInfo`（level / exp / AddExp / GetSlotCount / BuyExp）、配置表 `PlayerLevelConfig`（1~10 级）、战斗结算 `PlayerInfo.onBattleResult` 获得经验。
> 经验曲线参考金铲铲，**节奏放慢一倍**（升级所需经验为金铲铲经典曲线的 2 倍；不再每回合自动加经验，改为战斗获胜/失败获得）。

### 1. 等级与经验
- 玩家有等级（1~10 级，最高 10 级）与经验值；经验通过**战斗结算**获得：
  - **获胜 +2 经验**、**失败 +1 经验**（金铲铲每回合 +2 经验，这里节奏放慢一倍：胜利才给满、失败给一点）。
  - 后续支持**花金币买经验**：已预留 `PlayerInfo.BuyExp()`（4 金币 = 4 经验，1 金币 = 1 经验），UI 待接入。
- 经验达到当前等级需求自动升级，溢出经验顺延到下一级；满级 10 级后不再获得经验。

### 2. 玩家等级表（PlayerLevelConfig）

| 等级 | 升级所需经验 | 上阵格子数 |
| --- | --- | --- |
| 1 | 4 | 2 |
| 2 | 4 | 3 |
| 3 | 12 | 4 |
| 4 | 20 | 5 |
| 5 | 40 | 6 |
| 6 | 72 | 7 |
| 7 | 112 | 8 |
| 8 | 160 | 9 |
| 9 | 200 | 9 |
| 10 | 0（满级） | 9 |

- 升级所需经验 = 金铲铲经典曲线（2/2/6/10/20/36/56/80/100）× 2（节奏放慢一倍）。
- 上阵格子：每升一级 +1，**10 级后 9 个格子全解锁**；格子数上限 9（`CombatConst.PlayerMaxSlot`）。数值可在表内调整。

### 3. 上阵格子解锁（已实现）
- `PlayerInfo.battleCards` 由 `int[6]` 扩展为 `int[9]`（兼容旧存档：反序列化后自动补齐为 9 格）。
- `GetSlotCount()` 返回当前等级可上阵格子数；`SetBattlePos / AutoSetBattleCard / GetBattleCardList / GetStrongCardList / RearrangePos` 均按格子数限制（未解锁格子不可上阵、不参与战斗）。
- AI 与玩家一视同仁：AI 同样通过战斗获得经验、等级成长，自动按格子数上阵最强英雄。
- UI：`BagControl` 战场区固定显示 9 个格子，信息栏显示 Lv / 经验 / 格子数。
- ⚠️ 场景注意：`MapConfig.RegionHeroSideX` 武将出生点目前每阵营约 5~6 个，要让 9 格全部生效，需在场景中为每阵营补充武将出生点。

### 4. 商店牌范围（品质）
- **取消**：不需要按等级解锁商店品质范围，商店仍按原 `ShopConfig`（按 year）刷牌。

### 5. 买经验（预留）
- 规则：**4 金币 = 4 经验**（金铲铲 4 金币买 4 经验，1 金币 = 1 经验）。
- 接口：`PlayerInfo.BuyExp()` 已就绪（扣 4 金 + 4 经验），UI 入口后续接入。

---

## 六、TODO 清单

### 已实现（无需开发）
- [x] 国家连锁：同阵营护盾（`FactionShieldManager`）+ 君主（帅）护盾加倍
- [x] 好友连锁·普通：连线特效 + 分档攻击强化（`FriendLineManager`）
- [x] 好友连锁·特殊：HeroFriendConfig 新增 `SkillId / LineColor`，特殊关系激活关联技能（等级 = 关系组在场人数）、不加攻击属性、按配置颜色拉线（`FriendLineManager.ApplyFriendSpecialSkills` / `ConfigManager.GetFriendSkillId / GetFriendLineColor`）
- [x] 兵种连锁：兵种技能等级 = 同兵种英雄数（默认 1 级，每多一个 +1 级）（`JobLinkManager` / `CombatConst.JobLinkBaseLevel`）
- [x] 技能等级字段：`Skill.Level`（默认取配置 Lv，当前全表已统一刷为 5）；`Skill.SetLevel()` 供连锁机制修正等级
- [x] 玩家等级体系（基础）：`PlayerLevelConfig`（1~10 级）+ `PlayerInfo.level/exp`（含存档）+ 战斗获胜/失败给经验 + 上阵格子按等级解锁（9 格封顶）+ `BagControl` 9 格展示
- [x] 商店牌范围（品质）：确认取消，不按等级解锁

### 待开发
- [ ] **技能等级 → 数值联动**：把 `Skill.Level` 接入技能数值/倍率公式（`Skill.cs` / `SkillManager.cs`），当前仅记录等级、不缩放数值
- [ ] **买经验 UI**：`PlayerInfo.BuyExp()`（4 金币 = 4 经验）已就绪，接商店/玩家面板入口
- [ ] **买完立刻刷新卡**：购买某张卡后该卡位立即刷新（`CardShopManager` / `CardViewControl.OnSold`）
- [ ] **场景武将出生点**：`MapConfig.RegionHeroSideX` 每阵营补充到 9 个，让 9 格全部生效

---

## 七、验证点
1. 3/5/7/9 同阵营英雄分别获得 18%/24%/30%/36% 护盾；帅在场翻倍。
2. 2~7 名好友连线获得 5%~30% 攻击加成，连线特效正常，好友阵亡实时重算。
3. HeroFriendConfig 配置了 SkillId 的关系在场时：关联技能被授予且等级 = 关系组在场人数；不加攻击属性；连线颜色取配置色（日志：`FriendSpecial 关系X 武将Y 好友数N 技能Z 等级N`）。
4. 场上同兵种英雄数为 N 时，该兵种技能等级 = N（日志：`JobLink 兵种X 英雄数N 兵种技能Z 等级N`）。
5. 战斗获胜 +2 经验、失败 +1 经验；升级后上阵格子数按 PlayerLevelConfig 解锁（日志：`玩家X 升级到 N 级，上阵格子 M`），10 级后 9 格全解锁。
6. 玩家升到对应等级后：上阵格子按 PlayerLevelConfig 解锁（商店牌范围/品质不随等级解锁，已确认取消）；买卡立即刷新卡位。

> 注：第 3、4 点当前仅修正技能等级（`Skill.Level`），技能数值尚未按等级缩放；数值联动见 TODO「技能等级 → 数值联动」。
