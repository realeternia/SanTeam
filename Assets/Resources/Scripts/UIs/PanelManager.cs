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

    public GameObject tipNode;

    // Tooltip 运行时动态创建并挂到 tipNode 下（首次访问时加载，之后缓存复用）
    private Tooltip tooltip;

    public List<GameObject> openPanelList;

    // Start is called before the first frame update
    void Start()
    {
        // 面板已改为动态加载，开局先创建选牌面板（其 Start 会继续走游戏初始化流程）
        // Tooltip 按需创建：首次经 GetTooltip() 实例化并挂到 tipNode 下，调用入口统一走 PanelManager
        ShowPick();
    }

    // 从 Resources/Prefabs 动态加载并实例化 Tooltip（挂到 tipNode 下，只创建一次）
    public Tooltip GetTooltip()
    {
        if (tooltip != null)
            return tooltip;
        if (tipNode == null)
        {
            GameLog.Error("PanelManager tipNode 未在场景中配置");
            return null;
        }
        var prefab = Resources.Load<GameObject>("Prefabs/ToolTipHero");
        if (prefab == null)
        {
            GameLog.Error("PanelManager ToolTipHero 预制体加载失败: Prefabs/ToolTipHero");
            return null;
        }
        var go = Instantiate(prefab, tipNode.transform);
        tooltip = go.GetComponent<Tooltip>();
        if (tooltip == null)
            GameLog.Error("PanelManager ToolTipHero 预制体上缺少 Tooltip 组件");
        return tooltip;
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
