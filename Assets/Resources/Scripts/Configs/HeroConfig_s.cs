﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroConfig
    {
        public class FieldMetaInfo
        {
            public string fieldName;
            public string fieldType;
            public int fieldWidth;
            public string fieldRule;
            public bool fieldIndex;
            public FieldMetaInfo(string name, string type, int width = 0, string rule = "", bool index = false)
            {
                fieldName = name;
                fieldType = type;
                fieldWidth = width;
                fieldRule = rule;
                fieldIndex = index;
            }
        }

        public class CellMeta
        {
            public int row;
            public int col;
            public int? foreColor;
            public int? backColor;
            public CellMeta(int row, int col, int? foreColor, int? backColor)
            {
                this.row = row;
                this.col = col;
                this.foreColor = foreColor;
                this.backColor = backColor;
            }
        }

        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()
        {
            {"Id", new FieldMetaInfo("序列", "int", 60)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 60)},
            {"Job", new FieldMetaInfo("职业", "string", 0)},
            {"Scenario1", new FieldMetaInfo("剧本1·群雄割据（品质:阵营，如3:蜀；空=该剧本不出场）", "string", 72, "qs,1:#666666,2:#33CC33,3:#3333FF,4:#FFD700")},
            {"Scenario2", new FieldMetaInfo("剧本2·三国鼎立（品质:阵营，如3:蜀；空=该剧本不出场）", "string", 72, "qs,1:#666666,2:#33CC33,3:#3333FF,4:#FFD700")},
            {"Scenario3", new FieldMetaInfo("剧本3·三家归晋（品质:阵营，如3:蜀；空=该剧本不出场）", "string", 68, "qs,1:#666666,2:#33CC33,3:#3333FF,4:#FFD700")},
            {"Atk", new FieldMetaInfo("攻击（0=职业基准，>0与职业相加）", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"Ap", new FieldMetaInfo("法术强度（0=职业基准，>0与职业相加）", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"MagicRate", new FieldMetaInfo("法术倍率（0=继承职业；非0时覆盖职业值。该英雄专属技能伤害/治疗×本值、英雄攻击÷本值）", "float", 60)},
            {"Hp", new FieldMetaInfo("生命", "int", 60)},
            {"AtkP", new FieldMetaInfo("攻击成长百分比（每星，如80=每星+80%）", "int", 60)},
            {"HpP", new FieldMetaInfo("生命成长百分比（每星）", "int", 60)},
            {"AtkSpeed", new FieldMetaInfo("攻速（0=职业基准，>0与职业相加；30=每秒攻击1次，攻速20=1.5秒/次，15=2秒/次）", "int", 83)},
            {"HpRegen", new FieldMetaInfo("生命回复/秒（0=职业基准，非0=相对职业基准±%）", "int", 60)},
            {"MpRegen", new FieldMetaInfo("法力获取加成（%，加法叠加到职业基准；如 +20=额外+20%）", "int", 60)},
            {"Armor", new FieldMetaInfo("护甲（0=职业基准，>0与职业相加）", "int", 60)},
            {"MagicRes", new FieldMetaInfo("魔抗（0=职业基准，>0与职业相加）", "int", 79)},
            {"MoveSpeed", new FieldMetaInfo("移动速度（0=职业默认，>0与职业相加）", "int", 81)},
            {"Range", new FieldMetaInfo("攻击距离（0=职业默认，>0与职业相加）", "int", 60)},
            {"Skill1", new FieldMetaInfo("技能", "string", 0)},
            {"Icon", new FieldMetaInfo("背景图", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///职业
        /// </summary>
        public string Job;
        /// <summary>
        ///剧本1·群雄割据（品质:阵营，如3:蜀；空=该剧本不出场）
        /// </summary>
        public string Scenario1;
        /// <summary>
        ///剧本2·三国鼎立（品质:阵营，如3:蜀；空=该剧本不出场）
        /// </summary>
        public string Scenario2;
        /// <summary>
        ///剧本3·三家归晋（品质:阵营，如3:蜀；空=该剧本不出场）
        /// </summary>
        public string Scenario3;
        /// <summary>
        ///攻击（0=职业基准，>0与职业相加）
        /// </summary>
        public int Atk;
        /// <summary>
        ///法术强度（0=职业基准，>0与职业相加）
        /// </summary>
        public int Ap;
        /// <summary>
        ///法术倍率（0=继承职业；非0时覆盖职业值。该英雄专属技能伤害/治疗×本值、英雄攻击÷本值）
        /// </summary>
        public float MagicRate;
        /// <summary>
        ///生命
        /// </summary>
        public int Hp;
        /// <summary>
        ///攻击成长百分比（每星，如80=每星+80%）
        /// </summary>
        public int AtkP;
        /// <summary>
        ///生命成长百分比（每星）
        /// </summary>
        public int HpP;
        /// <summary>
        ///攻速（0=职业基准，>0与职业相加；20=每秒攻击1次，40=每秒2次）
        /// </summary>
        public int AtkSpeed;
        /// <summary>
        ///生命回复/秒（0=职业基准，非0=相对职业基准±%）
        /// </summary>
        public int HpRegen;
        /// <summary>
        ///法力获取加成（%，加法叠加到职业基准值；普攻/受击获得法力时按此百分比提升）
        /// </summary>
        public int MpRegen;
        /// <summary>
        ///护甲
        /// </summary>
        public int Armor;
        /// <summary>
        ///魔抗（0=职业基准，>0与职业相加）
        /// </summary>
        public int MagicRes;
        /// <summary>
        ///移动速度（0=职业默认，>0与职业相加）
        /// </summary>
        public int MoveSpeed;
        /// <summary>
        ///攻击距离（0=职业默认，>0与职业相加）
        /// </summary>
        public int Range;
        /// <summary>
        ///技能
        /// </summary>
        public string Skill1;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public HeroConfig(int Id, string Name, int Lv, string Job, string Scenario1, string Scenario2, string Scenario3, int Atk, int Ap, float MagicRate, int Hp, int AtkP, int HpP, int AtkSpeed, int HpRegen, int MpRegen, int Armor, int MagicRes, int MoveSpeed, int Range, string Skill1, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Job = Job;
            this.Scenario1 = Scenario1;
            this.Scenario2 = Scenario2;
            this.Scenario3 = Scenario3;
            this.Atk = Atk;
            this.Ap = Ap;
            this.MagicRate = MagicRate;
            this.Hp = Hp;
            this.AtkP = AtkP;
            this.HpP = HpP;
            this.AtkSpeed = AtkSpeed;
            this.HpRegen = HpRegen;
            this.MpRegen = MpRegen;
            this.Armor = Armor;
            this.MagicRes = MagicRes;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.Skill1 = Skill1;
            this.Icon = Icon;
        }

        public HeroConfig() { }

        private static Dictionary<int, HeroConfig> config = new Dictionary<int, HeroConfig>();
        public static Dictionary<int, HeroConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, HeroConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[100001] = new HeroConfig(100001, "刘备", 1, "王", "3:蜀", "3:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "刘备", "liubei");
            config[100002] = new HeroConfig(100002, "曹操", 1, "王", "4:魏", "4:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "曹操", "caocao");
            config[100003] = new HeroConfig(100003, "孙权", 1, "王", "", "3:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "孙权", "sunquan");
            config[100004] = new HeroConfig(100004, "董卓", 1, "王", "3:董", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "董卓", "dongzhuo");
            config[100005] = new HeroConfig(100005, "司马炎", 1, "王", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "司马炎", "simayan");
            config[100006] = new HeroConfig(100006, "袁绍", 1, "王", "2:袁", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "袁绍", "yuanshao");
            config[100007] = new HeroConfig(100007, "刘璋", 1, "王", "1:璋", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "liuzhang");
            config[100008] = new HeroConfig(100008, "刘表", 1, "王", "2:表", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "liubiao");
            config[100009] = new HeroConfig(100009, "马腾", 1, "王", "2:马", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "mateng");
            config[101001] = new HeroConfig(101001, "赵云", 1, "士", "4:蜀", "4:蜀", "4:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "赵云", "zhaoyun");
            config[101002] = new HeroConfig(101002, "张飞", 1, "枪", "4:蜀", "4:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张飞", "zhangfei");
            config[101003] = new HeroConfig(101003, "马超", 1, "马", "4:马", "4:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "马超", "machao");
            config[101004] = new HeroConfig(101004, "诸葛亮", 1, "工", "", "4:蜀", "4:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "诸葛亮", "zhugeliang");
            config[101005] = new HeroConfig(101005, "关羽", 1, "车", "4:蜀", "4:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "关羽", "guanyu");
            config[101006] = new HeroConfig(101006, "徐庶", 1, "炮", "3:蜀", "2:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "徐庶", "xusu");
            config[101007] = new HeroConfig(101007, "魏延", 1, "戟", "2:表", "3:蜀", "3:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "魏延", "weiyan");
            config[101008] = new HeroConfig(101008, "黄忠", 1, "弓", "3:表", "3:蜀", "4:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "黄忠", "huangzhong");
            config[101009] = new HeroConfig(101009, "周仓", 1, "盾", "1:野", "1:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "周仓", "zhoucang");
            config[101010] = new HeroConfig(101010, "姜维", 1, "车", "", "4:蜀", "4:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "姜维", "jiangwei");
            config[101011] = new HeroConfig(101011, "马岱", 1, "弩", "2:马", "2:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "马岱", "madai");
            config[101012] = new HeroConfig(101012, "庞统", 1, "棋", "3:表", "4:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "庞统", "pangtong");
            config[101013] = new HeroConfig(101013, "李严", 1, "士", "2:璋", "2:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "李严", "liyan");
            config[101014] = new HeroConfig(101014, "张松", 1, "扇", "1:璋", "1:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张松", "zhangsong");
            config[101015] = new HeroConfig(101015, "蒋琬", 1, "相", "", "1:蜀", "1:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "蒋琬", "jiangwan");
            config[101016] = new HeroConfig(101016, "孙乾", 1, "鼓", "1:蜀", "1:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "孙乾", "sunqian");
            config[101017] = new HeroConfig(101017, "费祎", 1, "鼓", "", "2:蜀", "2:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "费祎", "feiyi");
            config[101018] = new HeroConfig(101018, "马谡", 1, "枪", "1:表", "1:蜀", "2:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "马谡", "masu");
            config[101019] = new HeroConfig(101019, "马良", 1, "相", "2:表", "2:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "马良", "maliang");
            config[101020] = new HeroConfig(101020, "法正", 1, "棋", "2:璋", "3:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "法正", "fazheng");
            config[100011] = new HeroConfig(100011, "刘禅", 1, "王", "", "", "1:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "刘禅", "liushan");
            config[101022] = new HeroConfig(101022, "严颜", 1, "盾", "2:璋", "2:蜀", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "严颜", "yanyan");
            config[101023] = new HeroConfig(101023, "黄月英", 1, "工", "", "2:蜀", "2:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "黄月英", "huangyueying");
            config[102001] = new HeroConfig(102001, "郭嘉", 1, "棋", "4:魏", "4:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "郭嘉", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯惇", 1, "车", "3:魏", "3:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "夏侯惇", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 1, "相", "4:魏", "4:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "荀彧", "xunyu");
            config[102004] = new HeroConfig(102004, "张辽", 1, "枪", "4:董", "4:魏", "4:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张辽", "zhangliao");
            config[102005] = new HeroConfig(102005, "许褚", 1, "锤", "4:魏", "4:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "许褚", "xuchu");
            config[102006] = new HeroConfig(102006, "夏侯渊", 1, "弓", "3:魏", "3:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "夏侯渊", "xiahouyuan");
            config[102007] = new HeroConfig(102007, "典韦", 1, "戟", "4:魏", "4:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "典韦", "dianwei");
            config[102008] = new HeroConfig(102008, "张郃", 1, "炮", "3:董", "3:魏", "4:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张郃", "zhanghe");
            config[102009] = new HeroConfig(102009, "徐晃", 1, "弩", "3:魏", "3:魏", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "徐晃", "xuhuang");
            config[102010] = new HeroConfig(102010, "荀攸", 1, "棋", "3:魏", "3:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "荀攸", "xunyou");
            config[102011] = new HeroConfig(102011, "于禁", 1, "盾", "2:魏", "2:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "于禁", "yujin");
            config[102012] = new HeroConfig(102012, "曹仁", 1, "盾", "3:魏", "3:魏", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "曹仁", "caoren");
            config[102013] = new HeroConfig(102013, "曹洪", 1, "锤", "2:魏", "2:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "曹洪", "caohong");
            config[102014] = new HeroConfig(102014, "庞德", 1, "士", "3:马", "3:魏", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "庞德", "pangde");
            config[102015] = new HeroConfig(102015, "乐进", 1, "车", "1:魏", "1:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "乐进", "lejin");
            config[102016] = new HeroConfig(102016, "司马懿", 1, "扇", "", "4:魏", "4:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "司马懿", "simayi");
            config[102017] = new HeroConfig(102017, "程昱", 1, "扇", "2:魏", "2:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "程昱", "chengyu");
            config[102018] = new HeroConfig(102018, "文鸯", 1, "弩", "", "2:魏", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "文鸯", "wenyuan");
            config[102019] = new HeroConfig(102019, "曹真", 1, "戟", "", "1:魏", "2:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "曹真", "caozhen");
            config[102020] = new HeroConfig(102020, "陈群", 1, "工", "1:魏", "1:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "陈群", "chenqun");
            config[102021] = new HeroConfig(102021, "李典", 1, "士", "2:魏", "2:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "李典", "lidian");
            config[102022] = new HeroConfig(102022, "刘晔", 1, "工", "2:魏", "2:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "刘晔", "liuye");
            config[102023] = new HeroConfig(102023, "曹彰", 1, "马", "", "3:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "曹彰", "caozhang");
            config[102024] = new HeroConfig(102024, "蔡文姬", 1, "琴", "", "2:魏", "2:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "蔡文姬", "caiyan");
            config[102025] = new HeroConfig(102025, "甄宓", 1, "琴", "", "1:魏", "1:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "甄宓", "zhenshi");
            config[103001] = new HeroConfig(103001, "孙策", 1, "戟", "4:吴", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "孙策", "sunce");
            config[100023] = new HeroConfig(100023, "孙坚", 1, "王", "4:吴", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "孙坚", "sunjian");
            config[103003] = new HeroConfig(103003, "甘宁", 1, "弩", "4:野", "4:吴", "4:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "甘宁", "ganning");
            config[103004] = new HeroConfig(103004, "太史慈", 1, "弓", "4:吴", "4:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "太史慈", "taishici");
            config[103005] = new HeroConfig(103005, "黄盖", 1, "炮", "2:吴", "2:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "黄盖", "huanggai");
            config[103006] = new HeroConfig(103006, "周泰", 1, "锤", "3:吴", "3:吴", "2:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "周泰", "zhoutai");
            config[103007] = new HeroConfig(103007, "鲁肃", 1, "鼓", "", "4:吴", "4:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "鲁肃", "lusu");
            config[103008] = new HeroConfig(103008, "周瑜", 1, "扇", "4:吴", "4:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "周瑜", "zhouyu");
            config[103009] = new HeroConfig(103009, "蒋钦", 1, "戟", "2:吴", "2:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "蒋钦", "jiangqing");
            config[103010] = new HeroConfig(103010, "吕蒙", 1, "盾", "", "3:吴", "4:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "吕蒙", "lvmeng");
            config[103011] = new HeroConfig(103011, "陆逊", 1, "炮", "", "4:吴", "4:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "陆逊", "luxun");
            config[103012] = new HeroConfig(103012, "张昭", 1, "相", "3:吴", "3:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张昭", "zhangzhao");
            config[103013] = new HeroConfig(103013, "诸葛瑾", 1, "鼓", "3:吴", "3:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "诸葛瑾", "zhugejin");
            config[103014] = new HeroConfig(103014, "孙尚香", 1, "弩", "3:吴", "3:吴", "3:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "孙尚香", "sunshangxiang");
            config[103015] = new HeroConfig(103015, "朱桓", 1, "马", "", "2:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "朱桓", "zhuhuan");
            config[103016] = new HeroConfig(103016, "大乔", 1, "琴", "2:吴", "2:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "大乔", "daqiao");
            config[103017] = new HeroConfig(103017, "小乔", 1, "琴", "2:吴", "2:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "小乔", "xiaoqiao");
            config[103018] = new HeroConfig(103018, "丁奉", 1, "炮", "", "1:吴", "1:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "丁奉", "dingfeng");
            config[103019] = new HeroConfig(103019, "凌统", 1, "弓", "", "1:吴", "1:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "凌统", "lingtong");
            config[103020] = new HeroConfig(103020, "潘璋", 1, "锤", "1:吴", "1:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "潘璋", "panzhang");
            config[103021] = new HeroConfig(103021, "徐盛", 1, "弓", "", "1:吴", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "徐盛", "xusheng");
            config[103022] = new HeroConfig(103022, "程普", 1, "车", "2:吴", "2:吴", "2:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "程普", "chengpu");
            config[103023] = new HeroConfig(103023, "韩当", 1, "马", "2:吴", "2:吴", "2:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "韩当", "handang");
            config[104001] = new HeroConfig(104001, "吕布", 1, "马", "4:董", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "吕布", "lvbu");
            config[104002] = new HeroConfig(104002, "华雄", 1, "马", "3:董", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "华雄", "huaxiong");
            config[104003] = new HeroConfig(104003, "贾诩", 1, "相", "3:董", "3:魏", "4:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "贾诩", "jiaxu");
            config[104004] = new HeroConfig(104004, "貂蝉", 1, "琴", "3:董", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "貂蝉", "diaochan");
            config[104006] = new HeroConfig(104006, "高顺", 1, "炮", "2:董", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "高顺", "gaoshun");
            config[104007] = new HeroConfig(104007, "李儒", 1, "扇", "3:董", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "李儒", "liru");
            config[104008] = new HeroConfig(104008, "陈宫", 1, "棋", "3:董", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "陈宫", "chengong");
            config[104009] = new HeroConfig(104009, "张绣", 1, "枪", "3:董", "3:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张绣", "zhangxiu");
            config[105001] = new HeroConfig(105001, "邓艾", 1, "盾", "", "", "4:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "邓艾", "dengai");
            config[105002] = new HeroConfig(105002, "司马师", 1, "鼓", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "司马师", "simashi");
            config[105003] = new HeroConfig(105003, "司马昭", 1, "扇", "", "", "3:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "司马昭", "simazhao");
            config[105005] = new HeroConfig(105005, "钟会", 1, "棋", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "钟会", "zhonghui");
            config[105006] = new HeroConfig(105006, "陈泰", 1, "马", "", "", "1:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "陈泰", "chentai");
            config[105007] = new HeroConfig(105007, "杜预", 1, "工", "", "", "1:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "杜预", "duyu");
            config[105008] = new HeroConfig(105008, "王濬", 1, "炮", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "王濬", "wangrui");
            config[105009] = new HeroConfig(105009, "王双", 1, "锤", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "王双", "wangshuang");
            config[106001] = new HeroConfig(106001, "颜良", 1, "车", "3:袁", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "颜良", "yanliang");
            config[106002] = new HeroConfig(106002, "文丑", 1, "戟", "3:袁", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "文丑", "wenchou");
            config[106003] = new HeroConfig(106003, "田丰", 1, "鼓", "3:袁", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "田丰", "tianfeng");
            config[106004] = new HeroConfig(106004, "鞠义", 1, "弩", "2:袁", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "鞠义", "juyi");
            config[106005] = new HeroConfig(106005, "许攸", 1, "扇", "2:袁", "2:魏", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "许攸", "xuyou");
            config[106006] = new HeroConfig(106006, "高览", 1, "弓", "1:袁", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "高览", "gaolan");
            config[106007] = new HeroConfig(106007, "沮授", 1, "相", "2:袁", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "沮授", "jushou");
            config[106008] = new HeroConfig(106008, "郭图", 1, "琴", "1:袁", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "郭图", "guotu");
            config[110002] = new HeroConfig(110002, "张任", 1, "弓", "2:璋", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张任", "zhangren");
            config[110003] = new HeroConfig(110003, "华佗", 1, "医", "2:野", "2:野", "2:野", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "华佗", "huatuo");
            config[110006] = new HeroConfig(110006, "于吉", 1, "医", "2:野", "2:野", "2:野", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "于吉", "yuji");
            config[110007] = new HeroConfig(110007, "张角", 1, "工", "3:野", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张角", "zhangjiao");
            config[110008] = new HeroConfig(110008, "张宝", 1, "医", "2:野", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张宝", "zhangbao2");
            config[110009] = new HeroConfig(110009, "张梁", 1, "士", "1:野", "", "", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "张梁", "zhangliang");
            config[101024] = new HeroConfig(101024, "孟获", 1, "锤", "", "", "2:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "孟获", "menghuo");
            config[110011] = new HeroConfig(110011, "左慈", 1, "医", "3:野", "3:野", "3:野", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "左慈", "zuoci");
            config[101025] = new HeroConfig(101025, "夏侯霸", 1, "马", "", "", "2:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "xiahouba");
            config[101026] = new HeroConfig(101026, "廖化", 1, "马", "", "1:蜀", "1:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "liaohua");
            config[102026] = new HeroConfig(102026, "郭淮", 1, "盾", "", "", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "guohuai");
            config[102027] = new HeroConfig(102027, "蒋济", 1, "扇", "", "", "2:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "jiangji");
            config[102028] = new HeroConfig(102028, "王凌", 1, "锤", "", "", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "wangling");
            config[102029] = new HeroConfig(102029, "毌丘俭", 1, "弓", "", "", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "guanqiujian");
            config[103024] = new HeroConfig(103024, "诸葛恪", 1, "扇", "", "1:吴", "3:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "zhugege");
            config[103025] = new HeroConfig(103025, "陆抗", 1, "士", "", "", "4:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "lukang");
            config[105010] = new HeroConfig(105010, "羊祜", 1, "相", "", "", "4:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "yangku");
            config[101029] = new HeroConfig(101029, "杨仪", 1, "相", "", "", "2:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "yangyi");
            config[101031] = new HeroConfig(101031, "谯周", 1, "棋", "", "", "1:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "qiaozhou");
            config[102030] = new HeroConfig(102030, "王昶", 1, "车", "", "", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "wangchang");
            config[102031] = new HeroConfig(102031, "文钦", 1, "锤", "", "", "1:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "wenqin");
            config[102032] = new HeroConfig(102032, "诸葛诞", 1, "戟", "", "", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "zhugedan");
            config[102033] = new HeroConfig(102033, "王基", 1, "炮", "", "", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "wangji");
            config[102034] = new HeroConfig(102034, "诸葛绪", 1, "马", "", "", "2:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "zhugexu");
            config[100012] = new HeroConfig(100012, "曹叡", 1, "王", "", "", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "caorui");
            config[103026] = new HeroConfig(103026, "张悌", 1, "士", "", "", "2:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "zhangdi");
            config[100013] = new HeroConfig(100013, "孙皓", 1, "王", "", "", "2:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "sunhao");
            config[105011] = new HeroConfig(105011, "贾充", 1, "相", "", "", "3:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "jiachong");
            config[105012] = new HeroConfig(105012, "王浑", 1, "弓", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "wanghun");
            config[105013] = new HeroConfig(105013, "张华", 1, "棋", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "zhanghua");
            config[105014] = new HeroConfig(105014, "裴秀", 1, "工", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "peixiu");
            config[105015] = new HeroConfig(105015, "司马攸", 1, "扇", "", "", "1:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "simayou");
            config[105017] = new HeroConfig(105017, "司马孚", 1, "士", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "simafu");
            config[105018] = new HeroConfig(105018, "司马伦", 1, "车", "", "", "1:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "simalun");
            config[105019] = new HeroConfig(105019, "荀顗", 1, "鼓", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "xunyi");
            config[101032] = new HeroConfig(101032, "祝融", 1, "弩", "", "", "3:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "zhurong");
            config[102036] = new HeroConfig(102036, "郭太后", 1, "琴", "", "", "3:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "guoshi");
            config[103029] = new HeroConfig(103029, "步骘", 1, "相", "", "2:吴", "3:吴", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "buzhi");
            config[105020] = new HeroConfig(105020, "辛宪英", 1, "棋", "", "", "3:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "xinxianying");
            config[101033] = new HeroConfig(101033, "关平", 1, "枪", "", "2:蜀", "3:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "guanping");
            config[101034] = new HeroConfig(101034, "关兴", 1, "戟", "", "", "3:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "guanxing");
            config[101035] = new HeroConfig(101035, "张苞", 1, "枪", "", "", "3:蜀", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "zhangbao");
            config[105021] = new HeroConfig(105021, "张春华", 1, "琴", "", "", "3:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "zhangchunhua");
            config[102037] = new HeroConfig(102037, "嵇康", 1, "琴", "", "", "2:魏", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "jikang");
            config[105022] = new HeroConfig(105022, "阮籍", 1, "棋", "", "", "2:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "ruanji");
            config[105023] = new HeroConfig(105023, "王元姬", 1, "琴", "", "", "3:晋", 0, 0, 0f, 0, 100, 100, 0, 0, 0, 0, 0, 0, 0, "", "wangyuanji");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static HeroConfig GetConfig(int id)
        {
            HeroConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, HeroConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, HeroConfig configData)
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
