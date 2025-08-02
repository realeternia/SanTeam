using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardShopManager : MonoBehaviour
{
    public static CardShopManager Instance;
    public List<CardViewControl> cardViews = new List<CardViewControl>();

    public GameObject cardViewPrefab; // 拖拽CardView预制体到此处
    private const int TOTAL_CARDS = 10;
    private const int CARDS_PER_ROW = 5;
    private float cardWidth = 176f;
    private float cardHeight = 288f;
    private float spacing = 10f;
    private int round = 10000;
    private bool[] playerPassed = new bool[4]; // 记录每个玩家是否pass过
    private int passedPlayers = 0; // 记录pass的玩家数量

    public Button passBtn;
    public Button bagBtn;
    private int era = 0;
    public TMP_Text eraText;
    public MySelectControl mySelect;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        HeroConfig.Load();

        passBtn.onClick.AddListener(() =>
        {
            OnP1Pass();
        });

        bagBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowBag();
        });

        ShopBegin();
    }

    string GetColoredText(int value)
    {
        if (value >= 95)
        {
            return $"<color=red>{value}</color>";
        }
        else if (value >= 90)
        {
            return $"<color=yellow>{value}</color>";
        }
        return value.ToString();
    }


    private IEnumerator DelayedUpdate()
    { 
        yield return new WaitForSeconds(1f);
        GameManager.Instance.OnPlayerTurn(0);
        while (true) // 模拟 Update 的循环
        {    
            yield return new WaitForSeconds(2f);
            // 你的逻辑代码
            doWork();

            // 等待 1 秒（不阻塞主线程）
            yield return new WaitForSeconds(1f);
        }
    }      

    // Update is called once per frame
    void Update()
    {
        
    }

    private void NewEra()
    {
        //移除并销毁旧卡片
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        cardViews.Clear();

        // 计算起始位置，使其居中显示
        float startX = -( (CARDS_PER_ROW * cardWidth) + (CARDS_PER_ROW - 1) * spacing ) / 2f + cardWidth / 2f;
        float startY = 145f;

        for (int i = 0; i < TOTAL_CARDS; i++)
        {
            // 计算行和列
            int row = i / CARDS_PER_ROW;
            int col = i % CARDS_PER_ROW;

            // 计算位置
            float x = startX + col * (cardWidth + spacing);
            float y = startY - row * (cardHeight + spacing);

            // 创建CardView实例
            GameObject card = Instantiate(cardViewPrefab, transform);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(x, y);
                rectTransform.sizeDelta = new Vector2(cardWidth, cardHeight);
            }

            // 初始化CardView属性
            CardViewControl cardView = card.GetComponent<CardViewControl>();
            if (cardView != null)
            {
                var heroId = HeroSelectionTool.GetRandomHeroId();
                var heroCfg = HeroConfig.GetConfig((uint)heroId);
                cardView.cardId = heroId;
                cardView.cardImage.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);
                cardView.cardName.text = heroCfg.Name;
                cardView.lead.text = GetColoredText(heroCfg.LeadShip);
                cardView.inte.text = GetColoredText(heroCfg.Inte);
                cardView.str.text = GetColoredText(heroCfg.Str);

                cardView.hp.text = heroCfg.Hp.ToString();
                cardView.priceI = HeroSelectionTool.GetPrice(heroCfg);
                cardView.price.text = cardView.priceI.ToString();

                cardViews.Add(cardView);
            }
        }

        era++;
        passBtn.gameObject.SetActive(true);
        if(mySelect.playerInfo == null)
            mySelect.playerInfo = GameManager.Instance.GetPlayer(0);        
        mySelect.UpdateCards();
        eraText.text = "第" + era + "轮";

        GameManager.Instance.PlaySound("Sounds/page");
    }

    public void OnPlayerBuyCard(CardViewControl ctr, int pid, int cardId, int price)
    {
        if((round % 4) != 0)
            return;
        var player = GameManager.Instance.GetPlayer(pid);
        if (player.BuyCard(ctr, cardId, price))
        {
            if(mySelect.playerInfo == null)
                mySelect.playerInfo = player;
            mySelect.UpdateCards();

            AfterAct();
        }
    }

    public void OnPlayerSellCard()
    {
        mySelect.UpdateCards();
    }

    public void OnP1Pass()
    {
        if((round % 4) != 0)
            return;        
        if(playerPassed[0])
            return;

        passBtn.gameObject.SetActive(false);
        playerPassed[0] = true;
        passedPlayers++;

        AfterAct();
    }

    private void NextTurn()
    {
        // 找到下一个没有pass的玩家
        int nextPlayerId;
        do
        {
            round++;
            nextPlayerId = round % 4;
        } while (playerPassed[nextPlayerId]);
        
        GameManager.Instance.OnPlayerTurn(nextPlayerId);
    }

    private void doWork()
    {
        int currentPlayerId = (round % 4);
               
        // 如果当前玩家已经pass，则直接进入下一回合
        if (playerPassed[currentPlayerId])
        {
            NextTurn();
            return;
        }
        
        if (currentPlayerId != 0)
        {
            var result = GameManager.Instance.GetPlayer(currentPlayerId).AiCheckBuyCard(era);
            
            if (!result)
            {
                // AI玩家放弃购买
                playerPassed[currentPlayerId] = true;
                passedPlayers++;
            }

            AfterAct();
        }
    }    

    private void AfterAct()
    {
        // 检查是否所有卡牌都已售出
        bool allCardsSold = true;
        foreach (var card in cardViews)
        {
            if (!card.isSold)
            {
                allCardsSold = false;
                break;
            }
        }
                // 检查是否4个玩家都放弃或所有卡牌都已售出
        if (passedPlayers >= 4 || allCardsSold)
        {
            // 进入下一轮并刷新卡牌
            round++;
            if (era == 3)
            {
                ShopEnd();
                return;
            }
            NewEra();
            // 重置所有玩家的pass状态
            for (int i = 0; i < playerPassed.Length; i++)
            {
                playerPassed[i] = false;
            }
            passedPlayers = 0;
        }
        NextTurn();
    }

    public void ShopBegin()
    {
        for(int i = 0; i < 4; i++)
            GameManager.Instance.GetPlayer(i).AddGold(35);
        era = 0;
        PanelManager.Instance.ShowShop();
        NewEra();
        // 重置所有玩家的pass状态
        for (int i = 0; i < playerPassed.Length; i++)
        {
            playerPassed[i] = false;
        }
        passedPlayers = 0;        
        StartCoroutine(DelayedUpdate()); 
    }

    private void ShopEnd()
    {
        GameManager.Instance.ClearTurn();
        PanelManager.Instance.HideShop();
        WorldManager.Instance.BattleBegin(); 
    }
}
