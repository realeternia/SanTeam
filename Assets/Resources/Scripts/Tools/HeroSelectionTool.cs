using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

// 定义一个单独的工具类
public static class HeroSelectionTool
{
    // 星级成长倍率：每升1星 ×1.7（2星=1.7倍，3星=1.7²，以此类推）
    private const float StarGrowthPerStar = 1.7f;

    private static List<Tuple<int, int>> heroPoolCache = new List<Tuple<int, int>>();

    // 获取指定阵营的所有英雄ID
    public static List<int> GetAllHeroIdsBySide(int side)
    {
        List<int> heroIds = new List<int>();
        // 假设HeroConfig有一个方法GetAllConfigs()返回所有英雄配置
        foreach (var config in HeroConfig.ConfigList)
        {
            if (config.Side == side)
            {
                heroIds.Add((int)config.Id);
            }
        }
        return heroIds;
    }

    // 从源ID列表中随机选择指定数量的不重复ID
    public static List<int> GetRandomUniqueIds(List<int> sourceIds, int count)
    {
        List<int> result = new List<int>();
        if (sourceIds == null || sourceIds.Count == 0 || count <= 0)
        {
            return result;
        }

        // 创建源列表的副本以避免修改原列表
        List<int> tempIds = new List<int>(sourceIds);
        int actualCount = Mathf.Min(count, tempIds.Count);

        for (int i = 0; i < actualCount; i++)
        {
            int randomIndex = SysRandom.Range(0, tempIds.Count);
            result.Add(tempIds[randomIndex]);
            tempIds.RemoveAt(randomIndex);
        }

        return result;
    }


    public static void UpdateHeroPoolCache(List<int> heroIds)
    {
        heroPoolCache.Clear();
        foreach (var heroId in heroIds)
        {
            var config = HeroConfig.GetConfig(heroId);
            var rate = 1000 / Math.Max(5, GetPrice(config));
            heroPoolCache.Add(new Tuple<int, int>(heroId, rate));
        }

        heroPoolCache.Sort((a, b) =>
        {
            var configA = HeroConfig.GetConfig(a.Item1);
            var configB = HeroConfig.GetConfig(b.Item1);
            int sideCompare = configA.Side.CompareTo(configB.Side);
            if (sideCompare != 0)
            {
                return sideCompare;
            }

            // 主公（王）排在同阵营非主公之前
            bool isKingA = ConfigManager.IsKingHero(a.Item1);
            bool isKingB = ConfigManager.IsKingHero(b.Item1);
            if (isKingA != isKingB)
            {
                return isKingA ? -1 : 1;
            }

            // 按攻+法总面板排序（无双强度已在 PostModify 并入 Atk，Atk/Ap 为 1星带品质面板）
            return (configB.Atk + configB.Ap).CompareTo(configA.Atk + configA.Ap);
        });

    }

    public static List<int> GetHeroPoolCache()
    {
        // 返回只包含heroId的列表
        List<int> result = new List<int>();
        foreach (var hero in heroPoolCache)
        {
            result.Add(hero.Item1);
        }
        return result;
    }

    public static void SetBanList(List<int> banList)
    {
        heroPoolCache.RemoveAll(hero => banList.Contains(hero.Item1));
    }

    // 刷牌：先按GameRoundConfig品质概率roll出品质，再从该品质的英雄池随机选一张（ban 已在池中剔除）
    public static int GetRandomHeroIdByQuality(GameRoundConfig shopCfg)
    {
        int quality = RollQuality(shopCfg);
        List<int> candidates = new List<int>();
        foreach (var hero in heroPoolCache)
        {
            if (HeroConfig.GetConfig(hero.Item1).Quality == quality)
                candidates.Add(hero.Item1);
        }

        // 该品质池为空（如早期品质4未解锁或全部被ban）时，回退到整个池随机
        if (candidates.Count == 0)
            candidates = GetHeroPoolCache();
        if (candidates.Count == 0)
            return 0;

        return candidates[SysRandom.Range(0, candidates.Count)];
    }

    // 品质1=100-品质2-品质3-品质4
    private static int RollQuality(GameRoundConfig shopCfg)
    {
        int q2 = Math.Max(0, shopCfg.Quality2Rate);
        int q3 = Math.Max(0, shopCfg.Quality3Rate);
        int q4 = Math.Max(0, shopCfg.Quality4Rate);
        int q1 = Math.Max(0, 100 - q2 - q3 - q4);

        int roll = SysRandom.Range(0, 100);
        if (roll < q1)
            return 1;
        if (roll < q1 + q2)
            return 2;
        if (roll < q1 + q2 + q3)
            return 3;
        return 4;
    }

