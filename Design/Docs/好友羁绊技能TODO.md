# 好友羁绊技能 TODO

> 状态：**待实施**
> 关联文件：`Configs/HeroFriendConfig_s.cs`（好友组）、`Configs/SkillConfig_s.cs`（技能行）、`Combat/FriendLineManager.cs`（技能授予）、`Combat/CombatConst.cs`（机制常量）
> 已完成：**G7 经世济民「济」**（Dumb 技能 2010091~2010095 + `PlayerInfo.RoundGold` 每回合金币结算），作为本批次的格式样板

---

## 一、机制约束（实施前必读）

### 1. 好友组配置

```csharp
HeroFriendConfig(Id, Name, Level, Heros, SkillId, LineColor)
// SkillId   关联技能缩写，空 = 只有默认连线属性（Sname="友"档位加成）
// LineColor 特殊连锁的拉线颜色（HTML 色值），不填用默认暗灰
```

### 2. 技能等级规则（`FriendLineManager.ApplyFriendSpecialSkills`）

- 等级 = `CombatConst.FriendSpecialBaseLevel`(0) + **本侧在场的"其他"成员数**
- **5 人组最高只能到 Lv4，只有 6 人组才能到 Lv5**（写数值时要把 Lv4 当实际上限）
- 在场仅 1 人时 `presentCount<=0` 直接跳过（不成团 → 无技能）
- 组内**每个成员各自获得一份技能**：
  - **自身类**效果（G6/G8/G18/G20/G21/G24）：各成员各拿一份，符合预期
  - **全队/全军类**效果（G11/G12）：会被每个成员各触发一次导致 N 倍叠加，需做"每侧只结算一次"保护（见 §四）
  - **目标为敌方/全队的 Buff 类**（G16/G32）：同 id Buff 重复施加只 `Refresh` 时间、不叠加数值，天然幂等

### 3. 技能行（SkillConfig）

- 同 Sname 展开 Lv1~Lv5；**Descript 只在 Lv1 填模板**（`/1 /2` 为参数占位符），各级 `DescriptVal` 填 `;` 分隔参数
- `Type` 填 `"连接"`；`Icon` 填 `Textures/SkillPic/` 下的文件名（不带扩展名）
- 数值字段含义（本批次常用）：
  | 字段 | 用途 |
  |---|---|
  | `Strength` | 通用强度值（Buff 类效果取值来源，如增伤比例） |
  | `SkillDamageRate` | 百分比系数（护盾=最大生命×该值） |
  | `BuffTime` | Buff 持续秒数 |
  | `BuffId` | `BuffConfig.NameS` 短名（空串=无 Buff） |
  | `Rate` | 触发概率 / 频率类 |
  | `LinkSelf` / `LinkTeam` | 属性加成串，格式 `attr+value,attr+value`（见 §四属性名表） |
  | `ItemId` | 发放道具 Id（`InitAddItem`） |
  | `Range` / `CD` / `MpCost` | 范围 / 冷却 / 蓝耗 |

### 4. 新增脚本的注册

- `SkillManager.CreateSkill` 的 `switch` 必须加 case（未注册会 `throw`）
- 新增 `.cs` 必须加进 `Assembly-CSharp.csproj` 的 `<Compile Include>`
- 机制数值/档位数组放 `CombatConst`，不硬编码在脚本里

### 5. 显示链路（配好配置即自动生效，无需额外开发）

- 羁绊提示 `TooltipFriend` / `TooltipHero`：`TooltipHero.GetFriendShowSkill(friendCfg, presentCount, out level)` → Sname → `ConfigManager.GetSkillDescript(cfg, withNext:true)`（当前档位 + 下一档差值淡绿）
- 图标：`MySelectControl.GetSkillIcon(sname)` → `cfg.Icon`，**每个新缩写都需要一张 `Textures/SkillPic/<sname>.png`**（美术资源；`.meta` 由 Unity 自动生成，禁止手写）

---

## 二、待办总览

