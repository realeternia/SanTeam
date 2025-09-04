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
        ///说明
        /// </summary>
        public string Descript;
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
        ///法术场持续
        /// </summary>
        public float SummonTime;
        /// <summary>
        ///法术场间隔
        /// </summary>
        public float SummonHitInterval;
        /// <summary>
        ///脚本名
        /// </summary>
        public string ScriptName;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///价值
        /// </summary>
        public int Price;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;


        public SkillConfig(int Id, string Name, string Descript, int Lv, float Rate, float CD, string Attr, float RateAttrH, float RateAttrHP, float Range, bool RangeOut, int TargetCount, float Strength, int DoCount, float TimeDelay, int BuffId, float BuffTime, int SummonCount, float SummonTime, float SummonHitInterval, string ScriptName, string HitEffect, int Price, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Descript = Descript;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.Attr = Attr;
            this.RateAttrH = RateAttrH;
            this.RateAttrHP = RateAttrHP;
            this.Range = Range;
            this.RangeOut = RangeOut;
            this.TargetCount = TargetCount;
            this.Strength = Strength;
            this.DoCount = DoCount;
            this.TimeDelay = TimeDelay;
            this.BuffId = BuffId;
            this.BuffTime = BuffTime;
            this.SummonCount = SummonCount;
            this.SummonTime = SummonTime;
            this.SummonHitInterval = SummonHitInterval;
            this.ScriptName = ScriptName;
            this.HitEffect = HitEffect;
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
            config[200002] = new SkillConfig(200002, "愤怒一击", "攻击几率造成1.5倍伤害", 1, 0.15f, 5f, "leadShip", 0.25f, 0.01f, 0, false, 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "CriticalAttack", "SwordHitRedCritical", 3, "che");
            config[200003] = new SkillConfig(200003, "主公技", "给与我方同阵营单位20%生命值护盾", 1, 0, 0, "", 0, 0, 0, false, 0, 0.2f, 0, 0, 300001, 999f, 0, 0, 0, "MasterShield", "", 4, "shuai");
            config[200004] = new SkillConfig(200004, "坚韧", "受击时几率发动减伤", 1, 0.4f, 4.5f, "str", 0, 0, 0, false, 0, 0.5f, 0, 0, 300002, 4f, 0, 0, 0, "AttackedBuff", "", 2, "shi");
            config[200005] = new SkillConfig(200005, "突破", "移动时穿越敌人,降低远程伤害", 1, 1f, 7f, "", 0, 0, 20f, false, 0, 0.1f, 0, 0, 0, 0, 0, 0, 0, "RunCross", "LightningExplosionBlue", 3, "ma");
            config[200006] = new SkillConfig(200006, "部署", "提升士兵等级", 1, 0, 0, "", 0, 0, 0, false, 0, 1f, 0, 0, 0, 0, 0, 0, 0, "SoldierUp", "MagicChargeYellow", 2, "xiang");
            config[200007] = new SkillConfig(200007, "射击", "射程很远", 1, 1f, 99f, "", 0, 0, 0, false, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Dumb", "", 2, "gong");
            config[200008] = new SkillConfig(200008, "决胜", "一定几率晕眩目标单位2s", 1, 0.15f, 4f, "inte", 0.2f, 0.01f, 0, false, 0, 0, 0, 0, 301001, 2f, 0, 0, 0, "HitBuff", "MagicChargeYellow", 2, "mou");
            config[201001] = new SkillConfig(201001, "治疗", "给与友军治疗", 1, 1f, 3f, "", 0, 0, 50f, false, 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "Heal", "MagicBuffGreen", 2, "heal");
            config[201002] = new SkillConfig(201002, "鼓舞", "给与友军攻速祝福", 1, 1f, 3f, "", 0, 0, 50f, false, 0, 0.4f, 0, 0, 0, 0, 0, 0, 0, "Song", "MagicChargePink", 2, "song");
            config[201003] = new SkillConfig(201003, "富甲一方", "参战获得5金币", 1, 0, 0, "", 0, 0, 0, false, 0, 5f, 0, 0, 0, 0, 0, 0, 0, "Gold", "MagicChargeYellow", 3, "gold");
            config[201004] = new SkillConfig(201004, "刺甲", "反弹40%近战伤害", 1, 0, 0, "", 0, 0, 20f, false, 0, 0.4f, 0, 0, 0, 0, 0, 0, 0, "Feedback", "SwordHitBlue", 2, "ci");
            config[201005] = new SkillConfig(201005, "诸葛弩", "射程非常远", 1, 1f, 99f, "", 0, 0, 0, false, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Dumb", "", 3, "nu");
            config[201006] = new SkillConfig(201006, "连射", "攻击时几率触发连射", 1, 0.3f, 5f, "", 0, 0, 0, false, 0, 0.8f, 0, 0, 0, 0, 0, 0, 0, "SpeedAttack", "", 3, "lian");
            config[201007] = new SkillConfig(201007, "藤甲", "受非智力攻击时高几率发动70%减伤", 1, 0.7f, 1f, "", 0, 0, 0, false, 0, 0.7f, 0, 0, 0, 0, 0, 0, 0, "PlantSkin", "", 3, "teng");
            config[201008] = new SkillConfig(201008, "多重箭", "攻击时40%发出2只箭", 1, 0.4f, 4f, "", 0, 0, 25f, false, 1, 0, 0, 0, 0, 0, 0, 0, 0, "MultiArrow", "", 3, "duo");
            config[201009] = new SkillConfig(201009, "指导", "提升队伍最低武将智力", 1, 0, 0, "", 0, 0, 0, false, 0, 0.4f, 0, 0, 0, 0, 0, 0, 0, "Help", "MagicChargeYellow", 2, "shi2");
            config[201010] = new SkillConfig(201010, "旋风斩", "攻击时几率对附近敌人造成伤害", 1, 0.4f, 5f, "", 0, 0, 25f, false, 99, 0.8f, 0, 0, 0, 0, 0, 0, 0, "SpinAttack", "SwordWhirlwindWhite", 3, "meng");
            config[201011] = new SkillConfig(201011, "落雷", "攻击召唤出持续伤害的雷电阵", 1, 0.3f, 10f, "inte", 0, 0, 25f, false, 1, 0.1f, 0, 0, 0, 0, 0, 5.2f, 0.5f, "HitRegion", "SummonStorm", 4, "tian");
            config[201012] = new SkillConfig(201012, "火墙", "攻击召唤出持续伤害的火墙", 1, 0.3f, 8f, "inte", 0, 0, 8f, false, 99, 0.15f, 0, 0, 0, 0, 5, 3.2f, 1f, "HitWall", "SoftFireBigRed", 4, "yan");
            config[201013] = new SkillConfig(201013, "神算", "高几率晕眩目标单位2s", 1, 0.2f, 3.5f, "inte", 0.55f, 0.01f, 0, false, 0, 0, 0, 0, 301001, 2f, 0, 0, 0, "HitBuff", "MagicChargeYellow", 5, "shen");
            config[201014] = new SkillConfig(201014, "连锁", "锁定敌人目标,传递一半受到伤害", 1, 0.5f, 3f, "inte", 0, 0, 25f, false, 1, 0.5f, 0, 0, 301002, 8f, 0, 0, 0, "HitBuffArea", "", 3, "suo");
            config[201015] = new SkillConfig(201015, "深谋", "一定几率晕眩多个单位2s", 1, 0.15f, 4f, "inte", 0.15f, 0.01f, 25f, false, 2, 0, 0, 0, 301001, 2f, 0, 0, 0, "HitBuffArea", "", 4, "mou2");
            config[201016] = new SkillConfig(201016, "强军", "提升士兵2级", 1, 0, 0, "", 0, 0, 0, false, 0, 2f, 0, 0, 0, 0, 0, 0, 0, "SoldierUp", "MagicChargeYellow", 3, "xiang2");
            config[201017] = new SkillConfig(201017, "鬼谋", "攻击几率造成1.3倍伤害,防御几率降低30%伤害", 1, 0.15f, 1f, "inte", 0.15f, 0.01f, 0, false, 0, 0.3f, 0, 0, 0, 0, 0, 0, 0, "AtkDefRate", "", 3, "gui");
            config[201018] = new SkillConfig(201018, "国士", "增加一个远程士兵,提升射程", 1, 0, 0, "", 0, 0, 15f, false, 0, 10f, 0, 0, 0, 0, 0, 0, 0, "SoldierSummon", "MagicChargeGreen", 3, "guo");
            config[201019] = new SkillConfig(201019, "豪杰", "攻击时回复生命", 1, 0.35f, 8f, "", 0, 0, 0, false, 0, 0.5f, 0, 0, 300003, 5f, 0, 0, 0, "AttackedBuff", "MagicBuffGreen", 3, "hao");
            config[201020] = new SkillConfig(201020, "分兵抗敌", "产生一个幻影军队", 1, 0.35f, 15f, "", 0, 0, 15f, false, 0, 0.4f, 0, 0, 0, 0, 0, 0, 0, "AttackedShadow", "MagicFieldGreen", 3, "huan");
            config[201021] = new SkillConfig(201021, "突进", "移动时穿越敌人,降低远程伤害", 1, 1f, 7f, "", 0, 0, 20f, false, 0, 0.12f, 0, 0, 0, 0, 0, 0, 0, "RunCrossPlus", "LightningExplosionBlue", 4, "ma2");
            config[201022] = new SkillConfig(201022, "埋伏", "被攻击时,瞬移到远程攻击者附近", 1, 0.4f, 4f, "", 0, 0, 30f, false, 0, 0, 0, 0, 301001, 1f, 0, 0, 0, "HitTeleport", "MagicNovaBlue", 2, "fu");
            config[201023] = new SkillConfig(201023, "明镜", "反弹50%智力伤害", 1, 0, 0, "inte", 0, 0, 20f, true, 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "Feedback", "SwordHitBlue", 3, "jing");
            config[201024] = new SkillConfig(201024, "威震", "攻击时晕眩周围目标", 1, 0.2f, 5f, "", 0, 0, 20f, false, 3, 0, 0, 0, 301001, 1.5f, 0, 0, 0, "HitBuffAreaSrc", "MagicNovaYellow", 4, "wei");
            config[201025] = new SkillConfig(201025, "火矢", "攻击时射出火箭", 1, 0.35f, 3f, "str", 0, 0, 8f, false, 1, 0.2f, 0, 0, 0, 0, 1, 3.2f, 1f, "HitWall", "SoftFireBigRed", 3, "shi3");
            config[201026] = new SkillConfig(201026, "坚毅", "受击时几率发动大减伤", 1, 0.4f, 6f, "str", 0, 0, 0, false, 0, 0.75f, 0, 0, 300002, 5f, 0, 0, 0, "AttackedBuff", "", 3, "shi4");
            config[201027] = new SkillConfig(201027, "虎卫队", "攻击时几率对目标进行三连击", 1, 0.15f, 5f, "str", 0.2f, 0.005f, 0, false, 0, 1f, 2, 0.3f, 0, 0, 0, 0, 0, "HitRepeat", "SwordHitRedCritical", 4, "hu");
            config[201028] = new SkillConfig(201028, "青州兵", "攻击时几率对目标进行2连击", 1, 0.15f, 5.5f, "leadShip", 0.2f, 0.01f, 0, false, 0, 1f, 1, 0.4f, 0, 0, 0, 0, 0, "HitRepeat", "SwordHitGreenCritical", 3, "qing");
            config[201029] = new SkillConfig(201029, "成长", "攻击时几率提升自己的属性", 1, 0.3f, 0, "", 0, 0, 0, false, 0, 0.05f, 0, 0, 0, 0, 0, 0, 0, "HitAttr", "MagicChargeYellow", 3, "zhang");
            config[201030] = new SkillConfig(201030, "击破", "使目标增伤20%", 1, 0.3f, 2f, "", 0, 0, 0, false, 0, 0.2f, 0, 0, 301003, 3f, 0, 0, 0, "HitBuff", "SoftFireBigRed", 2, "po");
            config[201031] = new SkillConfig(201031, "炮车", "攻击目标发生爆炸", 1, 0.5f, 0, "", 0, 0, 20f, false, 3, 0.6f, 0, 0, 0, 0, 0, 0, 0, "HitArea", "MagicNovaYellow", 3, "pao");

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
