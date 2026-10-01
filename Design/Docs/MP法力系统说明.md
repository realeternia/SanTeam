# MP（法力）系统说明

> 日期：2026-10-02
> 状态：已实施
> 范围：技能法力条（Skill.mp）的充能模型、mpRegen 全部来源及当前数值、攻击回蓝等附属机制
> 数值更新：2026-10-02 扇/相羁绊 mpRegen 由 +1~+5 下调为 +0.3~+1.5；刘备·仁德 mpRegen 下调为原值的 60%

---

## 一、基本模型

- **法力是技能级资源**：每个技能各有一条独立法力条 `Skill.mp`（`Combat/Skill/Skill.cs`），满值 = 该技能 `SkillConfig.MpCost`，战斗开始时为 0（默认）。
- `MpCost <= 0` 的技能不受法力限制，可随时发动；`MpCost > 0` 的技能必须满蓝才能发动（`Skill.IsMpFull` / `CheckBurst`）。
- **充能**：每秒（`Chess.OnSecond`）把棋子的 `mpRegen` 施加到该棋子**所有 `MpCost > 0` 的技能**上：
  `mp = Clamp(mp + mpRegen, 0, MpCost)`（`Skill.AddRegenMp`）。mpRegen 为负时即倒扣充能。
- 战斗开始是否拉满：`CombatConst.FullMpAtBattleBegin`（默认 `false`，即从 0 开始按 mpRegen 充能）。
- 因此某技能的"回蓝速度" = 棋子 mpRegen ÷ 该技能 MpCost（秒/次）。

---

## 二、mpRegen 来源一览（当前数值）

| # | 来源 | 类型 | 数值 | 作用对象 |
|---|------|------|------|----------|
| 1 | 职业基准 `JobConfig.MpRegen` | 基础 | **1/秒**（18 个职业全部） | 自身 |
| 2 | 英雄修正 `HeroConfig.MpRegen` | ±% | 当前 110 行**全为 0** | 自身 |
| 3 | 装备 `ItemConfig.Attrs` 的 `mpRegen+N` | 平铺叠加 | +1 ~ +4（见 3.2） | 自身 |
| 4 | 职业羁绊 `LinkTeam`（扇 / 相） | 羁绊 | **+0.3 ~ +1.5/秒** | 我方其他英雄 |
| 5 | Buff「仁政」（刘备·仁德） | 技能 Buff | **+0.6 ~ +1.5/秒** | 我方全体 |
| 6 | 攻击回蓝 `AttackMpGain` | 独立机制 | 概率 × 技能蓝量%（见第五节） | 自身 |

> 1~5 都是直接改棋子 `mpRegen` 属性、按加法累加；第 6 项是直接给技能补 mp，不属于 mpRegen。

---

## 三、各来源细节

### 3.1 基础：职业基准 + 英雄修正

- `Configs/JobConfig_s.cs` 的 `MpRegen` 列：18 个职业**全部 = 1/秒**（同级 `HpRegen` 全部 = 2/秒）。
- `Configs/HeroConfig_s.cs` 的 `MpRegen` 列是**相对职业基准的 ±% 修正**（0 = 不改），在 `ConfigManager.PostModify` 写回：
  `mpRegen = round(职业基准 × (100 + 修正%) / 100)`，**不乘品质/升星系数**。
- 当前 110 个英雄该列**全部为 0** → 英雄基础 mpRegen 恒为 **1/秒**。

### 3.2 装备（最多 3 件，平铺累加）

`Chess.InitAttr` 中按英雄装备列表把 `equipAttr.MpRegen` 逐件相加（每英雄上限 `PlayerInfo.MaxEquipSlots = 3`）：

| 装备 | Id | 品质 | mpRegen | 合成路径 |
|---|---|---|---|---|
| 葫芦 | 402005 | 1 | **+1** | 基础材料 |
| 易经 | 400011 | 2 | **+2** | 羽扇 + 葫芦 |
| 玉龙壁 | 400023 | 2 | **+2** | 斗篷 + 葫芦 |
| 古锭刀 | 400025 | 2 | **+2** | 长刀 + 葫芦 |
| 大克鼎 | 400028 | 2 | **+2** | 皮革甲 + 葫芦 |
| 象鞭 | 400031 | 2 | **+2** | 护手 + 葫芦 |
| 李广弓 | 400042 | 2 | **+2** | 檀木弓 + 葫芦 |
| 玉玺 | 400037 | 2 | **+4** | 葫芦 + 葫芦 |

### 3.3 职业羁绊（LinkTeam，"mpRegen+X"）

