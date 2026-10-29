using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CommonConfig;

public class CastleHUD : MonoBehaviour
{
    public TMP_Text textAtk;
    public TMP_Text textHp;
    public Image healthImg;
    public Image playerIcon;
    public Image failIcon;

    // 血条最大宽度（与预制体背景条宽度一致）
    private const float MaxHealthWidth = 120f;

    // ---- 城堡战败表现 ----
    /// <summary>阵营战败时在城堡位置播放的火焰特效名（EffectConfig.Name）</summary>
    public const string CastleBurnEffect = "SoftFireBigRed";
    /// <summary>阵营战败火焰特效持续时长(秒)</summary>
    public const float CastleBurnEffectTime = 3f;
    /// <summary>阵营战败后延迟多久开始消融移除城堡(秒)</summary>
    public const float CastleBurnDelay = 0.5f;
    /// <summary>城堡材质消融时长(秒)</summary>
    public const float CastleDissolveTime = 1f;

    private int side;
    private Transform castle;
    private Transform fallbackCenter;
    private Renderer[] castleRenderers;
    private Vector3 anchorWorldPos;

    private float baseTotalHp;  // 开场该方全部单位血量上限总和（血条分母，固定值，避免单位阵亡后血条回涨）
    private float lastRate = -1f;
    private bool defeated;
    private bool initialized;

    private int baseAtk;
    private int baseHp;

    public void Init(PlayerInfo p, Transform castleTransform, Transform center, int battleSide)
    {
        side = battleSide;
        castle = castleTransform;
        fallbackCenter = center;
        if (castle != null)
            castleRenderers = castle.GetComponentsInChildren<Renderer>();

        // 玩家头像：优先用玩家面板已加载的头像，未加载时按配置路径取
        if (playerIcon != null)
        {
            Sprite icon = p.playerImage != null ? p.playerImage.sprite : null;
            if (icon == null)
                icon = Resources.Load<Sprite>(p.imgPath);
            playerIcon.sprite = icon;
        }

        var soldierCfg = SoldierConfig.GetConfig(500001);
        baseAtk = soldierCfg.Atk + p.GetSoldierAtkAdd(soldierCfg);
        baseHp = soldierCfg.Hp + p.GetItemPAttr("shp") + p.GetSoldierHpAdd(soldierCfg);
        textAtk.text = baseAtk.ToString();
        textHp.text = baseHp.ToString();

        // 失败图标初始隐藏，战败时再显示
        if (failIcon != null)
            failIcon.gameObject.SetActive(false);

        int curHp, maxHp;
        WorldManager.Instance.GetSideHpSum(side, out curHp, out maxHp);
        baseTotalHp = maxHp;

        anchorWorldPos = GetCastleAnchor();
        initialized = true;
        RefreshHealth(curHp, maxHp);
        UpdatePosition();
    }

    void LateUpdate()
    {
        if (!initialized)
            return;

        // HUD 跟随城堡位置（城堡已被移除时保持在最后位置）
        if (castle != null)
            anchorWorldPos = GetCastleAnchor();
        UpdatePosition();

        if (WorldManager.Instance != null)
        {
            int curHp, maxHp;
            WorldManager.Instance.GetSideHpSum(side, out curHp, out maxHp);
            RefreshHealth(curHp, maxHp);
        }
    }

    // 血条宽度 = 当前该方全部单位血量之和 / 开场总血量 × 120（最大 120）
    private void RefreshHealth(int curHp, int maxHp)
    {
        if (healthImg == null)
            return;

        float total = baseTotalHp > 0f ? baseTotalHp : maxHp;
        float rate = total > 0f ? Mathf.Clamp01(curHp / total) : 0f;
        if (Mathf.Approximately(rate, lastRate))
            return;

        lastRate = rate;
        healthImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MaxHealthWidth * rate);
    }

    // 城堡顶端稍上方作为 HUD 锚点（没有城堡时退回布阵中心）
    private Vector3 GetCastleAnchor()
    {
        if (castle == null)
            return fallbackCenter != null ? fallbackCenter.position + new Vector3(5f, 3f, 5f) : anchorWorldPos;

        Vector3 pos = castle.position;
        float topY = pos.y;
        if (castleRenderers != null)
        {
            foreach (var r in castleRenderers)
            {
                if (r != null)
                    topY = Mathf.Max(topY, r.bounds.max.y);
            }
        }
        return new Vector3(pos.x + 5f, topY + 1.5f, pos.z + 5f);
    }

    private void UpdatePosition()
    {
        RectTransform rectTransform = (RectTransform)transform;
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        if (parentCanvas == null || WorldManager.Instance == null)
            return;

        var screenPosition = WorldManager.Instance.TransformWorldToScreen(anchorWorldPos, parentCanvas);
        rectTransform.anchoredPosition = screenPosition + new Vector2(-MaxHealthWidth * 0.5f, 0f);
    }

    // 战败：显示失败图标，头像与攻/血量文字置灰
    public void SetDefeated()
    {
        if (defeated)
            return;
        defeated = true;

        if (failIcon != null)
            failIcon.gameObject.SetActive(true);

        var gray = SysColor.Theme.ActedHeroTextColor;
        if (playerIcon != null)
            playerIcon.color = gray;
        if (textAtk != null)
            textAtk.color = gray;
        if (textHp != null)
            textHp.color = gray;
    }
}
