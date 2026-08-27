public class SkillInitMasterShield : Skill
{
    public SkillInitMasterShield(int id, Chess chess) : base(id, chess)
    {
        // 初始化护盾
    }

    public override void BattleBegin()
    {
        // 主公技效果已改为：主公所在同阵营护盾效果加倍，由 FactionShieldManager 统一处理
    }
}