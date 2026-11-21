using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class RankPlayerCellInfo : MonoBehaviour
{
    public Image playerImg;
    public TMP_Text playerName;
    public TMP_Text goldText;
    public TMP_Text powerText;
    public TMP_Text lifeText;
    public TMP_Text winRateText;

    public int playerId;
    public int gold;
    public int power;
    public int life;
    public int winRatePercent;

    // 最近 PvP 战绩图标布局：从名字右侧开始，逐个向右排列
    private const float ResultStartX = 770f;
    private const float ResultStepX = 28f;
    private const float ResultIconSize = 24f;
    private const float ResultY = -39f;

    private static Sprite winIcon;
    private static Sprite loseIcon;
    private Image[] resultIcons;

    // Start is called before the first frame update
    void Start()
    {
        playerName.raycastTarget = false;
        goldText.raycastTarget = false;
        powerText.raycastTarget = false;
        lifeText.raycastTarget = false;
        winRateText.raycastTarget = false;
    }

    public void Init(PlayerInfo playerInfo)
    {
        // 设置英雄信息
        playerImg.sprite = Resources.Load<Sprite>(playerInfo.imgPath);

        playerName.text = playerInfo.playerConfig.Name;
        playerId = playerInfo.playerId;

        gold = playerInfo.gold;
        power = playerInfo.lastFightMark;
        life = playerInfo.mark;
        winRatePercent = playerInfo.GetPvpWinRatePercent();

        goldText.text = gold.ToString();
        powerText.text = power.ToString();
        lifeText.text = life.ToString();
        winRateText.text = winRatePercent + "%";

        RefreshRecentResults(playerInfo.recentBattleResults);
    }

    // 最近 PvP 战绩：1=胜（绿圈） 0=负（红叉） -1=无记录（隐藏）
    private void RefreshRecentResults(int[] results)
    {
        EnsureResultIcons();

        if (winIcon == null)
            winIcon = LoadSystemIcon("pvpwin");
        if (loseIcon == null)
            loseIcon = LoadSystemIcon("pvplose");

        for (int i = 0; i < resultIcons.Length; i++)
        {
            int result = (results != null && i < results.Length) ? results[i] : -1;
            Image icon = resultIcons[i];

            if (result == 1 && winIcon != null)
            {
                icon.sprite = winIcon;
                icon.gameObject.SetActive(true);
            }
            else if (result == 0 && loseIcon != null)
            {
                icon.sprite = loseIcon;
                icon.gameObject.SetActive(true);
            }
            else
            {
                icon.gameObject.SetActive(false);
            }
        }
    }

    // 单元格第一次绑定时创建 10 个战绩图标槽位（循环列表复用后不再重复创建）
    private void EnsureResultIcons()
    {
        if (resultIcons != null)
            return;

        resultIcons = new Image[GameConst.PvpRecentCount];
        for (int i = 0; i < resultIcons.Length; i++)
        {
            var go = new GameObject("PvpResult_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(transform, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(ResultStartX + i * ResultStepX, ResultY);
            rt.sizeDelta = new Vector2(ResultIconSize, ResultIconSize);

            var img = go.GetComponent<Image>();
            img.raycastTarget = false;
            img.preserveAspect = true;
            resultIcons[i] = img;
        }
    }

    // 通过 SystemAttrConfig 的 name 找到图标文件名，再统一走 IconLoader 加载
    private static Sprite LoadSystemIcon(string name)
    {
        SystemAttrConfig cfg = SystemAttrConfig.GetConfigByname(name);
        if (cfg == null || string.IsNullOrEmpty(cfg.Icon))
        {
            GameLog.Error("RankPlayerCellInfo LoadSystemIcon: SystemAttrConfig 缺少图标 name=" + name);
            return null;
        }
        return IconLoader.LoadSprite(cfg.Icon);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
