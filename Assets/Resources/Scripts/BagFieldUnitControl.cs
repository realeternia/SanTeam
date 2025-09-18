using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class BagFieldUnitControl : MonoBehaviour
{
    public Image heroIcon;
    public Image jobIcon;

    public void Init(int heroId)
    {
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
            if (heroCfg.Skills != null && heroCfg.Skills.Length > 0)
            {
                var skillConfig = SkillConfig.GetConfig(heroCfg.Skills[0]);
                jobIcon.sprite = Resources.Load<Sprite>("SkillPic/" + skillConfig.Icon);
            }
        }
    }
}