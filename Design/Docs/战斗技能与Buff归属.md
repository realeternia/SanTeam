﻿﻿# 战斗技能脚本归属与 Buff 引用统计

> 整理日期：2026-11-22（脚本命名规范整理后按当前 `SkillConfig` / `HeroConfig` / `BuffConfig` 重新统计）
> 2026-11-22 变更：袁绍→`SkillComDamageArea`（中心双倍）、黄月英/裴秀→`SkillComBuffArea`、于吉/张春华→`SkillComHealSingle`（治疗诅咒）、貂蝉→`SkillComHealSingle`（链接双方）、荀攸→`SkillComDamageSingle`（多段）；删除 `SkillAidExplode` / `SkillAidMultiHit` / `SkillAidCraftBuff` / `SkillAidCurseHeal` / `SkillAidLifeLink`；徐盛/潘璋/诸葛绪/马谡 归入共享 `SkillComInitAttr`（才干）。
> 2026-11-28 变更：刘备·仁德→`SkillComBuffArea`（3人加攻+立刻回蓝）、曹叡·压制→`SkillComDamageSingle`（单体伤害+法力转移）、司马炎·一统→`SkillComBuffArea`（近远分档组件）、陆抗·远略→`SkillComDamageSingle`（飞弹+距离增伤）、吕蒙·白衣袭/孙尚香·纵舞/典韦·天降恶来→`SkillComDamageSingle`/`SkillComDamageArea`（冲锋/后退/跳跃组件）；删除 `SkillAidManaTeamShare` / `SkillAidManaSingleBurn` / `SkillAidBuffAreaRangeSplit` / `SkillAidDamageSingleDashBuff` / `SkillAidDamageShotBackstep` / `SkillAidDamageAreaFall`。
> 范围：技能脚本命名规范、技能分类与脚本归属、英雄个人技能 → 脚本映射、Buff 归属规则与引用统计
> 数据源：[SkillConfig_s.cs](../../Assets/Resources/Scripts/Configs/SkillConfig_s.cs)、[HeroConfig_s.cs](../../Assets/Resources/Scripts/Configs/HeroConfig_s.cs)、[BuffConfig_s.cs](../../Assets/Resources/Scripts/Configs/BuffConfig_s.cs)

---

## 一、技能构成总览

| 类别 | 识别方式 | 技能数(Sname) | 配置行 | 使用脚本数 |
| --- | --- | --- | --- | --- |
| 英雄个人技能 | `Sname` = 英雄名，Type 非职业/连接/道具 | 155 | 775 | 29 |
| 职业/国家连锁 | Type=职业 | 19 | 95 | 6 |
| 好友连锁 | Type=连接 | 31 | 155 | 30 |
| 道具技能 | Type=道具 | 19 | 19 | 9 |
| **合计** | — | **224** | **1044** | **73** |

- 每个英雄技能按 `Sname` 展开为 Lv1~Lv5；道具技能多为单行 Lv1。
- 技能类型分布：光环 35 行、兵增 20 行、单伤 185 行、单增 45 行、单控 45 行、单疗 30 行、召唤 10 行、群伤 210 行、群增 55 行、群控 20 行、群疗 35 行、职业 95 行、自增 85 行、连接 155 行、道具 19 行。

### 技能脚本命名规范

`SkillManager.CreateSkill` 按 `SkillConfig.ScriptName` 反射创建 `(int skillId, Chess owner)` 构造的脚本；**ScriptName 必须与类名、文件名一致**。脚本按用途分五类前缀：

| 前缀 | 适用范围 | 判定方式 |
| --- | --- | --- |
| `SkillAidXxx` | 英雄专属技能 | 一个英雄（或少数英雄）一套实现，含专属机制/演出；重写 `Skill.CheckAidSkill` 或挂专属钩子，逻辑无法由配置完全驱动 |
| `SkillComXxx` | 通用模板技能 | 行为由 `SkillConfig` 字段 + `TargetType` 标记驱动，一份脚本服务多个英雄/多行配置（如 `SkillComDamageSingle` 服务 35 名英雄） |
| `SkillSoldierXxx` | 兵种/召唤物专属技能 | 只被 `SoldierConfig.Skill` 引用（如喷火兽的 `SkillSoldierHitWall`） |
| `SkillBondXxx` | 羁绊/连锁技能 | `SkillConfig.Type` 为 `职业` 或 `连接` 的行 |
| `SkillEquipXxx` | 道具技能 | `SkillConfig.Type` 为 `道具` 的行 |

