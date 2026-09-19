using UnityEngine;
using TMPro;
using CommonConfig;
using System.Collections.Generic;
using System.Linq;

public class TooltipHero : BaseTooltip
{
    // 英雄名字 + 等级行：tooltip 最上方显示（仅英雄卡），名字按品质上色
    private const float NameRowHeight = 30f;
    private TMP_Text textName;

    // 卡片属性显示：图标 + 属性值，一行两个（最多4个属性 = 2行）
    private const float AttrRowHeight = 40f;
    private static GameObject attrPrefab;                                           // 属性格预制体缓存
    private readonly List<TooltipHeroAttr> attrCells = new List<TooltipHeroAttr>(); // 属性格（按需生成）

    // 道具描述文本（动态创建，道具卡显示）
    private TMP_Text textDes;

    // 好友连接显示：每组 = 技能图标 + 技能描述(最多2行) + 人员列表(1行)，最多4组
    private const int MaxFriendGroups = 4;
    private readonly List<TooltipHeroSkill> friendRows = new List<TooltipHeroSkill>(); // 好友行（按需扩容）

    // 技能行：每个技能使用一个 ToolTipHeroSkill.prefab（每行固定高度100）
    private const float SkillRowHeight = 100f;
    private static GameObject skillRowPrefab;                             // 预制体缓存
    private readonly List<TooltipHeroSkill> skillRows = new List<TooltipHeroSkill>(); // 按需扩容

    // 装备行：每个装备一个 ToolTipHeroEquip.prefab（500x70），英雄身上有装备时显示装备名+图标
    private const float EquipRowHeight = 60f;
    private static GameObject equipRowPrefab;   // 预制体缓存
    private readonly List<TooltipHeroEquip> equipRows = new List<TooltipHeroEquip>(); // 装备行（按需扩容）

    protected override void Awake()
    {
        base.Awake();
        CreateAttrControls();
    }

    // 用属性格预制体生成属性控件（2行×2列，最多10格）
    private void CreateAttrControls()
    {
        if (attrPrefab == null)
            attrPrefab = Resources.Load<GameObject>("Prefabs/ToolTipHeroAttr");
        if (attrPrefab == null)
        {
            GameLog.Error("TooltipHero 属性格预制体加载失败: Prefabs/ToolTipHeroAttr");
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            var go = Instantiate(attrPrefab, rect);
            go.name = "AttrCell" + i;
            var cell = go.GetComponent<TooltipHeroAttr>();
            if (cell == null)
            {
                GameLog.Error("TooltipHero 属性格预制体缺少 TooltipHeroAttr 组件");
                Destroy(go);
                continue;
            }
            int row = i / 2;
            int col = i % 2;
            float baseX = 20f + col * 200f;
            // 属性区顶部让出名字行（名字行底 -50 起再留 15 间距，首行中心 -65）
            float y = -20f - NameRowHeight - 15f - row * AttrRowHeight;
            var rt = (RectTransform)cell.transform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(baseX + 20, y);
            attrCells.Add(cell);
        }

        // 道具描述文本（复用属性格预制体的文字控件，直接挂到 tooltip 下）
        var desGo = Instantiate(attrPrefab, rect);
        var desCell = desGo.GetComponent<TooltipHeroAttr>();
        if (desCell == null || desCell.text == null)
        {
            GameLog.Error("TooltipHero 属性格预制体缺少 TooltipHeroAttr 组件或文字控件");
            Destroy(desGo);
            return;
        }
        textDes = desCell.text;
        textDes.rectTransform.SetParent(rect, false);
        var desRt = textDes.rectTransform;
        desRt.anchorMin = new Vector2(0, 1);
        desRt.anchorMax = new Vector2(0, 1);
        desRt.pivot = new Vector2(0, 1);
        desRt.anchoredPosition = new Vector2(20, 0);
        desRt.sizeDelta = new Vector2(360, 30);
        textDes.gameObject.SetActive(false);
        Destroy(desGo);

        // 英雄名字 + 等级行（复用属性格预制体的文字控件，tooltip 最上方，仅英雄卡显示）
        var nameGo = Instantiate(attrPrefab, rect);
        var nameCell = nameGo.GetComponent<TooltipHeroAttr>();
        if (nameCell == null || nameCell.text == null)
        {
            GameLog.Error("TooltipHero 属性格预制体缺少 TooltipHeroAttr 组件或文字控件");
            Destroy(nameGo);
            return;
        }
        textName = nameCell.text;
        textName.rectTransform.SetParent(rect, false);
        var nameRt = textName.rectTransform;
        nameRt.anchorMin = new Vector2(0, 1);
        nameRt.anchorMax = new Vector2(0, 1);
        nameRt.pivot = new Vector2(0, 1);
        nameRt.anchoredPosition = new Vector2(15, -10);
        nameRt.sizeDelta = new Vector2(360, NameRowHeight);
        textName.gameObject.SetActive(false);
        Destroy(nameGo);
    }

