using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 单体伤害体系基类（ScriptName = "SkillComDamageSingle"，模板「单体伤害大/中/小」）：
/// 锁定射程内的一名敌人，结算 GetSkillDamage() 技能伤害，并可选附加 SkillConfig.BuffId 指定的 Buff。
///
/// 本类是所有「单体伤害」脚本的基类，CheckAidSkill 固定按「选人 → 发动判定 → 施法表现 → 伤害结算 → 命中后处理」
/// 五步走，派生类只覆写需要的钩子（其余步骤自动继承整条通用体系）：
/// · SelectTarget()  选目标（默认 SelectSingleTarget(skillCfg.Range)，可覆写为 Area 选人或别的索敌规则）
/// · CanCast(target) 发动判定（默认 CheckBurst，可覆写追加"正在结算中"等阻止条件）
/// · PlayCast(target) 施法表现（默认动作 + SelfEffect，可覆写追加位移/自身 Buff 等）
/// · CalcDamage(target) 伤害计算（默认 ApplyDamageModifiers，含 execute/shieldBonus/missingHp/vsJob/soldierDamage）
/// · ResolveDamage(target, baseDamage) 统一伤害路径（修正 → 双击 → 直伤/弹道 → 破盾/燃蓝/击退/驱散），
///   单体 / 群体 / 跳跃链 / 持续伤害场全部共用；冲击波在导弹命中回调里补同一组命中后效果
/// · ResolveHit(target) 伤害结算（默认 ResolveDamage + AfterDamage，导弹/多段等可整段覆写，仍可复用 CalcDamage / ApplyDamage）
/// · ApplyDamage(target, dmg, baseDamage) 伤害落地（默认直伤 / missile 标记走飞弹，并播命中特效）
/// · AfterDamage(target, dmg) 命中后的形态补充处理（默认苦肉 + Buff 归属；破盾/燃蓝/击退/驱散已在 ResolveDamage）
///
/// 通用组件效果全部由基类统一实现（TargetType 标记 / Strength2 键），派生类不要重复判断：
/// · self        = 自身中心（近战贴脸 AoE），沿用当前索敌目标、不做射程校验（SelectTarget）
/// · missile     = 以飞弹逐个结算（ApplyDamage）；飞弹特效取 HitEffect，飞行速度取 SummonSpeed（0 时用单位导弹速度），
///                 破盾/燃蓝/击退/驱散与 Buff 归属统一延后到飞弹命中回调，避免飞弹未落地就生效
/// · execute / shieldBonus / missingHp / highHp / vsJob / soldierDamage / critRateBonus（ApplyDamageModifiers）
/// · antiShield / mpBurn / 击退 pushMin~pushMax / 拖回出生点 pullToSpawn / selfHurt / selfBuff / dispel（AfterDamage）
/// · hitCount / hitInterval = 多段攻击：第1段立即结算，第2段起每隔 hitInterval 秒追加命中（默认1段、间隔0.3秒）
/// 伤害体系派生（各自覆写 CheckAidSkill，自动继承上述全部组件能力）：
/// · SkillComDamageArea        范围伤害 + 可选 Buff（目标中心 / self 自身中心）
/// · SkillComDamageShockWave   冲击波（导弹沿路径命中）
/// · SkillComDamageJumpChain   跳跃弹射（逐跳衰减）
/// · SkillComDamageHitRegion   落点持续伤害场
/// 持续伤害（dot）不在本脚本结算：BuffId 挂 DoT Buff（如"败"），每秒跳伤由 BuffTimeDamage 读 StrengthBuff1 的 dot。
/// </summary>
public class SkillComDamageSingle : Skill
{
    /// <summary>多段攻击未配置 hitInterval 时的默认间隔（秒）</summary>
    private const float DefaultHitInterval = 0.3f;

    public SkillComDamageSingle(int id, Chess unit) : base(id, unit)
    {
    }

    public override float GetArmorDelta(bool isAttackerSide)
    {
        // 破甲：自身作为攻击方时物理攻击完全无视目标护甲
        return HasSkillFlag("armorIgnore") && isAttackerSide ? -1f : 0f;
    }

