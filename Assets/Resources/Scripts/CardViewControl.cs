using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardViewControl : MonoBehaviour
{
    public int cardId;
    public bool isSold = false;
    public int priceI;

    public Image cardImage;
    public Image soldImage;
    public TMP_Text cardName;
    public TMP_Text price;
    public TMP_Text lead;
    public TMP_Text inte;
    public TMP_Text str;
    public TMP_Text hp;
    public Button buyButton;

    // Start is called before the first frame update
    void Start()
    {
        buyButton.onClick.AddListener(() =>
        {
            CardShopManager.Instance.OnPlayerBuyCard(this, 0, cardId, priceI);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSold()
    {
        isSold = true;
        buyButton.gameObject.SetActive(false);
        soldImage.gameObject.SetActive(true);
    }

}
