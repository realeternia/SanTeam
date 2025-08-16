using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonConfig;
using System.Reflection;
using UnityEngine.UI;

public class SelectCardNodeControl : MonoBehaviour
{
    public TMP_Text cardName;
    public Image expBar;
    public Image jobImg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateExp(string name, int exp, string icon)
    {
        expBar.rectTransform.sizeDelta = new Vector2(194 * HeroSelectionTool.GetExpRate(exp), 70);
        cardName.text = HeroSelectionTool.GetCardLevel(exp) + name;
        if (string.IsNullOrEmpty(icon))
        {
            jobImg.gameObject.SetActive(false);
        }
        else
        {
            jobImg.gameObject.SetActive(true);
            jobImg.sprite = Resources.Load<Sprite>("SkillPic/" + icon);

        }
    }
}