补充约定：

- 同一行为同时被英雄与羁绊/道具使用时，拆成两个完整脚本：英雄侧保留 `SkillAidXxx` / `SkillComXxx` 实现，羁绊侧完整复制一份 `SkillBondXxx`，道具侧完整复制一份 `SkillEquipXxx`，不通过继承复用。
- 同一行为同时被羁绊与道具使用时，羁绊侧保留 `SkillBondXxx` 实现，道具侧完整复制一份 `SkillEquipXxx`。
- 新增脚本必须先判定归属再命名，不能出现 `SkillTaunt`、`SkillMultiHit` 这类无分类前缀的名字；配置行的 `ScriptName` 必须与目标脚本类名完全一致。
- 复制脚本必须完整保留原实现的业务逻辑，不得只写空壳或通过继承复用。`SkillDumb` 为特殊占位脚本，保持原名 `SkillDumb`，不加 `SkillBond` 前缀。

## 二、ScriptName 全量清单

| ScriptName | 行数 | 类型 | 技能示例 |
| --- | --- | --- | --- |
| SkillComBuffArea | 105 | 群增、单增 | 一统 |
| SkillComDamageArea | 115 | 群伤、群控 | 暴敛 |
| SkillComHealArea | 25 | 群疗 | 抚军 |
| SkillComHealSingle | 30 | 单疗 | 放权 |
| SkillComInitAttr | 20 | 自增 | 才干 |
| SkillAidHealSingleLowHp | 5 | 单疗 | 机巧疗 |
| SkillComHealAreaCasting | 5 | 群疗 | 胡笳 |
| SkillAidDamageCastingDrain | 5 | 单控 | 魔神 |
| SkillAidDamageCastingFrontal | 5 | 群伤 | 绝策 |
| SkillAidDamageCastingLine | 5 | 群伤 | 遗计 |
| SkillAidDamageAreaField | 5 | 群伤 | 火海 |
| SkillAidDamageLineCharge | 5 | 群伤 | 铁骑冲 |
| SkillAidDamageAreaTeamBuff | 5 | 群伤 | 护驾 |
| SkillAidDamageLineLaser | 5 | 群伤 | 流光 |
| SkillAidSummonSingleClone | 5 | 召唤 | 制衡 |
| SkillAidDamageAreaWall | 5 | 群伤 | 火墙 |
| SkillAidDamageChainBlink | 5 | 群伤 | 龙胆 |
| SkillComDamageHitRegion | 20 | 群伤 | 屯田 |
| SkillComDamageJumpChain | 5 | 群伤 | 明断 |
| SkillComHealJumpChain | 5 | 群疗 | 妖疗 |
| SkillAidDamageSingleJump | 5 | 单控 | 险渡 |
| SkillAidDamageSingleDashStealth | 5 | 单伤 | 劫营 |
| SkillComBuffSingle | 90 | 自增 | 破阵 |
| SkillAidDamageSingleSummon | 5 | 召唤 | 分兵 |
| SkillComDamageShockWave | 25 | 群伤 | 贯穿箭 |
| SkillAidDamageLineVolley | 5 | 群伤 | 连珠 |
| SkillComDamageSingle | 195 | 单伤、单控、兵增 | 蛇毒 |
| SkillAidSoldierFortify | 5 | 兵增 | 筑垒 |
| SkillComDamageTaunt | 10 | 群控 | 抬棺 |
| SkillBondAreaBuff | 5 | 连接 | 奇才 |
| SkillBondAttackAddDamage | 5 | 职业 | 猛将 |
| SkillBondAttackArmorPierce | 5 | 连接 | 悍将 |
| SkillBondAttackedShield | 5 | 连接 | 益壮 |
| SkillBondAttackMagicDamage | 5 | 连接 | 风华 |
| SkillBondAttackMpGain | 5 | 连接 | 儒将 |
| SkillBondAttackRunCross | 5 | 连接 | 虎狼 |
| SkillBondAttackRunCrossPlus | 5 | 连接 | 铁骑 |
| SkillBondAttackShieldPierce | 5 | 连接 | 陷阵 |
| SkillBondAttackSpeedAttack | 5 | 连接 | 迅捷 |
| SkillBondAttackStackBuffer | 5 | 连接 | 将相 |
| SkillBondAttackStunDamage | 5 | 连接 | 深谋 |
| SkillBondBacklineStrike | 5 | 连接 | 骑射 |
| SkillBondBuffLowHp | 5 | 连接 | 义胆 |
| SkillBondDeathGroupHeal | 5 | 连接 | 背水 |
| SkillBondDefReduceDamageRate | 5 | 连接 | 善守 |
| SkillDumb | 65 | 职业、连接 | 诸侯 |
| SkillBondFactionShield | 5 | 职业 | 国家护盾 |
| SkillBondHitArea | 10 | 职业 | 炮车 |
| SkillBondHitBuff | 5 | 职业 | 枪阵 |
| SkillBondHitFireArea | 5 | 连接 | 野火 |
| SkillBondHitRepeat | 5 | 连接 | 无双 |
| SkillBondInitAddItem | 5 | 连接 | 仁德 |
| SkillBondInitAddItemChance | 5 | 连接 | 文采 |
| SkillBondInitAttrChange | 10 | 连接 | 智勇 |
| SkillBondInitDecayDef | 5 | 连接 | 望族 |
| SkillBondInitEnemyRandomBuff | 5 | 连接 | 权奸 |
| SkillBondInitMpFill | 5 | 连接 | 王佐 |
| SkillBondInitNoEquipAttr | 5 | 连接 | 异禀 |
| SkillBondInitSoldierBuff | 5 | 连接 | 安民 |
| SkillBondKillGain | 5 | 连接 | 枭雄 |
| SkillBondModifyBuffTime | 10 | 职业 | 羽扇 |
| SkillBondSelfBuff | 5 | 连接 | 自守 |
| SkillBondSelfShield | 5 | 连接 | 温良 |
| SkillBondSneakChangePos | 5 | 连接 | 偷袭 |
| SkillAidSpinAttack | 5 | 群伤 | 旋风斩 |
| SkillComBuffAura | 35 | 光环 | 鼓舞 |
| SkillSoldierHitWall | 5 | 群伤 | 喷火 |
| SkillEquipAttackArmorPierce | 3 | 道具 | 破甲 |
| SkillEquipAttackMagicDamage | 3 | 道具 | 会心 |
| SkillEquipAttackMpGain | 3 | 道具 | 回气 |
| SkillEquipAttackShieldPierce | 1 | 道具 | 破盾 |
| SkillEquipHitWall | 1 | 道具 | 火箭 |
| SkillEquipInitShieldValue | 3 | 道具 | 玄盾 |
| SkillEquipKillGain | 2 | 道具 | 枭雄 |
| SkillEquipModifyBuffTime | 2 | 道具 | 谋定 |
| SkillEquipModifyShootSpeed | 1 | 道具 | 速射 |

