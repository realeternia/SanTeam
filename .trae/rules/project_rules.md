# SanTeam 项目规则

## 项目概述

Unity C# 三国题材自动战斗卡牌游戏（金铲铲式玩法）：卡牌商店/抽卡 → 布阵（5x5）→ 实时自动战斗（棋子 Chess + 技能/导弹/Buff）→ 排名结算。含职业连锁（JobLink）、好友连锁（FriendLine）、同阵营护盾等羁绊机制。

## 技术栈

Unity (C#) | JsonUtility | UGUI | TextMeshPro

## 目录结构

- 脚本：`Assets/Resources/Scripts/`，其中 `Configs/`（配置类）、`Combat/`（战斗层：Chess、Skill/、Buff/、各 Manager）、`PO/`（纯数据对象）
- 资源：`Assets/Resources/` 下按类型分目录（Skins/ SkinsBig/ ItemPic/ SkillPic/ PlayerPic/ BGMs/ Anims/ Prefabs/ Maps/）
- Editor 脚本：`Assets/Editor/`

## 编码规范

### 命名约定

- 类名/方法名/属性/枚举：PascalCase
- 公共/私有字段：camelCase（不使用下划线前缀）
- 常量：PascalCase 命名 `public const`，集中在常量类中
- 配置类命名空间：`CommonConfig`，类文件命名 `XxxConfig_s.cs`（类名不带 `_s`）

### 单例模式

- Manager 类：MonoBehaviour 单例，`public static XxxManager Instance`，`Awake()` 中赋值 `Instance = this`
- 静态工具类：`static class`（如 `SysRandom`, `SysColor`, `CombatConst`, `ConfigManager`, `JobLinkManager`）
- 日志单例：`GameLog.Instance`（线程安全懒加载，不要模仿它写新单例）

### 日志规范

使用 `GameLog`（Debug/Info/Warn/Error），消息用中文；带标签日志用 `GameLog.SetTag("tag")` 返回 `TaggedLogger`。禁止直接 `UnityEngine.Debug.Log`。

### 随机数规范

统一使用 `SysRandom`（Range/Value/Next/InsideUnitCircle，支持 Seed）。禁止 `UnityEngine.Random`，也不要散落 `new System.Random()`。

## 核心架构

### 配置系统

- 命名空间：`CommonConfig`，文件命名 `XxxConfig_s.cs`，类内只定义公共字段（禁止属性），字段加 `/// <summary>` 中文注释
- 加载入口：`ConfigManager.Init()` 统一调用各 `XxxConfig.Load()`，新增配置类必须在 `ConfigManager.Init()` 中注册
- 获取配置：`XxxConfig.GetConfig(id)` / `HasConfig(id)` / `ConfigList`；技能按缩写+等级取：`SkillConfig.GetConfig(sname, lv)`；属性配置按名取：`GetConfigByname(name)`
- 配置后处理（字段补默认值/联动修正）统一放 `ConfigManager.PostModify()`，如 HeroConfig 数值为 0 时取职业基准值
- 禁止在配置类（`XxxConfig_s.cs`）中添加业务方法，配置类仅保留数据定义、Load、GetConfig、HasConfig 等基础方法
- 战斗中用到的技能/Buff Id 常量放 `CombatConst`（`Combat/CombatConst.cs`），不要在战斗代码里硬编码 Id

### 技能系统

- 基类 `Skill`（`Combat/Skill/Skill.cs`），派生类 `SkillXxx` 放 `Combat/Skill/`，一技能一文件
- 技能等级机制：同一技能在 SkillConfig 表按 Sname 展开为 Lv1~5 多行，`SetLevel(lv)` 会切换到对应配置行；连锁机制（兵种/好友特殊）通过修改 `Level` 生效
- 技能伤害统一公式：`Strength + 关联属性 × SkillDamageAttrRate`（Attr：ap=法强 / might=无双 / atk=武力）

### Buff 系统

- 基类 `Buff`（`Combat/Buff/Buff.cs`），派生类 `BuffXxx` 放 `Combat/Buff/`，由 `BuffManager` 统一管理

### 连锁/羁绊机制

- 兵种连锁 `JobLinkManager`（静态类）：上阵同职业 1/2/3/4/5 人对应职业技能 Lv1~5 档位，LinkSelf=连接英雄自身加成，LinkTeam=全队总量加成（配置即总量，不乘人数），数值全部走 SkillConfig，不硬编码
- 数值档位（如阵营护盾 3/5/7/9、好友连线 2~7 人）统一放 `CombatConst` 的数组常量

### 纯数据对象 - PO

`PO/` 目录存放 `[System.Serializable]` 纯数据类（AttrInfo、BuffTime、SideInfo 等），只含字段和简单存取方法，不放业务逻辑。

### 存档系统

`JsonUtility` 序列化，保存到 `Application.persistentDataPath + "/game_save.json"`；存档辅助类（SaveData）嵌套在 GameManager 内，玩家数据通过 PlayerInfo.Serialize/Deserialize 转字符串存取。

### 面板管理

`PanelManager` 统一 Show/Hide 各面板（ShowShop/ShowBag/ShowRank...），面板组件实现 `OnShow()/OnHide()` 供刷新；面板打开/关闭播配音走 `GameManager.Instance.PlaySound(path, priority)`。

### 图标加载

UI 图标统一走 `IconLoader` 组件（Inspector 配 sourceType：Path / HeroAttr / SysAttr），图标资源位于 `Textures/Icons/` 下。业务代码新增图标加载优先复用 IconLoader，不要散落 `Resources.Load<Sprite>`。

### 颜色系统 - SysColor

禁止硬编码颜色，使用 `SysColor` 静态类（`SysColor.cs`）：
- 品质/阵营/属性颜色：`GetQualityColor` / `GetSideColor` / `GetColorByValue` / `GetColoredText`
- 主题与 UI 常用色：嵌套类 `SysColor.Theme` / `SysColor.UI` / `SysColor.Battle` / `SysColor.Hero` / `SysColor.Chess` / `SysColor.WorldMap` / `SysColor.Tech` / `SysColor.Tier` / `SysColor.Card` / `SysColor.BattleText` / `SysColor.Player`，新增颜色加到对应嵌套类

## UI 规范（Tooltip / 列表）

- Tooltip 定位计算必须在 Canvas 本地空间用 UI 单位进行，边界判断用 `canvasRect.rect`，禁止拿 `Screen.height`（像素）与 UI 单位直接比较
- Tooltip 默认显示在点击位置右侧，超出右边界则换左侧；垂直居中于点击点；高度超屏幕 95% 时动态调整 UIScale；底部超出屏幕时上抬并保留 40 底边距
- 大数据量 UI 列表（如英雄排行榜）必须使用虚拟滚动（循环滚动 + 对象池），只实例化可见 cell，禁止一次性实例化全部条目
- 英雄 Tooltip 中职业技能需展示当前档与下一档数值（最高5级，对应1/2/3/4/5人），并根据当前上阵同职业人数高亮当前档位

## 全局标识符

- heroId：英雄配置 ID（HeroConfig.Id，10万段：10xxxx，阵营主公 100000+side）
- side：阵营 ID（1魏 2蜀 3吴 4晋 5群 6神）
- pid：玩家 ID（0 为人类玩家）
- sname：技能缩写名（SkillConfig.Sname，同 Sname 不同 Lv 为同一技能）

## 禁止事项

- 禁止 `UnityEngine.Random` 和散落的 `new System.Random()`，统一用 `SysRandom`
- 禁止 `UnityEngine.Debug.Log`，统一用 `GameLog`，消息用中文
- 禁止配置类使用属性，用公共字段
- 禁止在配置类（`XxxConfig_s.cs`）中添加业务方法
- 禁止硬编码颜色，用 `SysColor`（含嵌套类）
- 禁止在战斗代码中硬编码技能/Buff Id 和机制数值，提取到 `CombatConst`
- 新增 `.cs` 文件必须在 `Assembly-CSharp.csproj` 添加 `<Compile Include>`（Editor 脚本加到 `Assembly-CSharp-Editor.csproj`）
- 新增配置类必须在 `ConfigManager.Init()` 中注册 Load
- 禁止静默 null check return，必须记录日志

## 错误处理

从 ID 获取数据失败、配置加载失败、关键业务逻辑前置条件不满足、存档读写失败时必须用 `GameLog` 记录日志。可静默返回的情况：遍历跳过无效项、可选参数为空、UI 数据未就绪。
