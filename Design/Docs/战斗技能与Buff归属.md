# 战斗技能脚本归属与 Buff 引用统计

> 整理日期：2026-10-27（按当前 `SkillConfig` / `HeroConfig` / `BuffConfig` 全量重生成）
> 范围：技能分类与脚本归属、110 个英雄个人技能 → 脚本映射、Buff 归属规则与引用统计
> 数据源：[SkillConfig_s.cs](../../Assets/Resources/Scripts/Configs/SkillConfig_s.cs)、[HeroConfig_s.cs](../../Assets/Resources/Scripts/Configs/HeroConfig_s.cs)、[BuffConfig_s.cs](../../Assets/Resources/Scripts/Configs/BuffConfig_s.cs)

---

## 一、技能构成总览

| 类别 | 识别方式 | 技能数(Sname) | 配置行 | 使用脚本数 |
| --- | --- | --- | --- | --- |
| 英雄个人技能 | `Sname` = 英雄名，Type 非职业/连接/道具 | 110 | 550 | 45 |
| 职业/国家连锁 | Type=职业 | 19 | 95 | 6 |
| 好友连锁 | Type=连接 | 31 | 155 | 30 |
| 道具技能 | Type=道具 | 19 | 19 | 9 |
| **合计** | — | **179** | **819** | **79** |

- 每个技能按 `Sname` 展开为 Lv1~Lv5（道具技能多为单行 Lv1）。
- 技能类型分布：光环 30 行、兵增 25 行、单伤 140 行、单增 35 行、单控 30 行、单疗 15 行、召唤 10 行、群伤 145 行、群增 20 行、群控 25 行、群疗 20 行、职业 95 行、自增 55 行、连接 155 行、道具 19 行

## 二、ScriptName 全量清单

`SkillManager.CreateSkill` 按 `SkillConfig.ScriptName` 反射创建 `(int skillId, Chess owner)` 构造的脚本；约定 ScriptName 即类名。

