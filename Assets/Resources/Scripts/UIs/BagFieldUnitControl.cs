using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CommonConfig;

public class BagFieldUnitControl : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int posId;
    public Image heroIcon;
    public Image jobIcon;
    public BagControl bagControl;
    public int myHeroId;

    public GameObject effectJob;
    public Image effectForce;

    // effectForce 呼吸参数：透明度在 隐藏端~显示 之间循环（呼吸效果）
    private const float ForceBreatheSpeed = 3f;
    private const float ForceHideAlpha = 0.3f;

    private GameObject dragInstance;
    private Coroutine breatheCoroutine;

    public void SetInfo(int id, int unitId)
    {
        myHeroId = unitId;
        posId = id;
        if (unitId == 0)
        {
            heroIcon.gameObject.SetActive(false);
            jobIcon.gameObject.SetActive(false);
        }
        else if (unitId == 500001 || unitId == 500002)
        {
            // 小兵格：近战显示bing1，远程显示bing2
            heroIcon.gameObject.SetActive(true);
            jobIcon.gameObject.SetActive(false);
            heroIcon.sprite = Resources.Load<Sprite>("Textures/" + (unitId == 500001 ? "bing1" : "bing2"));
        }
        else
        {
            heroIcon.gameObject.SetActive(true);
            jobIcon.gameObject.SetActive(true);
            var heroCfg = HeroConfig.GetConfig(unitId);
            heroIcon.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
            var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
            if (skillCfgs.Count > 0)
            {
                jobIcon.sprite = Resources.Load<Sprite>("SkillPic/" + skillCfgs[0].Icon);
            }
        }

        UpdateLinkEffects();
    }

    // 更新羁绊特效显示（参考 CardViewControl.UpdateEffects 直接 SetActive 控制）：
    // 职业与国家的特效默认不点亮，上阵卡片达到2张才开始亮：
    // 职业：同职业英雄数>=2 亮 effectJob；国家：同阵营英雄数>=2（即等级>=1）亮 effectForce 并播放呼吸效果
    private void UpdateLinkEffects()
    {
        if (effectJob == null || effectForce == null)
            return;

        var showJob = false;
        var showForce = false;
        if (bagControl != null && bagControl.bindPlayer != null
            && myHeroId > 0 && ConfigManager.IsHeroCard(myHeroId))
        {
            var heroCfg = HeroConfig.GetConfig(myHeroId);
            var jobCount = 0;
            var forceCount = 0;
            foreach (var id in bagControl.bindPlayer.battleCards)
            {
                if (id <= 0 || !ConfigManager.IsHeroCard(id))
                    continue;
                var cfg = HeroConfig.GetConfig(id);
                if (cfg.Job == heroCfg.Job)
                    jobCount++;
                if (cfg.Side == heroCfg.Side)
                    forceCount++;
            }

            // 职业：同职业英雄数>=2 才点亮（默认不亮）
            showJob = jobCount >= 2;
            // 国家：同阵营英雄数>=2 才点亮（默认不亮）；与 FactionShieldManager 一致，不参与同阵营护盾的势力（野=10）不显示
            var forceCfg = ConfigManager.GetForceConfig(heroCfg.Side);
            showForce = forceCfg != null && forceCfg.JoinFactionShield && forceCount - 1 >= 1;
        }

        effectJob.SetActive(showJob);

        if (showForce)
        {
            if (!effectForce.gameObject.activeSelf)
                effectForce.gameObject.SetActive(true);
            StartBreathing();
        }
        else
        {
            StopBreathing();
            effectForce.gameObject.SetActive(false);
        }
    }

    // 启动 effectForce 呼吸效果（显示↔隐藏循环）
    private void StartBreathing()
    {
        if (breatheCoroutine == null)
            breatheCoroutine = StartCoroutine(BreathLoop());
    }

    // 停止呼吸效果并复位透明度为完全显示
    private void StopBreathing()
    {
        if (breatheCoroutine != null)
        {
            StopCoroutine(breatheCoroutine);
            breatheCoroutine = null;
        }
        if (effectForce == null)
            return;
        var c = effectForce.color;
        effectForce.color = new Color(c.r, c.g, c.b, 1f);
    }

    // 呼吸循环：透明度在 隐藏端(0.3)~显示(1) 之间按 sin 平滑往返
    private IEnumerator BreathLoop()
    {
        while (true)
        {
            float sinValue = (Mathf.Sin(Time.time * ForceBreatheSpeed) + 1) * 0.5f;
            var alpha = Mathf.Lerp(ForceHideAlpha, 1f, sinValue);
            var c = effectForce.color;
            effectForce.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }

    // 当有物体拖放到此对象上时调用
    public void OnDrop(PointerEventData eventData)
    {       
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null)
            return;

        // 从背包拖英雄布阵
        BagCell draggedCell = draggedObject.GetComponent<BagCell>();
        if (draggedCell != null && ConfigManager.IsHeroCard(draggedCell.cardId))
        {
            draggedCell.RemoveTagImg();
            bagControl.SetHeroForBattle(draggedCell.cardId, posId);
            return;
        }

        // 从布阵格拖单位（英雄/小兵）到此格：交换/移动位置
        BagFieldUnitControl draggedField = draggedObject.GetComponent<BagFieldUnitControl>();
        if (draggedField != null)
        {
            draggedField.RemoveDragIcon();
            bagControl.SwapFieldUnit(draggedField.posId, posId);
        }
    }

    // 开始拖动时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (bagControl == null || bagControl.bindPlayer.isAI)
            return;
        if (myHeroId == 0)
            return;

        // 创建拖动时的预览对象
        dragInstance = new GameObject("FieldUnitDragIcon");
        dragInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
        dragInstance.transform.localScale = Vector3.one;

        Image dragImage = dragInstance.AddComponent<Image>();
        dragImage.sprite = GetUnitSprite(myHeroId);
        dragImage.rectTransform.sizeDelta = new Vector2(80, 80);
        dragImage.raycastTarget = false;
        dragInstance.transform.SetAsLastSibling();

        // 使当前对象半透明
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    // 拖动过程中调用
    public void OnDrag(PointerEventData eventData)
    {
        if (dragInstance == null)
            return;

        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GameObject.Find("Canvas").GetComponent<RectTransform>(),
            Input.mousePosition,
            eventData.pressEventCamera,
            out worldPosition);

        dragInstance.transform.position = worldPosition;
    }

    // 结束拖动时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        RemoveDragIcon();
    }

    public void OnDestroy()
    {
        RemoveDragIcon();
    }

    public void RemoveDragIcon()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        if (dragInstance != null)
        {
            Image dragImage = dragInstance.GetComponent<Image>();
            if (dragImage != null)
                dragImage.enabled = false;
            Destroy(dragInstance);
            dragInstance = null;
        }
    }

    private Sprite GetUnitSprite(int unitId)
    {
        if (unitId == 500001)
            return Resources.Load<Sprite>("Textures/bing1");
        if (unitId == 500002)
            return Resources.Load<Sprite>("Textures/bing2");
        var heroCfg = HeroConfig.GetConfig(unitId);
        return Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
    }
}
