using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class RankCellInfo : MonoBehaviour
{
    public Image heroPic;
    public Image heroSkill;
    public TMP_Text heroName;
    public TMP_Text heroStr;
    public TMP_Text heroInte;
    public TMP_Text heroLeadShip;
    public TMP_Text heroHp;
    public TMP_Text heroPrice;
    public Button loveBtn;

    public int heroId;
    public int str;
    public int inte;
    public int leadShip;
    public int hp;
    public int price;

    // Start is called before the first frame update
    void Start()
    {
        loveBtn.onClick.AddListener(() =>
        {
            if (Profile.Instance.cardLoves.Contains(heroId))
                Profile.Instance.cardLoves.Remove(heroId);
            else if(Profile.Instance.cardLoves.Count < 5)
                Profile.Instance.cardLoves.Add(heroId);
            else
                return;

            Profile.Instance.SaveTextFile();
            UpdateLoveBtn();
        });
        UpdateLoveBtn();
    }

    private void UpdateLoveBtn()
    {
        if (Profile.Instance.cardLoves.Contains(heroId))
            loveBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("love");
        else
            loveBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("loveoff");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
