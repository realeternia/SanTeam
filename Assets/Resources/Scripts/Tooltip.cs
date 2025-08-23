using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public TMP_Text tooltipText;
    public RectTransform rect;
    public TMP_Text tooltipTitle;
    public Image tooltipIcon;
    public int maxWidth = 300;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        // else
        //     Destroy(gameObject);

        gameObject.SetActive(false);
    }

    private void Update()
    {

    }

    public void ShowTooltip(int skillId)
    {
        var skillConfig = SkillConfig.GetConfig(skillId);
        tooltipTitle.text = skillConfig.Name;
        tooltipText.text = "<color=yellow>[" + skillConfig.Price.ToString() + "元]</color>" + skillConfig.Descript; //富文本
        tooltipIcon.sprite = Resources.Load<Sprite>("SkillPic/" + skillConfig.Icon);

        rect.sizeDelta = new Vector2(300, 100 + tooltipText.preferredHeight);

        transform.position = Input.mousePosition + new Vector3(0, rect.sizeDelta.y + 100, 0); // 稍微偏移鼠标位置     
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0, Screen.height), transform.position.z); // 确保在屏幕内
        
        gameObject.SetActive(true);

        // 调整背景大小
        
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}