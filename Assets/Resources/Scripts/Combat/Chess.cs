using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chess : MonoBehaviour
{
    public int id;
    public int playerId;

    public int maxHp = 100;  // 最大生命值
    private Canvas canvas;
    private ChessHUD hud;
    public int side;
    public bool isHero;
    public int heroId;
    public string chessName = "0";
    public Renderer rend;

    // 目标单位
    private Chess targetChess;
    // 移动速度
    public float moveSpeed = 5f;
    public float attackRange = 10f;
    public int inte;
    public int str;
    public int leadShip;
    public int level = 1;
    public bool isShadow;
    public bool isFakeHero;

    public int lastDamagedPlayerId = -1;

    private Vector3? moveDirection = null;
    // 移动失败计数器
    private int moveFailCount = 0;
    // 最大连续移动尝试次数

    // 是否正在使用偏移路径
    public int hp = 100;
    public int attackDamage = 30;
    public string hitEffect;
    public int missileSpeed = 10;
    public float missileHight;
    public int soldierId;
    private int soldierLevel = 0;


    // 攻击冷却时间
    private float attackPoint;
    private float lastActionTime = 0f;
    private float lastTargetUpdateTime = 0f; // 上次更新目标的时间

    public HeroInfo heroInfo;

    public List<Skill> skills = new List<Skill>();

    public List<Buff> buffs = new List<Buff>();
    public int noMoveCount = 0;
    public int noActionCount = 0;

    public Material material;
    private Coroutine colorEffectCoroutine; // 协程引用，用于追踪颜色效果协程

    private bool DieAfterLifeTime;
    private float LifeTime;
    private Dictionary<int, AttrInfo> supportAttrs = new Dictionary<int, AttrInfo>();

    // Start is called before the first frame update
    void Start()
    {
        // 创建HUD
        CreateHUD();
    }

    public void Init(int pid, Color c)
    {
        // 创建材质实例
        material = new Material(rend.sharedMaterial);
        if (!string.IsNullOrEmpty(chessName))
        {
            if (chessName.StartsWith("PlayerPic"))
                material.mainTexture = Resources.Load<Texture>(chessName);
            else
                material.mainTexture = Resources.Load<Texture>("Skins/" + chessName);
        }
        material.SetColor("_OutlineColor", c);

        var hasSKill = false;

        if (isHero)
        {
            UnityEngine.Debug.Log("Init Hero" + heroId);

            var heroCfg = HeroConfig.GetConfig(heroId);
            // 初始化技能
            if (heroCfg.Skills != null)
            {
                foreach (var skillId in heroCfg.Skills)
                {
                    skills.Add(SkillManager.CreateSkill(skillId, this));
                    var skillCfg = SkillConfig.GetConfig(skillId);
                    if (!string.IsNullOrEmpty(skillCfg.Icon))
                    {
                        material.SetTexture("_SecondTex", Resources.Load<Texture>("SkillPic/" + skillCfg.Icon));
                        hasSKill = true;
                    }
                }
            }
        }

        if (!hasSKill)
        {
            material.SetFloat("_SecondTexSize", 0.1f);
        }

        if (!isHero)
        {
            var soldierCfg = SoldierConfig.GetConfig(soldierId);
            var playerInfo = GameManager.Instance.GetPlayer(playerId);
            if (playerInfo != null && soldierCfg.IsSoldier)
            {
                maxHp += playerInfo.sodhp * 5;
                attackDamage += playerInfo.sodatk;
            }
        }
        hp = maxHp;

        rend.material = material; // 这会为这个渲染器创建一个独立的材质实例
        if (heroInfo != null) // 英雄
            heroInfo.SetHpRate(hp, maxHp);
    }

    // 创建血条HUD
    private void CreateHUD()
    {
        // 查找或创建Canvas
        canvas = FindObjectOfType<Canvas>();

        // 加载Hud预制体
        GameObject hudPrefab = Resources.Load<GameObject>(isHero || isFakeHero ? "Prefabs/Hud" : "Prefabs/HudSmall");

        // 实例化HUD对象
        GameObject hudObj = Instantiate(hudPrefab, WorldManager.Instance.HudNode.transform);
        hudObj.name = "ChessHUD";

        // 获取ChessHUD组件
        hud = hudObj.GetComponent<ChessHUD>();
        if (hud == null)
        {
            Debug.LogError("ChessHUD component not found on Hud.prefab");
            return;
        }

        // 设置属性
        hud.chessUnit = this;
        //  hud.canvas = canvas;

        // 初始化血条显示
        hud.UpdateHealthDisplay();

    }

    // 寻找side不等于自己的单位
    private void FindTarget()
    {
        if (attackRange == 0)
            return;

        // 获取所有Chess组件
        Chess[] allChess = FindObjectsOfType<Chess>();
        List<(Chess chess, float distance)> validTargets = new List<(Chess, float)>();

        // 收集所有有效目标及其距离
        foreach (Chess chess in allChess)
        {
            if (chess != this && !chess.isShadow && WorldManager.Instance.IsEnemy(this.side, chess.side))
            {
                float distance = Vector3.Distance(transform.position, chess.transform.position);
                validTargets.Add((chess, distance));
            }
        }

        // 如果没有有效目标，直接返回
        if (validTargets.Count == 0)
        {
            targetChess = null;
            return;
        }

        // 按距离排序
        validTargets.Sort((a, b) => a.distance.CompareTo(b.distance));

        // 获取最近单位的距离
        float nearestDistance = validTargets[0].distance;

        // 筛选出距离不超过最近单位10的单位
        List<(Chess chess, float distance)> filteredTargets = validTargets
            .Where(t => t.distance <= nearestDistance + 10f)
            .ToList();

        // 如果筛选后不足3个，则取全部
        int takeCount = Mathf.Min(3, filteredTargets.Count);
        List<(Chess chess, float distance)> topTargets = filteredTargets.Take(takeCount).ToList();

        // 对目标进行打分
        List<(Chess chess, float score)> scoredTargets = new List<(Chess, float)>();
        foreach (var (chess, distance) in topTargets)
        {
            float score = CalculateTargetScore(chess, distance);
            scoredTargets.Add((chess, score));
        }

        // 按分数降序排序
        scoredTargets.Sort((a, b) => b.score.CompareTo(a.score));

        // 选择分数最高的作为目标
        targetChess = scoredTargets[0].chess;
    }

    // 计算目标分数
    private float CalculateTargetScore(Chess target, float distance)
    {
        float score = 10;

        if (!target.isHero)
            score += 30;

        // 距离权重（距离越近分数越高）
        score += 100f / (distance + 1f);  // 避免除以0

        // 添加最大属性差作为积分项（权重可根据游戏平衡调整）
        score += calculateDamage(this, target, out var type) / 2;

        // 生命值权重（生命值越低分数越高）
        if (target.hp < (int)(target.maxHp * 0.5))
            score *= 2;
        else
            score -= target.level * 5f;

        return score;
    }

    public void LogicUpdate(float deltaTime)
    {
        if (hp <= 0)
            return;

        if (lastActionTime < 1)
            lastActionTime = Time.time;

        if (hp > 0)
            MoveAndFight();
        lastActionTime = Time.time;

        if (DieAfterLifeTime)
        {
            LifeTime -= deltaTime;
            if (LifeTime <= 0)
            {
                Ondying();
            }
        }
    }

    public void LockTarget(Chess target1)
    {
        targetChess = target1;
        lastTargetUpdateTime = Time.time;
    }

    void MoveAndFight()
    {
        if (noActionCount > 0)
            return;

        // 每3秒重新寻找目标
        if (Time.time - lastTargetUpdateTime >= 3f)
        {
            FindTarget();
            lastTargetUpdateTime = Time.time;
        }

        // 检查目标是否存在
        if (targetChess == null || targetChess.hp <= 0)
        {
            // 如果没有目标，尝试寻找新目标
            FindTarget();

            if (targetChess == null)
                return;
        }

        // 检查是否有辅助技能
        if (SkillManager.CheckAidSkill(this))
            return;

        // 检查目标是否在攻击范围内
        if (WorldManager.Instance.CheckInRange(transform.position, targetChess.transform.position, attackRange))
        {
            attackPoint += Time.time - lastActionTime;

            // 检查攻击冷却
            if (attackPoint >= (attackRange > 20 ? 2 : 1.5f)) //集气2s
            {
                attackPoint = 0;
                SkillManager.AimTarget(this, targetChess);
                if (attackRange >= 20)
                {
                    WorldManager.Instance.CreateMissile(this, targetChess, hitEffect);
                }
                else
                {
                    Attack(targetChess); // 普通攻击
                }
            }
            return;
        }

        if (noMoveCount > 0 || moveSpeed == 0)
            return;

        if (moveDirection == null)
            moveDirection = targetChess.transform.position;

        //如果当前位置很接近moveDirection，就直接移动到moveDirection
        if (Vector3.Distance(transform.position, moveDirection.Value) <= moveSpeed * 0.1f)
        {
            moveDirection = targetChess.transform.position;
        }

        if (moveDirection != null)
        {
            // 计算下一步位置
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, moveDirection.Value, moveSpeed * 0.05f);

            // 尝试锁定目标格子
            if (WorldManager.Instance.TryLockGridPositions(this, nextPosition, out List<Vector2Int> requiredGrids))
            {
                WorldManager.Instance.DoLockGridPositions(this, requiredGrids);
                // 锁定成功，移动到新位置
                transform.position = nextPosition;
                moveFailCount = 0; // 重置失败计数器
            }
            else
            {
                // 锁定失败，不动
                moveFailCount++;

                // 根据连续失败次数尝试不同角度找路
                // 如果已经在使用偏移路径或者失败次数达到阈值，则继续使用偏移
                // 计算原始方向
                Vector3 direction = (targetChess.transform.position - transform.position).normalized;
                float angleOffset = 0f;

                // 根据失败次数确定偏移角度
                if (moveFailCount <= 3)
                    angleOffset = 45f;
                else if (moveFailCount <= 5)
                    angleOffset = 90f;
                else
                    angleOffset = 135f;

                // 随机选择向上或向下偏移
                angleOffset *= UnityEngine.Random.value > 0.5f ? 1 : -1;

                // 计算旋转后的方向
                Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
                Vector3 newDirection = rotation * direction;

                // 计算新的下一步位置
                nextPosition = transform.position + newDirection * moveSpeed * 0.05f;

                // 尝试移动到新位置
                if (WorldManager.Instance.TryLockGridPositions(this, nextPosition, out requiredGrids))
                {
                    WorldManager.Instance.DoLockGridPositions(this, requiredGrids);
                    transform.position = nextPosition;
                    moveDirection = transform.position + newDirection * moveSpeed * 0.05f * 10;
                    moveFailCount = 0; // 重置失败计数器
                }
            }
        }
    }

    void Update()
    {
        buffs.Where(x => Time.time > x.endTime).ToList().ForEach(x => BuffManager.RemoveBuff(this, x.id));

        foreach (var buff in buffs)
        {
            buff.Update();
        }

    }

    private void OnDestroy()
    {
        // 单位销毁时释放格子锁定
        if (WorldManager.Instance != null)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                WorldManager.Instance.ReleaseGridPositions(this);
            }
        }
    }

    public void CheckInitAttr(PlayerInfo player, int lv, List<int> friendIds)
    {
        level = lv;

        var heroConfig = HeroConfig.GetConfig(heroId);
        var attr = HeroSelectionTool.GetCardAttr(heroId, lv);

        maxHp = attr.Hp;
        moveSpeed = heroConfig.MoveSpeed;
        attackRange = heroConfig.Range;
        attackDamage = heroConfig.Atk * (9 + lv) / 10;
        inte = attr.Inte;
        str = attr.Str;
        leadShip = attr.Lead;

        if (player.itemEquips.ContainsKey(heroId))
        {
            var equipId = player.itemEquips[heroId];
            var cardLevel = HeroSelectionTool.GetCardLevel(player.cards[equipId]);

            var equipAttr = HeroSelectionTool.GetCardAttr(equipId, cardLevel);

            inte += equipAttr.Inte;
            str += equipAttr.Str;
            leadShip += equipAttr.Lead;
            maxHp += equipAttr.Hp;
        }

        if (friendIds != null)
        {
            foreach (var friendId in friendIds)
            {
                var friendAttr = GetSupportAttr(friendId, lv);
                if (friendAttr != null)
                {
                    supportAttrs[friendId] = friendAttr;
                    var friendChess = WorldManager.Instance.FindByHeroIdAndSide(friendId, side);
                    if (friendChess != null)
                    {
                        GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/LaserLine");
                        GameObject heroInstance = Instantiate(heroPrefab, Vector3.zero, Quaternion.identity);
                        heroInstance.transform.SetParent(transform);
                        heroInstance.transform.localScale = new Vector3(1, 1, 1);
                        var beam = heroInstance.transform.Find("Beam").GetComponent<GlowBeamController>();
                        beam.SetSourceAndTarget(this, friendChess);
                        beam.SetGlowColor(GetPlayerInfo().lineColor);
                    }
                }
            }

            foreach (var friendAttr in supportAttrs.Values)
            {
                inte += friendAttr.Inte;
                str += friendAttr.Str;
                leadShip += friendAttr.Lead;
            }
        }

        hp = maxHp;

        if (heroInfo != null)
            heroInfo.SetAttr(inte, str, leadShip);
    }

    public AttrInfo GetSupportAttr(int pid, int lv)
    {
        if(!ConfigManager.IsFriend(heroId, pid))
            return null;
        
        var friendHeroCfg = HeroConfig.GetConfig(pid);
        var myHeroCfg = HeroConfig.GetConfig(heroId);
        
        // 获取三个属性值
        int friendStr = friendHeroCfg.Str;
        int friendInte = friendHeroCfg.Inte;
        int friendLead = friendHeroCfg.LeadShip;
        
        int myStr = myHeroCfg.Str;
        int myInte = myHeroCfg.Inte;
        int myLead = myHeroCfg.LeadShip;
        
        // 计算差值
        int strDiff = friendStr - myStr;
        int inteDiff = friendInte - myInte;
        int leadDiff = friendLead - myLead;
        
        // 判断情况
        bool allLower = strDiff < 0 && inteDiff < 0 && leadDiff < 0;
        bool allHigher = strDiff > 0 && inteDiff > 0 && leadDiff > 0;
        
        int totalPoints;
        float[] weights = new float[3];

        weights[0] = FormulaLearnAttrConfig.GetConfig(friendStr - myStr).Weight;
        weights[1] = FormulaLearnAttrConfig.GetConfig(friendInte - myInte).Weight;
        weights[2] = FormulaLearnAttrConfig.GetConfig(friendLead - myLead).Weight;
        totalPoints = Math.Clamp((strDiff + inteDiff + leadDiff) / 2, 15, 25);

        // 计算总权重
        float totalWeight = weights[0] + weights[1] + weights[2];
        if (totalWeight <= 0)
            return new AttrInfo();
        
        // 计算属性点分配，避免四舍五入导致总和不等的问题
        float[] diffs = { weights[0], weights[1], weights[2] };
        int[] addValues = new int[3];
        
        // 找出三个差值中的排序索引（从小到大）
        int[] indices = { 0, 1, 2 }; // 0=Str, 1=Inte, 2=Lead
        Array.Sort(indices, (a, b) => diffs[a].CompareTo(diffs[b]));
        
        // 先计算最低差值的属性
        addValues[indices[0]] = Mathf.FloorToInt(totalPoints * weights[indices[0]] / totalWeight);
        addValues[indices[1]] = Mathf.FloorToInt(totalPoints * weights[indices[1]] / totalWeight);
        addValues[indices[2]] = totalPoints - addValues[indices[0]] - addValues[indices[1]];

        var attr = new AttrInfo();
        attr.Str = addValues[0] * (lv + 9) / 10;
        attr.Inte = addValues[1] * (lv + 9) / 10;
        attr.Lead = addValues[2] * (lv + 9) / 10;
        
        return attr;
    }

    public void UpdateAttr(int inte, int str, int leadShip)
    {
        if (inte > 0)
            this.inte = inte;
        if (str > 0)
            this.str = str;
        if (leadShip > 0)
            this.leadShip = leadShip;
        if (heroInfo != null)
            heroInfo.SetAttr(this.inte, this.str, this.leadShip);
    }

    // 只能开场用
    public void AddSoldierLevel(int lv)
    {
        if (isHero)
            return;

        var soldierCfg = SoldierConfig.GetConfig(soldierId);
        if (!soldierCfg.IsSoldier)
            return;

        //根据level变化模型scale
        soldierLevel += lv;
        transform.localScale = new Vector3(5 + soldierLevel * 0.75f, 3, 5 + soldierLevel * 0.75f);

        attackDamage += lv * 4;
        maxHp += lv * 20;
        hp = maxHp;
    }


    // 攻击目标
    public void Attack(Chess victim)
    {
        if (victim == null)
            return;

        // 造成伤害
        var damage = calculateDamage(this, victim, out var damType);
        var effect = hitEffect;
        var damageBase = damage;
        var damageMulti = 1f;

        SkillManager.DuringAttack(this, victim, damType, ref damageBase, ref damageMulti, ref effect);

        damage = (int)(damageBase * damageMulti);
        var minDamage = 10;
        var maxDamage = 60;
        if(isHero && victim.isHero)
        {
            //等级压制
            var levelDiff = level - victim.level;
            if(levelDiff != 0)
            {
                minDamage = Math.Max(2, minDamage + levelDiff * 2);
                maxDamage = Math.Max(10, maxDamage + levelDiff * 10);
            }
        }
        damage = Mathf.Clamp(damage, minDamage, maxDamage);
        //这里不改数值，只能伤害吸收
        SkillManager.BeforeAttack(this, victim, ref damage);

        victim.hp -= damage;
        if(victim != this)
            victim.lastDamagedPlayerId = playerId;

        SkillManager.OnAttack(this, victim, damType, damage);
        
        // 记录日志
        // Debug.Log($"{attacker.heroId}攻击{defender.heroId}，属性差值：Inte={inteDiff}, LeadShip={leadShipDiff}, Str={strDiff}，最大差值={maxDiff}，伤害：{damage}");

        EffectManager.PlayHitEffect(this, victim, effect);
        victim.OnHpChanged();
    }

    public void OnSkillDamaged(Chess speller, int damage)
    {
        hp -= damage;
        if(speller != this)
            lastDamagedPlayerId = speller.playerId;

        OnHpChanged();
    }


    public void OnHpChanged()
    {
        if (heroInfo != null) // 英雄
            heroInfo.SetHpRate(hp, maxHp);
        if (hp <= 0)
        {
            Ondying();
        }
    }

    public void Ondying()
    {
        buffs.Clear();
        WorldManager.Instance.OnUnitDying(this, lastDamagedPlayerId);

        Destroy(gameObject);

        if ((side == 1 || side == 2 && !isShadow ))
            GameManager.Instance.PlaySound("Sounds/tnt", 7);

        if (isHero)
        {
            foreach (var chess in WorldManager.Instance.GetUnitsMySide(transform.position, 0, side))
            {
                if (!chess.isHero)
                    continue;
                chess.OnFriendDie(heroId);
            }
        }
    }

    public void OnFriendDie(int friendId)
    {
        if (supportAttrs.ContainsKey(friendId))
        {
            var friendAttr = supportAttrs[friendId];
            inte -= friendAttr.Inte;
            str -= friendAttr.Str;
            leadShip -= friendAttr.Lead;
            supportAttrs.Remove(friendId);


            if (heroInfo != null)
                heroInfo.SetAttr(inte, str, leadShip);
        }
    }


    private int calculateDamage(Chess attacker, Chess defender, out string type)
    {
        if (!attacker.isHero || !defender.isHero)
        {
            type = "leadShip";
            return attacker.attackDamage;
        }

        // 计算攻击者三属性与防御者对应属性的差值
        float inteDiff = attacker.inte - defender.inte;
        float leadShipDiff = attacker.leadShip - defender.leadShip;
        float strDiff = attacker.str - defender.str;

        // 找出最大差值
        float maxDiff = Mathf.Max(inteDiff, leadShipDiff, strDiff);
        type = "";
        if(maxDiff == inteDiff)
        {
            type = "inte";
        }
        else if(maxDiff == leadShipDiff)
        {
            type = "leadShip";
        }
        else
        {
            type = "str";
        }

        // 伤害 = 最大差值 * 2
        int damage = Mathf.RoundToInt(maxDiff * 2);
        return damage;
    }

    public void AddHp(int addon)
    {
        hp = Mathf.Clamp(hp + addon, 0, maxHp);
        OnHpChanged();
    }

    public void Cooldown(float time)
    {
        attackPoint += time;
    }

    public void SetLifeTime(float time)
    {
        DieAfterLifeTime = true;
        LifeTime = time;
    }

    public PlayerInfo GetPlayerInfo()
    {
        return GameManager.Instance.GetPlayer(playerId);
    }


    public void AddColorEffect(Color start, Color end)
    {
        // 如果协程已经在运行，则直接返回
        if (colorEffectCoroutine != null)
            return;
        
        colorEffectCoroutine = StartCoroutine(ColorLerpCoroutine(start, end));
    }

    public void RemoveColorEffect()
    {
        // 停止颜色效果协程
        if (colorEffectCoroutine != null)
        {
            StopCoroutine(colorEffectCoroutine);
            colorEffectCoroutine = null;
        }
        
        // 恢复默认颜色
        material.SetColor("_Color", Color.white);
    }

    IEnumerator ColorLerpCoroutine(Color start, Color end)
    {
        float time = 0f;
        while (true)
        {
            // 使用正弦函数实现颜色平滑过渡
            float t = Mathf.Sin(time*20) * 0.5f + 0.5f;
            var color = Color.Lerp(start, end, t);
            UnityEngine.Debug.Log("ColorLerpCoroutine " + color + " start=" + start + " end=" + end);

            material.SetColor("_Color", color);
            time += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);

        }
    }

    public int GetAttr(string attr)
    {
        switch (attr)
        {
            case "inte":
                return inte;
            case "leadShip":
                return leadShip;
            case "str":
                return str;
            default:
                return 0;
        }
    }

    public void AddAttr(string attr, int value)
    {
        switch (attr)
        {
            case "inte":
                inte += value;
                break;
            case "leadShip":
                leadShip += value;
                break;
            case "str":
                str += value;
                break;
        }
        if(heroInfo != null)
            heroInfo.SetAttr(inte, str, leadShip);
    }

    public bool HasBuff(int id)
    {
        // Use Exists method since buffs is a List<Buff>
        return buffs.Exists(buff => buff.id == id);
    }

    public bool MoveTo(Vector3 targetPosition, bool isForce = false)
    {
        return WorldManager.Instance.MoveTo(this, targetPosition, isForce);
    }

}
