using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CommonConfig;

public class PickPanelControl : MonoBehaviour
{
    public GameObject pickPanelCellPrefab; // 引用PickPanelCell预制体
    public Transform cellParent; // 用于放置单元格的父容器

    public Button refreshBtn;
    public TMP_Text refreshText;
    public Button okBtn;
    private int refreshCount = 4;

    private List<PickPanelCellControl> cellControls = new List<PickPanelCellControl>();

    // ---- 许愿面板运行时状态 ----
    private int wishSlot = -1;
    private int wishPid = -1;
    private System.Action wishDoneCallback;
    private PickPanelCellControl pendingWishCell;

    private Tween startTextTween;
    private const float StartPanelFadeDuration = 1.5f; // 开始面板点击后渐隐时长
    private bool startPanelClicked;

    public GameObject loadGamePanel;
    public Button loadGameBtn;
    public Button newGameBtn;

    // 存档列表容器（运行时创建）与生成的存档项
    private RectTransform loadItemRoot;
    private readonly List<LoadGameItem> loadItems = new List<LoadGameItem>();
    private int selectedSlot = -1;

    // 删除确认模式：>=0 表示待删除的槽位，此时两个按钮变为「取消删除/确认删除」
    private int pendingDeleteSlot = -1;
    private TMP_Text loadGameBtnText;
    private TMP_Text newGameBtnText;
    private string loadGameBtnOriginText;
    private string newGameBtnOriginText;

    public GameObject startPanel;
    public TMP_Text startText;

    public Button finBtn;


    private void Awake()
    {

    }
 
