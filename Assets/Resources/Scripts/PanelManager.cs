using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject cardShopPanel;
    public GameObject rankPanel;
    public GameObject pickPanel;


    public GameObject bagPanel;

    public int panelCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowShop()
    {
        cardShopPanel.SetActive(true);
      //  cardShopTxt.SetActive(true);

        ChangePanelCount(1);
    }

    public void HideShop()
    {
        cardShopPanel.SetActive(false);
     //   cardShopTxt.SetActive(false);

        ChangePanelCount(-1);
    }
    
    public void ShowBag()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        bagPanel.SetActive(true);
        bagPanel.GetComponent<BagControl>().OnShow();

        ChangePanelCount(1);
    }

    public void HideBag()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        bagPanel.SetActive(false);
        bagPanel.GetComponent<BagControl>().OnHide();

        ChangePanelCount(-1);
    }

    public void ShowRank()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPanel.SetActive(true);
        rankPanel.GetComponent<RankPanelManager>().OnShow();

        ChangePanelCount(1);        
    }

    public void HideRank()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPanel.SetActive(false);
        rankPanel.GetComponent<RankPanelManager>().OnHide();

        ChangePanelCount(-1);        
    }

    public void ShowPick()
    {
      //  GameManager.Instance.PlaySound("Sounds/deck");
        pickPanel.SetActive(true);

        ChangePanelCount(1);
    }

    public void HidePick()
    {
     //   GameManager.Instance.PlaySound("Sounds/deck");
        pickPanel.SetActive(false);

        ChangePanelCount(-1);
    }


    private void ChangePanelCount(int change)
    {
        panelCount += change;
        if(panelCount <= 0)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
