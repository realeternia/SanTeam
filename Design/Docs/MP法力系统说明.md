# MP（法力）系统说明

> 日期：2026-11-03（按当前配置与代码校对）
> 状态：已实施
> 范围：技能法力条（Skill.mp）的充能模型、mpRegen 全部来源及当前数值、攻击回蓝、法力分享，以及与 `realeternia/TOMClassic` 的系统对比
> 数值更新：
> - 2026-10-03 职业基准 mpRegen 1→10；全部技能 MpCost ×5；装备 mpRegen 基础装备→2、合成装备→4
> - 2026-10-27 扇羁绊 mpRegen 现为 +0.5~+4.5/秒；相羁绊不再提供 mpRegen（只加士兵攻血）；刘备·仁德改为「法力分享」，不再提供 mpRegen
> - 2026-11-03 补充与 `realeternia/TOMClassic` 的对比；该仓库不是金铲铲/云顶之弈项目，不能将其当作金铲铲规则来源

---

## 一、基本模型

- **法力是技能级资源**：每个技能各有一条独立法力条 `Skill.mp`，满值 = 该技能 `SkillConfig.MpCost`。
- `MpCost <= 0` 的技能不受法力限制，可随时发动；`MpCost > 0` 的技能必须满蓝才能发动（`Skill.IsMpFull` / `CheckBurst`）。
- **充能**：每秒（`Chess.OnSecond`）把棋子的 `mpRegen` 施加到该棋子**所有 `MpCost > 0` 的技能**上：
  `mp = Clamp(mp + mpRegen, 0, MpCost)`（`Skill.AddRegenMp`）。mpRegen 为负时即倒扣充能。
- 战斗开始是否拉满由 `CombatConst.FullMpAtBattleBegin` 控制；默认 `false`，即从 0 开始按 mpRegen 充能。
- 因此某技能从空蓝充满的时间 = `MpCost ÷ mpRegen` 秒（mpRegen>0，且不计攻击回蓝、开局充能、分享等额外效果）。

---

## 二、mpRegen 来源一览（当前数值）

| # | 来源 | 类型 | 数值 | 作用对象 |
|---|------|------|------|----------|
| 1 | 职业基准 `JobConfig.MpRegen` | 基础 | **10/秒**（18 个职业全部） | 自身 |
| 2 | 英雄修正 `HeroConfig.MpRegen` | ±% | 当前 110 行全为 0 | 自身 |
| 3 | 装备 `ItemConfig.Attrs` 的 `mpRegen+N` | 平铺叠加 | +2 ~ +4（见 3.2） | 自身 |
| 4 | 职业羁绊 `LinkTeam`（扇） | 羁绊 | **+0.5 ~ +4.5/秒** | 我方其他英雄 |
| 5 | 攻击回蓝 `AttackMpGain` | 独立机制 | 概率 × 技能蓝量比例（见第五节） | 自身 |
| 6 | 刘备·仁德（法力分享） | 技能机制 | 把自身累积 MP × 80%~160% 分给全队（见 3.4） | 我方全体 |

> 1~4 都是直接改棋子 `mpRegen` 属性、按加法累加；第 5、6 项直接给技能补 MP，不属于 mpRegen。

---

## 三、各来源细节

### 3.1 基础：职业基准 + 英雄修正

- `JobConfig_s.cs` 的 `MpRegen` 列：18 个职业全部 = **10/秒**（同级 `HpRegen` 全部 = 2/秒）。
- `HeroConfig_s.cs` 的 `MpRegen` 列是相对职业基准的 ±% 修正（0 = 不改），由 `ConfigManager.PostModify` 写回：
  `mpRegen = round(职业基准 × (100 + 修正%) / 100)`，不乘品质/升星系数。
- 当前 110 个英雄该列全部为 0 → 英雄基础 mpRegen 恒为 **10/秒**。

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

### 3.3 职业羁绊（LinkTeam，`mpRegen+X`）

上阵同职业 1/2/3/4/5 人 → Lv1~Lv5；羁绊的 `LinkTeam` 效果施加给我方除该职业英雄外的全体英雄（`JobLinkManager`），连接英雄自身不吃 LinkTeam。

| 职业 | Sname | 技能 Id | Lv1~Lv5 mpRegen |
|---|---|---|---|
| 智士(401) | 扇 | 2000021 ~ 2000025 | **+0.5 / +1 / +2 / +3 / +4.5** 每秒 |

