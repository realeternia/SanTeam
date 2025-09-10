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
    public GameObject cardItemViewPrefab; // 拖拽CardView预制体到此处
    public GameObject cardItemView;
    private const int TOTAL_HERO_CARDS = 21;
    private const int CARDS_PER_ROW = 7;
    private float cardWidth = 176f;
    private float cardHeight = 245f;
    private float spacing = 5f;
    private int round = 10002;
    private bool[] playerPassed = new bool[6]; // 记录每个玩家是否pass过
    private int passedPlayers = 0; // 记录pass的玩家数量

    public Button passBtn;
    public Button bagBtn;
    public Button rankBtn;

    private int era = 0;
    public TMP_Text eraText;
    public MySelectControl mySelect;
    private bool isShopEnd = false;

    public int nextFirstPicker = -1;

    private Coroutine shopCoroutine;


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
        isShopEnd = false;
        while (!isShopEnd) // 模拟 Update 的循环
        {    
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.7f, 1.5f));

            // 你的逻辑代码
            doWork();

            // 等待 1 秒（不阻塞主线程）
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 0.6f));
        }
    }      

    // Update is called once per frame
    void Update()
    {
        
    }

    private void NewEra()
    {
        var movingCardImages = GameObject.FindGameObjectsWithTag("MovingCard");
        foreach (var img in movingCardImages)
            Destroy(img);

        //移除并销毁旧卡片
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        foreach (Transform child in cardItemView.transform)
            Destroy(child.gameObject);
        var unsoldItems = cardViews.FindAll(x => !x.isHeroCard && !x.isSold && !ItemConfig.GetConfig(x.cardId).AutoRemove).ConvertAll(a => a.cardId);
        cardViews.Clear();
        if(era == 0) //第一个回合不存装备
            unsoldItems.Clear();

        foreach(var player in GameManager.Instance.players)
            player.OnEra(era);

        // 计算起始位置，使其居中显示
        float startX = -((CARDS_PER_ROW * cardWidth) + (CARDS_PER_ROW - 1) * spacing) / 2f + cardWidth / 2f;
        float startY = 250f;

        var shopOpenIndex = GameManager.Instance.GetPlayer(0).GamePlayed(); //第几场比赛
        var shopCfg = ShopConfig.GetConfig(Math.Min(100, shopOpenIndex + 1));
        List<Tuple<int, int>> heroIds = new List<Tuple<int, int>>();
        // hero card
        for (int i = 0; i < TOTAL_HERO_CARDS; i++)
        {
            var heroId = HeroSelectionTool.GetRandomHeroId();
            var existingIndex = heroIds.FindIndex(x => x.Item1 == heroId);
            if (existingIndex >= 0)
            { //重复卡的处理
                if (shopCfg.Id > 3)
                {
                    var existingTuple = heroIds[existingIndex];
                    heroIds[existingIndex] = new Tuple<int, int>(existingTuple.Item1, existingTuple.Item2 + 1);
                }

                i--;
                continue;
            }

            var count = 1;
            var heroPrice = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(heroId));
            if (shopCfg.MultiPriceTotal > 2 * heroPrice && UnityEngine.Random.Range(0, 100) < shopCfg.MultiCardRate) //第3局后有多张卡
                count = UnityEngine.Random.Range(1, shopCfg.MultiPriceTotal / heroPrice + 1);

            heroIds.Add(new Tuple<int, int>(heroId, count));
        }
        heroIds.Sort((a, b) =>
        {
            // 先按卡单价排序
            int priceCompare = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(b.Item1)).CompareTo(HeroSelectionTool.GetPrice(HeroConfig.GetConfig(a.Item1)));
            if (priceCompare != 0)
                return priceCompare;

            // 单价相同，按id排序
            int idCompare = b.Item1.CompareTo(a.Item1);
            if (idCompare != 0)
                return idCompare;

            // id相同按item2排序
            return b.Item2.CompareTo(a.Item2);
        });

        for(int i = 0; i < heroIds.Count; i++)
        {
            var heroId = heroIds[i].Item1;
            var heroCount = heroIds[i].Item2;

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
                rectTransform.anchoredPosition = new Vector2(x, y);

            CardViewControl cardView = card.GetComponent<CardViewControl>();

            cardView.Init(heroId, true, heroCount, shopOpenIndex);
            cardViews.Add(cardView);            
        }

        var total = shopCfg.ItemCount;
        UnityEngine.Debug.Log("totalItem = " + total);
        // 先把ItemConfig里所有RateAbs非0的item随出来，放到一个列表
        var itemIds = new List<int>();
        foreach (var itemCfg in ItemConfig.ConfigList)
        {
            if (itemCfg.RateAbs > 0 && itemCfg.ShopIdx <= shopCfg.Id && UnityEngine.Random.Range(0, 100) < itemCfg.RateAbs)
                itemIds.Add(itemCfg.Id);
        }

        for (int i = itemIds.Count; i < total; i++)
        {
            itemIds.Add(HeroSelectionTool.GetRandomItemId(shopCfg.Id));
        }
        if(unsoldItems.Count > 0)
            itemIds.InsertRange(0, unsoldItems);

        int ids = 0;
        // item card
        foreach (var itemId in itemIds)
        {
            // 计算位置
            float x = -500 + ids * (140 + 5);
            ids++;
            float y = 0;

            // 创建CardView实例
            GameObject card = Instantiate(cardItemViewPrefab, cardItemView.transform);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            if (rectTransform != null)
                rectTransform.anchoredPosition = new Vector2(x, y);

            var count = 1;
            var itemCfg = ItemConfig.GetConfig(itemId);
            var cardPrice = itemCfg.Price;
            if (!itemCfg.SellOne && shopCfg.MultiPriceTotal > 2 * cardPrice && UnityEngine.Random.Range(0, 100) < shopCfg.MultiCardRate) //第3局后有多张卡
            {
                count = UnityEngine.Random.Range(1, shopCfg.MultiPriceTotal / cardPrice + 1);
            }
            CardViewControl cardView = card.GetComponent<CardViewControl>();

            cardView.Init(itemId, false, count, shopOpenIndex);
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

        if (nextFirstPicker >= 0)
        {
            round = 6 * 100 + nextFirstPicker;
            GameManager.Instance.OnPlayerTurn(nextFirstPicker);
        }
        nextFirstPicker = -1;

        // 获取所有玩家
        var players = new List<(int id, int gold)>();
        foreach (var player in GameManager.Instance.players)
            players.Add((player.pid, player.gold));

        // 按金币数量升序排序
        players.Sort((a, b) => a.gold.CompareTo(b.gold));

        if(players[0].gold == players[1].gold && players[1].gold == players[2].gold)
        {
            
        }
        else
        {
            if (players[0].gold < players[1].gold)
            {
                GameManager.Instance.GetPlayer(players[0].id).AddGold(5);
                if (players[1].gold < players[2].gold)
                {
                    GameManager.Instance.GetPlayer(players[1].id).AddGold(3);
                    GameManager.Instance.GetPlayer(players[2].id).AddGold(1);
                }
                else
                {
                    GameManager.Instance.GetPlayer(players[1].id).AddGold(2);
                    GameManager.Instance.GetPlayer(players[2].id).AddGold(2);
                }
            }
            else
            {
                GameManager.Instance.GetPlayer(players[0].id).AddGold(4);
                GameManager.Instance.GetPlayer(players[1].id).AddGold(4);
                GameManager.Instance.GetPlayer(players[2].id).AddGold(1);
            }
        }

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
        UnityEngine.Debug.Log("NextTurn");
        for(int i = 0; i < 6; i++)
        {
            round++;
            if (!playerPassed[round % 6])
            {
                GameManager.Instance.OnPlayerTurn(round % 6);
                return;
            }
        }
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
            var player = GameManager.Instance.GetPlayer(currentPlayerId);
            var result = PlayerAI.AiCheckBuyCard(player, era);
            
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

        NextTurn();

        // 检查是否6个玩家都放弃或所有卡牌都已售出
        if (passedPlayers >= 6 || allCardsSold)
        {
            if (era == 3)
            {
                StartCoroutine(ShopEnd());
                return;
            }
            NewEra();
        }
    }

    public void ShopBegin()
    {
        UnityEngine.Debug.Log("ShopBegin");

        var shopOpenIndex = GameManager.Instance.GetPlayer(0).GamePlayed(); //第几场比赛
        var roundGold = ShopConfig.GetConfig(shopOpenIndex + 1).RoundGold;
        for(int i = 0; i < 6; i++)
            GameManager.Instance.GetPlayer(i).AddGold(roundGold);
        era = 0;
        NewEra();     
        shopCoroutine = StartCoroutine(DelayedUpdate()); 
    }

    private IEnumerator ShopEnd()
    {
        isShopEnd = true;

        yield return new WaitForSeconds(0.5f);
        if(shopCoroutine != null)
            StopCoroutine(shopCoroutine);
        shopCoroutine = null;

        GameManager.Instance.ClearTurn();

        var movingCardImages = GameObject.FindGameObjectsWithTag("MovingCard");
        foreach(var img in movingCardImages)
            Destroy(img);

        Tooltip.Instance.HideTooltip();
        PanelManager.Instance.HideShop();
        WorldManager.Instance.BattleBegin(); 
    }
}
