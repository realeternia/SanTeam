using System;
using System.Collections;
using System.Collections.Generic;
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
    public string chessName = "0";
    public Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        // 创建材质实例
        Material newMaterial = new Material(rend.sharedMaterial);
        newMaterial.mainTexture = Resources.Load<Texture>("Skins/" + chessName);
        rend.material = newMaterial; // 这会为这个渲染器创建一个独立的材质实例

        // 初始化HP
        hp = maxHp;

        // 创建HUD
        CreateHUD();

        // 延迟一点时间后寻找目标，确保所有棋子都已初始化
        Invoke("FindTarget", 0.5f);
    }

    // 创建血条HUD
    private void CreateHUD()
    {
        // 查找或创建Canvas
        canvas = FindObjectOfType<Canvas>();

        // 加载Hud预制体
        GameObject hudPrefab = Resources.Load<GameObject>("Prefabs/Hud");
        if (hudPrefab == null)
        {
            Debug.LogError("Hud.prefab not found in Resources/Prefabs");
            return;
        }

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

        // 遍历寻找不同side的单位
        foreach (Chess chess in allChess)
        {
            if (chess.side != this.side)
            {
                targetChess = chess;
                isMoving = true;
                break;
            }
        }
    }

    public void UpdateVal(int cardId)
    {
        id = cardId;
        // if(cardId > 0)
        //     chessName = CardBook.GetCard(id).icon.ToString();
        // else
        //     chessName = "0";
        rend.material.mainTexture = Resources.Load<Texture>("Skins/" + chessName);
    }

    // 目标单位
    private Chess targetChess;
    // 移动速度
    public float moveSpeed = 5f;
    public float attackRange = 10f;

    public int hp = 100;
    public int atk = 30;
    // 是否正在移动
    private bool isMoving = false;

    // 攻击冷却时间
    private float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    // Update is called once per frame
    void Update()
    {
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
            // 在攻击范围内，停止移动并尝试攻击
            isMoving = false;

            // 检查攻击冷却
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else if (isMoving)
        {
            // 计算下一步位置
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetChess.transform.position, moveSpeed * Time.deltaTime);

            // 尝试锁定目标格子
            if (WorldManager.Instance.TryLockGridPositions(this, nextPosition))
            {
                // 锁定成功，移动到新位置
                transform.position = nextPosition;
            }
            else
            {
                // 锁定失败，停止移动
                isMoving = false;
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
        if (targetChess == null) return;

        // 造成伤害
        targetChess.hp -= this.atk;
        Debug.Log($"{gameObject.name} 攻击了 {targetChess.gameObject.name}，造成 {this.atk} 点伤害，目标剩余生命值：{targetChess.hp}");

        // 检查目标是否被击败
        if (targetChess.hp <= 0)
        {
            Debug.Log($"{targetChess.gameObject.name} 被击败了！");
            Destroy(targetChess.gameObject);
            targetChess = null;

            // 寻找新目标
            FindTarget();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        //rend.material.mainTexture = Resources.Load<Texture>("ChessPic/" + UnityEngine.Random.Range(1, 54).ToString());
    }
}