| ScriptName | 行数 | 类型 | 技能示例 |
| --- | --- | --- | --- |
| SkillAidAmbush | 5 | 单伤 | 白衣渡江 |
| SkillAidBackstepShot | 5 | 单伤 | 纵舞 |
| SkillAidBuffLowHp | 5 | 连接 | 义胆 |
| SkillAidCavalryStrike | 5 | 群伤 | 铁骑冲 |
| SkillAidChargeImpale | 5 | 群伤 | 破阵冲阵 |
| SkillAidCraftBuff | 5 | 单增 | 巧工 |
| SkillAidCurseHeal | 5 | 单疗 | 妖咒 |
| SkillAidDominion | 5 | 群伤 | 护驾 |
| SkillAidDualLaser | 5 | 群伤 | 流光 |
| SkillAidEnduranceStrike | 5 | 单控 | 隐忍 |
| SkillAidFeint | 5 | 召唤 | 制衡 |
| SkillAidFireWall | 5 | 群伤 | 火墙 |
| SkillAidFlashStrike | 5 | 群伤 | 龙胆 |
| SkillAidHitRegion | 10 | 群伤 | 落雷、震碎 |
| SkillAidJumpReturn | 5 | 单控 | 偷渡阴平 |
| SkillAidLifeLink | 5 | 单疗 | 离间 |
| SkillAidManaRally | 5 | 群增 | 名门 |
| SkillAidManaShare | 5 | 群增 | 仁德 |
| SkillAidRaidCamp | 5 | 单伤 | 劫营 |
| SkillAidSelfShield | 5 | 连接 | 温良 |
| SkillAidShadowSplit | 5 | 召唤 | 分兵 |
| SkillAidSkyFall | 5 | 群伤 | 天降恶来 |
| SkillAreaBuff | 45 | 单增、群增、连接 | 奇才、洛神、鼓舞、国色、忠勇… |
| SkillAreaDamageDeBuff | 60 | 群伤、群控 | 乱阵、连锁、困敌、乱射、双射… |
| SkillAreaHeal | 15 | 单疗、群疗 | 安乐、青囊、天书 |
| SkillAttackAddDamage | 5 | 职业 | 猛将 |
| SkillAttackArmorPierce | 8 | 连接、道具 | 悍将、破甲、疾风、破军 |
| SkillAttackMagicDamage | 8 | 连接、道具 | 风华、会心、玄妙、灵犀 |
| SkillAttackMpGain | 8 | 连接、道具 | 儒将、回气、浩然、凝气 |
| SkillAttackRunCross | 5 | 连接 | 虎狼 |
| SkillAttackRunCrossPlus | 10 | 群伤、连接 | 铁骑、突阵 |
| SkillAttackShieldPierce | 6 | 连接、道具 | 陷阵、破盾 |
| SkillAttackSpeedAttack | 5 | 连接 | 迅捷 |
| SkillAttackSpinAttack | 5 | 群伤 | 旋风斩 |
| SkillAttackStackBuffer | 5 | 连接 | 将相 |
| SkillAttackStunDamage | 5 | 连接 | 深谋 |
| SkillAttackedShield | 10 | 自增、连接 | 益壮、老当益壮 |
| SkillAura | 30 | 光环 | 鼓舞、安民、敦睦、直谏、果决… |
| SkillBacklineStrike | 15 | 单伤、连接 | 骑射、单骑、强击 |
| SkillCastingAidAreaHeal | 5 | 群疗 | 胡笳 |
| SkillCastingAidDrain | 5 | 单控 | 魔神 |
| SkillCastingAidFrontalFlaming | 5 | 群伤 | 绝策 |
| SkillCastingAidLastStrategy | 5 | 群伤 | 遗计 |
| SkillCastingAidScorchedEarth | 5 | 群伤 | 火海 |
| SkillDeathGroupHeal | 5 | 连接 | 背水 |
| SkillDefFeedback | 5 | 连接 | 自守 |
| SkillDefReduceDamageRate | 5 | 连接 | 善守 |
| SkillDumb | 65 | 职业、连接 | 诸侯、坚韧、灵活、运筹、弓手… |
| SkillFactionShield | 5 | 职业 | 国家护盾 |
| SkillHitArea | 10 | 职业 | 炮车、戟阵 |
| SkillHitBuff | 5 | 职业 | 枪阵 |
| SkillHitFireArea | 5 | 连接 | 野火 |
| SkillHitRepeat | 5 | 连接 | 无双 |
| SkillHitWall | 1 | 道具 | 火箭 |
| SkillInitAddItem | 5 | 连接 | 仁德 |
| SkillInitAddItemChance | 5 | 连接 | 文采 |
| SkillInitAttrChange | 10 | 连接 | 智勇、严明 |
| SkillInitDecayDef | 5 | 连接 | 望族 |
| SkillInitEnemyRandomBuff | 5 | 连接 | 权奸 |
| SkillInitMpFill | 5 | 连接 | 王佐 |
| SkillInitNoEquipAttr | 5 | 连接 | 异禀 |
| SkillInitShieldValue | 3 | 道具 | 玄盾、文润、玄武 |
| SkillInitSneakChangePos | 5 | 连接 | 偷袭 |
| SkillInitSoldierBuff | 5 | 连接 | 安民 |
| SkillJumpChain | 10 | 群伤、群疗 | 妖疗、明断 |
| SkillKillGain | 7 | 连接、道具 | 枭雄、凶威 |
| SkillModifyBuffTime | 12 | 职业、道具 | 羽扇、声乐、谋定、影遁 |
| SkillModifyShootSpeed | 1 | 道具 | 速射 |
| SkillMultiHit | 5 | 单伤 | 奇策 |
| SkillSelfBuff | 50 | 自增 | 疑城、天人守城、奋威、疾击、连射… |
| SkillShockWave | 15 | 群伤 | 惊雷、贯穿箭、虎啸 |
| SkillShockWaveMulti | 5 | 群伤 | 连珠 |
| SkillSingleDamage | 125 | 单伤、单控 | 崩山、破甲、腾驾、猎鹰、御守… |
| SkillSoldierArcherBuff | 5 | 兵增 | 强弓 |
| SkillSoldierBless | 5 | 兵增 | 兵精 |
| SkillSoldierFortify | 5 | 兵增 | 筑垒 |
| SkillSoldierGrow | 5 | 兵增 | 励军 |
| SkillSoldierSingle | 5 | 兵增 | 抚军 |
| SkillTaunt | 15 | 群控 | 抬棺、苦肉、铁壁 |

## 三、英雄个人技能 → 脚本归属

110 名英雄各有 1 个个人技能（`HeroConfig.Skill1` = 技能缩写 = 英雄名），共使用 45 个脚本。

