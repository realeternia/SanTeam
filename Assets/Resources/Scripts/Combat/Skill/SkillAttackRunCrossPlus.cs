using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;    

public class SkillAttackRunCrossPlus : Skill
{
    public SkillAttackRunCrossPlus(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        // 计算镜像位置
        Vector3 ownerPos = owner.transform.position;
        Vector3 defenderPos = defender.transform.position;

        // 计算镜像位置（以defender为中心）
        float mirrorX = 3 * defenderPos.x - 2 * ownerPos.x;
        float mirrorZ = 3 * defenderPos.z - 2 * ownerPos.z;
        Vector3 mirrorPos = new Vector3(mirrorX, ownerPos.y, mirrorZ);

        // 检查是否可以移动到镜像位置
        if (WorldManager.Instance.TryLockGridPositions(owner, mirrorPos, out _) && CheckBurst(defender))
        {
            // 启动协程移动
            owner.noMoveCount++;
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

            owner.StartCoroutine(JumpToPosition(mirrorPos));
        }
    }

    // 跳跃移动协程
    private IEnumerator JumpToPosition(Vector3 targetPos)
    {
        Vector3 startPos = owner.transform.position;
        float moveDuration = .8f; // 移动持续时间
        float elapsedTime = 0f;
        
        // 计算移动方向
        Vector3 moveDirection = (targetPos - startPos).normalized;
        
        // 计算垂直于移动方向的两个方向（90度和-90度）
        Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * moveDirection;
        Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * moveDirection;
        
        List<int> pushedList = new List<int>();
        while (elapsedTime < moveDuration)
        {
            // 计算插值因子
            float t = elapsedTime / moveDuration;
            
            // 计算当前位置（带跳跃效果）
          //  float yOffset = jumpHeight * Mathf.Sin(t * Mathf.PI);
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
          //  currentPos.y += yOffset;

            var enmeyList = WorldManager.Instance.GetUnitsInRange(currentPos, 12, owner.side, true);
            foreach(var chess in enmeyList)
            {
                if(pushedList.Contains(chess.id))
                    continue;
                    
                // 计算敌人相对于移动直线的位置
                Vector3 enemyToLineVector = chess.transform.position - startPos;
                // 计算叉积来判断敌人在移动直线的哪一侧
                float crossProduct = Vector3.Cross(moveDirection, enemyToLineVector.normalized).y;
                
                // 根据叉积结果决定推动方向
                // 如果敌人在移动直线的左侧(crossProduct > 0)，则向左推
                // 如果敌人在移动直线的右侧(crossProduct < 0)，则向右推
                Vector3 pushDirection = (crossProduct > 0) ? leftDirection : rightDirection;
                
                // 计算推送后的位置
                chess.MoveTo(chess.transform.position + pushDirection * 15f, true);
                pushedList.Add(chess.id);

                BuffManager.AddBuff(chess, owner, id, skillCfg.BuffId, skillCfg.BuffTime); //加负面buff
            }

            owner.transform.position = currentPos;
            
            // 等待下一帧
            elapsedTime += 0.025f;
            yield return new WaitForSeconds(0.025f);
        }
        
        // 确保到达目标位置
        owner.MoveTo(targetPos, true);
        owner.noMoveCount --;

        owner.FindTarget(); //重新锁定一次
    }

}
