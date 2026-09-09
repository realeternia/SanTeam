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
    public bool isChecked;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 羁绊模式显示：隐藏经验条，名字直接为"等级+羁绊名"，图标为空则不显示；颜色为羁绊连线色（可为空=默认白）
    public void UpdateBond(string name, string icon, Color? color)
    {
        expBar.gameObject.SetActive(false);
        cardName.text = name;
        cardName.color = color.HasValue ? color.Value : Color.white;
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

    public void UpdateExp(int pid, string name, int exp, string icon)
    {
        expBar.gameObject.SetActive(true);
        cardName.color = Color.white;
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

    }
}
