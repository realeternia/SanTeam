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
                var heroCfg = HeroConfig.GetConfig(heroId);
                cardView.cardId = heroId;
                cardView.cardImage.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);
                cardView.cardName.text = heroCfg.Name;
                if(heroCfg.Skills != null && heroCfg.Skills.Length > 0)
                    cardView.jobImage.sprite = Resources.Load<Sprite>("SkillPic/" + SkillConfig.GetConfig(heroCfg.Skills[0]).Icon);
                else
                    cardView.jobImage.gameObject.SetActive(false);
                cardView.lead.text = GetColoredText(heroCfg.LeadShip);
                cardView.inte.text = GetColoredText(heroCfg.Inte);
                cardView.str.text = GetColoredText(heroCfg.Str);

                cardView.hp.text = heroCfg.Hp.ToString();
                cardView.priceI = HeroSelectionTool.GetPrice(heroCfg);
                cardView.price.text = cardView.priceI.ToString();
                if(heroCfg.Side == 1)
                    cardView.gameObject.GetComponent<Image>().color = new Color(40/255f, 70/255f, 0/255f, 255/255f);
                else if(heroCfg.Side == 2)
                    cardView.gameObject.GetComponent<Image>().color = new Color(0/255f, 35/255f, 100/255f, 255/255f);
                else if(heroCfg.Side == 3)
                    cardView.gameObject.GetComponent<Image>().color = new Color(100/255f, 0/255f, 0/255f, 255/255f);
                else
                    cardView.gameObject.GetComponent<Image>().color = new Color(50/255f, 50/255f, 50/255f, 255/255f);

                cardViews.Add(cardView);
            }
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

    public void OnPlayerBuyCard(CardViewControl ctr, int pid, int cardId, int price)
    {
        if((round % 6) != 0)
            return;
        var player = GameManager.Instance.GetPlayer(pid);
        if (player.BuyCard(ctr, cardId, price))
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
        if (passedPlayers >= 4 || allCardsSold)
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

        for(int i = 0; i < 6; i++)
            GameManager.Instance.GetPlayer(i).AddGold(60);
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