## 三、英雄个人技能 → 脚本归属

152 名英雄各有 1 个个人技能（`HeroConfig.Skill1` = 技能缩写 = 英雄名），共使用 29 个脚本。

| ScriptName | 英雄数 | 英雄(技能名) |
| --- | --- | --- |
| SkillComBuffArea | 21 | 周仓(忠勇)、李严(护军)、杨仪(调度)、李典(御阵)、甄宓(惊鸿)、郭太后(垂帘)、嵇康(绝响)、娄圭(筑城)、大乔(国色)、小乔(鼓舞)、步骘(镇抚)、司马孚(纯臣)、阮籍(长啸)、郭图(强阵)、张梁(黄天)、马良(励军)、荀彧(兵精)、黄月英(巧工)、裴秀(制图)、刘备(仁德)、司马炎(一统) |
| SkillComDamageArea | 23 | 孙皓(暴敛)、徐庶(困敌)、魏延(奇袭)、黄忠(乱射)、庞统(连锁)、祝融(飞刀)、关平(承志)、刘晔(卸甲)、蒋济(伪书)、毌丘俭(征东)、诸葛诞(举兵)、孙策(破阵冲阵)、韩当(双射)、诸葛恪(刚愎)、贾诩(乱阵)、张绣(枪出如龙)、司马昭(篡逆)、张华(博物)、司马伦(乱政)、文丑(陷阵怒吼)、袁绍(名门)、典韦(天降恶来) |
| SkillComHealArea | 5 | 蒋琬(抚军)、羊祜(遗爱)、王元姬(淑德)、华佗(青囊)、左慈(天书) |
| SkillComHealSingle | 6 | 刘璋(守护)、刘禅(放权)、王浑(楼船)、于吉(妖咒)、张春华(狠决)、貂蝉(离间) |
| SkillComInitAttr | 4 | 徐盛(才干)、潘璋(才干)、诸葛绪(才干)、马谡(才干) |
| SkillComHealAreaCasting | 1 | 蔡文姬(胡笳) |
| SkillAidDamageCastingDrain | 1 | 吕布(魔神) |
| SkillAidDamageCastingFrontal | 1 | 陈宫(绝策) |
| SkillAidDamageCastingLine | 1 | 郭嘉(遗计) |
| SkillAidDamageAreaField | 1 | 陆逊(火海) |
| SkillAidDamageLineCharge | 1 | 马超(铁骑冲) |
| SkillAidDamageAreaTeamBuff | 1 | 曹操(护驾) |
| SkillAidDamageLineLaser | 1 | 姜维(流光) |
| SkillAidSummonSingleClone | 1 | 孙权(制衡) |
| SkillAidDamageAreaWall | 1 | 周瑜(火墙) |
| SkillAidDamageChainBlink | 1 | 赵云(龙胆) |
| SkillComDamageHitRegion | 4 | 张郃(震碎)、王昶(屯田)、王基(攻城)、张角(落雷) |
| SkillComDamageJumpChain | 1 | 法正(明断) |
| SkillComHealJumpChain | 1 | 张宝(妖疗) |
| SkillAidDamageSingleJump | 1 | 邓艾(险渡) |
| SkillAidDamageSingleDashStealth | 1 | 甘宁(劫营) |
| SkillComBuffSingle | 17 | 孙坚(破阵)、马岱(连射)、严颜(固守)、张辽(威震)、徐晃(长驱)、曹仁(天人守城)、郭淮(壁垒)、王凌(举义)、文钦(奋锤)、周泰(不屈)、丁奉(奋威)、凌统(迅射)、张悌(死守)、朱然(坚壁)、华雄(狂暴)、王濬(疾击)、陈群(律令) |
| SkillAidDamageSingleSummon | 1 | 关羽(分兵) |
| SkillComDamageShockWave | 5 | 钟会(惊雷)、关兴(复仇)、张苞(蛇矛)、许褚(虎啸)、鞠义(贯穿箭) |
| SkillAidDamageLineVolley | 1 | 太史慈(连珠) |
| SkillComDamageSingle | 47 | 董卓(暴虐)、刘表(乱政)、马腾(腾驾)、张松(舌辩)、孟获(蛮勇)、夏侯霸(归汉)、廖化(先锋斩)、谯周(劝学)、夏侯惇(歼灭)、夏侯渊(猎鹰)、曹洪(裂盾)、乐进(御斩)、司马懿(隐忍)、程昱(断粮)、文鸯(单骑)、曹真(虎豹)、黄盖(苦肉)、蒋钦(断流)、朱桓(标记)、程普(横扫)、高顺(破甲)、李儒(鸩酒)、陈泰(扬鞭)、杜预(发火)、王双(崩山)、贾充(构陷)、司马攸(清谈)、辛宪英(明鉴)、颜良(斩杀)、许攸(傲言)、高览(御守)、沮授(强弓)、张任(毒箭)、曹彰(突阵)、荀攸(奇策)、曹叡(压制)、陆抗(远略)、吕蒙(白衣袭)、孙尚香(纵舞) |
| SkillAidSoldierFortify | 1 | 张昭(筑垒) |
| SkillComDamageTaunt | 2 | 于禁(铁壁)、庞德(抬棺) |
| SkillAidSpinAttack | 1 | 张飞(旋风斩) |
| SkillComBuffAura | 7 | 孙乾(鼓舞)、费祎(安民)、鲁肃(长者)、诸葛瑾(敦睦)、司马师(果决)、荀顗(礼乐)、田丰(直谏) |

