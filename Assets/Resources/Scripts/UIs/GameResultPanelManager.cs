using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

// 游戏结算面板：显示当前名次（存活玩家数+1）与人类玩家全部上阵武将头像，点关闭直接退出游戏
public class GameResultPanelManager : MonoBehaviour
{
    public TMP_Text resultTxt;
    public GameObject heroIcons;
    public Button closeBtn;

    private const float HeroIconSize = 90f; // 武将头像尺寸
    private const float HeroIconSpacing = 100f; // 武将头像横向间距

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            // 游戏结束退出：删除本局存档
            if (GameManager.Instance.currentSaveSlot >= 0)
                GameManager.Instance.DeleteSave(GameManager.Instance.currentSaveSlot);
            // 关闭游戏
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
    }

    public void OnShow()
    {
        // 结算面板切到游戏开始 BGM
        BGMPlayer.Instance.PlaySound("BGMs/start");
        ShowRankText();
        ShowHeroIcons();
    }

    public void OnHide()
    {
    }

    // 名次 = 当前存活玩家数（不含自己）+ 1
    private void ShowRankText()
    {
        int aliveOthers = 0;
        foreach (var player in GameManager.Instance.players)
        {
            if (player != null && player.pid != 0 && !player.isEliminated)
                aliveOthers++;
        }
        resultTxt.text = $"第{aliveOthers + 1}名";
    }

    // 横向铺开人类玩家所有上阵武将的头像
    private void ShowHeroIcons()
    {
        foreach (Transform child in heroIcons.transform)
            Destroy(child.gameObject);

        var heros = GameManager.Instance.GetPlayer(0)
            .GetBattleCardList()
            .Where(x => x != null)
            .ToList();
        if (heros.Count == 0)
        {
            GameLog.Warn("游戏结算：人类玩家没有上阵武将，头像列表为空");
            return;
        }

        for (int i = 0; i < heros.Count; i++)
        {
            var heroCfg = HeroConfig.GetConfig(heros[i].Item1);
            if (heroCfg == null)
            {
                GameLog.Error($"游戏结算：武将配置不存在 id={heros[i].Item1}");
                continue;
            }
            var sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
            if (sprite == null)
                GameLog.Warn($"游戏结算：武将{heroCfg.Name}(id{heroCfg.Id})头像加载失败: Textures/Skins/{heroCfg.Icon}");

            var go = new GameObject(heroCfg.Name, typeof(RectTransform), typeof(Image));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(heroIcons.transform, false);
            rect.sizeDelta = new Vector2(HeroIconSize, HeroIconSize);
            rect.anchoredPosition = new Vector2((i - (heros.Count - 1) * 0.5f) * HeroIconSpacing, 0f);

            var image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }
    }
}
