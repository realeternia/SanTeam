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

    private const float HeroIconSize = 50f; // 英雄头像尺寸
    private const float HeroIconGap = 4f;   // 头像间距

    private Button selfBtn;

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
            infoText.text = "第" + summary.year + "年  积分:" + summary.mark + "  金钱:" + summary.gold;

        if (selfBtn != null)
        {
            selfBtn.onClick.RemoveAllListeners();
            int s = slot;
            selfBtn.onClick.AddListener(() => onSelect?.Invoke(s));
        }

        if (deleteBtn != null)
        {
            deleteBtn.onClick.RemoveAllListeners();
            int s = slot;
            deleteBtn.onClick.AddListener(() => onDelete?.Invoke(s));
        }

        LayoutChildren();
        BuildHeroImages(summary.lineHeroes);
        SetSelected(false);
    }

    // 选中效果：选中时背景变色
    public void SetSelected(bool selected)
    {
        if (bgImg != null)
            bgImg.color = selected ? SysColor.Theme.CellSelected : SysColor.Theme.CellNormalDark;
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
            rt.sizeDelta = new Vector2(440, 36);
            infoText.fontSize = 24;
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
        rt.anchoredPosition = new Vector2(12, -54);
        rt.sizeDelta = new Vector2(560, HeroIconSize);
        return rt;
    }
}