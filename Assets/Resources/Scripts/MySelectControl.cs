using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonConfig;

public class MySelectControl : MonoBehaviour
{
    public GameObject nodePrefab; // 拖拽CardView预制体到此处
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCards(PlayerInfo playerInfo)
    {
        // 假设 playerInfo 中有 cards 列表
        List<int> cards = new List<int>(playerInfo.cards.Keys);

        // 获取当前已有的 TMP_Text 组件
        List<SelectCardNodeControl> existingTexts = new List<SelectCardNodeControl>(GetComponentsInChildren<SelectCardNodeControl>());

        // 遍历 cards 列表
        for (int i = 0; i < cards.Count; i++)
        {
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
            var cardCfg = HeroConfig.GetConfig(cards[i]);
            var skillIcon = "";
            if(cardCfg.Skills != null && cardCfg.Skills.Length > 0)
            {
                skillIcon = SkillConfig.GetConfig(cardCfg.Skills[0]).Icon;
            }
            selectNode.UpdateExp(cardCfg.Name, playerInfo.cards[cards[i]], skillIcon);
        }

        // 移除多余的 TMP_Text
        for (int i = cards.Count; i < existingTexts.Count; i++)
        {
            Destroy(existingTexts[i].gameObject);
        }
    }
}
