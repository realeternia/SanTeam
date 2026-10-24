using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CommonConfig;

public class BagRecycler : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public BagControl bagControl;

    // 功能模式："sell"=拖过来卖出，"unwear"=拖过来脱下该英雄所有装备进背包，
    // "compose"=拖过来打开该道具的合成配方侧边栏；在合成区拖入分解符则进入分解状态
    public string mode = "sell";

    // 分解状态：合成区拖入分解符(401011)后进入，背景切到 dcompose；点击可还原，分解成功后也还原
    private bool isSplitArmed;
    // 重铸状态：合成区拖入重铸镜(401009)后进入，背景切到 drandom；点击可还原，重铸成功后也还原
    private bool isRerollArmed;
    private Image bgImage;
    private Sprite defaultSprite;
    private Sprite splitSprite;
    private Sprite rerollSprite;

    private void Awake()
    {
        bgImage = GetComponent<Image>();
        if (bgImage != null)
            defaultSprite = bgImage.sprite;
    }

    // 当有物体拖放到此对象上时调用
    public void OnDrop(PointerEventData eventData)
    {
        // 获取拖动的BagCell
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null)
            return;

        BagCell draggedCell = draggedObject.GetComponent<BagCell>();
        if (draggedCell == null)
            return;

        draggedCell.RemoveTagImg();

        if (mode == "compose")
        {
            var itemCfg = ItemConfig.HasConfig(draggedCell.cardId) ? ItemConfig.GetConfig(draggedCell.cardId) : null;
            // 拖入分解符：进入分解状态（分解符本身不消耗，等真正分解时再扣）
            if (itemCfg != null && itemCfg.Effect == "tpsplit")
            {
                ArmSplit();
                return;
            }
            // 拖入重铸镜：进入重铸状态（重铸镜同样等真正重铸时再扣）
            if (itemCfg != null && itemCfg.Effect == "tpreroll")
            {
                ArmReroll();
                return;
            }
            // 分解状态下拖入装备：执行分解
            if (isSplitArmed)
            {
                bagControl.DecomposeItem(draggedCell.cardId);
                return;
            }
            // 重铸状态下拖入装备：执行重铸
            if (isRerollArmed)
            {
                bagControl.RerollItem(draggedCell.cardId);
                return;
            }
            bagControl.OpenComposePanel(draggedCell.cardId);
        }
        else if (mode == "unwear")
            bagControl.UnwearHeroEquips(draggedCell.cardId);
        else
            bagControl.SellCard(draggedCell.cardId);
    }

    // 分解状态下点击合成区：退出当前状态并还原背景图
    public void OnPointerClick(PointerEventData eventData)
    {
        if (mode == "compose" && (isSplitArmed || isRerollArmed))
            ResetSplitState();
    }

    // 进入分解状态：背景图切换为 dcompose.png
    private void ArmSplit()
    {
        if (isSplitArmed && !isRerollArmed)
            return;
        if (!EnsureSprite(ref splitSprite, "dcompose"))
            return;
        isSplitArmed = true;
        isRerollArmed = false;
        bgImage.sprite = splitSprite;
        SystemTip.Show("已进入分解状态");
    }

    // 进入重铸状态：背景图切换为 drandom.png
    private void ArmReroll()
    {
        if (isRerollArmed && !isSplitArmed)
            return;
        if (!EnsureSprite(ref rerollSprite, "drandom"))
            return;
        isRerollArmed = true;
        isSplitArmed = false;
        bgImage.sprite = rerollSprite;
        SystemTip.Show("已进入重铸状态");
    }

    // 准备状态切换用的背景图（Textures 根目录下），失败记录日志
    private bool EnsureSprite(ref Sprite cache, string spriteName)
    {
        if (bgImage == null)
        {
            GameLog.Error("BagRecycler.EnsureSprite: 合成区缺少 Image 组件，无法切换背景");
            return false;
        }
        if (cache == null)
        {
            cache = Resources.Load<Sprite>("Textures/" + spriteName);
            if (cache == null)
            {
                // 图片未按 Sprite 类型导入时，用 Texture2D 在运行时生成 Sprite
                var tex = Resources.Load<Texture2D>("Textures/" + spriteName);
                if (tex != null)
                    cache = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            }
        }
        if (cache == null)
        {
            GameLog.Error(string.Format("BagRecycler.EnsureSprite: 背景图不存在或无法加载 Textures/{0}", spriteName));
            return false;
        }
        return true;
    }

    // 退出分解/重铸状态：还原原始背景图
    public void ResetSplitState()
    {
        if (!isSplitArmed && !isRerollArmed)
            return;
        isSplitArmed = false;
        isRerollArmed = false;
        if (bgImage != null)
            bgImage.sprite = defaultSprite;
    }
}
