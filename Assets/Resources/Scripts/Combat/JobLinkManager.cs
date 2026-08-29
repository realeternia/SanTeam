using System.Collections.Generic;
using CommonConfig;

/// <summary>
/// 兵种连锁：战斗开始时统计本侧同兵种(HeroConfig.Job)英雄数量，
/// 提升该兵种默认技能(JobConfig.SkillId)等级。
/// 规则：默认兵种技能1级，每多一个同兵种英雄 +1 级（等级 = 同兵种英雄数）。
/// 档位/基础等级统一维护在 CombatConst。
/// </summary>
public static class JobLinkManager
{
    public static void ApplyJobLinks()
    {
        var handledSides = new HashSet<int>();
        foreach (var player in GameManager.Instance.players)
        {
            if (!handledSides.Add(player.battleSide))
                continue;
            ApplyJobLinks(player.battleSide);
        }
    }

    private static void ApplyJobLinks(int side)
    {
        var units = WorldManager.Instance.GetUnitsMySide(side);
        if (units.Count == 0)
            return;

        // 统计各兵种英雄数量
        var jobCount = new Dictionary<string, int>();
        foreach (var unit in units)
        {
            if (!unit.isHero || unit.hp <= 0)
                continue;
            var job = HeroConfig.GetConfig(unit.heroId).Job;
            if (!jobCount.ContainsKey(job))
                jobCount[job] = 0;
            jobCount[job]++;
        }

        foreach (var kv in jobCount)
        {
            var jobCfg = ConfigManager.GetJobConfig(kv.Key);
            if (jobCfg == null || string.IsNullOrEmpty(jobCfg.SkillId))
                continue;

            // 默认1级，每多一个同兵种英雄 +1 级
            var skillLevel = CombatConst.JobLinkBaseLevel + (kv.Value - 1);
            foreach (var unit in units)
            {
                if (!unit.isHero || unit.hp <= 0)
                    continue;
                if (HeroConfig.GetConfig(unit.heroId).Job != kv.Key)
                    continue;

                foreach (var skill in unit.skills)
                {
                    if (skill.skillCfg.Sname == jobCfg.SkillId)
                    {
                        skill.SetLevel(skillLevel);
                        UnityEngine.Debug.Log($"JobLink 兵种{kv.Key} 英雄数{kv.Value} 兵种技能{skill.skillCfg.Sname} 等级{skillLevel}");
                    }
                }
            }
        }
    }
}