    public static bool HasHeroInPool(int heroId)
    {
        return heroPoolCache.Exists(hero => hero.Item1 == heroId);
    }

    public static int CountFriendInPool(int heroId)
    {
        int count = 0;
        foreach (var hero in heroPoolCache)
        {
            if (ConfigManager.GetFriendLevel(heroId, hero.Item1) > 0)
                count++;
        }
        return count;
    }

    public static int GetPrice(HeroConfig heroCfg)
    {
        return heroCfg.Price;
    }

    // 英雄近战/远程判定：HeroConfig.Range 经 ConfigManager.PostModify 写回为 职业基准×(1+修正%/100)
    // （近战职业 17，远程职业 35~70），按写回射程 > 20 判为远程，与战斗侧 JobLinkManager 的 attackRange>20 规则一致
    public static bool IsRangedHero(HeroConfig heroCfg)
    {
        return heroCfg != null && heroCfg.Range > 20;
    }

    public static bool IsMeleeHero(HeroConfig heroCfg)
    {
        return heroCfg != null && heroCfg.Range <= 20;
    }

    // 主属性面板（Atk/Ap/Hp 统一计算入口，无双强度已并入 Atk）：
    // HeroConfig 数值列经 ConfigManager.PostModify 写回为“1星带品质面板” = 职业基准×(1+修正%/100) × 品质系数1.15^(Q-1)，
    // 此处只按星级成长放大：每星 ×StarGrowthPerStar（2星=1.7倍，3星=1.7²，以此类推），对主属性统一
    public static AttrInfo GetHeroAttr(HeroConfig heroCfg, int lv)
    {
        var attrInfo = new AttrInfo();
        if (heroCfg == null)
        {
            GameLog.Error("HeroSelectionTool.GetHeroAttr: heroCfg 为 null，无法计算面板");
            return attrInfo;
        }
        int lvGrow = Mathf.Max(1, lv) - 1;
        attrInfo.Hp = GrowPanel(heroCfg.Hp, lvGrow);    // 生命独立成长字段
        attrInfo.Atk = GrowPanel(heroCfg.Atk, lvGrow);
        attrInfo.Ap = GrowPanel(heroCfg.Ap, lvGrow);
        return attrInfo;
    }

    private static int GrowPanel(int panelValue, int lvGrow)
    {
        // 1星带品质面板 × 1.7^已升星数（乘方式成长）
        return (int)Math.Round(panelValue * Mathf.Pow(StarGrowthPerStar, lvGrow));
    }

    // 1星带品质主属性面板：图鉴/排行/开局发卡/AI判断/卡池排序统一口径（= GetHeroAttr 的 lv=1，即 PostModify 写回值）
    public static AttrInfo GetRankAttr(HeroConfig heroCfg)
    {
        return GetHeroAttr(heroCfg, 1);
    }