    public override bool CheckAidSkill()
    {
        var target = SelectTarget();
        if (target == null)
            return false;
        if (!CanCast(target))
            return false;

        PlayCast(target);
        ResolveHit(target);

        // 多段攻击：hitCount>1 时按 hitInterval 秒间隔追加命中（默认1段，保持原有单体技能行为）
        var hitCount = Mathf.Max(1, (int)GetStr2("hitCount", 1f));
        if (hitCount > 1)
            owner.StartCoroutine(ExtraHits(target, hitCount, Mathf.Max(0f, GetStr2("hitInterval", DefaultHitInterval))));
        return true;
    }

    /// <summary>多段攻击：第2段起每隔 interval 秒再次结算 ResolveHit；目标死亡时重新索敌。</summary>
    private IEnumerator ExtraHits(Chess target, int hitCount, float interval)
    {
        for (var i = 1; i < hitCount; i++)
        {
            if (interval > 0f)
                yield return new WaitForSeconds(interval);

            if (owner == null || owner.hp <= 0)
                yield break;

            // 延迟期间原目标死亡时重新索敌
            if (target == null || target.hp <= 0)
            {
                owner.FindTarget();
                target = owner.targetChess;
            }
            if (target == null || target.hp <= 0)
                yield break;

            ResolveHit(target);
        }
    }

    /// <summary>选目标：self 标记沿用当前索敌目标（自身中心 AoE），否则按射程校验（配 SelectFirst 时走优先级链）。</summary>
    protected virtual Chess SelectTarget()
    {
        var target = HasSkillFlag("self") ? owner.targetChess : SelectSingleTarget(skillCfg.Range);
        return target != null && target.hp > 0 ? target : null;
    }

    /// <summary>发动判定：默认 CheckBurst（MP/概率/CD/触发条件）。</summary>
    protected virtual bool CanCast(Chess target)
    {
        return CheckBurst(target);
    }

