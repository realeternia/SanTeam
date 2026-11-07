using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CommonConfig;

/// <summary>
/// 登录/存档面板：开始画面点击后展示存档列表（读档或新游戏），进入商店后隐藏。
/// 该脚本由 PanelManager 在加载 SaveLoadPanel 预制体时动态补挂，节点引用在 Awake 中按预制体结构查找。
/// </summary>
public class SaveLoadPanelControl : MonoBehaviour
{
    private GameObject startPanel;
    private TMP_Text startText;
    private GameObject loadGamePanel;
    private Button loadGameBtn;
    private Button newGameBtn;

    private Tween startTextTween;
    private const float StartPanelFadeDuration = 1.5f; // 开始面板点击后渐隐时长
    private bool startPanelClicked;

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

    // 剧本选择面板（新游戏时展示，运行时创建）：3 个剧本图按钮 + 描述 + 高品质武将头像 + 确定按钮
    private GameObject scenarioPanel;
    private readonly List<int> scenarioIds = new List<int>();
    private readonly List<RawImage> scenarioImages = new List<RawImage>();
    private int selectedScenarioId = -1;
    private TMP_Text scenarioDesText;
    private RectTransform scenarioAvatarRoot;

    private const float ScenarioImageWidth = 432f;  // 剧本图宽（720:320 = 72:32 等比）
    private const float ScenarioImageHeight = 192f; // 剧本图高（432:192 = 72:32）
    private const float ScenarioImageGap = 48f;     // 剧本按钮横向间距
    private const float ScenarioRowY = 120f;        // 剧本按钮行相对面板中心Y偏移
    private const float ScenarioDesY = -60f;        // 剧本历史描述相对面板中心Y偏移
    private const float ScenarioAvatarY = -150f;    // 高品质武将头像行相对面板中心Y偏移
    private const float ScenarioAvatarSize = 96f;   // 武将头像尺寸
    private const float ScenarioAvatarGap = 10f;    // 武将头像横向间距
    private const int ScenarioAvatarCount = 10;     // 预览的高品质武将数量

