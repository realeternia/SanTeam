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

    public GameObject loadGamePanel;
    public Button loadGameBtn;
    public Button newGameBtn;

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
            FinishLikePhase();
        });
        okBtn.onClick.AddListener(() =>
        {
            refreshBtn.gameObject.SetActive(false); // ok后，不能再refresh
            foreach (var cell in cellControls)
                cell.canLike = true;

            StartCoroutine(AllPlayerLikes());
            okBtn.gameObject.SetActive(false);
        });

        finBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        refreshBtn.gameObject.SetActive(false);

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
        if(GameManager.Instance.IsGameSaveExist())
        {
            loadGamePanel.SetActive(true);
            loadGameBtn.onClick.AddListener(() =>
            {
                var isSuccess = GameManager.Instance.LoadFromSave();
                if(isSuccess)
                {
                    GameManager.Instance.InitFriend(true);
                    GameManager.Instance.InitHeros(true);
                    PanelManager.Instance.ShowShop();
                    PanelManager.Instance.HidePick();
                }
                else
                {
                    loadGamePanel.SetActive(false);
                    GameManager.Instance.InitHeros(false);
                    GameManager.Instance.InitFriend(false);
                    RefreshBtnClick();
                }
            });
            newGameBtn.onClick.AddListener(() =>
            {
                loadGamePanel.SetActive(false);
                GameManager.Instance.InitHeros(false);
                GameManager.Instance.InitFriend(false);
                RefreshBtnClick();
            });            
        }
        else
        {
            loadGamePanel.SetActive(false);
            GameManager.Instance.InitHeros(false);
            GameManager.Instance.InitFriend(false);
            RefreshBtnClick();
        }

    }

    // 显示开始面板：startText 循环缩放，任意点击后进入选牌流程
    private void ShowStartPanel()
    {
        startPanel.SetActive(true);

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

    private void OnStartPanelClick()
    {
        if (startTextTween != null)
        {
            startTextTween.Kill();
            startTextTween = null;
        }

        startPanel.SetActive(false);
        SetPlayerInfoVisible(true);
        BeginSelection();
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
