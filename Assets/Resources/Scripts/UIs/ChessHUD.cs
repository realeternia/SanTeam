using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;

public class ChessHUD : MonoBehaviour
{
    public Chess chessUnit;
    public Image healthImg;
    public Image manaImg;
    public Image healthBarImg;
    public Image qualityBgImg;
    public Image heroLevelImg;

    /// <summary>血条刻度：每格固定代表 400 点生命值（视觉标准）</summary>
    private const float HpPerCell = 400f;
    /// <summary>血条最多显示 MaxCellCount 格；血量超过 HpPerCell×MaxCellCount 后，每格代表的生命值随之增大</summary>
    private const int MaxCellCount = 15;
    /// <summary>血条满血时的像素宽度（英雄 70 / 小兵 50），与血量填充一致</summary>
    private const float HeroBarWidth = 80f;
    private const float SoldierBarWidth = 50f;

    private int lastHp;
    private float manaTimer; // 法力条刷新计时，满1s更新一次（与技能mp充能/回复节奏一致）
    private RectTransform tickRoot; // 刻度线容器
    private readonly List<RectTransform> tickLines = new List<RectTransform>();
    private float lastTickMaxHp = -1f; // 上次绘制刻度时的最大生命值
    private RectTransform manaTickRoot; // 法力条分层刻度线容器
    private readonly List<RectTransform> manaTickLines = new List<RectTransform>();

    void Start()
    {
        if (chessUnit != null)
        {
            // 所属玩家/阵营在整场战斗中不变，颜色只需设置一次
            if (healthImg != null)
                healthImg.color = GetHealthBarColor();
            UpdateHeroBadge();
            UpdateHealthDisplay();
            UpdateManaDisplay();
        }
    }

    void Update()
    {
        if (chessUnit == null)
        {
            Destroy(gameObject);
            return;
        }

        // 更新血条位置，使其跟随单位
        Vector3 worldPosition = new Vector3(chessUnit.transform.position.x + 5, chessUnit.transform.position.y + 3f, chessUnit.transform.position.z + 5);
        // 将屏幕坐标转换为UI相机的Canvas局部坐标
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPosition = WorldManager.Instance.TransformWorldToScreen(worldPosition, parentCanvas);

        rectTransform.anchoredPosition = screenPosition + new Vector2(chessUnit.isHero ? -60 : -55, 0);

        // 更新血条显示
        UpdateHealthDisplay();

        // 法力条每秒更新（与技能mp充能/回复节奏一致）
        manaTimer += Time.deltaTime;
        if (manaTimer >= 1f)
        {
            manaTimer -= 1f;
            UpdateManaDisplay();
        }
    }

    public void UpdateHealthDisplay()
    {
        if (chessUnit == null)
        {
            GameLog.Error("ChessUnit is null in UpdateHealthDisplay");
            return;
        }

        // 刻度只与最大生命值有关，单独判断刷新（升星/装备导致 maxHp 变化时重绘）
        RefreshHealthTicks();

        //chessUnit.hp如果变化不大，就return，降低开销
        if (Mathf.Abs(chessUnit.hp - lastHp) < 0.1f)
            return;
        lastHp = chessUnit.hp;

        // 血条常驻显示（满血也显示）
        if (healthImg != null)
        {
            var wid = chessUnit.isHero ? HeroBarWidth : SoldierBarWidth;
            healthImg.rectTransform.sizeDelta = new Vector2(chessUnit.hp * wid / chessUnit.maxHp, healthImg.rectTransform.sizeDelta.y);
        }
    }

    /// <summary>
    /// 血条颜色：自己军队（人类玩家 pid=0）绿色，敌方橙色，同阵营友军浅蓝色；
    /// 1v1配对地图（MapRule=0）上与自己不相干的其他玩家按号位奇偶分浅粉/浅黄两色。
    /// </summary>
    private Color GetHealthBarColor()
    {
        if (chessUnit.playerId == 0)
            return SysColor.Battle.HealthOwnColor;

        var world = WorldManager.Instance;
        var gm = GameManager.Instance;
        if (world == null || gm == null || gm.players == null || gm.players.Length == 0)
        {
            GameLog.Warn("WorldManager/GameManager 未就绪，无法判断敌我关系，血条按友军色显示");
            return SysColor.Battle.HealthAllyColor;
        }

        // 人类玩家固定占1号位，以其 battleSide 为基准判断是否敌对阵营
        int mySide = gm.players[0].battleSide;
        bool isEnemy = world.IsEnemy(chessUnit.side, mySide);

        // 1v1配对地图：除自己与配对的对手外，其他玩家互不作为敌人，按号位奇偶分两色区分
        if (world.MapRule == 0 && !isEnemy)
            return chessUnit.side % 2 == 0 ? SysColor.Battle.HealthSideEvenColor : SysColor.Battle.HealthSideOddColor;

        return isEnemy ? SysColor.Battle.HealthEnemyColor : SysColor.Battle.HealthAllyColor;
    }

