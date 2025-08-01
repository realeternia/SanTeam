using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chess : MonoBehaviour, IPointerClickHandler
{
    public int id;
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

    private Vector3? moveDirection = null;
    // 移动失败计数器
    private int moveFailCount = 0;
    // 最大连续移动尝试次数

    // 是否正在使用偏移路径
    public int hp = 100;
    public int attackDamage = 30;

    // 攻击冷却时间
    private float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    private float lastTargetUpdateTime = 0f; // 上次更新目标的时间

    public HeroInfo heroInfo;

    // Start is called before the first frame update
    void Start()
    {
        // 创建材质实例
        Material newMaterial = new Material(rend.sharedMaterial);
        newMaterial.mainTexture = Resources.Load<Texture>("Skins/" + chessName);
        newMaterial.SetColor("_OutlineColor", side == 1 ? Color.green : Color.blue);
        rend.material = newMaterial; // 这会为这个渲染器创建一个独立的材质实例

        // 初始化HP
        hp = maxHp;

        // 创建HUD
        CreateHUD();

        StartCoroutine(MoveAndFightCoroutine());
    }

    // 创建血条HUD
    private void CreateHUD()
    {
        // 查找或创建Canvas
        canvas = FindObjectOfType<Canvas>();

        // 加载Hud预制体
        GameObject hudPrefab = Resources.Load<GameObject>( isHero ? "Prefabs/Hud" : "Prefabs/HudSmall");

        // 实例化HUD对象
        GameObject hudObj = Instantiate(hudPrefab, canvas.transform);
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
        // 获取所有Chess组件
        Chess[] allChess = FindObjectsOfType<Chess>();
        float minDistance = Mathf.Infinity;
        targetChess = null;

        // 遍历寻找不同side的最近单位
        foreach (Chess chess in allChess)
        {
            if (chess.side != this.side && (chess.side + 1) / 2 == (side + 1) / 2)
            {
                float distance = Vector3.Distance(transform.position, chess.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetChess = chess;
                }
            }
        }
    }

    // Update is called once per frame
    private IEnumerator MoveAndFightCoroutine()
    {    
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            MoveAndFight();
            yield return new WaitForSeconds(0.05f);
        }
    }

    void MoveAndFight()
    {
        // 每2秒重新寻找目标
        if (Time.time - lastTargetUpdateTime >= 2f)
        {
            FindTarget();
            lastTargetUpdateTime = Time.time;
        }

        // 检查目标是否存在
        if (targetChess == null)
        {
            // 如果没有目标，尝试寻找新目标
            FindTarget();
            return;
        }
        // 检查目标是否在攻击范围内
        float distanceToTarget = Vector3.Distance(transform.position, targetChess.transform.position);

        if (distanceToTarget <= attackRange)
        {
            // 检查攻击冷却
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
            return;
        }

        if(moveDirection == null)
        {
            moveDirection = targetChess.transform.position;
        }

        //如果当前位置很接近moveDirection，就直接移动到moveDirection
        if (Vector3.Distance(transform.position, moveDirection.Value) <= moveSpeed * 0.1f)
        {
            moveDirection = targetChess.transform.position;
        }

        if (moveDirection != null)
        {
            // 计算下一步位置
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, moveDirection.Value, moveSpeed * 0.1f);

            // 尝试锁定目标格子
            if (WorldManager.Instance.TryLockGridPositions(this, nextPosition))
            {
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
                nextPosition = transform.position + newDirection * moveSpeed * 0.1f;

                // 尝试移动到新位置
                if (WorldManager.Instance.TryLockGridPositions(this, nextPosition))
                {
                    transform.position = nextPosition;
                    moveDirection = transform.position + newDirection * moveSpeed * 0.1f * 10;
                    moveFailCount = 0; // 重置失败计数器
                }
            }
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

    // 攻击目标
    private void Attack()
    {
        if (targetChess == null) 
            return;

        // 造成伤害
        targetChess.hp -= calculateDamage(this, targetChess);
        if(targetChess.heroInfo != null) // 英雄
            targetChess.heroInfo.SetHpRate((float)targetChess.hp / targetChess.maxHp);
        //Debug.Log($"{gameObject.name} 攻击了 {targetChess.gameObject.name}，造成 {this.attackDamage} 点伤害，目标剩余生命值：{targetChess.hp}");

        // 播放粒子特效
        var swordHitBluePrefab = Resources.Load<GameObject>(isHero ? "Prefabs/SwordHitYellowCritical" : "Prefabs/SwordHitBlue");
        GameObject hitEffect = Instantiate(swordHitBluePrefab, targetChess.transform.position, Quaternion.identity);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = targetChess.transform;
        hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);
        hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        Destroy(hitEffect, 2f);

        // 检查目标是否被击败
        if (targetChess.hp <= 0)
        {
            Debug.Log($"{targetChess.gameObject.name} 被击败了！");
            WorldManager.Instance.OnUnitDie(targetChess);
            Destroy(targetChess.gameObject);
            targetChess = null;

            // 寻找新目标
            FindTarget();
        }
    }

    private int calculateDamage(Chess attacker, Chess defender)
    {
        if(!attacker.isHero || !defender.isHero)
            return attacker.attackDamage;

        var attackerHeroCfg = HeroConfig.GetConfig((uint)attacker.heroId);
        var defenderHeroCfg = HeroConfig.GetConfig((uint)defender.heroId);

        // 计算攻击者三属性与防御者对应属性的差值
        float inteDiff = attackerHeroCfg.Inte - defenderHeroCfg.Inte;
        float leadShipDiff = attackerHeroCfg.LeadShip - defenderHeroCfg.LeadShip;
        float strDiff = attackerHeroCfg.Str - defenderHeroCfg.Str;

        // 找出最大差值
        float maxDiff = Mathf.Max(inteDiff, leadShipDiff, strDiff);

        // 伤害 = 最大差值 * 2
        int damage = Mathf.RoundToInt(maxDiff * 2);

        // 记录日志
        Debug.Log($"{attackerHeroCfg.Name}攻击{defenderHeroCfg.Name}，属性差值：Inte={inteDiff}, LeadShip={leadShipDiff}, Str={strDiff}，最大差值={maxDiff}，伤害：{damage}");

        // 限制伤害在10-60之间
        return Mathf.Clamp(damage, 10, 60);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        //rend.material.mainTexture = Resources.Load<Texture>("ChessPic/" + UnityEngine.Random.Range(1, 54).ToString());
    }
}