| 组 | 缩写 | 效果 | 实现方式 | 新脚本 | 档位上限 |
|---|---|---|---|---|---|
| G7 经世济民 | 济 | 每回合额外金币 | ✅ 已完成（Dumb） | — | Lv4 |
| G6 身负异禀 | 异 | 开局随机获得 攻/法强/护甲/魔抗 之一强化 | 复用 `SkillInitAttrChange`（加随机分组） | 小改 | **Lv5** |
| G8 老当益壮 | 壮 | 最大生命 +X、每秒回血 +X | 复用 `SkillInitAttrChange` | 无 | Lv4 |
| G11 治军严明 | 令 | 全军士兵 攻 +X%、生命 +X% | 新脚本（复用士兵系数机制） | ✅ | **Lv5** |
| G12 王佐之才 | 佐 | 我方其他英雄 攻击/法强 +X（辅佐光环） | 新脚本（遍历友方 `ApplyAttr`） | ✅ | **Lv5** |
| G16 权奸当道 | 奸 | 开局对随机 1 名敌方英雄施加增伤（10~20s，+30%~60%），每个有 link 的 hero 各自触发 | ✅ 已完成（通用技能 `SkillInitEnemyRandomBuff`） | — | Lv4 |
| G17 名门望族 | 贵 | 开局获得装备道具（玉如意/虎王重甲等） | 复用 `SkillInitAddItem` | 无（可能小改道具池） | **Lv5** |
| G18 出将入相 | 仕※ | 自身 攻击 +X、法强 +X | 复用 `SkillInitAttrChange` | 无 | Lv4 |
| G20 身负奇才 | 奇 | 技能触发概率提升 / 技能 CD 缩短 | 复用 `SkillModifySkillRateTime` | 无 | **Lv5** |
| G21 温良恭俭 | 和 | 自身治疗量 + 受治疗量提升 | 复用 `SkillInitAttrChange` | 无 | **Lv5** |
| G24 文采出众 | 文 | 每秒法力回复 +X | 复用 `SkillInitAttrChange` | 无 | Lv4 |
| G25 儒将风范 | 儒 | 自身周围友军获得属性加成 | 复用 `SkillHelpAidBuff` + 新增属性增益 Buff | ✅ | Lv4 |
| G31 乱世枭雄 | 枭 | 击杀敌方英雄后永久叠加自身攻击 | 新脚本 + 新增击杀事件钩子 | ✅ | Lv4 |
| G32 济世安民 | 安 | 开局给全队英雄护盾 | 新脚本（参照 `SkillFactionShield.cs`） | ✅ | **Lv5** |

※ G18 原缩写「相」**已被职业羁绊「运筹」占用**（`SkillConfig 2000061~2000065`），必须换缩写，本表暂用 **「仕」**（可替换为「出」「将」等）

---

## 三、逐组方案

### G6 身负异禀「异」（6 人，可到 Lv5）

- 成员：孙权·王 100003、左慈·医 110011、张角·工 110007、张松·扇 101014、马良·相 101019、邓艾·盾 105001
- 效果：开局**随机**获得 攻 / 法强 / 护甲 / 魔抗 之一强化
- 复用：`SkillInitAttrChange`，需小改支持"随机取一组"
- 小改方案（推荐）：`LinkSelf` 用 `?` 分隔多个候选组，脚本检测到 `?` 就随机挑一组解析
  ```
  LinkSelf = "atk+10?ap+10?armor+10?magicRes+10"
  ```
  `ParseBonuses` 保持原样，只在 `SkillInitAttrChange.BattleBegin` 前做一次"随机选组"拆分（`SysRandom.Range`）
- 建议数值（四属性共用同一套，可调）：`+10 / +18 / +28 / +40 / +55`
- 备注：随机结果建议在 `GameLog.Debug` 里打出来便于排查

### G8 老当益壮「壮」（5 人，最高 Lv4）

- 成员：黄忠·弓 101008、严颜·盾 101022、黄盖·士 103005、程普·车 103022、许褚·锤 102005
- 效果：最大生命 +X、每秒回血 +X
- 复用：`SkillInitAttrChange`，`LinkSelf = "maxHp+X,hpRegen+Y"`
- ⚠️ **属性名是 `maxHp` 不是 `hp`**（`JobLinkManager.ApplyAttr` 只认 `maxHp`）
- 建议数值：
  | Lv | 1 | 2 | 3 | 4 | 5 |
  |---|---|---|---|---|---|
  | maxHp | +80 | +150 | +240 | +360 | +500 |
  | hpRegen | +1 | +2 | +3 | +5 | +7 |
- 备注：`maxHp` 加成后 `ApplyAttr` 内部会同步 `heroInfo.SetHpRate`，无需额外处理

### G11 治军严明「令」（6 人，可到 Lv5）— 需新脚本

