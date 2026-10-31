using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

// 侧边栏·英雄操作面板：展示英雄名与大图，提供"脱下所有装备"与"卖出"两个操作
// 打开流程：背包卖出区(BagRecycler)拖入英雄 → BagControl.OpenHeroPanel → ShowSideBar("SideHeroControl")
public class SideHeroControl : MonoBehaviour
{
    public TMP_Text textHeroName;
    public Image heroBigImage; // 英雄大图（Textures/SkinsBig，按 HeroConfig.Icon 取）
    public Image[] equipImages; // 大图下方一行最多 3 个装备图标，按槽位顺序
    public Button unwearAllButton;
    public Button sellButton;

    private static int currentPid;
    private static int currentHeroId;
    private static System.Action<int> onUnwearAll;
    private static System.Action<int> onSell;

    /// <summary>
    /// 打开侧边栏前调用：pid 指定取哪名玩家的背包，heroId 为拖入卖出区的英雄卡，
    /// unwearAll/sell 由调用方（BagControl）执行具体逻辑，参数为当前英雄卡 Id
    /// </summary>
    public static void SetContext(int pid, int heroId, System.Action<int> unwearAll, System.Action<int> sell)
    {
        currentPid = pid;
        currentHeroId = heroId;
        onUnwearAll = unwearAll;
        onSell = sell;

        GameLog.Info($"SideHeroControl.SetContext: pid={pid}, heroId={heroId}");
    }

    void Start()
    {
        if (unwearAllButton != null)
            unwearAllButton.onClick.AddListener(OnUnwearAll);
        if (sellButton != null)
            sellButton.onClick.AddListener(OnSell);

        RefreshView();
    }

    void RefreshView()
    {
        var heroCfg = HeroConfig.GetConfig(currentHeroId);
        if (heroCfg == null)
        {
            GameLog.Error($"SideHeroControl.RefreshView: HeroConfig 不存在 heroId={currentHeroId}");
            return;
        }

        if (textHeroName != null)
        {
            textHeroName.text = heroCfg.Name;
            textHeroName.color = SysColor.GetQualityColor(heroCfg.Quality);
        }

        // 英雄大图：与背包卡面共用 SkinsBig 资源
        if (heroBigImage != null)
        {
            Sprite bigSprite = Resources.Load<Sprite>("Textures/SkinsBig/" + heroCfg.Icon);
            if (bigSprite == null)
                GameLog.Warn($"SideHeroControl.RefreshView: 英雄大图不存在 Textures/SkinsBig/{heroCfg.Icon}");
            heroBigImage.sprite = bigSprite;
            heroBigImage.enabled = bigSprite != null;
        }

        var player = GameManager.Instance.GetPlayer(currentPid);
        var equipIds = player != null ? player.GetItemIdsOnHero(currentHeroId) : new List<int>();

        // 装备图标：最多 3 个，空槽隐藏
        if (equipImages != null)
        {
            for (int i = 0; i < equipImages.Length; i++)
            {
                if (equipImages[i] == null)
                    continue;

                bool has = i < equipIds.Count;
                equipImages[i].gameObject.SetActive(has);
                if (has)
                    equipImages[i].sprite = ItemIconLoader.Load(ItemConfig.GetConfig(equipIds[i]).Icon);
            }
        }

        // 该英雄没有装备时"脱下所有装备"不可用：可点击用金色，不可点击置灰
        bool hasEquip = equipIds.Count > 0;
        if (unwearAllButton != null)
        {
            unwearAllButton.interactable = hasEquip;
        }
    }

    // 脱下所有装备：先收起侧边栏，再由背包执行卸装并播放装备飞回物品区的动画
    void OnUnwearAll()
    {
        if (onUnwearAll == null)
        {
            GameLog.Warn("SideHeroControl.OnUnwearAll: 未设置卸装回调，直接关闭侧边栏");
            PanelManager.Instance.HideSideBar();
            return;
        }

        GameManager.Instance.PlaySound("Sounds/click");
        PanelManager.Instance.HideSideBar(() => onUnwearAll(currentHeroId));
    }

    // 卖出英雄：先收起侧边栏，再走背包的卖出流程（内含装备卸下与飞回动画）
    void OnSell()
    {
        if (onSell == null)
        {
            GameLog.Warn("SideHeroControl.OnSell: 未设置卖出回调，直接关闭侧边栏");
            PanelManager.Instance.HideSideBar();
            return;
        }

        GameManager.Instance.PlaySound("Sounds/click");
        PanelManager.Instance.HideSideBar(() => onSell(currentHeroId));
    }
}