    // 新卡牌升级曲线：累计卡数 1,4,8,13,20，后续继续递增以保持节奏
    private static int[] cardHeroExp = new int[] { 1, 4, 8, 13, 20, 28, 37, 48, 61, 76, 93, 112, 133, 156, 181, 208, 237, 268, 301, 336, 373, 412, 453, 496, 541, 588, 637, 688, 741, 796, 853, 912, 973, 1036, 1101, 1168, 1237, 1308, 1381, 1456, 1533, 1612, 1693, 1776, 1861, 1948, 2037, 2128, 2221, 2316, 2413, 2512, 2613, 2716, 2821, 2928, 3037, 3148, 3261, 3376, 3493, 3612, 3733, 3856, 3981, 4108, 4237, 4368, 4501, 4636, 4773, 4912, 5053, 5196, 5341, 5488, 5637, 5788, 5941, 6096, 6253, 6412, 6573, 6736, 6901, 7068, 7237, 7408, 7581, 7756, 7933, 8112, 8293, 8476, 8661, 8848, 9037, 9228, 9421, 9616, 9813, 10012, 10213, 10416, 10621, 10828, 11037, 11248, 11461, 11676, 11893, 12112, 12333, 12556, 12781, 13008, 13237, 13468, 13701, 13936, 14173, 14412, 14653, 14896, 15141, 15388, 15637, 15888, 16141, 16396, 16653, 16912, 17173, 17436, 17701, 17968, 18237, 18508, 18781, 19056, 19333, 19612, 19893, 20176, 20461, 20748, 21037, 21328, 21621, 21916, 22213, 22512, 22813, 23116, 23421, 23728, 24037, 24348, 24661, 24976, 25293, 25612, 25933, 26256, 26581, 26908, 27237, 27568, 27901, 28236, 28573, 28912, 29253, 29596, 29941, 30288, 30637, 30988, 31341, 31696, 32053, 32412, 32773, 33136, 33501, 33868, 34237, 34608, 34981, 35356, 35733, 36112, 36493, 36876, 37261, 37648, 38037, 38428, 38821, 39216, 39613, 40012, 40413, 40816, 41221, 41628, 42037, 42448, 42861, 43276, 43693, 44112, 44533, 44956, 45381, 45808, 46237, 46668, 47101, 47536, 47973, 48412, 48853, 49296, 49741, 50188, 50637, 51088, 51541, 51996, 52453, 52912, 53373, 53836, 54301, 54768, 55237, 55708, 56181, 56656, 57133, 57612, 58093, 58576, 59061, 59548, 60037, 60528, 61021, 61516, 62013, 62512, 63013, 63516, 64021, 64528, 65037, 65548, 66061, 66576, 67093, 67612, 68133, 68656, 69181, 69708, 70237, 70768, 71301, 71836, 72373, 72912, 73453, 73996, 74541, 75088, 75637, 76188, 76741, 77296, 77853, 78412, 78973, 79536, 80101, 80668, 81237, 81808, 82381, 82956, 83533, 84112, 84693, 85276, 85861, 86448, 87037, 87628, 88221, 88816, 89413, 90012, 90613, 91216, 91821, 92428, 93037, 93648, 94261, 94876, 95493, 96112, 96733, 97356, 97981, 98608, 99237, 99868, 100501, 101136, 101773, 102412, 103053, 103696, 104341, 104988, 105637, 106288, 106941, 107596, 108253, 108912, 109573, 110236, 110901, 111568, 112237, 112908, 113581, 114256, 114933, 115612, 116293, 116976, 117661, 118348, 119037, 119728, 120421, 121116, 121813, 122512, 123213, 123916, 124621, 125328, 126037, 126748, 127461, 128176, 128893, 129612, 130333, 131056, 131781, 132508, 133237, 133968, 134701, 135436, 136173, 136912, 137653, 138396, 139141, 139888, 140637, 141388, 142141, 142896, 143653, 144412, 145173, 145936, 146701, 147468, 148237, 149008, 149781, 150556, 151333, 152112, 152893, 153676, 154461, 155248, 156037, 156828, 157621, 158416, 159213, 160012, 160813, 161616, 162421, 163228, 164037, 164848, 165661, 166476, 167293, 168112, 168933, 169756, 170581, 171408, 172237, 173068, 173901, 174736, 175573, 176412, 177253, 178096, 178941, 179788, 180637, 181488, 182341, 183196, 184053, 184912, 185773, 186636, 187501, 188368, 189237, 190108, 190981, 191856, 192733, 193612, 194493, 195376, 196261, 197148, 198037, 198928, 199821, 200716, 201613, 202512, 203413, 204316, 205221, 206128, 207037, 207948, 208861, 209776, 210693, 211612, 212533, 213456, 214381, 215308, 216237, 217168, 218101, 219036, 219973, 220912, 221853, 222796, 223741, 224688, 225637, 226588, 227541, 228496, 229453, 230412, 231373, 232336, 233301, 234268, 235237, 236208, 237181, 238156, 239133, 240112, 241093, 242076, 243061, 244048, 245037, 246028, 247021, 248016, 249013, 250012, 251013, 252016, 253021, 254028, 255037, 256048, 257061, 258076, 259093, 260112, 261133, 262156, 263181, 264208, 265237, 266268, 267301, 268336, 269373, 270412, 271453, 272496, 273541, 274588, 275637, 276688, 277741, 278796, 279853, 280912, 281973, 283036, 284101, 285168, 286237, 287308, 288381, 289456, 290533, 291612, 292693, 293776, 294861, 295948, 297037, 298128, 299221, 300316, 301413, 302512, 303613, 304716, 305821, 306928, 308037, 309148, 310261, 311376, 312493, 313612, 314733, 315856, 316981, 318108, 319237, 320368, 321501, 322636, 323773, 324912, 326053, 327196, 328341, 329488, 330637, 331788, 332941, 334096, 335253, 336412, 337573, 338736, 339901, 341068, 342237, 343408, 344581, 345756, 346933, 348112, 349293, 350476, 351661, 352848, 354037, 355228, 356421, 357616, 358813, 360012, 361213, 362416, 363621, 364828, 366037, 367248, 368461, 369676, 370893, 372112, 373333, 374556, 375781, 377008, 378237, 379468, 380701, 381936, 383173, 384412, 385653, 386896, 388141, 389388, 390637, 391888, 393141, 394396, 395653, 396912, 398173, 399436, 400701, 401968, 403237, 404508, 405781, 407056, 408333, 409612, 410893, 412176, 413461, 414748, 416037, 417328, 418621, 419916, 421213, 422512, 423813, 425116, 426421, 427728, 429037, 430348, 431661, 432976, 434293, 435612, 436933, 438256, 439581, 440908, 442237, 443568, 444901, 446236, 447573, 448912, 450253, 451596, 452941, 454288, 455637, 456988, 458341, 459696, 461053, 462412, 463773, 465136, 466501, 467868, 469237, 470608, 471981, 473356, 474733, 476112, 477493, 478876, 480261, 481648, 483037, 484428, 485821, 487216, 488613, 490012, 491413, 492816, 494221, 495628, 497037, 498448, 499861, 501276, 502693, 504112, 505533, 506956, 508381, 509808, 511237, 512668, 514101, 515536, 516973, 518412, 519853, 521296, 522741, 524188, 525637, 527088, 528541, 529996, 531453, 532912, 534373, 535836, 537301, 538768, 540237, 541708, 543181, 544656, 546133, 547612, 549093, 550576, 552061, 553548, 555037, 556528, 558021, 559516, 561013, 562512, 564013, 565516, 567021, 568528, 570037, 571548, 573061, 574576, 576093, 577612, 579133, 580656, 582181, 583708, 585237, 586768, 588301, 589836, 591373, 592912, 594453, 596 - 1 };
    // 装备升级机制已移除：背包可存同id装备多件，每件独立生效，等级恒为1
    public static int GetCardLevel(int exp, bool isHero)
    {
        if(!isHero)
            return 1; // 装备不升级，多件同id装备各自生效
        for(int i = 0; i < cardHeroExp.Length; i++)
        {
            if(exp < cardHeroExp[i])
                return i;
        }
        return cardHeroExp.Length;
    }

