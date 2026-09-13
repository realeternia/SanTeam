using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 技能类，处理技能相关逻辑
/// </summary>

public class Skill
{
    public int id;
    public Chess owner;
    public bool isGivenSkill; //别人给的技能
    public SkillConfig skillCfg;
    private float lastUpdateTime; // 上次更新CD的时间
    public bool isBurst;

    /// <summary>
    /// 有效技能等级：默认取配置等级(skillCfg.Lv=5)，由连锁机制(兵种连锁/好友连锁·特殊)修正
    /// </summary>
    public int Level;

    public int skillId{ get{ return skillCfg.Id; } }

    public float mp; // 当前技能MP，战斗开始为0，满值=MpCost

    /// <summary>
    /// 统一技能伤害公式：固定系数(Strength) + 比例系数(SkillDamageAttrRate) × 关联属性(IsMagic映射 ap/atk)
    /// IsMagic=true→ap 法强(受魔抗减免)；false→atk 攻击(无双已并入，受护甲减免)
    /// </summary>
    public int GetSkillDamage()
    {
        if(skillCfg.IsMagic)
            return (int)(skillCfg.Strength + owner.GetAttr("ap") * skillCfg.SkillDamageAttrRate);
        else
            return (int)(skillCfg.Strength + owner.GetAttr("atk") * skillCfg.SkillDamageAttrRate);
    }

    /// <summary>
    /// 技能类型限定判断：CheckTypeLimit=false 不限定；否则 CheckIsMagic 与目标技能是否法术比对（true=仅法术 / false=仅物理）
    /// </summary>
    public static bool TypeMatched(SkillConfig cfg, bool isMagic)
    {
        return !cfg.CheckTypeLimit || cfg.CheckIsMagic == isMagic;
    }

    public Skill(int id, Chess unit)
    {
        this.id = id;
        this.owner = unit;

        skillCfg = SkillConfig.GetConfig(id);
        Level = skillCfg.Lv;
    }

    /// <summary>
    /// 设置技能等级（连锁机制：兵种连锁/好友连锁·特殊）
    /// 实际起效的配置 = 同一Sname组内 level 匹配的那一行（未命中时回退组内配置）
    /// </summary>
    public void SetLevel(int lv)
    {
        Level = lv;
        var newCfg = ConfigManager.GetSkillConfig(skillCfg.Sname, lv);
        if (newCfg != null)
            skillCfg = newCfg;
    }

    /// <summary>
    /// 更新技能CD时间
    /// </summary>
    public void UpdateCD()
    {
        if (skillCfg.CD > 0)
        {
            if (IsInCD())
            {
                return;
            }

            var cdTime = skillCfg.CD;
            SkillManager.OnCheckCD(owner, skillCfg, ref cdTime);

            lastUpdateTime = Time.time - skillCfg.CD + cdTime;
        }
    }

    /// <summary>
    /// 检查技能是否在CD中
    /// </summary>
    /// <returns>如果在CD中返回true，否则返回false</returns>
    public bool IsInCD()
    {
        if(skillCfg.CD <= 0)
            return false;

        return Time.time < lastUpdateTime + skillCfg.CD;
    }

    // 每次行动（攻击）为技能充能：增加MpCost/3，3次行动充满；达到MpCost后不再增加
    public void AddActionMp()
    {
        if (skillCfg.MpCost <= 0)
            return;
        mp = Mathf.Min(mp + skillCfg.MpCost / 3f, skillCfg.MpCost);
    }

    // 法力回复属性（mpRegen）为技能持续充能：正=回复，负=倒扣，范围[0, MpCost]
    public void AddRegenMp(float add)
    {
        if (skillCfg.MpCost <= 0)
            return;
        mp = Mathf.Clamp(mp + add, 0f, skillCfg.MpCost);
    }

    // MP是否已满（未设置MpCost的技能不受MP限制）
    public bool IsMpFull()
    {
        return skillCfg.MpCost <= 0 || mp >= skillCfg.MpCost;
    }

    public bool CheckBurst(Chess target)
    {
        // 设置了MpCost的技能：MP未满时无法发动
        if (!IsMpFull())
        {
            isBurst = false;
            return false;
        }

        var rate = skillCfg.Rate;
        if (rate > 0 && rate < 1 && target != null && target != owner)
        {
            // 不再按双方属性差值放大/缩小发动概率，仅保留其他技能/机制对概率的修正(如百出)
            SkillManager.OnCheckBurst(owner, skillCfg, ref rate);
        }

        isBurst = !IsInCD() && (skillCfg.Rate <= 0 || SysRandom.Value < rate);

        // 触发条件不满足（如 hprate<50=自身生命低于50%）则本次不触发技能
        if (isBurst && !MeetTriggerCondition())
            isBurst = false;

        GameLog.Debug("CheckBurst isBurst=" + isBurst.ToString() + " skillId=" + id.ToString());
        if(isBurst)
        {
            UpdateCD();
            mp = 0; // 发动技能后清空MP
        }
        return isBurst;
    }

