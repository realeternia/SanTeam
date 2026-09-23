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
    public int pos;
    public bool sneakSwapped; // 偷袭交换标记：每个敌方英雄整场只可能被交换一次（战斗内Chess实例每次新建，天然按场重置）


    // 目标单位
    public Chess targetChess;
    // 移动速度
    public float moveSpeed = 5f;
    public float attackRange = 10f;
    public int ap;      // 法术强度（原智力）
    public int atk;     // 攻击（原统王；无双强度已并入此值）
    public int level = 1;
    public bool isShadow;
    public bool isFakeHero;
    public float dodgeRate; //闪避
    public float critRate; //暴击
    public float critDamageMulti = 0.5f; //暴击伤害倍率
    public float healRate; //治疗强化系数（0.1=治疗效果+10%）
    public float healedRate; //受治疗系数（可为负，-0.1=受到的治疗-10%，减疗）
    public float auroEffectRate = 1f; //光环效果加成系数（1=无加成，鼓光环等 AuroAttrs 光环属性效果值乘算）

    public int lastDamagedPlayerId = -1;

    private Vector3? moveDest = null;
    // 上次短程寻路重规划时间(移动目标远距离变化大，定期重算途经点)
    private float moveDestPlanTime = -1f;
    // 单位侧向错位方向(±1，按id奇偶分边)：拥挤时叠加横向分量打散同向队列，独行不偏移
    private float laneBias;
    // 地面基准高度与微调偏移：贴脸单位按id微调y错开(0/0.15/0.3三档)，避免模型重叠z-fighting闪面
    public float baseY;
    public float heightOffset;

    // 是否正在使用偏移路径
    public int hp = 100;
    public float soldierAtkRate = 1f; // 士兵攻击加成系数（相的职业羁绊：全军士兵攻击+%，ApplyJobLinks 末尾结算时折算进 atk）
    public float soldierHpRate = 1f; // 士兵生命加成系数（相的职业羁绊：全军士兵生命+%，由 JobLinkManager 累加系数后统一结算）
    public int soldierBaseMaxHp; // 士兵初始生命基准快照（Init 末尾记录，JobLinkManager 按此基数统一结算生命系数）

    // 护甲/魔抗：英雄与士兵初始化时从配置赋值
    public int armor;
    public int magicRes;
    public string hitEffect;
    public int missileSpeed = 10;
    public float missileHight;
    public int soldierId;
    /// <summary>召唤物标签（技能召唤时由 SkillConfig.SummonTag 标记，如"火"，用于技能识别场景中的召唤物类型）</summary>
    public string SummonTag;
    private int soldierLevel = 0;


    // 攻击冷却时间
    public float attackPoint;
    public float attackSpeed; //攻击频率（每秒攻击次数，=攻速值/30；攻速20=1.5秒/次，15=2秒/次）
    public float attackSpeedRate; //攻速比例加成（0.1=+10%，最终攻速=attackSpeed×(1+attackSpeedRate)）
    private float lastAttackTime = 0f;
    private float lastTargetUpdateTime = 0f; // 上次更新目标的时间

    public HeroInfo heroInfo;

    public List<Skill> skills = new List<Skill>();

    public List<Buff> buffs = new List<Buff>();
    public List<BuffTime> buffTimes = new List<BuffTime>(); //记录最近20s的buff记录
    public int noMoveCount = 0;
    public int noActionCount = 0;

    public Renderer rend;
    public Material material;
    public Renderer rendFlag;
    public Material materialFlag;    
    private Coroutine colorEffectCoroutine; // 协程引用，用于追踪颜色效果协程

    private bool dieAfterLifeTime;
    private float lifeTime;

    private float secondTimer; //每秒事件计时，满1s触发一次OnSecond
    public float hpRegen; //生命回复/秒（正=回复，负=扣减；来源：属性羁绊或复原/药仙技能加成，OnSecond事件结算）
    public float mpRegen; //法力回复/秒（为设置了MpCost的技能持续充能，可为负=倒扣，OnSecond事件结算）

    // Start is called before the first frame update
    void Start()
    {
        // 创建HUD
        CreateHUD();
    }

    public void Init(int pid, int posId, Color c)
    {
        playerId = pid;
        pos = posId;
        // 侧向错位方向按id奇偶分边，避免同向部队整齐排队成一条线
        laneBias = id % 2 == 0 ? 1f : -1f;
        // 记录地面基准高度并微调y错开：贴脸单位高度差0.15m三档，避免模型重叠z-fighting闪面
        baseY = transform.position.y;
        heightOffset = (id % 10) * 0.0015f;
        var initPos = transform.position;
        initPos.y = baseY + heightOffset;
        transform.position = initPos;
        // 创建材质实例
        material = new Material(rend.sharedMaterial);
        if (!string.IsNullOrEmpty(chessName))
        {
            if (chessName.StartsWith("Textures/"))
                material.mainTexture = Resources.Load<Texture>(chessName);
            else if (chessName.StartsWith("PlayerPic") || chessName.StartsWith("MonsterPic"))
                material.mainTexture = Resources.Load<Texture>("Textures/" + chessName);
            else
                material.mainTexture = Resources.Load<Texture>("Textures/Skins/" + chessName);
        }
        material.SetColor("_OutlineColor", c);

        var hasSKill = false;

        if (isHero)
        {
            GameLog.Debug("Init Hero" + heroId);

            var heroCfg = HeroConfig.GetConfig(heroId);
            var jobCfg = ConfigManager.GetJobConfig(heroCfg.Job);
            var jobSkillSname = jobCfg != null ? jobCfg.SkillId : "";
            var playerInfo = GameManager.Instance.GetPlayer(playerId);
            // 初始化技能：默认取1级行创建，随后按来源修正等级——
            // 个人技能(Skill1)等级 = 卡片等级（超出技能配置最高等级行时按最高等级行生效）；兵种技能为占位技能(Dumb)，职业被动加成由 JobLinkManager 按同职业英雄数直接施加；
            // 好友特殊技能由 FriendLineManager 按在场好友数计算。
            foreach (var skillCfg in ConfigManager.GetHeroSkillConfigs(heroCfg))
            {
                var skill = SkillManager.CreateSkill(skillCfg.Id, this);
                if (skillCfg.Sname != jobSkillSname && playerInfo != null && playerInfo.cards.TryGetValue(heroId, out int heroExp))
                    skill.SetLevel(HeroSelectionTool.GetCardLevel(heroExp, true));
                skills.Add(skill);
            }

            // 棋子上叠加的第二个纹理用职业图标（取该职业对应技能的 Icon，资源在 Textures/SkillPic/）
            var jobSkillCfg = ConfigManager.GetSkillConfig(jobSkillSname);
            if (jobSkillCfg != null && !string.IsNullOrEmpty(jobSkillCfg.Icon))
            {
                material.SetTexture("_SecondTex", Resources.Load<Texture>("Textures/SkillPic/" + jobSkillCfg.Icon));
                hasSKill = true;
            }

            materialFlag = new Material(rendFlag.sharedMaterial);
            materialFlag.mainTexture = Resources.Load<Texture>(playerInfo.imgPath);
            rendFlag.material = materialFlag;
            GameLog.Debug("Init Hero done " + heroId);
        }

        if (!hasSKill)
            material.SetFloat("_SecondTexSize", 0.1f);
        rend.material = material; // 这会为这个渲染器创建一个独立的材质实例

        if (!isHero)
        {
            var soldierCfg = SoldierConfig.GetConfig(soldierId);
            // playerId=999为PVE怪物(虚拟玩家，无PlayerInfo实体)，不享受任何玩家加成
            var playerInfo = (playerId >= 0 && playerId < GameManager.Instance.players.Length) ? GameManager.Instance.GetPlayer(playerId) : null;
            if (playerInfo != null && soldierCfg.SoldierAtkRate > 0)
            {
                maxHp += (int)((playerInfo.sodhp + playerInfo.GetItemPAttr("shp") + playerInfo.GetSoldierHpAdd()) * soldierCfg.SoldierHpRate);
                atk += (int)((playerInfo.sodatk + playerInfo.GetItemPAttr("satk") + playerInfo.GetSoldierAtkAdd()) * soldierCfg.SoldierAtkRate);
            }
        }
        // 记录士兵初始生命基准：职业生命系数结算始终以它为基数（士兵无其他生命来源，此值即为战斗前最终生命）
        soldierBaseMaxHp = maxHp;
        hp = maxHp;
        if (heroInfo != null) // 英雄
            heroInfo.SetHpRate(hp, maxHp);
        
        attackPoint = SysRandom.Range(0f, 1f); // 随机获得初始气力
        // attackSpeed 已在 SpawnUnitsForRegion/SpawnHerosForRegion 中按配置设置（攻速值/30），此处不能覆盖
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
            GameLog.Error("ChessHUD component not found on Hud.prefab");
            return;
        }

        // 设置属性
        hud.chessUnit = this;
        //  hud.canvas = canvas;

        // 初始化血条显示
        hud.UpdateHealthDisplay();

    }


    public void LogicUpdate(float deltaTime)
    {
        if (hp <= 0)
            return;

        buffs.Where(x => Time.time > x.endTime).ToList().ForEach(x => BuffManager.RemoveBuff(this, x.id));

        // 每秒计时，满1s触发一次OnSecond事件（回复/充能等按秒结算的逻辑统一在该事件处理）
        secondTimer += deltaTime;
        while (secondTimer >= 1)
        {
            secondTimer -= 1;
            OnSecond();
        }

        MoveAndFight(deltaTime);

        if (dieAfterLifeTime)
        {
            lifeTime -= deltaTime;
            if (lifeTime <= 0)
            {
                Ondying();
            }
        }
    }

    // 每秒事件：hpRegen/mpRegen 等按秒结算的逻辑统一在此处理
    private void OnSecond()
    {
        if (hpRegen != 0)
        {
            // 生命回复属性：正=回复，负=扣减（可为负=持续扣减）
            hp = (int)Mathf.Clamp(hp + hpRegen, 0, maxHp);
            OnHpChanged();
            if (hp <= 0)
                Ondying();
        }

        if (mpRegen != 0)
        {
            // 法力回复属性：为所有设置了MpCost的技能持续充能（可为负=充能倒扣）
            foreach (var skill in skills)
                skill.AddRegenMp(mpRegen);
        }
    }


    void Update()
    {

    }

    public void CheckInitAttr(PlayerInfo player, int lv)
    {
        level = lv;

        var heroConfig = HeroConfig.GetConfig(heroId);
        var attr = HeroSelectionTool.GetCardAttr(player, heroId, lv);

        maxHp = attr.Hp;
        // 次级面板（移速/射程/攻速/护甲/魔抗）已由 PostModify 写回为 职业基准×(1+修正%/100)
        moveSpeed = heroConfig.MoveSpeed;
        attackRange = heroConfig.Range;
        attackSpeed = heroConfig.AtkSpeed / 30f; // 攻速值→每秒攻击次数（30=1次/秒；攻速20=1.5秒/次，15=2秒/次）
        ap = attr.Ap;
        atk = attr.Atk;
        armor = heroConfig.Armor;
        magicRes = heroConfig.MagicRes;
        // 生命/魔法回复同样由 PostModify 写回职业基准（OnSecond 中按秒结算）；装备加成在下方累加
        hpRegen = heroConfig.HpRegen;
        mpRegen = heroConfig.MpRegen;

        // 装备升级机制已移除：装备属性固定，不再按持有数量计算等级；最多3件装备属性累加
        var equipIds = player != null ? player.GetItemIdsOnHero(heroId) : new List<int>();
        if (player != null)
        {
            foreach (var equipId in equipIds)
            {
                if (equipId == 0)
                    continue;
                var equipAttr = HeroSelectionTool.GetCardAttr(player, equipId, 1);

                ap += equipAttr.Ap;
                atk += equipAttr.Atk;
                maxHp += equipAttr.Hp;
                // 金铲铲式基础组件扩展属性：护甲/魔抗/攻速/暴击/回蓝
                armor += equipAttr.Armor;
                magicRes += equipAttr.MagicRes;
                attackSpeedRate += equipAttr.AttackSpeedRate;
                critRate += equipAttr.CritRate;
                mpRegen += equipAttr.MpRegen;

                // 装备技能：ItemConfig.SkillId 按 sname 引用（空=无），此处为该英雄卡添加对应技能（固定1级）
                var equipCfg = ItemConfig.GetConfig(equipId);
                if (equipCfg != null && !string.IsNullOrEmpty(equipCfg.SkillId))
                {
                    var equipSkillCfg = ConfigManager.GetSkillConfig(equipCfg.SkillId, 1) ?? ConfigManager.GetSkillConfig(equipCfg.SkillId);
                    if (equipSkillCfg != null)
                        AddSkill(equipSkillCfg.Id, equipSkillCfg.Id, 1);
                    else
                        GameLog.Warn($"装备所属技能未配置：itemId={equipId} SkillId={equipCfg.SkillId}");
                }
            }
        }

        hp = maxHp;

        if (heroInfo != null)
            heroInfo.SetAttr(ap, atk);
    }

    // 刷新英雄属性显示(连线加成在战斗开始时应用后调用)
    public void RefreshHeroAttr()
    {
        if (heroInfo != null)
            heroInfo.SetAttr(ap, atk);
    }

    public void UpdateAttr(int ap, int atk)
    {
        if (ap > 0)
            this.ap = ap;
        if (atk > 0)
            this.atk = atk;
        if (heroInfo != null)
            heroInfo.SetAttr(this.ap, this.atk);
    }

    // 只能开场用
    public void AddSoldierLevel(int lv, int atkAdd, int hpAdd)
    {
        if (isHero)
            return;

        var soldierCfg = SoldierConfig.GetConfig(soldierId);
        if (soldierCfg.SoldierAtkRate <= 0)
            return;

        //根据level变化模型scale
        soldierLevel += lv;
        transform.localScale = new Vector3(5 + soldierLevel * 0.75f, 3, 5 + soldierLevel * 0.75f);

        atk += (int)(lv * atkAdd * soldierCfg.SoldierAtkRate);
        maxHp += (int)(lv * hpAdd * soldierCfg.SoldierHpRate);
        hp = maxHp;
    }

    public void LockTarget(Chess target1)
    {
        targetChess = target1;
        lastTargetUpdateTime = Time.time;
    }

    // 寻找side不等于自己的单位
    public void FindTarget()
    {
        if (attackRange == 0)
            return;

        // 获取所有敌方单位：range传0表示全地图索敌（单位必须知道远处敌人的位置才能向其推进，近战单位射程近不能因此失去索敌能力）
        var allChess = WorldManager.Instance.GetUnitsInRange(transform.position, 0, side, true);
        List<(Chess chess, float distance)> validTargets = new List<(Chess, float)>();

        // 收集所有有效目标及其距离
        foreach (Chess chess in allChess)
        {
            if (chess != this)
            {
                float distance = WorldManager.Instance.GetRange(transform.position, chess.transform.position);
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
        List<(Chess chess, float distance)> filteredTargets = null;
        if(nearestDistance <= attackRange)
            filteredTargets = validTargets.Where(t => t.distance <= attackRange).ToList(); //如果有射程内的，就继续找一个射程内的
        else
            filteredTargets = validTargets.Where(t => t.distance <= nearestDistance + 10f).ToList();

        // 如果筛选后不足3个，则取全部
        int takeCount = Mathf.Min(3, filteredTargets.Count);
        List<(Chess chess, float distance)> topTargets = filteredTargets.Take(takeCount).ToList();

        // 对目标进行打分
        List<(Chess chess, float score)> scoredTargets = new List<(Chess, float)>();
        foreach (var (chess, distance) in topTargets)
        {
            float score = CalculateTargetScore(chess);
            scoredTargets.Add((chess, score));
        }

        // 按分数降序排序
        scoredTargets.Sort((a, b) => b.score.CompareTo(a.score));

        // 选择分数最高的作为目标
        targetChess = scoredTargets[0].chess;
    }

    // 计算目标分数
    private float CalculateTargetScore(Chess target)
    {
        float score = target.isHero ? 10 : 30;

        // 生命值权重（生命值越低分数越高）
        var targetHpRate = (float)target.hp / target.maxHp;
        if (targetHpRate < 0.5f)
            score += (0.5f - targetHpRate) * 100f + 10;

        return score;
    }

    private void MoveAndFight(float deltaTime)
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
            attackPoint += deltaTime * attackSpeed * (1 + attackSpeedRate);
            // 检查攻击冷却（攻击频率累积满1次即可出手，attackSpeed=攻速值/30，attackSpeedRate为乘法比例加成）
            if (attackPoint >= 1f)
            {
            //    PlayerAnim("jumpspin");
                attackPoint = 0;
                SkillManager.AimTarget(this, targetChess);
                if (attackRange >= 20)
                {
                    WorldManager.Instance.CreateAttackMissile(this, targetChess, hitEffect);
                }
                else
                {
                    Attack(targetChess, hitEffect); // 普通攻击

                }
            }
            lastAttackTime = Time.time;
            return;
        }

        if (noMoveCount > 0 || moveSpeed == 0)
            return;

        // 短程寻路重规划：无目的地/到达途经点/定期重算，绕墙取下一格
        float now = Time.time;
        if (moveDest == null
            || now - moveDestPlanTime >= CombatConst.MoveReplanInterval
            || WorldManager.Instance.GetRange(transform.position, moveDest.Value) <= moveSpeed * 0.1f)
        {
            ReplanMoveDest();
        }

        if (moveDest != null)
        {
            // 基础移动方向：朝向途经点(直线通畅时途经点即目标位置)
            Vector3 moveDir = moveDest.Value - transform.position;
            moveDir.y = 0;
            float dist = moveDir.magnitude;
            if (dist < 0.01f)
                return;
            moveDir /= dist;

            // 互斥力：与周围过近的单位互相推开(目标除外)，防止贴脸黏住、顺势绕人
            Vector3 separation = GetSeparationPush();
            if (separation.sqrMagnitude > 0.0001f)
                moveDir = (moveDir + separation).normalized;

            // 侧向错位：拥挤(存在互斥力)时叠加横向分量，打散同向队列；独行时不偏移
            Vector3 lateral = Vector3.Cross(Vector3.up, moveDir) * laneBias
                * CombatConst.MoveLaneBias * Mathf.Min(separation.magnitude, 1f);
            moveDir = (moveDir + lateral).normalized;

            // 计算下一步位置
            Vector3 nextPosition = transform.position + moveDir * moveSpeed * 0.05f;

            // 兜底：下一位置踩进墙(寻路失效)则原地等待下次重规划，绝不穿墙
            if (WorldManager.Instance.CheckPositionBlocked(this, nextPosition))
                return;

            transform.position = nextPosition;
        }
    }

    // 短程寻路重规划：朝目标绕墙取下一个途经点；直线通畅则直接朝目标
    private void ReplanMoveDest()
    {
        moveDestPlanTime = Time.time;
        var wp = WorldManager.Instance.FindMoveWaypoint(transform.position, targetChess.transform.position);
        if (wp.HasValue)
        {
            var p = wp.Value;
            p.y = transform.position.y;
            moveDest = p;
        }
        else
        {
            // 直线通畅(或无法寻路时直接朝目标)，由踩墙校验兜底
            moveDest = targetChess.transform.position;
        }
    }

    // 互斥力：与周围过近的单位互相推开(目标除外)，防止贴脸黏住导致移动卡死
    private Vector3 GetSeparationPush()
    {
        Vector3 push = Vector3.zero;
        // 获取半径4格(约12米)内敌我双方单位，再按米级距离过滤
        var nearUnits = WorldManager.Instance.GetUnitsInRange(transform.position, 4f, side, true);
        nearUnits.AddRange(WorldManager.Instance.GetUnitsInRange(transform.position, 4f, side, false));
        foreach (var other in nearUnits)
        {
            if (other == this || other == targetChess || other.hp <= 0 || other.isShadow)
                continue;
            Vector3 offset = transform.position - other.transform.position;
            offset.y = 0;
            float dist = offset.magnitude;
            if (dist < 0.01f || dist >= CombatConst.MoveSeparationDist)
                continue;
            // 同阵营：恒定强推力(>1 压住前进意图)，防止跟屁股堆叠黏住；
            // 敌方(非目标)：线性衰减推开，顺滑擦身而过
            float strength = other.side == side
                ? CombatConst.MoveSeparationAllyForce
                : CombatConst.MoveSeparationForce * (1f - dist / CombatConst.MoveSeparationDist);
            push += offset / dist * strength;
        }
        return push;
    }

    // 攻击目标
    public void Attack(Chess victim, string hitEffectName)
    {
        if (victim == null)
            return;

        // 普攻基准统一为 atk（英雄取攻击；士兵加成系数已折算进atk），受目标护甲减免：
        // 实际护甲 = 目标护甲 × 攻击方破甲/受击方加甲修正系数（实际伤害 = 攻击 × 100/(100+等效护甲)）
        var damage = Math.Max(1, (int)(atk * CombatConst.ResistMultiplier(victim.GetEffectiveArmor(this))));
        var effect = hitEffectName;
        var damageBase = damage;
        var damageMulti = 1f;

        // 暴击
        if (critRate > 0 && SysRandom.Value < critRate)
        {
            damageMulti += critDamageMulti;
            WorldManager.Instance.AddBattleText("暴!", transform.position, new UnityEngine.Vector2(0, 40), Color.red, 3);
        }

       if (victim.dodgeRate > 0 && SysRandom.Value < victim.dodgeRate)
        {
            damage = 0;
            WorldManager.Instance.AddBattleText("闪!", victim.transform.position, new UnityEngine.Vector2(0, 40), Color.red, 3);
        }
        else
        {
            // 伤害计算阶段·统一入口：普攻与技能伤害都进入，调整伤害基数与倍率（增伤/减伤/破甲/连锁等）
                SkillManager.BeforeCalDamaged(this, victim, null, ref damageBase, ref damageMulti, ref effect, "", false);
            damage = (int)(damageBase * damageMulti);
            // 结算阶段·受击方：只做伤害吸收（护盾），不做伤害放大
                SkillManager.DuringCalDamage(this, victim, null, ref damage, "", false);
        }

        if (damage > 0)
        {
            victim.hp -= damage;
            if (victim != this)
                victim.lastDamagedPlayerId = playerId;
            // 记录战斗统计
            if (isHero)
                BattleStatManager.AddBattleStat(playerId, heroId, damage, true, victim.isHero);

            SkillManager.OnAttack(this, victim, damage);
        }

        if(!string.IsNullOrEmpty(effect))
            EffectManager.PlayHitEffect(this, victim, effect);
        victim.OnHpChanged();
    }

    public void OnSkillDamaged(Chess caster, int skillId, int damage, bool isFeedback = false, string hurtTag = "")
    {
        if(damage <= 0)
            throw new Exception("伤害值不能小于等于0");

        // 抗性减免（英雄与士兵统一结算，参考金铲铲）：IsMagic=true 法术受魔抗减免；false 物理(atk)受护甲减免
        var skillCfg = SkillConfig.GetConfig(skillId);
        if (skillCfg != null)
        {
            if (skillCfg.IsMagic)
                damage = Math.Max(1, (int)(damage * CombatConst.ResistMultiplier(magicRes))); // 法术：魔抗减免
            else
                damage = Math.Max(1, (int)(damage * CombatConst.ResistMultiplier(GetEffectiveArmor(caster)))); // 物理(atk)：等效护甲减免（攻方破甲可无视守方护甲）
        }

        // 伤害计算阶段·统一入口：普攻与技能伤害都进入，调整伤害基数与倍率（增伤/减伤/破甲/连锁等；跳过当前施放技能自身）
        var effect = "";
        var damageBase = damage;
        var damageMulti = 1f;
        SkillManager.BeforeCalDamaged(caster, this, skillCfg, ref damageBase, ref damageMulti, ref effect, hurtTag, isFeedback);
        damage = (int)(damageBase * damageMulti);
        if (damage <= 0)
            return; // 减伤把伤害压到0，不再结算（护盾无需吸收）

        // 伤害结算阶段·受击方：只做伤害吸收（护盾），不做伤害放大
        SkillManager.DuringCalDamage(caster, this, skillCfg, ref damage, hurtTag, isFeedback);

        if(hp <= 0)
            return;

        hp -= damage;
        if(caster != this)
            lastDamagedPlayerId = caster.playerId;

        // 记录战斗统计
        if(caster.isHero)
            BattleStatManager.AddBattleStat(caster.playerId, caster.heroId, damage, false, isHero);            

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
        SkillManager.OnDeath(this);
        buffs.Clear();
        WorldManager.Instance.OnUnitDying(this, lastDamagedPlayerId);

        Destroy(gameObject);

        if ((side == 1 || side == 2 && !isShadow ))
            GameManager.Instance.PlaySound("Sounds/tnt", 7);
    }


    /// <summary>
    /// 物理伤害结算时受击方的等效护甲：原始护甲 × (1 + Σ攻击方破甲增量 + Σ受击方加甲增量)，多技能按加法叠加
    /// </summary>
    private int GetEffectiveArmor(Chess attacker)
    {
        var delta = 0f;
        foreach (var s in attacker.skills)
            delta += s.GetArmorDelta(true);
        foreach (var s in skills)
            delta += s.GetArmorDelta(false);
        var rate = Mathf.Max(0f, 1f + delta);
        return (int)(armor * rate);
    }

    public void AddHp(int addon)
    {
        if(addon <= 0)
            throw new Exception("添加的血量不能小于等于0");

        hp = Mathf.Clamp(hp + addon, 0, maxHp);
        OnHpChanged();
    }

    public void HealTarget(Chess target, int checkSkillId, int addon)
    {
        SkillManager.OnHealTarget(this, target, checkSkillId, ref addon);
        // 治疗强化系数（治疗者）与受治疗系数（目标，可为负=减疗）
        addon = Mathf.RoundToInt(addon * (1f + healRate + target.healedRate));
        if (addon > 0)
            target.AddHp(addon);
    }

    public void Cooldown(float time)
    {
        // 冷却进度按百分比填充（1=完全冷却），最大不超过1
        attackPoint = Mathf.Min(attackPoint + time, 1f);
    }

    public void SetLifeTime(float time)
    {
        dieAfterLifeTime = true;
        lifeTime = time;
    }

    public PlayerInfo GetPlayerInfo()
    {
        // playerId=999为PVE怪物(虚拟玩家，无PlayerInfo实体)
        if (playerId < 0 || playerId >= GameManager.Instance.players.Length)
            return null;
        return GameManager.Instance.GetPlayer(playerId);
    }

    public bool IsInFight()
    {
        return Time.time < lastAttackTime + 0.3f;
    }

    public void AddBuff(Buff buff, Chess caster, float time)
    {
        float buffCount = 0;
        var nowTime = Time.time;
        buffTimes.RemoveAll(buff => nowTime - buff.time > 30);
        foreach (var existingBuffTime in buffTimes)
        {
            if (existingBuffTime.id == buff.id)
                buffCount++;
        }
        if(buffCount >= 3)
        {
            time = Math.Max(.1f, time * (10 - buffCount) * .1f);
            buff.SetTime(time);
        }

        // 保留原有的buff刷新逻辑
        foreach(var item in buffs)
        {
            if(item.id == buff.id)
            {
                item.Refresh(caster, time);
                return;
            }
        }

        buffs.Add(buff);
        buff.OnAdd(this, caster);
        buffTimes.Add(new BuffTime{id = buff.id, time = Time.time});
    }

    // buff 移除事件分发：参考技能体系，虚方法派发到本单位的各技能（技能覆写 OnBuffRemoved 响应，如天人守城护盾破爆炸）
    public virtual void OnBuffRemoved(Buff buff)
    {
        foreach (var skill in skills)
            skill.OnBuffRemoved(this, buff);
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
            GameLog.Debug("ColorLerpCoroutine " + color + " start=" + start + " end=" + end);

            material.SetColor("_Color", color);
            time += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);

        }
    }

    public int GetAttr(string attr)
    {
        switch (attr)
        {
            case "ap":
                return ap;
            case "atk":
            case "might": // 无双已并入攻击：老配置(未同步源表的 might 键)兼容为 atk
                return atk;
            case "hp":
                return hp;
            case "hprate":
                return (int)(HpRate * 100f);
            default:
                throw new ArgumentException("Invalid attribute name: " + attr);
        }
    }

    public int GetAttrTotal()
    {
        return ap + atk;
    }

    public void AddAttr(string attr, int value)
    {
        switch (attr)
        {
            case "ap":
                ap += value;
                break;
            case "atk":
            case "might": // 无双已并入攻击：老配置(未同步源表的 might 键)兼容为加攻击
                atk += value;
                break;
        }
        if(heroInfo != null)
            heroInfo.SetAttr(ap, atk);
    }

    public float HpRate{ get { return (float)hp / maxHp; } }

    public bool HasBuff(int id)
    {
        // Use Exists method since buffs is a List<Buff>
        return buffs.Exists(buff => buff.id == id);
    }

    public Buff GetBuff(int id)
    {
        return buffs.Find(buff => buff.id == id);
    }

    public bool MoveTo(Vector3 targetPosition, bool isForce = false)
    {
        return WorldManager.Instance.MoveTo(this, targetPosition, isForce);
    }

    private Coroutine jumpCoroutine = null;

    public void PlayerAnim(string name)
    {
        if(string.IsNullOrEmpty(name))
            return;
        var animator = GetComponent<Animator>();
        if(animator == null)
            return;
        animator.Play(name);
    }

    public void StartJump(float time)
    {
        var height = 15;
        GameLog.Debug("StartJump " + height + " " + id + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        // 如果已经在跳跃，先打断当前跳跃
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
            transform.position = new Vector3(transform.position.x, baseY + heightOffset, transform.position.z); // 恢复到原始位置
        }
        
        jumpCoroutine = StartCoroutine(JumpCoroutine(height, time));
    }

    public void StopJump()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
            transform.position = new Vector3(transform.position.x, baseY + heightOffset, transform.position.z); // 恢复到原始位置
        }
    }

    IEnumerator JumpCoroutine(int jumpHeight, float jumpDuration)
    {
        float elapsedTime = 0f;
        
        Vector3 originalPosition = transform.position;
        while (elapsedTime < jumpDuration)
        {
            float progress = elapsedTime / jumpDuration;
            
            // 使用抛物线运动：y = 4h * (x - x²) 其中h是最大高度
            float height = 4f * jumpHeight * (progress - progress * progress) + 7;
            
            // 更新位置
            Vector3 newPosition = originalPosition;
            newPosition.y += height;
            transform.position = Vector3.Lerp(originalPosition, newPosition, progress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 确保最终回到原始位置
        transform.position = new Vector3(transform.position.x, baseY + heightOffset, transform.position.z);
        jumpCoroutine = null;
    }

    public void AddSkill(int skillId, int parentSkillId, int level = 0)
    {
        if(skills.Find(skill => skill.id == skillId || skill.id == parentSkillId) != null)
            return;

        var skillAdd = SkillManager.CreateSkill(skillId, this);
        if (level > 0)
            skillAdd.SetLevel(level);
        skills.Add(skillAdd);
    }


}
