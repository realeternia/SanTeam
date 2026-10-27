# MP（法力）系统说明

> 日期：2026-10-27（按当前配置与代码校对）
> 状态：已实施
> 范围：技能法力条（Skill.mp）的充能模型、mpRegen 全部来源及当前数值、攻击回蓝与法力分享
> 数值更新：
> - 2026-10-03 职业基准 mpRegen 1→10；全部技能 MpCost ×5；装备 mpRegen 基础装备→2、合成装备→4
> - 2026-10-27 扇羁绊 mpRegen 现为 +0.5~+4.5/秒；相羁绊不再提供 mpRegen（只加士兵攻血）；刘备·仁德改为「法力分享」，不再提供 mpRegen

---

## 一、基本模型

- **法力是技能级资源**：每个技能各有一条独立法力条 `Skill.mp`（[Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs)），满值 = 该技能 `SkillConfig.MpCost`，战斗开始时为 0（默认）。
- `MpCost <= 0` 的技能不受法力限制，可随时发动；`MpCost > 0` 的技能必须满蓝才能发动（`Skill.IsMpFull` / `CheckBurst`）。
- **充能**：每秒（`Chess.OnSecond`）把棋子的 `mpRegen` 施加到该棋子**所有 `MpCost > 0` 的技能**上：
  `mp = Clamp(mp + mpRegen, 0, MpCost)`（`Skill.AddRegenMp`）。mpRegen 为负时即倒扣充能。
- 战斗开始是否拉满：`CombatConst.FullMpAtBattleBegin`（默认 `false`，即从 0 开始按 mpRegen 充能）。
- 因此某技能的"回蓝速度" = 棋子 mpRegen ÷ 该技能 MpCost（秒/次）。

---

## 二、mpRegen 来源一览（当前数值）

| # | 来源 | 类型 | 数值 | 作用对象 |
|---|------|------|------|----------|
| 1 | 职业基准 `JobConfig.MpRegen` | 基础 | **10/秒**（18 个职业全部） | 自身 |
| 2 | 英雄修正 `HeroConfig.MpRegen` | ±% | 当前 110 行**全为 0** | 自身 |
| 3 | 装备 `ItemConfig.Attrs` 的 `mpRegen+N` | 平铺叠加 | +2 ~ +4（见 3.2） | 自身 |
| 4 | 职业羁绊 `LinkTeam`（扇） | 羁绊 | **+0.5 ~ +4.5/秒** | 我方其他英雄 |
| 5 | 攻击回蓝 `AttackMpGain` | 独立机制 | 概率 × 技能蓝量%（见第五节） | 自身 |
| 6 | 刘备·仁德（法力分享） | 技能机制 | 把自身累积 MP × 80%~160% 分给全队（见 3.4） | 我方全体 |

> 1~4 都是直接改棋子 `mpRegen` 属性、按加法累加；第 5、6 项直接给技能补 mp，不属于 mpRegen。

---

## 三、各来源细节

### 3.1 基础：职业基准 + 英雄修正

- [JobConfig_s.cs](../../Assets/Resources/Scripts/Configs/JobConfig_s.cs) 的 `MpRegen` 列：18 个职业**全部 = 10/秒**（同级 `HpRegen` 全部 = 2/秒）。
- [HeroConfig_s.cs](../../Assets/Resources/Scripts/Configs/HeroConfig_s.cs) 的 `MpRegen` 列是**相对职业基准的 ±% 修正**（0 = 不改），在 `ConfigManager.PostModify` 写回：
  `mpRegen = round(职业基准 × (100 + 修正%) / 100)`，**不乘品质/升星系数**。
- 当前 110 个英雄该列**全部为 0** → 英雄基础 mpRegen 恒为 **10/秒**。

### 3.2 装备（最多 3 件，平铺累加）

`Chess.InitAttr` 中按英雄装备列表把 `equipAttr.MpRegen` 逐件相加（每英雄上限 `PlayerInfo.MaxEquipSlots = 3`）：

| 装备 | Id | 品质 | mpRegen | 合成路径 |
|---|---|---|---|---|
| 葫芦 | 402005 | 1 | **+2** | 基础材料 |
| 易经 | 400011 | 2 | **+4** | 羽扇 + 葫芦 |
| 玉龙壁 | 400023 | 2 | **+4** | 斗篷 + 葫芦 |
| 古锭刀 | 400025 | 2 | **+4** | 长刀 + 葫芦 |
| 大克鼎 | 400028 | 2 | **+4** | 皮革甲 + 葫芦 |
| 象鞭 | 400031 | 2 | **+4** | 护手 + 葫芦 |
| 李广弓 | 400042 | 2 | **+4** | 檀木弓 + 葫芦 |
| 玉玺 | 400037 | 2 | **+4** | 葫芦 + 葫芦 |

### 3.3 职业羁绊（LinkTeam，"mpRegen+X"）

上阵同职业 1/2/3/4/5 人 → Lv1~Lv5；羁绊的 `LinkTeam` 效果施加给**我方除该职业英雄外的全体英雄**（`JobLinkManager`），连接英雄自身不吃 LinkTeam。

