using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PCRCaculator.Battle
{
    /// <summary>
    /// 移动类
    /// </summary>
    public class MoveAction : ActionParameter
    {
        private enum eMoveType
        {
            TARGET_POS_RETURN = 1,
            ABSOLUTE_POS_RETURN = 2,
            TARGET_POS = 3,
            ABSOLUTE_POS = 4,
            TARGET_MOVE_BY_VELOSITY = 5,
            ABSOLUTE_MOVE_BY_VELOCITY = 6,
            ABSOLUTE_MOVE_DONOT_USE_DIRECTION = 7
        }

        private const int MOVE_BY_VELOCITY_LOOP_MOTION_SUFFIX = 1;
        private const int MOVE_BY_VELOCITY_LOOP_END_MOTION_SUFFIX = 2;
        private const int MOVE_BY_VELOCITY_RETURN_LOOP_MOTION_SUFFIX = 3;
        private const int MOVE_BY_VELOCITY_RETURN_LOOP_MOTION_END_SUFFIX = 4;
        private const float MOVE_POSITION_Y = 1f;
        private ActionParameter endAction;

        public ActionParameter EndAction { get => endAction; set => endAction = value; }

        private IEnumerator absoluteMoveByVerocity(float _distance, float _speed, UnitActionController _sourceUnitActionController, UnitCtrl _source, Skill _skill) =>
            null;

        private static Vector3 CalculatePosotion(UnitCtrl _source, BasePartsData _target, UnitActionController _sourceActionController, Dictionary<int, float> _valueDictionary)
        {
            float x = _valueDictionary[1];
            if (_source.IsOther)
            {
                x *= -1;
            }
            Vector3 vector3 = _target.GetPosition() - _source.FixedPosition;
            float distance = vector3.x - _target.GetBodyWidth() - x;
            Vector3 a = Vector3.zero;
            a.x = distance;
            return a;
        }
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            if(ActionDetail2 != 0)
            {
                endAction.CancelByIfForAll = true;
            }
            switch (ActionDetail1)
            {
                case 1://瞬间移动
                    if (num == 1)
                    {
                        source.SetPosition(skill.OwnerReturnPosition);
                        break;
                    }
                    float deltax = CalculatePosotion(source, target, sourceActionController, valueDictionary).x;
                    source.SetPosition(deltax);
                    break;
                case 2://未知
                    Debug.LogError("未知移动方式！");
                    break;
                case 3://瞬间移动并且改变Y坐标
                    float deltax_3 = CalculatePosotion(source, target, sourceActionController, valueDictionary).x;
                    source.SetPosition(deltax_3);
                    source.StartCoroutine(ResetPositionY(skill, source));
                    break;
                case 4://怪物才有的，鸽了
                    Debug.LogError("未知移动方式！");
                    break;
                case 5://朝目标方向移动
                    source.PlaySkillAnimation(skill.SkillId, 1, true);
                    source.StartCoroutine(TargetMoveByVerocity(valueDictionary[1], valueDictionary[2], target, sourceActionController, source, skill));
                    break;
                case 6://朝固定位置移动
                    Debug.LogError("未知移动方式！");
                    break;
                case 7://???
                    Debug.LogError("未知移动方式！");
                    break;
                default:
                    Debug.LogError("未知移动方式！");
                    break;
            }
            if(ActionDetail1 != 5 && ActionDetail1 != 6)
            {
                if (source.FixedPosition.x < -1024)
                {
                    source.SetPosition((int)-1024);
                }
                else if (source.FixedPosition.x > 1024)
                {
                    source.SetPosition((int)1024);
                }
            }

        }

        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            endAction = _skill.ActionParmeters.Find(a => a.ActionId == this.ActionDetail2);
        }

        private IEnumerator MoveByVerocityEnd(UnitActionController _sourceActionController, UnitCtrl _source, Skill _skill, float _velocity)
        {
            if(endAction != null)
            {
                endAction.CancelByIfForAll = false;
                _sourceActionController.ExecUnitActionWithDelay(endAction, _skill, false, false, false);
            }
            //特效
            while (true)
            {
                if (!_source.SpineController.IsPlayingAnimeBattleComplete)
                {
                    yield return null;
                    continue;
                }
                if (_source.JudgeHasSkillAnimation(_skill.SkillId,3))
                {
                    _source.PlaySkillAnimation(_skill.SkillId, 3, true);
                    _source.StartCoroutine(MoveByVerocityReturn(_sourceActionController, _source, _skill, _velocity));
                }
                else
                {
                    _source.SkillEndProcess();
                }
                break;
            }
        }

        private IEnumerator MoveByVerocityReturn(UnitActionController _sourceActionController, UnitCtrl _source, Skill _skill, float _velocity)
        {
            yield return null;
            Debug.LogError("咕咕咕！");
        }

        private IEnumerator moveType4ReturnEnd(UnitCtrl _source, Skill _skill) =>
            null;

        private IEnumerator ResetPositionY(Skill _skill, UnitCtrl _source)
        {
            while (true)
            {
                if(_source .SpineController.IsPlayingAnimeBattleComplete||
                    _source.IsUnableActionState()||
                    _source.ActionState == eActionState.DAMAGE)
                {
                    _source.SetPosition(_skill.OwnerReturnPosition);
                    //遍历所有敌方，更新正在走路的敌方单位朝向
                    yield break;
                }
                yield return null;
            }
        }

        private IEnumerator TargetMoveByVerocity(float _main, float _sub, BasePartsData _target, UnitActionController _sourceUnitActionController, UnitCtrl _source, Skill _skill)
        {
            _main += _target.GetBodyWidth() * 0.5f;
            _sourceUnitActionController.MoveEnd = false;
            Transform sourceTransform = _sourceUnitActionController.transform;
            bool isright = sourceTransform.position.x < _target.Owner.transform.position.x;
            bool isUB = _skill.SkillId == _source.UBSkillId;
            while (true)
            {
                if (_source.IsCancalActionState(_skill.SkillId)||
                    (!isUB&&_source.IsUnableActionState()))
                {
                    //清除技能特效                    
                }
                else
                {
                    float deltaX = _sub * BattleManager.Instance.DeltaTimeForPause;
                    if (!isright)
                    {
                        deltaX *= -1;
                    }
                    _source.SetPosition(deltaX);
                    float distance = Mathf.Abs(sourceTransform.position.x - _target.Owner.transform.position.x) * 60;
                    if (distance > _main)
                    {
                        yield return null;
                        continue;
                    }
                    _sourceUnitActionController.MoveEnd = true;
                    _source.PlaySkillAnimation(_skill.SkillId, 2, false);
                    //清除技能特效  
                    _source.StartCoroutine(MoveByVerocityEnd(_sourceUnitActionController, _source, _skill, _sub));
                }
                break;
            }
        }

    }
    public class KnockAction : ActionParameter
    {
        private enum eKnockType
        {
            KNOCK_UP_DOWN = 1,
            KNOCK_UP = 2,
            KNOCK_BACK = 3,
            MOVE_TARGET = 4,
            MOVE_TARGET_PARABORIC = 5,
            KNOCK_BACK_LIMITED = 6
        }
        private const float DURATION = 0.5f;
        private const float HEIGHT = 300f;
        private List<BasePartsData> unitListForSort;

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            AppendIsAlreadyexeced(target.Owner, num);
            switch (ActionDetail1)
            {
                case 1:
                    sourceActionController.StartCoroutine(UpdateKnockUpDown(target.Owner));
                    SetTargetDamageNoOverrap(target.Owner);
                    return;
                case 2:
                    return;
                case 3:
                    sourceActionController.StartCoroutine(UpdateKnockback(target.Owner, source, GetKnockValue));
                    SetTargetDamageNoOverrap(target.Owner);
                    return;
                case 4:
                case 5://目前没有角色的技能在此分支，故鸽了
                    Debug.LogError("咕咕咕！");
                    break;
                case 6://只有一个怪的技能分类在这里
                    sourceActionController.StartCoroutine(UpdateKnockback(target.Owner, source, GetKnockValueLimited));
                    SetTargetDamageNoOverrap(target.Owner);
                    return;
                default:
                    return;
            }
            ///后续代码鸽了
        }

        private float GetKnockValue(UnitCtrl _target, UnitCtrl _source)
        {
            int direction = _target.FixedPosition.x > _source.FixedPosition.x ? 1 : -1;
            float value = 0;
            //如果不是boss
            value = Value[1] * direction;
            if (value <= 0)
            {
                return Mathf.Max(value,-1024 + Mathf.Abs(_target.FixedPosition.x));
            }
            else
            {
                return Mathf.Min(value, 1024 - Mathf.Abs(_target.FixedPosition.x));
            }
        }

        private float GetKnockValueLimited(UnitCtrl _target, UnitCtrl _source)
        {
            Debug.LogError("咕咕咕！");
            return 0;
        }

        private void SetTargetDamageNoOverrap(UnitCtrl _target)
        {
            if(_target.ActionState != eActionState.DAMAGE)
            {
                //???
                _target.SetState(eActionState.DAMAGE, 0);
            }
        }

        private IEnumerator UpdateKnockback(UnitCtrl _target, UnitCtrl _source, Func<UnitCtrl, UnitCtrl, float> _func)
        {
            float knockValue = _func.Invoke(_target, _source);
            Vector3 startpos = _target.transform.position;
            float duration = Mathf.Max(Mathf.Abs(knockValue) / Value[3], 0.1f);
            _target.KnockBackEnableCount++;
            float time_5 = 0;
            while (true)
            {
                Vector3 addpos = new Vector3(knockValue * time_5 / duration, 0, 0);
                _target.SetPosition(startpos + addpos/60.0f);
                time_5 += BattleManager.Instance.DeltaTimeForPause;
                if (time_5 <= duration)
                {
                    yield return null;
                    continue;
                }
                startpos.x += knockValue / 60.0f;
                _target.SetPosition(startpos);
                _target.KnockBackEnableCount--;
                if (!_target.IsUnableActionState())
                {
                    _target.SpineController.Resume();
                }
                break;
            }
        }

        private IEnumerator UpdateKnockUpDown(UnitCtrl _target)
        {
            float time_5 = 0;
            float knockValue = Value[1];
            float duration = knockValue / Value[3];
            _target.KnockBackEnableCount++;
            Vector3 startpos = _target.transform.position;
            while (true)
            {
                if (duration > time_5)
                {
                    time_5 += BattleManager.Instance.DeltaTimeForPause;
                    if (time_5 > duration)
                    {
                        time_5 = duration;
                    }
                    Vector3 addpos = new Vector3(knockValue * time_5 / duration, 0, 0);
                    _target.SetPosition(startpos + addpos/60.0f);
                    yield return null;
                    continue;
                }
                startpos = _target.transform.position;
                time_5 = 0;
                break;
            }
            duration = knockValue / Value[4];
            while (true)
            {
                if (duration <= time_5)
                {
                    _target.KnockBackEnableCount--;
                    if (!_target.IsUnableActionState())
                    {
                        _target.SpineController.Resume();
                    }
                    yield break;
                }
                time_5 += BattleManager.Instance.DeltaTimeForPause;
                if (time_5 > duration)
                {
                    time_5 = duration;
                }
                Vector3 minuspos = new Vector3(knockValue * time_5 / duration, 0, 0);
                _target.SetPosition(startpos -minuspos/60.0f);
                yield return null;
            }
        }

        private IEnumerator UpdateParaboricKnock(Vector3 _pos, UnitCtrl _target) =>
            null;
    }
    /// <summary>
        /// BUFF/DEBUFF类技能
        /// </summary>
    public class BuffDebuffAction : ActionParameter
    {
        /*private const int THOUSAND = 0x3e8;
        private const float PERCENT_DIGIT = 100f;
        private const int UNDESPELABLE_NUMBER = 2;
        private const int DETAIL_DIGIT = 10;
        private const int DETAIL_DEBUFF = 1;*/

        public static int CalculateBuffDebuffParam(BasePartsData _target, float _value, eChangeParameterType _changeParamType, BuffParamKind _targetChangeParamKind, bool _isDebuf)
        {
            float result = 0;
            if(_changeParamType== eChangeParameterType.PERCENTAGE)
            {
                float v12 = _value / 100.0f;
                float v13 = 0;
                switch (_targetChangeParamKind)
                {
                    case BuffParamKind.ATK:
                        v13 = _target.GetStartAtk();
                        break;
                    case BuffParamKind.DEF:
                        v13 = _target.GetStartDef();
                        break;
                    case BuffParamKind.MAGIC_STR:
                        v13 = _target.GetStartMagicStr();
                        break;
                    case BuffParamKind.MAGIC_DEF:
                        v13 = _target.GetStartMagicDef();
                        break;
                    case BuffParamKind.DODGE:
                        v13 = _target.GetStartDodge();
                        break;
                    case BuffParamKind.PHYSICAL_CRITICAL:
                        v13 = _target.GetStartPhysicalCritical();
                        break;
                    case BuffParamKind.MAGIC_CRITICAL:
                        v13 = _target.GetStartMagicCritical();
                        break;
                    case BuffParamKind.ENERGY_RECOVER_RATE:
                        v13 = _target.Owner.BaseValues.Energy_recovery_rate;
                        break;
                    case BuffParamKind.LIFE_STEAL:
                        v13 = _target.Owner.BaseValues.Life_steal;
                        break;
                    case BuffParamKind.MOVE_SPEED:
                        v13 = _target.Owner.MoveRate;
                        break;
                    case BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE:
                        v13 = _target.Owner.PhysicalCriticalDamageRate;
                        break;
                    case BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE:
                        v13 = _target.Owner.MagicalCriticalDamageRate;
                        break;
                    default:
                        BattleUIManager.Instance.LogMessage("ERROR!", eLogMessageType.ERROR, _target.Owner.IsOther);
                        break;
                }
                result = v12 * v13;
            }
            else if(_changeParamType == eChangeParameterType.FIXED)
            {
                result = BattleUtil.FloatToIntReverseTruncate(_value);
            }
            int result_int = Mathf.CeilToInt(Mathf.Abs(result));
            return result_int;
        }

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            AppendIsAlreadyexeced(target.Owner, num);
            if (ActionDetail2 != 2)//所有玩家角色技能都满足这个条件
            {
                Dictionary<BasePartsData, int> dic = new Dictionary<BasePartsData, int>();
                BuffParamKind paramKind = (BuffParamKind)Mathf.FloorToInt(ActionDetail1/10.0f);
                bool isDebuff = ActionDetail1 - Mathf.FloorToInt(ActionDetail1 / 10.0f) == 1;
                if(target.Owner.IsPartsBoss && paramKind != BuffParamKind.ENERGY_RECOVER_RATE && paramKind != BuffParamKind.MOVE_SPEED)
                {
                    Debug.LogError("咕咕咕！");
                }
                else
                {
                    int v34 = CalculateBuffDebuffParam(target, valueDictionary[2], (eChangeParameterType)valueDictionary[1], paramKind, isDebuff);
                    dic.Add(target.Owner.DummyPartsData, v34);
                }
                target.Owner.SetBuffParam(
                    paramKind,
                    dic,
                    valueDictionary[4],
                    skill.SkillId,
                    source,
                    valueDictionary[7] != 2,
                    EffectType,
                    !isDebuff,
                    false//目前没有满足additional条件的……
                    ) ;
            }
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
            //if(ActionDetail2 == 2)
            //{
                //只有怪物的技能满足这个条件，先鸽了
            //}

        }
        public override void SetLevel(float level)
        {
            base.SetLevel(level);
            Value[2] = MasterData_mine.values[1] + MasterData_mine.values[2] * level;
            Value[4] = MasterData_mine.values[3] + MasterData_mine.values[4] * level;

        }
        public enum eBuffDebuffStartReleaseType
        {
            NORMAL = 1,
            BREAK = 2
        }

        public enum eChangeParameterType
        {
            FIXED = 1,
            PERCENTAGE = 2
        }
    }
    /// <summary>
    /// 羊驼用开局罚站
    /// </summary>
    public class WaveStartIdleAction : ActionParameter
    {
        public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<int, float> _valueDictionary)
        {
            base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
            //更改透明为白色
        }

        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
        }

        public override void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            List<UnitCtrl> targetlist = _source.IsOther ? BattleManager.Instance.PlayersList : BattleManager.Instance.EnemiesList;
            List<UnitCtrl> targets = targetlist.FindAll(a => a.Hp > 0);
            if (targets.Count > 0)
            {
                _source.IdleOnly = true;
                int pos = _source.IsOther ? 1400 : -1400;
                _source.SetPosition(pos);
                //将颜色改为透明
                _sourceActionController.StartCoroutine(updateIdle(_source));
            }
            else
            {
                _source.SkillAreaWidthList[_skill.SkillId] = _source.SearchAreaWidth;
            }
        }

        private IEnumerator updateIdle(UnitCtrl _source)
        {
            _source.IsMoveSpeedForceZero = true;
            _source.SetLeftDirection(_source.IsOther);
            float time = Value[1];
            while (true)
            {
                time -= BattleManager.Instance.DeltaTimeForPause;
                if(BattleManager.Instance.GameState != eGameBattleState.FIGHTING)
                {
                    _source.IsMoveSpeedForceZero = false;
                    yield break;
                }
                else
                {
                    if (time >= 0)
                    {
                        yield return null;
                        continue;
                    }
                    _source.IsMoveSpeedForceZero = false;
                    _source.IdleOnly = false;
                    _source.SetState(eActionState.WALK, 0);
                    yield break;
                }
            }
        }

    }


}