    /// <summary>施法表现：默认施法动作 + 自身特效。</summary>
    protected virtual void PlayCast(Chess target)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
    }

    /// <summary>本次施放的中心点：self 标记取自身位置（近战贴脸 AoE），否则取目标位置。</summary>
    protected Vector3 GetCastCenter(Chess target)
    {
        return HasSkillFlag("self") ? owner.transform.position : target.transform.position;
    }

    /// <summary>伤害计算：基础伤害 → 最大生命追加 → 全部通用伤害修正组件。</summary>
    protected virtual int CalcDamage(Chess target)
    {
        return CalcDamage(target, GetSkillDamage());
    }

    /// <summary>按指定基础值计算伤害（跳跃链逐跳衰减、弹道逐目标修正等复用）。</summary>
    protected virtual int CalcDamage(Chess target, int baseDamage)
    {
        var dmg = baseDamage;
        return ApplyDamageModifiers(dmg, target);
    }

    /// <summary>
    /// 中心双倍：centerDouble 标记下，锁定的主目标基础伤害 × centerRate（缺省 2 倍），其余目标不变。
    /// 统一在伤害体系基类读取标记，群体 / 冲击波等形态直接复用，子类不再自行判断。
    /// </summary>
    protected int ApplyCenterDouble(int baseDamage, bool isCenterTarget)
    {
        if (!isCenterTarget || !HasSkillFlag("centerDouble"))
            return baseDamage;
        return Mathf.RoundToInt(baseDamage * GetStr2("centerRate", 2f));
    }

    /// <summary>centerDouble 时确保锁定目标在命中列表内（范围选人可能被 TargetCount 截断）。</summary>
    protected void EnsureCenterDoubleTarget(List<Chess> units, Chess target)
    {
        if (!HasSkillFlag("centerDouble") || target == null || target.hp <= 0)
            return;
        if (!units.Contains(target))
            units.Add(target);
    }

    /// <summary>
    /// 统一伤害结算（单体 / 群体 / 跳跃链 / 持续伤害场共用）：
    /// 基础值 → CalcDamage 修正 → 概率双击 → 落地（直伤或弹道，命中特效随之播放）
    /// → 破盾追加 / 燃烧法力 / 击退 / 驱散。
    /// 冲击波由导弹自身按 CalcDamage 倍率结算，命中回调里补同一组“命中后”效果；
    /// 每施放一次的自伤（苦肉）与 Buff 挂载由调用方处理（单体在 AfterDamage，跳跃链逐跳挂 Buff）。
    /// </summary>
    protected int ResolveDamage(Chess target, int baseDamage)
    {
        var dmg = CalcDamage(target, baseDamage);
        var isDouble = TryDoubleDamage(ref dmg);
        // 拖回出生点：先位移再结算伤害，保证目标被拖走（死亡也落在出生点）
        PullTargetToSpawn(target);
        ApplyDamage(target, dmg, baseDamage);
        if (isDouble)
            WorldManager.Instance.AddBattleText("暴击", target.transform.position,
                new Vector2(0, 80), SysColor.BattleText.SkillName, 3);
        // 导弹型：命中后效果已随飞弹延后到 ApplyMissileImpact，施法瞬间不再重复结算
        if (!IsMissileDeferred(dmg))
            ApplyHitFollowUps(target, baseDamage);
        return dmg;
    }

    /// <summary>本次命中是否走飞弹延迟结算（missile 标记 + 有伤害），供派生类判断是否需要立即挂 Buff。</summary>
    protected bool IsMissileDeferred(int dmg)
    {
        return HasSkillFlag("missile") && dmg > 0;
    }

    /// <summary>命中后效果：破盾追加 / 燃烧法力 / 击退 / 驱散（直伤落地即执行，导弹型在命中回调执行）。</summary>
    private void ApplyHitFollowUps(Chess target, int baseDamage)
    {
        ApplyAntiShieldBonus(target, baseDamage);
        ApplyMpBurn(target);
        PushTargetBack(target);
        ApplyDispel(target);
    }

    /// <summary>飞弹命中回调：补做原本施法瞬间执行的命中后结算与 Buff 归属，保证效果与弹道同步。</summary>
    private void ApplyMissileImpact(Chess target, int baseDamage)
    {
        ApplyHitFollowUps(target, baseDamage);
        ApplySkillBuffs(target);
        // 导弹型主伤害落地后再折射（同一次施放只折射一次）
        TryRefract(target, target.transform.position, GetSkillDamage());
    }

    /// <summary>伤害结算主流程：统一伤害路径 + 命中后的自伤与 Buff 归属。</summary>
    protected virtual void ResolveHit(Chess target)
    {
        var dmg = ResolveDamage(target, GetSkillDamage());
        // 直伤：主伤害落地后触发折射（导弹型延后到命中回调）
        if (!IsMissileDeferred(dmg))
            TryRefract(target, target.transform.position, GetSkillDamage());
        AfterDamage(target, dmg);
    }

    /// <summary>伤害落地：missile 标记走飞弹（命中回调补命中后效果与 Buff），否则直接结算；命中特效统一在此播放。</summary>
    protected virtual void ApplyDamage(Chess target, int dmg, int baseDamage)
    {
        if (HasSkillFlag("missile"))
        {
            if (dmg > 0)
                WorldManager.Instance.CreateSpellMissile(owner, target, owner.transform.position, id, dmg, skillCfg.HitEffect, skillCfg.SummonSpeed,
                    hit => ApplyMissileImpact(hit, baseDamage), effectTint);
            return;
        }

        if (dmg > 0)
            target.OnSkillDamaged(owner, id, dmg);
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            PlaySkillEffect(target, skillCfg.HitEffect);
    }

    /// <summary>
    /// 命中后的形态补充处理：苦肉反噬 + Buff 归属。
    /// 破盾 / 燃烧法力 / 击退 / 驱散已收敛到 ResolveDamage，所有伤害形态共用。
    /// 导弹型的 Buff 归属已延后到飞弹命中回调，施法瞬间不再挂载。
    /// </summary>
    protected virtual void AfterDamage(Chess target, int dmg)
    {
        HurtSelfByDamage(dmg);
        if (!IsMissileDeferred(dmg))
            ApplySkillBuffs(target);
    }
}
