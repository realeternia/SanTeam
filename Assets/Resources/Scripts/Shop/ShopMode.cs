using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>商店模式类型（与 GameRoundConfig.ShopType 取值一致）</summary>
public enum ShopModeType
{
    /// <summary>共享轮换：8 名玩家共用一份商店，按积分顺序轮流买卡</summary>
    Shared = 0,
    /// <summary>独立买卡：每个玩家拥有自己的私有商店，各自买卡</summary>
    Independent = 1,
}

/// <summary>
/// 一份“可购卡报价”：把人类可见卡位与 AI 隐藏商店统一成同一个数据结构，
/// 让 AI 评分/购买逻辑不再直接依赖 CardViewControl。
/// </summary>
public class ShopOffer
{
    public int cardId;
    public bool isHero;
    public int price;   // 单价
    public int count;   // 剩余可购数量
    public CardViewControl view; // 可见卡位（人类/共享牌）；AI 隐藏商店为 null
}

/// <summary>
/// 商店流程抽象基类：CardShopManager 作为“宿主”提供预制体/容器/布局/购买/收尾等公共能力，
/// 具体模式只负责“生成本商店 + 操作节奏 + 回合推进”，方便后续扩展新的商店方案。
/// </summary>
public abstract class ShopMode
{
    protected CardShopManager host;

    public void Bind(CardShopManager h)
    {
        host = h;
    }

    public abstract ShopModeType Type { get; }

    /// <summary>商店阶段开始（宿主已完成 BGM/加钱/排序/year++/era 公共开场）：生成本模式的商店</summary>
    public abstract void Begin();

    /// <summary>当前“操作中”玩家：共享=回合玩家；独立=人类(pid0)</summary>
    public abstract PlayerInfo GetActingPlayer();

    /// <summary>某玩家当前可购卡（共享=全部未售共享卡；独立人类=可见卡位；独立AI=私有商店数据）</summary>
    public abstract List<ShopOffer> GetOffers(int pid);

    /// <summary>任一玩家（含 AI）买卡成功后回调：共享=触发售出倒计时/相邻刷新；独立=无</summary>
    public virtual void OnBought(ShopOffer offer, PlayerInfo player) { }

    /// <summary>仅“人类”买卡成功后回调：共享=推进回合；独立=无</summary>
    public virtual void OnHumanBought(PlayerInfo player, ShopOffer offer, int buyCount) { }

    /// <summary>人类点击“跳过/结束”</summary>
    public abstract void OnHumanPass();

    /// <summary>人类点击“刷新”</summary>
    public abstract void OnHumanRefresh();

    /// <summary>推进协程（AI 行动/等待人类），本阶段结束时调用 host.RequestEnd()</summary>
    public abstract IEnumerator Drive();

    /// <summary>阶段收尾</summary>
    public virtual void End() { }

    /// <summary>商店以外的面板（背包/排行/查看玩家等）打开时暂停 AI 推进；商店面板常驻，需排除</summary>
    protected static bool IsPanelBlocked()
    {
        return PanelManager.Instance != null
            && PanelManager.Instance.openPanelList.Any(p => p != null && p != PanelManager.Instance.cardShopPanel);
    }
}