    private void Awake()
    {
        // 预制体未绑定脚本，节点引用按固定结构在运行时查找
        startPanel = transform.Find("StartPanel") != null ? transform.Find("StartPanel").gameObject : null;
        startText = transform.Find("StartPanel/Text (TMP)") != null ? transform.Find("StartPanel/Text (TMP)").GetComponent<TMP_Text>() : null;
        loadGamePanel = transform.Find("LoadSavePanel") != null ? transform.Find("LoadSavePanel").gameObject : null;
        loadGameBtn = transform.Find("LoadSavePanel/ButtonLoad") != null ? transform.Find("LoadSavePanel/ButtonLoad").GetComponent<Button>() : null;
        newGameBtn = transform.Find("LoadSavePanel/Cancel") != null ? transform.Find("LoadSavePanel/Cancel").GetComponent<Button>() : null;

        if (startPanel == null || startText == null || loadGamePanel == null || loadGameBtn == null || newGameBtn == null)
            GameLog.Error("SaveLoadPanelControl 子节点绑定失败，请检查 SaveLoadPanel 预制体结构");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (startPanel == null || loadGamePanel == null || loadGameBtn == null || newGameBtn == null)
            return;

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

        // 启动画面（Unity 内置）结束后进入开始面板：隐藏读档/新游戏面板、开始面板与上方玩家信息
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
            GameLog.Warn("SaveLoadPanelControl 获取玩家列表失败，跳过上方玩家信息显隐");
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

    // 点击开始面板后的流程（读档 / 新游戏 → 进入商店）
    private void BeginSelection()
    {
        if (GameManager.Instance.GetUsedSaveSlots().Count > 0)
        {
            loadGamePanel.SetActive(true);
            BuildLoadGameItems();
        }
        else
        {
            ShowScenarioPanel();
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
            GameLog.Error("SaveLoadPanelControl 加载存档项预制体失败: Prefabs/UIs/Cells/LoadGameItem");
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
            GameLog.Warn("SaveLoadPanelControl 按钮未找到文本子节点: " + btn.name);
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
            ShowScenarioPanel();
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
            GameLog.Warn("SaveLoadPanelControl 未选中任何存档，忽略读档");
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
            GameLog.Error("SaveLoadPanelControl 读档失败 slot=" + selectedSlot);
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
        ShowScenarioPanel();
    }

    // 新游戏剧本选择：3 个剧本图按钮（选中高亮、未选中暗调）+ 确定进入游戏
    private void ShowScenarioPanel()
    {
        loadGamePanel.SetActive(false);
        if (scenarioPanel == null)
            BuildScenarioPanel();
        scenarioPanel.SetActive(true);
        scenarioPanel.transform.SetAsLastSibling();
        SelectScenario(GameConst.DefaultScenarioId);
    }

    // 运行时创建剧本选择面板（预制体未提供该节点，全部按代码布局）
    private void BuildScenarioPanel()
    {
        scenarioPanel = new GameObject("ScenarioPanel", typeof(RectTransform), typeof(Image));
        scenarioPanel.transform.SetParent(transform, false);

        var panelRt = scenarioPanel.GetComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = Vector2.zero;
        panelRt.offsetMax = Vector2.zero;

        var panelImg = scenarioPanel.GetComponent<Image>();
        panelImg.color = SysColor.Theme.CellNormalDark;
        panelImg.raycastTarget = true;

        CreateScenarioText("ScenarioTitle", "选择剧本", new Vector2(0, 300), new Vector2(800, 90), 60);
        scenarioDesText = CreateScenarioText("ScenarioDes", "", new Vector2(0, ScenarioDesY), new Vector2(1200, 100), 30);
        scenarioAvatarRoot = CreateScenarioAvatarRoot();

        scenarioIds.Clear();
        scenarioImages.Clear();

        var scenarios = ScenarioConfig.ConfigList.OrderBy(c => c.Id).ToList();
        if (scenarios.Count == 0)
        {
            GameLog.Error("SaveLoadPanelControl 剧本配置为空，跳过剧本按钮创建");
            return;
        }

        float totalWidth = scenarios.Count * ScenarioImageWidth + (scenarios.Count - 1) * ScenarioImageGap;
        for (int i = 0; i < scenarios.Count; i++)
        {
            var cfg = scenarios[i];
            int scenarioId = cfg.Id;

            var go = new GameObject("Scenario_" + scenarioId, typeof(RectTransform), typeof(RawImage), typeof(Button));
            go.transform.SetParent(scenarioPanel.transform, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(ScenarioImageWidth, ScenarioImageHeight);
            rt.anchoredPosition = new Vector2(
                -totalWidth / 2 + ScenarioImageWidth / 2 + i * (ScenarioImageWidth + ScenarioImageGap),
                ScenarioRowY);

            var rawImg = go.GetComponent<RawImage>();
            rawImg.texture = Resources.Load<Texture2D>("Textures/Scenario/" + cfg.Pic);
            if (rawImg.texture == null)
                GameLog.Error("SaveLoadPanelControl 剧本图片加载失败: Textures/Scenario/" + cfg.Pic);
            rawImg.raycastTarget = true;

            var btn = go.GetComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.targetGraphic = rawImg;
            btn.onClick.AddListener(() =>
            {
                GameManager.Instance.PlaySound("Sounds/click");
                SelectScenario(scenarioId);
            });

            scenarioIds.Add(scenarioId);
            scenarioImages.Add(rawImg);
        }

        BuildScenarioConfirmButton();
    }

    // 复用存档面板按钮上的 TMP 文本节点创建文本，保证字体资源一致
    private TMP_Text CreateScenarioText(string name, string content, Vector2 anchoredPos, Vector2 size, int fontSize)
    {
        var template = newGameBtnText != null ? newGameBtnText : loadGameBtnText;
        if (template == null)
        {
            GameLog.Warn("SaveLoadPanelControl 未找到可用文本模板，跳过剧本文本创建: " + name);
            return null;
        }

        var go = Instantiate(template.gameObject, scenarioPanel.transform);
        go.name = name;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var text = go.GetComponent<TMP_Text>();
        if (text == null)
        {
            GameLog.Warn("SaveLoadPanelControl 文本模板缺少 TMP_Text 组件: " + name);
            return null;
        }

        text.text = content;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        return text;
    }

    // 高品质武将头像容器（选中剧本后按该剧本重建头像）
    private RectTransform CreateScenarioAvatarRoot()
    {
        var go = new GameObject("ScenarioAvatars", typeof(RectTransform));
        go.transform.SetParent(scenarioPanel.transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, ScenarioAvatarY);
        rt.sizeDelta = new Vector2(
            ScenarioAvatarCount * ScenarioAvatarSize + (ScenarioAvatarCount - 1) * ScenarioAvatarGap,
            ScenarioAvatarSize);
        return rt;
    }

    // 按剧本重建前 10 名高品质武将头像（品质降序，同品质按英雄Id）
    private void BuildScenarioAvatars(int scenarioId)
    {
        if (scenarioAvatarRoot == null)
            return;

        for (int i = scenarioAvatarRoot.childCount - 1; i >= 0; i--)
            Destroy(scenarioAvatarRoot.GetChild(i).gameObject);

        var heroes = HeroConfig.ConfigList
            .Where(c => c != null && c.Id > 0 && HeroScenario.IsAvailable(c.Id, scenarioId))
            .OrderByDescending(c => HeroScenario.GetQuality(c.Id, scenarioId))
            .ThenBy(c => c.Id)
            .Take(ScenarioAvatarCount)
            .ToList();

        float totalWidth = heroes.Count * ScenarioAvatarSize + (heroes.Count - 1) * ScenarioAvatarGap;
        for (int i = 0; i < heroes.Count; i++)
        {
            var heroCfg = heroes[i];

            var go = new GameObject("Avatar_" + heroCfg.Id, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(scenarioAvatarRoot, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(ScenarioAvatarSize, ScenarioAvatarSize);
            rt.anchoredPosition = new Vector2(
                -totalWidth / 2 + ScenarioAvatarSize / 2 + i * (ScenarioAvatarSize + ScenarioAvatarGap),
                0);

            var img = go.GetComponent<Image>();
            img.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
            if (img.sprite == null)
                GameLog.Warn("SaveLoadPanelControl 武将头像加载失败: Textures/Skins/" + heroCfg.Icon);
            img.preserveAspect = true;
            img.raycastTarget = false;
        }
    }

    // 确定按钮：复用存档面板的绿色按钮，点击后按所选剧本进入游戏
    private void BuildScenarioConfirmButton()
    {
        if (loadGameBtn == null)
        {
            GameLog.Error("SaveLoadPanelControl 缺少按钮模板，无法创建剧本确定按钮");
            return;
        }

        var confirmGo = Instantiate(loadGameBtn.gameObject, scenarioPanel.transform);
        confirmGo.name = "ScenarioConfirm";
        confirmGo.SetActive(true);

        var confirmRt = confirmGo.GetComponent<RectTransform>();
        confirmRt.anchorMin = confirmRt.anchorMax = new Vector2(0.5f, 0.5f);
        confirmRt.pivot = new Vector2(0.5f, 0.5f);
        confirmRt.anchoredPosition = new Vector2(0, -300);
        confirmRt.sizeDelta = new Vector2(200, 80);

        var confirmText = confirmGo.GetComponentInChildren<TMP_Text>(true);
        if (confirmText != null)
            confirmText.text = "确定";

        var confirmBtn = confirmGo.GetComponent<Button>();
        if (confirmBtn == null)
        {
            GameLog.Error("SaveLoadPanelControl 确定按钮缺少 Button 组件");
            return;
        }
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            OnScenarioConfirm();
        });
    }

    // 选中剧本：选中高亮、未选中暗调
    private void SelectScenario(int scenarioId)
    {
        if (!scenarioIds.Contains(scenarioId))
        {
            GameLog.Warn("SaveLoadPanelControl 剧本Id[" + scenarioId + "]不在可选列表中，改为选中第一个");
            scenarioId = scenarioIds.Count > 0 ? scenarioIds[0] : GameConst.DefaultScenarioId;
        }

        selectedScenarioId = scenarioId;
        for (int i = 0; i < scenarioImages.Count; i++)
        {
            if (scenarioImages[i] == null)
                continue;
            bool selected = scenarioIds[i] == scenarioId;
            scenarioImages[i].color = selected ? SysColor.Theme.ScenarioSelected : SysColor.Theme.ScenarioNormal;
        }

        // 切换剧本时刷新历史描述与该剧本的高品质武将头像
        if (scenarioDesText != null)
            scenarioDesText.text = ScenarioConfig.HasConfig(scenarioId) ? ScenarioConfig.GetConfig(scenarioId).Des : "";
        BuildScenarioAvatars(scenarioId);
    }

    // 点击确定：隐藏剧本面板并按所选剧本开始新游戏
    private void OnScenarioConfirm()
    {
        if (selectedScenarioId < 0)
        {
            GameLog.Warn("SaveLoadPanelControl 未选中剧本，使用默认剧本 " + GameConst.DefaultScenarioId);
            selectedScenarioId = GameConst.DefaultScenarioId;
        }

        if (scenarioPanel != null)
            scenarioPanel.SetActive(false);
        StartNewGame(selectedScenarioId);
    }

    // 开始一局新游戏：不再展示选牌/like 面板，所有英雄直接进入随机池并进入商店
    private void StartNewGame(int scenarioId)
    {
        if (GameManager.Instance.currentSaveSlot < 0)
            GameManager.Instance.CreateNewSaveSlot();

        CardShopManager.RequestResetGameState();
        // 所选剧本决定英雄池（ResetPlayersForNewGame 内按剧本重算），进而影响商店出卡
        GameManager.Instance.scenarioId = scenarioId;
        // 重置全部玩家，避免上一局的卡片/等级/生命等数据残留到新的一局
        GameManager.Instance.ResetPlayersForNewGame();
        loadGamePanel.SetActive(false);
        GameManager.Instance.InitHeros(false);
        GameManager.Instance.InitFriend(false);
        // 新开局首次进入商店时写一次存档（由 ShopBegin 消费该标记）
        GameManager.Instance.pendingNewGameSave = true;
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
        PanelManager.Instance.HideSaveLoad();
    }

    // 显示开始面板：startText 循环缩放，任意点击后进入读档/新游戏流程
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
            GameLog.Warn("SaveLoadPanelControl.startText 未绑定，跳过开始文字动画");
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
}