    // 取第 index 个好友行（ToolTipHeroSkill.prefab 实例，按需创建扩容）
    private TooltipHeroSkill GetFriendRow(int index)
    {
        while (friendRows.Count <= index)
        {
            if (skillRowPrefab == null)
                skillRowPrefab = Resources.Load<GameObject>("Prefabs/ToolTipHeroSkill");
            if (skillRowPrefab == null)
            {
                GameLog.Error("TooltipHero 好友行预制体加载失败: Prefabs/ToolTipHeroSkill");
                return null;
            }
            var go = Instantiate(skillRowPrefab, rect);
            go.name = "FriendRow" + friendRows.Count;
            var comp = go.GetComponent<TooltipHeroSkill>();
            if (comp == null)
            {
                GameLog.Error("TooltipHero 好友行预制体缺少 TooltipHeroSkill 组件");
                Destroy(go);
                return null;
            }
            friendRows.Add(comp);
        }
        return friendRows[index];
    }

    // 取第 index 个技能行（ToolTipHeroSkill.prefab 实例，按需创建扩容）
    private TooltipHeroSkill GetSkillRow(int index)
    {
        while (skillRows.Count <= index)
        {
            if (skillRowPrefab == null)
                skillRowPrefab = Resources.Load<GameObject>("Prefabs/ToolTipHeroSkill");
            if (skillRowPrefab == null)
            {
                GameLog.Error("TooltipHero 技能行预制体加载失败: Prefabs/ToolTipHeroSkill");
                return null;
            }
            var go = Instantiate(skillRowPrefab, rect);
            go.name = "SkillRow" + skillRows.Count;
            var comp = go.GetComponent<TooltipHeroSkill>();
            if (comp == null)
            {
                GameLog.Error("TooltipHero 技能行预制体缺少 TooltipHeroSkill 组件");
                Destroy(go);
                return null;
            }
            skillRows.Add(comp);
        }
        return skillRows[index];
    }

    // 取第 index 个装备行（ToolTipHeroEquip.prefab 实例，按需创建扩容，最多=装备数量）
    private TooltipHeroEquip GetEquipRow(int index)
    {
        while (equipRows.Count <= index)
        {
            if (equipRowPrefab == null)
                equipRowPrefab = Resources.Load<GameObject>("Prefabs/ToolTipHeroEquip");
            if (equipRowPrefab == null)
            {
                GameLog.Error("TooltipHero 装备行预制体加载失败: Prefabs/ToolTipHeroEquip");
                return null;
            }
            var go = Instantiate(equipRowPrefab, rect);
            go.name = "EquipRow" + equipRows.Count;
            var comp = go.GetComponent<TooltipHeroEquip>();
            if (comp == null)
            {
                GameLog.Error("TooltipHero 装备行预制体缺少 TooltipHeroEquip 组件");
                Destroy(go);
                return null;
            }
            equipRows.Add(comp);
        }
        return equipRows[index];
    }

    // 基础属性 + 装备加成："11 + 12"（+12 用淡绿），无加成就只显示基础值
    private static string AppendEquip(int baseVal, int bonus)
    {
        if (bonus <= 0)
            return baseVal.ToString();
        return baseVal + " + " + SysColor.ColorText(bonus.ToString(), SysColor.UI.NextLv);
    }

    // 攻速/暴击类比例属性：装备给的是比例（0.1=+10%），显示为 "+20%"
    private static string AppendEquipSpeed(int baseVal, float bonusRate)
    {
        if (bonusRate <= 0)
            return baseVal.ToString();
        return baseVal + " + " + SysColor.ColorText(Mathf.RoundToInt(bonusRate * 100) + "%", SysColor.UI.NextLv);
    }

