using CommonConfig;
using UnityEngine;

public class SkillInitAttrZhiheng : Skill
{
    public SkillInitAttrZhiheng(int id, Chess chess) : base(id, chess)
    {
        // 初始化护盾
    }

    public override void BattleBegin()
    {
        // 获取我方所有单位
        var unitList = WorldManager.Instance.GetUnitsMySide(owner.transform.position, 0, owner.side);
        
        // 统计我方hero的不同Job数量
        System.Collections.Generic.HashSet<string> uniqueJobs = new System.Collections.Generic.HashSet<string>();
        
        foreach (var unit in unitList)
        {
            if (unit.hp > 0 && unit.isHero && unit != owner)
            {
                // 添加hero的Job到HashSet中以自动去重
                var heroCfg = HeroConfig.GetConfig(unit.heroId);
                uniqueJobs.Add(heroCfg.Job);
            }
        }
        
        // 根据不同Job数量确定属性提升值
        int attrValue = 0;
        int uniqueJobCount = uniqueJobs.Count;
        
        if (uniqueJobCount >= 5)
        {
            attrValue = 30;
        }
        else if (uniqueJobCount >= 4)
        {
            attrValue = 20;
        }
        else if (uniqueJobCount >= 3)
        {
            attrValue = 10;
        }

        // 如果有属性提升，则提升owner的属性
        if (attrValue > 0)
        {
            // 提升主要属性，可以根据游戏设计选择合适的属性类型
            // 这里参考SkillHitAttr的实现方式
            owner.AddAttr("str", attrValue); // 力量属性
            owner.AddAttr("leadShip", attrValue); // 力量属性
            owner.AddAttr("inte", attrValue); // 智力属性
            UnityEngine.Debug.Log($"BattleBegin SkillInitAttrZhiheng: 我方有{uniqueJobCount}种不同兵种，提升属性{attrValue}点");

            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
            owner.PlayerAnim(skillCfg.Action);
            WorldManager.Instance.AddBattleText("制衡+" + attrValue.ToString(), owner.transform.position, new UnityEngine.Vector2(0, 60), new Color(1, .3f, .3f), 3);
        }
    }
}