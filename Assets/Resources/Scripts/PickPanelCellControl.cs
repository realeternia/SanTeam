using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;


public class PickPanelCellControl : MonoBehaviour
{
    public Image bgImg;
    public Image heroImg;
    public Image jobImg;
    public TMP_Text heroName;
    public Image forbidImg;
    public Button banBtn;
    public int heroId;
    public bool canBan = false;    

    public int banState; //0，不ban，非0，玩家对应的ban

    // Start is called before the first frame update
    void Start()
    {
        banBtn.onClick.AddListener(() =>
        {
            BanBtnClick();
        });

        // 设置forbidImg不阻挡鼠标点击
        if (forbidImg != null)
        {
            forbidImg.raycastTarget = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetBan(int pid)
    {
        if(!canBan)
            return;
        if(banState > 0)
            return;

        if(heroId < 100100) //主公不能ban
            return;

        banState = pid + 1;
        var player = GameManager.Instance.GetPlayer(pid);
        player.banCount--;
        forbidImg.color = player.lineColor;
        heroName.color = Color.gray;

        forbidImg.gameObject.SetActive(true);
    }

    private void BanBtnClick()
    {
        if(banState == 0 && GameManager.Instance.GetPlayer(0).banCount > 0)
        {
            SetBan(0);
        }
        else if(banState == 1) //只能解封自己的
        {
            banState = 0;
            GameManager.Instance.GetPlayer(0).banCount++;
            forbidImg.gameObject.SetActive(false);
            heroName.color = Color.white;

        }
        
    }


}
