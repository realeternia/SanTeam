using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BagCell : MonoBehaviour
{
    public int cardId;
    public int level;
    public TMP_Text itemNameText;
    public Image itemImage;
    public Button cellButton;
    public BagControl bagControl;
    // Start is called before the first frame update
    void Start()
    {
        cellButton.onClick.AddListener(OnCellClick);
    }

    // 点击事件处理方法
    private void OnCellClick()
    {
        bagControl.OnCellClick(this);
    }

    public void OnSelect(bool isSelect)
    {
        if(isSelect)
        {
            cellButton.image.color = Color.green;
        }
        else
        {
            cellButton.image.color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
