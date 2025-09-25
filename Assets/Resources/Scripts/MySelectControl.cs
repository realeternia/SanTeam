using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonConfig;

public class MySelectControl : MonoBehaviour
{
    public GameObject nodePrefab; // 拖拽CardView预制体到此处
    private List<int> playerCards;
    private PlayerInfo playerInfo;
    
    public TMP_Text playerNameText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuickView(PlayerInfo checkPlayer)
    {
        var tmpCards = new List<int>(checkPlayer.cards.Keys);
        ShowCards(checkPlayer, tmpCards);
        playerNameText.text = checkPlayer.playerConfig.Name;
    }

    public void QuickViewFin()
    {
        ShowCards(playerInfo, playerCards);
        playerNameText.text = playerInfo.playerConfig.Name;
    }

    public void UpdateCards(PlayerInfo playerInfo)
    {
        // 假设 playerInfo 中有 cards 列表
        playerCards = new List<int>(playerInfo.cards.Keys);
        this.playerInfo = playerInfo;
        ShowCards(playerInfo, playerCards);
        playerNameText.text = playerInfo.playerConfig.Name;
        UnityEngine.Debug.Log($"UpdateCards {playerInfo.name} {playerCards.Count}");
    }

    private void ShowCards(PlayerInfo checkPlayer, List<int> cards)
    {
          // 获取当前已有的 TMP_Text 组件
        List<SelectCardNodeControl> existingTexts = new List<SelectCardNodeControl>(GetComponentsInChildren<SelectCardNodeControl>());

        // 遍历 cards 列表
        int i = 0;
        foreach (var cardId in cards)
        {
            if(!ConfigManager.IsHeroCard(cardId))
                continue;

            SelectCardNodeControl selectNode;
            if (i < existingTexts.Count)
            {
                // 增量更新，复用已有的 TMP_Text
                selectNode = existingTexts[i];
            }
            else
            {
                // 如果找不到对应的 TMP_Text，则创建 nodePrefab 实例
                GameObject textObject = Instantiate(nodePrefab, transform);
                textObject.name = $"CardText_{i}";
                selectNode = textObject.GetComponent<SelectCardNodeControl>();
                if (selectNode == null)
                {
                    Debug.LogError("Failed to get SelectCardNodeControl component from the instantiated prefab.");
                }

                // 设置基本布局，纵向排列
                RectTransform rectTransform = textObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    rectTransform.anchoredPosition = new Vector2(0, - i * 63); // 假设每个文本高度 30 单位
                    rectTransform.sizeDelta = new Vector2(0, 60);
                }
            }

            // 更新文本内容，这里假设 CardInfo 有一个 GetDisplayText 方法
            var cardCfg = HeroConfig.GetConfig(cardId);
            var skillIcon = "";
            if(cardCfg.Skills != null && cardCfg.Skills.Length > 0)
            {
                skillIcon = SkillConfig.GetConfig(cardCfg.Skills[0]).Icon;
            }
            selectNode.cardId = cardId;
            selectNode.UpdateExp(checkPlayer.pid, cardCfg.Name, checkPlayer.cards[cardId], skillIcon);

            i++;
        }

        // 移除多余的 TMP_Text
        for (int j = i; j < existingTexts.Count; j++)
        {
            Destroy(existingTexts[j].gameObject);
        }
    }
}
