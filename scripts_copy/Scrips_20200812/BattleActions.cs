using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using UnityEngine;

namespace PCRCaculator.Battle
{
    /// <summary>
    /// 技能片段基类
    /// </summary>
    public class ActionParameter
    {
        private static BattleManager staticBattleManager;
        private UnitCtrl owner;
        private eTargetAssignment targetAssignment;
        private PirorityPattern targetSort;
        private int targetNth;
        private int targetNum;
        private float targetWidth;
        private DirectionType direction;
        private float position;
        private int actionDetail1;
        private int actionDetail2;
        private int actionDetail3;
        private int actionId;
        private List<BasePartsData> targetList;
        private Dictionary<int, float> value;
        private eActionType actionType;
        private float[] execTime;
        private List<ActionExecTime> actionExecTimeList;
        private float actionWeightSum;
        private int dependActionId;
        private bool referencedByEffect;
        private bool referencedByReflection;
        private List<int> actionChildrenIndexes;
        private OnDamageHitDelegate OnDamageHit;
        //特效列表
        private AbnormalStateFieldAction abnormalStateFieldAction;
        public OnActionEndDelegate OnActionEnd;
        private Action OnDefeatEnemy;
        private Action OnInitWhenNoTarget;
        private bool cancelByIfForAll;
        private Dictionary<BasePartsData, long> idOffsetDictionary = new Dictionary<BasePartsData, long>();
        private int continuousTargetCount;
        private GameObject summonPerferb;
        //动画
        private Dictionary<BasePartsData, bool> hitOnceDic;
        private Dictionary<UnitCtrl, Dictionary<int, ActionExecedData>> alreadyExecedData;
        private Dictionary<UnitCtrl, List<int>> alreadyExecedKeys;
        private List<BasePartsData> hitOnceKeyList;
        private Dictionary<BasePartsData, List<CriticalData>> criticalDataDictionary = new Dictionary<BasePartsData, List<CriticalData>>();
        private Dictionary<BasePartsData, int> totalDamageDictionary = new Dictionary<BasePartsData, int>();
        private Dictionary<int, float> additionalValue;
        private Dictionary<int, float> multipleValue;
        private Dictionary<int, float> divideValue;
        private SkillAction masterData_mine;
        private eEffectType effectType;
        public bool isSearchAndSorted;

        public int TargetNth { get => targetNth; set => targetNth = value; }
        public int TargetNum { get => targetNum; set => targetNum = value; }
        public float TargetWidth { get => targetWidth; set => targetWidth = value; }
        public DirectionType Direction { get => direction; set => direction = value; }
        public float Position { get => position; set => position = value; }
        public int ActionDetail1 { get => actionDetail1; set => actionDetail1 = value; }
        public int ActionDetail2 { get => actionDetail2; set => actionDetail2 = value; }
        public int ActionDetail3 { get => actionDetail3; set => actionDetail3 = value; }
        public int ActionId { get => actionId; set => actionId = value; }
        public List<BasePartsData> TargetList { get => targetList; set => targetList = value; }
        public Dictionary<int, float> Value { get => value; set => this.value = value; }
        public eActionType ActionType { get => actionType; set => actionType = value; }
        public float[] ExecTime { get => execTime; set => execTime = value; }
        public List<ActionExecTime> ActionExecTimeList { get => actionExecTimeList; set => actionExecTimeList = value; }
        public float ActionWeightSum { get => actionWeightSum; set => actionWeightSum = value; }
        public int DependActionId { get => dependActionId; set => dependActionId = value; }
        public bool ReferencedByEffect { get => referencedByEffect; set => referencedByEffect = value; }
        public bool ReferencedByReflection { get => referencedByReflection; set => referencedByReflection = value; }
        public List<int> ActionChildrenIndexes { get => actionChildrenIndexes; set => actionChildrenIndexes = value; }
        public OnDamageHitDelegate OnDamageHit1 { get => OnDamageHit; set => OnDamageHit = value; }
        public Action OnDefeatEnemy1 { get => OnDefeatEnemy; set => OnDefeatEnemy = value; }
        public Action OnInitWhenNoTarget1 { get => OnInitWhenNoTarget; set => OnInitWhenNoTarget = value; }
        public bool CancelByIfForAll { get => cancelByIfForAll; set => cancelByIfForAll = value; }
        public Dictionary<BasePartsData, long> IdOffsetDictionary { get => idOffsetDictionary; set => idOffsetDictionary = value; }
        public int ContinuousTargetCount { get => continuousTargetCount; set => continuousTargetCount = value; }
        public GameObject SummonPerferb { get => summonPerferb; set => summonPerferb = value; }
        public Dictionary<BasePartsData, bool> HitOnceDic { get => hitOnceDic; set => hitOnceDic = value; }
        public Dictionary<UnitCtrl, Dictionary<int, ActionExecedData>> AlreadyExecedData { get => alreadyExecedData; set => alreadyExecedData = value; }
        public Dictionary<UnitCtrl, List<int>> AlreadyExecedKeys { get => alreadyExecedKeys; set => alreadyExecedKeys = value; }
        public List<BasePartsData> HitOnceKeyList { get => hitOnceKeyList; set => hitOnceKeyList = value; }
        public Dictionary<BasePartsData, List<CriticalData>> CriticalDataDictionary { get => criticalDataDictionary; set => criticalDataDictionary = value; }
        public Dictionary<BasePartsData, int> TotalDamageDictionary { get => totalDamageDictionary; set => totalDamageDictionary = value; }
        public Dictionary<int, float> AdditionalValue { get => additionalValue; set => additionalValue = value; }
        public SkillAction MasterData_mine { get => masterData_mine; set => masterData_mine = value; }
        public eEffectType EffectType { get => effectType; set => effectType = value; }
        public Dictionary<int, float> MultipleValue { get => multipleValue; set => multipleValue = value; }
        public Dictionary<int, float> DivideValue { get => divideValue; set => divideValue = value; }
        public eTargetAssignment TargetAssignment { get => targetAssignment; set => targetAssignment = value; }
        public PirorityPattern TargetSort { get => targetSort; set => targetSort = value; }
        public UnitCtrl Owner { get => owner; set => owner = value; }
        public AbnormalStateFieldAction AbnormalStateFieldAction { get => abnormalStateFieldAction; set => abnormalStateFieldAction = value; }

        //咕咕咕


        public delegate void OnDamageHitDelegate(float damage);
        public delegate void OnActionEndDelegate();

        public void AppendTargetNum(UnitCtrl target, int num)
        {
            if (target == null) { return; }
            if (alreadyExecedData.ContainsKey(target))
            {
                if (!alreadyExecedData[target].ContainsKey(num))
                {
                    ActionExecedData actionExecedData = new ActionExecedData
                    {
                        TargetPartsNumber = 1
                    };
                    alreadyExecedData[target].Add(num, actionExecedData);
                }
                ++alreadyExecedData[target][num].TargetPartsNumber;
            }
            else
            {
                ActionExecedData actionExecedData = new ActionExecedData
                {
                    TargetPartsNumber = 1
                };
                Dictionary<int, ActionExecedData> dic = new Dictionary<int, ActionExecedData>
                {
                    { num, actionExecedData }
                };
                alreadyExecedData.Add(target, dic);
                List<int> list = new List<int>
                {
                    num
                };
                alreadyExecedKeys.Add(target, list);
            }
        }
        public void AppendIsAlreadyexeced(UnitCtrl _target, int _num)
        {
            AlreadyExecedData[_target][_num].AlreadyExeced = true;
        }
        public virtual void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController,
            Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            int index = skill.ActionParmeters.FindIndex(a => a == this) + 1;
            int index2 = skill.ActionParmeters.Count;
            string loging = source.UnitName + "执行技能" + skill.SkillName + "(" + index + "/" + index2 + ")"
                + ",目标" + target.Owner.UnitName;
            BattleUIManager.Instance.LogMessage(loging, eLogMessageType.EXEC_ACTION, source.IsOther);