    // Start is called before the first frame update
    void Start()
    {
        // 许愿面板：确定（提交选中的英雄）与关闭（暂时不选）
        okBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            OnWishConfirm();
        });
        finBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            OnWishClose();
        });

        finBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        refreshBtn.gameObject.SetActive(false);

        // 存档面板：读档按钮默认隐藏，选中某个存档项后再显示
        loadGameBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            OnLoadGameClick();
        });
        newGameBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            OnNewGameClick();
        });
        loadGameBtn.gameObject.SetActive(false);

        // 缓存两个按钮的文本与原始文字（删除确认模式下需临时改成取消/确认删除）
        loadGameBtnText = GetButtonText(loadGameBtn);
        newGameBtnText = GetButtonText(newGameBtn);
        loadGameBtnOriginText = loadGameBtnText != null ? loadGameBtnText.text : "";
        newGameBtnOriginText = newGameBtnText != null ? newGameBtnText.text : "";

        PanelManager.Instance.ShowPick();

        // 启动画面（Unity 内置）结束后进入开始面板：隐藏读档/新游戏面板（含 loadGameBtn、newGameBtn）、开始面板与上方玩家信息
        loadGamePanel.SetActive(false);
        startPanel.SetActive(false);
        SetPlayerInfoVisible(false);
        PlayIntro();
    }

    // 进入开始面板：播放开始 BGM，再展示开始面板
    private void PlayIntro()
    {
        BGMPlayer.Instance.PlaySound("BGMs/start");

        ShowStartPanel();
    }

    // 上方玩家信息（TopBar 上每个玩家一个 PlayerInfo）
    private void SetPlayerInfoVisible(bool visible)
    {
        if (GameManager.Instance == null || GameManager.Instance.players == null)
        {
            GameLog.Warn("PickPanelControl 获取玩家列表失败，跳过上方玩家信息显隐");
            return;
        }

        foreach (var player in GameManager.Instance.players)
        {
            if (player == null)
                continue;
            // 显示时按淘汰状态处理（已淘汰/生命≤0的玩家不能因为开面板重新复活），隐藏时全部隐藏
            if (visible)
                player.ApplyEliminatedState();
            else
                player.gameObject.SetActive(false);
        }
    }

    // 点击开始面板后的原有选牌流程（读档 / 新游戏 → 刷新英雄池）
    private void BeginSelection()
    {
        if (GameManager.Instance.GetUsedSaveSlots().Count > 0)
        {
            loadGamePanel.SetActive(true);
            BuildLoadGameItems();
        }
        else
        {
            StartNewGame();
        }
    }

    // 按已用存档数量生成存档项列表
    private void BuildLoadGameItems()
    {
        if (loadItemRoot == null)
        {
            var rootGo = new GameObject("LoadGameItemRoot", typeof(RectTransform));
            rootGo.transform.SetParent(loadGamePanel.transform, false);
            loadItemRoot = rootGo.GetComponent<RectTransform>();
            loadItemRoot.anchorMin = loadItemRoot.anchorMax = new Vector2(0.5f, 0.5f);
            loadItemRoot.pivot = new Vector2(0.5f, 0.5f);
            loadItemRoot.anchoredPosition = Vector2.zero;
            loadItemRoot.sizeDelta = new Vector2(600, 900);
        }

        for (int i = loadItemRoot.childCount - 1; i >= 0; i--)
            Destroy(loadItemRoot.GetChild(i).gameObject);
        loadItems.Clear();

        // 重建列表时退出删除确认模式
        if (pendingDeleteSlot >= 0)
            ExitDeleteMode();

        selectedSlot = -1;
        loadGameBtn.gameObject.SetActive(false);

        var slots = GameManager.Instance.GetUsedSaveSlots();
        var prefab = Resources.Load<GameObject>("Prefabs/UIs/Cells/LoadGameItem");
        if (prefab == null)
        {
            GameLog.Error("PickPanelControl 加载存档项预制体失败: Prefabs/UIs/Cells/LoadGameItem");
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            int slot = slots[i];
            var summary = GameManager.Instance.GetSaveSummary(slot);

            GameObject go = Instantiate(prefab, loadItemRoot);
            go.transform.localScale = Vector3.one;

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(-220, 260 - i * 130);
            rt.sizeDelta = new Vector2(600, 120);

            var item = go.GetComponent<LoadGameItem>();
            item.Setup(slot, summary, OnLoadItemSelected, OnLoadItemDeleteRequested);
            loadItems.Add(item);
        }

        // 新游戏始终可用（槽位已满时覆盖选中的存档）
        newGameBtn.gameObject.SetActive(true);

        // 默认选中第一个存档，使读档按钮立即可见
        if (loadItems.Count > 0)
            OnLoadItemSelected(loadItems[0].Slot);
    }

    // 选中某存档项：背景变色并显示读档按钮；切换到其它项时退出删除确认模式
    private void OnLoadItemSelected(int slot)
    {
        if (pendingDeleteSlot >= 0 && pendingDeleteSlot != slot)
            ExitDeleteMode();

        selectedSlot = slot;
        foreach (var item in loadItems)
            item.SetSelected(item.Slot == slot);
        loadGameBtn.gameObject.SetActive(true);
    }

    // 点击存档项的删除：进入删除确认模式（继续游戏→取消删除，重新开始→确认删除）
    private void OnLoadItemDeleteRequested(int slot)
    {
        EnterDeleteMode(slot);
    }

    // 进入删除确认模式：按钮文字变化 + DoTween 效果，被点项的删除按钮置灰
    private void EnterDeleteMode(int slot)
    {
        pendingDeleteSlot = slot;
        selectedSlot = slot;
        loadGameBtn.gameObject.SetActive(true);

        foreach (var item in loadItems)
        {
            item.SetSelected(item.Slot == slot);
            item.SetDeletePending(item.Slot == slot);
        }

        SetConfirmButtons("取消删除", "确认删除");
    }

    // 退出删除确认模式：还原按钮文字与存档项删除按钮外观
    private void ExitDeleteMode()
    {
        pendingDeleteSlot = -1;

        foreach (var item in loadItems)
            item.SetDeletePending(false);

        SetConfirmButtons(loadGameBtnOriginText, newGameBtnOriginText);
    }

    private void SetConfirmButtons(string loadText, string newText)
    {
        if (loadGameBtnText != null)
            loadGameBtnText.text = loadText;
        if (newGameBtnText != null)
            newGameBtnText.text = newText;

        PlayButtonChangeEffect(loadGameBtn);
        PlayButtonChangeEffect(newGameBtn);
    }

    // 按钮文字变化时的 DoTween 弹跳效果
    private void PlayButtonChangeEffect(Button btn)
    {
        if (btn == null)
            return;

        var t = btn.transform;
        t.DOKill();
        t.localScale = Vector3.one;
        t.DOPunchScale(Vector3.one * 0.15f, 0.3f, 8, 1);
    }

    private TMP_Text GetButtonText(Button btn)
    {
        if (btn == null)
            return null;

        var text = btn.GetComponentInChildren<TMP_Text>(true);
        if (text == null)
            GameLog.Warn("PickPanelControl 按钮未找到文本子节点: " + btn.name);
        return text;
    }

    // 确认删除：删除待删槽位并重建列表；全部删完则进入新游戏
    private void ConfirmDelete()
    {
        int slot = pendingDeleteSlot;
        ExitDeleteMode();

        GameManager.Instance.DeleteSave(slot);

        if (GameManager.Instance.GetUsedSaveSlots().Count == 0)
        {
            loadGamePanel.SetActive(false);
            StartNewGame();
        }
        else
        {
            BuildLoadGameItems();
        }
    }

    // 点击读档：删除确认模式下作为「取消删除」；否则加载选中槽位并进入游戏
    private void OnLoadGameClick()
    {
        if (pendingDeleteSlot >= 0)
        {
            ExitDeleteMode();
            return;
        }

        if (selectedSlot < 0)
        {
            GameLog.Warn("PickPanelControl 未选中任何存档，忽略读档");
            return;
        }

        // 读档前先整体重置玩家，避免存档中未覆盖的字段沿用上一局残留数据
        GameManager.Instance.ResetPlayersForNewGame();

        if (GameManager.Instance.LoadFromSave(selectedSlot))
        {
            GameManager.Instance.InitFriend(true);
            GameManager.Instance.InitHeros(true);
            CardShopManager.RequestResetGameState();
            EnterShop();
        }
        else
        {
            GameLog.Error("PickPanelControl 读档失败 slot=" + selectedSlot);
        }
    }

    // 点击新游戏：删除确认模式下作为「确认删除」；否则优先分配新槽位（槽位已满则覆盖选中的存档）
    private void OnNewGameClick()
    {
        if (pendingDeleteSlot >= 0)
        {
            ConfirmDelete();
            return;
        }

        int slot = GameManager.Instance.CreateNewSaveSlot();
        if (slot < 0)
        {
            int overwrite = selectedSlot >= 0 ? selectedSlot : 0;
            GameManager.Instance.currentSaveSlot = overwrite;
            GameLog.Warn("存档槽位已满，新游戏将覆盖槽位 " + overwrite);
        }
        StartNewGame();
    }

    // 开始一局新游戏：不再展示选牌/like 面板，所有英雄直接进入随机池并进入商店
    private void StartNewGame()
    {
        if (GameManager.Instance.currentSaveSlot < 0)
            GameManager.Instance.CreateNewSaveSlot();

        CardShopManager.RequestResetGameState();
        // 重置全部玩家，避免上一局的卡片/等级/生命等数据残留到新的一局
        GameManager.Instance.ResetPlayersForNewGame();
        loadGamePanel.SetActive(false);
        GameManager.Instance.InitHeros(false);
        GameManager.Instance.InitFriend(false);
        EnterShop();
    }

    // 从开始界面进入商店：面板若已创建（回到开始界面后再开一局）需手动重新走一遍 ShopBegin
    private void EnterShop()
    {
        SetPlayerInfoVisible(true);
        bool shopExisted = PanelManager.Instance.cardShopPanel != null;
        PanelManager.Instance.ShowShop();
        if (shopExisted && PanelManager.Instance.cardShopPanel != null)
            PanelManager.Instance.cardShopPanel.GetComponent<CardShopManager>()?.ShopBegin();
        PanelManager.Instance.HidePick();
    }

    // 显示开始面板：startText 循环缩放，任意点击后进入选牌流程
    private void ShowStartPanel()
    {
        startPanel.SetActive(true);
        startPanelClicked = false;

        // 渐隐用 CanvasGroup（预制体未挂时运行时添加）
        var canvasGroup = startPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = startPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        var startBtn = startPanel.GetComponent<Button>();
        if (startBtn == null)
        {
            startBtn = startPanel.AddComponent<Button>();
            startBtn.transition = Selectable.Transition.None;
            startBtn.targetGraphic = startPanel.GetComponent<Graphic>();
        }
        startBtn.onClick.AddListener(OnStartPanelClick);

        PlayStartTextLoop();
    }

    // 开始文字循环缩放动画
    private void PlayStartTextLoop()
    {
        if (startText == null)
        {
            GameLog.Warn("PickPanelControl.startText 未绑定，跳过开始文字动画");
            return;
        }

        startText.transform.localScale = Vector3.one;
        startTextTween = startText.transform
            .DOScale(1.15f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }

    // 点击开始面板：播放音效 → 渐隐 → 消失后显示 LoadSave 页面（含上方玩家信息）
    private void OnStartPanelClick()
    {
        if (startPanelClicked)
            return;
        startPanelClicked = true;

        if (startTextTween != null)
        {
            startTextTween.Kill();
            startTextTween = null;
        }

        GameManager.Instance.PlaySound("Sounds/biang");

        var canvasGroup = startPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = startPanel.AddComponent<CanvasGroup>();

        canvasGroup.DOFade(0f, StartPanelFadeDuration).OnComplete(() =>
        {
            startPanel.SetActive(false);
            canvasGroup.alpha = 1f; // 还原透明度，便于后续复用

            // 玩家信息保持隐藏，进入商店时才显示
            BeginSelection();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ---- 许愿面板（复用选牌面板的格子区域；隐藏品质1的卡，确定/关闭按钮见 SetupWishButtons）----

    // 打开许愿面板：由 BagControl 的 loveBtn 调用（PanelManager.ShowWish 已激活本面板）
    public void ShowWishPanel(int slot, int pid, System.Action onDone)
    {
        wishSlot = slot;
        wishPid = pid;
        wishDoneCallback = onDone;
        pendingWishCell = null;

        // 隐藏开始/读档面板与刷新按钮，只保留许愿格子和确定/关闭
        startPanel.SetActive(false);
        loadGamePanel.SetActive(false);
        refreshBtn.gameObject.SetActive(false);

        SetupWishButtons();
        RefreshWishCells();
    }

    // 配置确定（底部居中）与关闭（右上角）按钮
    private void SetupWishButtons()
    {
        var okText = GetButtonText(okBtn);
        if (okText != null)
            okText.text = "确定";
        var finText = GetButtonText(finBtn);
        if (finText != null)
            finText.text = "关闭";

        var okRt = okBtn.GetComponent<RectTransform>();
        if (okRt != null)
        {
            okRt.anchorMin = okRt.anchorMax = new Vector2(0.5f, 0f);
            okRt.pivot = new Vector2(0.5f, 0f);
            okRt.anchoredPosition = new Vector2(0f, 24f);
        }

        var finRt = finBtn.GetComponent<RectTransform>();
        if (finRt != null)
        {
            finRt.anchorMin = finRt.anchorMax = new Vector2(1f, 1f);
            finRt.pivot = new Vector2(1f, 1f);
            finRt.anchoredPosition = new Vector2(-24f, -24f);
        }

        okBtn.gameObject.SetActive(true);
        finBtn.gameObject.SetActive(true);
    }

    // 刷新许愿格子：展示品质>1且未被该玩家许愿过的英雄
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
            .Where(heroId => !HeroSelectionTool.IsWishedHero(player, heroId))
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
        finBtn.gameObject.SetActive(false);
        PanelManager.Instance.HidePick();

        var callback = wishDoneCallback;
        wishDoneCallback = null;
        callback?.Invoke();
    }

}
