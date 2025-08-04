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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateExp(string name, int exp)
    {
        expBar.rectTransform.sizeDelta = new Vector2(194 * HeroSelectionTool.GetExpRate(exp), 70);
        cardName.text = name + " Lv" + HeroSelectionTool.GetCardLevel(exp);
    }
}