- 成员：马超·马 101003、高顺·炮 104006、程昱·扇 102017、李严·士 101013、曹仁·盾 102012、高览·弓 106006
- 效果：全军士兵 攻 +X%、生命 +X%（治军 → 练兵）
- 新脚本：`SkillInitSoldierBuff.cs`（`ScriptName = "InitSoldierBuff"`）
  1. `BattleBegin` 遍历 `WorldManager.Instance.GetUnitsMySide(owner.side)` 中 `!isHero` 的单位，`JobLinkManager.ApplyAttr(unit, "soldierAtk"/"soldierHp", value)`
  2. ⚠️ **必须自行结算**：`JobLinkManager.ApplyJobLinks()` 的士兵结算在 `SkillManager.BattleBegin` **之前**执行完并已把 `soldierAtkRate` 复位成 1，之后再累加系数不会生效。参照 `JobLinkManager.ApplyJobLinks` 末尾写法补一次结算：
     ```csharp
     unit.maxHp = unit.hp = (int)(unit.soldierBaseMaxHp * unit.soldierHpRate);
     unit.atk = (int)(unit.atk * unit.soldierAtkRate);
     unit.soldierAtkRate = 1f;
     ```
  3. ⚠️ **每侧只结算一次**（否则组内 N 个成员各触发一次 → N 倍，且 `atk` 会按乘算复利）；保护做法见 §四
- 建议数值（Lv1~5）：`+8% / +14% / +20% / +28% / +38%`
- 说明：`soldierHp` 走"初始基准快照 × 系数"，重复结算不会把已加成值当基数二次乘算
- 也可考虑把结算搬到 `FriendLineManager`（它按 side 遍历，天然只跑一次），代价是不再"技能驱动"，实施时二选一

### G12 王佐之才「佐」（6 人，可到 Lv5）— 需新脚本

- 成员：姜维·车 101010、诸葛瑾·鼓 103013、荀攸·棋 102010、荀彧·相 102003、徐庶·炮 101006、杜预·工 105007
- 效果：我方**其他**英雄 攻击 / 法强 +X（辅佐光环）
- 新脚本：`SkillInitTeamAttr.cs`（`ScriptName = "InitTeamAttr"`）
  - `BattleBegin` 遍历 `GetUnitsMySide(owner.side)`，跳过自己与非英雄，`JobLinkManager.ApplyAttr(unit, "atk"/"ap", value)`
  - 数值直接配在 `LinkSelf`（如 `"atk+8,ap+8"`），脚本按 `ParseBonuses` 透传，无需为每种属性写代码
  - ⚠️ 同样存在**每侧只结算一次**问题（组内 N 个成员 → 每人给其他人加一次）
- 建议数值：每级 攻击 +X、法强 +X，`+8 / +14 / +22 / +32 / +45`

### G16 权奸当道「奸」（5 人，最高 Lv4）— ✅ 已实现

- 成员：董卓·王 100004、李儒·扇 104007、郭图·棋 106008、鞠义·弩 106004、司马师·鼓 105002
- **效果（本次改版，覆盖旧的"开局削弱敌方全体攻/护甲"方案）**：
  战斗开始时，对**随机 1 名敌方 hero** 添加**增伤**效果，持续 **10~20s**，增伤比例 **30%~60%**；**每个有 link 的 hero 各自触发一次**
- 落地内容：
  - 新脚本 `Combat/Skill/SkillInitEnemyRandomBuff.cs`（`ScriptName = "InitEnemyRandomBuff"`，**通用技能**：开局对随机 1 名敌方 hero 施加 `BuffId` 指定的 Buff，强度取 `Strength`、时长取 `BuffTime`，已注册 `SkillManager.CreateSkill` + `Assembly-CSharp.csproj`）
  - 技能行 `SkillConfig 2010096~2010100`（`Name = 弄权跋扈`、`Sname = 奸`、`Type = 连接`、`BuffId = "伤"`、`NegBuff = false`）
  - `HeroFriendConfig[16]` → `SkillId = "奸"`、`LineColor = "#660033"`
  - 随机选敌照抄 `SkillInitSneakChangePos.BattleBegin`（`GetUnitsInRange(pos, 0, side, true)` 过滤 `isHero && hp > 0`，`SysRandom.Range` 取一名）
  - Buff 走 `BuffConfig 301003「伤」/BuffDamagedAddRate`，"受到的伤害按比例增加"
  - ⚠️ 增伤比例配在技能行 **`Strength`**（不是 `SkillDamageRate`）：`BuffDamagedAddRate.DuringAttacked` 用的是 `skillCfg.Strength`（`skillCfg` = 施加 Buff 时传入的 `skillId` 那一行）
