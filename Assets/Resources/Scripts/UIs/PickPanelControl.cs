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
        refreshBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/page");
            GameManager.Instance.InitHeros(false);
            GameManager.Instance.InitFriend(false);            
            RefreshBtnClick();
        });
        finBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            FinishLikePhase();
        });
        okBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            refreshBtn.gameObject.SetActive(false); // ok后，不能再refresh
            foreach (var cell in cellControls)
                cell.canLike = true;

            StartCoroutine(AllPlayerLikes());
            okBtn.gameObject.SetActive(false);
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
            if (player != null)
                player.gameObject.SetActive(visible);
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

        if (GameManager.Instance.LoadFromSave(selectedSlot))
        {
            GameManager.Instance.InitFriend(true);
            GameManager.Instance.InitHeros(true);
            SetPlayerInfoVisible(true);
            PanelManager.Instance.ShowShop();
            PanelManager.Instance.HidePick();
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

    // 开始一局新游戏（更新英雄池/好友池并刷新选牌）
    private void StartNewGame()
    {
        if (GameManager.Instance.currentSaveSlot < 0)
            GameManager.Instance.CreateNewSaveSlot();

        loadGamePanel.SetActive(false);
        GameManager.Instance.InitHeros(false);
        GameManager.Instance.InitFriend(false);
        RefreshBtnClick();
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

    // 玩家轮流点赞
    private IEnumerator AllPlayerLikes()
    {
        // 等待1秒
        yield return new WaitForSeconds(.3f);

        for (int i = 1; i <= 7; i++)
        {
            var pid = (i % 7) + 1;
            var player = GameManager.Instance.GetPlayer(pid);
            if (player.likeCount > 0)
            {
                PlayerAI.CheckLike(player, cellControls);
                yield return new WaitForSeconds(SysRandom.Range(0.1f, 0.3f));
            }
        }
        
        finBtn.gameObject.SetActive(true);

    }    

    // like阶段结束：把全部玩家（人类+AI）点赞的卡牌写入收藏池（8玩家×2张共16张），
    // 收藏池会在每次刷新卡牌时有 LikeCardRefreshRate 概率重新刷出一张
    private void FinishLikePhase()
    {
        HeroSelectionTool.SetLikePool(GetLikePool());

        // 新游戏like阶段结束进入，存档一次
        GameManager.Instance.SaveToFile();

        SetPlayerInfoVisible(true);
        PanelManager.Instance.ShowShop();
        PanelManager.Instance.HidePick();
    }

    // 收集全部玩家点赞的卡牌（likeState > 0）
    private List<int> GetLikePool()
    {
        List<int> likeList = new List<int>();
        foreach (var cell in cellControls)
        {
            if (cell.likeState > 0)
            {
                likeList.Add(cell.heroId);
            }
        }
        return likeList;
    }


    private void RefreshBtnClick()
    {
        RefreshHeroPool();
        okBtn.gameObject.SetActive(true);
        
        refreshCount--;
        refreshText.text = "刷新(" + refreshCount + ")";
        if (refreshCount <= 0)
        {
            refreshBtn.gameObject.SetActive(false);
        }
        else
        {
            refreshBtn.gameObject.SetActive(true);
        }

    }


    void RefreshHeroPool()
    {
        // 销毁节点下所有子对象
        for (int i = cellParent.childCount - 1; i >= 0; i--)
        {
            Destroy(cellParent.GetChild(i).gameObject);
        }
        cellControls.Clear();


        // 获取英雄池缓存
        List<int> heroPool = HeroSelectionTool.GetHeroPoolCache();

        // 王(主公)不参与pick展示与ban，但仍保留在英雄池用于商店卡池
        heroPool = heroPool.Where(heroId => !ConfigManager.IsKingHero(heroId)).ToList();
        
        // 每行显示10个，共5行
        int itemsPerRow = 13;
        int rows = 7;
        int totalItems = Mathf.Min(heroPool.Count, itemsPerRow * rows);
        
        // 单元格大小和间距
        float cellWidth = 108f;
        float cellHeight = 108f;
        float spacingX = 0f;
        float spacingY = 2f;

        // 创建单元格
        for (int i = 0; i < totalItems; i++)
        {
            int heroId = heroPool[i];
            HeroConfig heroCfg = HeroConfig.GetConfig(heroId);

            // 实例化单元格
            GameObject cell = Instantiate(pickPanelCellPrefab, cellParent);
            cell.transform.localScale = Vector3.one;

            // 计算位置
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;
            float posX = 5 + col * (cellWidth + spacingX) + 60;
            float posY = -5 -row * (cellHeight + spacingY) - 60;

            // 设置位置
            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(posX, posY);

            // 设置单元格数据
            PickPanelCellControl cellControl = cell.GetComponent<PickPanelCellControl>();
            cellControl.heroId = heroId;
            cellControls.Add(cellControl);
            if (cellControl != null)
            {
                // 设置英雄图片
                cellControl.heroImg.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
                // 设置job图片（英雄第一个技能图标，与战斗一致按缩写解析）
                var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
                var icon = skillCfgs.Count > 0 ? skillCfgs[0].Icon : "";
                cellControl.jobImg.sprite = Resources.Load<Sprite>("Textures/SkillPic/" + icon);

                // 设置英雄名称，颜色按品质
                cellControl.heroName.text = heroCfg.Name;
                cellControl.heroName.color = SysColor.GetQualityColor(heroCfg.Quality);

                cellControl.bgImg.GetComponent<Image>().color = SysColor.GetSideColor(heroCfg.Side);

                // 默认隐藏点赞图标
                cellControl.forbidImg.gameObject.SetActive(false);
            }
        }
    }

}
