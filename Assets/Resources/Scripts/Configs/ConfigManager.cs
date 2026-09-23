using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonConfig;
using UnityEngine;

public static class ConfigManager
{
    private static Dictionary<int, Dictionary<int, int>> heroFriendDict = new Dictionary<int, Dictionary<int, int>>();
    private static Dictionary<int, HashSet<int>> heroFriendInfoDict = new Dictionary<int, HashSet<int>>(); // heroId, heroId, level
    // 好友连锁·特殊：英雄 -> (好友 -> 关联技能缩写Sname / 连线颜色)，仅在关系配置了 SkillId(非空) 时记录
    private static Dictionary<int, Dictionary<int, string>> heroFriendSkillDict = new Dictionary<int, Dictionary<int, string>>();
    private static Dictionary<int, Dictionary<int, string>> heroFriendColorDict = new Dictionary<int, Dictionary<int, string>>();
    private static Dictionary<string, JobConfig> jobDict = new Dictionary<string, JobConfig>();
    private static Dictionary<string, SkillConfig> skillDict = new Dictionary<string, SkillConfig>();

    private static bool hasInit = false;

    public static void Init()
    {
        if (hasInit)
            return;
        hasInit = true;
        
        HeroConfig.Load();
        SkillConfig.Load();
        BuffConfig.Load();
        ItemConfig.Load();
        SoldierConfig.Load();
        GameRoundConfig.Load();
        PlayerConfig.Load();
        HeroFriendConfig.Load();
        PlayerLevelConfig.Load();
        SoldierLevelConfig.Load();
        FormulaLearnAttrConfig.Load();
        JobConfig.Load();
        ForceConfig.Load();
        HeroAttrConfig.Load();
        SystemAttrConfig.Load();
        ItemCombineConfig.Load();

        ConfigManager.PostModify();      

        GameLog.Debug("ConfigManager Init fin");
    }