- 数值：
  | Lv | 1 | 2 | 3 | 4 | 5(预留) |
  |---|---|---|---|---|---|
  | Strength（增伤） | 0.30 | 0.40 | 0.50 | 0.60 | 0.70 |
  | BuffTime（秒） | 10 | 12 | 15 | 18 | 20 |
- 遗留：
  - 同组 5 人上阵 → 最多触发 4 次，可能重复命中同一目标；`Chess.AddBuff` 同 id 只 `Refresh` 时间，**增伤值不叠加**
  - 若希望"每个敌方英雄每场只被选中一次"，可参照 `SkillInitSneakChangePos` 的 `sneakSwapped` 思路加标记 —— **待确认是否需要**

### G17 名门望族「贵」（6 人，可到 Lv5）

- 成员：孙坚·枪 103002、袁绍·王 100006、甄宓·琴 102025、法正·棋 101020、孙策·戟 103001、夏侯惇·车 102002
- 效果：开局获得装备道具（玉如意 / 虎王重甲等）
- 复用：`SkillInitAddItem`（`ScriptName = "InitAddItem"`），逻辑为"`Rate` 概率发放 `ItemId` 道具"
- **与「仁」区分道具池**：「仁」发 401012 万民书；「贵」用 409xxx 装备池：
  | ItemId | 名称 | 效果 |
  |---|---|---|
  | 409005 | 酒 | 英雄 `atk+10,ap+6` |
  | 409003 | 虎王重甲 | 玩家 `shp+40` |
  | 409004 | 玉如意 | 出售卡牌多获得 25% 金币 |
- 方案 A（推荐，不改脚本）：**每级行配不同 ItemId**，等级越高道具越好（Lv1/Lv2 = 酒，Lv3/Lv4 = 虎王重甲，Lv5 = 玉如意），`Rate = 1`（必得）
- 方案 B：想要"随机道具池"则需小改 `SkillInitAddItem`（`ItemId` 单值 → 池 + `SysRandom` 挑选），并在 `SkillConfig` 增设配置方式 —— **待确认**
- 备注：`SkillInitAddItem` 是**战斗开始时**判定；发放到玩家背包（`player.AddItemCard`），非战斗内即时生效

### G18 出将入相「仕」（5 人，最高 Lv4）

- 成员：司马懿·扇 102016、钟会·棋 105005、鲁肃·鼓 103007、陆逊·炮 103011、贾诩·相 104003
- 效果：自身 攻击 +X、法强 +X
- 复用：`SkillInitAttrChange`，`LinkSelf = "atk+X,ap+X"`
- ⚠️ 缩写「相」与职业羁绊「运筹」（2000061~2000065）**冲突**，必须换（暂定「仕」）
- 建议数值：攻击 / 法强 各 `+10 / +18 / +28 / +40 / +55`

### G20 身负奇才「奇」（6 人，可到 Lv5）

- 成员：曹操·王 100002、刘晔·工 102022、周瑜·扇 103008、吕蒙·盾 103010、左慈·医 110011（※左慈同时属于 G6）
- 效果：技能触发概率提升 / 技能 CD 缩短（放技能更频）
- 复用：`SkillModifySkillRateTime`（`ScriptName = "ModifySkillRateTime"`），它有三个钩子：
  - `OnCheckBurst`：`rate += Math.Min(rate, checkSkillCfg.Rate)` → **触发率最多翻倍**，由本技能行 `Rate` 控制（填 1 = 必定翻倍）
  - `OnCheckCD`：`cdTime = Math.Max(1, cdTime * Strength)` → 本技能行 `Strength` 是 **CD 倍率（<1 才缩短）**
  - `OnAddBuff`：延长负面 Buff 时长，由 `BuffTime` 控制（本组不需要，填 0）
  - 生效范围受 `CheckType` 限制（0 不限 / 1 仅物理 / 2 仅法术），本组建议 `CheckType = 0` 对全部技能生效
- 建议数值：
  | Lv | 1 | 2 | 3 | 4 | 5 |
  |---|---|---|---|---|---|
  | Rate | 1 | 1 | 1 | 1 | 1 |
  | Strength | 0.90 | 0.80 | 0.70 | 0.60 | 0.50 |