    /// <summary>
    /// 英雄品质底板与等级图标：qualityBgImg 染英雄品质色，heroLevelImg 显示 Textures/Fight/lv{等级}.png（1~5）
    /// </summary>
    private void UpdateHeroBadge()
    {
        // 只有武将棋子有品质/等级表现（小兵/怪物用 HudSmall，无对应图标）
        if (!chessUnit.isHero)
            return;

        if (qualityBgImg != null)
        {
            var heroCfg = HeroConfig.GetConfig(chessUnit.heroId);
            if (heroCfg != null)
                qualityBgImg.color = SysColor.GetQualityColor(HeroScenario.GetQuality(heroCfg));
            else
                GameLog.Error("英雄配置不存在，无法设置品质底色 heroId=" + chessUnit.heroId);
        }

        if (heroLevelImg != null)
        {
            int lv = Mathf.Clamp(chessUnit.level, 1, HeroSelectionTool.MaxHeroCardLevel);
            var levelSprite = Resources.Load<Sprite>("Textures/Fight/lv" + lv);
            if (levelSprite == null)
            {
                GameLog.Error("等级图标加载失败：Textures/Fight/lv" + lv);
                return;
            }
            heroLevelImg.sprite = levelSprite;
        }
    }

    /// <summary>
    /// 绘制血条刻度：整条血条按每格 400 点生命值分格，格与格之间画黑色竖线；
    /// 最多 MaxCellCount 格，血量更高的棋子每格代表的生命值随之增大（maxHp / MaxCellCount）。
    /// </summary>
    private void RefreshHealthTicks()
    {
        if (chessUnit == null)
            return;
        if (Mathf.Approximately(lastTickMaxHp, chessUnit.maxHp))
            return;
        lastTickMaxHp = chessUnit.maxHp;

        if (tickRoot == null)
        {
            // Hud.prefab 血条填充与 healthBarImg 同父；HudSmall.prefab 只有 healthImg，
            // 两者都以填充的左边缘为刻度起点，故统一用填充的父节点作为刻度容器
            Transform barParent = healthImg != null ? healthImg.transform.parent : null;
            if (barParent == null && healthBarImg != null)
                barParent = healthBarImg.transform.parent;
            RectTransform parentRect = barParent as RectTransform;
            if (parentRect == null)
            {
                GameLog.Error("血条节点不是 RectTransform，无法绘制血条刻度");
                return;
            }

            var rootGo = new GameObject("HpTicks", typeof(RectTransform));
            tickRoot = rootGo.GetComponent<RectTransform>();
            tickRoot.SetParent(parentRect, false);
            tickRoot.anchorMin = new Vector2(0f, 0f);
            tickRoot.anchorMax = new Vector2(0f, 1f);
            tickRoot.pivot = new Vector2(0f, 0.5f);
            tickRoot.sizeDelta = Vector2.zero;
            tickRoot.anchoredPosition = Vector2.zero;
            tickRoot.SetAsLastSibling(); // 压在血量填充之上
        }

        var wid = chessUnit.isHero ? HeroBarWidth : SoldierBarWidth;
        // 常规每格 HpPerCell 点；超出上限格数时抬高每格代表值，保证网格数不超过 MaxCellCount
        float hpPerCell = Mathf.Max(HpPerCell, chessUnit.maxHp / (float)MaxCellCount);
        int cellCount = chessUnit.maxHp > 0f ? Mathf.Min(MaxCellCount, Mathf.CeilToInt(chessUnit.maxHp / hpPerCell)) : 0;
        int lineCount = Mathf.Max(0, cellCount - 1); // 格子之间的分隔线
        float cellWidth = cellCount > 0 ? wid / cellCount : 0f;

        while (tickLines.Count < lineCount)
        {
            var lineGo = new GameObject("Tick", typeof(RectTransform), typeof(Image));
            var lineRect = lineGo.GetComponent<RectTransform>();
            lineRect.SetParent(tickRoot, false);
            lineRect.anchorMin = new Vector2(0f, 0f);
            lineRect.anchorMax = new Vector2(0f, 1f);
            lineRect.pivot = new Vector2(0.5f, 0.5f);
            lineRect.sizeDelta = new Vector2(1f, 0f);

            var lineImage = lineGo.GetComponent<Image>();
            lineImage.color = SysColor.Battle.HealthTickColor;
            lineImage.raycastTarget = false;
            tickLines.Add(lineRect);
        }

        for (int i = 0; i < tickLines.Count; i++)
        {
            var lineRect = tickLines[i];
            bool visible = i < lineCount;
            if (lineRect.gameObject.activeSelf != visible)
                lineRect.gameObject.SetActive(visible);
            if (!visible)
                continue;
            lineRect.anchoredPosition = new Vector2(cellWidth * (i + 1), 0f);
        }
    }

