using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using CommonConfig;


public class PickPanelCellControl : MonoBehaviour
{
    public Image bgImg;
    public Image heroImg;
    public Image jobImg;
    public TMP_Text heroName;
    public Image forbidImg; // 点赞标记图（运行时会替换为红心 Textures/love，原字段名保留以兼容prefab引用）
    public Button banBtn;   // 点赞按钮（原ban按钮，字段名保留以兼容prefab引用）
    public int heroId;
    public bool canLike = false;    

    public int likeState; //0，未点赞，非0，玩家对应的点赞（pid+1）

    // Start is called before the first frame update
    void Start()
    {
        banBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            LikeBtnClick();
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

    public void SetLike(int pid)
    {
        if(!canLike)
            return;
        if(likeState > 0)
            return;

        if(ConfigManager.IsKingHero(heroId)) //主公不能点赞
            return;

        // 品质1的卡不进入许愿（收藏）池：较弱，避免占用许愿名额并在商店重复刷出
        if(HeroConfig.GetConfig(heroId).Quality == 1)
            return;

        likeState = pid + 1;
        var player = GameManager.Instance.GetPlayer(pid);
        player.likeCount--;
        forbidImg.color = player.lineColor;
        forbidImg.sprite = Resources.Load<Sprite>("Textures/love"); // 点赞显示红心
        forbidImg.gameObject.SetActive(true);
    }

    private void LikeBtnClick()
    {
        if(likeState == 0 && GameManager.Instance.GetPlayer(0).likeCount > 0)
        {
            SetLike(0);
        }
        else if(likeState == 1) //只能取消自己的点赞
        {
            likeState = 0;
            GameManager.Instance.GetPlayer(0).likeCount++;
            forbidImg.gameObject.SetActive(false);
        }
        
    }


}
