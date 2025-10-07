using System.Collections.Generic;
using System.Linq;

public static class BattleStatManager
{
    public class BattleStat
    {
        public int playerId;
        public int heroId;
        public float damage;
        public int attackCount;
        public float skillDamage;
        public float heroDamage;
    }

    private static Dictionary<int, BattleStat> battleStats = new Dictionary<int, BattleStat>();

    public static void AddBattleStat(int playerId, int heroId, float damage, bool isAttack, bool isToHero)
    {
        var uid = playerId * 1000000 + heroId;
        if (battleStats.TryGetValue(uid, out var battleStat))
        {
            battleStat.damage += damage;
            if (isAttack)
                battleStat.attackCount++;
            else
                battleStat.skillDamage += damage;
            if (isToHero)
                battleStat.heroDamage += damage;
        }
        else
        {
            var battleStat1 = new BattleStat
            {
                playerId = playerId,
                heroId = heroId,
                damage = damage,
                attackCount = isAttack ? 1 : 0,
                skillDamage = isAttack ? 0 : damage,
                heroDamage = isToHero ? damage : 0,
            };
            battleStats.Add(uid, battleStat1);
        }
    }

    public static void Clear()
    {
        battleStats.Clear();
    }

    public static List<BattleStat> GetTop10()
    {
        return battleStats.Values.OrderByDescending(x => x.damage).Take(10).ToList();
    }
}
