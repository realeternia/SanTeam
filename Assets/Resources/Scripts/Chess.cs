using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chess : MonoBehaviour, IPointerClickHandler
{
    public int id;
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

        // 延迟一点时间后寻找目标，确保所有棋子都已初始化
        Invoke("FindTarget", 0.5f);
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
    private float attackCooldown = 1f;
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
            // 不在攻击范围内且正在移动，继续移动
            transform.position = Vector3.MoveTowards(transform.position, targetChess.transform.position, moveSpeed * Time.deltaTime);
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