| ScriptName | 英雄数 | 英雄(技能名) |
| --- | --- | --- |
| SkillSingleDamage | 25 | 董卓(暴虐)、张松(舌辩)、马谡(守势)、夏侯惇(歼灭)、夏侯渊(猎鹰)、曹洪(裂盾)、乐进(御斩)、程昱(断粮)、曹真(虎豹)、陈群(律令)、蒋钦(江河)、朱桓(标记)、潘璋(偷刃)、程普(横扫)、高顺(破甲)、李儒(鸩酒)、陈泰(扬鞭)、杜预(发火)、王双(崩山)、颜良(斩杀)、许攸(傲言)、高览(御守)、张任(毒箭)、马腾(腾驾)、孟获(蛮勇) |
| SkillAreaDamageDeBuff | 12 | 徐庶(困敌)、魏延(奇袭)、黄忠(乱射)、庞统(连锁)、刘晔(卸甲)、孙坚(破阵)、韩当(双射)、贾诩(乱阵)、张绣(枪出如龙)、司马昭(篡逆)、钟会(蓄谋)、文丑(陷阵怒吼) |
| SkillSelfBuff | 10 | 马岱(连射)、张辽(威震)、徐晃(长驱)、曹仁(天人守城)、周泰(不屈)、丁奉(奋威)、凌统(迅射)、徐盛(疑城)、华雄(狂暴)、王濬(疾击) |
| SkillAreaBuff | 8 | 司马炎(一统)、周仓(忠勇)、李严(护军)、李典(御阵)、甄宓(洛神)、大乔(国色)、小乔(鼓舞)、张梁(黄天) |
| SkillAura | 6 | 孙乾(鼓舞)、费祎(安民)、鲁肃(长者)、诸葛瑾(敦睦)、司马师(果决)、田丰(直谏) |
| SkillAreaHeal | 3 | 刘禅(安乐)、华佗(青囊)、左慈(天书) |
| SkillShockWave | 3 | 诸葛亮(惊雷)、许褚(虎啸)、鞠义(贯穿箭) |
| SkillTaunt | 3 | 于禁(铁壁)、庞德(抬棺)、黄盖(苦肉) |
| SkillAidHitRegion | 2 | 张郃(震碎)、张角(落雷) |
| SkillBacklineStrike | 2 | 文鸯(单骑)、郭图(强击) |
| SkillJumpChain | 2 | 法正(明断)、张宝(妖疗) |
| SkillAidAmbush | 1 | 吕蒙(白衣渡江) |
| SkillAidBackstepShot | 1 | 孙尚香(纵舞) |
| SkillAidCavalryStrike | 1 | 马超(铁骑冲) |
| SkillAidChargeImpale | 1 | 孙策(破阵冲阵) |
| SkillAidCraftBuff | 1 | 黄月英(巧工) |
| SkillAidCurseHeal | 1 | 于吉(妖咒) |
| SkillAidDominion | 1 | 曹操(护驾) |
| SkillAidDualLaser | 1 | 姜维(流光) |
| SkillAidEnduranceStrike | 1 | 司马懿(隐忍) |
| SkillAidFeint | 1 | 孙权(制衡) |
| SkillAidFireWall | 1 | 周瑜(火墙) |
| SkillAidFlashStrike | 1 | 赵云(龙胆) |
| SkillAidJumpReturn | 1 | 邓艾(偷渡阴平) |
| SkillAidLifeLink | 1 | 貂蝉(离间) |
| SkillAidManaRally | 1 | 袁绍(名门) |
| SkillAidManaShare | 1 | 刘备(仁德) |
| SkillAidRaidCamp | 1 | 甘宁(劫营) |
| SkillAidShadowSplit | 1 | 关羽(分兵) |
| SkillAidSkyFall | 1 | 典韦(天降恶来) |
| SkillAttackRunCrossPlus | 1 | 曹彰(突阵) |
| SkillAttackSpinAttack | 1 | 张飞(旋风斩) |
| SkillAttackedShield | 1 | 严颜(老当益壮) |
| SkillCastingAidAreaHeal | 1 | 蔡文姬(胡笳) |
| SkillCastingAidDrain | 1 | 吕布(魔神) |
| SkillCastingAidFrontalFlaming | 1 | 陈宫(绝策) |
| SkillCastingAidLastStrategy | 1 | 郭嘉(遗计) |
| SkillCastingAidScorchedEarth | 1 | 陆逊(火海) |
| SkillMultiHit | 1 | 荀攸(奇策) |
| SkillShockWaveMulti | 1 | 太史慈(连珠) |
| SkillSoldierArcherBuff | 1 | 沮授(强弓) |
| SkillSoldierBless | 1 | 荀彧(兵精) |
| SkillSoldierFortify | 1 | 张昭(筑垒) |
| SkillSoldierGrow | 1 | 马良(励军) |
| SkillSoldierSingle | 1 | 蒋琬(抚军) |

## 四、Buff 归属规则

| 字段 | 含义 |
| --- | --- |
| `Buff.owner` | 被挂者（持有者），效果数值与表现都加在它身上 |
| `Buff.caster` | 施加者（归属来源），用于判定"谁给的 buff" |
| `Buff.skillCfg` | 效果数值取自 `skillId` 对应的 SkillConfig 行 |
| `Buff.id` | 取 `BuffConfig.Id` |

