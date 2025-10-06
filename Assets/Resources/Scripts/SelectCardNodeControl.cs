using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonConfig;
using System.Reflection;
using UnityEngine.UI;

public class SelectCardNodeControl : MonoBehaviour
{
    public int cardId;

    public TMP_Text cardName;
    public Image expBar;
    public Image jobImg;
    public Button checkBtn;
    public bool isChecked;
    // Start is called before the first frame update
    void Start()
    {
        checkBtn.onClick.AddListener(() =>
        {
            var player = GameManager.Instance.GetPlayer(0);

            // if (isChecked)
            // {
            //     checkBtn.gameObject.GetComponent<Image>().color = Color.gray;
            //     player.battleCards.Remove(cardId);
            // }
            // else
            // {
            //     if(player.battleCards.Count >= 5)
            //         return;

            //     checkBtn.gameObject.GetComponent<Image>().color = Color.red;
            //     player.battleCards.Add(cardId);

            // }
            isChecked = !isChecked;
        });
        checkBtn.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateExp(int pid, string name, int exp, string icon)
    {
        expBar.rectTransform.sizeDelta = new Vector2(194 * HeroSelectionTool.GetExpRate(exp, true), 70);
        cardName.text = HeroSelectionTool.GetCardLevel(exp, true) + name;
        if (string.IsNullOrEmpty(icon))
        {
            jobImg.gameObject.SetActive(false);
        }
        else
        {
            jobImg.gameObject.SetActive(true);
            jobImg.sprite = Resources.Load<Sprite>("SkillPic/" + icon);
        }
        if (pid == 0)
        {
            checkBtn.gameObject.SetActive(false);
            var player = GameManager.Instance.GetPlayer(0);
         //   isChecked = player.battleCards.Contains(cardId);
            if (isChecked)
            {
                checkBtn.gameObject.GetComponent<Image>().color = Color.red;
            }
            else
            {
                checkBtn.gameObject.GetComponent<Image>().color = Color.gray;
            }
        }
        else
        {
            checkBtn.gameObject.SetActive(false);
        }

    }
}