### G21 温良恭俭「和」（6 人，可到 Lv5）

- 成员：荀彧·相 102003、蒋钦·戟 103009、小乔·琴 103017、孙乾·鼓 101016、于吉·医 110006、夏侯惇·车 102002
- 效果：自身治疗量 + 受治疗量提升
- 复用：`SkillInitAttrChange`，`LinkSelf = "healRate+X,healedRate+X"`
- 建议数值（小数，0.1 = +10%）：`+0.10 / +0.18 / +0.26 / +0.36 / +0.48`

### G24 文采出众「文」（5 人，最高 Lv4）

- 成员：曹操·王 100002、诸葛亮·工 101004、蔡文姬·琴 102024、法正·棋 101020、诸葛瑾·鼓 103013
- 效果：每秒法力回复 `mpRegen` +X（文士技能循环更快）
- 复用：`SkillInitAttrChange`，`LinkSelf = "mpRegen+X"`（参考琴·战鼓 `2000021` 的职业行写法）
- 建议数值：`+1 / +2 / +3 / +4 / +5`

### G25 儒将风范「儒」（5 人，最高 Lv4）— 需新增 Buff

- 成员：陆逊·炮 103011、姜维·车 101010、李典·士 102021、司马昭·扇 105003、马谡·枪 101018
- 效果：自身周围友军获得属性加成（带兵）
- 复用：`SkillHelpAidBuff.cs`（`ScriptName = "HelpAidBuff"`）：按 `Range` 找范围内友军 → `CheckBurst` 判定 → 排序（英雄优先、属性总量高优先）→ 给 1 名友军挂 `BuffId` 指定的 Buff
- ⚠️ **现有 BuffConfig 没有"属性增益"类 Buff**（现只有 护盾/减伤盾/吸血/伤害提升/攻速提升/增伤/减速/陷阵/溃败/混乱/连锁），需新增：
  - `BuffConfig_s.cs` 新增一行，如 `NameS = "勇"`，`ScriptName = "BuffAttrChange"`，`IsPositive = true`
  - 新增 `Buff/BuffAttrChange.cs`：`OnAdd` 加属性、`OnRemove` 对称减回去（数值取 `skillCfg.LinkSelf` / `Strength`，属性种类可约定用 `LinkSelf` 串表达，与 `JobLinkManager.ParseBonuses` 保持一致）
  - `BuffManager.AddBuff` 的 `switch` 加 `case "BuffAttrChange"`；新 `.cs` 注册进 `Assembly-CSharp.csproj`
- 备选（最省事）：直接复用 `300004「伤害提升」`（`BuffDamageAddRate`，`Strength` 为伤害加成比例）——语义变成"周围友军伤害 +X%"，与"属性加成"略有差异，**待确认取哪种**
- 建议数值（若走属性增益）：`atk/ap +8 / +14 / +20 / +28 / +38`，`Range` 参考 `SkillHelpAidBuff` 现配（80），`CD` 6~8s，`BuffTime` 5~10s

### G31 乱世枭雄「枭」（5 人，最高 Lv4）— 需新脚本 + 击杀事件