    // 排列技能/好友行：锚定 tooltip 左上角，每行固定高度100，自上而下排列；返回累加后的Y
    private float LayoutRow(TooltipHeroSkill row, float currentY)
    {
        var rowRt = (RectTransform)row.transform;
        rowRt.anchorMin = new Vector2(0, 1);
        rowRt.anchorMax = new Vector2(0, 1);
        rowRt.pivot = new Vector2(0.5f, 0.5f);
        rowRt.anchoredPosition = new Vector2(250, -currentY - SkillRowHeight * 0.5f);
        return currentY + SkillRowHeight;
    }

    public void ShowTooltip(List<SkillConfig> skillCfgs, HashSet<int> friendInfo, int heroId, PlayerInfo player = null, bool isShopCard = false)
    {
        bool hasSkill = skillCfgs != null && skillCfgs.Count > 0;
        bool hasFriend = friendInfo != null && friendInfo.Count > 0;

        // 卡片等级：在身上时=卡片等级（可能超5，普通技能显示时截断到5）；商店/排行榜默认1级
        int cardLv = 1;
        // 属性取值与战斗统一：HeroConfig 数值经 PostModify 写回为 1星带品质面板（四主），星级成长走 GetCardAttr
        AttrInfo attr;
        if (player != null)
        {
            int exp = player.cards.TryGetValue(heroId, out int e) ? e : 1;
            cardLv = HeroSelectionTool.GetCardLevel(exp, ConfigManager.IsHeroCard(heroId));
            attr = HeroSelectionTool.GetCardAttr(player, heroId, cardLv);
        }
        else
        {
            // 无玩家上下文（如排行榜）：直接显示写回的 1星带品质面板（无双强度已并入 Atk）
            var heroCfg = HeroConfig.GetConfig(heroId);
            attr = new AttrInfo() { Atk = heroCfg.Atk, Ap = heroCfg.Ap, Hp = heroCfg.Hp };
        }

        // 属性列表：英雄显示全部8项（攻/法/命/攻速/护甲/魔抗/移速/射程，无双已并入攻），道具只显示有效属性
        bool isHero = ConfigManager.IsHeroCard(heroId);

        // 装备加成汇总：英雄已装备物品（最多3槽）的属性总和（攻/法/命/护甲/魔抗/攻速比例）；
        // 无装备或无玩家上下文（商店/排行榜）时全为0。装备行内容与基础属性数值调整都依赖该汇总
        var equipIds = new List<int>();
        int eAtk = 0, eAp = 0, eHp = 0, eArmor = 0, eMagicRes = 0;
        float eAtkSpeedRate = 0f, eMpRegen = 0f, eHpRegen = 0f;
        var heroEquipIds = player != null && isHero ? player.GetItemIdsOnHero(heroId) : new List<int>();
        if (player != null && isHero)
        {
            foreach (var itemId in heroEquipIds)
            {
                if (itemId == 0)
                    continue;
                equipIds.Add(itemId);
                var ea = HeroSelectionTool.GetCardAttr(player, itemId, 1);
                eAtk += ea.Atk; eAp += ea.Ap; eHp += ea.Hp;
                eArmor += ea.Armor; eMagicRes += ea.MagicRes;
                eAtkSpeedRate += ea.AttackSpeedRate;
                eMpRegen += ea.MpRegen; eHpRegen += ea.HpRegen;
            }
        }
        bool hasEquip = equipIds.Count > 0;

        string[] attrKeys;
        string[] attrVals;
        if (isHero)
        {
            var heroCfg = HeroConfig.GetConfig(heroId);
            attrKeys = new string[] { "atk", "atkspeed", "ap", "mpRegen", "hp", "hpRegen", "armor", "magicres", "movespeed", "range" };
            attrVals = new string[]
            {
                AppendEquip(attr.Atk, eAtk),
                AppendEquipSpeed(heroCfg.AtkSpeed, eAtkSpeedRate),
                AppendEquip(attr.Ap, eAp),
                AppendEquip(0, (int)eMpRegen),
                AppendEquip(attr.Hp, eHp),
                AppendEquip(0, (int)eHpRegen),
                AppendEquip(heroCfg.Armor, eArmor),
                AppendEquip(heroCfg.MagicRes, eMagicRes),
                heroCfg.MoveSpeed.ToString(), heroCfg.Range.ToString()
            };
        }
        else
        {
            // 道具只显示配置的有效属性行（Attrs："attr+value,attr+value" 解析，含护甲/魔抗/攻速/暴击/回蓝等扩展属性）
            var itemCfg = ItemConfig.GetConfig(heroId);
            var listKeys = new List<string>();
            var listVals = new List<string>();
            foreach (var bonus in JobLinkManager.ParseBonuses(itemCfg.Attrs))
                AddItemAttrRow(bonus.Attr, bonus.Value, listKeys, listVals);
            attrKeys = listKeys.ToArray();
            attrVals = listVals.ToArray();
        }

        int attrRows = 0;
        int shownAttr = 0;
        int lastShownRow = -1;
        for (int i = 0; i < attrCells.Count; i++)
        {
            bool show = i < attrKeys.Length;
            attrCells[i].gameObject.SetActive(show);
            if (show)
            {
                // 属性图标统一读 HeroAttrConfig 表（name → Icon）
                var attrCfg = HeroAttrConfig.GetConfigByname(attrKeys[i]);
                string attrIcon = string.IsNullOrEmpty(attrCfg.Icon) ? "attrhp" : attrCfg.Icon;
                attrCells[i].SetAttr(attrIcon, attrVals[i]);
                shownAttr++;
                lastShownRow = i / 2;
            }
        }
        // 按最后一个显示格的所在行计算高度（中间可能有值为0被隐藏的项，不能按显示个数算）
        attrRows = lastShownRow + 1;

        // 英雄名字 + 等级行（仅英雄卡显示，名字按品质上色；等级=卡片等级）
        if (textName != null)
        {
            if (isHero)
            {
                var heroCfg = HeroConfig.GetConfig(heroId);
                string nameHex = ColorUtility.ToHtmlStringRGB(SysColor.GetQualityColor(heroCfg.Quality));
                textName.text = "<color=#" + nameHex + ">" + heroCfg.Name + "</color> Lv" + cardLv;
                textName.gameObject.SetActive(true);
            }
            else
            {
                textName.gameObject.SetActive(false);
            }
        }

        // 道具卡：显示道具描述（属性区下方）
        bool isItem = !ConfigManager.IsHeroCard(heroId);
        string itemDes = null;
        if (isItem)
        {
            var itemCfg = ItemConfig.GetConfig(heroId);
            if (itemCfg != null)
                itemDes = itemCfg.Des;
        }
        bool hasDes = !string.IsNullOrEmpty(itemDes);

        // 没有任何可显示内容时（如无属性的道具），不弹空 Tip
        if (shownAttr == 0 && !hasSkill && !hasFriend && !hasDes && !hasEquip)
        {
            HideTooltip();
            return;
        }

        float currentY = 10f + NameRowHeight + 15f + attrRows * AttrRowHeight; // 起始Y位置（名字行 + 属性区下方）
        float spacing = 5f;   // 控件间距

        if (hasDes)
        {
            textDes.gameObject.SetActive(true);
            textDes.text = itemDes;
            textDes.rectTransform.anchoredPosition = new Vector2(20, -currentY);
            textDes.rectTransform.sizeDelta = new Vector2(360, textDes.preferredHeight);
            currentY += textDes.preferredHeight + spacing;
        }
        else if (textDes != null)
        {
            textDes.gameObject.SetActive(false);
        }
        
        // 先隐藏全部技能行，再按需显示（无技能时不残留上次的行）
        for (int i = 0; i < skillRows.Count; i++)
            skillRows[i].gameObject.SetActive(false);

        if (hasSkill)
        {
            // 职业技能行：档位按当前上阵同职业人数（与战斗连锁一致）；列表行显示配置表里所有同职业英雄
            int jobFieldCount = 0;
            string heroJob = null;
            var jobHeroIds = new List<int>();
            if (isHero)
            {
                heroJob = HeroConfig.GetConfig(heroId).Job;
                if (player != null)
                {
                    // 档位：上阵同职业人数
                    foreach (var cardId in player.battleCards)
                    {
                        if (cardId > 0 && ConfigManager.IsHeroCard(cardId) && HeroConfig.GetConfig(cardId).Job == heroJob)
                            jobFieldCount++;
                    }
                }
                // 列表：配置表里所有同职业英雄（含自己，与好友组一致）
                foreach (var cfg in HeroConfig.ConfigList)
                {
                    if (cfg.Job == heroJob)
                        jobHeroIds.Add(cfg.Id);
                }
            }

            for(int i = 0; i < skillCfgs.Count; i++)
            {
                var row = GetSkillRow(i);
                if (row == null)
                    break;
                row.gameObject.SetActive(true);
                var skillConfig = skillCfgs[i];
                string skillText;
                if (skillConfig.Type == "职业")
                {
                    // 职业技能（兵种连锁）：技能名 + 效果（当前档描述 + 下一档不同描述括号）
                    // 等级：商店牌默认1级；背包按上阵同职业人数（0级时显示1级效果并置灰）
                    int jobLv = isShopCard || player == null ? 1 : Mathf.Max(1, jobFieldCount);
                    var jobCfg = ConfigManager.GetJobConfig(heroJob);
                    string effect = "";
                    if (jobCfg != null && !string.IsNullOrEmpty(jobCfg.SkillId))
                    {
                        var jobSkillCfg = ConfigManager.GetSkillConfig(jobCfg.SkillId, jobLv);
                        if (jobSkillCfg != null)
                            effect = ConfigManager.GetSkillDescript(jobSkillCfg, true);
                    }
                    skillText = string.IsNullOrEmpty(effect) ? skillConfig.Name : skillConfig.Name + " " + effect;
                    // 列表行：同职业英雄（按品质倒排；商店全部按品质上色，背包仅上阵的上色）
                    string jobList = BuildHeroNameList(jobHeroIds, player, isShopCard);
                    row.SetFriendSkill(skillText, skillConfig.Icon, jobList, jobLv);
                }
                else
                {
                    skillText = skillConfig.Name + ConfigManager.GetSkillDescript(skillConfig); //富文本
                    // 非职业等级：在身上时等级=卡片等级（最高5）；商店/排行榜默认1级
                    int skillLv = player != null ? Mathf.Min(cardLv, 5) : 1;
                    row.SetSkill(skillText, skillConfig.Icon, skillLv);
                }

                // 每行固定高度100，自上而下排列
                currentY = LayoutRow(row, currentY);
            }
        }
        
        // 先隐藏全部好友行，再按需显示（无好友时不残留上次的行）
        for (int i = 0; i < friendRows.Count; i++)
            friendRows[i].gameObject.SetActive(false);

        if (hasFriend)
        {
            int idx = 0;
            foreach (var item in friendInfo)
            {
                if (idx >= MaxFriendGroups)
                    break;
                var friendCfg = HeroFriendConfig.GetConfig(item);
                if (friendCfg == null)
                    continue;

                var row = GetFriendRow(idx);
                if (row == null)
                    break;
                row.gameObject.SetActive(true);

                // 技能描述（最多2行，超出截断）+ 图标：组内配了特殊技能用特殊技能，未配则用默认连线技能"友"
                int present = CountFriendPresent(friendCfg, heroId, player);
                var friendSkillCfg = GetFriendShowSkill(friendCfg, present, out int friendLv);
                if (isShopCard || player == null)
                    friendLv = 1; // 商店牌默认1级；排行榜无上下文也默认1级
                string skillText = "";
                string icon = "";
                if (friendSkillCfg != null)
                {
                    skillText = friendSkillCfg.Name + " " + ConfigManager.GetSkillDescript(friendSkillCfg, true);
                    icon = friendSkillCfg.Icon;
                }

                // 人员列表（1行，超出截断）：按品质倒排；商店全部按品质上色，背包仅上阵的上色
                string listStr = BuildHeroNameList(friendCfg.Heros, player, isShopCard);

                // 图标 + 描述(最多2行) + 列表(1行) 由好友行承载，行高固定与技能行一致
                row.SetFriendSkill(skillText, icon, listStr, friendLv);
                currentY = LayoutRow(row, currentY);
                idx++;
            }
        }

        // 装备行（每个装备一个 500x70 行）：显示在技能/好友行之后，图标 + 装备名
        for (int i = 0; i < equipRows.Count; i++)
            equipRows[i].gameObject.SetActive(false);

        if (hasEquip)
        {
            for (int i = 0; i < equipIds.Count; i++)
            {
                var row = GetEquipRow(i);
                if (row == null)
                    break;
                row.gameObject.SetActive(true);
                var itemCfg = ItemConfig.GetConfig(equipIds[i]);
                row.img.sprite = Resources.Load<Sprite>("Textures/ItemPic/" + itemCfg.Icon);
                row.textEquip.text = itemCfg.Name + ":" + itemCfg.Des;
                var rowRt = (RectTransform)row.transform;
                rowRt.anchorMin = new Vector2(0, 1);
                rowRt.anchorMax = new Vector2(0, 1);
                rowRt.pivot = new Vector2(0.5f, 0.5f);
                rowRt.anchoredPosition = new Vector2(250, -currentY - EquipRowHeight * 0.5f);
                currentY += EquipRowHeight + spacing;
            }
        }

        // 调整背景大小
        float height = Mathf.Max(50f, currentY + 10f);
        rect.sizeDelta = new Vector2(500, height);

        // 内容填充完毕，走基类统一显示/定位逻辑（同一时刻只显示一个、贴边不出屏）
        Show();
    }