> 扇的连接英雄自身获得“负面 buff 时长 +30%~200%”，不含 mpRegen；`Strength2[mpRegen]` 与 `LinkTeam` 两处数值需保持一致。相（丞相/402）现在只提供 `LinkSelf` 士兵攻击/生命（20%~120%），不再提供 mpRegen。

### 3.4 刘备·仁德（法力分享，`SkillAidManaShare`）

技能 Id 2020001 ~ 2020005，`MpCost=100`、`CD=3.5s`。发动时清空本技能累积法力，按下列公式分给我方全体英雄（含自身）：

```text
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

> 每名友军会把“每人”份额加到其所有未满的蓝耗技能上（逐技能充能），因此队伍技能越多，实际总灌注量越大。刘备自身也要先靠 mpRegen 攒满 100 点蓝才能发动。“仁德”好友组技能缩写是 `仁`（`SkillInitAddItem`，开局概率获得万民书），与刘备个人技能“刘备”（仁德）不是同一个技能。

---

## 四、叠加示例（单英雄）

| 组成 | 数值 |
|---|---|
| 职业基准 | 10.0/秒 |
| 3 件装备满配（玉玺 ×3） | +12/秒（需 6 个葫芦，实际受经济/掉落限制） |
| 羁绊（队伍上阵 2 智士，非智士英雄） | +0.5/秒 |
| **常规对局典型值** | **约 10 ~ 22/秒** |
| 极端堆叠理论上限 | 22.5/秒（不含仁德一次性灌注） |

例：100 MP 的技能，基础 10 MP/秒时从 0 到满约需 **10 秒**；mpRegen 为 10~22 MP/秒时约需 **10~4.5 秒**。实际放技能还会受控制状态、攻击回蓝、初始蓝量等机制影响。

---

## 五、攻击回蓝（独立机制，非 mpRegen）

`SkillAttackMpGain`：普攻命中时按 `Rate` 概率，回复“该英雄首个 `MpCost > 0` 技能”的 `Strength2[mpGain] × MpCost` 蓝量。

| 技能 | Sname | 技能 Id | 触发概率 | 回复蓝量 |
|---|---|---|---|---|
| 儒雅 | 儒 | 2010151 ~ 2010155 | 15% / 25% / 35% / 45% / 60% | 100% |
| 回气 | 回气 | 2090005 | 50% | 50% |
| 浩然 | 浩然 | 2090016 | 70% | 50% |
| 凝气 | 凝气 | 2090025 | 65% | 50% |

> 来源：儒雅为好友连锁连接技能；回气/浩然/凝气分别来自装备 玄光法袍(400036) / 易经(400011) / 玉龙壁(400023)。该类效果仅适用于拥有对应技能/装备的单位，并非全员每次攻击都回蓝。

---

## 六、与 `realeternia/TOMClassic` 的对比

### 6.1 先说明比较对象

用户提供的 [TOMClassic 仓库](https://github.com/realeternia/TOMClassic) 是一个较早期的卡牌/塔防战斗项目（仓库描述为 “Card game protomodel”，默认分支 `master`），**不是金铲铲/云顶之弈的代码仓库**。因此不能从该项目推断金铲铲“每次普攻回多少蓝”等规则；此前把金铲铲概括为“普攻基础回 10 蓝、受伤回蓝”并无该仓库代码依据，不应作为本项目的对比结论。

### 6.2 TOMClassic 仓库里能确认的法力机制

- TOMClassic 的 `Player` 有玩家级 `Mp`（与 `Lp`、`Pp` 并列），战斗初始值设为 3；它是支付卡牌消耗的资源，不是每个战斗单位各自的技能法力条。
- `Player.Update` 累积 `recoverTime`，达到配置间隔后从 `EnergyGenerator.NextAimMana` 取得 1 点对应资源；`EnergyGenerator` 按职业配置的 `RateMp` 等比例生��� MP/LP/PP 队列。也就是说，仓库呈现的是**玩家资源随时间周期性生成**，不是“每次单位普攻固定增加 MP”的规则。
- TOMClassic 也存在击杀、技能、效果等其他资源变动入口；上面只描述已核实的核心自然生成流程，不代表所有特殊效果。
- 它与本项目虽都包含时间驱动的法力增长，但资源归属和消费对象不同：TOMClassic 是玩家公共资源，用于卡牌；SanTeam 是棋子属性驱动、按技能分别充能，`mpRegen` 每秒施加到每个有蓝耗技能。

### 6.3 充能速率与伤害口径

- **SanTeam：**职业基础 `10 MP/秒`；100 MP 技能基础充满时间约 10 秒。常规叠加约 10~22 MP/秒，对应约 10~4.5 秒；攻击回蓝、法力分享等另计。
- **TOMClassic：**自然恢复是按 `DrawManaTime`/`DrawManaTimeFast` 等间隔发放 1 点队列资源，并受到 `GainEpRate`、回合阶段和 MP 生成权重影响。两边 MP 的单位、上限和消费主体不一致，因此不能把数值直接换算成“谁每秒回蓝更多”；若要定量比较，需要先选定 TOMClassic 的战斗速度、职业能量配置与资源上限。
- **技能伤害：**TOMClassic 的 `LiveMonster.OnSpellDamage` 接收技能/法术传入的基础伤害，再按 `FormulaBook.GetMagDefRate(RealMag)` 结算魔抗；这里没有证据表明其技能伤害按 SanTeam 的英雄品质、`MpCost/100`、`MagicRate` 公式缩放。因此不做虚假的固定伤害比例对照。SanTeam 英雄专属技能的修正及伤害公式见第七节。

代码参考：
- TOMClassic [`Player.Update`（法力周期恢复）](https://github.com/realeternia/TOMClassic/blob/6805a4ab16e949304a08946749270f5ecdc75894/TaleofMonsters2/Controler/Battle/Data/Players/Player.cs#L168-L179)
- TOMClassic [`EnergyGenerator`（MP/LP/PP 队列及权重）](https://github.com/realeternia/TOMClassic/blob/6805a4ab16e949304a08946749270f5ecdc75894/TaleofMonsters2/Controler/Battle/Data/Players/Frag/EnergyGenerator.cs#L21-L68)
- TOMClassic [`LiveMonster.OnSpellDamage`（魔抗结算）](https://github.com/realeternia/TOMClassic/blob/6805a4ab16e949304a08946749270f5ecdc75894/TaleofMonsters2/Controler/Battle/Data/MemMonster/LiveMonster.cs#L449-L465)
- SanTeam [`Chess.OnSecond`（每秒充能）](../../Assets/Resources/Scripts/Combat/Chess.cs)
- SanTeam [`Skill.AddRegenMp`（技能蓝量上下限）](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs)

---

## 七、技能伤害公式与代码索引

### 7.1 英雄专属技能伤害修正

英雄专属技能（`HeroConfig.Skill1` 指向的技能缩写）的 `DamageStrength` / `HealStrength` 在 `ConfigManager.PostModify` 中统一修正：

```text
修正后基值 = 配置基值 × 品质系数 × Clamp(MpCost / 100, 0.2, 2.0) × MagicRate
```

当前技能品质系数 Q1~Q4 为 **1.0 / 1.6 / 2.2 / 3.0**；`MagicRate` 法系常见为 1.5、其他为 1.0。职业技能、好友连锁技能、道具技能不参与这项英雄专属修正。该式是基值修正，不等于最终伤害占目标生命的百分比。

按技能类别，SanTeam 当前伤害口径可概括为：法术伤害 `DamageStrength × (100 + AP) / 100`，再受魔抗；物理伤害 `DamageStrength + ATK`，再受护甲；真实伤害 `DamageStrength + ATK`，不受抗性。范围/多目标技能还需结合目标数、伤害段数和技能配置比较总伤害。

### 7.2 代码索引

| 内容 | 位置 |
|---|---|
| 每秒充能结算 | `Chess.OnSecond()` → `Skill.AddRegenMp` |
| 技能法力条 | `Skill.cs`（`mp` / `MpCost` / `AddRegenMp` / `IsMpFull`） |
| 开局拉满开关 | `CombatConst.cs`（`FullMpAtBattleBegin`） |
| 职业基准值 | `JobConfig_s.cs`（`MpRegen`） |
| 英雄 ±% 修正 | `HeroConfig_s.cs`（`MpRegen`） + `ConfigManager.cs`（`PostModify`） |
| 装备叠加 | `Chess.cs`（装备循环 `mpRegen += equipAttr.MpRegen`） |
| 羁绊施加 | `JobLinkManager.cs`（`case "mpRegen"`） |
| 刘备·仁德法力分享 | `SkillAidManaShare.cs` |
| 攻击回蓝 | `SkillAttackMpGain.cs` |
| 英雄专属技能数值修正 | `ConfigManager.cs`（`PostModify`） |
| 技能伤害/治疗计算 | `Skill.cs`（`GetSkillDamage()` / `GetSkillHeal()`） |