    public static void PostModify()
    {
        foreach (var jobCfg in JobConfig.ConfigList)
        {
            jobDict.Add(jobCfg.NameS, jobCfg);
        }      
        foreach (var skillCfg in SkillConfig.ConfigList)
        {
            // 每个技能在配置表中展开为1-5级多行，按缩写去重时优先取5级作为代表（数值各级相同）
            if (!skillDict.ContainsKey(skillCfg.Sname) || skillCfg.Lv == 5)
                skillDict[skillCfg.Sname] = skillCfg;
        }

        // 数值写回（百分比语义）：HeroConfig 数值列 = 相对 JobConfig 职业基准的百分比修正（0=无修正用基准，非0=±%）
        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            var jobCfg = GetJobConfig(heroCfg.Job);
            if (jobCfg == null)
            {
                GameLog.Error(string.Format("ConfigManager.PostModify: 英雄[{0}]职业[{1}]缺少 JobConfig，无法写回基准属性", heroCfg.Name, heroCfg.Job));
                continue;
            }
            // 次级面板（移速/射程/攻速/护甲/魔抗/生命回复/魔法回复）：写回 = 职业基准×(1+修正%/100)，不乘品质系数
            heroCfg.MoveSpeed = (int)Math.Round(jobCfg.MoveSpeed * (100f + heroCfg.MoveSpeed) / 100f);
            heroCfg.Range = (int)Math.Round(jobCfg.Range * (100f + heroCfg.Range) / 100f);
            heroCfg.AtkSpeed = (int)Math.Round(jobCfg.AtkSpeed * (100f + heroCfg.AtkSpeed) / 100f);
            heroCfg.Armor = (int)Math.Round(jobCfg.Armor * (100f + heroCfg.Armor) / 100f);
            heroCfg.MagicRes = (int)Math.Round(jobCfg.MagicRes * (100f + heroCfg.MagicRes) / 100f);
            heroCfg.HpRegen = (int)Math.Round(jobCfg.HpRegen * (100f + heroCfg.HpRegen) / 100f);
            heroCfg.MpRegen = (int)Math.Round(jobCfg.MpRegen * (100f + heroCfg.MpRegen) / 100f);
            heroCfg.Ap = (int)Math.Round(jobCfg.Ap * (100f + heroCfg.Ap) / 100f);
            // 主属性（攻击/法术/生命）：写回 = 职业基准×(1+修正%/100) × 品质系数1.15^(Q-1)，即“1星带品质面板”
            // （图鉴/排行/发卡/AI/排序直接读即为此口径）；星级成长保留到运行时按每星 ×1.7 乘
            float qualityFactor = Mathf.Pow(1.23f, Mathf.Max(1, heroCfg.Quality) - 1);
            heroCfg.Atk = (int)Math.Round(jobCfg.Atk * (100f + heroCfg.Atk) / 100f * qualityFactor);
            heroCfg.Hp = (int)Math.Round(jobCfg.Hp * (100f + heroCfg.Hp) / 100f * qualityFactor);
        }
    }

    public static void InitFriend()
    {
        // 先收集需要移除的键，然后移除
        List<int> idsToRemove = new List<int>();
        foreach (var config in HeroFriendConfig.ConfigList)
        {
            if (config.Id >= 1000)  // 注意这里应该是Id而不是id
                idsToRemove.Add(config.Id);
        }
        
        foreach (int id in idsToRemove)
            HeroFriendConfig.Remove(id);

        foreach(var f in GameManager.Instance.friendRdData)
        {
            GameLog.Debug($"创建{f.id} / {f.name} 配对: {string.Join(",", f.friendIds.Select(id => HeroConfig.GetConfig(id).Name))}");
            var config = new HeroFriendConfig(f.id, f.name, 2, f.friendIds, "", "");
            HeroFriendConfig.Add(f.id, config);
        }

        heroFriendDict.Clear();
        heroFriendInfoDict.Clear();
        heroFriendSkillDict.Clear();
        heroFriendColorDict.Clear();
        foreach (var heroFriendCfg in HeroFriendConfig.ConfigList)
        {
            var friendIds = heroFriendCfg.Heros;
            for (int i = 0; i < friendIds.Length; i++)
            {
                for (int j = i + 1; j < friendIds.Length; j++)
                {
                    int id1 = friendIds[i];
                    int id2 = friendIds[j];

                    // 双向添加，确保两两配对
                    if (!heroFriendDict.ContainsKey(id1))
                        heroFriendDict.Add(id1, new Dictionary<int, int>());
                    heroFriendDict[id1][id2] = Math.Max(heroFriendCfg.Level, heroFriendDict[id1].ContainsKey(id2) ? heroFriendDict[id1][id2] : 0);

                    if (!heroFriendDict.ContainsKey(id2))
                        heroFriendDict.Add(id2, new Dictionary<int, int>());
                    heroFriendDict[id2][id1] = Math.Max(heroFriendCfg.Level, heroFriendDict[id2].ContainsKey(id1) ? heroFriendDict[id2][id1] : 0);

                    // 记录该关系行配置的连线颜色（普通/特殊通用；同一对先配先得，未配置跳过）
                    AddFriendPairColor(id1, id2, heroFriendCfg.LineColor);

                    // 特殊连锁（配置了关联技能）：记录技能缩写与连线颜色
                    if (!string.IsNullOrEmpty(heroFriendCfg.SkillId))
                        AddFriendSpecialPair(id1, id2, heroFriendCfg.SkillId, heroFriendCfg.LineColor);

                }
                if (!heroFriendInfoDict.ContainsKey(friendIds[i]))
                    heroFriendInfoDict.Add(friendIds[i], new HashSet<int>());
                heroFriendInfoDict[friendIds[i]].Add(heroFriendCfg.Id);
            }
        }
    }

    // 双向记录一对英雄的特殊连锁关联技能缩写与连线颜色（同一对保留先配置的）
    private static void AddFriendSpecialPair(int id1, int id2, string skillSname, string lineColor)
    {
        if (!heroFriendSkillDict.ContainsKey(id1))
            heroFriendSkillDict.Add(id1, new Dictionary<int, string>());
        if (!heroFriendSkillDict[id1].ContainsKey(id2))
        {
            heroFriendSkillDict[id1][id2] = skillSname;
            if (!heroFriendColorDict.ContainsKey(id1))
                heroFriendColorDict.Add(id1, new Dictionary<int, string>());
            heroFriendColorDict[id1][id2] = lineColor;
        }

        if (!heroFriendSkillDict.ContainsKey(id2))
            heroFriendSkillDict.Add(id2, new Dictionary<int, string>());
        if (!heroFriendSkillDict[id2].ContainsKey(id1))
        {
            heroFriendSkillDict[id2][id1] = skillSname;
            if (!heroFriendColorDict.ContainsKey(id2))
                heroFriendColorDict.Add(id2, new Dictionary<int, string>());
            heroFriendColorDict[id2][id1] = lineColor;
        }
    }

    // 记录一对英雄的连线颜色（未配置跳过；同一对保留先配置的）
    private static void AddFriendPairColor(int id1, int id2, string lineColor)
    {
        if (string.IsNullOrEmpty(lineColor))
            return;
        if (!heroFriendColorDict.ContainsKey(id1))
            heroFriendColorDict.Add(id1, new Dictionary<int, string>());
        if (!heroFriendColorDict[id1].ContainsKey(id2))
            heroFriendColorDict[id1][id2] = lineColor;
        if (!heroFriendColorDict.ContainsKey(id2))
            heroFriendColorDict.Add(id2, new Dictionary<int, string>());
        if (!heroFriendColorDict[id2].ContainsKey(id1))
            heroFriendColorDict[id2][id1] = lineColor;
    }

    // 好友连锁·特殊：返回两个英雄之间的关联技能缩写（非空表示特殊连线，空表示普通连线）
    public static string GetFriendSkillId(int heroId, int friendId)
    {
        if (heroFriendSkillDict.TryGetValue(heroId, out Dictionary<int, string> value))
            return value.ContainsKey(friendId) ? value[friendId] : "";
        return "";
    }

    // 好友连锁·特殊：返回两个英雄之间特殊连线的颜色（未配置则返回空字符串）
    public static string GetFriendLineColor(int heroId, int friendId)
    {
        if (heroFriendColorDict.TryGetValue(heroId, out Dictionary<int, string> value))
            return value.ContainsKey(friendId) ? value[friendId] : "";
        return "";
    }

    public static int GetFriendLevel(int heroId, int friendId)
    {
        if (heroFriendDict.TryGetValue(heroId, out Dictionary<int, int> value))
        {   
            return value.ContainsKey(friendId) ? value[friendId] : 0;
        }
        return 0;
    }
    
    public static int GetShowHelpSkillId(int heroId, int targetHeroId, int srcPos, int targetPos)
    {
        var heroCfg = HeroConfig.GetConfig(heroId);
        foreach (var skillCfg in GetHeroSkillConfigs(heroCfg))
        {
            if (skillCfg.UnitHelpType <= 0)
                continue;

            var targetHeroCfg = HeroConfig.GetConfig(targetHeroId);
            var tarJobCfg = ConfigManager.GetJobConfig(targetHeroCfg.Job);
            var targetHasSkill = false;
            foreach (var tSkillCfg in GetHeroSkillConfigs(targetHeroCfg))
            {
                if (tSkillCfg.Sname == skillCfg.Sname)
                {
                    targetHasSkill = true;
                    break;
                }
            }
            if (targetHasSkill || (skillCfg.HelpSkillJob != "" && !skillCfg.HelpSkillJob.Contains(tarJobCfg.NameS)))
                continue;

            if (skillCfg.UnitHelpType == 1 && srcPos / 3 == targetPos / 3)
                return skillCfg.Id;
            else if (skillCfg.UnitHelpType == 2 && ((srcPos % 3) == (targetPos % 3)))
                return skillCfg.Id;
            // else if (skillCfg.UnitHelpType == 3)
            //     return skill;
        }

        return 0;
    }

    public static HashSet<int> GetHeroFriendInfo(int heroId)
    {
        if (heroFriendInfoDict.TryGetValue(heroId, out HashSet<int> value))
        {
            return value;
        }
        return null;
    }

    // 英雄是否属于指定技能缩写的好友羁绊组（道具使用限制等按技能缩写判定时复用，如万民书限定「仁」）
    public static bool HeroHasFriendSkill(int heroId, string sname)
    {
        if (string.IsNullOrEmpty(sname))
            return true;

        var relIds = GetHeroFriendInfo(heroId);
        if (relIds == null)
            return false;

        foreach (var relId in relIds)
        {
            var relCfg = HeroFriendConfig.GetConfig(relId);
            if (relCfg != null && relCfg.SkillId == sname)
                return true;
        }
        return false;
    }

    // 英雄技能列表：职业兵种技能 + 个人技能(Skill1)，按缩写去重。
    // 默认取各缩写的1级行（界面显示统一1级）。战斗创建后按来源修正：
    // 个人技能按卡片等级、兵种技能由JobLinkManager、好友特殊技能由FriendLineManager 各自 SetLevel 匹配 Sname+等级 的行。
    // 配置中未登记的缩写（如技能表缺失）跳过并告警，避免绑定固定Id时抛异常。
    public static List<SkillConfig> GetHeroSkillConfigs(HeroConfig heroCfg)
    {
        var list = new List<SkillConfig>();
        var jobCfg = GetJobConfig(heroCfg.Job);
        if (jobCfg != null)
            AddHeroSkillCfg(list, jobCfg.SkillId);
        AddHeroSkillCfg(list, heroCfg.Skill1);
        return list;
    }

    private static void AddHeroSkillCfg(List<SkillConfig> list, string sname)
    {
        if (string.IsNullOrEmpty(sname))
            return;
        foreach (var c in list)
        {
            if (c.Sname == sname)
                return; // 同一技能只保留一个，避免重复实例
        }
        var cfg = GetSkillConfig(sname, 1);
        if (cfg == null)
        {
            GameLog.Warn($"技能缩写[{sname}]未在SkillConfig中配置，暂不加入英雄技能");
            return;
        }
        list.Add(cfg);
    }

    public static bool IsHeroCard(int cardId)
    {
        return cardId < 200000;
    }

    public static JobConfig GetJobConfig(string jobName)
    {
        if (jobDict.TryGetValue(jobName, out JobConfig value))
        {
            return value;
        }
        return null;
    }

    // 势力配置：按阵营Id(HeroConfig.Side)取，未登记返回 null
    public static ForceConfig GetForceConfig(int side)
    {
        return ForceConfig.HasConfig(side) ? ForceConfig.GetConfig(side) : null;
    }

    // 势力主公英雄Id（未登记返回0）
    public static int GetKingHeroId(int side)
    {
        var forceCfg = GetForceConfig(side);
        return forceCfg != null ? forceCfg.KingId : 0;
    }

    // 是否某势力的主公（王）：主公Id登记在 ForceConfig.KingId
    public static bool IsKingHero(int heroId)
    {
        foreach (var forceCfg in ForceConfig.ConfigList)
        {
            if (forceCfg.KingId == heroId)
                return true;
        }
        return false;
    }

    public static SkillConfig GetSkillConfig(string skillName)
    {
        if (skillDict.TryGetValue(skillName, out SkillConfig value))
        {
            return value;
        }
        return null;
    }

    // 按 缩写+等级 取技能配置：同一技能在 SkillConfig 表按 Sname 展开为 Lv1~5 多行，
    // 战斗/界面（兵种连锁档位、好友特殊连线、技能等级切换）按需定位到具体等级行；未配置对应等级行返回 null。
    public static SkillConfig GetSkillConfig(string sname, int lv)
    {
        foreach (var skillCfg in SkillConfig.ConfigList)
        {
            if (skillCfg.Sname == sname && skillCfg.Lv == lv)
                return skillCfg;
        }
        return null;
    }

    // 技能完整描述：Lv1 的 Descript 为模板（占位符/1 /2...），用"当前等级的 DescriptVal"按顺序替换拼出；
    // DescriptVal 支持 Lv1 填字段引用（动态字段）作为模板：/字段名（如 /StrengthInt、/Range、/Strength、/Rate），
    // 各级用对应字段值替换（+30 等）。仅需在 Lv1 填字段引用模板，其余等级留空自动推导；也可在某级手动填 DescriptVal（保底，直接使用）。
    // 结构无法用模板表达的技能（如机巧/炮车）各等级行直接填完整 Descript，无占位符时原样返回。
    // withNext=true 为"当前+下一级"模式：每个参数位后附下一级不同值（括号内、淡绿色，相同则不附）；供职业/好友连接技能档位提示使用
    public static string GetSkillDescript(SkillConfig cfg, bool withNext = false)
    {
        if (cfg == null)
            return "";
        var templateCfg = cfg;
        if (string.IsNullOrEmpty(cfg.Descript))
            templateCfg = GetSkillConfig(cfg.Sname, 1) ?? cfg;
        if (templateCfg == null || string.IsNullOrEmpty(templateCfg.Descript))
            return "";
        // 当前等级有效的 DescriptVal（Lv1 模板 + 字段引用替换，或手填保底）
        var curVal = GetLevelDescriptVal(cfg, templateCfg);
        if (string.IsNullOrEmpty(curVal))
            return templateCfg.Descript; // 无参数：模板即完整文案（全同文案/结构兜底技能）

        string[] nextVals = null;
        if (withNext)
        {
            var nextCfg = GetSkillConfig(cfg.Sname, cfg.Lv + 1);
            if (nextCfg != null)
            {
                var nextVal = GetLevelDescriptVal(nextCfg, templateCfg);
                if (!string.IsNullOrEmpty(nextVal))
                    nextVals = nextVal.Split(';');
            }
        }

        var vals = curVal.Split(';');
        var desc = templateCfg.Descript;
        for (int i = 0; i < vals.Length; i++)
        {
            var cur = vals[i];
            var rep = cur;
            if (nextVals != null && i < nextVals.Length && nextVals[i] != cur)
                rep = cur + SysColor.ColorText("(" + nextVals[i] + ")", SysColor.UI.NextLv);
            desc = desc.Replace("/" + (i + 1), rep);
        }
        return desc;
    }

    // 取某等级行有效的 DescriptVal：
    // 1) 非 Lv1 模板行且已手动填写 -> 直接使用（保底）；
    // 2) 否则用 Lv1 模板的 DescriptVal，把其中 /字段名 动态引用替换为该等级行对应字段值
    private static string GetLevelDescriptVal(SkillConfig cfg, SkillConfig templateCfg)
    {
        bool isTemplateRow = cfg.Lv == templateCfg.Lv; // Lv1 行的 DescriptVal 即字段引用模板
        if (!isTemplateRow && !string.IsNullOrEmpty(cfg.DescriptVal))
            return cfg.DescriptVal;
        if (templateCfg == null || string.IsNullOrEmpty(templateCfg.DescriptVal))
            return null;
        return SubstituteFieldRefs(templateCfg.DescriptVal, cfg);
    }

    // 把 DescriptVal 中的 "/字段名" 动态引用替换为 cfg 对应字段的数值（字段名大小写不敏感）；非字母开头的斜杠(如/1 /2占位)不动
    private static string SubstituteFieldRefs(string input, SkillConfig cfg)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        var sb = new StringBuilder();
        for (int i = 0; i < input.Length;)
        {
            if (input[i] == '/' && i + 1 < input.Length && char.IsLetter(input[i + 1]))
            {
                int j = i + 1;
                while (j < input.Length && (char.IsLetterOrDigit(input[j]) || input[j] == '_'))
                    j++;
                var fieldName = input.Substring(i + 1, j - i - 1);
                // 可选百分比修饰符：/字段名% 表示该数字字段按百分比输出（如 strength=0.3 -> 30%）
                bool pct = i < input.Length && j < input.Length && input[j] == '%';
                int adv = j + (pct ? 1 : 0);
                var val = GetFieldRefValue(cfg, fieldName, pct);
                if (val != null)
                    sb.Append(val);
                else
                    sb.Append('/').Append(fieldName); // 未知字段保持原样
                i = adv;
            }
            else
            {
                sb.Append(input[i]);
                i++;
            }
        }
        return sb.ToString();
    }

    // 动态字段取值与格式化：Rate/Strength2 转百分比，其余数值字段按整数字面展示（无小数）
    private static string GetFieldRefValue(SkillConfig cfg, string fieldName, bool pct)
    {
        switch (fieldName.ToLowerInvariant())
        {
            case "rate": return PercentText(cfg.Rate);
            case "strength2": return PercentText(cfg.Strength2);
            case "cd": return pct ? PercentText(cfg.CD) : cfg.CD.ToString("0.##");
            case "range": return pct ? PercentText(cfg.Range) : cfg.Range.ToString("0.##");
            case "area": return pct ? PercentText(cfg.Area) : cfg.Area.ToString("0.##");
            case "strength": return pct ? PercentText(cfg.Strength) : cfg.Strength.ToString("0.##");
            case "bufftime": return pct ? PercentText(cfg.BuffTime) : cfg.BuffTime.ToString("0.##");
            case "summontime": return pct ? PercentText(cfg.SummonTime) : cfg.SummonTime.ToString("0.##");
            case "summonspeed": return pct ? PercentText(cfg.SummonSpeed) : cfg.SummonSpeed.ToString("0.##");
            case "effectsize": return pct ? PercentText(cfg.EffectSize) : cfg.EffectSize.ToString("0.##");
            case "attackpointreduce": return pct ? PercentText(cfg.AttackPointReduce) : cfg.AttackPointReduce.ToString("0.##");
            case "mpcost": return cfg.MpCost.ToString();
            case "targetcount": return cfg.TargetCount.ToString();
            case "strengthint": return cfg.StrengthInt.ToString();
            case "lv": return cfg.Lv.ToString();
            default: return null;
        }
    }

    private static string PercentText(float v)
    {
        return (v * 100).ToString("0.##") + "%";
    }
}
