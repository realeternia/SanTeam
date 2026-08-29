using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CommonConfig;

public class BagFieldUnitControl : MonoBehaviour, IDropHandler
{
    public int posId;
    public Image heroIcon;
    public Image jobIcon;
    public BagControl bagControl;
    public int myHeroId;

    public void SetInfo(int id, int heroId)
    {
        myHeroId = heroId;
        posId = id;
        if (heroId == 0)
        {
            heroIcon.gameObject.SetActive(false);
            jobIcon.gameObject.SetActive(false);
        }
        else
        {
            heroIcon.gameObject.SetActive(true);
            jobIcon.gameObject.SetActive(true);
            var heroCfg = HeroConfig.GetConfig(heroId);
            heroIcon.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
            var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
            if (skillCfgs.Count > 0)
            {
                jobIcon.sprite = Resources.Load<Sprite>("SkillPic/" + skillCfgs[0].Icon);
            }
        }
    }

        // 当有物体拖放到此对象上时调用
    public void OnDrop(PointerEventData eventData)
    {       
        // 获取拖动的BagCell
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null)
            return;
        
        BagCell draggedCell = draggedObject.GetComponent<BagCell>();
        if (draggedCell == null || !ConfigManager.IsHeroCard(draggedCell.cardId))
            return;
        
        // 调用装备方法，与equipBtn相同的功能
        draggedCell.RemoveTagImg();
        bagControl.SetHeroForBattle(draggedCell.cardId, posId);
    }
}