using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        ShopConfig.Load();
        PlayerConfig.Load();
        HeroFriendConfig.Load();
        PlayerLevelConfig.Load();
        FormulaLearnAttrConfig.Load();
        JobConfig.Load();
        HeroAttrConfig.Load();
        SystemAttrConfig.Load();

        ConfigManager.PostModify();      

        UnityEngine.Debug.Log("ConfigManager Init fin");
    }

    public static void PostModify()
    {
        var jobNameConvDict = new Dictionary<string, string>();
        foreach (var jobCfg in JobConfig.ConfigList)
        {
            jobDict.Add(jobCfg.Name, jobCfg);
            jobDict.Add(jobCfg.NameS, jobCfg);
            jobNameConvDict[jobCfg.NameS] = jobCfg.Name;
        }      
        foreach (var skillCfg in SkillConfig.ConfigList)
        {
            // 每个技能在配置表中展开为1-5级多行，按缩写去重时优先取5级作为代表（数值各级相同）
            if (!skillDict.ContainsKey(skillCfg.Sname) || skillCfg.Lv == 5)
                skillDict[skillCfg.Sname] = skillCfg;
        }

        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            heroCfg.Job = jobNameConvDict.ContainsKey(heroCfg.Job) ? jobNameConvDict[heroCfg.Job] : heroCfg.Job;
            // 技能不再在此绑定为固定Id：同一技能会因等级不同映射到不同行Id，
            // 改由战斗/界面按 Sname+等级 实时解析（GetHeroSkillConfigs / SkillConfig.GetConfig(sname, lv)）
        }


        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            // 移速/射程/攻速/护甲/魔抗/攻击/法术/无双：HeroConfig 为 0 时使用职业基准值，非 0 时与职业值相加
            var jobCfg = GetJobConfig(heroCfg.Job);
            if (jobCfg != null)
            {
                heroCfg.MoveSpeed = heroCfg.MoveSpeed == 0 ? jobCfg.MoveSpeed : heroCfg.MoveSpeed + jobCfg.MoveSpeed;
                heroCfg.Range = heroCfg.Range == 0 ? jobCfg.Range : heroCfg.Range + jobCfg.Range;
                heroCfg.AtkSpeed = heroCfg.AtkSpeed == 0 ? jobCfg.AtkSpeed : heroCfg.AtkSpeed + jobCfg.AtkSpeed;
                heroCfg.Armor = heroCfg.Armor == 0 ? jobCfg.Armor : heroCfg.Armor + jobCfg.Armor;
                heroCfg.MagicRes = heroCfg.MagicRes == 0 ? jobCfg.MagicRes : heroCfg.MagicRes + jobCfg.MagicRes;
                heroCfg.Atk = heroCfg.Atk == 0 ? jobCfg.Atk : heroCfg.Atk + jobCfg.Atk;
                heroCfg.Ap = heroCfg.Ap == 0 ? jobCfg.Ap : heroCfg.Ap + jobCfg.Ap;
                heroCfg.Might = heroCfg.Might == 0 ? jobCfg.Might : heroCfg.Might + jobCfg.Might;
            }
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
            Debug.Log($"创建{f.id} / {f.name} 配对: {string.Join(",", f.friendIds.Select(id => HeroConfig.GetConfig(id).Name))}");
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

    // 英雄技能列表：职业兵种技能 + 个人技能(Skill1/Skill2)，按缩写去重。
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
        AddHeroSkillCfg(list, heroCfg.Skill2);
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
        var cfg = SkillConfig.GetConfig(sname, 1);
        if (cfg == null)
        {
            UnityEngine.Debug.LogWarning($"技能缩写[{sname}]未在SkillConfig中配置，暂不加入英雄技能");
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

    public static SkillConfig GetSkillConfig(string skillName)
    {
        if (skillDict.TryGetValue(skillName, out SkillConfig value))
        {
            return value;
        }
        return null;
    }
}