    public static float GetExpRate(int exp, bool isHero)
    {
        if(!isHero)
            return 0; // 装备无升级进度条
        int level = GetCardLevel(exp, isHero);
        if(level >= cardHeroExp.Length)
            return 1f;
        if(level == 0)
            return 0;
        return (float)(exp - cardHeroExp[level - 1]) / (cardHeroExp[level] - cardHeroExp[level - 1]);
    }

    public static AttrInfo GetCardAttr(PlayerInfo player, int cardId, int lv)
    {
        var attrInfo = new AttrInfo();
        if (ConfigManager.IsHeroCard(cardId))
        {
            // 四主属性统一入口：职业基准×(1+修正%/100) × 品质系数 × 星级成长
            attrInfo = GetHeroAttr(HeroConfig.GetConfig(cardId), lv);
        }
        else
        {
            var itemConfig = ItemConfig.GetConfig(cardId);
            // 属性加成走 Attrs 配置（"attr+value,attr+value"，与 JobLink 同格式同比例约定）
            foreach (var bonus in JobLinkManager.ParseBonuses(itemConfig.Attrs))
                ApplyItemAttr(attrInfo, bonus.Attr, bonus.Value);
            // 装备升级机制已移除：属性不再乘等级，每件装备固定属性
        }
        if(player.attrAddons.ContainsKey(cardId))
            attrInfo.AddAttr(player.attrAddons[cardId]);

        return attrInfo;

    }

    // 道具属性键解析：四主属性(四维)外，支持护甲/魔抗/回蓝及金铲铲式基础组件的攻速/暴击
    // 比例属性（atkspeed/crit）按 Attrs 约定直接配比例（0.1=+10%），不再 ÷100；其余直接按数值
    private static void ApplyItemAttr(AttrInfo attrInfo, string key, float value)
    {
        if (string.IsNullOrEmpty(key) || value == 0)
            return;
        switch (key)
        {
            case "might": // 无双已并入攻击：老数据(未同步源表的 might 键)兼容为加攻击
            case "atk":
                attrInfo.Atk = (int)value;
                break;
            case "ap":
                attrInfo.Ap = (int)value;
                break;
            case "hp":
                attrInfo.Hp = (int)value;
                break;
            case "armor":
                attrInfo.Armor = (int)value;
                break;
            case "magicres":
                attrInfo.MagicRes = (int)value;
                break;
            case "mpRegen":
                attrInfo.MpRegen = value;
                break;
            case "hpRegen":
                attrInfo.HpRegen = value;
                break;
            case "atkspeed":
                attrInfo.AttackSpeedRate = value;
                break;
            case "crit":
                attrInfo.CritRate = value;
                break;
        }
    }

    // 品质色/阵营色定义已迁至 SysColor.GetQualityColor / SysColor.GetSideColor
}
