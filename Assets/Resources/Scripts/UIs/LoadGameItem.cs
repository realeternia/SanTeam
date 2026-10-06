using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class LoadGameItem : MonoBehaviour
{
    public Image bgImg;
    public TMP_Text infoText;
    public Button deleteBtn;

    // 上阵英雄头像容器（预制体未绑定时运行时创建）
    public RectTransform heroParent;

    private const float HeroIconSize = 60f; // 英雄头像尺寸
    private const float HeroIconGap = 4f;   // 头像间距

    private Button selfBtn;

    // 删除按钮原始外观（用于取消删除时还原）
    private Image deleteBtnImg;
    private TMP_Text deleteBtnText;
    private Color deleteImgOrigin;
    private Color deleteTextOrigin;
    private bool deleteVisualCached;

    /// <summary>本条对应的存档槽位</summary>
    public int Slot { get; private set; } = -1;

    private void Awake()
    {
        selfBtn = GetComponent<Button>();
    }

    // 初始化存档项：slot 槽位号，summary 存档摘要，onSelect 选中回调，onDelete 删除回调
    public void Setup(int slot, GameManager.SaveSummary summary, Action<int> onSelect, Action<int> onDelete)
    {
        Slot = slot;

        if (selfBtn == null)
            selfBtn = GetComponent<Button>();

        if (infoText != null)
        {
            string yearStr = GetYearText(summary.year);
            string markStr = SysColor.ColorText(summary.mark.ToString(), SysColor.UI.SaveMark);
            string goldStr = SysColor.ColorText(summary.gold.ToString(), SysColor.UI.SaveGold);
            string timeStr = SysColor.ColorText(GetTimeAgo(summary.saveTime), SysColor.UI.SaveTime);
            infoText.text = yearStr + "  积分:" + markStr + "  金钱:" + goldStr + "  " + timeStr;
        }

        if (selfBtn != null)
        {
            selfBtn.onClick.RemoveAllListeners();
            int s = slot;
            selfBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.PlaySound("Sounds/click");
                onSelect?.Invoke(s);
            });
        }

        if (deleteBtn != null)
        {
            deleteBtn.onClick.RemoveAllListeners();
            int s = slot;
            deleteBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.PlaySound("Sounds/click");
                onDelete?.Invoke(s);
            });
        }

        LayoutChildren();
        BuildHeroImages(summary.lineHeroes);
        SetSelected(false);
        SetDeletePending(false);
    }

    // 选中效果：选中时背景变色
    public void SetSelected(bool selected)
    {
        if (bgImg != null)
            bgImg.color = selected ? SysColor.Theme.CellSelected : SysColor.Theme.CellNormalDark;
    }

    // 删除待确认状态：点击删除后本项删除按钮置灰且不可再点
    public void SetDeletePending(bool pending)
    {
        CacheDeleteVisuals();
        if (deleteBtn != null)
            deleteBtn.interactable = !pending;
        if (deleteBtnImg != null)
            deleteBtnImg.color = pending ? SysColor.UI.BtnClickedGray : deleteImgOrigin;
        if (deleteBtnText != null)
            deleteBtnText.color = pending ? SysColor.Theme.DisabledTextColor : deleteTextOrigin;
    }

    private void CacheDeleteVisuals()
    {
        if (deleteVisualCached || deleteBtn == null)
            return;

        deleteBtnImg = deleteBtn.GetComponent<Image>();
        deleteBtnText = deleteBtn.GetComponentInChildren<TMP_Text>(true);
        if (deleteBtnImg != null)
            deleteImgOrigin = deleteBtnImg.color;
        if (deleteBtnText != null)
            deleteTextOrigin = deleteBtnText.color;
        deleteVisualCached = true;
    }

    // 年份显示走 GameRoundConfig（如 41 → "220年"），配置缺失时回退原始年份
    private string GetYearText(int year)
    {
        if (GameRoundConfig.HasConfig(year))
            return GameRoundConfig.GetConfig(year).Name;
        return year + "年";
    }

    // 存档时间显示：刚刚 / x分钟前 / x小时前 / x天前
    private string GetTimeAgo(DateTime time)
    {
        if (time == default(DateTime))
            return "";

        TimeSpan span = DateTime.Now - time;
        if (span.TotalMinutes < 1)
            return "刚刚";
        if (span.TotalHours < 1)
            return (int)span.TotalMinutes + "分钟前";
        if (span.TotalDays < 1)
            return (int)span.TotalHours + "小时前";
        return (int)span.TotalDays + "天前";
    }

    // 统一由代码排版（预制体布局为占位）
    private void LayoutChildren()
    {
        if (infoText != null)
        {
            var rt = infoText.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(10, -8);
            rt.sizeDelta = new Vector2(470, 34);
            infoText.fontSize = 28;
            infoText.alignment = TextAlignmentOptions.MidlineLeft;
        }

        if (deleteBtn != null)
        {
            var rt = deleteBtn.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-10, -8);
            rt.sizeDelta = new Vector2(100, 40);
        }
    }

    // 为每个上阵英雄创建头像，从左往右排列
    private void BuildHeroImages(List<int> heroIds)
    {
        if (heroParent == null)
            heroParent = CreateHeroContainer();

        for (int i = heroParent.childCount - 1; i >= 0; i--)
            Destroy(heroParent.GetChild(i).gameObject);

        if (heroIds == null)
            return;

        for (int i = 0; i < heroIds.Count; i++)
        {
            int heroId = heroIds[i];
            if (!HeroConfig.HasConfig(heroId))
            {
                GameLog.Warn("LoadGameItem 上阵英雄配置缺失 heroId=" + heroId);
                continue;
            }

            var go = new GameObject("HeroIcon", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(heroParent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(HeroIconSize, HeroIconSize);
            rt.anchoredPosition = new Vector2(i * (HeroIconSize + HeroIconGap), 0);

            var img = go.GetComponent<Image>();
            img.sprite = Resources.Load<Sprite>("Textures/Skins/" + HeroConfig.GetConfig(heroId).Icon);
            img.raycastTarget = false;
        }
    }

    private RectTransform CreateHeroContainer()
    {
        var go = new GameObject("HeroContainer", typeof(RectTransform));
        go.transform.SetParent(transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(12, -50);
        rt.sizeDelta = new Vector2(560, HeroIconSize);
        return rt;
    }
}