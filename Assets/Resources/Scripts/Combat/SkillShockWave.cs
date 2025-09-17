using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillShockWave : Skill
{
    private Vector3 targetPos;
    public SkillShockWave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.targetChess == null)
            return false;

        if(!WorldManager.Instance.CheckInRange(owner.transform.position, owner.targetChess.transform.position, skillCfg.Range))
            return false;

        if(!CheckBurst())
            return false;

        this.targetPos = owner.targetChess.transform.position; // 使用目标位置而不是自身位置
       // var magicStub = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501001, owner.transform.position, owner.side, "");
      //  magicStub.SetLifeTime(skillCfg.SummonTime);

     //   var hitEffect = EffectManager.PlayPosSkillEffect(magicStub, owner.transform.position, skillCfg.EffectSize, skillCfg.HitEffect, skillCfg.SummonTime);
     //   owner.StartCoroutine(MoveHitEffectToTarget(hitEffect, skillCfg.SummonTime, skillCfg.SummonSpeed)); // 启动新协程让特效飞向目标
        WorldManager.Instance.CreateSpellMissile(owner, targetPos, skillCfg.SummonTime, skillCfg.SummonSpeed, skillCfg.EffectSize, skillCfg.Id, skillCfg.HitEffect);

        return true;
    }

   

}