## 四、Buff 归属规则

| 字段 | 含义 |
| --- | --- |
| `Buff.owner` | 被挂者（持有者），效果数值与表现都加在它身上 |
| `Buff.caster` | 施加者（归属来源），用于判定"谁给的 buff" |
| `Buff.skillCfg` | 效果数值取自 `skillId` 对应的 SkillConfig 行 |
| `Buff.id` | 取 `BuffConfig.Id` |

- **施加入口**：`BuffManager.AddBuff(target, caster, skillId, buffId, time)`，target 获得 buff、caster 记归属；按 `BuffConfig.ScriptName` 反射创建 `BuffXxx`。
- **配置归属**：`SkillConfig.BuffId` 填 `BuffConfig.NameS`（短名），空串=无 buff；支持 `,` `，` `;` `；` 多段，解析走 `Skill.GetSkillBuffIds()`；多段语义由各技能脚本约定（通用组合如 `攻,甲,抗` 按位置取 `StrengthBuff1`）。
- **数值槽声明**：`BuffConfig` 用两个字段声明该 Buff 生效所需的数值槽键（多个用 `,` 分隔，槽键可落在 `StrengthBuff1` 或 `Strength2`）：
  - `BuffAttrDynamic`：受品质/等级影响的动态键，如 `缓`=`slow,atkSpeed`、`守`=`armor`；运行时品质系数只乘这些键，`2910 效果逐级提升20` 也只乘这些键；
  - `BuffAttrConst`：不受品质/等级影响的常量键，如 `守`=`perStack,maxStack`、`焰`=`damageShare`、`箭`=`extraArrows`、`锁`=`chain`。
  `SkillConfig.BuffId` 引用该 Buff 时，两个字段里声明的键必须都已赋值，`SkillBaseTool` 自检会校验（模板化技能按实际生效的 `SkillBaseConfig` 行检查）；每个使用中的槽键必须且只能落在其中一个字段。组件表不再手写属性键，`BuffEffectRate` 直接读取 `BuffAttrDynamic` 乘算；组件里 Buff 属性数值只填一级（取 Lv3）。