    // 好友关系组在场（上阵）成员数（不含自己）
    private static int CountFriendPresent(HeroFriendConfig friendCfg, int heroId, PlayerInfo player)
    {
        int present = 0;
        if (player != null && player.battleCards != null)
        {
            foreach (var mid in friendCfg.Heros)
            {
                if (mid != heroId && player.battleCards.Contains(mid))
                    present++;
            }
        }
        return present;
    }

    // 好友组提示里展示的技能：组内配置了特殊技能就用特殊技能（等级=在场成员数，0级=未激活，文本按1级档显示）；
    // 未配置特殊技能则回退默认连线技能"友"（等级按 CombatConst.FriendLineCounts 档位，好友不足2人=0级）。
    // level 返回实际展示等级（调用方据此置灰），返回 null 表示无可用技能配置
    internal static SkillConfig GetFriendShowSkill(HeroFriendConfig friendCfg, int presentCount, out int level)
    {
        string sname = friendCfg.SkillId;
        if (string.IsNullOrEmpty(sname))
        {
            sname = CombatConst.FriendLineSkillSname;
            level = FriendLineManager.GetFriendLineLevel(presentCount);
        }
        else
        {
            level = CombatConst.FriendSpecialBaseLevel + presentCount;
        }
        return ConfigManager.GetSkillConfig(sname, Mathf.Max(1, level));
    }