| 职业 | Sname | 技能 Id | Lv1~Lv5 mpRegen |
|---|---|---|---|
| 智士(401) | 扇 | 2000021 ~ 2000025 | **+0.5 / +1 / +2 / +3 / +4.5** 每秒 |

> 扇的连接英雄自身获得"负面 buff 时长 +30%~200%"，不含 mpRegen；`Strength2[mpRegen]` 与 `LinkTeam` 两处数值需保持一致。
> 相（丞相/402）现在只提供 `LinkSelf` 士兵攻击/生命（20%~120%），**不再提供 mpRegen**。

### 3.4 刘备·仁德（法力分享，`SkillAidManaShare`）

技能 Id 2020001 ~ 2020005，`MpCost=100`、`CD=3.5s`。发动时清空本技能累积法力，按下列公式分给我方全体英雄（含自身）：

```
总量 = 发动前累积 MP × (100 + 法强) / 100 × shareRate
每人 = 总量 / 我方英雄数
```

| Lv | shareRate | 自身攻击 Buff(`攻`) | 持续 | MpCost | CD |
|---|---|---|---|---|---|
| 1 | **80%** | +20 | 5s | 100 | 3.5s |
| 2 | **100%** | +32 | 5s | 100 | 3.5s |
| 3 | **120%** | +48 | 5s | 100 | 3.5s |
| 4 | **140%** | +70 | 5s | 100 | 3.5s |
| 5 | **160%** | +100 | 5s | 100 | 3.5s |

> 每名友军会把"每人"份额加到其**所有未满的蓝耗技能**上（逐技能充能），因此队伍技能越多，实际总灌注量越大。
> 刘备自身也要先靠 mpRegen 攒满 100 点蓝才能发动——"攒蓝→分享放大"。
> 注意：好友组 G5「仁德」的技能缩写是 `仁`（`SkillInitAddItem`，开局概率获得万民书），与本节的刘备个人技能 `刘备`（仁德）不是同一个技能。

---

## 四、叠加示例（单英雄）

| 组成 | 数值 |
|---|---|
| 职业基准 | 10.0 |
| 3 件装备满配（玉玺 ×3） | +12（需 6 个葫芦，实际受经济/掉落限制） |
| 羁绊（队伍上阵 2 智士，非智士英雄） | +0.5 |
| **常规对局典型值** | **约 10 ~ 22/秒** |
| 极端堆叠理论上限 | 22.5/秒（不含仁德一次性灌注） |

---

## 五、攻击回蓝（独立机制，非 mpRegen）

`SkillAttackMpGain`：普攻命中时按 `Rate` 概率，回复"该英雄首个 `MpCost > 0` 技能"的 `Strength2[mpGain] × MpCost` 蓝量。

| 技能 | Sname | 技能 Id | 触发概率 | 回复蓝量 |
|---|---|---|---|---|
| 儒雅 | 儒 | 2010151 ~ 2010155 | 15% / 25% / 35% / 45% / 60% | 100% |
| 回气 | 回气 | 2090005 | 50% | 50% |
| 浩然 | 浩然 | 2090016 | 70% | 50% |
| 凝气 | 凝气 | 2090025 | 65% | 50% |

> 来源：儒雅为好友连锁连接技能；回气/浩然/凝气分别来自装备 玄光法袍(400036) / 易经(400011) / 玉龙壁(400023)。

---

## 六、代码索引

| 内容 | 位置 |
|---|---|
| 每秒充能结算 | [Chess.cs](../../Assets/Resources/Scripts/Combat/Chess.cs) `OnSecond()` → `Skill.AddRegenMp` |
| 技能法力条 | [Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs)（`mp` / `MpCost` / `AddRegenMp` / `IsMpFull`） |
| 开局拉满开关 | [CombatConst.cs](../../Assets/Resources/Scripts/Combat/CombatConst.cs) `FullMpAtBattleBegin` |
| 职业基准值 | [JobConfig_s.cs](../../Assets/Resources/Scripts/Configs/JobConfig_s.cs) `MpRegen` |
| 英雄 ±% 修正 | [HeroConfig_s.cs](../../Assets/Resources/Scripts/Configs/HeroConfig_s.cs) `MpRegen` + [ConfigManager.cs](../../Assets/Resources/Scripts/Configs/ConfigManager.cs) `PostModify` |
| 装备叠加 | [Chess.cs](../../Assets/Resources/Scripts/Combat/Chess.cs) `InitAttr`（装备循环 `mpRegen += equipAttr.MpRegen`） |
| 羁绊施加 | [JobLinkManager.cs](../../Assets/Resources/Scripts/Combat/JobLinkManager.cs)（`case "mpRegen"`） |
| 刘备·仁德法力分享 | [SkillAidManaShare.cs](../../Assets/Resources/Scripts/Combat/Skill/SkillAidManaShare.cs) |
| 攻击回蓝 | [SkillAttackMpGain.cs](../../Assets/Resources/Scripts/Combat/Skill/SkillAttackMpGain.cs) |