    // 技能触发条件是否满足（TriggerCondition为空表示无条件触发）
    // 支持格式：键+比较符+数值，如 hprate<50=自身生命低于50%；多条件用;分隔，全部满足才可触发
    private bool MeetTriggerCondition()
    {
        var condStr = skillCfg.TriggerCondition;
        if (string.IsNullOrEmpty(condStr))
            return true;

        foreach (var seg in condStr.Split(';'))
        {
            var s = seg.Trim();
            if (s.Length == 0)
                continue;

            // 拆分比较符（先匹配多字符 <= >= == !=，再匹配单字符 < >）
            var op = "";
            var opIdx = -1;
            for (var i = 0; i < s.Length; i++)
            {
                var two = i + 1 < s.Length ? s.Substring(i, 2) : "";
                if (two == "<=" || two == ">=" || two == "==" || two == "!=")
                {
                    op = two;
                    opIdx = i;
                    break;
                }
                var one = s[i].ToString();
                if (one == "<" || one == ">")
                {
                    op = one;
                    opIdx = i;
                    break;
                }
            }
            if (op == "" || opIdx <= 0)
            {
                GameLog.Warn("无法解析技能触发条件：" + s + "，技能id=" + skillCfg.Id);
                return false; // 配置异常时不触发，避免条件形同虚设
            }

            float val;
            if (!float.TryParse(s.Substring(opIdx + op.Length).Trim(), out val))
            {
                GameLog.Warn("技能触发条件数值无效：" + s + "，技能id=" + skillCfg.Id);
                return false;
            }

            var key = s.Substring(0, opIdx).Trim();
            // 条件键取值统一走 Chess.GetAttr（hprate=自身生命百分比0~100），不在技能侧重复维护属性映射
            int cur;
            try
            {
                cur = owner.GetAttr(key);
            }
            catch (Exception)
            {
                GameLog.Warn("未知的技能触发条件键：" + key + "，技能id=" + skillCfg.Id);
                return false; // 配置异常时不触发，避免条件形同虚设
            }

            var satisfied = false;
            switch (op)
            {
                case "<": satisfied = cur < val; break;
                case "<=": satisfied = cur <= val; break;
                case ">": satisfied = cur > val; break;
                case ">=": satisfied = cur >= val; break;
                case "==": satisfied = Mathf.Approximately(cur, val); break;
                case "!=": satisfied = !Mathf.Approximately(cur, val); break;
            }
            if (!satisfied)
                return false;
        }
        return true;
    }

    public virtual void BattleBegin()
    {

    }

    public virtual void AimTarget(Chess target)
    {

    }

    public virtual void OnAttack(Chess defender, int damage)
    {
    }

    public virtual void OnAttacked(Chess attacker, int damage)
    {
    }

    public virtual void DuringAttack(Chess defender, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual void DuringAttacked(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual bool CheckAidSkill()
    {
        return false;
    }

    public virtual void OnCheckBurst(SkillConfig checkSkillCfg, ref float rate)
    {
        
    }

    public virtual void OnAddBuff(Chess target, ref int buffId, int skillId, ref float time)
    {
        
    }

    public virtual void OnCheckCD(SkillConfig checkSkillCfg, ref float cdTime)
    {

    }

    public virtual void OnBeAddBuff(Chess caster, ref int buffId, int checkSkillId, ref float time)
    {
        
    }

    public virtual void BeforeCalDamage(Chess target, SkillConfig checkSkillCfg, ref int damage, string hurtTag, bool isFeedback)
    {
        
    }

    public virtual void BeforeCalDamaged(Chess caster, SkillConfig checkSkillCfg, ref int damage, string hurtTag, bool isFeedback)
    {
        
    }

    /// <summary>
    /// 护甲修正增量：物理伤害结算时，对受击方护甲的倍率增量（最终倍率 = 1 + Σ各技能增量；0=不改变护甲）。
    /// 多个技能按加法叠加：破甲类(攻击方, isAttackerSide=true)返回负值（如 -0.3 → 护甲×0.7，完全无视=返回-1）；
    /// 加甲类(受击方, isAttackerSide=false)返回正值（如 0.2 → 护甲×1.2）。
    /// 注意：不能把"Strength 即生效"做进默认实现，否则扇(ModifyBuffTime)/速射(ModifyShootSpeed)等
    /// 无关技能因 Strength>0 会让持有者凭空获得护甲加成，必须显式覆写。
    /// </summary>
    public virtual float GetArmorDelta(bool isAttackerSide)
    {
        return 0f;
    }

    public virtual void OnHealTarget(Chess target, int checkSkillId, ref int addon)
    {
        
    }

    public virtual void OnCheckSummonTime(SkillConfig checkSkillCfg, ref float summonTime)
    {

    }

    /// <summary>
    /// 召唤技能单位（法术场/分身等）：在指定位置创建单位，并将技能配置的 SummonTag 标记到召唤物上
    /// </summary>
    protected Chess SummonUnit(Vector3 pos, int soldierId, string imgPath = "")
    {
        var unit = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), soldierId, -1, pos, owner.side, imgPath);
        if (!string.IsNullOrEmpty(skillCfg.SummonTag))
            unit.SummonTag = skillCfg.SummonTag;
        return unit;
    }

    /// <summary>
    /// 召唤技能法术场(501001)：创建并标记 SummonTag、设置存在时长，返回召唤物与存在时长（时长同时用于技能特效生命周期）
    /// </summary>
    protected Chess SummonMagicField(Vector3 pos, out float summonTime)
    {
        var magicStub = SummonUnit(pos, CombatConst.SoldierMagicField);
        summonTime = GetSummonTime();
        magicStub.SetLifeTime(summonTime);
        return magicStub;
    }

    public float GetSummonTime()
    {
        var summonTime = skillCfg.SummonTime;
        SkillManager.OnCheckSummonTime(owner, skillCfg, ref summonTime);
        return summonTime;
    }

}
