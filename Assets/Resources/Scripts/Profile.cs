using UnityEngine;

// 收藏/点赞已被移除，本类保留为空壳组件以兼容场景预制体引用。
// 每局 like 阶段的收藏卡池改由 HeroSelectionTool.likeHeroPool 管理（8玩家×2张=16张）。
public class Profile : MonoBehaviour
{
    public static Profile Instance;

    void Start()
    {
        Instance = this;
    }

    void OnApplicationQuit()
    {
    }
}