    // 拼英雄名字列表（职业/好友列表共用）：按品质倒排（同品质保持原顺序），名字只保留最后一个字；
    // 商店牌：全部名字按品质上色；背包：只有上阵（battleCards 内）的按品质上色，未上阵的保持默认色
    // internal 供 TooltipFriend（羁绊提示）复用
    internal static string BuildHeroNameList(IEnumerable<int> heroIds, PlayerInfo player, bool isShopCard)
    {
        var valid = new List<int>();
        foreach (var hid in heroIds)
        {
            if (HeroConfig.GetConfig(hid) != null)
                valid.Add(hid);
        }

        var parts = new List<string>();
        foreach (var hid in valid.OrderByDescending(id => HeroConfig.GetConfig(id).Quality))
        {
            var cfg = HeroConfig.GetConfig(hid);
            string name = cfg.Name.Substring(cfg.Name.Length - 1);
            if (isShopCard || (player != null && player.battleCards.Contains(hid)))
                name = "<color=#" + ColorUtility.ToHtmlStringRGB(SysColor.GetQualityColor(cfg.Quality)) + ">" + name + "</color>";
            parts.Add(name);
        }
        return string.Join(" ", parts);
    }

    // 道具属性行：键值按配置输出，比例属性（攻速/暴击）带 % 后缀，其余直接显示数值
    private void AddItemAttrRow(string key, float value, List<string> keys, List<string> vals)
    {
        if (string.IsNullOrEmpty(key) || value == 0)
            return;
        keys.Add(key);
        bool isPercent = key == "atkspeed" || key == "crit";
        vals.Add(isPercent ? Mathf.RoundToInt(value * 100) + "%" : value.ToString("0.##"));
    }
}