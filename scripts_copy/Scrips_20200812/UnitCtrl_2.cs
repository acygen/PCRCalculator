using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PCRCaculator.Battle
{
    public partial class UnitCtrl : MonoBehaviour
    {
        private static readonly int[] abnormalStateSwitch = new int[]
        {
            0,1,2,1,2,//0-1-2-3-4
            3,3,7,4,5,//5-6-7-8-9
            6,7,9,8,12,//10-11-12-13-14
            14,13,21,20,10,//15-16-17-18-19
            11,24,17,18,19,//20-21-22-23-24
            22,23,16,15,25,//25-26-27-28-29
            26,27,28,29,30,//30-31-32-33-34
            31,32,33,34,35,//35-36-37-38-39
            36,37,38,39,40,//40-41-42-43-44
            41,42,43,0//45-46-47
        };
        public static eAbnormalStateCategory GetAbnormalStateCategory(eAbnormalState abnormalState)
        {
            return (eAbnormalStateCategory)abnormalStateSwitch[(int)abnormalState];
        }
        internal int SetDamage(DamageData damageData, bool byAttack, int actionId, ActionParameter.OnDamageHitDelegate onDamageHit, bool hasEffect, Skill skill, bool energyAdd, Action onDefeat, bool noMotion, float weight, float actionWeightSum, Func<int, float, int> p)
        {
            float randomFloat =((float)staticBattleManager.Random())/100.0f;
            bool critical = false;
            if (randomFloat <= damageData.CriticalRate && damageData.CriticalRate != 0)
            {
                critical = true;
            }
            if (damageData.ActionType == eActionType.ATTACK)
            {
                eDamageType damageType = damageData.DamageType;
                if ((damageType == eDamageType.ATK && IsAbnormalState(eAbnormalState.LOG_ATK_BARRIR)) ||
                   (damageType == eDamageType.MGC && IsAbnormalState(eAbnormalState.LOG_MGC_BARRIR)) ||
                   IsAbnormalState(eAbnormalState.LOG_ALL_BARRIR))
                {
                    critical = damageData.IsLogBarrierCritical;
                }
            }
            int damage = SetDamageImpl(damageData, byAttack, onDamageHit, hasEffect, skill, energyAdd, critical, onDefeat, noMotion, p);
            //boss相关代码鸽了
            if (damageData.Source != null)
            {
                if (damageData.ActionType != eActionType.DESTROY)
                {
                    if (damageData.ActionType != eActionType.ATTACK_FIELD && (skill == null || skill.IsLifeStealEnabled))
                    {
                        int lifesteal = damageData.LifeSteal;
                        if (skill != null)
                        {
                            lifesteal += skill.LifeSteal;
                        }
                        if (lifesteal >= 1)
                        {
                            float recovery_value = lifesteal * damage / (lifesteal + UnitData.level + 100);
                            if (recovery_value > 0)
                            {
                                eInhibitHealType eh = damageData.DamageType == eDamageType.MGC ? eInhibitHealType.MAGIC : eInhibitHealType.PHYSICS;
                                SetRecovery((int)recovery_value, eh, damageData.Source, false, false,
                                    false,//鸽了
                                    false, true, null);
                            }
                        }
                    }
                }
                if (IsOther ^ damageData.Source.IsOther)
                {
                    UnitCtrl source = damageData.Source;
                    if (damageData.Source.SummonType != eSummonType.NONE)
                    {
                        source = source.SummonSource;
                    }
                    //伤害统计
                    if (source != null)
                    {
                        //source.UnitDamageinfo.Setdamage(damage + 原伤害)
                    }
                }
            }
            accumulateDamage += damage;
            //log
            if (skill != null)
            {
                if (damageData.Source != null)
                {
                    if (skill.SkillId == damageData.Source.UBSkillId)
                    {
                        if (byAttack)
                        {
                            damageData.Target.PassiveUbIsMagic = damageData.DamageType == eDamageType.MGC;
                        }
                        damageData.Target.TotalDamage += damage;
                    }
                }
                if (damage >= 1)
                {
                    if (!skill.DamageedPartsList.Contains(damageData.Target))
                    {
                        skill.DamageedPartsList.Add(damageData.Target);
                    }
                }
            }
            if (damageData.Source != null)
            {
                if (damage >= 1 && DamageSealDataDictionary.ContainsKey(damageData.Source))
                {
                    Dictionary<int, AttackSealData> dic = DamageSealDataDictionary[damageData.Source];
                    foreach (AttackSealData sealData in dic.Values)
                    {
                        if (!sealData.OnlyCritical || critical)
                        {
                            sealData.AddSeal(this);
                        }
                    }
                }
                if (damage >= 1 && damageData.Source.DamageOnceOwnerSealDateDictionary.ContainsKey(damageData.Source) && skill != null)
                {
                    if (!skill.AlreadyAddAttackSelfSeal)
                    {
                        Dictionary<int, AttackSealData> dic2 = DamageOnceOwnerSealDateDictionary[damageData.Source];
                        foreach (AttackSealData sealData in dic2.Values)
                        {
                            sealData.AddSeal(damageData.Source);
                        }
                        skill.AlreadyAddAttackSelfSeal = true;
                    }
                }
            }
            if (damageData.Source == null || damage < 1 || !damageData.Source.DamageOwnerSealDataDictionary.ContainsKey(damageData.Source))
            {
                if (skill == null) { return damage; }
                skill.TotalDamage += damage;
                return damage;
            }
            foreach (AttackSealData sealData1 in damageData.Source.DamageOwnerSealDataDictionary[damageData.Source].Values)
            {
                if (sealData1.OnlyCritical)
                {
                    if (critical)
                    {
                        sealData1.AddSeal(damageData.Source);
                    }
                }
                else
                {
                    sealData1.AddSeal(damageData.Source);
                }
            }
            if (skill != null)
            {
                skill.TotalDamage += damage;
            }
            return damage;
        }
        internal int SetDamageImpl(DamageData damageData, bool byAttack, ActionParameter.OnDamageHitDelegate onDamageHit, bool hasEffect, Skill skill, bool energyAdd, bool critical, Action onDefeat, bool noMotion, Func<int, float, int> p)
        {
            if (IdleOnly) { return 0; }
            if (IsDivisionSourceForDamage)
            {
                if (!damageData.IsDivisionDamage)
                {
                    return 0;
                }
            }
            //boss
            if (IsAbnormalState(eAbnormalState.NO_DAMAGE_MOTION))
            {
                return 0;
            }
            if (IsAbnormalState(eAbnormalState.CONFUSION))
            {
                if (damageData.DamageType == eDamageType.ATK)
                {
                    SetMissAtk(damageData.Source, eMissLogType.DODGE_BY_NO_DAMAGE_MOTION, eDamageEffectType.NORMAL, damageData.Target, 1);
                    return 0;
                }
            }
            float damage = damageData.Damage;
            if (critical)
            {
                damage *= 2 * damageData.CriticalDamageRate;
            }
            eActionType actionType = damageData.ActionType;
            eDamageType damageType = damageData.DamageType;
            bool uselog = false;
            if (actionType == eActionType.ATTACK)
            {
                if ((damageType == eDamageType.ATK && IsAbnormalState(eAbnormalState.LOG_ATK_BARRIR)) ||
                   (damageType == eDamageType.MGC && IsAbnormalState(eAbnormalState.LOG_MGC_BARRIR)) ||
                   IsAbnormalState(eAbnormalState.LOG_ALL_BARRIR))
                {
                    uselog = true;
                    damage = damageData.LogBarrieryexpectedDamage;
                }
            }
            if (damageType == eDamageType.ATK && IsAbnormalState(eAbnormalState.CUT_ATK_DAMAGE))
            {
                damage *= (1 - GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_ATK_DAMAGE) / 100);
            }
            if (damageType == eDamageType.MGC && IsAbnormalState(eAbnormalState.CUT_MGC_DAMAGE))
            {
                damage *= (1 - GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_MGC_DAMAGE) / 100);
            }
            if (IsAbnormalState(eAbnormalState.CUT_ALL_DAMAGE))
            {
                damage *= (1 - GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_ALL_DAMAGE) / 100);
            }
            if (uselog)
            {
                float m, s;
                if (damageType == eDamageType.ATK && IsAbnormalState(eAbnormalState.LOG_ATK_BARRIR))
                {
                    s = GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_ATK_BARRIR);
                    m = GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_ATK_BARRIR);
                    if (damage > s)
                    {
                        damage = s + m * Mathf.Log10(1 + (damage - s) / m);
                    }
                }
                if (damageType == eDamageType.MGC && IsAbnormalState(eAbnormalState.LOG_MGC_BARRIR))
                {
                    s = GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_MGC_BARRIR);
                    m = GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_MGC_BARRIR);
                    if (damage > s)
                    {
                        damage = s + m * Mathf.Log10(1 + (damage - s) / m);
                    }
                }
                if (IsAbnormalState(eAbnormalState.LOG_ALL_BARRIR))
                {
                    s = GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_ALL_BARRIR);
                    m = GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_ALL_BARRIR);
                    if (damage > s)
                    {
                        damage = s + m * Mathf.Log10(1 + (damage - s) / m);
                    }
                }
            }
            if (!uselog && !damageData.IgnoreDef)
            {
                if (damageType == eDamageType.ATK)
                {
                    float def = damageData.Target.GetDefZero();
                    damage *= (1.0f - Mathf.Max(0, def - damageData.DefPenetrate) / (100.0f + def));
                }
                else if (damageType == eDamageType.MGC)
                {
                    float def = damageData.Target.GetMagicDefZero();
                    damage *= (1.0f - Mathf.Max(0, def - damageData.DefPenetrate) / (100.0f + def));
                }
            }
            damage = Mathf.Min(1000000, damage);
            if (hasEffect)
            {
                //显示特效
                BattleUIManager.Instance.SetDamageNumber(damageData.Source, this, (int)damage, damageData.DamageType, damageData.DamageEffectType, critical, false);
            }
            if (actionState == eActionState.DIE)
            {
                string mes = UnitName + "受到" + damage + "过量伤害";
                BattleUIManager.Instance.LogMessage(mes,eLogMessageType.GET_DAMAGE, isOther);
                return 0;
            }
            if (p != null)
            {
                damage = p((int)damage, 1);
            }
            if (damageData.Source != null && actionType != eActionType.FORCE_HP_CHANGE)
            {
                //反击相关
            }
            if (skill != null)
            {
                if (actionType == eActionType.ATTACK)
                {
                    damage *= skill.AweValue;
                }
            }
            if (energyAdd)
            {
                ChargeEnergy(eSetEnergyType.BY_SET_DAMAGE, damage * skillStackValDmg, false, this, false, false, true, false);
            }
            if (damage <= 0 && actionType == eActionType.FORCE_HP_CHANGE)
            {
                //特效
                return 0;
            }
            int overRecDamage = 0;
            if (actionType != eActionType.FORCE_HP_CHANGE && actionType != eActionType.INHIBIT_HEAL)
            {
                ExecBarrier(damageData, ref damage, ref overRecDamage);
            }
            Hp -= damage - overRecDamage;
            if (Hp >= 1 && onDamageHit != null)
            {
                onDamageHit(damage);
            }
            //设置HP条
            unitUI.SetHP(Hp / BaseValues.Hp);
            Ondamage?.Invoke(byAttack, damage, critical);
            if (!HasUnDeadTime)
            {
                if (!noMotion)
                {
                    bool k = false;
                    if (skill != null)
                    {
                        k = skill.PauseStopState;
                    }
                    PlayDamageWhenIdle(true, k);
                }
                if (Hp <= 0 && !isDead && actionState <= eActionState.DAMAGE)
                {
                    if (!staticBattleManager.GetCheatValues(eCheatValueType.FORCE_ALIVE, IsOther))
                    {
                        onDefeat?.Invoke();
                        SetState(eActionState.DIE, 0);
                    }
                    else
                    {
                        SetRecovery(1, eInhibitHealType.MAGIC, null, true, true);
                    }

                }
            }

            return (int)damage;
        }
        public void PlayDamageWhenIdle(bool _damageMotionWhenUnionBurst = false, bool _pauseStopState = false)
        {
            //gugugu
        }
        public void SetRecovery(int _value, eInhibitHealType _inhibitHealType, UnitCtrl _source, bool _isEffect = true, bool _isRevival = false, bool _isUnionBurstLifeSteal = false, bool _isRegenerate = false, bool _useNumberEffect = true, BasePartsData _target = null)
        {
            if(_target == null)
            {
                _target = GetFirstParts(true, 0);
            }
            if ((isDead || Hp<=0)&&!_isRevival)
            {
                Debug.LogError("治疗无效，目标已经死了！");
            }
            else
            {
                if(_inhibitHealType != eInhibitHealType.NO_EFFECT && IsAbnormalState(eAbnormalState.INHIBIT_HEAL))
                {
                    if(GetAbnormalStateMainValue(eAbnormalStateCategory.INHIBIT_HEAL)!= 0)
                    {
                        DamageData damageData_0 = new DamageData();
                        damageData_0.Target = GetFirstParts(this != null, 0);
                        damageData_0.Damage = Mathf.FloorToInt(GetAbnormalStateMainValue(eAbnormalStateCategory.INHIBIT_HEAL) * _value);
                        damageData_0.DamageType = eDamageType.NONE;
                        AbnormalStateCategoryData abdata = abnormalStateCategoryDataDictionary[eAbnormalStateCategory.INHIBIT_HEAL];
                        damageData_0.Source = abdata.Source;
                        damageData_0.DamageNumMagic = _inhibitHealType == eInhibitHealType.MAGIC;
                        damageData_0.ActionType = eActionType.INHIBIT_HEAL;
                        SetDamage(damageData_0, false, abdata.ActionId, null, true, abdata.Skill, true, null, false, 1, 1, null);
                    }
                }
                else
                {
                    Hp += _value;
                    //AppendbattleLog;
                    if(Hp > BaseValues.Hp)
                    {
                        Hp = BaseValues.Hp;
                    }
                    unitUI.SetHP(Hp / BaseValues.Hp);
                    string msg = UnitName + "恢复<color=#00FF00>" + _value + "</color>生命";
                    BattleUIManager.Instance.LogMessage(msg,eLogMessageType.HP_RECOVERY,IsOther);
                    if (_isUnionBurstLifeSteal)
                    {
                        unionburstLifeStealNum += _value;
                    }
                    else
                    {
                        if (_useNumberEffect)
                        {
                            //特效数字
                            if (_source == null) { _source = this; }
                            BattleUIManager.Instance.SetHealNumber(_source,this, _value);
                        }
                        if (_isEffect)
                        {
                            //治疗特效
                        }
                    }
                }
            }
        }

        private void ExecBarrier(DamageData _damageData, ref float _fDamage, ref int _overRecoverValue)
        {
            if (IsAbnormalState(eAbnormalState.GUARD_ATK))
            {
                if(_damageData.DamageType == eDamageType.ATK)
                {
                    AbnormalStateCategoryData abdata = abnormalStateCategoryDataDictionary[eAbnormalStateCategory.DAMAGE_RESISTANCE_ATK];
                    float v13 = _fDamage - abdata.MainValue;
                    if (v13 <= 0)//如果伤害小于护盾量
                    {
                        abdata.MainValue -= Mathf.FloorToInt(_fDamage);
                        _fDamage = 0;
                    }
                    else//伤害大于护盾量
                    {
                        EnableAbnormalState(eAbnormalState.GUARD_ATK, false, false, false);
                        _fDamage = v13;
                    }
                }
            }
            if (IsAbnormalState(eAbnormalState.GUARG_MGC))
            {
                if(_damageData.DamageType == eDamageType.MGC)
                {
                    AbnormalStateCategoryData abdata = abnormalStateCategoryDataDictionary[eAbnormalStateCategory.DAMAGE_RESISTANCE_MGK];
                    float v13 = _fDamage - abdata.MainValue;
                    if (v13 <= 0)//如果伤害小于护盾量
                    {
                        abdata.MainValue -= Mathf.FloorToInt(_fDamage);
                        _fDamage = 0;
                    }
                    else//伤害大于护盾量
                    {
                        EnableAbnormalState(eAbnormalState.GUARG_MGC, false, false, false);
                        _fDamage = v13;
                    }
                }
            }
            if (IsAbnormalState(eAbnormalState.DRAIN_ATK))
            {
                if(_damageData.DamageType == eDamageType.ATK)
                {
                    AbnormalStateCategoryData abdata = abnormalStateCategoryDataDictionary[eAbnormalStateCategory.DAMAGE_RESISTANCE_ATK];
                    float v13 = _fDamage - abdata.MainValue;
                    if (v13 <= 0)//如果伤害小于护盾量
                    {
                        long rec = SetRecoveryAndGetOverRecovery(Mathf.FloorToInt(_fDamage), this, _damageData.Target, _damageData.DamageType == eDamageType.MGC);
                        _overRecoverValue += (int)rec;
                        abdata.MainValue -= Mathf.FloorToInt(_fDamage);
                        _fDamage = 0;
                    }
                    else//伤害大于护盾量
                    {
                        long rec = SetRecoveryAndGetOverRecovery(Mathf.FloorToInt(abdata.MainValue), this, _damageData.Target, _damageData.DamageType == eDamageType.MGC);
                        _overRecoverValue += (int)rec;
                        EnableAbnormalState(eAbnormalState.DRAIN_ATK, false);
                        _fDamage = v13;
                    }
                }
            }
            if (IsAbnormalState(eAbnormalState.DRAIN_MGC))
            {
                if (_damageData.DamageType == eDamageType.MGC)
                {
                    AbnormalStateCategoryData abdata = abnormalStateCategoryDataDictionary[eAbnormalStateCategory.DAMAGE_RESISTANCE_MGK];
                    float v13 = _fDamage - abdata.MainValue;
                    if (v13 <= 0)//如果伤害小于护盾量
                    {
                        long rec = SetRecoveryAndGetOverRecovery(Mathf.FloorToInt(_fDamage), this, _damageData.Target, _damageData.DamageType == eDamageType.MGC);
                        _overRecoverValue += (int)rec;
                        abdata.MainValue -= Mathf.FloorToInt(_fDamage);
                        _fDamage = 0;
                    }
                    else//伤害大于护盾量
                    {
                        long rec = SetRecoveryAndGetOverRecovery(Mathf.FloorToInt(abdata.MainValue), this, _damageData.Target, _damageData.DamageType == eDamageType.MGC);
                        _overRecoverValue += (int)rec;
                        EnableAbnormalState(eAbnormalState.DRAIN_MGC, false);
                        _fDamage = v13;
                    }
                }
            }
            if (IsAbnormalState(eAbnormalState.GUANG_BOTH))
            {
                if(_damageData.ActionType != eActionType.DESTROY)
                {
                    AbnormalStateCategoryData abdata = abnormalStateCategoryDataDictionary[eAbnormalStateCategory.DAMAGE_RESISTANCE_BOTH];
                    float v13 = _fDamage - abdata.MainValue;
                    if (v13 <= 0)
                    {
                        abdata.MainValue -= Mathf.FloorToInt(_fDamage);
                        _fDamage = 0;
                    }
                    else
                    {
                        EnableAbnormalState(eAbnormalState.GUANG_BOTH, false);
                        _fDamage = v13;
                    }
                }
            }
            if (IsAbnormalState(eAbnormalState.DRAIN_BOTH))
            {
                if (_damageData.ActionType != eActionType.DESTROY)
                {
                    AbnormalStateCategoryData abdata = abnormalStateCategoryDataDictionary[eAbnormalStateCategory.DAMAGE_RESISTANCE_BOTH];
                    float v13 = _fDamage - abdata.MainValue;
                    if (v13 <= 0)//如果伤害小于护盾量
                    {
                        long rec = SetRecoveryAndGetOverRecovery(Mathf.FloorToInt(_fDamage), this, _damageData.Target, _damageData.DamageType == eDamageType.MGC);
                        _overRecoverValue += (int)rec;
                        abdata.MainValue -= Mathf.FloorToInt(_fDamage);
                        _fDamage = 0;
                    }
                    else//伤害大于护盾量
                    {
                        long rec = SetRecoveryAndGetOverRecovery(Mathf.FloorToInt(abdata.MainValue), this, _damageData.Target, _damageData.DamageType == eDamageType.MGC);
                        _overRecoverValue += (int)rec;
                        EnableAbnormalState(eAbnormalState.DRAIN_BOTH, false);
                        _fDamage = v13;
                    }
                }
            }
        }
        public void SetAbnormalState(UnitCtrl source, eAbnormalState abnormalState, float effectTime, ActionParameter action, Skill skill, float value, float value2, bool reduceEnergy, float reduceEnergyRate)
        {
            if (action!= null)
            {
                //获取特效
            }
            if ((this.staticBattleManager.GameState == eGameBattleState.FIGHTING) && ((!this.IsAbnormalState(eAbnormalState.NO_DAMAGE_MOTION) && !this.IsAbnormalState(eAbnormalState.NO_ABNORMAL)) || ABNORMAL_CONST_DATA[abnormalState].IsBuff))
            {
                switch (abnormalState)
                {
                    case eAbnormalState.BURN:
                    case eAbnormalState.CURSE:
                    case eAbnormalState.SLOW:
                    case eAbnormalState.INHIBIT_HEAL:
                    case eAbnormalState.FAINT:
                        this.OnSlipDamage?.Invoke();
                        break;
                }
                int effecttime_int = (int)(staticBattleManager.FrameRate * effectTime);
                if (abnormalState == eAbnormalState.GUANG_BOTH && IsAbnormalState(eAbnormalState.DRAIN_BOTH)||
                    abnormalState == eAbnormalState.GUARD_ATK && IsAbnormalState(eAbnormalState.DRAIN_ATK)||
                    abnormalState == eAbnormalState.GUARG_MGC && IsAbnormalState(eAbnormalState.DRAIN_MGC))
                {
                    //设置特效
                    return;
                }
                AbnormalStateCategoryData categoryData = abnormalStateCategoryDataDictionary[GetAbnormalStateCategory(abnormalState)];
                if (isDead) { return; }
                if (IsAbnormalState(GetAbnormalStateCategory(abnormalState)))
                {
                    SwitchAbnormalState(abnormalState, null);
                }
                categoryData.Time = effectTime;
                categoryData.Duration = effectTime;
                categoryData.MainValue = value;
                categoryData.SubValue = value2;
                categoryData.EnergyReduceRate = reduceEnergyRate;

                categoryData.ActionId = action == null ? 0 : action.ActionId;
                categoryData.IsEnergyReduceMode = reduceEnergy;
                categoryData.Skill = skill;
                categoryData.Source = source;
                if (!IsAbnormalState(GetAbnormalStateCategory(abnormalState)))
                {
                    categoryData.CurrentAbnormalState = abnormalState;
                    StartCoroutine(UpdateAbnormalState(abnormalState, null));
                }
                return;
            }
            if (IsAbnormalState(eAbnormalState.NO_ABNORMAL))
            {
                SetMissAtk(source, eMissLogType.DODGE_BY_NO_DAMAGE_MOTION, eDamageEffectType.NORMAL, null, 1);
            }
        }
        private IEnumerator UpdateAbnormalState(eAbnormalState _abnormalState, AbnormalStateEffectPrefabData _specialEffectData)
        {
            eAbnormalStateCategory category = GetAbnormalStateCategory(_abnormalState);
            AbnormalStateCategoryData categoryData = abnormalStateCategoryDataDictionary[category];
            categoryData.Time = categoryData.Duration;
            //特效处理
            EnableAbnormalState(_abnormalState, true, categoryData.IsEnergyReduceMode, false);
            while (true)
            {
                if (IsAbnormalState(category))
                {
                    _abnormalState = categoryData.CurrentAbnormalState;
                    if (categoryData.IsEnergyReduceMode)
                    {
                        float energy_min = categoryData.EnergyReduceRate * DeltaTimeForPause_BUFF;
                        SetEnergy(Energy - energy_min, eSetEnergyType.BY_MODE_CHANGE, null);
                        if(Energy <= 0)
                        {
                            EnableAbnormalState(_abnormalState, false);
                            yield break;
                        }
                    }
                    else
                    {
                        categoryData.Time -= DeltaTimeForPause_BUFF;
                        if (categoryData.Time <= 0)
                        {
                            EnableAbnormalState(_abnormalState, false);
                            yield break;
                        }
                    }
                    if (!IsDead)
                    {
                        yield return null;
                        continue;
                    }
                    EnableAbnormalState(_abnormalState, false);
                    break;
                }
                break;
            }
        }
        private void SwitchAbnormalState(eAbnormalState abnormalState, AbnormalStateEffectPrefabData _specialEffectData)
        {
            AbnormalStateCategoryData categoryData = abnormalStateCategoryDataDictionary[GetAbnormalStateCategory(abnormalState)];
            EnableAbnormalState(categoryData.CurrentAbnormalState, false, false, true);
            EnableAbnormalState(abnormalState, true, false, false);
            categoryData.CurrentAbnormalState = abnormalState;
            //处理特效
        }
        private void EnableAbnormalState(eAbnormalState abnormalState, bool enable, bool reduceEnergy = false, bool _switch = false)
        {
            eAbnormalStateCategory category = GetAbnormalStateCategory(abnormalState);
            AbnormalStateCategoryData categoryData = abnormalStateCategoryDataDictionary[category];
            if (!enable)
            {          
                //销毁特效
                categoryData.MainValue = 0;
                categoryData.Time = 0;
                categoryData.Duration = 0;
            }
            categoryData.enable = enable;
            m_abnormalState[abnormalState] = enable;
            switch (abnormalState)
            {
                case eAbnormalState.HASTE:
                    if (enable)
                    {
                        if(actionState == eActionState.IDLE)
                        {
                            spineController.SetTimeScale(timeScale * 2.0f);
                            break;
                        }
                    }
                    else if (!IsUnableActionState() && !isPause)
                    {
                        spineController.Resume();
                    }
                    break;
                case eAbnormalState.POISON:
                case eAbnormalState.BURN:
                case eAbnormalState.CURSE:
                case eAbnormalState.NO_EFFECT_SLIP_DAMAGE:
                case eAbnormalState.VENOM:
                case eAbnormalState.HEX:
                case eAbnormalState.COMPENSATION:
                    if (enable)
                    {
                        if (!slipDamageIdDictionary.ContainsKey(category))
                        {
                            slipDamageIdDictionary.Add(category, 0);
                        }
                        slipDamageIdDictionary[category]++;
                        StartCoroutine(UpdateSlipDamage(category, slipDamageIdDictionary[category]));
                    }
                    break;
                case eAbnormalState.SLOW:
                    if (enable)
                    {
                        //改变颜色
                        if(actionState == eActionState.IDLE)
                        {
                            spineController.SetTimeScale(timeScale * 0.5f);
                        }
                    }
                    else
                    {
                        //改变颜色
                        if (!IsUnableActionState())
                        {
                            spineController.Resume();
                        }
                    }
                    break;
                case eAbnormalState.PARALYSIS:
                case eAbnormalState.FREEZE:
                case eAbnormalState.CHAINED:
                case eAbnormalState.SLEEP:
                case eAbnormalState.STUN:
                case eAbnormalState.DETAIN:
                case eAbnormalState.FAINT:
                    if (enable)
                    {
                        if(actionState!= eActionState.DAMAGE)
                        {
                            SetState(eActionState.DAMAGE, 0);
                            break;
                        }
                    }
                    else if (!IsUnableActionState() && !_switch)
                    {
                        SetMotionResume();
                    }
                    break;
                case eAbnormalState.CONVERT:
                case eAbnormalState.CONFUSION:
                    SetLeftDirection(IsNearestEnemyLeft());
                    if(actionState == eActionState.ATK||actionState == eActionState.IDLE||actionState == eActionState.SKILL1 || actionState == eActionState.SKILL)
                    {
                        CancalByCovert = true;
                        idleStartAfterWaitFrame = m_fCastTimer <= DeltaTimeForPause;
                    }
                    if(Hp>=1&&!IsUnableActionState()&&actionState != eActionState.DAMAGE)
                    {
                        SetState(eActionState.IDLE, 0);
                    }
                    break;
                case eAbnormalState.CONTINUOUS_ATTACK_NEARBY:
                    Debug.LogError("咕咕咕！");
                    break;
                case eAbnormalState.DECOY:
                    Debug.LogError("咕咕咕！");
                    break;
                case eAbnormalState.STONE:
                    if (enable)
                    {
                        if(actionState != eActionState.DAMAGE)
                        {
                            SetState(eActionState.DAMAGE, 0);
                        }
                        //设置石化颜色
                    }
                    else
                    {
                        if (!IsUnableActionState() && !_switch)
                        {
                            SetMotionResume();
                        }
                        //设置石化颜色
                    }
                    break;
                case eAbnormalState.REGENERATION:
                    if (enable)
                    {
                        currentHpRegeneId++;
                        StartCoroutine(UpdateHpRegeneration(currentHpRegeneId));
                    }
                    break;
                case eAbnormalState.TP_REGENERATION:
                    if (enable)
                    {
                        currentTpRegeneId++;
                        StartCoroutine(UpdateTpRegeneration(currentHpRegeneId));
                    }
                    break;
                default:
                    break;
            }
            OnChangeState?.Invoke(this, ABNORMAL_CONST_DATA[abnormalState].IconType, enable);

            if(!enable && !_switch)
            {
                //Elements_Battle_BattleManager__RestartAbnormalStateField(v43, this, abnormalState, 0LL)
            }
        }
        public void EnableBuffParam(BuffParamKind _kind, Dictionary<BasePartsData, int> _value, bool _enable, UnitCtrl _source, bool _isBuff, bool _additional)
        {
            eStateIconType bufficon;
            BuffDebuffConstData buffDebuffConstData = new BuffDebuffConstData(0,0);
            if(BUFF_DEBUFF_ICON_DIC.TryGetValue(_kind,out BuffDebuffConstData buffDebuffConstData1))
            {
                buffDebuffConstData = buffDebuffConstData1;
            }
            else
            {
                string word = UnitName + "的BUFF图标种类出错，未找到种类为" + _kind.GetDescription() + "的图标！";
                BattleUIManager.Instance.LogMessage(word, eLogMessageType.ERROR, IsOther);
            }
            if (_isBuff)
            {
                bufficon = buffDebuffConstData.BuffIcon;
            }
            else
            {
                bufficon = buffDebuffConstData.DebuffIcon;
            }
            OnChangeState?.Invoke(this, bufficon, _enable);
            if(bufficon != eStateIconType.NONE)
            {
                if (_enable)
                {
                    if (_isBuff)
                    {
                        buffCounter++;
                    }
                    else
                    {
                        debuffCounter++;
                    }
                }
                else
                {
                    if (_isBuff)
                    {
                        buffCounter--;
                    }
                    else
                    {
                        debuffCounter--;
                    }
                }
            }
            foreach(BasePartsData a in _value.Keys)
            {
                a.SetBuffDebuff(_enable, _value[a], _kind, _source, null, _additional);
            }
        }
        public void SetBuffParam(BuffParamKind _kind, Dictionary<BasePartsData, int> _value, float _time, int _skillId, UnitCtrl _source, bool _despelable, eEffectType _effectType, bool _isBuff, bool _additional)
        {
            if(_isBuff || !IsAbnormalState(eAbnormalState.NO_EFFECT_SLIP_DAMAGE))
            {
                if(_effectType == eEffectType.COMMON)
                {
                    //添加特效图标
                }
                buffDebuffIndex++;
                StartCoroutine(UpdateBuffParam(_kind, _value, _time, _skillId, _source, _despelable, buffDebuffIndex, _isBuff, _additional));
            }
            else
            {
                SetMissAtk(_source, eMissLogType.DODGE_BY_NO_DAMAGE_MOTION, eDamageEffectType.NORMAL, null, 1);
            }
        }
        private IEnumerator UpdateBuffParam(BuffParamKind _kind, Dictionary<BasePartsData, int> _value, float _maxTime, int _skillId, UnitCtrl _source, bool _despelable, int _buffDebuffId, bool _isBuff, bool _additional)
        {
            EnableBuffParam(_kind, _value, true, _source, _isBuff, _additional);
            float time_5 = 0;
            while (true)
            {
                time_5 += DeltaTimeForPause_BUFF;
                if (DeltaTimeForPause > 1)
                {
                    //buffDebuffSkilIds.Remove(_skillId);
                }
                bool k = _buffDebuffId <= (_despelable && _isBuff ? clearedBuffIndex : clearedDebuffIndex);
                bool v16 = false;
                if (time_5 >= _maxTime || IdleOnly)
                {
                    v16 = true;
                }
                else
                {
                    v16 = Hp < 1;
                }
                if (k || v16)
                {
                    //buffDebuffSkilIds.Remove(_skillId);//特效相关
                    EnableBuffParam(_kind, _value, false, _source, _isBuff, _additional);
                    yield break;
                }
                yield return null;
            }
        }
        public void ChargeEnergy(eSetEnergyType setEnergyType, float energy, bool hasEffect, UnitCtrl source, bool hasNumberEffect, bool isEffectTypeCommon, bool useRecoveryRate, bool isRegenerate)
        {
            if(!IsAbnormalState(eAbnormalState.TP_REGENERATION)||setEnergyType != eSetEnergyType.BY_ATK&&setEnergyType != eSetEnergyType.KILL_BONUS)
            {
                if (energy > 0 && useRecoveryRate)
                {
                    energy *= (100.0f + GetEnergyRecoveryRateZero()) / 100.0f;
                }
                SetEnergy(energy + Energy, setEnergyType, source);
                //加能量特效
                if (hasNumberEffect)
                {
                    BattleUIManager.Instance.SetEnergyNumber(source, this, (int)energy);
                }
                string word = UnitName + "通过" + setEnergyType.GetDescription() + (energy >= 0 ? "增加<color=#3791FF>" : "减少<color=#3791FF>") + Mathf.Abs(energy) + "</color>点能量";
                BattleUIManager.Instance.LogMessage(word, eLogMessageType.CHANGE_TP, IsOther);

            }
        }
        public void SetEnergy(float energy, eSetEnergyType type, UnitCtrl source)
        {
            //log
            energy = Mathf.Min(Mathf.Max(0, energy), BattleDefine.MAX_ENERGY);
            Energy = energy;
            if(unitUI!= null)
            {
                unitUI.SetTP(energy / BattleDefine.MAX_ENERGY);
            }
            EnergyChange?.Invoke(this);

        }
        public eConsumeResult ConsumeEnergy()
        {
            if (Energy < 1000)
            {
                return eConsumeResult.FAILED;
            }
            float enerey = BaseValues.Enerey_reduce_rate * 10;
            if (actionController.Skill1IsChargeTime)
            {
                if (actionController.DisableUBByModeChange)
                {
                    return eConsumeResult.FAILED;
                }
                if (actionController.Skill1Charging)
                {
                    SetEnergy(enerey, eSetEnergyType.BY_USE_SKILL, null);
                    return eConsumeResult.SKILL_RELEASE;
                }
                else
                {
                    SetEnergy(999, eSetEnergyType.BY_USE_SKILL, null);
                    return eConsumeResult.SKILL_CHARGE;
                }
            }
            else
            {
                SetEnergy(enerey, eSetEnergyType.BY_USE_SKILL, null);
                return eConsumeResult.SKILL_OK;
            }
        }
        public void CureAllAbnormalState()
        {
            for(int i = 1; i <= 48; i++)
            {
                if (IsAbnormalState((eAbnormalState)i))
                {
                    EnableAbnormalState((eAbnormalState)i, false);
                }
            }
            for(int i = LifeStealQueueList.Count - 1; i >= 0; i--)
            {
                LifeStealQueueList[i].Dequeue();
                if(LifeStealQueueList[i].Count == 0)
                {
                    LifeStealQueueList.RemoveAt(i);
                    OnChangeState?.Invoke(this, eStateIconType.BUFF_ADD_LIFE_STEAL, false);
                }
            }
            //处理反击字典
            //处理knightGuardDatas
            DamageSealDataDictionary.Clear();
            DamageOnceOwnerSealDateDictionary.Clear();
            DamageOwnerSealDataDictionary.Clear();
        }
        public void SkillEndProcess()
        {
            SetState(eActionState.IDLE, 0);
            CancalByCovert = false;
        }
        public void DisableAbnormalStateById(eAbnormalState _abnormalState, int _actionId)
        {
            if (abnormalStateCategoryDataDictionary[GetAbnormalStateCategory(_abnormalState)].ActionId == _actionId)
            {
                EnableAbnormalState(_abnormalState, false, false, false);
            }
        }
        /// <summary>
        /// 设置回复生命并且获得溢出的治疗
        /// </summary>
        /// <param name="_value"></param>
        /// <param name="_source"></param>
        /// <param name="_target"></param>
        /// <param name="_isMagic"></param>
        /// <returns>溢出的治疗量</returns>
        private int SetRecoveryAndGetOverRecovery(int _value, UnitCtrl _source, BasePartsData _target, bool _isMagic)
        {
            eInhibitHealType eInhibit = _isMagic ? eInhibitHealType.MAGIC : eInhibitHealType.PHYSICS;
            SetRecovery(_value, eInhibit, _source, true, false, false, false, true, _target);
            int overRecoveryValue = (int)(Hp + _value - BaseValues.Hp);
            return Mathf.Max(0,overRecoveryValue);
        }
        private void SetMotionResume()
        {
            spineController.Resume();
        }
        private IEnumerator UpdateSlipDamage(eAbnormalStateCategory _category, int _slipDamageId)
        {
            AbnormalStateCategoryData categoryData = abnormalStateCategoryDataDictionary[_category];
            float time_5 = 0;
            int damage_5 = (int)categoryData.MainValue;
            int incrementDamage_5 = (int)(categoryData.MainValue * categoryData.SubValue / 100.0f);//一般都为0            yield return null;
            yield return null;
            while (true)
            {
                if (IsAbnormalState(_category))
                {
                    if(slipDamageIdDictionary[_category] == _slipDamageId)
                    {
                        time_5 += DeltaTimeForPause_BUFF;
                        if (time_5 > 0.995f)
                        {
                            DamageData damageData = new DamageData();
                            damageData.Target = GetFirstParts(false, 0);
                            damageData.Damage = damage_5;
                            damageData.DamageType = eDamageType.NONE;
                            damageData.Source = categoryData.Source;
                            damageData.IsSlipDamage = true;
                            damageData.ActionType = eActionType.SLIP_DAMAGE;
                            SetDamage(damageData, false, categoryData.ActionId, null, true, categoryData.Skill, true, null, false, 1, 1, null);
                            damage_5 += incrementDamage_5;
                            time_5 = 0;
                        }
                        yield return null;
                        continue;
                    }
                }
                break;
            }
        }
        private IEnumerator UpdateHpRegeneration(int _regeneId)
        {
            yield return null;
            Debug.LogError("咕咕咕！");
        }
        private IEnumerator UpdateTpRegeneration(int _regeneId)
        {
            yield return null;
            Debug.LogError("咕咕咕！");

        }
    }
}