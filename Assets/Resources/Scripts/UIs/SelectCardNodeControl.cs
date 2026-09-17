using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonConfig;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 羁绊类型
public enum BondKind
{
    None = 0,
    Job,    // 职业羁绊
    Friend, // 好友羁绊
    Force,  // 国家羁绊
}

// 羁绊提示数据：MySelectControl 填充，SelectCardNodeControl 点击时传给 TooltipFriend 显示
public class BondTipData
{
    public BondKind Kind;     // 羁绊类型
    public string Name;       // 羁绊名（职业名/好友组名/势力名）
    public int Level;         // 当前等级：职业=上阵人数(1起)；好友/国家=人数-1(可为0)
    public Color? Color;      // 名称颜色（好友连线色，可为空）
    public string Icon;       // 图标：职业/好友=技能图标名；国家="Textures/Icons/sideX"
    public string SkillId;    // 技能缩写（国家=国 护盾展示技能）
    public List<int> HeroIds; // 该羁绊当前上阵的英雄（国家不显示列表，可为空）
    public PlayerInfo Player; // 被查看的玩家（人员列表按品质上色判断用）
}

public class SelectCardNodeControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int cardId;

    public TMP_Text cardName;
    public Image expBar;
    public Image jobImg;
    public bool isChecked;

    // 羁绊提示数据（羁绊模式填充；英雄模式不填 → 点击不弹提示）
    private BondTipData bondTip;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 按下显示羁绊提示（与商店/背包卡牌交互一致：按下弹Tip，抬起收起）
    public void OnPointerDown(PointerEventData eventData)
    {
        if (bondTip != null)
            PanelManager.Instance.GetTooltip<TooltipFriend>()?.ShowTooltip(bondTip);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PanelManager.Instance.GetTooltip<BaseTooltip>()?.HideTooltip();
    }

    // 羁绊模式显示：隐藏经验条，名字直接为"等级+羁绊名"，图标为空则不显示；颜色为羁绊连线色（可为空=默认白）
    public void UpdateBond(BondTipData data)
    {
        bondTip = data;
        expBar.gameObject.SetActive(false);
        cardName.text = data.Level + data.Name;
        cardName.color = data.Color.HasValue ? data.Color.Value : Color.white;
        if (string.IsNullOrEmpty(data.Icon))
        {
            jobImg.gameObject.SetActive(false);
        }
        else
        {
            jobImg.gameObject.SetActive(true);
            // 带路径的图标（如国家 Textures/Icons/xxx）直接加载，否则按技能图标 SkillPic/ 前缀
            var loadPath = data.Icon.Contains("/") ? data.Icon : "Textures/SkillPic/" + data.Icon;
            jobImg.sprite = Resources.Load<Sprite>(loadPath);
        }
    }

    public void UpdateExp(int pid, string name, int exp, string icon)
    {
        bondTip = null;
        expBar.gameObject.SetActive(true);
        cardName.color = Color.white;
        expBar.rectTransform.sizeDelta = new Vector2(194 * HeroSelectionTool.GetExpRate(exp, true), 70);
        cardName.text = HeroSelectionTool.GetCardLevel(exp, true) + name;
        if (string.IsNullOrEmpty(icon))
        {
            jobImg.gameObject.SetActive(false);
        }
        else
        {
            jobImg.gameObject.SetActive(true);
            jobImg.sprite = Resources.Load<Sprite>("Textures/SkillPic/" + icon);
        }

    }
}
