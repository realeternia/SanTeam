using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SkillConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///缩写
        /// </summary>
        public string Sname;
        /// <summary>
        ///说明
        /// </summary>
        public string Descript;
        /// <summary>
        ///分类
        /// </summary>
        public string Type;
        /// <summary>
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///发动概率
        /// </summary>
        public float Rate;
        /// <summary>
        ///发动cd
        /// </summary>
        public float CD;
        /// <summary>
        ///条件参数
        /// </summary>
        public float ConditionParm;
        /// <summary>
        ///相关属性
        /// </summary>
        public string Attr;
        /// <summary>
        ///属性高于修正
        /// </summary>
        public float RateAttrH;
        /// <summary>
        ///属性高于修正系数
        /// </summary>
        public float RateAttrHP;
        /// <summary>
        ///范围
        /// </summary>
        public float Range;
        /// <summary>
        ///范围外
        /// </summary>
        public bool RangeOut;
        /// <summary>
        ///选取点
        /// </summary>
        public string TargetType;
        /// <summary>
        ///最大目标数
        /// </summary>
        public int TargetCount;
        /// <summary>
        ///技能强度
        /// </summary>
        public float Strength;
        /// <summary>
        ///计数次数
        /// </summary>
        public int DoCount;
        /// <summary>
        ///计数延迟
        /// </summary>
        public float TimeDelay;
        /// <summary>
        ///BuffId
        /// </summary>
        public int BuffId;
        /// <summary>
        ///BuffLast
        /// </summary>
        public float BuffTime;
        /// <summary>
        ///法术场数
        /// </summary>
        public int SummonCount;
        /// <summary>
        ///法术场范围
        /// </summary>
        public float SummonArea;
        /// <summary>
        ///法术场持续
        /// </summary>
        public float SummonTime;
        /// <summary>
        ///法术场间隔
        /// </summary>
        public float SummonHitInterval;
        /// <summary>
        ///法术场速度
        /// </summary>
        public float SummonSpeed;
        /// <summary>
        ///脚本名
        /// </summary>
        public string ScriptName;
        /// <summary>
        ///动作
        /// </summary>
        public string Action;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///size
        /// </summary>
        public float EffectSize;
        /// <summary>
        ///价值
        /// </summary>
        public int Price;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;


        public SkillConfig(int Id, string Name, string Sname, string Descript, string Type, int Lv, float Rate, float CD, float ConditionParm, string Attr, float RateAttrH, float RateAttrHP, float Range, bool RangeOut, string TargetType, int TargetCount, float Strength, int DoCount, float TimeDelay, int BuffId, float BuffTime, int SummonCount, float SummonArea, float SummonTime, float SummonHitInterval, float SummonSpeed, string ScriptName, string Action, string HitEffect, float EffectSize, int Price, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Descript = Descript;
            this.Type = Type;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.ConditionParm = ConditionParm;
            this.Attr = Attr;
            this.RateAttrH = RateAttrH;
            this.RateAttrHP = RateAttrHP;
            this.Range = Range;
            this.RangeOut = RangeOut;
            this.TargetType = TargetType;
            this.TargetCount = TargetCount;
            this.Strength = Strength;
            this.DoCount = DoCount;
            this.TimeDelay = TimeDelay;
            this.BuffId = BuffId;
            this.BuffTime = BuffTime;
            this.SummonCount = SummonCount;
            this.SummonArea = SummonArea;
            this.SummonTime = SummonTime;
            this.SummonHitInterval = SummonHitInterval;
            this.SummonSpeed = SummonSpeed;
            this.ScriptName = ScriptName;
            this.Action = Action;
            this.HitEffect = HitEffect;
            this.EffectSize = EffectSize;
            this.Price = Price;
            this.Icon = Icon;

        }

        public SkillConfig() { }

        private static Dictionary<int, SkillConfig> config = new Dictionary<int, SkillConfig>();
        public static Dictionary<int, SkillConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, SkillConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[200001] = new SkillConfig(200001, "王", "帅", "给与我方同阵营单位20%生命值护盾", "职业", 1, 0, 0, 0, "", 0, 0, 0, false, "", 0, 0.2f, 0, 0, 300001, 999f, 0, 0, 0, 0, 0, "MasterShield", "", "", 0, 4, "shuai");
            config[200002] = new SkillConfig(200002, "羽扇", "扇", "远程攻击", "职业", 1, 1f, 99f, 0, "inte", 0, 0, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Dumb", "", "", 0, 2, "shan");
            config[200003] = new SkillConfig(200003, "刀兵", "刀", "攻击几率造成1.5倍伤害", "职业", 1, 0.1f, 5f, 0, "leadShip", 0.1f, 0.01f, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "CriticalAttack", "jump", "SwordHitRedCritical", 0, 1, "dao");
            config[200004] = new SkillConfig(200004, "坚韧", "士", "受击时几率发动减伤", "职业", 1, 0.4f, 4.5f, 0, "str", 0, 0, 0, false, "", 0, 0.5f, 0, 0, 300002, 4f, 0, 0, 0, 0, 0, "AttackedBuff", "spin", "", 0, 2, "shi");
            config[200005] = new SkillConfig(200005, "突破", "马", "移动时穿越敌人,造成额外伤害", "职业", 1, 1f, 7f, 0, "leadShip", 0, 0, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "RunCross", "", "LightningExplosionBlue", 0, 2, "ma");
            config[200006] = new SkillConfig(200006, "运筹", "相", "提升士兵等级", "职业", 1, 0, 0, 0, "inte", 0, 0, 0, false, "", 0, 1f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "SoldierUp", "", "MagicChargeYellow", 0, 2, "xiang");
            config[200007] = new SkillConfig(200007, "弓手", "弓", "远程射击单位", "职业", 1, 1f, 99f, 0, "leadShip", 0, 0, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Dumb", "", "", 0, 2, "gong");
            config[200008] = new SkillConfig(200008, "谋略", "谋", "一定几率晕眩目标单位2s", "职业", 1, 0.15f, 4f, 0, "inte", 0.2f, 0.01f, 0, false, "", 0, 0, 0, 0, 301001, 2f, 0, 0, 0, 0, 0, "HitBuff", "throw", "MagicChargeYellow", 0, 2, "mou");
            config[200009] = new SkillConfig(200009, "炮车", "炮", "攻击目标发生爆炸", "职业", 1, 0.5f, 0, 0, "leadShip", 0, 0, 20f, false, "", 3, 0.6f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "HitArea", "throw", "MagicNovaYellow", 0, 3, "pao");
            config[200010] = new SkillConfig(200010, "弩手", "弩", "射程非常远", "职业", 1, 1f, 99f, 0, "leadShip", 0, 0, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Dumb", "", "", 0, 3, "nu");
            config[200011] = new SkillConfig(200011, "冲锋", "车", "移动时穿越敌人,降低目标防御", "职业", 1, 1f, 7f, 0, "leadShip", 0, 0, 0, false, "", 0, 0, 0, 0, 301003, 3f, 0, 0, 0, 0, 0, "RunCrossPlus", "", "LightningExplosionRed", 0, 2, "che");
            config[200012] = new SkillConfig(200012, "声乐", "乐", "给与友军攻速祝福", "职业", 1, 1f, 3f, 0, "inte", 0, 0, 50f, false, "", 0, 0.7f, 0, 0, 301005, 5f, 0, 0, 0, 0, 0, "HelpAidBuff", "sway", "MagicChargePink", 0, 2, "song");
            config[200013] = new SkillConfig(200013, "治疗", "医", "给与友军治疗", "职业", 1, 1f, 3f, 0, "inte", 0, 0, 50f, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "HelpHeal", "sway", "MagicBuffGreen", 0, 2, "heal");
            config[200014] = new SkillConfig(200014, "枪阵", "枪", "一定几率晕眩目标单位2.5s", "职业", 1, 0.15f, 4f, 0, "leadShip", 0.23f, 0.01f, 0, false, "", 0, 0, 0, 0, 301001, 2.5f, 0, 0, 0, 0, 0, "HitBuff", "spin", "MagicChargeYellow", 0, 2, "qiang");
            config[200015] = new SkillConfig(200015, "戟阵", "戟", "攻击目标发生爆炸", "职业", 1, 0.5f, 4f, 0, "leadShip", 0, 0, 25f, false, "", 2, 0.6f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "HitAround", "throw", "SwordSlashMiniWhite", 0, 2, "ji");
            config[200016] = new SkillConfig(200016, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 1, 1f, 3f, 0, "inte", 0, 0, 50f, false, "", 0, 0.35f, 0, 0, 301004, 5f, 0, 0, 0, 0, 0, "HelpAidBuff", "sway", "MagicChargeYellow", 0, 2, "gu");
            config[201006] = new SkillConfig(201006, "连射", "连", "攻击时几率触发连射", "", 1, 0.3f, 5f, 0, "leadShip", 0, 0, 0, false, "", 0, 0.8f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "SpeedAttack", "", "", 0, 2, "lian");
            config[201008] = new SkillConfig(201008, "箭雨", "雨", "攻击时30%发出2只箭", "", 1, 0.3f, 4f, 0, "leadShip", 0, 0, 25f, false, "", 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "MultiArrow", "", "", 0, 3, "duo");
            config[201010] = new SkillConfig(201010, "旋风斩", "猛", "攻击时几率对附近敌人造成伤害", "", 1, 0.4f, 5f, 0, "str", 0, 0, 25f, false, "", 99, 0.8f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "SpinAttack", "spin", "SwordWhirlwindWhite", 0, 3, "meng");
            config[201011] = new SkillConfig(201011, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "", 1, 0.3f, 10f, 0, "inte", 0, 0, 0, false, "", 1, 0.1f, 0, 0, 0, 0, 0, 25f, 5.2f, 0.5f, 0, "HitRegion", "spin", "SummonStorm", 5f, 4, "tian");
            config[201012] = new SkillConfig(201012, "火墙", "炎", "攻击召唤出持续伤害的火墙", "", 1, 0.3f, 8f, 0, "inte", 0, 0, 0, false, "", 99, 0.15f, 0, 0, 0, 0, 5, 8f, 3.2f, 1f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, 4, "yan");
            config[201013] = new SkillConfig(201013, "地震波", "波", "召唤出冲击波攻击前方敌人", "", 1, 1f, 7f, 0, "str", 0, 0, 50f, false, "", 99, 0.3f, 0, 0, 0, 0, 0, 12f, 1.5f, 0, 40f, "ShockWave", "spin", "NukeMissileFires", 3f, 2, "bo");
            config[201017] = new SkillConfig(201017, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", 1, 0.25f, 2f, 1f, "inte", 0.25f, 0.01f, 0, false, "", 0, 0.2f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "DamageReal", "sway", "", 0, 3, "gui");
            config[201018] = new SkillConfig(201018, "斩", "斩", "直接杀死低生命值单位", "", 1, 0, 7f, 0.3f, "str", 0, 0, 0, false, "", 0, 1f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "DamageReal", "spin", "SwordHitRedCritical", 0, 3, "zhan");
            config[201019] = new SkillConfig(201019, "豪杰", "豪", "攻击时回复生命", "", 1, 0.35f, 6f, 0, "str", 0, 0, 0, false, "", 0, 0.5f, 0, 0, 300003, 5f, 0, 0, 0, 0, 0, "AttackedBuff", "", "MagicBuffGreen", 0, 2, "hao");
            config[201022] = new SkillConfig(201022, "埋伏", "伏", "被攻击时,瞬移到远程攻击者附近", "", 1, 1f, 6f, 0, "leadShip", 0, 0, 30f, false, "", 0, 0, 0, 0, 301001, 1f, 0, 0, 0, 0, 0, "HitTeleport", "", "MagicNovaBlue", 0, 1, "fu");
            config[201025] = new SkillConfig(201025, "火矢", "矢", "攻击时射出火箭", "", 1, 0.35f, 3f, 0, "str", 0, 0, 0, false, "", 1, 0.2f, 0, 0, 0, 0, 1, 8f, 3.2f, 1f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, 2, "shi3");
            config[201027] = new SkillConfig(201027, "虎卫队", "虎", "攻击时几率对目标进行三连击", "", 1, 0.15f, 6f, 0, "leadShip", 0.2f, 0.005f, 0, false, "", 0, 1f, 2, 0.3f, 0, 0, 0, 0, 0, 0, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0, 4, "hu");
            config[201028] = new SkillConfig(201028, "青州兵", "青", "攻击时几率对目标进行2连击", "", 1, 0.15f, 7f, 0, "leadShip", 0.2f, 0.01f, 0, false, "", 0, 1f, 1, 0.4f, 0, 0, 0, 0, 0, 0, 0, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0, 3, "qing");
            config[202001] = new SkillConfig(202001, "刺甲", "刺", "反弹50%近战伤害", "", 1, 0, 0, 0, "str", 0, 0, 20f, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Feedback", "", "SwordHitBlue", 0, 2, "ci");
            config[202002] = new SkillConfig(202002, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", 1, 0.7f, 1f, 0, "str", 0, 0, 0, false, "", 0, 0.7f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "PlantSkin", "", "", 0, 3, "teng");
            config[202003] = new SkillConfig(202003, "明镜", "镜", "反弹50%智力伤害", "", 1, 0, 0, 0, "inte", 0, 0, 20f, true, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Feedback", "", "SwordHitBlue", 0, 2, "jing");
            config[202004] = new SkillConfig(202004, "护卫", "护", "给与友军护盾祝福", "", 1, 1f, 8f, 0, "str", 0, 0, 50f, false, "", 0, 0.18f, 0, 0, 300001, 10f, 0, 0, 0, 0, 0, "HelpAidBuff", "sway", "MagicChargeYellow", 0, 1, "hu1");
            config[203002] = new SkillConfig(203002, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "", 1, 0.5f, 3f, 0, "inte", 0, 0, 25f, false, "targetUnit", 1, 0.5f, 0, 0, 301002, 8f, 0, 0, 0, 0, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0, 4, "suo");
            config[203003] = new SkillConfig(203003, "深谋", "谋2", "一定几率晕眩多个单位2s", "", 1, 0.15f, 4f, 0, "inte", 0.15f, 0.01f, 25f, false, "targetUnit", 2, 0, 0, 0, 301001, 2f, 0, 0, 0, 0, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0, 4, "mou2");
            config[203004] = new SkillConfig(203004, "威震", "威", "攻击时晕眩周围目标", "", 1, 0.2f, 5f, 0, "str", 0, 0, 20f, false, "castUnit", 3, 0, 0, 0, 301001, 1.5f, 0, 0, 0, 0, 0, "HitBuffArea", "spin", "MagicNovaYellow", 0, 4, "wei");
            config[203005] = new SkillConfig(203005, "击破", "破", "使目标增伤40%", "", 1, 0.4f, 2f, 0, "str", 0, 0, 0, false, "", 0, 0.4f, 0, 0, 301003, 3f, 0, 0, 0, 0, 0, "HitBuff", "jump", "SoftFireBigRed", 0, 2, "po");
            config[203006] = new SkillConfig(203006, "影分队", "影", "产生一个幻影军队", "", 1, 0.35f, 15f, 0, "leadShip", 0, 0, 15f, false, "", 0, 0.4f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "AttackedShadow", "", "MagicFieldGreen", 0, 3, "ying");
            config[208001] = new SkillConfig(208001, "百出", "百", "降低法术CD时间", "智技up", 1, 0, 0, 0, "inte", 0, 0, 0, false, "", 0, 0.3f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "ModifySkillRateTime", "", "", 0, 2, "bai");
            config[208002] = new SkillConfig(208002, "神算", "神", "提升法术命中率和持续时间", "智技up", 1, 0.3f, 0, 0, "inte", 0, 0, 0, false, "", 0, 0.5f, 0, 0, 0, 0.5f, 0, 0, 0, 0, 0, "ModifySkillRateTime", "", "", 0, 3, "shen");
            config[209001] = new SkillConfig(209001, "富甲", "商", "参战获得5金币", "", 1, 0, 0, 0, "inte", 0, 0, 0, false, "", 0, 5f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Gold", "", "MagicChargeYellow", 0, 3, "gold");
            config[209002] = new SkillConfig(209002, "国士", "国", "增加一个远程士兵,提升射程", "", 1, 0, 0, 0, "inte", 0, 0, 15f, false, "", 0, 10f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "SoldierSummon", "", "MagicChargeGreen", 0, 4, "guo");
            config[209003] = new SkillConfig(209003, "指导", "师", "提升队伍最低武将智力", "", 1, 0, 0, 0, "inte", 0, 0, 0, false, "", 0, 0.6f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "HelpTeach", "", "MagicChargeYellow", 0, 2, "shi2");
            config[209004] = new SkillConfig(209004, "学习", "学", "攻击时几率提升自己的属性", "", 1, 0.3f, 0, 0, "inte", 0, 0, 0, false, "", 0, 0.05f, 0, 0, 0, 0, 0, 0, 0, 0, 0, "HitAttr", "", "MagicChargeYellow", 0, 3, "zhang");

        }

        public static SkillConfig GetConfig(int id)
        {
            SkillConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SkillConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SkillConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SkillConfig configData)
        {
            if (!config.ContainsKey(id))
            {
                config.Add(id, configData);
            }
        }

        public static void Remove(int id)
        {
            if (config.ContainsKey(id))
            {
                config.Remove(id);
            }
        }
    }
}
