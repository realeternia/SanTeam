using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    public Button finBtn;


    private void Awake()
    {

    }
 
    // Start is called before the first frame update
    void Start()
    {
        refreshBtn.onClick.AddListener(() =>
        {
            RefreshBtnClick();
        });
        finBtn.onClick.AddListener(() =>
        {
            HeroSelectionTool.SetBanList(GetBanList());

            PanelManager.Instance.ShowShop();
            PanelManager.Instance.HidePick();
        });
        okBtn.onClick.AddListener(() =>
        {
            refreshBtn.gameObject.SetActive(false); // ok后，不能再refresh
            StartCoroutine(AllPlayerBans());
            okBtn.gameObject.SetActive(false);          
        });

        finBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        refreshBtn.gameObject.SetActive(false);

        StartCoroutine(DelaySetMode());
        PanelManager.Instance.ShowPick();
    
    }

    IEnumerator DelaySetMode()
    {
        yield return new WaitForSeconds(0.1f);
        WorldManager.Instance.isDebug = false;
        RefreshBtnClick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 玩家轮流ban
    private IEnumerator AllPlayerBans()
    {
        // 等待1秒
        yield return new WaitForSeconds(1f);

        for (int i = 1; i <= 10; i++)
        {
            var pid = (i % 5) + 1;
            var player = GameManager.Instance.GetPlayer(pid);
            if (player.banCount > 0)
            {
                player.CheckBan(cellControls);
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.5f));
            }
        }
        
        finBtn.gameObject.SetActive(true);

    }    

    private List<int> GetBanList()
    {
        List<int> banList = new List<int>();
        foreach (var cell in cellControls)
        {
            if (cell.banState > 0)
            {
                banList.Add(cell.heroId);
            }
        }
        return banList;
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
        
        // 每行显示10个，共5行
        int itemsPerRow = 10;
        int rows = 5;
        int totalItems = Mathf.Min(heroPool.Count, itemsPerRow * rows);
        
        // 单元格大小和间距
        float cellWidth = 114f;
        float cellHeight = 114f;
        float spacingX = 0f;
        float spacingY = 14f;

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
            float posX = col * (cellWidth + spacingX) + 60;
            float posY = -row * (cellHeight + spacingY) - 60;

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
                cellControl.heroImg.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);

                // 设置英雄名称
                cellControl.heroName.text = heroCfg.Name;

                if (heroCfg.Side == 1)
                    cellControl.bgImg.GetComponent<Image>().color = new Color(40 / 255f, 70 / 255f, 0 / 255f, 255 / 255f);
                else if (heroCfg.Side == 2)
                    cellControl.bgImg.GetComponent<Image>().color = new Color(0 / 255f, 35 / 255f, 100 / 255f, 255 / 255f);
                else if (heroCfg.Side == 3)
                    cellControl.bgImg.GetComponent<Image>().color = new Color(100 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
                else
                    cellControl.bgImg.GetComponent<Image>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f, 255 / 255f);

                // 默认隐藏禁止图标
                cellControl.forbidImg.gameObject.SetActive(false);
            }
        }
    }

}
