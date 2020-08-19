using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PCRCaculator.Battle
{

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

        public float Time { get => time; set => time = value; }
        public bool Enable { get => enable; set => enable = value; }
        public bool DisableByNextToad { get => disableByNextToad; set => disableByNextToad = value; }
        public BattleUnitBaseSpineController SpineController { get => spineController; set => spineController = value; }
        public Vector3 LeftDrScale { get => leftDrScale; set => leftDrScale = value; }
        public Vector3 RightDrScale { get => rightDrScale; set => rightDrScale = value; }
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
    public class AbnormalStateEffectPrefabData{ }
        [System.Serializable]
    public class Skill
    {
        public bool IsPrincessForm;
        public List<PrincessSkillMovieData> PrincessSkillMovieDataList = new List<PrincessSkillMovieData>();
        private List<ActionParameter> actionParmeters = new List<ActionParameter>();
        public List<ActionParameterOnPrefab> ActionParametersOnPrefab = new List<ActionParameterOnPrefab>();
        public List<NormalSkillEffect> SkillEffects;
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
            Level = (int)level;
            foreach (ActionParameter action in actionParmeters)
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
        public float StartTime;
        public float OffsetTime;
        public float Weight;
        public int Count;
        public eComboInterporationType InterporationType;
        public object Cuve;
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
        public void AddSeal(UnitCtrl target)
        {
            Debug.LogError("咕咕咕！");
        }
    }
    public abstract class Attachment
    {
        private string name;

        public Attachment(string name)
        {
            this.Name = name;
        }

        public string Name { get => name; set => name = value; }

        public override string ToString() => Name;

        
    }
    [Serializable]
    public class AttachmentNamePair
    {
        public string TargetSlotName;
        public string TargetAttachmentName;
        public string AppliedSlotName;
        public string AppliedAttachmentName;
        private int targetIndex;
        private Attachment targetAttachment;
        private Attachment appliedAttachment;

        public int TargetIndex { get => targetIndex; set => targetIndex = value; }
        public Attachment TargetAttachment { get => targetAttachment; set => targetAttachment = value; }
        public Attachment AppliedAttachment { get => appliedAttachment; set => appliedAttachment = value; }
    }

    [System.Serializable]
    public class BasePartsData
    {
        public float PositionX;
        public float PositionY;
        public List<AttachmentNamePair> AttachmentNamePairList;
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
        public virtual Vector3 GetFixedCenterPos()
        {
            return Vector3.zero;
        }
        public virtual Vector3 GetColliderCenter()
        {
            return owner.ColliderCenter;
        }
        public virtual Vector3 GetColliderSize()
        {
            return owner.ColliderSize;
        }
        public virtual float GetDodgeRate(int accuracy)
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
        public void SetMissAtk(UnitCtrl source, eMissLogType missLogType, eDamageEffectType damageEffectType, float scale)
        {
            owner.SetMissAtk(source, missLogType, damageEffectType, null, scale);
        }
        public virtual float GetStartAtk()
        {
            return owner.BaseValues.Atk;
        }
        public virtual float GetStartDef()
        {
            return owner.BaseValues.Def;
        }
        public virtual float GetStartMagicStr()
        {
            return owner.BaseValues.Magic_str;
        }
        public virtual float GetStartMagicDef()
        {
            return owner.BaseValues.Magic_def;
        }
        public virtual float GetStartDodge()
        {
            return owner.BaseValues.Dodge;
        }
        public virtual float GetStartPhysicalCritical()
        {
            return owner.BaseValues.Physical_critical;
        }
        public virtual float GetStartMagicCritical()
        {
            return owner.BaseValues.Magic_critical;
        }
        public virtual float GetStartLifeSteal()
        {
            return owner.BaseValues.Life_steal;
        }

        public virtual void SetBuffDebuff(bool _enable, int _value,BuffParamKind _kind, UnitCtrl _source, BattleManager _battleLog, bool _additional)
        {
            if (!_enable) { _value *= -1; }
            if (!_additional)//!!!算了就这样用吧……问题不大
            {
                if (Owner.AdditionalBuffDictionary.ContainsKey(_kind))
                {
                    Owner.AdditionalBuffDictionary[_kind] += _value;
                }
                else
                {
                    Owner.AdditionalBuffDictionary.Add(_kind, _value);
                }
            }
            else//直接加到数值上……
            {
                Debug.LogError("咕咕咕！");
            }
            string word = Owner.UnitName + "的" + _kind.GetDescription() + (_value > 0 ? "提升" : "降低") + _value;
            BattleUIManager.Instance.LogMessage(word, eLogMessageType.BUFF_DEBUFF, Owner.IsOther);
        }




        public void InitializeResistStatuts(int resistId)
        {
            //抗性
            //鸽了，PVP没有这个
            Dictionary<eActionType, Dictionary<int, int>> resistDic = new Dictionary<eActionType, Dictionary<int, int>>();
            ResistData unitResistdata = new ResistData(resistId);
            for (int i = 0; i < 20; i++)
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
        public bool ResistStatus(eActionType actionType, int detail1, UnitCtrl source, bool last, bool targetOneParts, BasePartsData target)
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
            if (randomInt < rate && last)
            {
                owner.SetMissAtk(source, eMissLogType.DODGE_ATTACK, eDamageEffectType.NORMAL,
                    targetOneParts ? target : null, 1);
                return true;
            }
            return randomInt < rate;
        }
        public void ShowHitEffect(eWeaponSeType weaponSeType, Skill skill, bool isLeft)
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
    [System.Serializable]
    public class ActionExecTime
    {
        public float Time;
        public eDamageEffectType DamageNumType;
        public float Weight;
        public float DamageNumScale;

        public ActionExecTime() { }
        public ActionExecTime(float time,float weight)
        {
            Time = time;
            Weight = weight;
        }
    }
    [System.Serializable]
    public class ActionExecedData
    {
        public bool AlreadyExeced;
        public int TargetPartsNumber;
        public int ExecedPartsNumber;
    }
    public class BuffDebuffConstData
    {
        private eStateIconType buffIcon;
        private eStateIconType debuffIcon;

        public BuffDebuffConstData(int a,int b)
        {
            buffIcon = (eStateIconType)a;
            debuffIcon = (eStateIconType)b;
        }
        public eStateIconType BuffIcon { get => buffIcon; set => buffIcon = value; }
        public eStateIconType DebuffIcon { get => debuffIcon; set => debuffIcon = value; }
    }
    [System.Serializable]
    public class CriticalData
    {
        public bool IsCritical;
        public float CriticalRate;
        public int ExpectedDamage;
    }
    public class ChangeParameterFieldData:PLFCLLHLDOO
    {
        private BuffParamKind buffParamKind;
        private eEffectType effectType;
        private float value;
        private BuffDebuffAction.eChangeParameterType valueType;
        private bool isBuff;
        private const float PERCENT_DIGIT = 100f;
        private Dictionary<UnitCtrl, int> alreadyExecedTargetCount;

        public BuffParamKind BuffParamKind { get => buffParamKind; set => buffParamKind = value; }
        public eEffectType EffectType { get => effectType; set => effectType = value; }
        public float Value { get => value; set => this.value = value; }
        public BuffDebuffAction.eChangeParameterType ValueType { get => valueType; set => valueType = value; }
        public bool IsBuff { get => isBuff; set => isBuff = value; }
        public Dictionary<UnitCtrl, int> AlreadyExecedTargetCount { get => alreadyExecedTargetCount; set => alreadyExecedTargetCount = value; }

        protected override int GetClearedIndex(UnitCtrl _unit)
        {
            return 0;
        }

        public override void OnEnter(BasePartsData parts)
        {
            base.OnEnter(parts);
            if (!AlreadyExecedTargetCount.ContainsKey(parts.Owner))
            {
                AlreadyExecedTargetCount.Add(parts.Owner, 1);
            }
            else
            {
                AlreadyExecedTargetCount[parts.Owner]++;
                if (AlreadyExecedTargetCount[parts.Owner] != 1)
                {
                    return;
                }
            }
            Dictionary<BasePartsData, int> dic = new Dictionary<BasePartsData, int>();
            if (parts.Owner.IsPartsBoss && (BuffParamKind == BuffParamKind.ENERGY_RECOVER_RATE || BuffParamKind == BuffParamKind.MOVE_SPEED))
            {
                Debug.LogError("咕咕咕！");
            }
            else
            {
                int buffid = BuffDebuffAction.CalculateBuffDebuffParam(parts, Value, ValueType, BuffParamKind, !IsBuff);
                dic.Add(parts.Owner.DummyPartsData, buffid);
            }
            parts.Owner.SetBuffParam(BuffParamKind.NONE, dic, 0, 0, null, true, Battle.eEffectType.COMMON, IsBuff, false);
            parts.Owner.EnableBuffParam(buffParamKind, dic, true, null, IsBuff, false);

        }

        public override void OnExit(BasePartsData parts)
        {
            base.OnExit(parts);
            AlreadyExecedTargetCount[parts.Owner]--;
            Dictionary<BasePartsData, int> dic = new Dictionary<BasePartsData, int>();

            if (AlreadyExecedTargetCount[parts.Owner] == 0)
            {
                if (parts.Owner.IsPartsBoss && (BuffParamKind == BuffParamKind.ENERGY_RECOVER_RATE || BuffParamKind == BuffParamKind.MOVE_SPEED))
                {
                    Debug.LogError("咕咕咕！");
                }
                else
                {
                    int buffid = BuffDebuffAction.CalculateBuffDebuffParam(parts, Value, ValueType, BuffParamKind, !IsBuff);
                    dic.Add(parts.Owner.DummyPartsData, buffid);
                }
                parts.Owner.EnableBuffParam(buffParamKind, dic, false, null, IsBuff, false);
            }
        }

        public override void OnRepeat()
        {
            //空着
        }

        public override void StartField()
        {
            //特效
            base.StartField();
        }
        public enum eEffectType
        {
            NONE = 0,
            BUFF = 1,
            DEBUFF = 2,
            UNIQUE = 3
        }

        public enum eValueType
        {
            FIXED = 1,
            PERCENTAGE = 2
        }
    }
    public class PLFCLLHLDOO : ISingletonField
    {
        private eFieldType fildType;
        private bool enable;
        private eFieldExecType execType;
        private float limitTime;
        private float centerX;
        private float size;
        private eFieldTargetType fieldTargetType;
        private List<BasePartsData> targetList;
        private List<UnitCtrl> allTargetList;
        private static BattleManager staticBattleManager;
        private const float FIELD_INTERVAL = 1f;
        protected Elements.SkillEffectCtrl skillEffect;
        private UnitCtrl pauseTarget;
        private Skill skill;
        private UnitCtrl source;
        private int fieldIndex = 1;
        private GameObject uniqueEffectPrefab;
        private GameObject uniqueEffectPrefabLeft;
        private const float BASE_EFFECT_SIZE = 350f;
        private bool stopFlag;

        public eFieldType FildType { get => fildType; set => fildType = value; }
        public bool Enable { get => enable; set => enable = value; }
        public eFieldExecType ExecType { get => execType; set => execType = value; }
        public float LimitTime { get => limitTime; set => limitTime = value; }
        public float CenterX { get => centerX; set => centerX = value; }
        public float Size { get => size; set => size = value; }
        public eFieldTargetType FieldTargetType { get => fieldTargetType; set => fieldTargetType = value; }
        public List<BasePartsData> TargetList { get => targetList; set => targetList = value; }
        public List<UnitCtrl> AllTargetList { get => allTargetList; set => allTargetList = value; }
        public static BattleManager StaticBattleManager { get => staticBattleManager; set => staticBattleManager = value; }
        public UnitCtrl PauseTarget { get => pauseTarget; set => pauseTarget = value; }
        public Skill Skill { get => skill; set => skill = value; }
        public UnitCtrl Source { get => source; set => source = value; }
        public int FieldIndex { get => fieldIndex; set => fieldIndex = value; }
        public GameObject UniqueEffectPrefab { get => uniqueEffectPrefab; set => uniqueEffectPrefab = value; }
        public GameObject UniqueEffectPrefabLeft { get => uniqueEffectPrefabLeft; set => uniqueEffectPrefabLeft = value; }
        public bool StopFlag { get => stopFlag; set => stopFlag = value; }

        protected virtual int GetClearedIndex(UnitCtrl FNHGFDNICFG)
        {
            return 0;
        }
        protected void initializeSkillEffect()
        {
        }

        private bool judgeStopTarget(eTargetAssignment OAHLOGOLMHD, bool MOBKHPNMEDM) =>
            new bool();

        public virtual void OnEnter(BasePartsData GEDLBPMPOKB)
        {
        }

        public virtual void OnExit(BasePartsData GEDLBPMPOKB)
        {
            TargetList.Remove(GEDLBPMPOKB);
        }

        public virtual void OnRepeat()
        {
            //空着
        }

        public virtual void ResetTarget(UnitCtrl FNHGFDNICFG, eAbnormalState GMDFAELOLFL)
        {
            //空着
        }

        public virtual void StartField()
        {
            Source.StartCoroutine(Update());
        }

        public static void StaticRelease()
        {
        }

        public void StopField(eTargetAssignment OAHLOGOLMHD, bool MOBKHPNMEDM)
        {

        }

        public IEnumerator Update()
        {
            float time_5_2 = 0;
            Enable = true;
            //特效
            while (true)
            {
                if (AllTargetList.Count > 0)
                {
                    foreach (UnitCtrl unitCtrl in AllTargetList)
                    {
                        if (unitCtrl.IsPartsBoss)
                        {
                            Debug.LogError("咕咕咕！");
                        }
                        else
                        {
                            Update_b_83(unitCtrl.GetFirstParts(false, 0));
                        }
                    }
                }
                if (ExecType == eFieldExecType.REPEAT)
                {
                    Debug.LogError("咕咕咕！");
                }
                time_5_2 += source.DeltaTimeForPause_BUFF;
                if (staticBattleManager.GameState == eGameBattleState.FIGHTING &&
                    time_5_2 <= LimitTime && !stopFlag)
                {
                    yield return null;
                    continue;
                }
                for (int j = TargetList.Count - 1; j >= 0; j--)
                {
                    OnExit(TargetList[j]);
                }
                Enable = false;
                yield break;
            }
        }
        public void Update_b_83(BasePartsData target)
        {
            if(target.Owner.isStealth || GetClearedIndex(target.Owner) >= FieldIndex || target.Owner.IsDead)
            {
                if (TargetList.Contains(target))
                {
                    OnExit(target);
                }
                return;
            }
            bool isIn = false;
            if (Mathf.Abs(target.GetPosition().x - CenterX) <= Size)
            {
                isIn = true;
            }
            if(target.Owner.IsSummonOrPhantom && target.Owner.IdleOnly || !isIn)
            {
                if (TargetList.Contains(target))
                {
                    OnExit(target);
                }
                return;
            }
            if (!TargetList.Contains(target))
            {
                OnEnter(target);
            }
        }
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
    public class PartsData : BasePartsData, ISingletonField
    {
        private Action OnBreak;
        private Action OnBreakEnd;
        private bool IsBreak;
        private UnitCtrl BreakSource;
        private const string CENER_BONE = "Center_{0:D2}";
        private const string STATE_BONE = "State_{0:D2}";
        //private Elements.MultiTargetCursor<MultiTargetCursor> k__BackingField;
        //private Bone<centerBone> k__BackingField;
        //private Bone<stateBone> k__BackingField;
        private Vector3 fixedCenterPos;
        //private DAGLMEOBLAA battleEffectPool;
        private int Level;
        /*private ObscuredInt<Atk> k__BackingField;
        private ObscuredInt<MagicStr> k__BackingField;
        private ObscuredInt<Def> k__BackingField;
        private ObscuredInt<MagicDef> k__BackingField;
        private ObscuredInt<Dodge> k__BackingField;
        private ObscuredInt<Accuracy> k__BackingField;
        private ObscuredInt<PhysicalCritical> k__BackingField;
        private ObscuredInt<MagicCritical> k__BackingField;
        private ObscuredInt<LifeSteal> k__BackingField;
        private ObscuredInt<HpRecoveryRate> k__BackingField;
        private ObscuredInt<StartAtk> k__BackingField;
        private ObscuredInt<StartMagicStr> k__BackingField;
        private ObscuredInt<StartDef> k__BackingField;
        private ObscuredInt<StartMagicDef> k__BackingField;
        private ObscuredInt<StartDodge> k__BackingField;
        private ObscuredInt<StartPhysicalCritical> k__BackingField;
        private ObscuredInt<StartMagicCritical> k__BackingField;
        private ObscuredInt<StartLifeSteal> k__BackingField;
        private ObscuredInt<BreakPoint> k__BackingField;*/
        private float BreakTime;
        private int MaxBreakPoint;
        private Dictionary<BuffParamKind, int> additionalBuffDictionary;

        private void createBreakEffect(int _targetMotion)
        {
        }

        public void DisableCursor()
        {
        }

        public void FixAttachment(UnitCtrl _owner)
        {
        }

        //public override int GetAccuracyZero()

        private int getAdditionalBuff(BuffParamKind _kind) =>
            new int();

       // public override int GetAtkZero() =>
        public override float GetBodyWidth() =>
            new float();

        //public override Vector3 GetBottomTransformPosition()

        //public override Bone GetCenterBone() =>

        //public override Vector3 GetColliderCenter() =>

        //public override Vector3 GetColliderSize()

        public override float GetDefZero()
        {
            return base.GetDefZero();
        }

        public override float GetDodgeRate(int _accuracy) =>
            new float();

        //private int getDodgeZero() =>
        /*public override Vector3 GetFixedCenterPos() =>
        public override float GetHpRecoverRateZero() =>
            new float();

        public override int GetLevel() =>
            new int();

        public override int GetLifeStealZero() =>
            new int();

        public override Vector3 GetLocalPosition() =>
            new Vector3();

        public override int GetMagicCriticalZero() =>
            new int();

        public override int GetMagicDefZero() =>
            new int();

        public override int GetMagicStrZero() =>
            new int();

        public override int GetPhysicalCriticalZero() =>
            new int();

        public override Vector3 GetPosition() =>
            new Vector3();

        public override int GetStartAtk() =>
            new int();

        public override int GetStartDef() =>
            new int();

        public override int GetStartDodge() =>
            new int();

        public override int GetStartLifeSteal() =>
            new int();

        public override int GetStartMagicCritical() =>
            new int();

        public override int GetStartMagicDef() =>
            new int();

        public override int GetStartMagicStr() =>
            new int();

        public override int GetStartPhysicalCritical() =>
            new int();

        public override Bone GetStateBone() =>
            null;

        public override bool GetTargetable() =>
            new bool();

        public void Initialize(MasterEnemyMParts.EnemyMParts _enemyMParts)
        {
        }

        public void SetBreak(bool _enable, Transform _unitUiCtrl)
        {
        }

        public override void SetBuffDebuff(bool _enable, int _value, UnitCtrl.BuffParamKind _kind, UnitCtrl _source, MLEGMHAOCON _battleLog, bool _additional)
        {
        }

        public void SetDamage(int _damage, UnitCtrl _source)
        {
        }

        public override void SetMissAtk(UnitCtrl _source, eMissLogType _missLogType, eDamageEffectType _damageEffectType, float _scale)
        {
        }

        public IEnumerator TrackBottom() =>
            null;

        private IEnumerator updateChangeAttachment(float _timer, bool _enable) =>
            null;

        private IEnumerator waitAndBreakPointReset() =>
            null;

        

        */
    }  
    
    public class NormalSkillEffect
    {
        public GameObject Prefab;
        public GameObject PrefabLeft;
        public float[] ExecTime;
        public bool IsReaction;
        public List<NormalSkillEffect> FireArmEndEffects;
        public bool TargetActionIsReflexive;
        public int TargetActionIndex;
        public int TargetActionId;
        public ActionParameter TargetAction;
        public ActionParameter FireAction;
        public int FireActionId;
        public int TargetMotionIndex;
        public eEffectBehavior EffectBehavior;
        public eEffectTarget EffectTarget;
        public eTargetBone TargetBone;
        public eEffectTarget FireArmEndTarget;
        public eTargetBone FireArmEndTargetBone;
        public eTrackType TrackType;
        public eTrackDimension TrackDimension;
        public string TargetBoneName;
        public bool TrackRotation;
        public bool DestroyOnEnemyDead;
        public float CenterY;
        public float DeltaY;
        public bool TrackTarget;
        public float Height;
        public bool IsAbsoluteFireArm;
        public float AbsoluteFireDistance;
        public List<ShakeEffect> ShakeEffects;
        public int TargetBranchId;
        public bool PlayWithCutIn;
        private Dictionary<UnitCtrl, bool> alreadyFireArmExecedData;
        private List<UnitCtrl> alreadyFireArmExecedKeys;

        public Dictionary<UnitCtrl, bool> AlreadyFireArmExecedData { get => alreadyFireArmExecedData; set => alreadyFireArmExecedData = value; }
        public List<UnitCtrl> AlreadyFireArmExecedKeys { get => alreadyFireArmExecedKeys; set => alreadyFireArmExecedKeys = value; }

        public bool AppendAndJudgeAlreadyExeced(UnitCtrl _target) =>
            new bool();

    }
    public class ShakeEffect
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
            public List<ActionExecTimeCombo4List> ExecTimeCombo;
            public int ActionId;
            public ActionParameterOnPerferbDetail CopyThis()
            {
                ActionParameterOnPerferbDetail ac = new ActionParameterOnPerferbDetail();
                ac.Visible = Visible;
                ac.ExecTimeForPrefab = new List<ActionExecTime>();
                ac.ExecTimeCombo = new List<ActionExecTimeCombo>();
                foreach(ActionExecTime4List a in ExecTimeForPrefab)
                {
                    ac.ExecTimeForPrefab.Add(a.data);
                }
                foreach(ActionExecTimeCombo4List b in ExecTimeCombo)
                {
                    ac.ExecTimeCombo.Add(b.data);
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
        public class ActionExecTimeCombo4List
        {
            public ActionExecTimeCombo data;
        }
    }
}
