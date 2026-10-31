using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

/// <summary>
/// 许愿（like 英雄）面板：隐藏品质1的卡，玩家单选一位英雄后点“确定”写入许愿槽位；
/// 右上角“关闭”可暂时不选。由 PanelManager.ShowWish 打开、通过回调通知调用方刷新按钮。
/// </summary>
public class LikeHeroPanelControl : MonoBehaviour
{
    public GameObject pickPanelCellPrefab; // 引用 PickPanelCell 预制体
    public Transform cellParent; // 用于放置单元格的父容器
    public Button okBtn; // 底部“确定”
    public Button closeBtn; // 右上角“关闭”

    private List<PickPanelCellControl> cellControls = new List<PickPanelCellControl>();

    // ---- 许愿面板运行时状态 ----
    private int wishSlot = -1;
    private int wishPid = -1;
    private System.Action wishDoneCallback;
    private PickPanelCellControl pendingWishCell;

    // Awake 在 Instantiate 时同步执行：先绑定按钮，之后 ShowWishPanel 再显示它们，避免 Start 晚于打开面板导致按钮被隐藏
    private void Awake()
    {
        okBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            OnWishConfirm();
        });
        closeBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            OnWishClose();
        });

        okBtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
    }

    // ---- 许愿面板（格子里只展示品质>1且该玩家尚未许愿的英雄）----

    // 打开许愿面板：由 BagControl 的 loveBtn 调用（PanelManager.ShowWish 已激活本面板）
    public void ShowWishPanel(int slot, int pid, System.Action onDone)
    {
        wishSlot = slot;
        wishPid = pid;
        wishDoneCallback = onDone;
        pendingWishCell = null;

        SetupWishButtons();
        RefreshWishCells();
    }

    // 显示确定（底部居中）与关闭（右上角）按钮，文字与位置在预制体里已配置好
    private void SetupWishButtons()
    {
        okBtn.gameObject.SetActive(true);
        closeBtn.gameObject.SetActive(true);
    }

    // 刷新许愿格子：展示品质>1的英雄，已被该玩家许愿过的英雄保留并标记（不可重复选择）
    private void RefreshWishCells()
    {
        for (int i = cellParent.childCount - 1; i >= 0; i--)
            Destroy(cellParent.GetChild(i).gameObject);
        cellControls.Clear();

        var player = GameManager.Instance.GetPlayer(wishPid);
        List<int> heroPool = HeroSelectionTool.GetHeroPoolCache()
            .Where(heroId => HeroConfig.HasConfig(heroId))
            .Where(heroId => HeroConfig.GetConfig(heroId).Quality > 1) // 隐藏所有品质1的卡
            .Where(heroId => !ConfigManager.IsKingHero(heroId)) // 主公不参与许愿（与 AI 一致）
            // 排序：品质高的在前，同品质按阵营，再按英雄 Id
            .OrderByDescending(heroId => HeroConfig.GetConfig(heroId).Quality)
            .ThenBy(heroId => HeroConfig.GetConfig(heroId).Side)
            .ThenBy(heroId => heroId)
            .ToList();

        int itemsPerRow = 13;
        int rows = 7;
        int totalItems = Mathf.Min(heroPool.Count, itemsPerRow * rows);

        float cellWidth = 108f;
        float cellHeight = 108f;
        float spacingX = 0f;
        float spacingY = 2f;

        for (int i = 0; i < totalItems; i++)
        {
            int heroId = heroPool[i];
            HeroConfig heroCfg = HeroConfig.GetConfig(heroId);
            bool wished = HeroSelectionTool.IsWishedHero(player, heroId);

            GameObject cell = Instantiate(pickPanelCellPrefab, cellParent);
            cell.transform.localScale = Vector3.one;

            int row = i / itemsPerRow;
            int col = i % itemsPerRow;
            float posX = 5 + col * (cellWidth + spacingX) + 60;
            float posY = -5 - row * (cellHeight + spacingY) - 60;

            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(posX, posY);

            PickPanelCellControl cellControl = cell.GetComponent<PickPanelCellControl>();
            cellControl.heroId = heroId;
            cellControl.wishMode = true;
            cellControl.onWishSelected = OnWishCellSelected;
            cellControls.Add(cellControl);

            cellControl.heroImg.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
            var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
            var icon = skillCfgs.Count > 0 ? skillCfgs[0].Icon : "";
            cellControl.jobImg.sprite = Resources.Load<Sprite>("Textures/SkillPic/" + icon);

            cellControl.heroName.text = heroCfg.Name;
            cellControl.heroName.color = SysColor.GetQualityColor(heroCfg.Quality);
            cellControl.bgImg.color = SysColor.GetSideColor(heroCfg.Side);

            // 隐藏原“点赞/禁用”标记图（预制体里默认激活，会挡住整格点击并让卡片变色）
            if (cellControl.forbidImg != null)
                cellControl.forbidImg.gameObject.SetActive(false);

            // 已被该玩家许愿过的英雄：显示红心标记，不挂点击，无法重复选择
            if (wished)
            {
                if (cellControl.forbidImg != null)
                {
                    cellControl.forbidImg.sprite = Resources.Load<Sprite>("Textures/love");
                    cellControl.forbidImg.color = Color.white;
                    cellControl.forbidImg.raycastTarget = false;
                    cellControl.forbidImg.gameObject.SetActive(true);
                }
                cellControl.SetWishHighlight(false);
                continue;
            }

            // 整格可点击：根节点补一个透明按钮（子级按钮除原红心按钮外统一转发为“选中该英雄”）
            var rootBtn = cell.GetComponent<Button>();
            if (rootBtn == null)
            {
                rootBtn = cell.AddComponent<Button>();
                rootBtn.transition = Selectable.Transition.None;
                rootBtn.targetGraphic = cellControl.bgImg;
            }
            foreach (var btn in cell.GetComponentsInChildren<Button>(true))
            {
                if (btn == cellControl.banBtn)
                    continue;
                btn.onClick.AddListener(() => OnWishCellSelected(cellControl));
            }

            cellControl.SetWishHighlight(false);
        }
    }

    // 许愿格子点击：单选高亮（最终由“确定”提交）
    private void OnWishCellSelected(PickPanelCellControl cell)
    {
        if (wishSlot < 0 || cell == null)
            return;

        GameManager.Instance.PlaySound("Sounds/click");
        if (pendingWishCell != null && pendingWishCell != cell)
            pendingWishCell.SetWishHighlight(false);
        pendingWishCell = cell;
        cell.SetWishHighlight(true);
    }

    // 确定：提交当前选中的英雄到许愿槽位（选定后不可修改）
    private void OnWishConfirm()
    {
        if (wishSlot < 0)
            return;

        if (pendingWishCell == null)
        {
            SystemTip.Show("请先选择一位英雄");
            return;
        }

        var player = GameManager.Instance.GetPlayer(wishPid);
        if (HeroSelectionTool.GetWishHeroId(player, wishSlot) != 0)
        {
            SystemTip.Show("该许愿已选择，无法修改");
            CloseWishPanel();
            return;
        }

        HeroSelectionTool.SetWishHero(player, wishSlot, pendingWishCell.heroId);
        GameLog.Debug($"玩家{wishPid} 许愿槽位{wishSlot}选择英雄 {pendingWishCell.heroId}({HeroConfig.GetConfig(pendingWishCell.heroId).Name})");
        // 许愿结果立即写档
        GameManager.Instance.SaveToFile();
        CloseWishPanel();
    }

    // 关闭：暂时不选，直接关闭面板
    private void OnWishClose()
    {
        if (wishSlot < 0)
            return;
        CloseWishPanel();
    }

    private void CloseWishPanel()
    {
        wishSlot = -1;
        wishPid = -1;
        pendingWishCell = null;

        okBtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);
        PanelManager.Instance.HideLikeHero();

        var callback = wishDoneCallback;
        wishDoneCallback = null;
        callback?.Invoke();
    }
}
