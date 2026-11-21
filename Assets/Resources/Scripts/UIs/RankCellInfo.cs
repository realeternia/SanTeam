using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;


public class RankCellInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ILoopScrollItem
{
    public Image heroPic;
    public Image[] heroSkill;
    public TMP_Text heroName;
    public TMP_Text heroAtk;
    public TMP_Text heroArmor;
    public TMP_Text heroMagicRes;
    public TMP_Text heroHp;
    public TMP_Text heroPrice;

    public int heroId;
    public int atk;
    public int armor;
    public int magicRes;
    public int hp;
    public int price;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < heroSkill.Length; i++)
            heroSkill[i].raycastTarget = false;
        heroName.raycastTarget = false;
        heroAtk.raycastTarget = false;
        heroArmor.raycastTarget = false;
        heroMagicRes.raycastTarget = false;
        heroHp.raycastTarget = false;
        heroPrice.raycastTarget = false;
    }

    public void Init(HeroConfig heroConfig)
    {
        // 设置英雄信息
        heroPic.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroConfig.Icon);

        // 图标列表：第一格保留首个技能图标，其余格子显示该英雄的羁绊图标
        var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroConfig);
        var bondIcons = GetBondIcons(heroConfig.Id);
        int slot = 0;
        if (skillCfgs.Count > 0)
        {
            heroSkill[0].gameObject.SetActive(true);
            heroSkill[0].sprite = Resources.Load<Sprite>("Textures/SkillPic/" + skillCfgs[0].Icon);
            slot = 1;
        }
        for (int i = slot; i < heroSkill.Length; i++)
        {
            int bondIndex = i - slot;
            if (bondIndex < bondIcons.Count)
            {
                heroSkill[i].gameObject.SetActive(true);
                heroSkill[i].sprite = Resources.Load<Sprite>("Textures/SkillPic/" + bondIcons[bondIndex]);
            }
            else
            {
                heroSkill[i].sprite = null;
                heroSkill[i].gameObject.SetActive(false);
            }
        }

        heroName.text = heroConfig.Name;
        heroName.color = SysColor.GetQualityColor(HeroScenario.GetQuality(heroConfig));
        heroId = (int)heroConfig.Id;
        // 排行统一展示 1 星面板：HeroConfig 攻击/生命已由 HeroScenario 按当前剧本品质写回，
        // 护甲/魔抗为职业基准×(1+修正%)，均不随星级成长
        atk = heroConfig.Atk;
        armor = heroConfig.Armor;
        magicRes = heroConfig.MagicRes;
        hp = heroConfig.Hp;
        price = HeroSelectionTool.GetPrice(heroConfig);

        var bg = GetComponent<Image>();
        bg.color = SysColor.GetSideColor(HeroScenario.GetSide(heroConfig));

        // 不在卡池中的英雄名字置灰（池中单元格每次绑定都要刷新）
        if (!HeroSelectionTool.HasHeroInPool(heroId))
            heroName.color = Color.gray;

        // 5 列：攻击 / 护甲(防御) / 魔抗 / 生命 / 价格
        heroAtk.text = atk.ToString();
        heroArmor.text = armor.ToString();
        heroMagicRes.text = magicRes.ToString();
        heroHp.text = hp.ToString();
        heroPrice.text = price.ToString();
    }

    public void BindData(object data)
    {
        if (data is HeroConfig heroConfig)
        {
            Init(heroConfig);
        }
        else
        {
            GameLog.Warn($"RankCellInfo.BindData: 数据类型不匹配 data={data?.GetType().Name ?? "null"}");
        }
    }

    public void OnReturnToPool()
    {
    }

    // 羁绊图标：英雄所属好友羁绊组的关联技能图标（按羁绊组Id稳定排序）
    private static List<string> GetBondIcons(int heroId)
    {
        var icons = new List<string>();
        var friendIds = ConfigManager.GetHeroFriendInfo(heroId);
        if (friendIds == null)
            return icons;

        foreach (var friendId in friendIds.OrderBy(id => id))
        {
            var friendCfg = HeroFriendConfig.GetConfig(friendId);
            if (friendCfg == null || string.IsNullOrEmpty(friendCfg.SkillId))
                continue;
            var skillCfg = ConfigManager.GetSkillConfig(friendCfg.SkillId, 1);
            if (skillCfg != null && !string.IsNullOrEmpty(skillCfg.Icon))
                icons.Add(skillCfg.Icon);
        }
        return icons;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        // 按住才显示：松开立即隐藏
        PanelManager.Instance.GetTooltip<BaseTooltip>()?.HideTooltip();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 点击整格（含头像/技能图标/数值）都弹出英雄详情 Tips
        var heroCfg = HeroConfig.GetConfig(heroId);
        if (heroCfg == null)
        {
            GameLog.Warn($"RankCellInfo.OnPointerDown: 英雄配置不存在 heroId={heroId}");
            return;
        }

        var friendInfo = ConfigManager.GetHeroFriendInfo(heroId);
        var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
        // 排行无玩家上阵上下文：名字列表按商店口径全部按品质点亮
        PanelManager.Instance.GetTooltip<TooltipHero>()?.ShowTooltip(skillCfgs, friendInfo, heroId, null, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