- **施加入口**：`BuffManager.AddBuff(target, caster, skillId, buffId, time)`，target 获得 buff、caster 记归属；按 `BuffConfig.ScriptName` 反射创建 `BuffXxx`。
- **配置归属**：`SkillConfig.BuffId` 填 `BuffConfig.NameS`（短名），空串=无 buff；支持 `,` `，` `;` `；` 多段，解析走 `Skill.GetSkillBuffIds()`；多段语义由各技能脚本约定（通用组合如 `攻,甲,抗` 按位置取 `StrengthBuff1`）。
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
| 300014 | 生命回复(血) | 正 | BuffHpRegenAdd | 0 |  |
| 300015 | 闪避(闪) | 正 | BuffEvasion | 1 | 左慈 |
| 300016 | 法强提升(术) | 正 | BuffApAdd | 1 | 曹操 |
| 300018 | 法力回复(蓝) | 正 | BuffMpRegenAdd | 0 |  |
| 300019 | 暴击提升(暴) | 正 | BuffCritRate | 2 | 丁奉、董卓 |
| 300025 | 反伤(反) | 正 | BuffReflect | 0 |  |
| 300030 | 隐匿(隐) | 正 | BuffStealth | 1 | 甘宁 |
| 300101 | 据守(守) | 正 | BuffDefStack | 1 | 曹仁 |
| 300102 | 复活(复) | 正 | BuffRevive | 0 |  |
| 300104 | 多重箭(箭) | 正 | BuffMultiShot | 0 |  |
| 300106 | 生命链接(链) | 正 | BuffLifeLink | 1 | 貂蝉 |
| 300107 | 望族(望) | 正 | BuffDecayDef | 1 | 族 |
| 300108 | 兼资(兼) | 正 | BuffStackBuf | 1 | 兼 |
| 300109 | 威震(震) | 正 | BuffVengefulStance | 1 | 张辽 |
| 300128 | 护驾(护) | 正 | BuffHealBoost | 0 |  |
| 301001 | 眩晕(乱) | 负 | BuffNoAction | 4 | 枪、谋、吕布、夏侯惇 |
| 301003 | 增伤(伤) | 负 | BuffDamagedAddRate | 6 | 冲、奸、朱桓、华雄、许攸、刘晔 |
| 301004 | 减攻(慑) | 负 | BuffAtkDown | 4 | 徐庶、马腾、陈宫、程昱 |
| 301005 | 陷阵(停) | 负 | BuffNoMove | 6 | 周瑜、邓艾、张昭、陆逊、司马昭、马谡 |
| 301006 | 溃败(败) | 负 | BuffTimeDamage | 5 | 张飞、于吉、张任、李儒、蒋钦 |
| 301007 | 减疗(疫) | 负 | BuffHealDown | 5 | 徐庶、钟会、李儒、魏延、陈群 |
| 301008 | 减速(缓) | 负 | BuffSlowDown | 5 | 诸葛亮、吕蒙、陈泰、荀攸、张松 |
| 301009 | 减防(破) | 负 | BuffArmorDown | 7 | 王双、夏侯渊、马超、曹彰、潘璋、孙坚、张绣 |
| 301010 | 嘲讽(嘲) | 负 | BuffTaunt | 0 |  |
| 301011 | 叛逃(叛) | 负 | BuffDefect | 1 | 司马懿 |
| 301012 | 混乱(惑) | 负 | BuffChaos | 1 | 贾诩 |
| 301102 | 连锁(锁) | 负 | BuffLock | 1 | 庞统 |

### 当前未被任何技能引用的 Buff

| Id | 名称(短名) | 脚本 |
| --- | --- | --- |
| 300014 | 生命回复(血) | BuffHpRegenAdd |
| 300018 | 法力回复(蓝) | BuffMpRegenAdd |
| 300025 | 反伤(反) | BuffReflect |
| 300102 | 复活(复) | BuffRevive |
| 300104 | 多重箭(箭) | BuffMultiShot |
| 300128 | 护驾(护) | BuffHealBoost |
| 301010 | 嘲讽(嘲) | BuffTaunt |

## 六、代码索引

| 内容 | 位置 |
| --- | --- |
| 技能脚本创建 | [SkillManager.cs](../../Assets/Resources/Scripts/Combat/Skill/SkillManager.cs) |
| 技能基类 / BuffId 解析 | [Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs) |
| Buff 基类 / 管理器 | [Buff.cs](../../Assets/Resources/Scripts/Combat/Buff/Buff.cs)、[BuffManager.cs](../../Assets/Resources/Scripts/Combat/Buff/BuffManager.cs) |
| 索敌与嘲讽/混乱 | [Chess.cs](../../Assets/Resources/Scripts/Combat/Chess.cs) `FindTarget` |
