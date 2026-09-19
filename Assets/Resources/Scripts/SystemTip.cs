using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class SystemTip : MonoBehaviour
{
    public static SystemTip Instance;

    public TMP_Text tipText1;
    public TMP_Text tipText2;
    // Start is called before the first frame update

    private Coroutine hideTipText1Coroutine;
    private Coroutine hideTipText2Coroutine;

    void Start()
    {
        Instance = this;
        tipText1.text = "";
        tipText2.text = "";
        tipText1.transform.parent.gameObject.SetActive(false);
        tipText2.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 统一入口：面板上的操作提示/错误提示都走这里（Shop/Bag 等）
    public static void Show(string tip)
    {
        if (Instance == null)
        {
            GameLog.Warn($"SystemTip 未就绪，提示未显示：{tip}");
            return;
        }
        Instance.ShowTip(tip);
    }

    public void ShowTip(string tip)
    {
        if(tipText2.text != "")
        {
            GameLog.Info("ShowTip: " + tip);
            if(hideTipText1Coroutine != null)
            {
                StopCoroutine(hideTipText1Coroutine);
                hideTipText1Coroutine = null;
            }
            if(hideTipText2Coroutine != null)
            {
                StopCoroutine(hideTipText2Coroutine);
                hideTipText2Coroutine = null;
            }            
            tipText1.text = tipText2.text;
            tipText2.text = tip;
            tipText1.transform.parent.gameObject.SetActive(true);

            hideTipText1Coroutine = StartCoroutine(HideTipText(tipText1));
            hideTipText2Coroutine = StartCoroutine(HideTipText(tipText2));
        }
        else
        {
            if(hideTipText2Coroutine != null)
            {
                StopCoroutine(hideTipText2Coroutine);
                hideTipText2Coroutine = null;
            }
            tipText2.text = tip;
            tipText2.transform.parent.gameObject.SetActive(true);

            //延迟3s，隐藏tipText2
            hideTipText2Coroutine = StartCoroutine(HideTipText(tipText2));
        }
    }
    IEnumerator HideTipText(TMP_Text tipText)
    {
        yield return new WaitForSeconds(3);
        tipText.transform.parent.gameObject.SetActive(false);
        tipText.text = "";
    }
}
