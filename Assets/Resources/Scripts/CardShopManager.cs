using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class CardShopManager : MonoBehaviour
{
    public static CardShopManager Instance;
    public List<CardViewControl> cardViews = new List<CardViewControl>();

    public GameObject cardViewPrefab; // 拖拽CardView预制体到此处
    public GameObject cardItemViewPrefab; // 拖拽CardView预制体到此处
    public GameObject cardItemView;

    private int round = 10000;
    private bool[] playerPassed = new bool[8]; // 记录每个玩家是否pass过
    private int passedPlayers = 0; // 记录pass的玩家数量

    public Button passBtn;
    public Button bagBtn;
    public Button rankBtn;
    public Button rankPlayerBtn;

    private int era = 0;
    public TMP_Text eraText;
    public MySelectControl mySelect;
    private bool isShopEnd = false;

    public int jadePlayer = -1; //购买和氏璧买家
    public int firstJumper = -1;
    private bool hasEnterBattle = false;

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
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });
        rankBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRank();
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });
        rankPlayerBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRankPlayer();
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });

        ShopBegin();
    }

    public void OnShow()
    {
        for(int i = 0; i < cardViews.Count; i++)
            cardViews[i].ShowEffectLayer(true);
    }

    private IEnumerator DelayedUpdate()
    { 
        yield return new WaitForSeconds(.7f);
        isShopEnd = false;
        while (!isShopEnd) // 模拟 Update 的循环
        {    
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 0.5f));

            int currentPlayerId = (round % 8);
                
            // 如果当前玩家已经pass，则直接进入下一回合
            if (playerPassed[currentPlayerId])
            {
                NextTurn();
                continue;
            }
            
            var playerInfo = GameManager.Instance.GetPlayer(currentPlayerId);
            if (playerInfo.isAI)
            {
                var result = PlayerAI.AiCheckBuyCard(playerInfo, era);
                
                if (!result)
                {
                    if(System.Linq.Enumerable.All(playerPassed, x => !x))
                        firstJumper = currentPlayerId;
                    // AI玩家放弃购买
                    playerPassed[currentPlayerId] = true;
                    passedPlayers++;
                    playerInfo.SetRoundOver(true);
                }
            }

            // 等待 1 秒（不阻塞主线程）
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 0.8f));

            if (playerInfo.isAI)
            {
                AfterAct();
            }
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

        var year = GameManager.Instance.year; //第几场比赛
        var shopCfg = ShopConfig.GetConfig(Math.Min(100, year + 1));
        List<Tuple<int, int>> heroIds = new List<Tuple<int, int>>();
        int TOTAL_HERO_CARDS = 21;        
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
            if (shopCfg.MultiPriceTotal > 2 * heroPrice)
            {
                var roll = UnityEngine.Random.Range(0, 100);
                if (roll < shopCfg.MultiCardRate)
                {
                    count = UnityEngine.Random.Range(1, shopCfg.MultiPriceTotal / heroPrice + 1);
                    if(roll >= 95 && shopCfg.ItemAmazingCount > count)
                        count = shopCfg.ItemAmazingCount;
                }

                if (count == 1)
                {
                    count = Math.Max(1, shopCfg.MultiPriceTotal / 3 / heroPrice);
                }
            }

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


        int CARDS_PER_ROW = 7;
        float cardWidth = 176f;
        float cardHeight = 245f;
        float spacing = 5f;

        // 计算起始位置，使其居中显示
        float startX = -((CARDS_PER_ROW * cardWidth) + (CARDS_PER_ROW - 1) * spacing) / 2f + cardWidth / 2f;
        float startY = 250f;        

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

            cardView.Init(heroId, true, heroCount, year);
            cardViews.Add(cardView);            
        }

        // 先把ItemConfig里所有RateAbs非0的item随出来，放到一个列表
        var itemIds = new List<int>();
        foreach (var itemCfg in ItemConfig.ConfigList)
        {
            if (itemCfg.RateAbs > 0 && itemCfg.ShopIdx <= shopCfg.Id && UnityEngine.Random.Range(0, 100) < itemCfg.RateAbs)
                itemIds.Add(itemCfg.Id);
        }

        for (int i = 0; i < shopCfg.ItemCount; i++)
        {
            itemIds.Add(HeroSelectionTool.GetRandomItemId(shopCfg.Id));
        }

        if (itemIds.Count < 9)
        {
            if (itemIds.Count + unsoldItems.Count > 9)
                unsoldItems.RemoveRange(0, itemIds.Count + unsoldItems.Count - 9);
            if (unsoldItems.Count > 0)
                itemIds.InsertRange(0, unsoldItems);
        }
        else if (itemIds.Count > 9)
        {
            itemIds.RemoveRange(9, itemIds.Count - 9);
        }

        int ids = 0;
        // item card
        foreach (var itemId in itemIds)
        {
            // 计算位置
            float x = -560 + ids * (140 + 5);
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

            if (!itemCfg.SellOne && shopCfg.MultiPriceTotal > 2 * cardPrice)
            {
                var roll = UnityEngine.Random.Range(0, 100);
                if (roll < shopCfg.MultiCardRate)
                {
                    count = UnityEngine.Random.Range(1, shopCfg.MultiPriceTotal / cardPrice + 1);
                    if(roll >= 95 && shopCfg.ItemAmazingCount > count)
                        count = shopCfg.ItemAmazingCount;
                }

                if (count == 1)
                {
                    count = Math.Max(1, shopCfg.MultiPriceTotal / 3 / cardPrice);
                }
            }            
            CardViewControl cardView = card.GetComponent<CardViewControl>();

            cardView.Init(itemId, false, count, year);
            cardViews.Add(cardView);
        }

        era++;
        passBtn.gameObject.SetActive(true);

        var nowYear = GameManager.Instance.year + 179;
        eraText.text = nowYear + "年\n" + era + "月";

        // 重置所有玩家的pass状态
        for (int i = 0; i < playerPassed.Length; i++)
        {
            playerPassed[i] = false;
            GameManager.Instance.GetPlayer(i).SetRoundOver(false);
        }
        passedPlayers = 0;

        if (jadePlayer >= 0)
        {
            round = 8 * 100 + jadePlayer;
        }
        else
        {
            if (firstJumper >= 0)
            {
                round = 8 * 100 + firstJumper;
            }
            else
            {
                round = 1000;
            }
        }
        jadePlayer = -1;
        firstJumper = -1;

        var pid = round % GameManager.Instance.players.Length;
        GameManager.Instance.OnPlayerTurn(pid);
        mySelect.UpdateCards(GameManager.Instance.GetPlayer(pid));

        CheckEraBonusGold();
        GameManager.Instance.PlaySound("Sounds/page");
    }

    private void CheckEraBonusGold()
    {
        if(GameManager.Instance.year <= 2)
            return;

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
    }

    public bool OnPlayerBuyCard(CardViewControl ctr, PlayerInfo player, int cardId, bool isHero, int price, int count)
    {
        if (player.BuyCard(ctr, cardId, isHero, price, count))
        {
            mySelect.UpdateCards(player);
            return true;
        }
        return false;
    }

    public void QuickView(int pid)
    {
        if (pid >= 0)
            mySelect.QuickView(GameManager.Instance.GetPlayer(pid));
        else
            mySelect.QuickViewFin();
    }

    public PlayerInfo GetCurrentPlayer()
    {
        return GameManager.Instance.GetPlayer(round % 8);
    }

    public void OnP1Pass()
    {
        var nowPlayer = GameManager.Instance.GetPlayer(round % 8);
        if(nowPlayer.isAI)
            return;
        if(playerPassed[nowPlayer.pid])
            return;

        passBtn.gameObject.SetActive(false);
        if(System.Linq.Enumerable.All(playerPassed, x => !x))
            firstJumper = nowPlayer.pid;
        playerPassed[nowPlayer.pid] = true;
        passedPlayers++;
        nowPlayer.SetRoundOver(true);

        AfterAct();
    }

    private void NextTurn()
    {
        UnityEngine.Debug.Log("NextTurn");
        for(int i = 0; i < 8; i++)
        {
            round++;
            if (!playerPassed[round % 8])
            {
                var nextPlayer = GameManager.Instance.GetPlayer(round % 8);
                passBtn.gameObject.SetActive(!nextPlayer.isAI);
                GameManager.Instance.OnPlayerTurn(round % 8);
                mySelect.UpdateCards(nextPlayer);
                return;
            }
        }
    }


    public void AfterAct()
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

        // 检查是否8个玩家都放弃或所有卡牌都已售出
        if (passedPlayers >= 8 || allCardsSold)
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
        if(hasEnterBattle) //存档拉起进入游戏，不会重复存储
            GameManager.Instance.SaveToFile();

        var roll = UnityEngine.Random.Range(0, 3);
        BGMPlayer.Instance.PlaySound(roll == 0 ? "BGMs/chun" : (roll == 1 ? "BGMs/xia" : "BGMs/qiu"));

        if (GameManager.Instance.year == 0)
        {
            UnityEngine.Debug.Log("FirstRound ck");
            for(int i = 0; i < 8; i++)
                GameManager.Instance.GetPlayer(i).FirstRound();
        }

        var shopOpenIndex = GameManager.Instance.year; //第几场比赛
        var shopCfg = ShopConfig.GetConfig(Math.Min(100, shopOpenIndex + 1));
        var roundGold = shopCfg.RoundGold;
        for(int i = 0; i < 8; i++)
            GameManager.Instance.GetPlayer(i).RoundGold(roundGold);

        GameManager.Instance.year++;
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
        hasEnterBattle = true;

        for(int i = 0; i < 8; i++)
            GameManager.Instance.GetPlayer(i).SetRoundOver(false);
    }
}
