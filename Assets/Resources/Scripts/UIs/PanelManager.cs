using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    // 面板改为运行时动态加载创建：首次 Show 时从 Resources/Prefabs 实例化并缓存，
    // Hide 只隐藏不销毁，避免商店等面板的运行时状态（回合/购买记录/单例引用）丢失
    public GameObject cardShopPanel;
    private GameObject rankPanel;
    private GameObject rankPlayerPanel;
    private GameObject pickPanel;
    private GameObject bagPanel;
    private GameObject sideBar;

    public GameObject tipNode;

    // Tooltip 运行时动态创建并挂到 tipNode 下（首次访问时加载，之后缓存复用）
    // 英雄/羁绊提示共用同一缓存：两者不会同时显示，新的创建前会隐藏旧的
    private BaseTooltip tooltip;

    public List<GameObject> openPanelList;

    // Start is called before the first frame update
    void Start()
    {
        // 面板已改为动态加载，开局先创建选牌面板（其 Start 会继续走游戏初始化流程）
        // Tooltip 按需创建：首次经 GetTooltip() 实例化并挂到 tipNode 下，调用入口统一走 PanelManager
        ShowPick();
    }

    // 从 Resources/Prefabs 动态加载并实例化 Tooltip（挂到 tipNode 下，每种只创建一次）
    // 泛型返回具体类型：填充内容用 GetTooltip<TooltipHero>() / GetTooltip<TooltipFriend>()（能拿到 ShowTooltip），
    // 仅隐藏用 GetTooltip<BaseTooltip>()（返回当前缓存的提示，不关心具体类型）
    public T GetTooltip<T>() where T : BaseTooltip
    {
        // 仅隐藏用：直接返回当前缓存的提示
        if (typeof(T) == typeof(BaseTooltip))
            return tooltip as T;
        // 填充用：缓存类型不匹配时重建并隐藏旧的（英雄/羁绊复用同一缓存，两者不会同时显示）
        if (tooltip == null || !(tooltip is T))
        {
            if (tooltip != null)
                tooltip.HideTooltip();
            tooltip = CreateTooltip(typeof(T) == typeof(TooltipFriend) ? "ToolTipFriend" : "ToolTipHero");
        }
        return tooltip as T;
    }

    // 加载指定 Tooltip 预制体并实例化到 tipNode 下
    private BaseTooltip CreateTooltip(string prefabName)
    {
        if (tipNode == null)
        {
            GameLog.Error("PanelManager tipNode 未在场景中配置");
            return null;
        }
        var prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
        if (prefab == null)
        {
            GameLog.Error($"PanelManager {prefabName} 预制体加载失败: Prefabs/{prefabName}");
            return null;
        }
        var go = Instantiate(prefab, tipNode.transform);
        var comp = go.GetComponent<BaseTooltip>();
        if (comp == null)
            GameLog.Error($"PanelManager {prefabName} 预制体上缺少 Tooltip 组件");
        return comp;
    }

    // 从 Resources/Prefabs 动态加载并实例化面板（挂在当前节点下，根节点为拉伸锚点铺满全屏）
    private GameObject LoadPanel(string prefabName)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
        if (prefab == null)
        {
            GameLog.Error($"PanelManager 面板预制体加载失败: Prefabs/{prefabName}");
            return null;
        }
        return Instantiate(prefab, transform);
    }

    public void ShowShop()
    {
        if (cardShopPanel == null)
            cardShopPanel = LoadPanel("ShopPanelBg");
        if (cardShopPanel == null)
            return;
        cardShopPanel.SetActive(true);
      //  cardShopTxt.SetActive(true);

        ChangePanelCount(cardShopPanel, true);
    }

    public void HideShop()
    {
        if (cardShopPanel == null)
            return;
        cardShopPanel.SetActive(false);
     //   cardShopTxt.SetActive(false);

        ChangePanelCount(cardShopPanel, false);
    }
    
    public void ShowBag()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        if (bagPanel == null)
            bagPanel = LoadPanel("BagPanel");
        if (bagPanel == null)
            return;
        bagPanel.SetActive(true);
        bagPanel.GetComponent<BagControl>().OnShow();

        ChangePanelCount(bagPanel, true);
    }

    public void HideBag()
    {
        if (bagPanel == null)
            return;
        GameManager.Instance.PlaySound("Sounds/deck");
        bagPanel.SetActive(false);
        bagPanel.GetComponent<BagControl>().OnHide();

        ChangePanelCount(bagPanel, false);
    }

    public void ShowRank()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        if (rankPanel == null)
            rankPanel = LoadPanel("RankInfoPanel");
        if (rankPanel == null)
            return;
        rankPanel.SetActive(true);
        rankPanel.GetComponent<RankPanelManager>().OnShow();

        ChangePanelCount(rankPanel, true);        
    }

    public void HideRank()
    {
        if (rankPanel == null)
            return;
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPanel.SetActive(false);
        rankPanel.GetComponent<RankPanelManager>().OnHide();

        ChangePanelCount(rankPanel, false);        
    }
    
    public void ShowRankPlayer()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        if (rankPlayerPanel == null)
            rankPlayerPanel = LoadPanel("PlayerInfoPanel");
        if (rankPlayerPanel == null)
            return;
        rankPlayerPanel.SetActive(true);
        rankPlayerPanel.GetComponent<RankPlayerPanelManager>().OnShow();

        ChangePanelCount(rankPlayerPanel, true);        
    }

    public void HideRankPlayer()
    {
        if (rankPlayerPanel == null)
            return;
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPlayerPanel.SetActive(false);
        rankPlayerPanel.GetComponent<RankPlayerPanelManager>().OnHide();

        ChangePanelCount(rankPlayerPanel, false);        
    }

    public void ShowPick()
    {
      //  GameManager.Instance.PlaySound("Sounds/deck");
        if (pickPanel == null)
            pickPanel = LoadPanel("PickPanelBg");
        if (pickPanel == null)
            return;
        pickPanel.SetActive(true);

        ChangePanelCount(pickPanel, true);
    }

    public void HidePick()
    {
        if (pickPanel == null)
            return;
     //   GameManager.Instance.PlaySound("Sounds/deck");
        pickPanel.SetActive(false);

        ChangePanelCount(pickPanel, false);
    }

    // 右侧滑出侧边栏：首次从 Resources/Prefabs/SideBar 实例化并缓存，子面板预制体按 panelName 加载到 SideBar 内容节点
    public void ShowSideBar(string panelName, System.Action<GameObject> onCreated = null)
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        if (sideBar == null)
            sideBar = LoadPanel("SideBar");
        if (sideBar == null)
            return;
        sideBar.SetActive(true);
        sideBar.GetComponent<SideBar>().OnShow(panelName, onCreated);

        ChangePanelCount(sideBar, true);
    }

    // 收起侧边栏：等滑出动画播完再隐藏并移除面板计数，避免中途被根节点隐藏打断动画
    public void HideSideBar()
    {
        if (sideBar == null)
            return;
        GameManager.Instance.PlaySound("Sounds/deck");
        sideBar.GetComponent<SideBar>().OnHide(() =>
        {
            sideBar.SetActive(false);
            ChangePanelCount(sideBar, false);
        });
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        GameLog.Debug($"PanelManager SendSignal {name} {parm1} {parm2}");
        foreach (var panel in openPanelList)
        {
            GameLog.Debug($"PanelManager SendSignal {panel.name} {name} {parm1} {parm2}");
            if (panel.TryGetComponent<IPanelEvent>(out IPanelEvent p))
                p.SendSignal(name, parm1, parm2);
        }
    }

    private void ChangePanelCount(GameObject panel, bool isShow)
    {
        if(isShow)
        {
            if(!openPanelList.Contains(panel))
                openPanelList.Add(panel);
        }
        else
            openPanelList.Remove(panel);
        if(openPanelList.Count <= 0)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