- 成员：董卓·王 100004、张角·工 110007、孟获·锤 101024、孙坚·枪 103002、马腾·弩 110005（※董卓同时属于 G16）
- 效果：击杀敌方英雄后**永久叠加**自身攻击（掠夺）；可选附带金币
- 现状：**没有"击杀事件"钩子**。`Chess.Ondying()` 只调 `SkillManager.OnDeath(this)`（阵亡者自己的技能），而 `lastDamagedPlayerId` 只记录了击杀者的**玩家**，拿不到击杀者 Chess 实例
- 推荐实现（最小改动）：
  1. `Chess` 增补 `public Chess lastDamagedChess;`，在 `OnDamaged`（[Chess.cs#L604-614](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/Chess.cs#L604-L614)）与 `OnSkillDamaged`（[Chess.cs#L643-645](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/Chess.cs#L643-L645)）里与 `lastDamagedPlayerId` 一并赋值
  2. `Chess.Ondying()` 里调 `SkillManager.OnKill(lastDamagedChess, this)`
  3. `SkillManager` 新增 `OnKill(Chess killer, Chess victim)`：遍历 killer 的 skills 调 `skill.OnKill(victim)`
  4. `Skill` 基类新增 `public virtual void OnKill(Chess victim) { }`
- 新脚本：`SkillKillAddAttr.cs`（`ScriptName = "KillAddAttr"`）：`OnKill` 里判断 `victim.isHero` → `JobLinkManager.ApplyAttr(owner, "atk", value)`，并维护叠加层数
- 建议数值（每层，Lv1~5 可到 Lv4）：`+6 / +10 / +15 / +22 / +30`；**建议加层数上限（如 5 层）**防止滚雪球 —— 上限值放 `CombatConst`
- 可选金币：若要"掠夺附带金币"，需走 `owner.GetPlayerInfo().AddGold(...)`（战斗内即时加钱，与 G7 的回合结算链路不同）；**建议先不做，待确认**

### G32 济世安民「安」（6 人，可到 Lv5）— 需新脚本

- 成员：沮授·相 106007、刘备·王 100001、田丰·鼓 106003、荀攸·棋 102010、韩当·马 103023、孙尚香·弩 103014
- 效果：开局给全队英雄护盾
- 新脚本：`SkillInitTeamShield.cs`（`ScriptName = "InitTeamShield"`），**直接参照 `SkillFactionShield.cs`**：
  1. 遍历 `WorldManager.Instance.GetUnitsMySide(owner.side)` 的 `isHero && hp > 0` 单位
  2. `BuffManager.AddBuff(unit, owner, id, buffCfg.Id, skillCfg.BuffTime)`（`BuffId` 用 `"盾"` = 300001 吸收护盾；若要"减伤盾"语义则用 `"硬"` = 300002 `BuffShieldValue`，注意它是**固定值减免**不是百分比）
  3. 取 `unit.GetBuff(buffCfg.Id) as BuffShield` → `SetHp((int)(unit.maxHp * skillCfg.SkillDamageRate))`
- 建议数值（`SkillDamageRate`，护盾=最大生命×该值）：
  | Lv | 1 | 2 | 3 | 4 | 5 |
  |---|---|---|---|---|---|
  | 比例 | 15% | 20% | 26% | 33% | 40% |
  | BuffTime | 999（整场） |
- 备注：组内多成员各自触发时，同 id Buff 只 `Refresh` 时间、`SetHp` 为**覆盖**（不累加），同侧同组成员等级相同 → 数值一致，天然幂等，**无需**"每侧一次"保护

---

## 四、通用约定与坑（实施时统一遵守）

1. **`JobLinkManager.ApplyAttr` 可用属性名**（`LinkSelf`/`LinkTeam` 串里只能用这些）：
   `atk`(含 `might` 兼容)、`ap`、`armor`、`magicRes`、`maxHp`（**不是 `hp`**）、`critRate`、`critDamageMulti`、`attackSpeedRate`、`dodgeRate`、`mpRegen`、`hpRegen`、`healRate`、`healedRate`、`auroEffectRate`、`soldierAtk`、`soldierHp`、`range`
   - `soldierAtk` / `soldierHp` 只对 `!isHero` 生效，且是**乘法系数**（0.1 = +10%）
   - `range` 只对远程（射程 > 20）生效
2. **士兵系数必须在技能里自行结算**：`JobLinkManager.ApplyJobLinks()` 的士兵结算发生在 `SkillManager.BattleBegin` **之前**（见 [WorldManager.cs#L121-134](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/WorldManager.cs#L121-L134)），之后再加系数不会生效 —— 影响 **G11**
3. **"每侧只结算一次"保护**（影响 **G11 / G12**）：组内每个成员各自持有一份技能，全队类效果会 N 倍叠加。推荐用静态 `HashSet<int>` 按 `side` 记录已结算侧，并在每次战斗开始时清空（例如在 `WorldManager` 战斗初始化处加一行 `Reset()`）；或把全队类结算搬到 `FriendLineManager`（天然按 side 只跑一次）
4. **Buff 数值来源是技能行的 `Strength`**，不是 `SkillDamageRate`（见 `BuffDamagedAddRate` / `BuffDamageAddRate` / `BuffShieldValue`）—— 影响 **G16**
5. **同 id Buff 重复施加只会 `Refresh` 时间**（`Chess.AddBuff` 命中已有 id 直接 return），数值不叠加、不会换 caster/配置行 —— 影响 **G16 / G32**
6. **5 人组只能到 Lv4**：Lv5 行为将来扩组预留，也必须填（否则 `SetLevel` 取到 null 会沿用旧行）
7. **图标**：每个新缩写都要有 `Textures/SkillPic/<sname>.png`，否则羁绊提示/选牌节点的图标为空；只落源文件，**不要手写 `.meta`**
8. **跨组重复英雄**（董卓 ∈ G16+G31、左慈 ∈ G6+G20、曹操 ∈ G20+G24、荀彧 ∈ G12+G21、孙坚 ∈ G17+G31、陆逊 ∈ G18+G25、姜维 ∈ G12+G25、孙尚香 ∈ G4+G32 …）：同一英雄可能同时挂多个羁绊技能，数值叠加效果需整体看一眼是否过强
9. **同时还要改 `HeroFriendConfig_s.cs`**：把对应组的 `SkillId` 填成新缩写、`LineColor` 填连线色（否则连线用默认暗灰）
10. 数值一律配在 `SkillConfig` / `CombatConst`，**不硬编码进脚本**；日志用 `GameLog`、随机用 `SysRandom`

### 建议连线色（`LineColor`，HTML 色值，可调整）

| 组 | 缩写 | 建议色 |
|---|---|---|
| G6 身负异禀 | 异 | `#AA44FF` |
| G8 老当益壮 | 壮 | `#CC7A00` |
| G11 治军严明 | 令 | `#CC0000` |
| G12 王佐之才 | 佐 | `#3366CC` |
| G16 权奸当道 | 奸 | `#660033` |
| G17 名门望族 | 贵 | `#AA8844` |
| G18 出将入相 | 仕 | `#009999` |
| G20 身负奇才 | 奇 | `#6633CC` |
| G21 温良恭俭 | 和 | `#66AA88` |
| G24 文采出众 | 文 | `#0088AA` |
| G25 儒将风范 | 儒 | `#557755` |
| G31 乱世枭雄 | 枭 | `#883300` |
| G32 济世安民 | 安 | `#33AA55` |

---

## 五、实施清单（每个组都按这个顺序）

- [ ] 1. `SkillConfig_s.cs`：新增该 Sname 的 Lv1~Lv5 五行（`Type = "连接"`、`Descript` 仅 Lv1 填模板、各级 `DescriptVal` 填参数、`Icon` 填缩写名）
- [ ] 2. `HeroFriendConfig_s.cs`：对应组填 `SkillId` + `LineColor`
- [ ] 3. 需要新脚本的组：写 `SkillXxx.cs`（或 `BuffXxx.cs`）→ 注册 `SkillManager.CreateSkill` / `BuffManager.AddBuff` → 加入 `Assembly-CSharp.csproj`
- [ ] 4. 需要常量的组：把机制数值/档位放 `CombatConst`
- [ ] 5. 新增图标 `Textures/SkillPic/<sname>.png`（美术）
- [ ] 6. MSBuild 编译通过（`Assembly-CSharp.csproj`，`EXITCODE=0`）
- [ ] 7. Unity 里验证：羁绊提示（当前档 + 下一档淡绿差值）、选牌/背包连线色与图标、战斗内实际效果

### 建议实施顺序（先易后难）

1. **零新增脚本批**（改配置即可）：G8 / G17 / G18 / G21 / G24 → 再 G20（复用 `ModifySkillRateTime`）
2. **小改批**：G6（`InitAttrChange` 加随机组）
3. **新脚本批**：G16（改版效果）→ G32（照抄 `FactionShield`）→ G12 → G11（含士兵结算坑）→ G25（含新 Buff）→ G31（含击杀事件钩子，改动面最大，放最后）

---

## 六、待确认问题

1. G18 缩写「相」冲突，改用「仕」是否可以？（或其他字）
2. G17 道具池：逐级换道具（方案 A，不改脚本）还是随机池（方案 B，需小改 `SkillInitAddItem`）？
3. G25 周围友军加成：新增"属性增益 Buff"（更贴合"属性加成"）还是直接复用「伤害提升」Buff（更省事）？
4. G16 是否需要"每个敌方英雄每场只被选中一次"限制（当前设计允许同一目标被重复命中，只刷时间）？
5. G31 是否要"击杀附带金币"？是否需要叠加层数上限（建议 5 层）？
6. G11/G12 的"每侧只结算一次"实现方式：技能内静态 guard，还是把全队类结算搬到 `FriendLineManager`？
