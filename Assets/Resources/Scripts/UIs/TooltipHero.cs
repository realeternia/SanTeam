using UnityEngine;
using TMPro;
using CommonConfig;
using System.Collections.Generic;
using System.Linq;

public class TooltipHero : BaseTooltip
{
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

    protected override void Awake()
    {
        base.Awake();
        CreateAttrControls();
    }

    // 用属性格预制体生成属性控件（2行×2列，最多9格）
    private void CreateAttrControls()
    {
        if (attrPrefab == null)
            attrPrefab = Resources.Load<GameObject>("Prefabs/ToolTipHeroAttr");
        if (attrPrefab == null)
        {
            GameLog.Error("TooltipHero 属性格预制体加载失败: Prefabs/ToolTipHeroAttr");
            return;
        }

        for (int i = 0; i < 9; i++)
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
            float y = -20f - row * AttrRowHeight;
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

    public void ShowTooltip(List<SkillConfig> skillCfgs, HashSet<int> friendInfo, int heroId, PlayerInfo player = null)
    {
        bool hasSkill = skillCfgs != null && skillCfgs.Count > 0;
        bool hasFriend = friendInfo != null && friendInfo.Count > 0;

        // 属性取值与战斗统一：HeroConfig 数值经 PostModify 写回为 1星带品质面板（四主），星级成长走 GetCardAttr
        AttrInfo attr;
        if (player != null)
        {
            int exp = player.cards.TryGetValue(heroId, out int e) ? e : 1;
            int lv = HeroSelectionTool.GetCardLevel(exp, ConfigManager.IsHeroCard(heroId));
            attr = HeroSelectionTool.GetCardAttr(player, heroId, lv);
        }
        else
        {
            // 无玩家上下文（如排行榜）：直接显示写回的 1星带品质面板（无双强度已并入 Atk）
            var heroCfg = HeroConfig.GetConfig(heroId);
            attr = new AttrInfo() { Atk = heroCfg.Atk, Ap = heroCfg.Ap, Hp = heroCfg.Hp };
        }

        // 属性列表：英雄显示全部8项（攻/法/命/攻速/护甲/魔抗/移速/射程，无双已并入攻），道具只显示有效属性
        bool isHero = ConfigManager.IsHeroCard(heroId);
        string[] attrKeys;
        string[] attrVals;
        if (isHero)
        {
            var heroCfg = HeroConfig.GetConfig(heroId);
            attrKeys = new string[] { "atk", "ap", "hp", "atkspeed", "armor", "magicres", "movespeed", "range" };
            attrVals = new string[]
            {
                attr.Atk.ToString(), attr.Ap.ToString(), attr.Hp.ToString(),
                heroCfg.AtkSpeed.ToString(),
                heroCfg.Armor.ToString(), heroCfg.MagicRes.ToString(),
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
        for (int i = 0; i < attrCells.Count; i++)
        {
            bool show = i < attrKeys.Length && attrVals[i] != "0";
            attrCells[i].gameObject.SetActive(show);
            if (show)
            {
                // 属性图标统一读 HeroAttrConfig 表（name → Icon）
                var attrCfg = HeroAttrConfig.GetConfigByname(attrKeys[i]);
                string attrIcon = string.IsNullOrEmpty(attrCfg.Icon) ? "attrhp" : attrCfg.Icon;
                attrCells[i].SetAttr(attrIcon, attrVals[i]);
                shownAttr++;
            }
        }
        attrRows = (shownAttr + 1) / 2;

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
        if (shownAttr == 0 && !hasSkill && !hasFriend && !hasDes)
        {
            HideTooltip();
            return;
        }

        float currentY = 10f + attrRows * AttrRowHeight; // 起始Y位置（属性区下方）
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
            // 职业技能行：统计当前玩家上阵同职业英雄数，用于羁绊档位高亮
            int jobFieldCount = 0;
            string heroJob = null;
            if (isHero)
            {
                heroJob = HeroConfig.GetConfig(heroId).Job;
                if (player != null)
                {
                    foreach (var cardId in player.battleCards)
                    {
                        if (cardId > 0 && ConfigManager.IsHeroCard(cardId) && HeroConfig.GetConfig(cardId).Job == heroJob)
                            jobFieldCount++;
                    }
                }
            }

            for(int i = 0; i < skillCfgs.Count; i++)
            {
                var row = GetSkillRow(i);
                if (row == null)
                    break;
                row.gameObject.SetActive(true);
                var skillConfig = skillCfgs[i];
                // 属性标签前缀：IsMagic=法术(蓝)，否则=物理(黄)
                var skillAttrStr = skillConfig.IsMagic ? "<color=blue>[法]</color>" : "<color=yellow>[攻]</color>";
                string skillText;
                if (skillConfig.Type == "职业")
                {
                    // 职业技能（兵种连锁）：单行差值格式（当前档数值 + 下一档差异括号）
                    skillText = skillConfig.Name + JobLinkManager.GetJobLinkTipText(heroJob, jobFieldCount);
                }
                else
                {
                    skillText = skillAttrStr + skillConfig.Name + skillConfig.Descript; //富文本
                }
                // 图标 + 文字（含字号缩放/截断）由 TooltipHeroSkill 统一实现
                row.SetSkill(skillText, skillConfig.Icon);

                // 每行固定高度100，自上而下排列（锚定 tooltip 左上角，与当前高度无关）
                var rowRt = (RectTransform)row.transform;
                rowRt.anchorMin = new Vector2(0, 1);
                rowRt.anchorMax = new Vector2(0, 1);
                rowRt.pivot = new Vector2(0.5f, 0.5f);
                rowRt.anchoredPosition = new Vector2(250, -currentY - SkillRowHeight * 0.5f);
                currentY += SkillRowHeight;
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

                // 技能描述（最多2行，超出截断）+ 图标
                var friendSkillCfg = !string.IsNullOrEmpty(friendCfg.SkillId) ? ConfigManager.GetSkillConfig(friendCfg.SkillId, 1) : null;
                string skillText = "";
                string icon = "";
                if (friendSkillCfg != null)
                {
                    var friendAttrStr = friendSkillCfg.IsMagic ? "<color=blue>[法]</color>" : "<color=yellow>[攻]</color>";
                    skillText = friendAttrStr + friendSkillCfg.Name
                        + JobLinkManager.GetTierDiffTipText(friendCfg.SkillId, GetFriendSkillLv(friendCfg, heroId, player));
                    icon = friendSkillCfg.Icon;
                }

                // 人员列表（1行，超出截断）
                string listStr = "<color=green>" + friendCfg.Name + "</color>：";
                foreach (var hid in friendCfg.Heros)
                {
                    var heroConfig = HeroConfig.GetConfig(hid);
                    if (!HeroSelectionTool.HasHeroInPool(hid))
                        listStr += "<color=#808080>" + heroConfig.Name + "</color> ";
                    else
                        listStr += heroConfig.Name + " ";
                }

                // 图标 + 描述(最多2行) + 列表(1行) 由好友行承载，行高固定与技能行一致
                row.SetFriendSkill(skillText, icon, listStr);
                var rowRt = (RectTransform)row.transform;
                rowRt.anchorMin = new Vector2(0, 1);
                rowRt.anchorMax = new Vector2(0, 1);
                rowRt.pivot = new Vector2(0.5f, 0.5f);
                rowRt.anchoredPosition = new Vector2(250, -currentY - SkillRowHeight * 0.5f);
                currentY += SkillRowHeight;
                idx++;
            }
        }
        
        // 调整背景大小
        float height = Mathf.Max(50f, currentY + 10f);
        rect.sizeDelta = new Vector2(500, height);

        // 内容填充完毕，走基类统一显示/定位逻辑（同一时刻只显示一个、贴边不出屏）
        Show();
    }

    // 好友连接技能当前档位：该关系组在场（上阵）成员数（不含自己），与战斗规则一致；
    // 无玩家上下文（排行榜）或未达标时默认显示1级档
    private static int GetFriendSkillLv(HeroFriendConfig friendCfg, int heroId, PlayerInfo player)
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
        int lv = CombatConst.FriendSpecialBaseLevel + present;
        return lv < 1 ? 1 : lv;
    }

    // 道具属性行：键值按配置输出，比例属性（攻速/暴击）带 % 后缀，其余直接显示数值
    private void AddItemAttrRow(string key, float value, List<string> keys, List<string> vals)
    {
        if (string.IsNullOrEmpty(key) || value == 0)
            return;
        keys.Add(key);
        bool isPercent = key == "attackSpeedRate" || key == "critRate";
        vals.Add(isPercent ? Mathf.RoundToInt(value * 100) + "%" : value.ToString("0.##"));
    }
}