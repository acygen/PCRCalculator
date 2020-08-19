using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCRCaculator.Battle
{

    public delegate void Action();
    
    public interface ISingletonField
    {

    }
    public class BattleDatas
    {
        
    }
    [System.Serializable]
    public class ToadData
    {
        private float time;
        private bool enable;
        private bool disableByNextToad;
        private BattleUnitBaseSpineController spineController;
        private Vector3 leftDrScale;
        private Vector3 rightDrScale;
        //gugugu
    }
    public class AbnormalConstData
    {
        public bool IsBuff;
        public eStateIconType IconType;

        public AbnormalConstData(bool isBuff, eStateIconType iconType)
        {
            IsBuff = isBuff;
            IconType = iconType;
        }
    }
    [System.Serializable]
    public class Skill
    {
        public bool IsPrincessForm;
        public List<PrincessSkillMovieData> PrincessSkillMovieDataList = new List<PrincessSkillMovieData>();
        private List<ActionParameter> actionParmeters = new List<ActionParameter>();
        public List<ActionParameterOnPrefab> ActionParametersOnPrefab = new List<ActionParameterOnPrefab>();
        public bool ForcePlayNoTarget;
        public int ParameterTarget;
        private float castTime;
        public int SkillId;
        public float skillAreaWidth;
        public eAnimationType animationId;
        public bool Cancel;
        private int skillNum;
        private List<int> hasParentIndexes;
        public float BlackOutTime;
        public bool BlackoutEndtWithMotion;
        public bool ForceComboDamage;
        public float CutInMovieFadeStartTime;
        public float CutInMovieFadeDurationTime;
        public float CutInSkipTime;//如果IsPrincessForm则该项为0
        public int Level;
        public string SkillName = "未命名";
        private Vector3 ownerReturnPosition;
        private bool isModeChange;
        public eSkillMotionType SkillMotionType;
        public bool TrackDamageNum;
        public eWeaponSeType WeaponType;
        public bool PauseStopState;
        //public List<int> BranchIds = new List<int>();

        private int defeatEnemycount;
        private bool defeatByThisSkill;
        private bool alreadyAddAttackSelfSeal;
        private int lifeSteal;
        private bool countBlind;
        private eAttacktype countBlindType;
        private bool hasAttack;
        private List<BasePartsData> damageedPartsList = new List<BasePartsData>();
        private long totalDamage;
        private float aweValue;
        private bool isLifeStealEnabled;

        public int DefeatEnemycount { get => defeatEnemycount; set => defeatEnemycount = value; }
        public bool DefeatByThisSkill { get => defeatByThisSkill; set => defeatByThisSkill = value; }
        public bool AlreadyAddAttackSelfSeal { get => alreadyAddAttackSelfSeal; set => alreadyAddAttackSelfSeal = value; }
        public int LifeSteal { get => lifeSteal; set => lifeSteal = value; }
        public bool CountBlind { get => countBlind; set => countBlind = value; }
        public eAttacktype CountBlindType { get => countBlindType; set => countBlindType = value; }
        public bool HasAttack { get => hasAttack; set => hasAttack = value; }
        public List<BasePartsData> DamageedPartsList { get => damageedPartsList; set => damageedPartsList = value; }
        public long TotalDamage { get => totalDamage; set => totalDamage = value; }
        public float AweValue { get => aweValue; set => aweValue = value; }
        public bool IsLifeStealEnabled { get => isLifeStealEnabled; set => isLifeStealEnabled = value; }
        public float CastTime { get => castTime; set => castTime = value; }
        public int SkillNum { get => skillNum; set => skillNum = value; }
        public List<int> HasParentIndexes { get => hasParentIndexes; set => hasParentIndexes = value; }
        public Vector3 OwnerReturnPosition { get => ownerReturnPosition; set => ownerReturnPosition = value; }
        public bool IsModeChange { get => isModeChange; set => isModeChange = value; }
        public List<ActionParameter> ActionParmeters { get => actionParmeters; set => actionParmeters = value; }

        public void ReadySkill()
        {
            damageedPartsList.Clear();
            totalDamage = 0;
            //Debug.Log("技能准备完毕！");
        }
        public void SetLevel(float level)
        {
            foreach(ActionParameter action in actionParmeters)
            {
                action.SetLevel(level);
            }
        }


    }
    public class PrincessSkillMovieData
    {
        public bool UseFade;
        public float FadeStartTime;
        public float FadeDuration;
    }
    public class ActionParameter
    {
        private static BattleManager staticBattleManager;
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
        //异常状态
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

        //咕咕咕


        public delegate void OnDamageHitDelegate(float damage);
        public delegate void OnActionEndDelegate();

        public void AppendTargetNum(UnitCtrl target,int num)
        {
            if(target == null) { return; }
            if (alreadyExecedData.ContainsKey(target))
            {
                if (!alreadyExecedData[target].ContainsKey(num))
                {
                    ActionExecedData actionExecedData = new ActionExecedData
                    {
                        TargetPartsNumber = 1
                    };
                    alreadyExecedData[target].Add(num,actionExecedData);
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
        public void AppendIsAlreadyexeced(UnitCtrl _target,int _num)
        {

        }
        public virtual void ExecAction(UnitCtrl source,BasePartsData target,int num,UnitActionController sourceActionController,
            Skill skill,float starttime,Dictionary<int,bool> enabledChildAction,Dictionary<int,float> valueDictionary)
        {
            int index = skill.ActionParmeters.FindIndex(a => a == this) + 1;
            int index2 = skill.ActionParmeters.Count;
            string loging = source.UnitName + "执行技能" + skill.SkillName + "(" + index + "/" + index2 + ")"
                + ",目标" + target.Owner.UnitName;
            Debug.Log(loging);
            BattleUIManager.Instance.LogMessage(loging, source.IsOther);

            //Debug.LogError("不要调用基类的执行函数！");
        }
        public virtual void ExecActionOnStart(Skill _skill,UnitCtrl _source,UnitActionController _sourceActionController)
        {
            
        }
        public virtual void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
        {

        }
        public virtual void ReadyAction(UnitCtrl _source,UnitActionController _sourceActionController,Skill _skill)
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
        public bool JudgeLastAndNotExeced(UnitCtrl target,int num)
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
        public bool JudgeIsAlreadyExeced(UnitCtrl target,int num)
        {
            return !AlreadyExecedData[target][num].AlreadyExeced;
        }
        public void ResitHitData()
        {
            foreach(BasePartsData basePartsData in HitOnceKeyList)
            {
                hitOnceDic[basePartsData] = true;
            }
            foreach(Dictionary<int,ActionExecedData> dic in AlreadyExecedData.Values)
            {
                foreach(int i in dic.Keys)
                {
                    dic[i].AlreadyExeced = false;
                    dic[i].ExecedPartsNumber = 0;
                    dic[i].TargetPartsNumber = 0;
                }
            }
        }

    }
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
            foreach(BasePartsData data in HitOnceKeyList)
            {
                HitOnceDic[data] = false;
            }
            CriticalDataDictionary.Clear();
            TotalDamageDictionary.Clear();
        }
        protected virtual float GetCriticalDamageRate(Dictionary<int, float> _valueDictionary) { return 1; }
        protected bool JudgeIsPhysical(eAttacktype _attacktype) { return !(_attacktype == eAttacktype.MAGIC); }
        protected bool JudgeMiss(BasePartsData _target,UnitCtrl _source,int num,UnitActionController _sourceActionController,
            Skill _skill,float _starttime,Dictionary<int,bool> _enabledChildAction,eAttacktype _actionDetail)
        {
            int randomInt = BattleManager.Instance.Random(0, 100);
            if (_source.IsPartsBoss)
            {
                Debug.LogError("咕咕咕！");
            }
            int zero = (int)_source.BaseValues.Accuracy;
            if (_skill.CountBlind)//吃到致盲
            {
                if(_skill.CountBlindType == _actionDetail)
                {
                    int dodgeRate = Mathf.RoundToInt(_target.GetDodgeRate(zero)*100);
                    if(_actionDetail == eAttacktype.PHYSICAL && randomInt < dodgeRate)
                    {
                        _target.Owner.OnDodge?.Invoke();
                    }
                    _target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK_DARK,
                        ActionExecTimeList[num].DamageNumType,ActionExecTimeList[num].DamageNumScale);
                    return true;
                }
            }
            if (_source.IsAbnormalState(eAbnormalState.INHIBIT_HEAL))
            {
                Debug.LogError("禁疗部分鸽了！");
                return true;
            }
            int dodgeRate2 = Mathf.RoundToInt(_target.GetDodgeRate(zero) * 100);
            if(ActionDetail1 != 1 || randomInt >= dodgeRate2)
            {
                if(ActionDetail1 == 1 && _source.IsAbnormalState(eAbnormalState.DARK))
                {
                    int randomInt2 = BattleManager.Instance.Random(0, 100);//黑暗状态再次判定
                    if (randomInt2 >= (_source.GetAbnormalStateMainValue(eAbnormalStateCategory.DARK) / -100) + 1)
                    {
                        return false;
                    }
                    _target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK_DARK,ActionExecTimeList[num].DamageNumType, ActionExecTimeList[num].DamageNumScale);
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
        protected  DamageData CreateDamageData(UnitCtrl source,BasePartsData target,int num,Dictionary<int,float> valueDictionary,eAttacktype actionDetail1,bool isCritical,Skill skill,eActionType actionType)
        {
            if (source.IsPartsBoss)
            {
                Debug.LogError("咕咕咕！");
            }
            float atk, critical, criticalRate;
            int penetrateZero;
            if(actionDetail1 == eAttacktype.PHYSICAL || actionDetail1 == eAttacktype.INEVITABLE_PHYSICAL)
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
                if(type == eAccumulativeDamageType.PERCENTAGE)
                {
                    accdamage = data.PercentageValue * (data.DamageCount * v79) / 100;
                }
                else if(type == eAccumulativeDamageType.FIXED)
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
            damageData.DamageType = actionDetail1 == eAttacktype.MAGIC ? eDamageType.ATK : eDamageType.MGC;
            damageData.CriticalRate = isCritical ? 1 : (critical * 0.05f * ((float)source.UnitData.level / target.Owner.UnitData.level) * 0.01f);
            ActionExecTime actionExecTime = ActionExecTimeList[num];
            damageData.DamageEffectType = actionExecTime.DamageNumType;
            damageData.DamageNumScale = actionExecTime.DamageNumScale;
            damageData.DefPenetrate = penetrateZero;
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
                bool v23 = valueDictionary[4] == (float)(num + 1);
                DamageData damageData = CreateDamageData(source, target, num, valueDictionary, (eAttacktype)ActionDetail1, v23, skill,eActionType.ATTACK);
                if (!TotalDamageDictionary.ContainsKey(target))
                {
                    int totalDamege = 0;
                    List<CriticalData> criticalDatas = new List<CriticalData>();
                    for(int i = 0; i < ActionExecTimeList.Count; i++)
                    {
                        CriticalData criticalData = new CriticalData();
                        float randomFloat = (float)BattleManager.Instance.Random(0, 100)/100.0f;
                        int expectedDamage = Mathf.FloorToInt(ActionExecTimeList[i].Weight * damageData.TotalDamageForLogBarrier / ActionWeightSum);
                        criticalData.ExpectedDamage = expectedDamage;
                        if (randomFloat <= damageData.CriticalRate && damageData.CriticalRate !=0)
                        {
                            criticalData.IsCritical = true;
                            criticalData.ExpectedDamage = Mathf.FloorToInt(expectedDamage * 2 * damageData.CriticalRate);
                        }
                        if (!damageData.IgnoreDef)
                        {
                            float def = 0;
                            if(damageData.DamageType == eDamageType.MGC)
                            {
                                def = target.GetMagicDefZero();
                            }
                            else if(damageData.DamageType == eDamageType.ATK)
                            {
                                def = target.GetDefZero();
                            }
                            if(damageData.DamageType != eDamageType.NONE)
                            {
                                float def_fin = Mathf.Max(0,def - damageData.DefPenetrate);
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
                if(target.Owner.SetDamage(damageData,true,ActionId,OnDamageHit1,true,skill,true,OnDefeatEnemy1,
                    false,ActionExecTimeList[num].Weight,ActionWeightSum,null) != 0)
                {
                    HitOnceDic[target] = true;
                }
                if(skill.animationId == eAnimationType.attack)
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
            Debug.Log(loging);
            BattleUIManager.Instance.LogMessage(loging, source.IsOther);
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
    public class AccumulativeDamageData
    {
        private eAccumulativeDamageType accumulativeDamageType;
        private float fixedValue;
        private float percentageValue;
        private int damageCount;
        private int countLimit;

        public eAccumulativeDamageType AccumulativeDamageType { get => accumulativeDamageType; set => accumulativeDamageType = value; }
        public float FixedValue { get => fixedValue; set => fixedValue = value; }
        public float PercentageValue { get => percentageValue; set => percentageValue = value; }
        public int DamageCount { get => damageCount; set => damageCount = value; }
        public int CountLimit { get => countLimit; set => countLimit = value; }
    }
    [System.Serializable]
    public class ActionParameterOnPrefab
    {
        //public Data data;
        public bool Visible;
        public eActionType ActionType;
        public List<ActionParameterOnPerferbDetail> Details;
        //public AnimationCurve KnockAnimationCurve
        //public AnimationCurve KnockDownAnimationCurve
        public eEffectType EffectType;

        /*public sealed class Data
        {
            public bool Visible;
            public eActionType ActionType;
            public List<ActionParameterOnPerferbDetail> Details;
            //public AnimationCurve KnockAnimationCurve
            //public AnimationCurve KnockDownAnimationCurve
            public eEffectType EffectType;

        }*/
    }
    [System.Serializable]
    public class ActionParameterOnPerferbDetail
    {
        //public Data data;
        public bool Visible;
        public List<ActionExecTime> execTime;
        public List<ActionExecTime> ExecTimeForPrefab;//ExecTimeForPrefab
        public List<ActionExecTimeCombo> ExecTimeCombo;
        public int ActionId;
        //public List<NormalSkillEffect> ActionEffectList;

        public List<ActionExecTime> ExecTime { get => execTime; set => execTime = value; }
        /*public sealed class Data
        {
            public bool Visible;
            public List<ActionExecTime> execTime;
            public List<ActionExecTime> ExecTimeForPrefab;//ExecTimeForPrefab
            public List<ActionExecTimeCombo> ExecTimeCombo;
            public int ActionId;
            //public List<NormalSkillEffect> ActionEffectList;

        }*/
    }
    public class ActionExecTimeCombo
    {

    }
    public class AttackSealData
    {
        private float limitTime;
        private eStateIconType iconType;
        private int actionid;
        private bool onlyCritical;

        public float LimitTime { get => limitTime; set => limitTime = value; }
        public eStateIconType IconType { get => iconType; set => iconType = value; }
        public int Actionid { get => actionid; set => actionid = value; }
        public bool OnlyCritical { get => onlyCritical; set => onlyCritical = value; }
    }
    [System.Serializable]
    public class BasePartsData
    {
        public float PositionX;
        public float PositionY;
        //public List<string> AttachmentNamePairList;
        public float ChangeAttachmentStartTime;
        public float ChangeAttachmentEndTime;
        //特效列表
        private float initialPositionX;
        public float BodyWidthValue;
        public int Index;
        public int EnemyId;
        private UnitCtrl owner;
        private float totalDamage;
        private int UbAttackHitCount;
        private bool passiveUbIsMagic;
        private float lastHealEffectTime;
        private bool isBlackOutTarget;
        private Dictionary<eActionType, Dictionary<int, int>> resisiStatusDictionary = new Dictionary<eActionType, Dictionary<int, int>>();

        public float InitialPositionX { get => initialPositionX; set => initialPositionX = value; }
        public UnitCtrl Owner { get => owner; set => owner = value; }
        public float TotalDamage { get => totalDamage; set => totalDamage = value; }
        public BattleManager BattleManager { get => BattleManager.Instance; }
        public int UbAttackHitCount1 { get => UbAttackHitCount; set => UbAttackHitCount = value; }
        public bool PassiveUbIsMagic { get => passiveUbIsMagic; set => passiveUbIsMagic = value; }
        public float LastHealEffectTime { get => lastHealEffectTime; set => lastHealEffectTime = value; }
        public bool IsBlackOutTarget { get => isBlackOutTarget; set => isBlackOutTarget = value; }
        public Dictionary<eActionType, Dictionary<int, int>> ResisiStatusDictionary { get => resisiStatusDictionary; set => resisiStatusDictionary = value; }
        
        public virtual bool GetTargetable()
        {
            //Debug.LogError("咕咕咕！");
            return true;
        }
        public virtual Vector3 GetPosition()
        {
            return owner.FixedPosition;
        }
        public virtual float GetBodyWidth()
        {
            return owner.BodyWidth;
        }
        public virtual Vector3 GetButtontransformPosition()
        {
            return owner.FixedPosition;
        }
        public float GetDodgeRate(int accuracy)
        {
            return owner.GetDodgeRate(accuracy);
        }
        public virtual float GetDefZero()
        {
            return owner.GetDefZero();
        }
        public virtual float GetMagicDefZero()
        {
            return owner.GetMagicDefZero();
        } 
        public void SetMissAtk(UnitCtrl source,eMissLogType missLogType,eDamageEffectType damageEffectType,float scale)
        {
            owner.SetMissAtk(source, missLogType, damageEffectType, null, scale);
        }
        public void InitializeResistStatuts(int resistId)
        {
            //抗性
            //鸽了，PVP没有这个
            Dictionary<eActionType, Dictionary<int, int>> resistDic = new Dictionary<eActionType, Dictionary<int, int>>();
            ResistData unitResistdata = new ResistData(resistId);
            for(int i = 0; i < 20; i++)
            {
                AilmentData ailment = StaticAilmentData.ailmentDatas[i];
                if (!resistDic.ContainsKey((eActionType)ailment.ailment_action))
                {
                    resistDic.Add((eActionType)ailment.ailment_action, new Dictionary<int, int>());
                }
                resistDic[(eActionType)ailment.ailment_action].Add(ailment.ailment_detail1, unitResistdata.ailments[i]);
            }
            ResisiStatusDictionary = resistDic;
        }
        public bool ResistStatus(eActionType actionType,int detail1,UnitCtrl source,bool last,bool targetOneParts,BasePartsData target)
        {
            if (!ResisiStatusDictionary.ContainsKey(actionType))
            {
                return false;
            }
            Dictionary<int, int> rateDic = ResisiStatusDictionary[actionType];
            if (rateDic.ContainsKey(-1))
            {
                detail1 = 1;
            }
            if (!rateDic.ContainsKey(detail1))
            {
                return false;
            }
            int randomInt = BattleManager.Random(0, 100);
            int rate = rateDic[detail1];
            if(randomInt<rate && last)
            {
                owner.SetMissAtk(source, eMissLogType.DODGE_ATTACK, eDamageEffectType.NORMAL,
                    targetOneParts ? target : null, 1);
                return true;
            }
            return randomInt < rate;
        }
        public void ShowHitEffect(eWeaponSeType weaponSeType,Skill skill,bool isLeft)
        {
            Owner.ShowHitEffect(weaponSeType, skill, isLeft, this);
        }
    }
    public class AbnormalStateCategoryData
    {
        public eAbnormalState CurrentAbnormalState;
        public bool enable;
        public float MainValue;
        private float subValue;
        private float energyReduceRate;
        private bool isEnergyReduceMode;
        public float Time;
        public float Duration;
        public List<GameObject> Effects;
        public int ActionId;
        private Skill skill;
        private UnitCtrl source;

        public float SubValue { get => subValue; set => subValue = value; }
        public float EnergyReduceRate { get => energyReduceRate; set => energyReduceRate = value; }
        public bool IsEnergyReduceMode { get => isEnergyReduceMode; set => isEnergyReduceMode = value; }
        public Skill Skill { get => skill; set => skill = value; }
        public UnitCtrl Source { get => source; set => source = value; }
    }
    public class FirearmCtrl
    {

    }
    [System.Serializable]
    public class ActionExecTime
    {
        public float Time;
        public eDamageEffectType DamageNumType;
        public float Weight;
        public float DamageNumScale;
    }
    [System.Serializable]
    public class ActionExecedData
    {
        public bool AlreadyExeced;
        public int TargetPartsNumber;
        public int ExecedPartsNumber;
    }
    [System.Serializable]
    public class CriticalData
    {
        public bool IsCritical;
        public float CriticalRate;
        public int ExpectedDamage;
    }
    public class DamageData
    {
        private BasePartsData target;
        private long totalDamageForLogBarrier;
        private bool isLogBarrierCritical;
        private long logBarrieryexpectedDamage;
        private long damage;
        private UnitCtrl source;
        private float criticalRate;
        private eDamageType damageType;
        private eDamageEffectType damageEffectType;
        private int defPenetrate;
        private eActionType actionType;
        private bool ignoreDef;
        private bool isDivisionDamage;
        private int lifeSteal;
        private bool isSlipDamage;
        private float criticalDamageRate;
        private bool damageNumMagic;
        private float damageNumScale;

        public BasePartsData Target { get => target; set => target = value; }
        public long TotalDamageForLogBarrier { get => totalDamageForLogBarrier; set => totalDamageForLogBarrier = value; }
        public bool IsLogBarrierCritical { get => isLogBarrierCritical; set => isLogBarrierCritical = value; }
        public long LogBarrieryexpectedDamage { get => logBarrieryexpectedDamage; set => logBarrieryexpectedDamage = value; }
        public long Damage { get => damage; set => damage = value; }
        public UnitCtrl Source { get => source; set => source = value; }
        public float CriticalRate { get => criticalRate; set => criticalRate = value; }
        public eDamageType DamageType { get => damageType; set => damageType = value; }
        public eDamageEffectType DamageEffectType { get => damageEffectType; set => damageEffectType = value; }
        public int DefPenetrate { get => defPenetrate; set => defPenetrate = value; }
        public eActionType ActionType { get => actionType; set => actionType = value; }
        public bool IgnoreDef { get => ignoreDef; set => ignoreDef = value; }
        public bool IsDivisionDamage { get => isDivisionDamage; set => isDivisionDamage = value; }
        public int LifeSteal { get => lifeSteal; set => lifeSteal = value; }
        public bool IsSlipDamage { get => isSlipDamage; set => isSlipDamage = value; }
        public float CriticalDamageRate { get => criticalDamageRate; set => criticalDamageRate = value; }
        public bool DamageNumMagic { get => damageNumMagic; set => damageNumMagic = value; }
        public float DamageNumScale { get => damageNumScale; set => damageNumScale = value; }
    }
    public class NormalSkillEffect
    {

    }
    public static class StaticAilmentData
    {
        public static readonly AilmentData[] ailmentDatas;
        static StaticAilmentData()
        {
            ailmentDatas = new AilmentData[20]{
                new AilmentData(1,8,1,"减速"),
                new AilmentData(2,8,2,"加速"),
                new AilmentData(3,8,3,"麻痹"),
                new AilmentData(4,8,4,"冻结"),
                new AilmentData(5,8,5,"束缚"),
                new AilmentData(6,8,6,"睡眠"),
                new AilmentData(7,8,7,"眩晕"),
                new AilmentData(8,8,8,"石化"),
                new AilmentData(9,8,9,"拘留"),
                new AilmentData(10,9,0,"拘留（造成伤害）"),
                new AilmentData(11,9,1,"毒"),
                new AilmentData(12,9,2,"烧伤"),
                new AilmentData(13,9,3,"诅咒"),
                new AilmentData(14,11,0,"魅惑"),
                new AilmentData(15,12,-1,"黑暗"),
                new AilmentData(16,13,0,"沉默"),
                new AilmentData(17,30,0,"即死"),
                new AilmentData(18,3,-1,"击退"),
                new AilmentData(19,11,1,"混乱"),
                new AilmentData(20,9,4,"猛毒"),
            };
        }
    }
    public class AilmentData
    {
        public readonly int ailment_id;
        public readonly int ailment_action;
        public readonly int ailment_detail1;
        public readonly string ailment_name;

        public AilmentData() { }
        public AilmentData(int ailment_id, int ailment_action, int ailment_detail1,string name)
        {
            this.ailment_id = ailment_id;
            this.ailment_action = ailment_action;
            this.ailment_detail1 = ailment_detail1;
            ailment_name = name;
        }
    }
    public class ResistData
    {
        public readonly int resist_ststus_id;
        public readonly int[] ailments;
        public ResistData()
        {
            resist_ststus_id = 0;
            ailments = new int[20]
            {
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            };
        }
        public ResistData(int id,int[] ails)
        {
            resist_ststus_id = id;
            ailments = ails;
        }
        public ResistData(int cent)
        {
            resist_ststus_id = cent;
            ailments = new int[20]
            {
                cent,cent,cent,cent,cent,
                cent,cent,cent,cent,cent,
                cent,cent,cent,cent,cent,
                cent,cent,cent,cent,cent
            };
        }

    }

    public class UnitDamageInfo
    {
        private int viewer_id;
        private int unit_id;
        private int damage;
        private int rarity;
        //private SkinData skin_data;

        public int Viewer_id { get => viewer_id; set => viewer_id = value; }
        public int Unit_id { get => unit_id; set => unit_id = value; }
        public int Damage { get => damage; set => damage = value; }
        public int Rarity { get => rarity; set => rarity = value; }

        public void SetDamage(int damage)
        {
            this.damage = damage;
        }
    }




    public class MyList<T>
    {
        public List<T> data;
    }    
    [System.Serializable]
    public class UnitActionController4Json
    {
        public MyActionParameterOnPerferbDetail AttackDetail;
        public bool UseDefaultDelay;
        public MySkill Attack;
        public List<Skill4List> UnionBurstList;
        public List<Skill4List> MainSkillList;
        public List<Skill4List> UnionBurstEvolutionList;
        public List<Skill4List> MainSkillEvolutionList;
        public MySkill Annihilation;

        public List<Skill> GetUBList()
        {
            List<Skill> a = new List<Skill>();
            foreach(Skill4List b in UnionBurstList)
            {
                a.Add(b.data.CopyThis());
            }
            return a;
        }
        public List<Skill> GetUBEvList()
        {
            List<Skill> a = new List<Skill>();
            foreach (Skill4List b in UnionBurstEvolutionList)
            {
                a.Add(b.data.CopyThis());
            }
            return a;
        }
        public List<Skill> GetMainSkillList()
        {
            List<Skill> a = new List<Skill>();
            foreach (Skill4List b in MainSkillList)
            {
                a.Add(b.data.CopyThis());
            }
            return a;
        }
        public List<Skill> GetMainSkillEvList()
        {
            List<Skill> a = new List<Skill>();
            foreach (Skill4List b in MainSkillEvolutionList)
            {
                a.Add(b.data.CopyThis());
            }
            return a;
        }





        [System.Serializable]
        public sealed class Skill4List
        {
            public MySkill data;
        }
        [System.Serializable]
        public class MySkill
        {
            public bool IsPrincessForm;
            public List<PrincessSkillMovieData> PrincessSkillMovieDataList = new List<PrincessSkillMovieData>();
            public List<MyActionParameterOnPrefab4List> ActionParametersOnPrefab = new List<MyActionParameterOnPrefab4List>();

            public bool ForcePlayNoTarget;
            public int ParameterTarget;
            public eAnimationType animationId;
            public float BlackOutTime;
            public bool BlackoutEndtWithMotion;
            public bool ForceComboDamage;
            public float CutInMovieFadeStartTime;
            public float CutInMovieFadeDurationTime;
            public float CutInSkipTime;
            public eSkillMotionType SkillMotionType;
            public bool TrackDamageNum;
            public bool PauseStopState;

            public List<ActionParameterOnPrefab> GetActionParameterOnPrefab()
            {
                List<ActionParameterOnPrefab> a = new List<ActionParameterOnPrefab>();
                foreach(MyActionParameterOnPrefab4List b in ActionParametersOnPrefab)
                {
                    a.Add(b.data.CopyThis());
                }
                return a;
            }
            public Skill CopyThis()
            {
                Skill s = new Skill();
                s.IsPrincessForm = IsPrincessForm;
                s.PrincessSkillMovieDataList = PrincessSkillMovieDataList;
                s.ActionParametersOnPrefab = GetActionParameterOnPrefab();
                s.ForcePlayNoTarget = ForcePlayNoTarget;
                s.ParameterTarget = ParameterTarget;
                s.animationId = animationId;
                s.BlackOutTime = BlackOutTime;
                s.BlackoutEndtWithMotion = BlackoutEndtWithMotion;
                s.ForceComboDamage = ForceComboDamage;
                s.CutInMovieFadeStartTime = CutInMovieFadeStartTime;
                s.CutInMovieFadeDurationTime = CutInMovieFadeDurationTime;
                s.CutInSkipTime = CutInSkipTime;
                s.SkillMotionType = SkillMotionType;
                s.TrackDamageNum = TrackDamageNum;
                s.PauseStopState = PauseStopState;
                return s;
            }
        }
        [System.Serializable]
        public class MyActionParameterOnPrefab4List
        {
            public MyActionParameterOnPrefab data;
        }
        [System.Serializable]
        public class MyActionParameterOnPrefab
        {
            public bool Visible;
            public eActionType ActionType;
            public List<MyActionParameterOnPerferbDetail4List> Details;
            public eEffectType EffectType;

            public ActionParameterOnPrefab CopyThis()
            {
                ActionParameterOnPrefab ac = new ActionParameterOnPrefab();
                ac.Visible = Visible;
                ac.ActionType = ActionType;
                ac.EffectType = EffectType;
                ac.Details = new List<ActionParameterOnPerferbDetail>();
                foreach(MyActionParameterOnPerferbDetail4List a in Details)
                {
                    ac.Details.Add(a.data.CopyThis());
                }
                return ac;
            }
        }
        [System.Serializable]
        public class MyActionParameterOnPerferbDetail4List
        {
            public MyActionParameterOnPerferbDetail data;
        }
        [System.Serializable]
        public class MyActionParameterOnPerferbDetail
        {
            public bool Visible;
            public List<ActionExecTime4List> ExecTimeForPrefab;//ExecTimeForPrefab
            public List<ActionExecTimeCombo> ExecTimeCombo;
            public int ActionId;
            public ActionParameterOnPerferbDetail CopyThis()
            {
                ActionParameterOnPerferbDetail ac = new ActionParameterOnPerferbDetail();
                ac.Visible = Visible;
                ac.ExecTimeForPrefab = new List<ActionExecTime>();
                foreach(ActionExecTime4List a in ExecTimeForPrefab)
                {
                    ac.ExecTimeForPrefab.Add(a.data);
                }
                ac.ActionId = ActionId;
                return ac;
            }
        }

        [System.Serializable]
        public class ActionExecTime4List
        {
            public ActionExecTime data;
        }
    }
}