    /// <summary>
    /// 更新法力条显示：manaImg非空且存在消耗MP的技能(MpCost>0)时，显示第一个此类技能的充能进度 Skill.mp/MpCost；无此类技能则隐藏
    /// </summary>
    private void UpdateManaDisplay()
    {
        if (manaImg == null || chessUnit == null)
            return;

        Skill mpSkill = null;
        foreach (var skill in chessUnit.skills)
        {
            if (skill.skillCfg.MpCost > 0)
            {
                mpSkill = skill;
                break;
            }
        }

        if (mpSkill == null)
        {
            if (manaImg.gameObject.activeSelf)
                manaImg.gameObject.SetActive(false);
            return;
        }

        if (!manaImg.gameObject.activeSelf)
            manaImg.gameObject.SetActive(true);
        var wid = chessUnit.isHero ? HeroBarWidth : SoldierBarWidth;
        // MpLimit>1 时整条法力条表示 MpLimit 层，满层填满；否则仍是单层进度条
        var layers = Mathf.Max(1, mpSkill.skillCfg.MpLimit);
        RefreshManaTicks(layers, wid);
        var cap = mpSkill.MpCap;
        var ratio = cap > 0f ? Mathf.Clamp01(mpSkill.mp / cap) : 0f;
        manaImg.rectTransform.sizeDelta = new Vector2(ratio * wid, manaImg.rectTransform.sizeDelta.y);
    }

    /// <summary>
    /// 法力条分层刻度：MpLimit>1 时把整条法力条按层数平分，层与层之间画竖线（与血条刻度一致）；
    /// MpLimit<=1 时不画线，保持原有单层进度条。
    /// </summary>
    private void RefreshManaTicks(int layers, float barWidth)
    {
        if (manaImg == null)
            return;

        int lineCount = Mathf.Max(0, layers - 1);
        if (lineCount == 0)
        {
            foreach (var line in manaTickLines)
            {
                if (line.gameObject.activeSelf)
                    line.gameObject.SetActive(false);
            }
            return;
        }

        if (manaTickRoot == null)
        {
            // 与血条刻度一致：以法力填充的左边缘为起点，刻度容器挂在填充的父节点上
            var parentRect = manaImg.transform.parent as RectTransform;
            if (parentRect == null)
            {
                GameLog.Error("法力条节点不是 RectTransform，无法绘制法力刻度");
                return;
            }

            var rootGo = new GameObject("MpTicks", typeof(RectTransform));
            manaTickRoot = rootGo.GetComponent<RectTransform>();
            manaTickRoot.SetParent(parentRect, false);
            manaTickRoot.anchorMin = new Vector2(0f, 0f);
            manaTickRoot.anchorMax = new Vector2(0f, 1f);
            manaTickRoot.pivot = new Vector2(0f, 0.5f);
            manaTickRoot.sizeDelta = Vector2.zero;
            manaTickRoot.anchoredPosition = Vector2.zero;
            manaTickRoot.SetAsLastSibling(); // 压在法力填充之上
        }

        float cellWidth = barWidth / layers;
        while (manaTickLines.Count < lineCount)
        {
            var lineGo = new GameObject("ManaTick", typeof(RectTransform), typeof(Image));
            var lineRect = lineGo.GetComponent<RectTransform>();
            lineRect.SetParent(manaTickRoot, false);
            lineRect.anchorMin = new Vector2(0f, 0f);
            lineRect.anchorMax = new Vector2(0f, 1f);
            lineRect.pivot = new Vector2(0.5f, 0.5f);
            lineRect.sizeDelta = new Vector2(1f, 0f);

            var lineImage = lineGo.GetComponent<Image>();
            lineImage.color = SysColor.Battle.ManaTickColor;
            lineImage.raycastTarget = false;
            manaTickLines.Add(lineRect);
        }

        for (int i = 0; i < manaTickLines.Count; i++)
        {
            var lineRect = manaTickLines[i];
            bool visible = i < lineCount;
            if (lineRect.gameObject.activeSelf != visible)
                lineRect.gameObject.SetActive(visible);
            if (!visible)
                continue;
            lineRect.anchoredPosition = new Vector2(cellWidth * (i + 1), 0f);
        }
    }
}