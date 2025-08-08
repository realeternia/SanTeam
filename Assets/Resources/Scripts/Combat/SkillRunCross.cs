using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;    
// 添加协程命名空间
using System.Collections;

public class SkillRunCross : Skill
{
    public SkillRunCross(int id, Chess unit) : base(id, unit)

    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
       if(CheckBurst())
       {
            // 计算镜像位置
            Vector3 ownerPos = owner.transform.position;
            Vector3 defenderPos = defender.transform.position;
            
            // 计算镜像位置（以defender为中心）
            float mirrorX = 2 * defenderPos.x - ownerPos.x;
            float mirrorZ = 2 * defenderPos.z - ownerPos.z;
            Vector3 mirrorPos = new Vector3(mirrorX, ownerPos.y, mirrorZ);
            
            // 检查是否可以移动到镜像位置
            if (WorldManager.Instance.TryLockGridPositions(owner, mirrorPos))
            {
                // 启动协程移动
                owner.canMove = false;
                EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

                owner.StartCoroutine(JumpToPosition(mirrorPos));
            }
       }
    }
    
    // 跳跃移动协程
    private IEnumerator JumpToPosition(Vector3 targetPos)
    {
        Vector3 startPos = owner.transform.position;
        float jumpHeight = 10f; // 跳跃高度
        float moveDuration = 0.5f; // 移动持续时间
        float elapsedTime = 0f;
        
        while (elapsedTime < moveDuration)
        {
            // 计算插值因子
            float t = elapsedTime / moveDuration;
            
            // 计算当前位置（带跳跃效果）
            float yOffset = jumpHeight * Mathf.Sin(t * Mathf.PI);
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += yOffset;
            
            // 更新位置
            owner.transform.position = currentPos;
            
            // 等待下一帧
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 确保到达目标位置
        owner.transform.position = targetPos;
        owner.canMove = true;
    }

}