- **正负面与驱散**：`IsPositive` 定正负；`CanDispel=false` 为招牌/兵种 buff 不可驱散；`DispelNegative` 只清负面可驱散、`DispelPositive` 只清正面可驱散。
- **存在性驱动**：嘲讽 `BuffTaunt(301010)` 由 `Chess.FindTarget` 判断；连锁 `BuffLock(301102)`、生命链接 `BuffLifeLink(300106)` 也以"身上有没有该 buff"驱动，索敌优先级：混乱 > 嘲讽 > 常规打分。
- **表现归属**：`BuffEffect`、`ColorStart/ColorEnd`、`Material` 均作用在 owner 上，与 caster 无关。
- **Id 号段**：`300xxx` 正面通用、`3001xx` 正面特化、`301xxx` 负面通用、`3011xx` 负面特化。

## 五、Buff 引用统计

统计口径：`SkillConfig.BuffId` 按短名匹配 `BuffConfig.NameS`，"引用技能数"按不同 `Sname` 去重（一个技能 5 级只算 1 个）。

| Id | 名称(短名) | 正/负 | Buff 脚本 | 引用技能数 | 引用技能示例 |
| --- | --- | --- | --- | --- | --- |
| 300001 | 护盾(盾) | 正 | BuffShield | 7 | 国、护、严颜、蒋琬、程普、周泰、李严 |
| 300002 | 减伤盾(硬) | 正 | BuffShieldValue | 8 | 老、徐盛、黄盖、张梁、于禁、玄盾、文润、玄武 |
| 300003 | 吸血(吸) | 正 | BuffSuck | 1 | 徐晃 |
| 300004 | 伤害提升(重) | 正 | BuffFrenzy | 2 | 徐晃、华雄 |
| 300005 | 攻速提升(快) | 正 | BuffCoolDown | 3 | 小乔、王濬、凌统 |
| 300007 | 急救(愈) | 正 | BuffTimeHeal | 2 | 老、吕蒙 |
| 300008 | 攻击提升(攻) | 正 | BuffAtkAdd | 7 | 才、甄宓、马良、荀彧、马腾、刘备、司马炎 |
| 300009 | 移速提升(速) | 正 | BuffMoveSpeedAdd | 1 | 小乔 |
| 300012 | 护甲(甲) | 正 | BuffArmorAdd | 11 | 才、俭、刘禅、大乔、周仓、荀彧、高览、周泰… |
| 300013 | 魔抗提升(抗) | 正 | BuffMagicResAdd | 3 | 才、吕蒙、荀彧 |
| 300014 | 生命回复(血) | 正 | BuffHpRegenAdd | 1 | 鼓舞 |
| 300015 | 闪避(闪) | 正 | BuffEvasion | 1 | 左慈 |
| 300016 | 法强提升(术) | 正 | BuffApAdd | 1 | 曹操 |
| 300018 | 法力回复(蓝) | 正 | BuffMpRegenAdd | 0 |  |
| 300019 | 暴击提升(暴) | 正 | BuffCritRate | 2 | 丁奉、董卓 |
| 300025 | 反伤(反) | 正 | BuffReflect | 2 | 自守、黄天 |
| 300030 | 隐匿(隐) | 正 | BuffStealth | 1 | 甘宁 |
| 300101 | 据守(守) | 正 | BuffDefStack | 1 | 曹仁 |
| 300106 | 生命链接(链) | 正 | BuffLifeLink | 1 | 貂蝉 |
| 300107 | 望族(望) | 正 | BuffDecayDef | 1 | 族 |
| 300108 | 兼资(兼) | 正 | BuffStackBuf | 1 | 兼 |
| 300109 | 威震(震) | 正 | BuffVengefulStance | 1 | 张辽 |
| 301001 | 眩晕(乱) | 负 | BuffNoAction | 4 | 枪、谋、吕布、夏侯惇 |
| 301003 | 增伤(伤) | 负 | BuffDamagedAddRate | 6 | 冲、奸、朱桓、华雄、许攸、刘晔 |
| 301004 | 减攻(慑) | 负 | BuffAtkDown | 4 | 徐庶、马腾、陈宫、程昱 |
| 301005 | 陷阵(停) | 负 | BuffNoMove | 6 | 周瑜、邓艾、张昭、陆逊、司马昭、马谡 |
| 301006 | 溃败(败) | 负 | BuffTimeDamage | 5 | 张飞、于吉、张任、李儒、蒋钦 |
| 301007 | 减疗(疫) | 负 | BuffHealDown | 4 | 徐庶、钟会、李儒、魏延 |
| 301008 | 减速(缓) | 负 | BuffSlowDown | 5 | 钟会、吕蒙、陈泰、荀攸、张松 |
| 301009 | 减防(破) | 负 | BuffArmorDown | 7 | 王双、夏侯渊、马超、曹彰、潘璋、孙坚、张绣 |
| 301010 | 嘲讽(嘲) | 负 | BuffTaunt | 0 |  |
| 301011 | 叛逃(叛) | 负 | BuffDefect | 1 | 司马懿 |
| 301012 | 混乱(惑) | 负 | BuffChaos | 1 | 贾诩 |
| 301102 | 连锁(锁) | 负 | BuffLock | 1 | 庞统 |

### 当前未被任何技能引用的 Buff

| Id | 名称(短名) | 脚本 |
| --- | --- | --- |
| 300018 | 法力回复(蓝) | BuffMpRegenAdd |

## 六、代码索引

| 内容 | 位置 |
| --- | --- |
| 技能脚本创建 | [SkillManager.cs](../../Assets/Resources/Scripts/Combat/Skill/SkillManager.cs) |
| 技能基类 / BuffId 解析 | [Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs) |
| Buff 基类 / 管理器 | [Buff.cs](../../Assets/Resources/Scripts/Combat/Buff/Buff.cs)、[BuffManager.cs](../../Assets/Resources/Scripts/Combat/Buff/BuffManager.cs) |
| 索敌与嘲讽/混乱 | [Chess.cs](../../Assets/Resources/Scripts/Combat/Chess.cs) `FindTarget` |