上阵同职业 1/2/3/4/5 人 → Lv1~Lv5；羁绊的 `LinkTeam` 效果施加给**我方除该职业英雄外的全体英雄**（`JobLinkManager`），连接英雄自身不吃 LinkTeam。

| 职业 | Sname | 技能 Id | Lv1~Lv5 mpRegen |
|---|---|---|---|
| 智士(401) | 扇 | 2000021 ~ 2000025 | **+0.3 / +0.6 / +0.9 / +1.2 / +1.5** 每秒 |
| 丞相(402) | 相 | 2000061 ~ 2000065 | **+0.3 / +0.6 / +0.9 / +1.2 / +1.5** 每秒 |

> 扇 / 相 的两个连接英雄自身分别获得"负面 buff 延长 / 士兵攻血%"，不含 mpRegen。
> 描述中该数值由 `LinkTeam` 字符串动态解析（相）或 `Strength2[1]`（扇）渲染，改配置即可同步。

### 3.4 Buff：仁政（刘备·仁德）

技能 Id 2021021 ~ 2021025，`SkillAidBenevolence` 给我方范围内全体挂 Buff「仁」，由 `BuffHpMpRegen` 每秒加减：

| Lv | mpRegen | 生命回复 | 持续 | CD | MpCost |
|---|---|---|---|---|---|
| 1 | **+0.6/秒** | +6/秒 | 6s | 8s | 16 |
| 2 | **+0.9/秒** | +7/秒 | 6s | 8s | 16 |
| 3 | **+1.2/秒** | +8/秒 | 7s | 8s | 16 |
| 4 | **+1.32/秒** | +9/秒 | 7s | 8s | 16 |
| 5 | **+1.5/秒** | +10/秒 | 8s | 8s | 16 |

> 刘备自身也要靠 mpRegen 攒满 16 点蓝才能施放，属于"回蓝→爆发"的自循环。

---

## 四、叠加示例（单英雄）

| 组成 | 数值 |
|---|---|
| 职业基准 | 1.0 |
| 3 件装备满配（玉玺 ×3） | +12（需 6 个葫芦，实际受经济/掉落限制） |
| 羁绊（队伍 1 智士 + 1 丞相，非这两职业的英雄） | +0.3 + 0.3 = +0.6 |
| 仁政（刘备在阵且覆盖到） | +0.6 ~ +1.5 |
| **常规对局典型值** | **约 1 ~ 3/秒** |
| 极端堆叠理论上限 | 15+/秒 |

---

## 五、攻击回蓝（独立机制，非 mpRegen）

`SkillAttackMpGain`：普攻命中时按 `Rate` 概率，回复"该英雄首个 `MpCost > 0` 技能"的 `Strength2[0] × MpCost` 蓝量。

| 技能 | Sname | 技能 Id | 触发概率 | 回复蓝量 |
|---|---|---|---|---|
| 儒雅 | 儒 | 2010151 ~ 2010155 | 15% / 20% / 25% / 30% / 40% | 20% / 30% / 40% / 50% / 60% |
| 回气 | 回气 | 2090005 | 50% | 50% |
| 浩然 | 浩然 | 2090016 | 70% | 50% |
| 凝气 | 凝气 | 2090025 | 65% | 50% |

> 来源：儒雅为好友连锁连接技能；回气/浩然/凝气分别来自装备 玄光法袍(400036) / 易经(400011) / 玉龙壁(400023)。

---

## 六、代码索引

| 内容 | 位置 |
|---|---|
| 每秒充能结算 | `Combat/Chess.cs` `OnSecond()` → `Skill.AddRegenMp` |
| 技能法力条 | `Combat/Skill/Skill.cs`（`mp` / `MpCost` / `AddRegenMp` / `IsMpFull`） |
| 开局拉满开关 | `Combat/CombatConst.cs` `FullMpAtBattleBegin` |
| 职业基准值 | `Configs/JobConfig_s.cs` `MpRegen` |
| 英雄 ±% 修正 | `Configs/HeroConfig_s.cs` `MpRegen` + `ConfigManager.PostModify` |
| 装备叠加 | `Combat/Chess.cs` `InitAttr`（装备循环 `mpRegen += equipAttr.MpRegen`） |
| 羁绊施加 | `Combat/JobLinkManager.cs`（`case "mpRegen"`） |
| 仁政 Buff | `Combat/Skill/SkillAidBenevolence.cs` + `Combat/Buff/BuffHpMpRegen.cs` |
| 攻击回蓝 | `Combat/Skill/SkillAttackMpGain.cs` |
