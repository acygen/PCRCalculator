namespace PCRCaculator.Battle
{
    using PCRCaculator;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    //using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public partial class UnitCtrl : MonoBehaviour
    {
        private PCRCaculator.UnitData unitData;
        private string unitName;
        private BattleUnitBaseSpineController spineController;
        private BattleManager staticBattleManager;
        private UnitActionController actionController;
        private CharacterPageButton unitUI;
        private CharacterBuffUIController unitBuffUI;
        private static float[] posy_fix = new float[] { -0.5f, -1.5f, 0.5f, -1f, 0f };
        private static int[] posz_fix = new int[] { 300, 100, 500, 200, 400 };
        private static int[] unableActionNumber = new int[] { 12, 13, 0x12, 0x11, 0x13, 0x1d, 20, 0x27 };
        public static readonly Dictionary<eAbnormalState, AbnormalConstData> ABNORMAL_CONST_DATA;
        public static readonly Dictionary<BuffParamKind, BuffDebuffConstData> BUFF_DEBUFF_ICON_DIC;
        private BaseData baseValues;
        private bool isOther;
        private bool isPartsBoss = false;
        private bool isSummonOrPhantom;
        private bool attackWhenSilence;
        private float ATKRecastTime;
        private float Skill1RecastTime;
        private float Skill2RecastTime;
        private float m_fAtkRecastTime;
        private int searchAreaWidth;
        private SkillData skill1Data;
        private SkillData skill2Data;
        private float bossAppearDelay = 0f;
        private float moveRate = 450f;
        private readonly float bodyWidth = 125f;
        private float timeScale = 1;
        private eActionState actionState;
        public Dictionary<eAbnormalState, bool> abnomeralStateDic = new Dictionary<eAbnormalState, bool>();
        private bool isPause;
        private bool isDead;
        private bool dieInToad;
        private bool isDeadBySetCurrentHp;
        private bool isLeftDir;
        private bool cancalByAwake;
        private bool cancalByCovert;
        private bool cancalByToad;
        private bool gameStartDone;
        private bool standByDone;
        private bool idleOnly;
        private bool idleStartAfterWaitFrame;
        private bool modeChangeEnd;
        private bool joyFlag;
        private bool isConfusionOrConvert;
        private bool isDivisionSourceForDamage;
        private bool isDivisionSourceForDie;
        private bool isRunForCatchUp;
        private bool onDeadForRevial;
        private bool toadRelease;
        private bool toadReleaseDamage;
        private bool attackPatternIsLoop;
        private bool unionBurstAnimeEndForIfAction;
        private Vector3 fixedPosition;
        private int currentSkillId;
        private int currentTriggerSkillId;
        private int currentActionPatternId;
        private int attackPatternIndex;
        private int nextSkillId;
        private int enemyPoint;
        private int knockBackEnableCount = 0;//击退效果计数器，不为零表示正在遭到击退效果
        private int unDeadTimeHitCount = 0;
        private int clearedBuffIndex;
        private int clearedDebuffIndex;
        private List<UnitCtrl> targetList = new List<UnitCtrl>();
        private List<UnitCtrl> targetPlayerList = new List<UnitCtrl>();
        private List<UnitCtrl> targetEnemyList = new List<UnitCtrl>();
        private List<int> buffDebuffSkilIds;
        private List<ToadData> toadDatas = new List<ToadData>();
        private List<int> actionsTargetOnMe = new List<int>();
        private List<FirearmCtrl> firearmCtrlsOnMe = new List<FirearmCtrl>();
        private Dictionary<int, List<int>> attackPatternDictionary = new Dictionary<int, List<int>>();
        private Dictionary<int, List<int>> attackPatternLoopDictionary = new Dictionary<int, List<int>>();


        private Dictionary<eAbnormalStateCategory, AbnormalStateCategoryData> abnormalStateCategoryDataDictionary = new Dictionary<eAbnormalStateCategory, AbnormalStateCategoryData>();
        private Dictionary<UnitCtrl, AccumulativeDamageData> accumulativeDamageDataDictionary = new Dictionary<UnitCtrl, AccumulativeDamageData>();
        private List<Queue<int>> lifeStealQueueList = new List<Queue<int>>();
        private BasePartsData dummyPartsData;
        private float actionRecastTime = 0f;
        private float actionStartTimeCounter;
        private float m_fCastTimer = 2.5f;
        private float deltaTimeForPause;
        private float hp = 100f;
        private float energy = 0f;
        private float skillStackVal = 90f;//行动前固定获取的TP
        private float skillStackValDmg = 1;//受伤TP获取率，在初始化时设置
        private float accumulateDamage;
        private float unionburstLifeStealNum = 0;
        private float physicalCriticalDamageRate = 50;
        private float magicalCriticalDamageRate = 50;
        private int walkCoroutineId = 0;
        private int damageCoroutineId = 0;
        private int dieCoroutineId = 0;
        private int buffDebuffIndex = 0;
        private int buffCounter;
        private int debuffCounter;
        public bool IsScaleChangeTarget;
        public List<long> ActionTargetOnMe = new List<long>();
        public int AtkType = 1;
        public eWeaponSeType WeaponSeType;
        public eWeaponMotionType WeaponMotionType;
        public eSummonType SummonType = eSummonType.NONE;
        public UnitCtrl SummonSource = null;
        public UnitCtrl KillBonusTarget = null;
        public Dictionary<int, UnitCtrl> SummonUnitDictionary = new Dictionary<int, UnitCtrl>();
        public Dictionary<int, float> SkillAreaWidthList = new Dictionary<int, float>();
        public Dictionary<BuffParamKind, int> AdditionalBuffDictionary = new Dictionary<BuffParamKind, int>();
        public Dictionary<UnitCtrl, Dictionary<int, AttackSealData>> DamageSealDataDictionary = new Dictionary<UnitCtrl, Dictionary<int, AttackSealData>>();
        public Dictionary<UnitCtrl, Dictionary<int, AttackSealData>> DamageOnceOwnerSealDateDictionary = new Dictionary<UnitCtrl, Dictionary<int, AttackSealData>>();
        public Dictionary<UnitCtrl, Dictionary<int, AttackSealData>> DamageOwnerSealDataDictionary = new Dictionary<UnitCtrl, Dictionary<int, AttackSealData>>();
        public Action OnSlipDamage;
        public Action OnDodge;
        public Action<UnitCtrl> OnDieForZeroHp;
        public Action<UnitCtrl> EnergyChange;
        public Action<UnitCtrl, eStateIconType, bool> OnChangeState;
        public int UBSkillId;
        public int Skill_1Id;
        public int Skill_2Id;
        public bool isStealth = false;
        public bool HasUnDeadTime = false;
        public bool IsMoveSpeedForceZero = false;
        internal int PhysicalPenetrateZero;
        internal int MagicpenetrateZero;

        public delegate void OnDamageDelegate(bool byAttack, float dmage, bool critical);
        public OnDamageDelegate Ondamage;
        public delegate void OnDeadDelegate(UnitCtrl owner);
        public OnDeadDelegate OnDeadForRevial;
        public OnDeadDelegate OnDead;
        static UnitCtrl()
        {
            Dictionary<eAbnormalState, AbnormalConstData> dictionary1 = new Dictionary<eAbnormalState, AbnormalConstData>
            {
                { eAbnormalState.GUARD_ATK, new AbnormalConstData(true, eStateIconType.PHYSICS_BARRIAR) },
                { eAbnormalState.GUARG_MGC, new AbnormalConstData(true, eStateIconType.MAGIC_BARRIAR) },
                { eAbnormalState.DRAIN_ATK, new AbnormalConstData(true, eStateIconType.PHYSICAS_DRAIN_BARRIAR) },
                { eAbnormalState.DRAIN_MGC, new AbnormalConstData(true, eStateIconType.MAGIC_DRAIN_BARRIAR) },
                { eAbnormalState.GUANG_BOTH, new AbnormalConstData(true, eStateIconType.BOTH_BARRIAR) },
                { eAbnormalState.HASTE, new AbnormalConstData(true, eStateIconType.HASTE) },
                { eAbnormalState.POISON, new AbnormalConstData(false, eStateIconType.SLIP_DAMAGE) },
                { eAbnormalState.BURN, new AbnormalConstData(false, eStateIconType.BURN) },
                { eAbnormalState.CURSE, new AbnormalConstData(false, eStateIconType.CURSE) },
                { eAbnormalState.SLOW, new AbnormalConstData(false, eStateIconType.SLOW) },
                { eAbnormalState.PARALYSIS, new AbnormalConstData(false, eStateIconType.PARALISYS) },
                { eAbnormalState.FREEZE, new AbnormalConstData(false, eStateIconType.FREEZE) },
                { eAbnormalState.CONVERT, new AbnormalConstData(false, eStateIconType.CONVER) },
                { eAbnormalState.DARK, new AbnormalConstData(false, eStateIconType.DARK) },
                { eAbnormalState.SLIENCE, new AbnormalConstData(false, eStateIconType.SILENCE) },
                { eAbnormalState.CHAINED, new AbnormalConstData(false, eStateIconType.CHAINED) },
                { eAbnormalState.SLEEP, new AbnormalConstData(false, eStateIconType.SLEEP) },
                { eAbnormalState.STUN, new AbnormalConstData(false, eStateIconType.STUN) },
                { eAbnormalState.DETAIN, new AbnormalConstData(false, eStateIconType.DETAIN) },
                { eAbnormalState.NO_EFFECT_SLIP_DAMAGE, new AbnormalConstData(false, eStateIconType.SLIP_DAMAGE) },
                { eAbnormalState.NO_DAMAGE_MOTION, new AbnormalConstData(true, eStateIconType.NO_DAMAGE) },
                { eAbnormalState.NO_ABNORMAL, new AbnormalConstData(true, eStateIconType.DEBUF_BARRIAR) },
                { eAbnormalState.CONTINUOUS_ATTACK_NEARBY, new AbnormalConstData(true, eStateIconType.DEBUF_BARRIAR) },
                { eAbnormalState.PARTS_NO_DAMAGE, new AbnormalConstData(true, eStateIconType.NONE) },
                { eAbnormalState.ACCUMULATIVE_DAMAGE, new AbnormalConstData(true, eStateIconType.NONE) },
                { eAbnormalState.DECOY, new AbnormalConstData(false, eStateIconType.NONE) },
                { eAbnormalState.MIFUYU, new AbnormalConstData(true, eStateIconType.DECOY) },
                { eAbnormalState.STONE, new AbnormalConstData(false, eStateIconType.NONE) },
                { eAbnormalState.REGENERATION, new AbnormalConstData(false, eStateIconType.STONE) },
                { eAbnormalState.PHYSICS_DODGE, new AbnormalConstData(true, eStateIconType.REGENERATION) },
                { eAbnormalState.CONFUSION, new AbnormalConstData(true, eStateIconType.PHYSICS_DODGE) },
                { eAbnormalState.VENOM, new AbnormalConstData(false, eStateIconType.CONFUSION) },
                { eAbnormalState.COUNT_BLIND, new AbnormalConstData(false, eStateIconType.VENOM) },
                { eAbnormalState.INHIBIT_HEAL, new AbnormalConstData(false, eStateIconType.COUNT_BLIND) },
                { eAbnormalState.FEAR, new AbnormalConstData(false, eStateIconType.INHIBIT_HEAL) },
                { eAbnormalState.TP_REGENERATION, new AbnormalConstData(false, eStateIconType.FEAR) },
                { eAbnormalState.HEX, new AbnormalConstData(true, eStateIconType.TP_REGENERATION) },
                { eAbnormalState.FAINT, new AbnormalConstData(false, eStateIconType.HEX) },
                { eAbnormalState.FAINT | eAbnormalState.GUARD_ATK, new AbnormalConstData(false, eStateIconType.FAINT) },
                { eAbnormalState.COMPENSATION, new AbnormalConstData(false, eStateIconType.COMPENSATION) },
                { eAbnormalState.CUT_ATK_DAMAGE, new AbnormalConstData(true, eStateIconType.CUT_ATK_DAMAGE) },
                { eAbnormalState.CUT_MGC_DAMAGE, new AbnormalConstData(true, eStateIconType.CUT_MGC_DAMAGE) },
                { eAbnormalState.CUT_ALL_DAMAGE, new AbnormalConstData(true, eStateIconType.CUT_ALL_DAMAGE) },
                { eAbnormalState.LOG_ATK_BARRIR, new AbnormalConstData(true, eStateIconType.LOG_ATK_BARRIER) },
                { eAbnormalState.LOG_MGC_BARRIR, new AbnormalConstData(true, eStateIconType.LOG_MGC_BARRIER) },
                { eAbnormalState.LOG_ALL_BARRIR, new AbnormalConstData(true, eStateIconType.LOG_ALL_BARRIER) }
            };
            ABNORMAL_CONST_DATA = dictionary1;
            Dictionary<BuffParamKind, BuffDebuffConstData> dictionary2 = new Dictionary<BuffParamKind, BuffDebuffConstData>
            {
                {BuffParamKind.NONE,new BuffDebuffConstData(0,0) },
                {BuffParamKind.ATK,new BuffDebuffConstData(1,13) },
                {BuffParamKind.DEF,new BuffDebuffConstData(2,14) },
                {BuffParamKind.MAGIC_STR,new BuffDebuffConstData(3,15) },
                {BuffParamKind.MAGIC_DEF,new BuffDebuffConstData(4,16) },
                {BuffParamKind.DODGE,new BuffDebuffConstData(5,17) },
                {BuffParamKind.PHYSICAL_CRITICAL,new BuffDebuffConstData(6,18) },
                {BuffParamKind.MAGIC_CRITICAL,new BuffDebuffConstData(6,18) },
                {BuffParamKind.ENERGY_RECOVER_RATE,new BuffDebuffConstData(7,19) },
                {BuffParamKind.LIFE_STEAL,new BuffDebuffConstData(11,21) },
                {BuffParamKind.MOVE_SPEED,new BuffDebuffConstData(0,47) },
                {BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE,new BuffDebuffConstData(67,68) },
                {BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE,new BuffDebuffConstData(69,70) },
            };
            BUFF_DEBUFF_ICON_DIC = dictionary2;
        }

        private IEnumerator _UpdateAttack()
        {
        Label_PostSwitchInIterator:;
            if (this.IsCancalActionState(1))
            {
                this.actionController.CancalAction(1);
            }
            else if (!this.SpineController.IsPlayingAnimeBattleComplete)
            {
                yield return null;
                goto Label_PostSwitchInIterator;
            }
            this.cancalByAwake = false;
            this.cancalByCovert = false;
            this.cancalByToad = false;
            this.SetState(eActionState.IDLE, 0, 0, false);
        }
        private IEnumerator _UpdateDamage(int damageCorId)
        {
            float time_0 = 0f;
            while (this.actionState == eActionState.DAMAGE)
            {
                if (damageCorId != this.damageCoroutineId)
                {
                    yield break;
                }
                if (this.Hp > 0f)
                {
                    if (this.IsUnableActionState())
                    {
                        /*if (!this.SpineController.IsPlayingAnimeBattleComplete)
                        {
                            yield return null;
                            continue;
                        }*/
                        yield return null;
                        continue;
                    }
                    if (!this.toadRelease || !this.idleOnly)
                    {
                        actionState = eActionState.IDLE;
                        this.SetState(eActionState.IDLE, 0, 0, false);
                    }
                    yield break;
                }
                else
                {
                    actionState = eActionState.IDLE;
                    SetState(eActionState.DIE, 0);
                    yield break;
                }
                if ((this.actionsTargetOnMe.Count <= 0) && (this.firearmCtrlsOnMe.Count <= 0))
                {
                    this.SpineController.Resume();
                    this.actionState = eActionState.IDLE;
                    this.SetState(eActionState.DIE, 0, 0, false);
                }
                time_0 += Time.deltaTime;
                if (time_0 > 10f)
                {
                    this.firearmCtrlsOnMe.Clear();
                    this.actionsTargetOnMe.Clear();
                    yield return null;
                }
                break;
            }
        }
        private IEnumerator _UpdateGameStart()
        {
            float time_count = 0f;
            this.SpineController.Pause(true);
            time_count += this.DeltaTimeForPause;
            while (time_count <= this.bossAppearDelay)
            {
                time_count += this.DeltaTimeForPause;
                yield return null;
            }
            this.SpineController.Resume();
            this.SpineController.SetAnimaton(eAnimationType.run_start, true);
            if (this.isDead)
            {
                this.gameStartDone = true;
            }
            while (this.staticBattleManager.GameState == eGameBattleState.PREPARING)
            {
                yield return null;
            }
            this.StartCoroutine(this._UpdateTargets());
            this.SetState(eActionState.WALK, this.nextSkillId, 0, false);
        }
        private IEnumerator _UpdateIdle()
        {
            int state = 0;
            this.m_fCastTimer = this.ATKRecastTime;
            while (state >= 0)
            {
                if (this.idleStartAfterWaitFrame)
                {
                    state = 1;
                    this.idleStartAfterWaitFrame = false;
                    yield return null;
                    continue;
                }
                if (this.modeChangeEnd)
                {
                    state = 2;
                    yield return null;
                    continue;
                }
                if (this.joyFlag)
                {
                    this.joyFlag = false;
                }
                if (this.idleOnly)
                {
                    if (this.enemyPoint != 0)
                    {
                    }
                }
                else
                {
                    int actionid5__5;
                    if (this.attackPatternLoopDictionary[this.currentActionPatternId].Count == 0)
                    {
                        yield return null;
                        continue;
                    }
                    if (this.actionState == eActionState.IDLE)
                    {
                    }
                    if (this.actionState > eActionState.IDLE)
                    {
                        yield break;
                    }
                    if (BattleManager.Instance.BlackOutUnitList.Contains(this))
                    {
                        yield return null;
                        continue;
                    }
                    if (this.isConfusionOrConvert)
                    {
                        if (!this.targetPlayerList.Contains(this) && (this.targetPlayerList.Count != 1))
                        {
                            this.SetState(eActionState.WALK, 0, 0, false);
                            yield break;
                        }
                    }
                    else if (this.targetEnemyList.Count <= 0)
                    {
                        this.isRunForCatchUp = true;
                        this.SetState(eActionState.WALK, 0, 0, false);
                        yield break;
                    }
                    if (((this.actionStartTimeCounter <= 0f) || this.isOther) && !this.toadRelease)
                    {
                        this.m_fCastTimer -= this.DeltaTimeForPause;
                    }
                    if (this.toadRelease)
                    {
                        yield return null;
                        continue;
                    }
                    if ((this.m_fCastTimer > 0f) || (this.attackPatternDictionary == null))
                    {
                        if (this.Hp > 0f)
                        {
                            yield return null;
                            continue;
                        }
                        this.SetState(eActionState.DIE, 0, 0, false);
                        yield break;
                    }
                    List<int> list5__2 = this.attackPatternIsLoop ? this.attackPatternLoopDictionary[this.currentActionPatternId] : this.attackPatternDictionary[this.currentActionPatternId];
                    int currentAction5__3 = list5__2[this.attackPatternIndex];
                    int skillid = list5__2[this.attackPatternIndex];
                    if (this.attackPatternIndex == (list5__2.Count - 1))
                    {
                        this.attackPatternIndex = 0;
                        this.attackPatternIsLoop = true;
                        actionid5__5 = this.attackPatternLoopDictionary[this.currentActionPatternId][0];
                    }
                    else
                    {
                        this.attackPatternIndex++;
                        actionid5__5 = list5__2[this.attackPatternIndex];
                    }
                    this.cancalByAwake = false;
                    if (currentAction5__3 <= 1)
                    {
                        this.SetState(eActionState.ATK, actionid5__5, skillid, false);
                        yield break;
                    }
                    this.SetState(eActionState.SKILL, actionid5__5, skillid, false);
                    yield break;
                }
                if (!this.isSummonOrPhantom)
                {
                    yield return null;
                    continue;
                }
                Debug.LogError("咕咕咕！");
                break;
            }
        }
        private IEnumerator _UpdateWalk(int corId)
        {
            while (this.actionState == eActionState.WALK && staticBattleManager.GameState == eGameBattleState.FIGHTING)
            {
                if (corId != this.walkCoroutineId)
                {
                    yield break;
                }
                if (((this.actionState == eActionState.SKILL1) || (this.actionState == eActionState.DAMAGE)) || (this.actionState == eActionState.DIE))
                {
                    this.isRunForCatchUp = false;
                    yield break;
                }
                if (this.isPause)
                {
                    yield return null;
                    continue;
                }
                List<UnitCtrl> targetList5__1 = this.isOther ? this.staticBattleManager.EnemiesList : this.staticBattleManager.PlayersList;
                if (this.isConfusionOrConvert)
                {
                    if (targetList5__1.FindAll(a=>!a.isDead).Count == 1)
                    {
                        this.isRunForCatchUp = false;
                        this.SetState(eActionState.IDLE, 0, 0, false);
                        yield break;
                    }
                    if ((this.targetPlayerList.Count > 1) || this.idleOnly)
                    {
                        this.SetState(eActionState.IDLE, 0, 0, false);
                        yield break;
                    }
                }
                else if (this.idleOnly || (this.targetEnemyList.Count > 0))
                {
                    this.isRunForCatchUp = false;
                    this.SetState(eActionState.IDLE, 0, 0, false);
                    yield break;
                }
                bool k15__2 = targetList5__1.FindIndex(a=> a.Hp > 0f) == -1;
                bool k25__3 = targetList5__1.TrueForAll( a=>a.isStealth || (a.Hp == 0f));
                bool k35__4 = this.staticBattleManager.GameState == eGameBattleState.FIGHTING;
                float speedRate_k5__5 = this.isRunForCatchUp ? 1f : 1.6f;
                float deltaX = (this.moveRate * speedRate_k5__5) * this.DeltaTimeForPause;
                if (!((k15__2 | k25__3) & k35__4))
                {
                    if ((this.SpineController.GetCurrentAnimationName() == eAnimationType.idle) && (this.moveRate != 0f))
                    {
                        this.SpineController.SetAnimaton(eAnimationType.run, true, 1f);
                    }
                    this.SetPosition(deltaX);
                    this.SetLeftDirection(deltaX > 0f);
                    yield return null;
                    continue;
                }
                if (this.SpineController.GetCurrentAnimationName() != eAnimationType.run)
                {
                    this.SetLeftDirection(deltaX > 0f);
                    yield return null;
                    continue;
                }
                this.SpineController.SetAnimaton(eAnimationType.idle, true, 1f);
                yield return null;
                targetList5__1 = null;
            }
        }
        private IEnumerator _UpdateSkill(int skillId)
        {
            while (true)
            {
                if (this.cancalByCovert || this.cancalByToad)
                {
                    this.actionController.CancalAction(skillId);
                    this.cancalByToad = false;
                    this.cancalByCovert = false;
                    this.cancalByAwake = false;
                    yield break;
                }
                if (((this.actionState == eActionState.SKILL1) || (this.actionState == eActionState.DAMAGE)) || (this.actionState == eActionState.DIE))
                {
                    this.actionController.CancalAction(skillId);
                    this.cancalByCovert = false;
                    this.cancalByAwake = false;
                    yield break;
                }
                if (this.cancalByAwake && (skillId != this.currentTriggerSkillId))
                {
                    this.actionController.CancalAction(skillId);
                    this.cancalByAwake = false;
                    this.cancalByCovert = false;
                    this.cancalByToad = false;
                    yield break;
                }
                if (this.actionController.HasNextAnime(skillId) && this.actionController.IsLoopMotionPlaying(skillId))
                {
                    Debug.LogError("特殊持续动画鸽了！");
                }
                if (this.SpineController.IsPlayingAnimeBattleComplete || (this.actionController.GetSkillMotionType(skillId) == eSkillMotionType.NONE))
                {
                    if (this.actionController.IsModeChange(skillId))
                    {
                        Debug.LogError("咕咕咕！");
                    }
                    if (!this.actionController.HasNextAnime(skillId))
                    {
                        this.SetState(eActionState.IDLE, 0, 0, false);
                        this.cancalByAwake = false;
                        this.cancalByCovert = false;
                        this.cancalByToad = false;
                        yield break;
                    }
                }
                yield return null;
            }
        }
        private IEnumerator _UpdateSkill1()
        {
            while (true)
            {
                if (cancalByCovert || cancalByToad ||
                    actionState == eActionState.DAMAGE ||
                    actionState == eActionState.DIE)
                {
                    actionController.CancalAction(UBSkillId);
                    cancalByCovert = false;
                    cancalByToad = false;
                    yield break;
                }
                if (!spineController.IsPlayingAnimeBattleComplete)
                {
                    if (!unionBurstAnimeEndForIfAction)
                    {
                        yield return null;
                        continue;
                    }
                }
                break;
            }
            unionBurstAnimeEndForIfAction = false;
            if (actionController.IsModeChange(UBSkillId))
            {
                Debug.LogError("咕咕咕！");
            }
            if (!actionController.HasNextAnime(UBSkillId))
            {
                SkillEndProcess();
                yield break;
            }
            while (true)
            {
                if (actionState != eActionState.IDLE)
                {
                    if (actionState != eActionState.DIE && actionState != eActionState.DAMAGE)
                    {
                        yield return null;
                        continue;
                    }
                    actionController.CancalAction(UBSkillId);
                    cancalByCovert = false;
                    cancalByToad = false;
                    yield break;
                }
                yield break;
            }
        }
        private IEnumerator _UpdateStandBy()
        {
            while (!this.SpineController.IsPlayingAnimeBattleComplete)
            {
                if (this.actionState == eActionState.IDLE)
                {
                    yield return null;
                    continue;
                }
                this.standByDone = true;
                yield return null;
            }
            this.standByDone = true;
            SetState(eActionState.IDLE, nextSkillId);
            BattleUIManager.Instance.LogMessage(UnitName + "准备完毕",eLogMessageType.BATTLE_READY,isOther);
            /*if (this.actionState == eActionState.IDLE)
            {
                this.SpineController.SetAnimaton(eAnimationType.idle, true, 1f);
            }*/
        }
        private IEnumerator _UpdateTargets()
        {
            yield return null;
            while (this.actionState != eActionState.DIE && staticBattleManager.GameState == eGameBattleState.FIGHTING)
            {
                if (!this.isPause)
                {
                    List<UnitCtrl> playersList;
                    if (this.abnomeralStateDic[eAbnormalState.CONFUSION] ^ this.isOther)
                    {
                        playersList = this.staticBattleManager.PlayersList;
                    }
                    else
                    {
                        playersList = this.staticBattleManager.EnemiesList;
                    }
                    this.targetList.Clear();
                    foreach (UnitCtrl a5__3 in playersList)
                    {
                        if (this.GetDistance(a5__3, true) < (this.SearchAreaWidth + BodyWidth))
                        {
                            if (!a5__3.IsDead)
                            {
                                this.targetList.Add(a5__3);
                            }
                        }
                    }
                    this.targetList.Sort(delegate (UnitCtrl x, UnitCtrl y) {
                        return ((float) this.GetDistance(x, true)).CompareTo(this.GetDistance(y, true));
                    });
                    this.targetEnemyList = this.targetList;
                    playersList = null;
                }
                yield return null;
            }
        }
        private IEnumerator _UpdateDie(int dieid)
        {
            float color_r = 1;
            OnDead?.Invoke(this);

            while (true)
            {
                if (dieid != dieCoroutineId)
                {
                    yield break;
                }
                if (spineController == null)
                {
                    yield break;
                }
                if (!spineController.IsPlayingAnimeBattleComplete)
                {
                    yield return null;
                    continue;
                }
                //播放声音
                //if (isDeadBySetCurrentHp)
                //{
                //
                //}
                color_r = 1;
                break;
            }
            while (dieid == dieCoroutineId)
            {
                float time_0 = DeltaTimeForPause;
                color_r -= time_0*1.35f;
                if (color_r <= 0)
                {
                    if(!onDeadForRevial)
                    {
                        if(SummonType != eSummonType.NONE)
                        {
                            Debug.LogError("咕咕咕！");
                        }
                        else
                        {
                            if (IsPartsBoss)
                            {
                                Debug.LogError("咕咕咕！");
                            }
                            gameObject.SetActive(false);
                        }
                        //callbackFadeoutDone;
                        yield break;
                    }
                    OnDeadForRevial?.Invoke(this);
                }
                Color color = new Color(color_r, color_r, color_r, color_r);
                spineController.SetCurColor(color);
                yield return null;
            }
        }
        private IEnumerator _UpdateUndoDivision()
        {
            Debug.LogError("咕咕咕！");
            yield return null;
        }
        internal float GetMagicCriticalDamageRateOrMin()
        {
            return physicalCriticalDamageRate;
        }
        internal float GetMagicCriticalZero()
        {
            return BaseValues.Magic_critical + GetAdditionalBuffDictionary(BuffParamKind.MAGIC_CRITICAL);
        }
        internal float GetMagicStrZero()
        {
            return BaseValues.Magic_str + GetAdditionalBuffDictionary(BuffParamKind.MAGIC_STR);
        }
        internal float GetPhysicalCriticalDamageRateOrMin()
        {
            return magicalCriticalDamageRate;
        }
        internal float GetPhysicalCriticalZero()
        {
            return BaseValues.Physical_critical + GetAdditionalBuffDictionary(BuffParamKind.PHYSICAL_CRITICAL);

        }
        internal float GetAtkZero()
        {
            return BaseValues.Atk + GetAdditionalBuffDictionary(BuffParamKind.ATK);
        }
        internal int GetLifeStealZero()
        {
            return (int)BaseValues.Life_steal + GetAdditionalBuffDictionary(BuffParamKind.LIFE_STEAL);
        }
        private int GetAdditionalBuffDictionary(BuffParamKind _kind)
        {
            return AdditionalBuffDictionary.TryGetValue(_kind, out int value) ? value : 0;
        }


        private void ApplyPassitiveSkill()
        {
            this.baseValues = MainManager.Instance.UnitRarityDic[this.unitData.unitId].GetBattleValuesApplyedEX(this.unitData);
        }

        public float CalcAweValue(bool isUB, bool isAttack)
        {
            return 1;
        }


        public float GetDistance(UnitCtrl target, bool isAbs)
        {
            if (target == this)
            {
                Debug.LogError("不能和自己比较！");
            }
            if (isAbs)
            {
                return Mathf.Abs((float) (target.FixedPosition.x - this.FixedPosition.x));
            }
            return (target.FixedPosition.x - this.FixedPosition.x);
        }

        public BasePartsData GetFirstParts(bool _owner, float _basePos)
        {
            if (!this.isPartsBoss | _owner)
            {
                return this.dummyPartsData;
            }
            Debug.LogError("boss的partsData还没做!");
            return new BasePartsData();
        }

        public bool IsAbnormalState(eAbnormalState eAbnomeralState)
        {
            bool flag;
            return (this.abnomeralStateDic.TryGetValue(eAbnomeralState, out flag) && flag);
        }

        public bool IsCancalActionState(int skillId)
        {
            bool flag = (this.cancalByAwake || this.cancalByCovert) || this.cancalByToad;
            bool flag2 = false;
            if (this.actionState == eActionState.SKILL1)
            {
                flag2 = skillId != this.UBSkillId;
            }
            else if (this.actionState == eActionState.SKILL)
            {
                flag2 = (skillId != 0x3e9) && (skillId != 0x3ea);
            }
            else if (this.actionState == eActionState.ATK)
            {
                flag2 = skillId != 1;
            }
            return ((flag | flag2) || (this.actionState == eActionState.GAMESTART));
        }

        internal void SetTimeScale(float scale)
        {
            TimeScale = scale;
            spineController.SetTimeScale(scale);
        }

        private bool IsNearestEnemyLeft()
        {
            List<UnitCtrl> playersList;
            float num = 4096f;
            bool flag = false;
            bool isOther = this.isOther;
            if (this.abnomeralStateDic[eAbnormalState.CONFUSION])
            {
                isOther = !isOther;
            }
            if (isOther)
            {
                playersList = this.staticBattleManager.PlayersList;
            }
            else
            {
                playersList = this.staticBattleManager.EnemiesList;
            }
            foreach (UnitCtrl ctrl in playersList)
            {
                if (ctrl != this)
                {
                    float distance = this.GetDistance(ctrl, false);
                    if (Mathf.Abs(distance) < num)
                    {
                        flag = distance > 0f;
                        num = Mathf.Abs(distance);
                    }
                }
            }
            return flag;
        }

        public bool IsUnableActionState()
        {
            /*for (int i = 0; i < unableActionNumber.Length; i++)
            {
                if (this.abnomeralStateDic[(eAbnormalState) i])
                {
                    return true;
                }
            }*/
            return (this.KnockBackEnableCount > 0);
        }

        private void LoadUnitActionController(int unitid)
        {
            this.actionController = base.gameObject.AddComponent<UnitActionController>();
            this.actionController.InterInitialize(this, false, null, unitid);
        }

        public void Pause()
        {
            this.isPause = !this.isPause;
            if (this.isPause)
            {
                this.SpineController.Pause(true);
            }
            else
            {
                this.SpineController.Pause(false);
            }
        }

        public void PlaySkillAnimation(int skillId,int perIndex = 0,bool loop = false)
        {
            int enumInt = 0;
            if (skillId == this.UBSkillId)
            {
                enumInt = (int)eAnimationType.skill0;
            }
            else if (skillId == 1)
            {
                enumInt = (int)eAnimationType.attack;
            }
            else if (skillId == this.Skill_1Id)
            {
                enumInt = (int)eAnimationType.skill1;
            }
            else if (skillId == this.Skill_2Id)
            {
                enumInt = (int)eAnimationType.skill2;
            }
            else
            {
                Debug.LogError("技能动画设置失败！");
            }
            this.SpineController.SetAnimaton((eAnimationType)(enumInt + perIndex), loop, 1f);
            

        }
        public bool JudgeHasSkillAnimation(int skillId, int perIndex)
        {
            int enumInt = 0;
            if (skillId == this.UBSkillId)
            {
                enumInt = (int)eAnimationType.skill0;
            }
            else if (skillId == 1)
            {
                enumInt = (int)eAnimationType.attack;
            }
            else if (skillId == this.Skill_1Id)
            {
                enumInt = (int)eAnimationType.skill1;
            }
            else if (skillId == this.Skill_2Id)
            {
                enumInt = (int)eAnimationType.skill2;
            }
            else
            {
                Debug.LogError("技能动画设置失败！");
            }
            return SpineController.HasAnimation((eAnimationType)(enumInt + perIndex));


        }


        public void SetLeftDirection(bool isLeft = false)
        {
            Vector3 one = Vector3.one;
            if (!isLeft)
            {
                one.x = -1f;
            }
            base.transform.localScale = one;
            this.isLeftDir = isLeft;
        }
        /// <summary>
        /// 自制的初始化函数，代替Initialize
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="unitData"></param>
        /// <param name="isOther"></param>
        public void SetOnAwake(BattleUnitBaseSpineController ac, PCRCaculator.UnitData unitData, bool isOther)
        {
            string str;
            this.staticBattleManager = BattleManager.Instance;
            this.SpineController = ac;
            this.unitData = unitData;
            this.isOther = isOther;
            UnitRarityData data = MainManager.Instance.UnitRarityDic[unitData.unitId];
            this.unitName = data.unitName;
            if (MainManager.Instance.UnitName_cn.TryGetValue(unitData.unitId, out str))
            {
                this.unitName = str;
            }
            this.unitName = (isOther ? "<color=#FF0000>" : "<color=#0024FF>") + this.unitName + "</color>";
            this.ATKRecastTime = data.detailData.normalAtkCastTime;
            this.SearchAreaWidth = data.detailData.searchAreaWidth;
            this.WeaponSeType = (eWeaponSeType) data.detailData.seType;
            this.WeaponMotionType = (eWeaponMotionType) data.detailData.motionType;
            int[] skillList = data.skillData.GetSkillList(unitData.rarity, false);
            this.UBSkillId = skillList[0];
            this.Skill_1Id = skillList[1];
            this.Skill_2Id = skillList[2];
            this.skill1Data = MainManager.Instance.SkillDataDic[this.Skill_1Id];
            this.Skill1RecastTime = this.skill1Data.casttime;
            this.skill2Data = MainManager.Instance.SkillDataDic[this.Skill_2Id];
            this.Skill2RecastTime = this.skill2Data.casttime;
            this.currentActionPatternId = 0;
            UnitSkillData skillData = data.skillData;
            List<int> list = new List<int>();
            for (int i = 0; i < (skillData.loopStart - 1); i++)
            {
                list.Add(skillData.atkPatterns[i]);
            }
            this.attackPatternDictionary.Add(0, list);
            List<int> list2 = new List<int>();
            for (int j = skillData.loopStart - 1; j < (skillData.loopEnd - 1); j++)
            {
                list2.Add(skillData.atkPatterns[j]);
            }
            this.attackPatternLoopDictionary.Add(0, list2);
            this.nextSkillId = attackPatternDictionary[0][0];
            SkillAreaWidthList.Add(Skill_1Id, skill1Data.areawidth);
            SkillAreaWidthList.Add(Skill_2Id, skill2Data.areawidth);
            for (int k = 1; k <= 0x2f; k++)
            {
                this.abnomeralStateDic.Add((eAbnormalState) k, false);
            }
            if (isOther)
            {
                this.moveRate *= -1f;
            }
            SetEnergy(0, eSetEnergyType.INITIALIZE, null);
            this.dummyPartsData = new BasePartsData();
            this.dummyPartsData.PositionX = 0f;
            this.dummyPartsData.Owner = this;
            this.ApplyPassitiveSkill();
            if (this.BaseValues.Magic_str > this.BaseValues.Atk)
            {
                this.AtkType = 2;
            }
            Hp = BaseValues.Hp;
            this.LoadUnitActionController(unitData.unitId);
            skillStackValDmg = 500.0f / Hp;
        }

        internal float GetDefZero()
        {
            return BaseValues.Def + GetAdditionalBuffDictionary(BuffParamKind.DEF);
        }

        internal float GetMagicDefZero()
        {
            return BaseValues.Magic_def + GetAdditionalBuffDictionary(BuffParamKind.MAGIC_DEF);
        }

        internal float GetDodgeRate(int accuracy)
        {
            float dodge = Mathf.Max(0, BaseValues.Dodge - accuracy);
            return dodge / (dodge + 100.0f);
        }
        internal float GetEnergyRecoveryRateZero()
        {
            return BaseValues.Energy_recovery_rate + GetAdditionalBuffDictionary(BuffParamKind.ENERGY_RECOVER_RATE);
        }

        internal void SetMissAtk(UnitCtrl source, eMissLogType missLogType, eDamageEffectType damageEffectType, object p, float scale)
        {
            //throw new NotImplementedException();
            Debug.Log("miss!");
        }

        public void SetPosition(float deltaX)
        {
            base.transform.Translate(new Vector3(deltaX / 60f, 0f));
            this.SetLeftDirection(deltaX > 0f);
            this.fixedPosition = base.transform.position * 60f;
        }

        public void SetPosition(Vector3Int fixedPos)
        {
            float num;
            if (!this.isOther)
            {
                num = ((float) (-200 - fixedPos.x)) / 60f;
            }
            else
            {
                num = ((float) fixedPos.x) / 60f;
            }
            float y = posy_fix[fixedPos.y];
            float z = posz_fix[fixedPos.y];
            base.transform.position = new Vector3(num, y, z);
            this.SetLeftDirection(this.isOther);
            this.fixedPosition = base.transform.position * 60f;
        }
        public void SetPosition(int FixedPosX)
        {
            Vector3 pos = transform.position;
            pos.x = FixedPosX / 60f;
            transform.position = pos;
            SetLeftDirection(IsOther);
            fixedPosition = pos * 60f;
        }
        public void SetPosition(Vector3 realPos)
        {
            transform.position = realPos;
            fixedPosition = realPos * 60f;
        }
        private void SetRecastTime(int skillId)
        {
            if (skillId <= 1)
            {
                this.actionRecastTime = this.ATKRecastTime;
            }
            else if (skillId == this.Skill_1Id || skillId == 1001)
            {
                this.actionRecastTime = this.Skill1RecastTime;
            }
            else if (skillId == this.Skill_2Id || skillId == 1002)
            {
                this.actionRecastTime = this.Skill2RecastTime;
            }
            else
            {
                this.actionRecastTime = 0f;
            }
            if (this.abnomeralStateDic[eAbnormalState.HASTE])
            {
                Debug.LogError("咕咕咕！");
            }
        }

        public void SetState(eActionState actionState, int nextSkillId, int skillid = 0, bool isquite = false)
        {
            if (actionState != eActionState.WALK && actionState != eActionState.IDLE)
            {
                cancalByCovert = false;
            }
            /*if(actionState == eActionState.SKILL1)
            {

            }*/
            //特效
            if (!isPause)
            {
                if(actionState == eActionState.IDLE && IsAbnormalState(eAbnormalState.HASTE))
                {
                    this.SpineController.SetTimeScale(TimeScale * 2f);
                    this.SetRecastTime(nextSkillId);
                    this.SetLeftDirection(IsNearestEnemyLeft());
                    this.SetStateIdle();
                    return;
                }
                //spineController.Resume();
            }
            if (actionState == eActionState.GAMESTART)
            {
                this.SetLeftDirection(!this.isOther);
                this.SetRecastTime(nextSkillId);
                base.StartCoroutine(this._UpdateGameStart());
            }
            else
            {
                this.SetLeftDirection(this.IsNearestEnemyLeft());
                this.SetRecastTime(nextSkillId);
                if (this.abnomeralStateDic[eAbnormalState.HASTE])
                {
                    this.SpineController.SetTimeScale(TimeScale* 2f);
                }
                else
                {
                    spineController.SetTimeScale(TimeScale);
                }
                switch (actionState)
                {
                    case eActionState.IDLE:
                        SetStateIdle();
                        break;
                    case eActionState.ATK:
                        SetStateATK();
                        break;
                    case eActionState.SKILL:
                        SetStateSkill(skillid);
                        break;
                    case eActionState.SKILL1:
                        SetStateSkill1();
                        break;
                    case eActionState.WALK:
                        SetStateWalk();
                        break;
                    case eActionState.DAMAGE:
                        SetStateDamage(isquite);
                        break;
                    case eActionState.DIE:
                        SetStateDie();
                        break;
                }
            }
        }
        private void SetStateDie()
        {
            if(actionState != eActionState.DIE)
            {
                if(actionState == eActionState.DAMAGE)
                {
                    if (!onDeadForRevial && !IsDead && UnDeadTimeHitCount<=1)
                    {
                        IsDead = true;

                    }
                }
                else
                {
                    if (!onDeadForRevial && !IsDead && UnDeadTimeHitCount<=1)
                    {
                        IsDead = true;

                    }
                    if (ActionTargetOnMe.Count <= 0&&firearmCtrlsOnMe.Count<1||UnDeadTimeHitCount>0)
                    {
                        if (ToadDatas.Count < 1)
                        {
                            if(KillBonusTarget != null)
                            {
                                KillBonusTarget.ChargeEnergy(eSetEnergyType.KILL_BONUS, staticBattleManager.EnergyStackValueDefeat, false, null, false, false, true, false);
                            }
                            if(UnDeadTimeHitCount == 1)
                            {
                                spineController.SetAnimaton(eAnimationType.die, false);
                            }
                            else if (IsDivisionSourceForDie)
                            {
                                Debug.LogError("ERROR!");
                            }
                            else
                            {
                                spineController.SetAnimaton(eAnimationType.die, false);
                            }
                            if (!onDeadForRevial)
                            {
                                OnDieForZeroHp?.Invoke(this);
                            }
                            //播放退场语音
                            targetEnemyList.Clear();
                            accumulateDamage = 0;
                            targetPlayerList.Clear();
                            actionState = eActionState.DIE;
                            if (isPartsBoss)
                            {
                                Debug.LogError("咕咕咕！");
                            }
                            foreach(var a in SummonUnitDictionary.Values)
                            {
                                a.CureAllAbnormalState();
                                //callbackDead
                            }
                            if (IsDivisionSourceForDie)
                            {
                                StartCoroutine(_UpdateUndoDivision());
                            }
                            else
                            {
                                dieCoroutineId++;
                                StartCoroutine(_UpdateDie(dieCoroutineId));
                            }
                            string msg = UnitName + "死亡";
                            BattleUIManager.Instance.LogMessage(msg,eLogMessageType.DIE, IsOther);
                        }
                        else
                        {
                            DieInToad = true;
                            ToadDatas[0].Enable = false;
                        }
                    }
                    else
                    {
                        SetState(eActionState.DAMAGE, 0, 0, true);
                    }
                }
            }
        }
        public void SetVictory()
        {
            StopAllCoroutines();
            spineController.SetAnimaton(eAnimationType.joy_long, true);
        }
        internal void ShowHitEffect(eWeaponSeType weaponSeType, Skill skill, bool isLeft, BasePartsData basePartsData)
        {
            //throw new NotImplementedException();
            Debug.Log("特效鸽了！");
        }
        private void SetStateATK()
        {
            this.actionState = eActionState.ATK;
            this.currentSkillId = 1;
            if (this.ToadDatas.Count >= 1)
            {
                Debug.LogError("咕咕咕！");
            }
            if (this.actionController.StartAction(1))
            {
                this.ChargeEnergy(eSetEnergyType.BY_ATK, this.skillStackVal, false, this, false, false, true, false);
                base.StartCoroutine(this._UpdateAttack());
            }
            else
            {
                Debug.LogError("普攻失败！");
                this.SetState(eActionState.IDLE, 0, 0, false);
            }
        }
        private void SetStateDamage(bool isQuite)
        {
            this.modeChangeEnd = false;
            if ((this.actionState != eActionState.DAMAGE) && (this.actionState != eActionState.DIE))
            {
                if (this.toadRelease)
                {
                    this.toadReleaseDamage = true;
                }
                else
                {
                    this.SpineController.SetAnimaton(eAnimationType.damage, false, 1f);
                    this.actionState = eActionState.DAMAGE;
                    this.damageCoroutineId++;
                    base.StartCoroutine(this._UpdateDamage(this.damageCoroutineId));
                }
            }
        }
        private void SetStateIdle()
        {
            if (this.currentSkillId == this.UBSkillId)
            {
            }
            this.actionState = eActionState.IDLE;
            if (this.idleOnly)
            {
            }
            if (this.standByDone)
            {
                this.SpineController.SetAnimaton(eAnimationType.idle, true, 1f);
                base.StartCoroutine(this._UpdateIdle());

            }
            else
            {
                this.SpineController.SetAnimaton(eAnimationType.standBy, true, 1f);
                base.StartCoroutine(this._UpdateStandBy());
            }
            //base.StartCoroutine(this._UpdateIdle());
        }
        private void SetStateSkill(int skillid)
        {
            if ((skillid != 0) && (skillid != 1))
            {
                if (this.actionController != null)
                {
                    this.actionState = eActionState.SKILL;
                    this.currentSkillId = skillid;
                    if (!this.abnomeralStateDic[eAbnormalState.SLIENCE])
                    {
                        if ((this.ToadDatas.Count < 1) || !this.AttackWhenSilence)
                        {
                            int skillId = (skillid == 0x3e9) ? this.Skill_1Id : this.Skill_2Id;
                            BattleUIManager.Instance.LogMessage(UnitName + "准备释放技能" + skillid, eLogMessageType.READY_ACTION,isOther);
                            if (this.actionController.StartAction(skillId))
                            {
                                this.ChargeEnergy(eSetEnergyType.BY_ATK, this.skillStackVal, false, this, false, false, true, false);
                                base.StartCoroutine(this._UpdateSkill(skillId));
                            }
                            else
                            {
                                BattleUIManager.Instance.LogMessage(UnitName + "技能" + skillid + "释放失败",eLogMessageType.ERROR, isOther);
                                this.SetState(eActionState.IDLE, 0, 0, false);
                            }
                        }
                        else if (this.AttackWhenSilence)
                        {
                            this.SetState(eActionState.ATK, 0, 0, false);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("技能数据错误！");
            }
        }
        private void SetStateSkill1()
        {
            //播放UB音效
            actionState = eActionState.SKILL1;
            currentSkillId = UBSkillId;
            if (actionController.GetIsSkillPrincessForm(UBSkillId))
            {
                //StartPrincessFormSkill
                Debug.LogError("特殊动画鸽了！");
            }
            else
            {
                if (actionController.StartAction(UBSkillId))
                {
                    StartCoroutine(_UpdateSkill1());
                }
                else
                {
                    SetState(eActionState.IDLE, 0);
                }
            }
        }
        private void SetStateWalk()
        {
            if (this.actionState != eActionState.WALK)
            {
                this.actionState = eActionState.WALK;
                this.walkCoroutineId++;
                base.StartCoroutine(this._UpdateWalk(this.walkCoroutineId));
                if (this.actionState == eActionState.WALK)
                {
                    if (this.isRunForCatchUp || this.standByDone)
                    {
                        this.SpineController.SetAnimaton(eAnimationType.run, true, 1f);
                    }
                    else
                    {
                        this.SpineController.SetAnimaton(eAnimationType.run_start, true, 1f);
                    }
                }
            }
        }
        public void SetUI(CharacterPageButton ui, CharacterBuffUIController buffui)
        {
            this.unitUI = ui;
            this.unitUI.SetButton(this);
            this.unitBuffUI = buffui;
            this.unitBuffUI.SetLeftDir(this.IsOther);
        }
        public float GetAbnormalStateMainValue(eAbnormalStateCategory _abnormalStateCategory)
        {
            return abnormalStateCategoryDataDictionary[_abnormalStateCategory].MainValue;
        }

        public float GetAbnormalStateSubValue(eAbnormalStateCategory abnormalStateCategory)
        {
            return abnormalStateCategoryDataDictionary[abnormalStateCategory].SubValue;

        }

        public BaseData BaseValues =>
            this.baseValues;

        public Vector3 FixedPosition =>
            this.fixedPosition;

        public float DeltaTimeForPause
        {
            get
            {
                if (this.isPause)
                {
                    return 0f;
                }
                return Time.deltaTime*TimeScale;
            }
        }

        public List<Queue<int>> LifeStealQueueList
        {
            get => 
                this.lifeStealQueueList;
            private set => 
                this.lifeStealQueueList = value;
        }

        public bool AttackWhenSilence
        {
            get => 
                this.attackWhenSilence;
            set => 
                this.attackWhenSilence = value;
        }

        public int SearchAreaWidth
        {
            get => 
                this.searchAreaWidth;
            set => 
                this.searchAreaWidth = value;
        }

        public string UnitName =>
            this.unitName;

        public bool IsOther =>
            this.isOther;

        public bool IsConfusionOrConvert
        {
            get => 
                this.isConfusionOrConvert;
            set => 
                this.isConfusionOrConvert = value;
        }

        public bool IdleOnly
        {
            get => 
                this.idleOnly;
            set => 
                this.idleOnly = value;
        }

        public bool IsDead
        {
            get => 
                this.isDead;
            set => 
                this.isDead = value;
        }

        public bool IsPartsBoss
        {
            get => 
                this.isPartsBoss;
            set => 
                this.isPartsBoss = value;
        }

        public bool IsLeftDir
        {
            get => 
                this.isLeftDir;
            set => 
                this.isLeftDir = value;
        }

        public float Hp
        {
            get => 
                this.hp;
            set => 
                this.hp = value;
        }

        public float BodyWidth =>
            this.bodyWidth;

        public float Energy
        {
            get => 
                this.energy;
            set => 
                this.energy = value;
        }

        public PCRCaculator.UnitData UnitData =>
            this.unitData;

        public BattleUnitBaseSpineController SpineController { get => spineController; set => spineController = value; }
        public Dictionary<UnitCtrl, AccumulativeDamageData> AccumulativeDamageDataDictionary { get => accumulativeDamageDataDictionary; set => accumulativeDamageDataDictionary = value; }
        public List<ToadData> ToadDatas { get => toadDatas; set => toadDatas = value; }
        public bool IsDivisionSourceForDamage { get => isDivisionSourceForDamage; set => isDivisionSourceForDamage = value; }
        public int UnDeadTimeHitCount { get => unDeadTimeHitCount; set => unDeadTimeHitCount = value; }
        public bool IsDivisionSourceForDie { get => isDivisionSourceForDie; set => isDivisionSourceForDie = value; }
        public bool DieInToad { get => dieInToad; set => dieInToad = value; }
        public eActionState ActionState { get => actionState; set => actionState = value; }
        public float MoveRate { get => moveRate; set => moveRate = value; }
        public float PhysicalCriticalDamageRate { get => physicalCriticalDamageRate; set => physicalCriticalDamageRate = value; }
        public float MagicalCriticalDamageRate { get => magicalCriticalDamageRate; set => magicalCriticalDamageRate = value; }
        public BasePartsData DummyPartsData { get => dummyPartsData; set => dummyPartsData = value; }
        public int KnockBackEnableCount { get => knockBackEnableCount; set => knockBackEnableCount = value; }
        public float TimeScale { get => timeScale; set => timeScale = value; }
    }
}