            //Debug.LogError("不要调用基类的执行函数！");
        }
        public virtual void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {

        }
        public virtual void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {

        }
        public virtual void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
        {
            ResitHitData();
        }
        public virtual void Initialize()
        {
            hitOnceDic = new Dictionary<BasePartsData, bool>();
            hitOnceKeyList = new List<BasePartsData>();
            alreadyExecedData = new Dictionary<UnitCtrl, Dictionary<int, ActionExecedData>>();
            alreadyExecedKeys = new Dictionary<UnitCtrl, List<int>>();
        }
        public virtual void Initialize(UnitCtrl _ownerUnitCtrl)
        {
            Initialize();
        }
        public virtual void SetLevel(float level)
        {

        }
        public bool JudgeLastAndNotExeced(UnitCtrl target, int num)
        {
            ++AlreadyExecedData[target][num].ExecedPartsNumber;
            if (AlreadyExecedData[target][num].AlreadyExeced)
            {
                return false;
            }
            else
            {
                return AlreadyExecedData[target][num].ExecedPartsNumber == AlreadyExecedData[target][num].TargetPartsNumber;
            }
        }
        /// <summary>
        /// 判断是否已经执行了
        /// </summary>
        /// <param name="target"></param>
        /// <param name="num"></param>
        /// <returns>没有执行则返回true</returns>
        public bool JudgeIsAlreadyExeced(UnitCtrl target, int num)
        {
            return !AlreadyExecedData[target][num].AlreadyExeced;
        }
        public void ResitHitData()
        {
            foreach (BasePartsData basePartsData in HitOnceKeyList)
            {
                hitOnceDic[basePartsData] = true;
            }
            foreach (Dictionary<int, ActionExecedData> dic in AlreadyExecedData.Values)
            {
                foreach (int i in dic.Keys)
                {
                    dic[i].AlreadyExeced = false;
                    dic[i].ExecedPartsNumber = 0;
                    dic[i].TargetPartsNumber = 0;
                }
            }
        }

    }
    /// <summary>
    /// 异常状态基类
    /// </summary>
    public class AbnormalStateFieldAction : ActionParameter
    {
        private ActionParameter targetAction;
        private eAbnormalState targetAbnormalState;

        public eAbnormalState TargetAbnormalState { get => targetAbnormalState; set => targetAbnormalState = value; }

        public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<int, float> _valueDictionary)
        {
        }

        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
        }

        public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
        {
        }

        public override void SetLevel(float _level)
        {
        }

    }
    /// <summary>
    /// 攻击类技能基类
    /// </summary>
    public class AttackActionBase : ActionParameter
    {
        private BasePartsData parts;
        //public enum eAttackType { PHYSICAL=1,MAGIC=2,INEVITABLE_PHYSICAL=3}

        //protected DamageData createDamageData(...){...}
        public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<int, float> _valueDictionary)
        {
            //原本就空着
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            //parts = _source.BossPartsList.Find(a=>a.Index == _skill.ParameterTarget);
        }
        public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
        {
            base.ReadyAction(_source, _sourceActionController, _skill);
            foreach (BasePartsData data in HitOnceKeyList)
            {
                HitOnceDic[data] = false;
            }
            CriticalDataDictionary.Clear();
            TotalDamageDictionary.Clear();
        }
        protected virtual float GetCriticalDamageRate(Dictionary<int, float> _valueDictionary) { return 1; }
        protected bool JudgeIsPhysical(eAttacktype _attacktype) { return !(_attacktype == eAttacktype.MAGIC); }
        protected bool JudgeMiss(BasePartsData _target, UnitCtrl _source, int num, UnitActionController _sourceActionController,
            Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, eAttacktype _actionDetail)
        {
            if (BattleManager.Instance.GetCheatValues(eCheatValueType.FORCE_IGNOR_DODGE, _source.IsOther))
            {
                return false;
            }
            int randomInt = BattleManager.Instance.Random(0, 100);
            if (_source.IsPartsBoss)
            {
                Debug.LogError("咕咕咕！");
            }
            int zero = (int)_source.BaseValues.Accuracy;
            if (_skill.CountBlind)//吃到致盲
            {
                if (_skill.CountBlindType == _actionDetail)
                {
                    int dodgeRate = Mathf.RoundToInt(_target.GetDodgeRate(zero) * 100);
                    if (_actionDetail == eAttacktype.PHYSICAL && randomInt < dodgeRate)
                    {
                        _target.Owner.OnDodge?.Invoke();
                    }
                    _target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK_DARK,
                        ActionExecTimeList[num].DamageNumType, ActionExecTimeList[num].DamageNumScale);
                    return true;
                }
            }
            if (_source.IsAbnormalState(eAbnormalState.INHIBIT_HEAL))
            {
                Debug.LogError("禁疗部分鸽了！");
                return true;
            }
            int dodgeRate2 = Mathf.RoundToInt(_target.GetDodgeRate(zero) * 100);
            if (ActionDetail1 != 1 || randomInt >= dodgeRate2)
            {
                if (ActionDetail1 == 1 && _source.IsAbnormalState(eAbnormalState.DARK))
                {
                    int randomInt2 = BattleManager.Instance.Random(0, 100);//黑暗状态再次判定
                    if (randomInt2 >= (_source.GetAbnormalStateMainValue(eAbnormalStateCategory.DARK) / -100) + 1)
                    {
                        return false;
                    }
                    _target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK_DARK, ActionExecTimeList[num].DamageNumType, ActionExecTimeList[num].DamageNumScale);
                    return true;
                }
                return false;
            }
            else
            {
                ActionExecTime actionExecTime = ActionExecTimeList[num];
                _target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK, actionExecTime.DamageNumType, actionExecTime.DamageNumScale);
                _source.OnDodge?.Invoke();
                return true;
            }
        }
        public override void SetLevel(float level)
        {
            Value[1] = MasterData_mine.values[0] + MasterData_mine.values[1] * level;
            Value[3] = MasterData_mine.values[2] + MasterData_mine.values[3] * level;
        }
        protected DamageData CreateDamageData(UnitCtrl source, BasePartsData target, int num, Dictionary<int, float> valueDictionary, eAttacktype actionDetail1, bool isCritical, Skill skill, eActionType actionType)
        {
            if (source.IsPartsBoss)
            {
                Debug.LogError("咕咕咕！");
            }
            float atk, critical, criticalRate;
            int penetrateZero;
            if (actionDetail1 == eAttacktype.PHYSICAL || actionDetail1 == eAttacktype.INEVITABLE_PHYSICAL)
            {
                atk = source.GetAtkZero();
                critical = source.GetPhysicalCriticalZero();
                criticalRate = source.GetPhysicalCriticalDamageRateOrMin();
                penetrateZero = source.PhysicalPenetrateZero;
            }
            else
            {
                atk = source.GetMagicStrZero();
                critical = source.GetMagicCriticalZero();
                criticalRate = source.GetMagicCriticalDamageRateOrMin();
                penetrateZero = source.MagicpenetrateZero;
            }
            float weight = ActionExecTimeList[num].Weight;
            int v78 = Mathf.FloorToInt((valueDictionary[1] + valueDictionary[3] * atk));
            int v79 = Mathf.FloorToInt((valueDictionary[1] + valueDictionary[3] * atk) * weight / ActionWeightSum);
            if (target.Owner.AccumulativeDamageDataDictionary.ContainsKey(source))
            {
                float accdamage = 0;
                AccumulativeDamageData data = target.Owner.AccumulativeDamageDataDictionary[source];
                eAccumulativeDamageType type = data.AccumulativeDamageType;
                if (type == eAccumulativeDamageType.PERCENTAGE)
                {
                    accdamage = data.PercentageValue * (data.DamageCount * v79) / 100;
                }
                else if (type == eAccumulativeDamageType.FIXED)
                {
                    accdamage = data.FixedValue * data.DamageCount;
                }
                data.DamageCount = Mathf.Min(data.CountLimit, data.DamageCount + 1);
                v79 += Mathf.FloorToInt(accdamage);
            }
            DamageData damageData = new DamageData();
            damageData.TotalDamageForLogBarrier = v78;
            damageData.Damage = v79;
            damageData.Target = target;
            damageData.Source = source;
            damageData.DamageType = actionDetail1 == eAttacktype.MAGIC ? eDamageType.MGC : eDamageType.ATK;
            damageData.CriticalRate = isCritical ? 1 : (critical * 0.05f * ((float)source.UnitData.level / target.Owner.UnitData.level) * 0.01f);
            ActionExecTime actionExecTime = ActionExecTimeList[num];
            damageData.DamageEffectType = actionExecTime.DamageNumType;
            damageData.DamageNumScale = actionExecTime.DamageNumScale;
            damageData.DefPenetrate = penetrateZero;
            if (BattleManager.Instance.GetCheatValues(eCheatValueType.FORCE_IGNOR_DEF, source.IsOther))
            {
                damageData.IgnoreDef = true;
            }
            if (source.IsPartsBoss)
            {
                Debug.LogError("咕咕咕！");
            }
            else
            {
                damageData.LifeSteal = source.GetLifeStealZero();
            }
            //爆伤计算有些问题
            damageData.CriticalDamageRate = criticalRate / 100.0f;
            damageData.ActionType = actionType;
            return damageData;
        }
    }
    /// <summary>
    /// 普通的攻击技能
    /// </summary>
    public class AttackAction : AttackActionBase
    {
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            int index = skill.ActionParmeters.FindIndex(a => a == this) + 1;
            int index2 = skill.ActionParmeters.Count;
            string loging = source.UnitName + "执行技能" + skill.SkillName + "(" + index + "/" + index2 + ")"
                + ",目标" + target.Owner.UnitName;

            ++target.UbAttackHitCount1;
            if (!HitOnceDic.ContainsKey(target))
            {
                HitOnceDic.Add(target, false);
                HitOnceKeyList.Add(target);
            }
            if (!JudgeMiss(target, source, num, sourceActionController, skill, starttime, enabledChildAction, (eAttacktype)ActionDetail1))
            {
                bool isForceCritical = valueDictionary[5] == (float)(num + 1);
                //bool isForceCritical = true;
                if (BattleManager.Instance.GetCheatValues(eCheatValueType.FORCE_CRITICAL, source.IsOther))
                {
                    isForceCritical = true;
                }
                DamageData damageData = CreateDamageData(source, target, num, valueDictionary, (eAttacktype)ActionDetail1, isForceCritical, skill, eActionType.ATTACK);
                if (!TotalDamageDictionary.ContainsKey(target))
                {
                    int totalDamege = 0;
                    List<CriticalData> criticalDatas = new List<CriticalData>();
                    for (int i = 0; i < ActionExecTimeList.Count; i++)
                    {
                        CriticalData criticalData = new CriticalData();
                        float randomFloat = (float)BattleManager.Instance.Random(0, 100) / 100.0f;
                        int expectedDamage = Mathf.FloorToInt(ActionExecTimeList[i].Weight * damageData.TotalDamageForLogBarrier / ActionWeightSum);
                        criticalData.ExpectedDamage = expectedDamage;
                        if (randomFloat <= damageData.CriticalRate && damageData.CriticalRate != 0)
                        {
                            criticalData.IsCritical = true;
                            criticalData.ExpectedDamage = Mathf.FloorToInt(expectedDamage * 2 * damageData.CriticalRate);
                        }
                        if (!damageData.IgnoreDef)
                        {
                            float def = 0;
                            if (damageData.DamageType == eDamageType.MGC)
                            {
                                def = target.GetMagicDefZero();
                            }
                            else if (damageData.DamageType == eDamageType.ATK)
                            {
                                def = target.GetDefZero();
                            }
                            if (damageData.DamageType != eDamageType.NONE)
                            {
                                float def_fin = Mathf.Max(0, def - damageData.DefPenetrate);
                                criticalData.ExpectedDamage = Mathf.FloorToInt(criticalData.ExpectedDamage * (100.0f / (100.0f + def_fin)));
                            }
                        }
                        criticalData.CriticalRate = damageData.CriticalDamageRate;
                        totalDamege += criticalData.ExpectedDamage;
                        criticalDatas.Add(criticalData);
                    }
                    CriticalDataDictionary.Add(target, criticalDatas);
                    TotalDamageDictionary.Add(target, totalDamege);
                }
                CriticalData data0 = CriticalDataDictionary[target][num];
                damageData.IsLogBarrierCritical = data0.IsCritical;
                damageData.LogBarrieryexpectedDamage = data0.ExpectedDamage;
                damageData.TotalDamageForLogBarrier = TotalDamageDictionary[target];
                if (target.Owner.SetDamage(damageData, true, ActionId, OnDamageHit1, true, skill, true, OnDefeatEnemy1,
                    false, ActionExecTimeList[num].Weight, ActionWeightSum, null) != 0)
                {
                    HitOnceDic[target] = true;
                }
                if (skill.animationId == eAnimationType.attack)
                {
                    eWeaponSeType weaponSeType;
                    if (source.ToadDatas.Count <= 0)
                    {
                        weaponSeType = source.WeaponSeType;
                    }
                    else
                    {
                        weaponSeType = skill.WeaponType;
                    }
                    target.ShowHitEffect(weaponSeType, skill, source.IsLeftDir);
                }
                if (damageData.IsLogBarrierCritical)
                {
                    loging += ",造成<color=#FFEA00>" + damageData.LogBarrieryexpectedDamage + "</color>点伤害";

                }
                else
                {
                    loging += ",造成" + damageData.LogBarrieryexpectedDamage + "点伤害";
                }
            }
            else
            {
                loging += ",未命中";
            }
            BattleUIManager.Instance.LogMessage(loging, eLogMessageType.EXEC_ACTION, source.IsOther);
        }
        protected override float GetCriticalDamageRate(Dictionary<int, float> _valueDictionary)
        {
            if (_valueDictionary[5] == 0)
            {
                return 1;
            }
            return _valueDictionary[5];
        }
        public override void SetLevel(float level)
        {
            base.SetLevel(level);
        }
    }
    /// <summary>
    /// 移动类，还有BUG
    /// </summary>
    public class MoveAction : ActionParameter
    {
        private enum eMoveType
        {
            [Description("瞬移至目标处，技能结束后返回")]
            TARGET_POS_RETURN = 1,
            [Description("瞬移至指定坐标处，技能结束后返回")]//没有实例
            ABSOLUTE_POS_RETURN = 2,
            [Description("瞬移至目标处")]//只有茉莉
            TARGET_POS = 3,
            [Description("瞬移至指定坐标处")]//只有怪物有
            ABSOLUTE_POS = 4,
            [Description("移动至目标处")]//兔兔和羊驼有
            TARGET_MOVE_BY_VELOSITY = 5,
            [Description("移动至指定坐标处")]//只有怪物有
            ABSOLUTE_MOVE_BY_VELOCITY = 6,
            [Description("移动至指定坐标处，不考虑角色方向")]//只有怪物有
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
            float distance = vector3.x - _target.GetBodyWidth() / 4 - x;
            Vector3 a = Vector3.zero;
            a.x = distance;
            return a;
        }
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            if (ActionDetail2 != 0)
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
            if (ActionDetail1 != 5 && ActionDetail1 != 6)
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
            if (endAction != null)
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
                if (_source.JudgeHasSkillAnimation(_skill.SkillId, 3))
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
                if (_source.SpineController.IsPlayingAnimeBattleComplete ||
                    _source.IsUnableActionState() ||
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
                if (_source.IsCancalActionState(_skill.SkillId) ||
                    (!isUB && _source.IsUnableActionState()))
                {
                    //清除技能特效                    
                }
                else
                {
                    float deltaX = _sub * Owner.DeltaTimeForPause;
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
        public static string GetActionDescription(int detail1)
        {
            return ((eMoveType)detail1).GetDescription();
        }

    }
    /// <summary>
    /// 击退、击飞类技能，只有熊锤有击飞（代码还有BUG），其他都为击退
    /// </summary>
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
                case 1://只有熊锤有的Y轴击飞
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
                return Mathf.Max(value, -1024 + Mathf.Abs(_target.FixedPosition.x));
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
            if (_target.ActionState != eActionState.DAMAGE)
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
                _target.SetPosition(startpos + addpos / 60.0f);
                time_5 += Owner.DeltaTimeForPause;
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
                    time_5 += Owner.DeltaTimeForPause;
                    if (time_5 > duration)
                    {
                        time_5 = duration;
                    }
                    Vector3 addpos = new Vector3(0, knockValue * time_5 / duration, 0);
                    _target.SetPosition(startpos + addpos / 60.0f);
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
                time_5 += Owner.DeltaTimeForPause;
                if (time_5 > duration)
                {
                    time_5 = duration;
                }
                Vector3 minuspos = new Vector3(0, knockValue * time_5 / duration, 0);
                _target.SetPosition(startpos - minuspos / 60.0f);
                yield return null;
            }
        }

        private IEnumerator UpdateParaboricKnock(Vector3 _pos, UnitCtrl _target) =>
            null;
    }
    /// <summary>
    /// 治疗类技能
    /// </summary>
    public class HealAction : ActionParameter
    {
        private const float PERCENT_DIGIT = 100f;
        private const float HP_RECOVER_RATE_BASE = 100f;
        private BasePartsData parts;

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            int valueType = (int)valueDictionary[1];
            float add_hp = 0;
            float hpRecoveryRate = 0;
            if (valueType == 2)
            {
                add_hp = target.Owner.BaseValues.Hp * valueDictionary[2] / 100.0f;

            }
            else if (valueType == 1)
            {
                if (source.IsPartsBoss)
                {
                    Debug.LogError("咕咕咕！");
                }
                else
                {
                    float atk;
                    if (ActionDetail1 == 1)
                    {
                        atk = source.GetAtkZero();
                    }
                    else
                    {
                        atk = source.GetMagicStrZero();
                    }
                    add_hp = atk * valueDictionary[4] + valueDictionary[2];
                }
            }
            if (source.IsPartsBoss)
            {
                Debug.LogError("咕咕咕！");
            }
            else
            {
                hpRecoveryRate = source.BaseValues.Hp_recovery_rate;
            }
            add_hp *= skill.AweValue * (1 + hpRecoveryRate / 100.0f) / ExecTime.Length;
            eInhibitHealType inhibitHealType = ActionDetail1 != 1 ? eInhibitHealType.MAGIC : eInhibitHealType.PHYSICS;
            target.Owner.SetRecovery((int)add_hp, inhibitHealType, source, EffectType == 0, false, false, false, true, target);
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
            //bossPartsListForBattle
        }
        public override void SetLevel(float level)
        {
            base.SetLevel(level);
            Value[2] = MasterData_mine.values[1] + MasterData_mine.values[2] * level;
            Value[4] = MasterData_mine.values[4] + MasterData_mine.values[5] * level;
        }

    }
    /// <summary>
    /// 护盾类技能
    /// </summary>
    public class BarrierAction : ActionParameter
    {
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            AppendIsAlreadyexeced(target.Owner, num);
            int value = BattleUtil.FloatToIntReverseTruncate(valueDictionary[1]);
            switch ((eBarrierType)ActionDetail1)
            {
                case eBarrierType.GUARD_ATK:
                    target.Owner.SetAbnormalState(source, (eAbnormalState)1, valueDictionary[3], this, skill, value, 0, false, 0);
                    break;
                case eBarrierType.GUARD_MGC:
                    target.Owner.SetAbnormalState(source, (eAbnormalState)2, valueDictionary[3], this, skill, value, 0, false, 0);
                    break;
                case eBarrierType.DRAIN_ATK:
                    target.Owner.SetAbnormalState(source, (eAbnormalState)3, valueDictionary[3], this, skill, value, 0, false, 0);
                    break;
                case eBarrierType.DRAIN_MGC:
                    target.Owner.SetAbnormalState(source, (eAbnormalState)4, valueDictionary[3], this, skill, value, 0, false, 0);
                    break;
                case eBarrierType.GUARD_BOTH:
                    target.Owner.SetAbnormalState(source, (eAbnormalState)5, valueDictionary[3], this, skill, value, 0, false, 0);
                    break;
                case eBarrierType.DRAIN_BOTH:
                    target.Owner.SetAbnormalState(source, (eAbnormalState)6, valueDictionary[3], this, skill, value, 0, false, 0);
                    break;

            }
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
        }
        public override void SetLevel(float level)
        {
            base.SetLevel(level);
            Value[1] = MasterData_mine.values[0] + MasterData_mine.values[1] * level;
            Value[3] = MasterData_mine.values[2] + MasterData_mine.values[3] * level;
        }

        private enum eBarrierType
        {
            [Description("抵挡物理攻击")]
            GUARD_ATK = 1,
            [Description("抵挡魔法攻击")]
            GUARD_MGC = 2,
            [Description("吸收物理攻击")]
            DRAIN_ATK = 3,
            [Description("吸收魔法攻击")]
            DRAIN_MGC = 4,
            [Description("抵挡攻击")]
            GUARD_BOTH = 5,
            [Description("吸收攻击")]
            DRAIN_BOTH = 6
        }
    }

    /// <summary>
    /// 加速减速类技能
    /// </summary>
    public class ChangeSpeedAction : ActionParameter
    {
        private static readonly Dictionary<eChangeSpeedType, eAbnormalState> abnormalStateDic;
        private BasePartsData target_displayClass;
        private eAbnormalState abnormalState_displayClass;
        static ChangeSpeedAction()
        {
            abnormalStateDic = new Dictionary<eChangeSpeedType, eAbnormalState> {
                {(eChangeSpeedType)1,(eAbnormalState)11 },
                {(eChangeSpeedType)2,(eAbnormalState)7 },
                {(eChangeSpeedType)3,(eAbnormalState)12 },
                {(eChangeSpeedType)4,(eAbnormalState)13 },
                {(eChangeSpeedType)5,(eAbnormalState)17 },
                {(eChangeSpeedType)6,(eAbnormalState)18 },
                {(eChangeSpeedType)7,(eAbnormalState)19 },
                {(eChangeSpeedType)8,(eAbnormalState)29 },
                {(eChangeSpeedType)9,(eAbnormalState)20 },
                {(eChangeSpeedType)10,(eAbnormalState)39 },
            };
        }
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            target_displayClass = target;
            int randomInt = BattleManager.Instance.Random();
            if (randomInt >= BattleUtil.GetDodgeByLevelDiff(skill.Level, target.Owner.UnitData.level) &&
                TargetAssignment != eTargetAssignment.OWNER_SIDE)
            {
                ActionExecedData execedData = AlreadyExecedData[target.Owner][num];
                if (execedData.ExecedPartsNumber == execedData.TargetPartsNumber)
                {
                    BasePartsData target_0 = null;
                    if (execedData.TargetPartsNumber == 1)
                    {
                        target_0 = target;
                    }
                    target.Owner.SetMissAtk(source, eMissLogType.DODGE_CHANGE_SPEED, eDamageEffectType.NORMAL, target_0, 1);
                }
            }
            else
            {
                AppendIsAlreadyexeced(target.Owner, num);
                eAbnormalState abnormalState = abnormalStateDic[(eChangeSpeedType)ActionDetail1];
                abnormalState_displayClass = abnormalState;
                if (abnormalState == eAbnormalState.HASTE)
                {
                    Dictionary<BasePartsData, int> dic = new Dictionary<BasePartsData, int>
                    {
                        { target, 0 }
                    };
                    target.Owner.SetBuffParam(BuffParamKind.NONE, dic, 0.5f, skill.SkillId, source, true, eEffectType.COMMON, true, false);
                }
                float value_1 = 0;
                if (AbnormalStateFieldAction != null)
                {
                    value_1 = 90;
                }
                else
                {
                    value_1 = valueDictionary[3];
                }
                target.Owner.SetAbnormalState(source, abnormalState, value_1, this, skill, valueDictionary[1], 0, false, 0);
                if (ActionDetail2 == 1)//只有一个怪有
                {
                    if (!target.Owner.OnDamageListForChangeSpeedDisableByAttack.ContainsKey(ActionId))
                    {
                        Action<bool> action = ExecAction;
                        target.Owner.OnDamageListForChangeSpeedDisableByAttack.Add(ActionId, action);
                    }
                }
            }
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
        }
        public override void SetLevel(float level)
        {
            base.SetLevel(level);
            Value[1] = MasterData_mine.values[0] + MasterData_mine.values[1] * level;
            Value[3] = MasterData_mine.values[2] + MasterData_mine.values[3] * level;

        }
        public void ExecAction(bool byAttack)
        {
            if (byAttack)
            {
                if (target_displayClass.Owner.IsAbnormalState(abnormalState_displayClass))
                {
                    target_displayClass.Owner.DisableAbnormalStateById(abnormalState_displayClass, ActionId);
                }
            }
        }

        private enum eCancelTriggerType
        {
            NONE = 0,
            DAMAGED = 1
        }

        public enum eChangeSpeedType
        {
            [Description("减速")]
            SLOW = 1,
            [Description("加速")]
            HASTE = 2,
            [Description("麻痹")]
            PARALYSIS = 3,
            [Description("冻结")]
            FREEZE = 4,
            [Description("束缚")]
            CHAINED = 5,
            [Description("睡眠")]
            SLEEP = 6,
            [Description("击晕")]
            STUN = 7,
            [Description("石化")]
            STONE = 8,
            [Description("拘留")]
            DETAIN = 9,
            [Description("昏厥")]
            FAINT = 10
        }

        public class eChangeSpeedType_DictComparer : IEqualityComparer<ChangeSpeedAction.eChangeSpeedType>
        {
            public bool Equals(ChangeSpeedAction.eChangeSpeedType _x, ChangeSpeedAction.eChangeSpeedType _y) =>
                new bool();

            public int GetHashCode(ChangeSpeedAction.eChangeSpeedType _obj) =>
                new int();
        }
    }
    /// <summary>
    /// DOT伤害类
    /// </summary>
    public class SlipDamageAction : ActionParameter
    {
        private static readonly Dictionary<eSlipDamageType, eAbnormalState> abnormalStateDic;
        static SlipDamageAction()
        {
            abnormalStateDic = new Dictionary<eSlipDamageType, eAbnormalState>
            {
                {(eSlipDamageType)1,(eAbnormalState)8 },
                {(eSlipDamageType)2,(eAbnormalState)9 },
                {(eSlipDamageType)3,(eAbnormalState)10 },
                {(eSlipDamageType)0,(eAbnormalState)21 },
                {(eSlipDamageType)4,(eAbnormalState)33 },
                {(eSlipDamageType)5,(eAbnormalState)38 },
                {(eSlipDamageType)6,(eAbnormalState)41 },

            };
        }
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            int randomInt = BattleManager.Instance.Random();
            if (randomInt < BattleUtil.GetDodgeByLevelDiff(skill.Level, target.Owner.UnitData.level))
            {
                AppendIsAlreadyexeced(target.Owner, num);
                float time = 90;
                if (AbnormalStateFieldAction != null)
                {
                    target.Owner.SetAbnormalState(source, abnormalStateDic[(eSlipDamageType)ActionDetail1], time, this, skill, valueDictionary[1], valueDictionary[5], false, 0);
                    return;
                }
                else
                {
                    time = valueDictionary[3];
                    target.Owner.SetAbnormalState(source, abnormalStateDic[(eSlipDamageType)ActionDetail1], time, this, skill, valueDictionary[1], valueDictionary[5], false, 0);
                    return;
                }
            }
            ActionExecedData acd = AlreadyExecedData[target.Owner][num];
            if (acd.ExecedPartsNumber == acd.TargetPartsNumber)
            {
                target.Owner.SetMissAtk(source, eMissLogType.DODGE_SLIP_DAMAGE, eDamageEffectType.NORMAL, null, 1);
            }
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
        }
        public override void SetLevel(float level)
        {
            Value[1] = MasterData_mine.values[0] + MasterData_mine.values[1] * level;
            Value[3] = MasterData_mine.values[2] + MasterData_mine.values[3] * level;
            Value[5] = MasterData_mine.values[4];
        }

        private enum eSlipDamageType
        {
            NO_EFFECT = 0,
            POISONE = 1,
            BURN = 2,
            CURSE = 3,
            VENOM = 4,
            HEX = 5,
            COMPENSATION = 6
        }

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
            if (_changeParamType == eChangeParameterType.PERCENTAGE)
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
            else if (_changeParamType == eChangeParameterType.FIXED)
            {
                result = BattleUtil.FloatToIntReverseTruncate(_value);
            }
            int result_int = Mathf.CeilToInt(Mathf.Abs(result));
            if (_isDebuf)
            {
                result_int *= -1;
            }
            return result_int;
        }

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            AppendIsAlreadyexeced(target.Owner, num);
            if (ActionDetail2 != 2)//所有玩家角色技能都满足这个条件
            {
                Dictionary<BasePartsData, int> dic = new Dictionary<BasePartsData, int>();
                BuffParamKind paramKind = (BuffParamKind)Mathf.FloorToInt(ActionDetail1 / 10.0f);
                bool isDebuff = (ActionDetail1 % 10) == 1;
                if (target.Owner.IsPartsBoss && paramKind != BuffParamKind.ENERGY_RECOVER_RATE && paramKind != BuffParamKind.MOVE_SPEED)
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
                    );
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
        public static BuffParamKind GetChangeParamKind(int value)
        {
            if (value % 10 == 1)
            {
                value -= 1;
            }
            value = value / 10;
            return (BuffParamKind)value;
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
    /// 致盲类
    /// </summary>
    public class BlindAction : ActionParameter
    {
        private const float PERCENTAGE_TO_RATE = 0.01f;

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            int randomInt = BattleManager.Instance.Random();
            if (randomInt <= valueDictionary[3] * BattleUtil.GetDodgeByLevelDiff(skill.Level, target.Owner.UnitData.level))
            {
                AppendIsAlreadyexeced(target.Owner, num);
                float time = 90;
                if(AbnormalStateFieldAction == null)
                {
                    time = valueDictionary[1];
                }
                target.Owner.SetAbnormalState(source, eAbnormalState.DARK, time, this, skill, ActionDetail1, 0, false, 0);
            }
            else
            {
                ActionExecedData execedData = AlreadyExecedData[target.Owner][num];
                if(execedData.ExecedPartsNumber == execedData.TargetPartsNumber)
                {
                    BasePartsData target_1 = null;
                    if(execedData.TargetPartsNumber == 1)
                    {
                        target_1 = target;
                    }
                    target.Owner.SetMissAtk(source, eMissLogType.DODGE_DARK, eDamageEffectType.NORMAL, target_1, 1);
                }
            }
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);

        }
        public override void SetLevel(float level)
        {
            Value[1] = MasterData_mine.values[0] + MasterData_mine.values[1] * level;
            Value[3] = MasterData_mine.values[2] + MasterData_mine.values[3] * level;
        }
    }

    /// <summary>
    /// 充能类
    /// </summary>
    public class ChargeEnergyAction : ActionParameter
    {
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            float energy = valueDictionary[1];
            if (ActionDetail1 != 1) { energy *= -1; }
            target.Owner.ChargeEnergy(eSetEnergyType.BY_CHANGE_ENERGY, energy, ActionDetail1 == 1, source, true, EffectType == eEffectType.COMMON, true, false);
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
        }
        public override void SetLevel(float level)
        {
            base.SetLevel(level);
            Value[1] = MasterData_mine.values[0] + Mathf.CeilToInt(MasterData_mine.values[1] * level);
        }
    }
    /// <summary>
    /// 抵挡伤害类，布丁和怜专属
    /// </summary>
    public class DamageChargeAction : ActionParameter
    {
        private const int LOOP_MOTION_NUMBER = 1;
        private float defaultValue;
        private ActionParameter targetAction;

        private UnitCtrl _source_b_1;

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            if (ActionDetail1 == 1 || ActionDetail1 == 3)
            {
                source.IsOnDamageCharge = true;
                sourceActionController.CreateNormalPrefabWithTargetMotion(skill, 1, false);
                sourceActionController.StartCoroutine(PlayMotionLoopForDamageCharge(skill.animationId, skill, source, sourceActionController, skill));
            }
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
            _source_b_1 = _source;
            if (ActionDetail1 == 2)//没有怪有这个
            {
                Debug.LogError("咕咕咕！");
                return;
            }
            _source.Ondamage += ExecactionOnStart_b_1;
            if (ActionDetail2 != 0)
            {
                targetAction = _skill.ActionParmeters.Find(a => a.ActionId == ActionDetail2);
                defaultValue = targetAction.Value[1];
            }

        }
        private bool IsActionCancel(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
        {
            if (!_source.IsUnableActionState())
            {
                if (!_source.IsCancalActionState(_skill.SkillId))
                {
                    return false;
                }
            }
            _source.CancalByAwake = false;
            _source.CancalByCovert = false;
            _source.CancalByToad = false;
            _sourceActionController.CancalAction(_skill.SkillId);
            return true;
        }

        private IEnumerator PlayMotionLoopForDamageCharge(eAnimationType animeId, Skill skill, UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
        {
            float timer_5 = 0;
            while (true)
            {
                if (IsActionCancel(_source, _sourceActionController, _skill))
                {
                    yield break;
                }
                if (!_source.SpineController.IsPlayingAnimeBattleComplete)
                {
                    yield return null;
                    continue;
                }
                //播放第二段动画
                _source.PlaySkillAnimation(_skill.SkillId, 1, true);
                //特效
                //结束时特效
                break;
            }
            while (true)
            {
                timer_5 += _source.DeltaTimeForPause;
                if (IsActionCancel(_source, _sourceActionController, _skill))
                {
                    _source.IsOnDamageCharge = false;
                    yield break;
                }
                if (_source.Hp <= 0)
                {
                    _source.SetState(eActionState.IDLE, 0);
                    _sourceActionController.CancalAction(_skill.SkillId);
                    //结束特效
                    yield break;
                }
                if (timer_5 > Value[3])
                {
                    _source.PlaySkillAnimation(_skill.SkillId, 2, false);
                    _sourceActionController.StartCoroutine(WaitMotinEnd(_source, _skill, _sourceActionController));
                    _sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 2, false);
                    //特效
                    _source.IsOnDamageCharge = false;
                    yield break;
                }
                yield return null;
            }
        }

        public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
        {
            base.ReadyAction(_source, _sourceActionController, _skill);
            if (targetAction != null)
            {
                targetAction.Value[1] = defaultValue;
            }
        }
        public override void SetLevel(float level)
        {
            Value[1] = MasterData_mine.values[0] + MasterData_mine.values[1] * level;
        }
        public void ExecactionOnStart_b_1(bool byAttack, float damage, bool critical)
        {
            if (byAttack)
            {
                if (_source_b_1.IsOnDamageCharge)
                {
                    if (ActionDetail2 != 0)
                    {
                        targetAction.Value[1] += Value[1] * damage;
                        string word = _source_b_1.UnitName + "通过无敌躲避了" + damage + "点伤害";
                        BattleUIManager.Instance.LogMessage(word, eLogMessageType.MISS_DAMAGE_BY_NO_DAMAGE, _source_b_1.IsOther);
                    }
                }
            }
        }

        private IEnumerator WaitMotinEnd(UnitCtrl _source, Skill _skill, UnitActionController _sourceActionController)
        {
            while (true)
            {
                if (!IsActionCancel(_source, _sourceActionController, _skill))
                {
                    if (!_source.SpineController.IsPlayingAnimeBattleComplete)
                    {
                        yield return null;
                        continue;
                    }
                    if (_skill.BlackoutEndtWithMotion)
                    {
                        Debug.LogError("特效鸽了");
                    }
                    _source.SkillEndProcess();
                }
                break;
            }
        }

        private enum eDamageChargeType
        {
            PASSIVE_DAMAGE = 1,
            ACTIVE_DAMAGE = 2,
            PASSIVE_DAMAGE_LOOP = 3
        }
    }
    /// <summary>
    /// 闪避无敌类
    /// </summary>
    public class NoDamageAction : ActionParameter
    {
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            AppendIsAlreadyexeced(target.Owner, num);
            switch ((eNoDamageType)ActionDetail1)
            {
                case eNoDamageType.NO_DAMAGE:
                    target.Owner.SetAbnormalState(source, eAbnormalState.NO_DAMAGE_MOTION, valueDictionary[1], this, skill, 0, 0, false, 0);
                    break;
                case eNoDamageType.PHYSICS_DODGE:
                    target.Owner.SetAbnormalState(source, eAbnormalState.PHYSICS_DODGE, valueDictionary[1], this, skill, 0, 0, false, 0);
                    break;
                case eNoDamageType.ALL_DODGE:
                    Debug.LogError("ERROR!");
                    break;
                case eNoDamageType.DEBUFF:
                    target.Owner.SetAbnormalState(source, eAbnormalState.NO_DEBUFF, valueDictionary[1], this, skill, 0, 0, false, 0);
                    break;
                case eNoDamageType.BREAK_POINT:
                    target.Owner.SetAbnormalState(source, eAbnormalState.PARTS_NO_DAMAGE, valueDictionary[1], this, skill, 0, 0, false, 0);
                    break;
            }
        }
        public override void SetLevel(float level)
        {
            Value[1] = MasterData_mine.values[0] + MasterData_mine.values[1] * level;
        }

        private enum eNoDamageType
        {
            NO_DAMAGE = 1,
            PHYSICS_DODGE = 2,
            ALL_DODGE = 3,
            ABNORMAL = 4,
            DEBUFF = 5,
            BREAK_POINT = 6
        }
    }
    /// <summary>
    /// 赋值类技能基类
    /// </summary>
    public class GiveValueAction : ActionParameter, ISingletonField
    {
        protected BasePartsData parts;
        private Skill skill_b_1;


        protected virtual float CalcDamageValue(UnitCtrl _source, Dictionary<int, float> _valueDictionary) =>
            new float();//空着

        protected virtual float CalcHpValue(UnitCtrl _source, Dictionary<int, float> _valueDictionary) =>
            new float();//空着

        private bool ComparePosition(BasePartsData _source, BasePartsData _target, bool _ahead)
        {
            if (_source == _target)
            {
                return false;
            }
            if (_target.Owner.isStealth)
            {
                return false;
            }
            if (!_target.Owner.IsSummonOrPhantom || !_target.Owner.IdleOnly)
            {
                if (Mathf.Abs(_source.GetButtontransformPosition().x - _target.GetButtontransformPosition().x) <= 0.01f)
                {
                    return true;
                }
                if (_source.Owner.IsLeftDir)
                {
                    return _source.GetButtontransformPosition().x > _target.GetButtontransformPosition().x;
                }
                else
                {
                    return _source.GetButtontransformPosition().x < _target.GetButtontransformPosition().x;
                }
            }
            return false;
        }

        protected int CountUnitNumCompareX(List<UnitCtrl> _targetList, BasePartsData _source, bool _ahead)
        {
            int v8 = 0;
            foreach (UnitCtrl unitCtrl in _targetList)
            {
                v8 += ComparePosition(_source, unitCtrl.GetFirstParts(false, 0), _ahead) ? 1 : 0;
            }
            return v8;
        }

        protected virtual void CreateValue(UnitCtrl _source, Skill _skill, Dictionary<int, float> _valueDictionary, Dictionary<int, float> _addValue, int _evalue, BasePartsData _target)
        {
            switch (_valueDictionary[1])//只有1，2，4会被运行到
            {
                case 0:
                    _addValue[_evalue] = CalcHpValue(_source, _valueDictionary);
                    break;
                case 1:
                    _addValue[_evalue] = CalcDamageValue(_source, _valueDictionary);
                    break;
                case 2:
                    _addValue[_evalue] = _skill.DefeatEnemycount * _valueDictionary[2];
                    break;
                case 3:
                    return;
                case 4:
                    _addValue[_evalue] = TargetList.Count * _valueDictionary[2];
                    break;
                case 5:
                    _addValue[_evalue] = _skill.DamageedPartsList.Count * _valueDictionary[2];
                    break;
                case 6:
                    _addValue[_evalue] = _skill.TotalDamage * _valueDictionary[2];
                    break;
                case 7:
                    _addValue[_evalue] = Owner.GetAtkZero() * _valueDictionary[1];
                    break;
                case 8:
                    _addValue[_evalue] = Owner.GetMagicStrZero() * _valueDictionary[1];
                    break;
                case 9:
                    _addValue[_evalue] = Owner.GetDefZero() * _valueDictionary[1];
                    break;
                case 10:
                    _addValue[_evalue] = Owner.GetMagicDefZero() * _valueDictionary[1];
                    break;
                case 11:
                    List<UnitCtrl> unitCtrls = _target.Owner.IsOther ? BattleManager.Instance.EnemiesList : BattleManager.Instance.PlayersList;
                    _addValue[_evalue] = CountUnitNumCompareX(unitCtrls, parts, true) * _valueDictionary[1];
                    break;
                case 12:
                    List<UnitCtrl> unitCtrls_2 = _target.Owner.IsOther ? BattleManager.Instance.EnemiesList : BattleManager.Instance.PlayersList;
                    _addValue[_evalue] = CountUnitNumCompareX(unitCtrls_2, parts, false) * _valueDictionary[1];
                    break;
                default:
                    BattleUIManager.Instance.LogMessage("这部分代码鸽了！", eLogMessageType.ERROR, _source.IsOther);
                    break;


            }
        }

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            Dictionary<int, float> dic = new Dictionary<int, float>();
            dic.Add(ActionDetail2, 0);
            CreateValue(source, skill, valueDictionary, dic, ActionDetail2, target);
            //log
            ActionParameter action = skill.ActionParmeters.Find(a => a.ActionId == ActionDetail1);
            SetValue(dic, action);
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
            skill_b_1 = _skill;
            if (Value[1] == 2)
            {
                if (_skill.ActionParmeters.Count >= 1)
                {
                    ActionParameter actionParameter = _skill.ActionParmeters[0];
                    foreach (ActionParameter parameter in _skill.ActionParmeters)
                    {
                        parameter.OnDefeatEnemy1 += D_1;
                        if (parameter.ActionId == ActionDetail2 || parameter.ActionId == ActionDetail3)
                        {

                        }
                        else
                        {
                            if (actionParameter.ExecTime[actionParameter.ExecTime.Length - 1] > parameter.ExecTime[parameter.ExecTime.Length - 1])
                            {

                            }
                            else
                            {
                                actionParameter = parameter;
                            }
                        }
                    }

                }
            }
            else if (Value[1] < 13 && Value[1] >= 11)//这段代码应该不会被运行到
            {
                parts = _source.GetFirstParts(false, 0);
            }
        }

        protected virtual void SetValue(Dictionary<int, float> _value, ActionParameter _action)
        {
            //空着
        }

        public void D_1()
        {
            skill_b_1.DefeatEnemycount++;
        }



        public enum eAdditiveValueType
        {
            [Description("HP")]
            HP = 0,
            [Description("造成伤害")]
            DAMAGE = 1,
            [Description("击杀单位数量")]
            DEFEAT_NUMBER = 2,
            [Description("？？？")]
            SEAL = 3,
            [Description("目标")]
            TARGET = 4,
            DAMAGED_TARGET = 5,
            SKILL_DAMAGE = 6,
            TARGET_ATK = 7,
            TARGET_MAGIC_ATK = 8,
            TARGET_DEF = 9,
            TARGET_MAGIC_DEF = 10,
            AHEAD_UNIT_NUM = 11,
            BEHIND_UNIT_NUM = 12,
            STRIKE_BACK_COUNT = 100,
            [Description("未知")]
            UNKNOW = 102,
            SEAL_COUNT = 200
        }
    }
    /// <summary>
    /// 赋值类技能——加法赋值
    /// </summary>
    public class GiveValueAdditiveAction : GiveValueAction
    {
        protected override float CalcDamageValue(UnitCtrl _source, Dictionary<int, float> _valueDictionary)
        {
            return (_source.BaseValues.Hp - _source.Hp) * _valueDictionary[2];
        }
        protected override float CalcHpValue(UnitCtrl _source, Dictionary<int, float> _valueDictionary)
        {
            return _source.Hp * _valueDictionary[2];
        }
        public override void SetLevel(float level)
        {
            Value[2] = MasterData_mine.values[1] + MasterData_mine.values[2] * level;
        }
        protected override void SetValue(Dictionary<int, float> _value, ActionParameter _action)
        {
            _action.AdditionalValue = _value;
        }
    }
    /// <summary>
    /// 赋值类技能——乘法赋值
    /// </summary>
    public class GiveValueMultipleAction : GiveValueAction
    {
        protected override float CalcDamageValue(UnitCtrl _source, Dictionary<int, float> _valueDictionary)
        {
            return ((_source.BaseValues.Hp - _source.Hp) / _source.BaseValues.Hp) * _valueDictionary[2];
        }
        protected override float CalcHpValue(UnitCtrl _source, Dictionary<int, float> _valueDictionary)
        {
            return (_source.Hp / _source.BaseValues.Hp) * _valueDictionary[2];
        }
        public override void SetLevel(float level)
        {
            Value[2] = MasterData_mine.values[1] + MasterData_mine.values[2] * level;
        }
        protected override void SetValue(Dictionary<int, float> _value, ActionParameter _action)
        {
            _action.MultipleValue = _value;
        }
    }
    /// <summary>
    /// 排他条件分支
    /// </summary>
    public class IfForAllAction : ActionParameter
    {
        public const int IF_DIGIT = 100;
        public const int IF_DIGIT_COUNT = 10;
        private Skill skill_b_0;
        private UnitCtrl source_b_2;
       
        private void CancelAction(ActionParameter _action, Skill _skill)
        {
            _action.CancelByIfForAll = true;
            //鸽了
            if(_action.GetType() == this.GetType())
            {
                BattleUIManager.Instance.LogMessage("排他条件分支的嵌套取消鸽了！", eLogMessageType.ERROR, Owner.IsOther);
            }
        }

        public void CancelBoth(Skill _skill)
        {
        }

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            if (ActionDetail1 != 1000)//33，1000，1211
            {
                if(ActionDetail1 == 33||ActionDetail1 == 1211)
                {
                    bool k = BattleManager.Instance.Random() < ActionDetail1;
                    if(ActionDetail1 == 1211)
                    {
                        int value = 0;
                        if(target.Owner.SkillExeccountDictionary.TryGetValue(ActionDetail1%10,out value))
                        {
                            k = value <= ActionDetail1 % 10;
                        }
                        else
                        {
                            k = false;
                        }
                    }
                    if (k)
                    {
                        //skill.effectbranchid = values[1];
                        if(ActionDetail3 == 0)
                        {
                            sourceActionController.StartCoroutine(sourceActionController.UpdateBranchMotion(this, skill));
                            return;
                        }
                        ActionParameter action = skill.ActionParmeters.Find(a => a.ActionId == ActionDetail3);
                        CancelAction(action, skill);
                    }
                    else
                    {
                        if (ActionDetail2 == 0)
                        {
                            sourceActionController.StartCoroutine(sourceActionController.UpdateBranchMotion(this, skill));
                            return;
                        }
                        ActionParameter action = skill.ActionParmeters.Find(a => a.ActionId == ActionDetail2);
                        CancelAction(action, skill);
                    }
                    sourceActionController.StartCoroutine(sourceActionController.UpdateBranchMotion(this, skill));
                    return;
                }
                else
                {
                    BattleUIManager.Instance.LogMessage("排他条件分支发生错误！",eLogMessageType.ERROR, Owner.IsOther);
                }
            }
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);
            skill_b_0 = _skill;
            source_b_2 = _source;
            if(ActionDetail1 == 1000)
            {
                ActionParameter actionParameter = _skill.ActionParmeters[0];
                foreach (ActionParameter parameter in _skill.ActionParmeters)
                {
                    parameter.OnDefeatEnemy1 += B_0;

                    if (parameter.ActionId == ActionDetail2 || parameter.ActionId == ActionDetail3)
                    {

                    }
                    else
                    {
                        if (actionParameter.ExecTime[actionParameter.ExecTime.Length - 1] < parameter.ExecTime[parameter.ExecTime.Length - 1])
                        {

                        }
                        else
                        {
                            actionParameter = parameter;
                        }
                    }
                }
                actionParameter.OnActionEnd += B_2;
                actionParameter.OnInitWhenNoTarget1 += B_3;
            }
        }
        public void B_0()
        {
            skill_b_0.DefeatByThisSkill = true;
            //texiao
            if (ActionDetail3 != 0)
            {
                skill_b_0.ActionParmeters.Find(a => a.ActionId == ActionDetail3).CancelByIfForAll = true;
            }
        }
        public void B_2()
        {
            source_b_2.StartCoroutine(WaitFrameEnd(skill_b_0));
        }
        public void B_3()
        {
            if (ActionDetail2 != 0)
            {
                skill_b_0.ActionParmeters.Find(a => a.ActionId == ActionDetail2).CancelByIfForAll = true;
            }

        }
        public static bool JudgeCountableUnit(UnitCtrl _unit) =>
            new bool();

        private IEnumerator WaitFrameEnd(Skill _skill)
        {
            yield return null;

            if (_skill.DefeatByThisSkill)
            {
                yield break;
            }
            if (ActionDetail2 != 0)
            {
                skill_b_0.ActionParmeters.Find(a => a.ActionId == ActionDetail2).CancelByIfForAll = true;
            }

        }



    }
    /// <summary>
    /// 狗拳用伤害累加类
    /// </summary>
    public class AccumulativeDamageAction : ActionParameter
    {
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            AppendIsAlreadyexeced(target.Owner, num);
            //调用了某个回调函数
            target.Owner.SetAbnormalState(source, eAbnormalState.ACCUMULATIVE_DAMAGE, 180, this, skill, 0, 0, false, 0);
            AccumulativeDamageData accumulativeDamage = new AccumulativeDamageData();
            accumulativeDamage.AccumulativeDamageType = (eAccumulativeDamageType)valueDictionary[1];
            if (valueDictionary[1] == 1)
            {
                accumulativeDamage.FixedValue = valueDictionary[2];
            }
            else if (valueDictionary[1] == 2)
            {
                accumulativeDamage.PercentageValue = valueDictionary[2];
            }
            accumulativeDamage.CountLimit = (int)valueDictionary[4];
            if (!target.Owner.AccumulativeDamageDataDictionary.ContainsKey(source))
            {
                target.Owner.AccumulativeDamageDataDictionary.Add(source, accumulativeDamage);
            }
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            //空着
        }
        public override void SetLevel(float level)
        {
            Value[2] = MasterData_mine.values[1] + MasterData_mine.values[2] * level;
            Value[4] = MasterData_mine.values[3] + MasterData_mine.values[4] * level;
        }
    }
    /// <summary>
    /// 领域类
    /// </summary>
    public class ChangeParameterFieldAction : ActionParameter
    {
        private const int DETAIL_DIGIT = 10;
        private const int DETAIL_DEBUFF = 1;

        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            //特效鸽了
            ChangeParameterFieldData data = new ChangeParameterFieldData();
            data.FildType = eFieldType.HEAL;
            data.ExecType = eFieldExecType.NORMAL;
            data.LimitTime = valueDictionary[3];
            data.CenterX = Position + target.Owner.FixedPosition.x;
            data.Size = valueDictionary[5];
            bool k_1 = source.IsOther ^( ActionDetail1 % 10 == 1 ? true : false);
            data.FieldTargetType = k_1 ? eFieldTargetType.ENEMY : eFieldTargetType.PLAYER;
            data.PauseTarget = skill.BlackOutTime <= 0 ? null:source;
            EffectType = (eEffectType)ActionDetail3;
            data.TargetList = new List<BasePartsData>();
            data.Value = BattleUtil.FloatToIntReverseTruncate(valueDictionary[1]);
            data.BuffParamKind = BuffDebuffAction.GetChangeParamKind(ActionDetail1);
            data.ValueType = (BuffDebuffAction.eChangeParameterType)ActionDetail2;
            data.Source = source;
            data.UniqueEffectPrefab = null;
            data.UniqueEffectPrefabLeft = null;
            data.IsBuff = ActionDetail1 % 10 == 1 ? false : true;
            data.AlreadyExecedTargetCount = new Dictionary<UnitCtrl, int>();
            //
            BattleManager.Instance.ExecField(data, ActionId);
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);

        }
        public override void Initialize()
        {
            base.Initialize();
            //loadDebuffFieldEffect
        }

        public override void SetLevel(float level)
        {
            Value[1] = MasterData_mine.values[0] + MasterData_mine.values[1] * level;
            Value[3] = MasterData_mine.values[2] + MasterData_mine.values[3] * level;
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
            List<UnitCtrl> targets = targetlist.FindAll(a => a.Hp > 0 && a.UnitData.unitId != 105201);
            if (targets.Count > 0)
            {
                _source.IdleOnly = true;
                int pos = _source.IsOther ? 1400 : -1400;
                _source.SetPosition(pos);
                //将颜色改为透明
                _sourceActionController.StartCoroutine(UpdateIdle(_source));
            }
            else
            {
                _source.SkillAreaWidthList[_skill.SkillId] = _source.SearchAreaWidth;
            }
        }

        private IEnumerator UpdateIdle(UnitCtrl _source)
        {
            _source.IsMoveSpeedForceZero = true;
            _source.SetLeftDirection(_source.IsOther);
            float time = Value[1];
            while (true)
            {
                time -= Owner.DeltaTimeForPause;
                if (BattleManager.Instance.GameState != eGameBattleState.FIGHTING)
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
                    //_source.SetState(eActionState.WALK, 0);
                    yield break;
                }
            }
        }

    }
    /// <summary>
    /// 计数器
    /// </summary>
    public class SkillExecCountAction : ActionParameter
    {
        public override void ExecAction(UnitCtrl source, BasePartsData target, int num, UnitActionController sourceActionController, Skill skill, float starttime, Dictionary<int, bool> enabledChildAction, Dictionary<int, float> valueDictionary)
        {
            base.ExecAction(source, target, num, sourceActionController, skill, starttime, enabledChildAction, valueDictionary);
            if (!source.SkillExeccountDictionary.ContainsKey(ActionDetail1))
            {
                source.SkillExeccountDictionary.Add(ActionDetail1, 0);
            }
            int count = (int)Mathf.Min(valueDictionary[1], source.SkillExeccountDictionary[ActionDetail1] + 1);
            source.SkillExeccountDictionary[ActionDetail1] = count;
        }
        public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnStart(_skill, _source, _sourceActionController);

        }
        public override void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {
            base.ExecActionOnWaveStart(_skill, _source, _sourceActionController);
            if (_source.SkillExeccountDictionary.ContainsKey(ActionDetail1))
            {
                _source.SkillExeccountDictionary[ActionDetail1] = 0;
            }
        }
        public override void SetLevel(float level)
        {
        }
    }
}

