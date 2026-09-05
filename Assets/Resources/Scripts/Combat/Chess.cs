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


    // 目标单位
    public Chess targetChess;
    // 移动速度
    public float moveSpeed = 5f;
    public float attackRange = 10f;
    public int ap;      // 法术强度（原智力）
    public int might;   // 无双强度（原武力）
    public int atk;     // 攻击（原统帅）
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
    // 移动失败计数器
    private int moveFailCount = 0;
    // 最大连续移动尝试次数

    // 是否正在使用偏移路径
    public int hp = 100;
    public int attackDamage = 30;
    public float soldierAtkRate = 1f; // 士兵攻击加成系数（相的职业羁绊：全军士兵攻击+%）

    // 护甲/魔抗：英雄与士兵初始化时从配置赋值
    public int armor;
    public int magicRes;
    public string hitEffect;
    public int missileSpeed = 10;
    public float missileHight;
    public int soldierId;
    private int soldierLevel = 0;


    // 攻击冷却时间
    public float attackPoint;
    public float attackRate; //攻击频率（每秒攻击次数，=攻速值/30；攻速20=1.5秒/次，15=2秒/次）
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
    private HashSet<int> friendIds = new HashSet<int>(); //连线(武将关系)好友
    private int friendAtkBonus; //连线好友带来的攻击加成值

    private float secondTimer; //每秒事件计时，满1s触发一次OnSecond
    public int regeHp; //回复血量（OnSecond事件结算）
    public int hpRegen; //生命回复/秒（正=回复，负=扣减，OnSecond事件结算）
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
        // 创建材质实例
        material = new Material(rend.sharedMaterial);
        if (!string.IsNullOrEmpty(chessName))
        {
            if (chessName.StartsWith("PlayerPic") || chessName.StartsWith("MonsterPic"))
                material.mainTexture = Resources.Load<Texture>(chessName);
            else
                material.mainTexture = Resources.Load<Texture>("Skins/" + chessName);
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
            // 个人技能(Skill1/Skill2)等级 = 卡片等级；兵种技能为占位技能(Dumb)，职业被动加成由 JobLinkManager 按同职业英雄数直接施加；
            // 好友特殊技能由 FriendLineManager 按在场好友数计算。
            foreach (var skillCfg in ConfigManager.GetHeroSkillConfigs(heroCfg))
            {
                var skill = SkillManager.CreateSkill(skillCfg.Id, this);
                if (skillCfg.Sname != jobSkillSname && playerInfo != null && playerInfo.cards.TryGetValue(heroId, out int heroExp))
                    skill.SetLevel(HeroSelectionTool.GetCardLevel(heroExp, true));
                skills.Add(skill);
                if (!string.IsNullOrEmpty(skillCfg.Icon) && !hasSKill)
                {
                    material.SetTexture("_SecondTex", Resources.Load<Texture>("SkillPic/" + skillCfg.Icon));
                    hasSKill = true;
                }
            }

            materialFlag = new Material(rendFlag.sharedMaterial);
            materialFlag.mainTexture = Resources.Load<Texture>(playerInfo.imgPath);
            rendFlag.material = materialFlag;
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
                attackDamage += (int)((playerInfo.sodatk + playerInfo.GetItemPAttr("satk") + playerInfo.GetSoldierAtkAdd()) * soldierCfg.SoldierAtkRate);
            }
        }
        hp = maxHp;
        if (heroInfo != null) // 英雄
            heroInfo.SetHpRate(hp, maxHp);
        
        attackPoint = SysRandom.Range(0f, 1f); // 随机获得初始气力
        // attackRate 已在 SpawnUnitsForRegion/SpawnHerosForRegion 中按配置设置（攻速值/30），此处不能覆盖
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

    // 每秒事件：regeHp/hpRegen/mpRegen 等按秒结算的逻辑统一在此处理
    private void OnSecond()
    {
        if (regeHp > 0)
            AddHp(regeHp);

        if (hpRegen != 0)
        {
            // 生命回复属性：正=回复，负=扣减（可为负=持续扣减）
            hp = Mathf.Clamp(hp + hpRegen, 0, maxHp);
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

    public void CheckInitAttr(PlayerInfo player, int lv)
    {
        level = lv;

        var heroConfig = HeroConfig.GetConfig(heroId);
        var attr = HeroSelectionTool.GetCardAttr(player, heroId, lv);

        maxHp = attr.Hp;
        moveSpeed = heroConfig.MoveSpeed;
        attackRange = heroConfig.Range;
        attackRate = heroConfig.AtkSpeed / 30f; // 攻速值→每秒攻击次数（30=1次/秒；攻速20=1.5秒/次，15=2秒/次）
        attackDamage = attr.Atk;
        ap = attr.Ap;
        might = attr.Might;
        atk = attr.Atk;
        armor = heroConfig.Armor;
        magicRes = heroConfig.MagicRes;

        // 装备升级机制已移除：装备属性固定，不再按持有数量计算等级；最多3件装备属性累加
        if (player.itemEquips.TryGetValue(heroId, out var equipIds) && equipIds != null)
        {
            foreach (var equipId in equipIds)
            {
                if (equipId == 0)
                    continue;
                var equipAttr = HeroSelectionTool.GetCardAttr(player, equipId, 1);

                ap += equipAttr.Ap;
                might += equipAttr.Might;
                atk += equipAttr.Atk;
                maxHp += equipAttr.Hp;
            }
        }

        hp = maxHp;

        if (heroInfo != null)
            heroInfo.SetAttr(ap, might, atk);
    }

    // 记录连线(武将关系)好友
    public void AddFriendId(int friendId)
    {
        friendIds.Add(friendId);
    }

    // 应用连线(武将关系)攻击强化
    public void ApplyFriendAtkBonus(float rate)
    {
        if (rate <= 0)
            return;
        friendAtkBonus = (int)(atk * rate);
        atk += friendAtkBonus;
    }

    // 刷新英雄属性显示(连线加成在战斗开始时应用后调用)
    public void RefreshHeroAttr()
    {
        if (heroInfo != null)
            heroInfo.SetAttr(ap, might, atk);
    }

    public void UpdateAttr(int ap, int might, int atk)
    {
        if (ap > 0)
            this.ap = ap;
        if (might > 0)
            this.might = might;
        if (atk > 0)
            this.atk = atk;
        if (heroInfo != null)
            heroInfo.SetAttr(this.ap, this.might, this.atk);
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

        attackDamage += (int)(lv * atkAdd * soldierCfg.SoldierAtkRate);
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

        // 获取所有Chess组件
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
        float score = target.isHero ? 10 : 30;

        // 距离权重（距离越近分数越高）
    //    score += 100f / (distance + 1f);  // 避免除以0

        // 添加最大属性差作为积分项（权重可根据游戏平衡调整）
        score += calculateDamage(this, target, out var type) / 2;
        score += (level - target.level) * 7f;

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
            attackPoint += deltaTime * attackRate;
            // 检查攻击冷却（攻击频率累积满1次即可出手，attackRate=攻速值/30）
            if (attackPoint >= 1f)
            {
            //    PlayerAnim("jumpspin");
                attackPoint = 0;
                AddActionMp(); // 每次攻击行动为技能充能
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

        if (moveDest == null || WorldManager.Instance.GetRange(targetChess.transform.position, moveDest.Value) > 40)
            moveDest = targetChess.transform.position;
        
        //如果当前位置很接近moveDirection，就直接移动到moveDirection
        if (WorldManager.Instance.GetRange(transform.position, moveDest.Value) <= moveSpeed * 0.1f)
        {
            moveDest = targetChess.transform.position;
        }

        if (moveDest != null)
        {
            // 计算下一步位置
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, moveDest.Value, moveSpeed * 0.05f);

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
                angleOffset *= SysRandom.Value > 0.5f ? 1 : -1;

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
                    moveDest = transform.position + newDirection * moveSpeed * 0.05f * 10;
                    moveFailCount = 0; // 重置失败计数器
                }
            }
        }
    }

    // 攻击目标
    public void Attack(Chess victim, string hitEffectName)
    {
        if (victim == null)
            return;

        // 造成伤害
        var damage = calculateDamage(this, victim, out var damType);
        var effect = hitEffectName;
        var damageBase = damage;
        var damageMulti = 1f;
        var damageReal = 0; //真实伤害
        bool isCrit = false;

        SkillManager.DuringAttack(this, victim, damType, ref damageBase, ref damageMulti, ref damageReal, ref effect);
        // 暴击
        if (critRate > 0 && SysRandom.Value < critRate)
        {
            damageMulti += critDamageMulti;
            WorldManager.Instance.AddBattleText("暴!", transform.position, new UnityEngine.Vector2(0, 40), Color.red, 3);
            isCrit = true;
        }

        damage = (int)(damageBase * damageMulti);
        var minDamage = 10 + level / 2;
        var maxDamage = 50 + level;
        if (isHero && victim.isHero)
        {
            //等级压制
            var levelDiff = level - victim.level;
            if (levelDiff != 0)
            {
                minDamage = Math.Clamp(minDamage + levelDiff, 8, minDamage * 2);
                maxDamage = Math.Clamp(maxDamage + levelDiff * 4, 40, maxDamage * 2);
            }
        }
        if(isCrit)
        {
            minDamage = (int)(minDamage * (1 + critDamageMulti));
            maxDamage = (int)(maxDamage * (1 + critDamageMulti));
        }
        damage = Mathf.Clamp(damage, minDamage, maxDamage);
        if (damage > 0)
        {
            if (victim.dodgeRate > 0 && SysRandom.Value < victim.dodgeRate)
            {
                damage = 0;
                WorldManager.Instance.AddBattleText("闪!", victim.transform.position, new UnityEngine.Vector2(0, 40), Color.red, 3);
            }
            else
            {
                //这里不改数值，只能伤害吸收
                SkillManager.BeforeAttack(this, victim, ref damage);
            }
        }

        if (damage + damageReal > 0)
        {
            damage = Math.Max(damage, damageReal);

            victim.hp -= damage;
            if (victim != this)
                victim.lastDamagedPlayerId = playerId;
            // 记录战斗统计
            if (isHero)
                BattleStatManager.AddBattleStat(playerId, heroId, damage, true, victim.isHero);

            SkillManager.OnAttack(this, victim, damType, damage);
        }

        if(!string.IsNullOrEmpty(effect))
            EffectManager.PlayHitEffect(this, victim, effect);
        victim.OnHpChanged();
    }

    public void OnSkillDamaged(Chess caster, int skillId, int damage, bool isFeedback = false)
    {
        if(damage <= 0)
            throw new Exception("伤害值不能小于等于0");

        // 抗性减免（英雄与士兵统一结算，参考金铲铲）：法术强度(Ap)类技能受魔抗减免，无双(Might)类技能受护甲减免；普攻联动(Atk)类已在普攻阶段结算护甲
        var skillCfg = SkillConfig.GetConfig(skillId);
        if (skillCfg != null)
        {
            if (skillCfg.Attr == "ap")
                damage = Math.Max(1, (int)(damage * CombatConst.ResistMultiplier(magicRes)));
            else if (skillCfg.Attr == "might")
                damage = Math.Max(1, (int)(damage * CombatConst.ResistMultiplier(armor)));
        }

        if (isHero)
        {
            SkillManager.OnDoSkillDamage(this, caster, SkillConfig.GetConfig(skillId), ref damage, isFeedback);
        }
        else
        {
            damage = Math.Max(damage, caster.attackDamage);//防止对士兵伤害过大
        }

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
        if (!friendIds.Contains(friendId))
            return;

        friendIds.Remove(friendId);
        // 移除旧加成，并按剩余好友数量重算攻击强化
        atk -= friendAtkBonus;
        friendAtkBonus = (int)(atk * FriendLineManager.GetFriendLineAtkRate(friendIds.Count));
        atk += friendAtkBonus;

        if (heroInfo != null)
            heroInfo.SetAttr(ap, might, atk);
    }


    private int calculateDamage(Chess attacker, Chess defender, out string type)
    {
        type = "atk";

        // 攻击基准：英雄取攻击(Atk)；士兵取士兵攻击×加成系数（相的职业羁绊：全军士兵攻击+%）
        int damage;
        if (attacker.isHero)
            damage = attacker.atk;
        else
            damage = (int)(attacker.attackDamage * attacker.soldierAtkRate);

        // 普攻受目标护甲减免（英雄与士兵统一结算，参考金铲铲）：实际伤害 = 攻击 × 100/(100+护甲)
        damage = (int)(damage * CombatConst.ResistMultiplier(defender.armor));
        return Mathf.Max(1, damage);
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
        attackPoint += time;
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
                return atk;
            case "might":
                return might;
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
        return ap + atk + might;
    }

    public void AddAttr(string attr, int value)
    {
        switch (attr)
        {
            case "ap":
                ap += value;
                break;
            case "atk":
                atk += value;
                break;
            case "might":
                might += value;
                break;
        }
        if(heroInfo != null)
            heroInfo.SetAttr(ap, might, atk);
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
            transform.position = new Vector3(transform.position.x, 7, transform.position.z); // 恢复到原始位置
        }
        
        jumpCoroutine = StartCoroutine(JumpCoroutine(height, time));
    }

    public void StopJump()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
            transform.position = new Vector3(transform.position.x, 7, transform.position.z); // 恢复到原始位置
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
        transform.position = new Vector3(transform.position.x, 7, transform.position.z);
        jumpCoroutine = null;
    }

    public void AddSkill(int skillId, int parentSkillId)
    {
        if(skills.Find(skill => skill.id == skillId || skill.id == parentSkillId) != null)
            return;

        var skillAdd = SkillManager.CreateSkill(skillId, this);
        skillAdd.isGivenSkill = true;
        skills.Add(skillAdd);
    }

    // 每次攻击行动，为所有设置了MpCost的技能充能（3次攻击充满）
    public void AddActionMp()
    {
        foreach (var skill in skills)
        {
            skill.AddActionMp();
        }
    }


}
