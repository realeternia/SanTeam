using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ItemConfig
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
            {"Des", new FieldMetaInfo("效果说明", "string", 220)},
            {"Quality", new FieldMetaInfo("品质", "int", 60)},
            {"Effect", new FieldMetaInfo("效果", "string", 0)},
            {"Attrs", new FieldMetaInfo("属性加成", "string", 210)},
            {"MainAttr", new FieldMetaInfo("主属性（该道具的主导属性名，取值见 HeroAttrConfig.name，如 atk/hp/ap/armor/magicres/atkspeed/crit/mpRegen；玩家级道具用其自身属性名，无属性留空）", "string", 0)},
            {"SkillId", new FieldMetaInfo("技能（Sname缩写，空=无技能效果）", "string", 0)},
            {"RefSkillSname", new FieldMetaInfo("指代技能（好友羁绊技能缩写，代友道具专用；空=无）", "string", 0)},
            {"LimitSkillSname", new FieldMetaInfo("使用限制技能（空=不限；目标英雄需属于该技能的好友羁绊组）", "string", 0)},
            {"RemoveWhenUse", new FieldMetaInfo("使用后消失", "bool", 0)},
            {"UseSound", new FieldMetaInfo("使用音效（Resources/Sounds 下的文件名，如 metalhit；空=不播放，仅在使用消耗时播放）", "string", 0)},
            {"CombineId", new FieldMetaInfo("合成目标", "int", 60)},
            {"CombineNeed", new FieldMetaInfo("合成数量", "int", 60)},
            {"DropStartRound", new FieldMetaInfo("开始掉落回合数（当前回合数 ≥ 该值时该道具进入掉落池）", "int", 60)},
            {"DropWeight", new FieldMetaInfo("掉落权重（相对权重，≤0=不参与掉落）", "int", 60)},
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
        ///效果说明
        /// </summary>
        public string Des;
        /// <summary>
        ///品质（1白 2绿 3蓝 4紫，同英雄品质）
        /// </summary>
        public int Quality;
        /// <summary>
        ///效果
        /// </summary>
        public string Effect;
        /// <summary>
        ///属性加成（Attrs 格式：逗号分隔多条 "attr+value,attr+value"；比例属性如攻速/暴击直接配比例，0.1=+10%；禁止用空格分隔）
        /// </summary>
        public string Attrs;
        /// <summary>
        ///主属性（该道具的主导属性名，取值见 HeroAttrConfig.name，如 atk/hp/ap/armor/magicres/atkspeed/crit/mpRegen；玩家级道具用其自身属性名，无属性留空）
        /// </summary>
        public string MainAttr;
        /// <summary>
        ///技能（Sname缩写，空=无技能效果）
        /// </summary>
        public string SkillId;
        /// <summary>
        ///指代技能（代友道具专用：装备后视为 SkillId 缩写对应羁绊组的在场1人，参与人数计算、连线与关联技能授予）
        /// </summary>
        public string RefSkillSname;
        /// <summary>
        ///使用限制技能（空=不限；仅可对属于该技能好友羁绊组的英雄卡使用）
        /// </summary>
        public string LimitSkillSname;
        /// <summary>
        ///使用后消失
        /// </summary>
        public bool RemoveWhenUse;
        /// <summary>
        ///使用音效（Resources/Sounds 下的文件名，如 metalhit；空=不播放；仅在使用消耗时播放）
        /// </summary>
        public string UseSound;
        /// <summary>
        ///合成目标（0=不参与合成；达到合成数量时自动合成1个该道具）
        /// </summary>
        public int CombineId;
        /// <summary>
        ///合成数量（合成1个目标所需的本道具数量）
        /// </summary>
        public int CombineNeed;
        /// <summary>
        ///开始掉落回合数（当前回合数 ≥ 该值时该道具进入掉落池）
        /// </summary>
        public int DropStartRound;
        /// <summary>
        ///掉落权重（相对权重，≤0=不参与掉落）
        /// </summary>
        public int DropWeight;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public ItemConfig(int Id, string Name, string Des, int Quality, string Effect, string Attrs, string MainAttr, string SkillId, string RefSkillSname, string LimitSkillSname, bool RemoveWhenUse, string UseSound, int CombineId, int CombineNeed, int DropStartRound, int DropWeight, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Des = Des;
            this.Quality = Quality;
            this.Effect = Effect;
            this.Attrs = Attrs;
            this.MainAttr = MainAttr;
            this.SkillId = SkillId;
            this.RefSkillSname = RefSkillSname;
            this.LimitSkillSname = LimitSkillSname;
            this.RemoveWhenUse = RemoveWhenUse;
            this.UseSound = UseSound;
            this.CombineId = CombineId;
            this.CombineNeed = CombineNeed;
            this.DropStartRound = DropStartRound;
            this.DropWeight = DropWeight;
            this.Icon = Icon;
        }

        public ItemConfig() { }

        private static Dictionary<int, ItemConfig> config = new Dictionary<int, ItemConfig>();
        public static Dictionary<int, ItemConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ItemConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[400001] = new ItemConfig(400001, "关王刀", "攻击+30，护甲+30", 2, "attr", "atk+30,armor+30", "atk", "破盾", "", "", false, "", 0, 0, 999, 0, "guanwangdao");
            config[400002] = new ItemConfig(400002, "方天画戟", "攻击+30，暴击+22%", 2, "attr", "atk+30,crit+0.22", "atk", "枭雄", "", "", false, "", 0, 0, 999, 0, "fangtian");
            config[400003] = new ItemConfig(400003, "丈八蛇矛", "攻击+30，生命+250", 2, "attr", "atk+30,hp+250", "atk", "", "", "", false, "", 0, 0, 999, 0, "zhangba");
            config[400007] = new ItemConfig(400007, "孙子兵法", "法强+15，攻击+20", 2, "attr", "ap+15,atk+20", "ap", "谋定", "", "", false, "", 0, 0, 999, 0, "sunzi");
            config[400008] = new ItemConfig(400008, "诗经", "法强+15，生命+250", 2, "attr", "ap+15,hp+250", "ap", "文润", "", "", false, "", 0, 0, 999, 0, "shijing");
            config[400011] = new ItemConfig(400011, "易经", "法强+10，法力回复+4", 2, "attr", "ap+10,mpRegen+4", "ap", "浩然", "", "", false, "", 0, 0, 999, 0, "yijing");
            config[400012] = new ItemConfig(400012, "道德经", "法强+15，魔法抗性+30", 2, "attr", "ap+15,magicres+30", "ap", "玄妙", "", "", false, "", 0, 0, 999, 0, "daode");
            config[400013] = new ItemConfig(400013, "赤兔马", "攻速+20%，魔法抗性+30，移速+2", 2, "attr", "atkspeed+0.2,magicres+30,movespeed+2", "atkspeed", "", "", "", false, "", 0, 0, 999, 0, "chitu");
            config[400014] = new ItemConfig(400014, "的卢马", "生命+250，生命回复+2", 2, "attr", "hp+250,hpRegen+2", "hp", "", "", "", false, "", 0, 0, 999, 0, "ma5");
            config[400015] = new ItemConfig(400015, "长生镜", "生命+150", 1, "attr", "hp+150", "hp", "", "", "", false, "", 0, 0, 999, 0, "jingzi");
            config[400016] = new ItemConfig(400016, "飞羽甲", "护甲+30，攻速+15%", 2, "attr", "armor+30,atkspeed+0.15", "armor", "疾风", "", "", false, "", 0, 0, 999, 0, "jia3");
            config[400017] = new ItemConfig(400017, "明光铠", "护甲+30，魔抗+30", 2, "attr", "armor+30,magicres+30", "armor", "", "", "", false, "", 0, 0, 999, 0, "jia2");
            config[400018] = new ItemConfig(400018, "兽面吞头铠", "护甲+30，暴击+22%", 2, "attr", "armor+30,crit+0.22", "armor", "凶威", "", "", false, "", 0, 0, 999, 0, "jia4");
            config[400019] = new ItemConfig(400019, "凤凰羽衣", "法强+15，护甲+30", 2, "attr", "ap+15,armor+30", "ap", "玄武", "", "", false, "", 0, 0, 999, 0, "pao4");
            config[400020] = new ItemConfig(400020, "爪电飞黄", "护甲+40，生命+500", 2, "attr", "armor+40,hp+500", "armor", "", "", "", false, "", 0, 0, 999, 0, "ma3");
            config[400021] = new ItemConfig(400021, "穿云弓", "攻速+15%，暴击+22%", 2, "attr", "atkspeed+0.15,crit+0.22", "atkspeed", "", "", "", false, "", 0, 0, 999, 0, "gong4");
            config[400022] = new ItemConfig(400022, "白玉环", "暴击+25%，魔抗+60", 2, "attr", "crit+0.25,magicres+60", "crit", "", "", "", false, "", 0, 0, 999, 0, "huan1");
            config[400023] = new ItemConfig(400023, "玉龙壁", "魔抗+30，法力回复+4", 2, "attr", "magicres+30,mpRegen+4", "magicres", "凝气", "", "", false, "", 0, 0, 999, 0, "huan2");
            config[400024] = new ItemConfig(400024, "绝影", "生命+250，魔抗+30", 2, "attr", "hp+250,magicres+30", "hp", "影遁", "", "", false, "", 0, 0, 999, 0, "ma2");
            config[400025] = new ItemConfig(400025, "古锭刀", "攻击+35，法力回复+4", 2, "attr", "atk+35,mpRegen+4", "atk", "", "", "", false, "", 0, 0, 999, 0, "jian2");
            config[400026] = new ItemConfig(400026, "淬毒飞刀", "攻击+30，攻速+15%", 2, "attr", "atk+30,atkspeed+0.15", "atk", "", "", "", false, "", 0, 0, 999, 0, "feidao");
            config[400027] = new ItemConfig(400027, "破军铁锤", "攻击+30，魔抗+30", 2, "attr", "atk+30,magicres+30", "atk", "破军", "", "", false, "", 0, 0, 999, 0, "chui");
            config[400028] = new ItemConfig(400028, "大克鼎", "护甲+40，法力回复+4", 2, "attr", "armor+40,mpRegen+4", "armor", "", "", "", false, "", 0, 0, 999, 0, "ding1");
            config[400029] = new ItemConfig(400029, "金马枪", "暴击+22%，法强+15", 2, "attr", "crit+0.22,ap+15", "crit", "灵犀", "", "", false, "", 0, 0, 999, 0, "mao1");
            config[400030] = new ItemConfig(400030, "博山炉", "暴击+25%，生命+300", 2, "attr", "crit+0.25,hp+300", "crit", "火箭", "", "", false, "", 0, 0, 999, 0, "lu1");
            config[400031] = new ItemConfig(400031, "象鞭", "暴击+25%，法力回复+4", 2, "attr", "crit+0.25,mpRegen+4", "crit", "", "", "", false, "", 0, 0, 999, 0, "bian1");
            config[400032] = new ItemConfig(400032, "牛灯", "法强+15，攻速+15%", 2, "attr", "ap+15,atkspeed+0.15", "ap", "", "", "", false, "", 0, 0, 999, 0, "deng1");
            config[400033] = new ItemConfig(400033, "双股剑", "攻击+40", 2, "attr", "atk+40", "atk", "破甲", "", "", false, "", 0, 0, 999, 0, "jian3");
            config[400034] = new ItemConfig(400034, "重装玄甲", "护甲+40", 2, "attr", "armor+40", "armor", "", "", "", false, "", 0, 0, 999, 0, "jia5");
            config[400035] = new ItemConfig(400035, "连环弩", "攻速+30%", 2, "attr", "atkspeed+0.3", "atkspeed", "", "", "", false, "", 0, 0, 999, 0, "gong5");
            config[400036] = new ItemConfig(400036, "玄光法袍", "魔抗+40", 2, "attr", "magicres+40", "magicres", "回气", "", "", false, "", 0, 0, 999, 0, "pao3");
            config[400037] = new ItemConfig(400037, "玉玺", "每秒回复4点法力", 2, "attr", "mpRegen+4", "mpRegen", "玄盾", "", "", false, "", 0, 0, 999, 0, "yuxi");
            config[400038] = new ItemConfig(400038, "铜马", "暴击+35%", 2, "attr", "crit+0.35", "crit", "会心", "", "", false, "", 0, 0, 999, 0, "deng2");
            config[400039] = new ItemConfig(400039, "长信宫灯", "法强+25", 2, "attr", "ap+25", "ap", "", "", "", false, "", 0, 0, 999, 0, "deng3");
            config[400040] = new ItemConfig(400040, "大宛宝马", "生命+500", 2, "attr", "hp+500", "hp", "", "", "", false, "", 0, 0, 999, 0, "ma4");
            config[400041] = new ItemConfig(400041, "养由基弓", "攻速+15%，生命+250", 2, "attr", "atkspeed+0.15,hp+250", "atkspeed", "速射", "", "", false, "", 0, 0, 999, 0, "gong3");
            config[400042] = new ItemConfig(400042, "李广弓", "攻速+10%，法力回复+4", 2, "attr", "atkspeed+0.1,mpRegen+4", "atkspeed", "速射", "", "", false, "", 0, 0, 999, 0, "gong2");
            config[401009] = new ItemConfig(401009, "西洋镜", "将一件装备随机重铸为另一件", 2, "tpreroll", "", "", "", "", "", true, "metalhit", 0, 0, 22, 10, "jingzi2");
            config[401010] = new ItemConfig(401010, "无字天书", "英雄经验+1", 2, "tpexp", "", "", "", "", "", true, "yell", 0, 0, 15, 10, "tianshu");
            config[401011] = new ItemConfig(401011, "分解锤", "可将合成装备分解为合成前的两件材料", 2, "tpsplit", "", "", "", "", "", true, "metalhit", 0, 0, 22, 10, "fenjie");
            config[401012] = new ItemConfig(401012, "万民书", "护甲+3，魔法抗性+3（仅限拥有仁的英雄）", 1, "tpattr", "armor+3,magicres+3", "armor", "", "", "仁", true, "yell", 401013, 5, 999, 0, "wanming1");
            config[401013] = new ItemConfig(401013, "万民书·精", "护甲+15，魔法抗性+15（仅限拥有仁的英雄）", 2, "tpattr", "armor+15,magicres+15", "armor", "", "", "仁", true, "yell", 0, 0, 999, 0, "wanming2");
            config[401014] = new ItemConfig(401014, "文赋", "法术强度+2，魔法抗性+4", 1, "tpattr", "ap+2,magicres+4", "ap", "", "", "", true, "yell", 401015, 5, 999, 0, "wenfu1");
            config[401015] = new ItemConfig(401015, "文赋·精", "法术强度+10，魔法抗性+20", 2, "tpattr", "ap+10,magicres+20", "ap", "", "", "", true, "yell", 0, 0, 999, 0, "wenfu2");
            config[402001] = new ItemConfig(402001, "长刀", "攻击+10", 1, "attr", "atk+10", "atk", "", "", "", false, "", 0, 0, 1, 20, "jian1");
            config[402002] = new ItemConfig(402002, "皮革甲", "护甲+20", 1, "attr", "armor+20", "armor", "", "", "", false, "", 0, 0, 1, 20, "jia1");
            config[402003] = new ItemConfig(402003, "檀木弓", "攻速+10%", 1, "attr", "atkspeed+0.1", "atkspeed", "", "", "", false, "", 0, 0, 1, 20, "gong1");
            config[402004] = new ItemConfig(402004, "长袍", "魔抗+20", 1, "attr", "magicres+20", "magicres", "", "", "", false, "", 0, 0, 1, 20, "pao1");
            config[402005] = new ItemConfig(402005, "葫芦", "法力回复+2", 1, "attr", "mpRegen+2", "mpRegen", "", "", "", false, "", 0, 0, 1, 20, "hulu");
            config[402006] = new ItemConfig(402006, "护手", "暴击+15%", 1, "attr", "crit+0.15", "crit", "", "", "", false, "", 0, 0, 1, 20, "hushou");
            config[402007] = new ItemConfig(402007, "羽扇", "法强+10", 1, "attr", "ap+10", "ap", "", "", "", false, "", 0, 0, 1, 20, "yushan");
            config[402008] = new ItemConfig(402008, "名马", "生命+150", 1, "attr", "hp+150", "hp", "", "", "", false, "", 0, 0, 1, 20, "ma1");
            config[403001] = new ItemConfig(403001, "代-无双", "视为无双羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "双", "", false, "", 0, 0, 20, 1, "SkillPic/shuang");
            config[403002] = new ItemConfig(403002, "代-野火", "视为野火羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "火", "", false, "", 0, 0, 20, 1, "SkillPic/huo");
            config[403003] = new ItemConfig(403003, "代-骑射", "视为骑射羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "冷", "", false, "", 0, 0, 20, 1, "SkillPic/leng");
            config[403004] = new ItemConfig(403004, "代-风华", "视为风华羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "华", "", false, "", 0, 0, 20, 1, "SkillPic/hua");
            config[403005] = new ItemConfig(403005, "代-仁德", "视为仁德羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "仁", "", false, "", 0, 0, 20, 1, "SkillPic/shu");
            config[403006] = new ItemConfig(403006, "代-异禀", "视为异禀羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "异", "", false, "", 0, 0, 20, 1, "SkillPic/yi");
            config[403007] = new ItemConfig(403007, "代-济世", "视为济世羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "济", "", false, "", 0, 0, 20, 1, "SkillPic/ji2");
            config[403008] = new ItemConfig(403008, "代-益壮", "视为益壮羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "老", "", false, "", 0, 0, 20, 1, "SkillPic/lao");
            config[403009] = new ItemConfig(403009, "代-陷阵", "视为陷阵羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "破", "", false, "", 0, 0, 20, 1, "SkillPic/po");
            config[403010] = new ItemConfig(403010, "代-深谋", "视为深谋羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "谋", "", false, "", 0, 0, 20, 1, "SkillPic/mou2");
            config[403011] = new ItemConfig(403011, "代-严明", "视为严明羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "律", "", false, "", 0, 0, 20, 1, "SkillPic/lv");
            config[403012] = new ItemConfig(403012, "代-王佐", "视为王佐羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "佐", "", false, "", 0, 0, 20, 1, "SkillPic/zuo");
            config[403013] = new ItemConfig(403013, "代-迅捷", "视为迅捷羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "连", "", false, "", 0, 0, 20, 1, "SkillPic/lian");
            config[403014] = new ItemConfig(403014, "代-悍将", "视为悍将羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "穿", "", false, "", 0, 0, 20, 1, "SkillPic/chuan");
            config[403015] = new ItemConfig(403015, "代-义胆", "视为义胆羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "护", "", false, "", 0, 0, 20, 1, "SkillPic/hu");
            config[403016] = new ItemConfig(403016, "代-权奸", "视为权奸羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "奸", "", false, "", 0, 0, 20, 1, "SkillPic/jian2");
            config[403017] = new ItemConfig(403017, "代-望族", "视为望族羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "族", "", false, "", 0, 0, 20, 1, "SkillPic/zu2");
            config[403018] = new ItemConfig(403018, "代-将相", "视为将相羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "兼", "", false, "", 0, 0, 20, 1, "SkillPic/jian4");
            config[403019] = new ItemConfig(403019, "代-自守", "视为自守羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "刺", "", false, "", 0, 0, 20, 1, "SkillPic/ci");
            config[403020] = new ItemConfig(403020, "代-奇才", "视为奇才羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "才", "", false, "", 0, 0, 20, 1, "SkillPic/cai");
            config[403021] = new ItemConfig(403021, "代-温良", "视为温良羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "俭", "", false, "", 0, 0, 20, 1, "SkillPic/jian3");
            config[403022] = new ItemConfig(403022, "代-背水", "视为背水羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "背", "", false, "", 0, 0, 20, 1, "SkillPic/bei");
            config[403023] = new ItemConfig(403023, "代-偷袭", "视为偷袭羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "偷", "", false, "", 0, 0, 20, 1, "SkillPic/tou");
            config[403024] = new ItemConfig(403024, "代-文采", "视为文采羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "文", "", false, "", 0, 0, 20, 1, "SkillPic/wen");
            config[403025] = new ItemConfig(403025, "代-儒将", "视为儒将羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "儒", "", false, "", 0, 0, 20, 1, "SkillPic/ru");
            config[403026] = new ItemConfig(403026, "代-铁骑", "视为铁骑羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "冲", "", false, "", 0, 0, 20, 1, "SkillPic/chong");
            config[403027] = new ItemConfig(403027, "代-智勇", "视为智勇羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "镜", "", false, "", 0, 0, 20, 1, "SkillPic/jing");
            config[403028] = new ItemConfig(403028, "代-善守", "视为善守羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "坚", "", false, "", 0, 0, 20, 1, "SkillPic/jian");
            config[403029] = new ItemConfig(403029, "代-虎狼", "视为虎狼羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "突", "", false, "", 0, 0, 20, 1, "SkillPic/tu");
            config[403030] = new ItemConfig(403030, "代-枭雄", "视为枭雄羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "枭", "", false, "", 0, 0, 20, 1, "SkillPic/xiao");
            config[403031] = new ItemConfig(403031, "代-安民", "视为安民羁绊组在场1人", 2, "friendrep", "hp+30,atk+3", "", "", "民", "", false, "", 0, 0, 20, 1, "SkillPic/min");
            config[409001] = new ItemConfig(409001, "火尖枪", "攻击+20", 5, "attr", "atk+20", "atk", "", "", "", false, "", 0, 0, 999, 0, "huojianqiang");
            config[409002] = new ItemConfig(409002, "聚宝盆", "每年额外获得2金币", 5, "pattr", "roundgold+2", "", "", "", "", false, "", 0, 0, 999, 0, "jubaopeng");
            config[409003] = new ItemConfig(409003, "虎王重甲", "士兵生命+100", 5, "pattr", "shp+100", "", "", "", "", false, "", 0, 0, 999, 0, "armor");
            config[409004] = new ItemConfig(409004, "玉如意", "出售卡牌多获得25%金币", 5, "sellhigh", "", "", "", "", "", false, "", 0, 0, 999, 0, "ruyi");
            config[409005] = new ItemConfig(409005, "酒", "攻击+10，法强+10", 5, "attr", "atk+10,ap+10", "atk", "", "", "", false, "", 0, 0, 999, 0, "jiu");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static ItemConfig GetConfig(int id)
        {
            ItemConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ItemConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ItemConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ItemConfig configData)
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
