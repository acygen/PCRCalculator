using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCRCaculator;
using PCRCaculator.Battle;
namespace Elements
{
    public class FirearmCtrl : SkillEffectCtrl
    {
        public float HitDelay;
        public float MoveRate;
        public float duration;
        public eMoveTypes MoveType;
        public float startRotate;
        public float endRotate;
        public bool IsAbsolute;
        public bool InFlag;
        public BasePartsData FireTarget;
        public List<NormalSkillEffect> SkillHitEffects;
        public List<ActionParameter> EndActions;
        public Skill Skill;
        public Action<FirearmCtrl> OnHitAction;
        public Vector3 TargetPos;
        public List<ShakeEffect> ShakeEffects;
        public Bounds ColliderBox;
        public bool stopFlag;
        public Vector3 initialPosistion;
        //private CustomEasing easingX;
        //private CustomEasing easingUpY;
        //private CustomEasing easingDownY;
        //private CustomEasing easingUpRotate;
        //private CustomEasing easingDownRotate;
        private bool activeSelf;
        public Vector3 speed;
        public UnitCtrl owner;
        public Action onCowHit;
        private const float HIT_DELAY_DISTANCE = 0.2f;

        public override bool _Update() =>
            new bool();

        protected virtual void Awake()
        {
            isRepeat = true;
        }

        private bool collisionDetection(bool _hitFlag, float _currentTime)
        {
            if(MoveType == eMoveTypes.PARABORIC||MoveType == eMoveTypes.PARABORIC_ROTATE)
            {
                if (duration * 0.5f > _currentTime)
                {
                    return _hitFlag;
                }
            }
            if (!_hitFlag)
            {
                if (!InFlag)
                {
                    if(Mathf.Abs(transform.position.x - FireTarget.GetPosition().x)<=(ColliderBox.size.x + FireTarget.GetColliderSize().x) * 0.5f)
                    {
                        if (MoveType != eMoveTypes.PARABORIC && MoveType != eMoveTypes.PARABORIC_ROTATE)
                        {
                            return true;
                        }
                        InFlag = true;
                    }
                }
                return false;
            }
            return true;
        }

        protected virtual Vector3 GetHeadBonePos(BasePartsData _target)
        {
            return _target.GetButtontransformPosition() + _target.GetFixedCenterPos();
        }

        private Vector3 GetParaboricPosition(float _currentTime, float _deltaTime)
        {
            Vector3 pos = initialPosistion;
            pos.x += speed.x * _currentTime;
            return pos;
        }

        protected virtual bool getStopFlag() =>
            new bool();

        public virtual void Initialize(BasePartsData _target, List<ActionParameter> _actions, Skill _skill, List<NormalSkillEffect> _skillEffect, UnitCtrl _owner, float _height, bool _hasBlackOutTime, bool _isAbsolute, Vector3 _targetPosition, List<ShakeEffect> _shakes, eTargetBone _targetBone)
        {
            ShakeEffects = _shakes;
            IsAbsolute = _isAbsolute;
            Skill = _skill;
            Vector3 pos = _targetPosition;
            if (!IsAbsolute)
            {
                if (_targetBone == eTargetBone.FIXED_CENETER||_targetBone == eTargetBone.CENTER)
                {
                    pos = _target.GetButtontransformPosition() + _target.GetFixedCenterPos();
                }
                if(_targetBone == eTargetBone.BOTTOM)
                {
                    pos = _target.GetButtontransformPosition();
                }
                if(_targetBone == eTargetBone.HEAD)
                {
                    pos = GetHeadBonePos(_target);
                }
            }
            TargetPos = pos;
            FireTarget = _target;
            _target.Owner.FirearmCtrlsOnMe.Add(this);
            SkillHitEffects = _skillEffect;
            EndActions = _actions;
            owner = _owner;
            SetInitialPosition();
            InitMoveType(_height, _owner);
            float distance = Vector3.Distance(TargetPos, initialPosistion) + 1;
            owner.StartCoroutine(UpdatePosition(distance));
        }

        private void InitMoveType(float _height, UnitCtrl _owner)
        {
            switch (MoveType)
            {
                case eMoveTypes.LINEAR:
                    speed = (Target.transform.position - transform.position) * MoveRate;
                    transform.rotation = Quaternion.FromToRotation(transform.position, Target.transform.position);
                    break;
                case eMoveTypes.NONE:
                case eMoveTypes.HORIZONTAL:
                    speed = new Vector3(MoveRate, 0, 0);
                    break;
                case eMoveTypes.PARABORIC://抛物线
                case eMoveTypes.PARABORIC_ROTATE:
                    speed = (Target.transform.position - transform.position) * MoveRate;
                    transform.rotation = Quaternion.FromToRotation(transform.position, Target.transform.position);
                    Debug.LogError("抛物线轨迹暂时鸽了，用直线代替！");
                    break;

            }
        }

        protected override void onDestroy()
        {
        }

        public override void ResetParameter(GameObject _prefab, int _skinId, bool _isShadow)
        {
        }

        protected virtual void SetInitialPosition()
        {
            initialPosistion = transform.position;
        }

        private IEnumerator UpdatePosition(float _lifeDistance)
        {
            float currentTime_5 = 0;
            bool hitfFlag_5 = false;
            float hitTimer_5 = 0;
            while (true)
            {
                if (!IsPlaying)
                {
                    yield break;
                }
                if (!activeSelf)
                {
                    yield return null;
                    continue;
                }
                if (hitfFlag_5)
                {
                    hitTimer_5 += owner.DeltaTimeForPause;
                    float delay = MoveType == eMoveTypes.LINEAR ? HIT_DELAY_DISTANCE / MoveRate : HitDelay;
                    if (hitTimer_5 >= delay)
                    {
                        hitTimer_5 = 0;
                        hitfFlag_5 = false;
                        stopFlag = true;
                        if (OnHitAction != null)
                        {
                            FireTarget.Owner.FirearmCtrlsOnMe.Remove(this);
                            OnHitAction(this);
                        }
                        onCowHit?.Invoke();
                        onCowHit = null;
                    }
                }
                if (!getStopFlag())
                {
                    if(this == null)
                    {
                        yield break;
                    }
                    Vector3 nextPos = transform.position;
                    if (MoveType == eMoveTypes.PARABORIC || MoveType == eMoveTypes.PARABORIC_ROTATE)
                    {
                        currentTime_5 += owner.DeltaTimeForPause;
                        nextPos = GetParaboricPosition(currentTime_5, owner.DeltaTimeForPause);
                        if(currentTime_5 > duration)
                        {
                            hitfFlag_5 = true;
                        }
                    }
                    else if(MoveType == eMoveTypes.HORIZONTAL || MoveType == eMoveTypes.LINEAR)
                    {
                        transform.position += speed * owner.DeltaTimeForPause;
                        if (!IsAbsolute)
                        {
                            hitfFlag_5 = collisionDetection(hitfFlag_5, currentTime_5);
                        }
                        yield return null;
                        continue;
                    }
                    if (Vector3.Distance(initialPosistion, transform.position) <= _lifeDistance)
                    {
                        if (!IsAbsolute)
                        {
                            hitfFlag_5 = collisionDetection(hitfFlag_5, currentTime_5);
                        }
                        yield return null;
                        continue;
                    }
                    transform.position = nextPos;
                }
                activeSelf = false;
                FireTarget.Owner.FirearmCtrlsOnMe.Remove(this);
                timeToDie = true;
                if (!IsAbsolute)
                {
                    hitfFlag_5 = collisionDetection(hitfFlag_5, currentTime_5);
                }
                yield return null;
                continue;
            }
        }


    }
}