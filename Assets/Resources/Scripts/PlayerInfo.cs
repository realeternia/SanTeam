    // sellCount<=0 表示全部卖出（英雄整组）；物品每格一件，传入1只卖一件
    public void SellCard(int cardId, int sellCount = 0)
    {
        var isHeroCard = ConfigManager.IsHeroCard(cardId);
        if (!isHeroCard)
        {
            GameLog.Warn($"物品不可出售 cardId={cardId}，仅掉落获得");
            return;
        }

        var price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(cardId));
        var count = cards.TryGetValue(cardId, out var owned) ? owned : 0;
        if (sellCount > 0)
            count = Math.Min(sellCount, count);

        // 按“总价值 = 单价 × 数量”分段回收：
        // 5 以下：全额返还
        // 6~10：返还 -1
        // 11~15：返还 -2
        // 16~20：返还 -3
        // 以此类推
        int totalValue = price * count;
        int refund = GetCardSellRefund(totalValue);
        AddGold(refund);
        RemoveCard(cardId, count);
    }

    // 卖卡回收规则：总价值 V
    // V<=5 -> V
    // 6<=V<=10 -> V-1
    // 11<=V<=15 -> V-2
    // 16<=V<=20 -> V-3
    // 以此类推：返还 = V - floor((V-1)/5)
    private int GetCardSellRefund(int totalValue)
    {
        if (totalValue <= 0)
            return 0;
        if (totalValue <= 5)
            return totalValue;
        return totalValue - (totalValue - 1) / 5;
    }
