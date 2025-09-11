using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CommonConfig;

public class CastleHUD : MonoBehaviour
{
    public TMP_Text castleName;
    public TMP_Text textAtk;
    public TMP_Text textHp;
    private int soldierLevel;
    private PlayerInfo owner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(PlayerInfo p, GameObject castleSpawn)
    {
        owner = p;
        castleName.text = p.playerNameText.text;

        var soldierCfg = SoldierConfig.GetConfig(500001);
        textAtk.text = (soldierCfg.Atk + p.sodatk).ToString();
        textHp.text = (soldierCfg.Hp + p.sodhp * 5).ToString();

        // 更新血条位置，使其跟随单位
        UpdatePosition(castleSpawn);
    }

    private void UpdatePosition(GameObject castleSpawn)
    {
        Vector3 worldPosition = new Vector3(castleSpawn.transform.position.x + 5, castleSpawn.transform.position.y + 3f, castleSpawn.transform.position.z + 5);
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPosition = WorldManager.Instance.TransformWorldToScreen(worldPosition, parentCanvas);
        rectTransform.anchoredPosition = screenPosition + new Vector2(-55, 25);
    }

    public void AddSoldierLevel(int level)
    {
        soldierLevel += level;
        var soldierCfg = SoldierConfig.GetConfig(500001);
        textAtk.text = (soldierCfg.Atk + owner.sodatk + soldierLevel * 4).ToString();
        textHp.text = (soldierCfg.Hp + owner.sodhp * 5 + soldierLevel * 20).ToString();
        textAtk.color = Color.green;
        textHp.color = Color.green;
    }
}
