using UnityEngine;

// 收藏/点赞已被移除，本类保留为空壳组件以兼容场景预制体引用。
// 每局 like 阶段的收藏卡池改由 HeroSelectionTool.likeHeroPools 管理（按 pid 分玩家记录各玩家点赞的卡）。
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