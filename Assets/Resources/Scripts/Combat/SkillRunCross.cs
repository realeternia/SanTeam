using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;    
// 添加协程命名空间
using System.Collections;
using System;

public class SkillRunCross : Skill
{
    public SkillRunCross(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        var maxVal = Math.Max(owner.leadShip, owner.str);

        if(!WorldManager.Instance.CheckInRange(owner.transform.position, attacker.transform.position, skillCfg.Range * 2))
        {
            damageBase -= Math.Max(10, (int)(maxVal * skillCfg.Strength * 2));
            return;
        }
        if(!WorldManager.Instance.CheckInRange(owner.transform.position, attacker.transform.position, skillCfg.Range))
        {
            damageBase -= Math.Max(5, (int)(maxVal * skillCfg.Strength));
        }
    }


    public override void OnAttack(Chess defender, string damType, int damage)
    {
        // 计算镜像位置
        Vector3 ownerPos = owner.transform.position;
        Vector3 defenderPos = defender.transform.position;

        // 计算镜像位置（以defender为中心）
        float mirrorX = 2 * defenderPos.x - ownerPos.x;
        float mirrorZ = 2 * defenderPos.z - ownerPos.z;
        Vector3 mirrorPos = new Vector3(mirrorX, ownerPos.y, mirrorZ);

        // 检查是否可以移动到镜像位置
        if (WorldManager.Instance.TryLockGridPositions(owner, mirrorPos, out _) && CheckBurst())
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

            owner.transform.position = currentPos;

            // 等待下一帧
            elapsedTime += 0.025f;
            yield return new WaitForSeconds(0.025f);
        }
        
        // 确保到达目标位置
            
        owner.MoveTo(targetPos, true);
        owner.noMoveCount --;
    }

}
