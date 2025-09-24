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
    public GameObject rankPlayerPanel;
    public GameObject pickPanel;


    public GameObject bagPanel;

    public List<GameObject> openPanelList;

    // Start is called before the first frame update
    void Start()
    {
       // ShowBag();
    }

    public void ShowShop()
    {
        cardShopPanel.SetActive(true);
      //  cardShopTxt.SetActive(true);

        ChangePanelCount(cardShopPanel, true);
    }

    public void HideShop()
    {
        cardShopPanel.SetActive(false);
     //   cardShopTxt.SetActive(false);

        ChangePanelCount(cardShopPanel, false);
    }
    
    public void ShowBag()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        bagPanel.SetActive(true);
        bagPanel.GetComponent<BagControl>().OnShow();

        ChangePanelCount(bagPanel, true);
    }

    public void HideBag()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        bagPanel.SetActive(false);
        bagPanel.GetComponent<BagControl>().OnHide();

        ChangePanelCount(bagPanel, false);
    }

    public void ShowRank()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPanel.SetActive(true);
        rankPanel.GetComponent<RankPanelManager>().OnShow();

        ChangePanelCount(rankPanel, true);        
    }

    public void HideRank()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPanel.SetActive(false);
        rankPanel.GetComponent<RankPanelManager>().OnHide();

        ChangePanelCount(rankPanel, false);        
    }
    
    public void ShowRankPlayer()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPlayerPanel.SetActive(true);
        rankPlayerPanel.GetComponent<RankPlayerPanelManager>().OnShow();

        ChangePanelCount(rankPlayerPanel, true);        
    }

    public void HideRankPlayer()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPlayerPanel.SetActive(false);
        rankPlayerPanel.GetComponent<RankPlayerPanelManager>().OnHide();

        ChangePanelCount(rankPlayerPanel, false);        
    }

    public void ShowPick()
    {
      //  GameManager.Instance.PlaySound("Sounds/deck");
        pickPanel.SetActive(true);

        ChangePanelCount(pickPanel, true);
    }

    public void HidePick()
    {
     //   GameManager.Instance.PlaySound("Sounds/deck");
        pickPanel.SetActive(false);

        ChangePanelCount(pickPanel, false);
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        UnityEngine.Debug.Log($"PanelManager SendSignal {name} {parm1} {parm2}");
        foreach (var panel in openPanelList)
        {
            UnityEngine.Debug.Log($"PanelManager SendSignal {panel.name} {name} {parm1} {parm2}");
            if (panel.TryGetComponent<IPanelEvent>(out IPanelEvent p))
                p.SendSignal(name, parm1, parm2);
        }
    }

    private void ChangePanelCount(GameObject panel, bool isShow)
    {
        if(isShow)
            openPanelList.Add(panel);
        else
            openPanelList.Remove(panel);
        if(openPanelList.Count <= 0)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
