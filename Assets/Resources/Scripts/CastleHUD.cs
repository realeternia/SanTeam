using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CommonConfig;

public class CastleHUD : MonoBehaviour
{
    public TMP_Text castleName;
    public TMP_Text textAtk;
    public TMP_Text textHp;
    public Image healthImg;
    private int soldierLevel;
    private PlayerInfo owner;

    private int baseAtk;
    private int baseHp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(PlayerInfo p, Transform center)
    {
        owner = p;
        castleName.text = p.playerNameText.text;

        var soldierCfg = SoldierConfig.GetConfig(500001);
        baseAtk = soldierCfg.Atk + p.sodatk + p.GetItemPAttr("satk");
        baseHp = soldierCfg.Hp + p.sodhp + p.GetItemPAttr("shp");
        textAtk.text = baseAtk.ToString();
        textHp.text = baseHp.ToString();

        // 更新血条位置，使其跟随单位
        UpdatePosition(center);
    }

    private void UpdatePosition(Transform center)
    {
        Vector3 worldPosition = center.position + new Vector3(5, 3f, 5);
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPosition = WorldManager.Instance.TransformWorldToScreen(worldPosition, parentCanvas);
        rectTransform.anchoredPosition = screenPosition + new Vector2(-75, 0);
    }

    public void AddSoldierLevel(int level, int atkAdd, int hpAdd)
    {
        soldierLevel += level;
        var soldierCfg = SoldierConfig.GetConfig(500001);
        textAtk.text = (baseAtk + atkAdd).ToString();
        textHp.text = (baseHp + hpAdd).ToString();
        textAtk.color = Color.green;
        textHp.color = Color.green;
    }
}
