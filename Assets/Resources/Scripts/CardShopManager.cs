using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardShopManager : MonoBehaviour
{
    public static CardShopManager Instance;
    public List<CardViewControl> cardViews = new List<CardViewControl>();

    public GameObject cardViewPrefab; // 拖拽CardView预制体到此处
    private const int TOTAL_CARDS = 12;
    private const int CARDS_PER_ROW = 6;
    private float cardWidth = 176f;
    private float cardHeight = 288f;
    private float spacing = 10f;
    private int round = 10002;
    private bool[] playerPassed = new bool[6]; // 记录每个玩家是否pass过
    private int passedPlayers = 0; // 记录pass的玩家数量

    public Button passBtn;
    public Button bagBtn;
    public Button rankBtn;

    private int era = 0;
    public TMP_Text eraText;
    public MySelectControl mySelect;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        passBtn.onClick.AddListener(() =>
        {
            OnP1Pass();
        });

        bagBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowBag();
        });
        rankBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRank();
        });

        ShopBegin();
    }

    private IEnumerator DelayedUpdate()
    { 
        yield return new WaitForSeconds(.7f);
        GameManager.Instance.OnPlayerTurn(0);
        while (true) // 模拟 Update 的循环
        {    
            yield return new WaitForSeconds(1.5f);
            // 你的逻辑代码
            doWork();

            // 等待 1 秒（不阻塞主线程）
            yield return new WaitForSeconds(.7f);
        }
    }      

    // Update is called once per frame
    void Update()
    {
        
    }

    private void NewEra()
    {
        var movingCardImages = GameObject.FindGameObjectsWithTag("MovingCard");
        foreach(var img in movingCardImages)
            Destroy(img);
        
        //移除并销毁旧卡片
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        cardViews.Clear();

        // 计算起始位置，使其居中显示
        float startX = -( (CARDS_PER_ROW * cardWidth) + (CARDS_PER_ROW - 1) * spacing ) / 2f + cardWidth / 2f;
        float startY = 145f;

        var shopOpenIndex = GameManager.Instance.GetPlayer(0).GamePlayed(); //第几场比赛

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

            var count = 1;
            if (shopOpenIndex >= 2) //第3局后有多张卡
            {
                if (UnityEngine.Random.Range(0, 100) < System.Math.Clamp((shopOpenIndex - 2) * 5, 6, 40))
                    count = 2;
            }            

            // 初始化CardView属性
            CardViewControl cardView = card.GetComponent<CardViewControl>();
          //  if(i == 0)
            if(shopOpenIndex >= 5 && i <= 1 && UnityEngine.Random.Range(0, 100) < System.Math.Clamp((shopOpenIndex - 5) * 3, 12, 35)) 
            {
                var itemId = HeroSelectionTool.GetRandomItemId();
                if (shopOpenIndex > 9 && UnityEngine.Random.Range(0, 500) > 200)
                    count += UnityEngine.Random.Range(0, shopOpenIndex / 5);
                cardView.Init(itemId, false, count);
            }
            else
            {
                var heroId = HeroSelectionTool.GetRandomHeroId();
                if (shopOpenIndex > 7 && UnityEngine.Random.Range(0, 500) > HeroConfig.GetConfig(heroId).Total)
                    count += UnityEngine.Random.Range(0, shopOpenIndex / 4);
                cardView.Init(heroId, true, count);
            }

            cardViews.Add(cardView);
        }

        era++;
        passBtn.gameObject.SetActive(true);    
        mySelect.UpdateCards(GameManager.Instance.GetPlayer(0));
        eraText.text = "第" + era + "轮";

        // 重置所有玩家的pass状态
        for (int i = 0; i < playerPassed.Length; i++)
            playerPassed[i] = false;
        passedPlayers = 0;        

        GameManager.Instance.PlaySound("Sounds/page");
    }

    public void OnPlayerBuyCard(CardViewControl ctr, int pid, int cardId, bool isHero, int price, int count)
    {
        if((round % 6) != 0)
            return;
        var player = GameManager.Instance.GetPlayer(pid);
        if (player.BuyCard(ctr, cardId, isHero, price, count))
        {
            mySelect.UpdateCards(player);

            AfterAct();
        }
    }

    public void OnPlayerSellCard()
    {
        mySelect.UpdateCards(GameManager.Instance.GetPlayer(0));
    }

    public void UpdateCards(int pid)
    {
        mySelect.UpdateCards(GameManager.Instance.GetPlayer(pid));
    }

    public void OnP1Pass()
    {
        if((round % 6) != 0) 
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
            nextPlayerId = round % 6;
        } while (playerPassed[nextPlayerId]);
        
        GameManager.Instance.OnPlayerTurn(nextPlayerId);
    }

    private void doWork()
    {
        int currentPlayerId = (round % 6);
               
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
        if (passedPlayers >= 6 || allCardsSold)
        {
            // 进入下一轮并刷新卡牌
            round++;
            if (era == 3)
            {
                StartCoroutine(ShopEnd());
                return;
            }
            NewEra();

        }
        NextTurn();
    }

    public void ShopBegin()
    {
        UnityEngine.Debug.Log("ShopBegin");

        var shopOpenIndex = GameManager.Instance.GetPlayer(0).GamePlayed(); //第几场比赛
        var roundGold = shopOpenIndex * 5 + 45;
        for(int i = 0; i < 6; i++)
            GameManager.Instance.GetPlayer(i).AddGold(roundGold);
        era = 0;
        NewEra();     
        StartCoroutine(DelayedUpdate()); 
    }

    private IEnumerator ShopEnd()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.ClearTurn();

        var movingCardImages = GameObject.FindGameObjectsWithTag("MovingCard");
        foreach(var img in movingCardImages)
            Destroy(img);

        PanelManager.Instance.HideShop();
        WorldManager.Instance.BattleBegin(); 
    }
}
