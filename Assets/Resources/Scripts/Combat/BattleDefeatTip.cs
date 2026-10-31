using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// 击败提示：默认隐藏；支持排队播放多组击败信息
// 单组表现：显示2秒，从第1.5秒开始渐隐，2秒时完全消失
public class BattleDefeatTip : MonoBehaviour
{
    public Image winPlayerIcon;
    public Image losePlayerIcon;
    public ParticleSystem winParticle;

    private const float ShowDuration = 2f; // 单组总时长
    private const float FadeStartTime = 1.5f; // 开始渐隐的时间点
    private const float FadeDuration = ShowDuration - FadeStartTime; // 渐隐时长
    private const float PunchDuration = 0.3f; // 单次震动时长（短促有力）
    private const float PunchStrength = 26f; // 震动位移幅度（像素）
    private const int PunchVibrato = 5; // 震动次数
    private const float PunchElasticity = 0.4f; // 回弹弹性

    // 一组击败信息：胜者头像、败者头像
    private struct DefeatInfo
    {
        public Sprite winIcon;
        public Sprite loseIcon;
    }

    private readonly Queue<DefeatInfo> pendingTips = new Queue<DefeatInfo>();
    private CanvasGroup canvasGroup;
    private bool isPlaying = false;

    // 播放期间头像的振动tween，以及头像原始位置（用于振完归位）
    private Tween winShakeTween;
    private Tween loseShakeTween;
    private Vector2 winIconOrigin;
    private Vector2 loseIconOrigin;

    void Awake()
    {
        // 渐隐用 CanvasGroup（场景中未挂时运行时添加）
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 默认隐藏（对象保持激活，避免协程随对象停用而中断）
        Hide();
    }

    /// <summary>
    /// 增加一组击败信息：当前空闲则立即播放2秒，正在播放则进队列等待。
    /// </summary>
    public void AddDefeatTip(Sprite winIcon, Sprite loseIcon)
    {
        pendingTips.Enqueue(new DefeatInfo { winIcon = winIcon, loseIcon = loseIcon });
        if (!isPlaying)
            StartCoroutine(PlayQueue());
    }

    // 依次播放队列中的击败信息，全部播完后回到隐藏状态
    private IEnumerator PlayQueue()
    {
        isPlaying = true;
        while (pendingTips.Count > 0)
        {
            DefeatInfo info = pendingTips.Dequeue();
            yield return PlayOnce(info);
        }
        isPlaying = false;
    }

    // 播放单组：显示2秒，1.5秒开始渐隐，2秒时隐藏
    private IEnumerator PlayOnce(DefeatInfo info)
    {
        if (winPlayerIcon != null)
            winPlayerIcon.sprite = info.winIcon;
        if (losePlayerIcon != null)
            losePlayerIcon.sprite = info.loseIcon;

        Show();

        yield return new WaitForSeconds(FadeStartTime);

        // 1.5秒后开始渐隐
        if (winParticle != null)
            winParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        float time = 0f;
        while (time < FadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(time / FadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        Hide();
    }

    // 显示：恢复透明度、开启粒子并播放，同时让两个头像振动
    private void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        if (winParticle != null)
        {
            winParticle.gameObject.SetActive(true);
            winParticle.Clear();
            winParticle.Play();
        }

        StartIconShake();
    }

    // 显示瞬间对两个头像做一次有力的震动（上下单向冲击，不来回晃动）
    private void StartIconShake()
    {
        StopIconShake();

        if (winPlayerIcon != null)
        {
            winIconOrigin = winPlayerIcon.rectTransform.anchoredPosition;
            winShakeTween = winPlayerIcon.rectTransform
                .DOPunchAnchorPos(new Vector2(0, PunchStrength), PunchDuration, PunchVibrato, PunchElasticity, false);
        }
        if (losePlayerIcon != null)
        {
            loseIconOrigin = losePlayerIcon.rectTransform.anchoredPosition;
            loseShakeTween = losePlayerIcon.rectTransform
                .DOPunchAnchorPos(new Vector2(0, PunchStrength), PunchDuration, PunchVibrato, PunchElasticity, false);
        }
    }

    // 停止振动并把头像位置还原
    private void StopIconShake()
    {
        if (winShakeTween != null)
        {
            winShakeTween.Kill();
            winShakeTween = null;
            if (winPlayerIcon != null)
                winPlayerIcon.rectTransform.anchoredPosition = winIconOrigin;
        }
        if (loseShakeTween != null)
        {
            loseShakeTween.Kill();
            loseShakeTween = null;
            if (losePlayerIcon != null)
                losePlayerIcon.rectTransform.anchoredPosition = loseIconOrigin;
        }
    }

    // 隐藏：透明度归零并停掉粒子
    private void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        StopIconShake();

        if (winParticle != null)
            winParticle.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        StopIconShake();
    }
}
