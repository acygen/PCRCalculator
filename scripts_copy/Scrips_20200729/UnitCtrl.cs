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

    public class UnitCtrl : MonoBehaviour
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
        private float bodyWidth = 100f;
        private float timeScale = 1;
        private eActionState actionState;
        public Dictionary<eAbnormalState, bool> abnomeralStateDic = new Dictionary<eAbnormalState, bool>();
        private bool isPause;
        private bool isDead;
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
        private bool isRunForCatchUp;
        private bool toadRelease;
        private bool toadReleaseDamage;
        private bool attackPatternIsLoop;
        private Vector3 fixedPosition;
        private int currentSkillId;
        private int currentTriggerSkillId;
        private int currentActionPatternId;
        private int attackPatternIndex;
        private int nextSkillId;
        private int enemyPoint;
        private int KnockBackEnableCount = 1;
        private int lifeSteal;
        private List<UnitCtrl> targetList = new List<UnitCtrl>();
        private List<UnitCtrl> targetPlayerList = new List<UnitCtrl>();
        private List<UnitCtrl> targetEnemyList = new List<UnitCtrl>();
        private List<ToadData> toadDatas = new List<ToadData>();
        private List<int> actionsTargetOnMe = new List<int>();
        private List<FirearmCtrl> firearmCtrlsOnMe = new List<FirearmCtrl>();
        private Dictionary<int, List<int>> attackPatternDictionary = new Dictionary<int, List<int>>();
        private Dictionary<int, List<int>> attackPatternLoopDictionary = new Dictionary<int, List<int>>();


        private Dictionary<eAbnormalStateCategory, AbnormalStateCategoryData> abnormalStateCategoryDataDictionary;
        private Dictionary<UnitCtrl, AccumulativeDamageData> accumulativeDamageDataDictionary = new Dictionary<UnitCtrl, AccumulativeDamageData>();
        private List<Queue<int>> lifeStealQueueList = new List<Queue<int>>();
        private BasePartsData dummyPartsData;
        private float actionRecastTime = 0f;
        private float actionStartTimeCounter;
        private float m_fCastTimer;
        private float deltaTimeForPause;
        private float hp = 100f;
        private float energy = 0f;
        private float skillStackVal = 90f;
        private float accumulateDamage;
        private int walkCoroutineId = 0;
        private int damageCoroutineId = 0;
        public bool IsScaleChangeTarget;
        public List<long> ActionTargetOnMe = new List<long>();
        public int AtkType = 1;
        public eWeaponSeType WeaponSeType;
        public eWeaponMotionType WeaponMotionType;
        public eSummonType SummonType = eSummonType.NONE;
        public UnitCtrl SummonSource = null;
        public Dictionary<int, UnitCtrl> SummonUnitDictionary;
        public Dictionary<BuffParamKind, int> AdditionalBuffDictionary = new Dictionary<BuffParamKind, int>();
        public Dictionary<UnitCtrl, Dictionary<int, AttackSealData>> DamageSealDataDictionary = new Dictionary<UnitCtrl, Dictionary<int, AttackSealData>>();
        public Dictionary<UnitCtrl, Dictionary<int, AttackSealData>> DamageOnceOwnerSealDateDictionary = new Dictionary<UnitCtrl, Dictionary<int, AttackSealData>>();
        public Dictionary<UnitCtrl, Dictionary<int, AttackSealData>> DamageOwnerSealDataDictionary = new Dictionary<UnitCtrl, Dictionary<int, AttackSealData>>();
        public Action OnSlipDamage;
        public Action OnDodge;
        public int UBSkillId;
        public int Skill_1Id;
        public int Skill_2Id;
        public bool isStealth = false;
        public bool HasUnDeadTime = false;
        internal int PhysicalPenetrateZero;
        internal int MagicpenetrateZero;

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
                }
                if (this.Hp > 0f)
                {
                    if (this.IsUnableActionState())
                    {
                        yield return null;
                        continue;
                    }
                    if (this.SpineController.IsPlayingAnimeBattleComplete)
                    {
                        yield return null;
                        continue;
                    }
                    if (!this.toadRelease || !this.idleOnly)
                    {
                        this.SetState(eActionState.IDLE, 0, 0, false);
                    }
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
            this.SpineController.SetAnimaton(eAnimationType.run_start, true, 1f);
            if (this.isDead)
            {
                this.gameStartDone = true;
            }
            while (this.staticBattleManager.GameState == eGameBattleState.PREPARING)
            {
                yield return null;
            }
            this.StartCoroutine(this._UpdateTargets());
            this.SetState(eActionState.IDLE, this.nextSkillId, 0, false);
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
            if (this.actionState == eActionState.IDLE)
            {
                this.SpineController.SetAnimaton(eAnimationType.idle, true, 1f);
            }
        }

        private IEnumerator _UpdateTargets()
        {
            yield return null;
            while (this.actionState != eActionState.DIE)
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
                            this.targetList.Add(a5__3);
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

        internal float GetMagicCriticalDamageRateOrMin()
        {
            return 50;
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
            return 50;
        }

        internal float GetPhysicalCriticalZero()
        {
            return BaseValues.Physical_critical + GetAdditionalBuffDictionary(BuffParamKind.PHYSICAL_CRITICAL);

        }

        internal float GetAtkZero()
        {
            return BaseValues.Atk + GetAdditionalBuffDictionary(BuffParamKind.ATK);
        }

        private IEnumerator _UpdateWalk(int corId)
        {
            while (this.actionState == eActionState.WALK)
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

        public float CalcAweValue(bool isUB, bool isAttack) => 
            0f;

        private void ChargeEnergy(eSetEnergyType setEnergyType, float energy, bool hasEffect, UnitCtrl source, bool hasNumberEffect, bool isEffectTypeCommon, bool useRecoveryRate, bool isRegenerate)
        {
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

        private bool IsCancalActionState(int skillId)
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

        internal int SetDamage(DamageData damageData, bool byAttack, int actionId, ActionParameter.OnDamageHitDelegate onDamageHit, bool hasEffect, Skill skill, bool energyAdd, Action onDefeat, bool noMotion, float weight, float actionWeightSum, Func<int,float,int> p)
        {
            int randomInt = staticBattleManager.Random();
            bool critical = false;
            if(randomInt<=damageData.CriticalRate && damageData.CriticalRate != 0)
            {
                critical = true;
            }
            if(damageData.ActionType == eActionType.ATTACK)
            {
                eDamageType damageType = damageData.DamageType;
                if((damageType == eDamageType.ATK && IsAbnormalState(eAbnormalState.LOG_ATK_BARRIR))||
                   (damageType == eDamageType.MGC && IsAbnormalState(eAbnormalState.LOG_MGC_BARRIR))||
                   IsAbnormalState(eAbnormalState.LOG_ALL_BARRIR))
                {
                    critical = damageData.IsLogBarrierCritical;
                }
            }
            int damage = SetDamageImpl(damageData, byAttack, onDamageHit, hasEffect, skill, energyAdd, critical, onDefeat, noMotion, p);
            //boss相关代码鸽了
            if(damageData.Source!= null)
            {
                if(damageData.ActionType != eActionType.DESTROY)
                {
                    if(damageData.ActionType != eActionType.ATTACK_FIELD&&(skill == null || skill.IsLifeStealEnabled))
                    {
                        int lifesteal = damageData.LifeSteal;
                        if(skill!= null)
                        {
                            lifesteal += skill.LifeSteal;
                        }
                        if (lifesteal >= 1)
                        {
                            float recovery_value = lifeSteal * damage / (lifeSteal + UnitData.level + 100);
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
                    if(damageData.Source.SummonType != eSummonType.NONE)
                    {
                        source = source.SummonSource;
                    }
                    //伤害统计
                    if(source != null)
                    {
                      //source.UnitDamageinfo.Setdamage(damage + 原伤害)
                    }
                }
            }
            accumulateDamage += damage;
            //log
            if (skill != null)
            {
                if(damageData.Source!= null)
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
                    foreach(AttackSealData sealData in dic.Values)
                    {
                        if (!sealData.OnlyCritical || critical)
                        {
                            sealData.AddSeal(this);
                        }
                    }
                }
                if(damage>=1&&damageData.Source.DamageOnceOwnerSealDateDictionary.ContainsKey(damageData.Source)&&skill!= null)
                {
                    if (!skill.AlreadyAddAttackSelfSeal)
                    {
                        Dictionary<int, AttackSealData> dic2 = DamageOnceOwnerSealDateDictionary[damageData.Source];
                        foreach(AttackSealData sealData in dic2.Values)
                        {
                            sealData.AddSeal(damageData.Source);
                        }
                        skill.AlreadyAddAttackSelfSeal = true;
                    }
                }
            }
            if(damageData.Source == null || damage < 1 || !damageData.Source.DamageOwnerSealDataDictionary.ContainsKey(damageData.Source))
            {
                if(skill == null) { return damage; }
                skill.TotalDamage += damage;
                return damage;
            }
            foreach(AttackSealData sealData1 in damageData.Source.DamageOwnerSealDataDictionary[damageData.Source].Values)
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
            if(skill != null)
            {
                skill.TotalDamage += damage;
            }
            return damage;
        }
        internal int SetDamageImpl(DamageData damageData, bool byAttack, ActionParameter.OnDamageHitDelegate onDamageHit, bool hasEffect, Skill skill, bool energyAdd, bool critical,Action onDefeat, bool noMotion,Func<int,float,int> p)
        {
            return 999;
        }
        public void SetRecovery(int _value, eInhibitHealType _inhibitHealType, UnitCtrl _source, bool _isEffect = true, bool _isRevival = false, bool _isUnionBurstLifeSteal = false, bool _isRegenerate = false, bool _useNumberEffect = true, BasePartsData _target = null)
        {

        }


        internal void SetTimeScale(float scale)
        {
            timeScale = scale;
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

        private bool IsUnableActionState()
        {
            for (int i = 0; i < unableActionNumber.Length; i++)
            {
                if (this.abnomeralStateDic[(eAbnormalState) i])
                {
                    return true;
                }
            }
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

        public void PlaySkillAnimation(int skillId)
        {
            if (skillId == this.UBSkillId)
            {
                this.SpineController.SetAnimaton(eAnimationType.skill0, false, 1f);
            }
            else if (skillId == 1)
            {
                this.SpineController.SetAnimaton(eAnimationType.attack, false, 1f);
            }
            else if (skillId == this.Skill_1Id)
            {
                this.SpineController.SetAnimaton(eAnimationType.skill1, false, 1f);
            }
            else if (skillId == this.Skill_2Id)
            {
                this.SpineController.SetAnimaton(eAnimationType.skill2, false, 1f);
            }
            else
            {
                Debug.LogError("技能动画设置失败！");
            }
        }

        public void SetAbnormalState(UnitCtrl _source, eAbnormalState abnomeralState, float effectTime, ActionParameter action, Skill skill, float value, float value2, bool reduceEnergy, float reduceEnergyRate)
        {
            if ((this.staticBattleManager.GameState == eGameBattleState.FIGHTING) && ((!this.IsAbnormalState(eAbnormalState.NO_DAMAGE_MOTION) && !this.IsAbnormalState(eAbnormalState.NO_ABNORMAL)) || ABNORMAL_CONST_DATA[abnomeralState].IsBuff))
            {
                switch (((int) abnomeralState))
                {
                    case 9:
                    case 10:
                    case 11:
                    case 0x22:
                    case 0x27:
                        this.OnSlipDamage();
                        break;
                }
            }
        }

        private void SetLeftDirection(bool isLeft = false)
        {
            Vector3 one = Vector3.one;
            if (!isLeft)
            {
                one.x = -1f;
            }
            base.transform.localScale = one;
            this.isLeftDir = isLeft;
        }

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
            for (int k = 1; k <= 0x2f; k++)
            {
                this.abnomeralStateDic.Add((eAbnormalState) k, false);
            }
            if (isOther)
            {
                this.moveRate *= -1f;
            }
            this.dummyPartsData = new BasePartsData();
            this.dummyPartsData.PositionX = 0f;
            this.dummyPartsData.Owner = this;
            this.ApplyPassitiveSkill();
            if (this.BaseValues.Magic_str > this.BaseValues.Atk)
            {
                this.AtkType = 2;
            }
            this.LoadUnitActionController(unitData.unitId);
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
                    this.SpineController.SetTimeScale(2f);
                }
                switch (actionState)
                {
                    case eActionState.IDLE:
                        this.SetStateIdle();
                        break;

                    case eActionState.ATK:
                        this.SetStateATK();
                        break;

                    case eActionState.SKILL:
                        this.SetStateSkill(skillid);
                        break;

                    case eActionState.WALK:
                        this.SetStateWalk();
                        break;

                    case eActionState.DAMAGE:
                        this.SetStateDamage(isquite);
                        break;
                }
            }
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
            }
            else
            {
                this.SpineController.SetAnimaton(eAnimationType.standBy, true, 1f);
                base.StartCoroutine(this._UpdateStandBy());
            }
            base.StartCoroutine(this._UpdateIdle());
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
                            if (this.actionController.StartAction(skillId))
                            {
                                this.ChargeEnergy(eSetEnergyType.BY_ATK, this.skillStackVal, false, this, false, false, true, false);
                                base.StartCoroutine(this._UpdateSkill(skillId));
                            }
                            else
                            {
                                Debug.LogError("技能失败！");
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
                return Time.deltaTime*timeScale;
            }
        }

        public List<Queue<int>> LifeStealQueueList
        {
            get => 
                this.lifeStealQueueList;
            private set => 
                this.lifeStealQueueList = value;
        }

        public int LifeSteal
        {
            get => 
                this.lifeSteal;
            set => 
                this.lifeSteal = value;
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
        /*
[CompilerGenerated]
private sealed class <_UpdateAttack>d__163 : IEnumerator<object>, IEnumerator, IDisposable
{
private int <>1__state;
private object <>2__current;
public UnitCtrl <>4__this;

[DebuggerHidden]
public <_UpdateAttack>d__163(int <>1__state)
{
this.<>1__state = <>1__state;
}

private bool MoveNext()
{
switch (this.<>1__state)
{
case 0:
this.<>1__state = -1;
break;

case 1:
this.<>1__state = -1;
break;

default:
return false;
}
if (this.<>4__this.IsCancalActionState(1))
{
this.<>4__this.actionController.CancalAction(1);
}
else if (!this.<>4__this.spineController.IsPlayingAnimeBattleComplete)
{
this.<>2__current = null;
this.<>1__state = 1;
return true;
}
this.<>4__this.cancalByAwake = false;
this.<>4__this.cancalByCovert = false;
this.<>4__this.cancalByToad = false;
this.<>4__this.SetState(eActionState.IDLE, 0, 0, false);
return false;
}

[DebuggerHidden]
void IEnumerator.Reset()
{
throw new NotSupportedException();
}

[DebuggerHidden]
void IDisposable.Dispose()
{
}

object IEnumerator<object>.Current =>
this.<>2__current;

object IEnumerator.Current =>
this.<>2__current;
}*/
        /*
        [CompilerGenerated]
        private sealed class <_UpdateDamage>d__165 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private object <>2__current;
            public int damageCorId;
            public UnitCtrl <>4__this;
            private float <time>5__1;

            [DebuggerHidden]
            public <_UpdateDamage>d__165(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        this.<time>5__1 = 0f;
                        while (this.<>4__this.actionState == eActionState.DAMAGE)
                        {
                            if (this.damageCorId != this.<>4__this.damageCoroutineId)
                            {
                                return false;
                            }
                            if (this.<>4__this.Hp <= 0f)
                            {
                                goto Label_0117;
                            }
                            if (!this.<>4__this.IsUnableActionState())
                            {
                                goto Label_00AB;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 1;
                            return true;
                        Label_009F:
                            this.<>1__state = -1;
                            continue;
                        Label_00AB:
                            if (!this.<>4__this.spineController.IsPlayingAnimeBattleComplete)
                            {
                                goto Label_00DE;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 2;
                            return true;
                        Label_00D2:
                            this.<>1__state = -1;
                            continue;
                        Label_00DE:
                            if (!this.<>4__this.toadRelease || !this.<>4__this.idleOnly)
                            {
                                this.<>4__this.SetState(eActionState.IDLE, 0, 0, false);
                            }
                            return false;
                        Label_0117:
                            if ((this.<>4__this.actionsTargetOnMe.Count <= 0) && (this.<>4__this.firearmCtrlsOnMe.Count <= 0))
                            {
                                this.<>4__this.spineController.Resume();
                                this.<>4__this.actionState = eActionState.IDLE;
                                this.<>4__this.SetState(eActionState.DIE, 0, 0, false);
                                return false;
                            }
                            this.<time>5__1 += Time.deltaTime;
                            if (this.<time>5__1 <= 10f)
                            {
                                continue;
                            }
                            this.<>4__this.firearmCtrlsOnMe.Clear();
                            this.<>4__this.actionsTargetOnMe.Clear();
                            this.<>2__current = null;
                            this.<>1__state = 3;
                            return true;
                        Label_01DE:
                            this.<>1__state = -1;
                        }
                        return false;

                    case 1:
                        goto Label_009F;

                    case 2:
                        goto Label_00D2;

                    case 3:
                        goto Label_01DE;
                }
                return false;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            object IEnumerator<object>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }*/
        /*
        [CompilerGenerated]
        private sealed class <_UpdateGameStart>d__159 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private object <>2__current;
            public UnitCtrl <>4__this;
            private float <time_count>5__1;

            [DebuggerHidden]
            public <_UpdateGameStart>d__159(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        this.<time_count>5__1 = 0f;
                        this.<>4__this.spineController.Pause(true);
                        this.<time_count>5__1 += this.<>4__this.DeltaTimeForPause;
                        while (this.<time_count>5__1 <= this.<>4__this.bossAppearDelay)
                        {
                            this.<time_count>5__1 += this.<>4__this.DeltaTimeForPause;
                            this.<>2__current = null;
                            this.<>1__state = 1;
                            return true;
                        Label_008E:
                            this.<>1__state = -1;
                        }
                        this.<>4__this.spineController.Resume();
                        this.<>4__this.spineController.SetAnimaton(eAnimationType.run_start, true, 1f);
                        if (this.<>4__this.isDead)
                        {
                            this.<>4__this.gameStartDone = true;
                            return false;
                        }
                        while (this.<>4__this.staticBattleManager.GameState == eGameBattleState.PREPARING)
                        {
                            this.<>2__current = null;
                            this.<>1__state = 2;
                            return true;
                        Label_010A:
                            this.<>1__state = -1;
                        }
                        this.<>4__this.StartCoroutine(this.<>4__this._UpdateTargets());
                        this.<>4__this.SetState(eActionState.IDLE, this.<>4__this.nextSkillId, 0, false);
                        return false;

                    case 1:
                        goto Label_008E;

                    case 2:
                        goto Label_010A;
                }
                return false;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            object IEnumerator<object>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }*/
        /*
        [CompilerGenerated]
        private sealed class <_UpdateIdle>d__161 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private object <>2__current;
            public UnitCtrl <>4__this;
            private int <state>5__1;
            private List<int> <list>5__2;
            private int <currentAction>5__3;
            private int <currentSkillid>5__4;
            private int <actionid>5__5;

            [DebuggerHidden]
            public <_UpdateIdle>d__161(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        this.<state>5__1 = 0;
                        this.<>4__this.m_fCastTimer = this.<>4__this.ATKRecastTime;
                        while (this.<state>5__1 >= 0)
                        {
                            if (!this.<>4__this.idleStartAfterWaitFrame)
                            {
                                goto Label_00BD;
                            }
                            this.<state>5__1 = 1;
                            this.<>4__this.idleStartAfterWaitFrame = false;
                            this.<>2__current = null;
                            this.<>1__state = 1;
                            return true;
                        Label_00B1:
                            this.<>1__state = -1;
                            continue;
                        Label_00BD:
                            if (!this.<>4__this.modeChangeEnd)
                            {
                                goto Label_00F0;
                            }
                            this.<state>5__1 = 2;
                            this.<>2__current = null;
                            this.<>1__state = 2;
                            return true;
                        Label_00E4:
                            this.<>1__state = -1;
                            continue;
                        Label_00F0:
                            if (this.<>4__this.joyFlag)
                            {
                                this.<>4__this.joyFlag = false;
                                return false;
                            }
                            if (this.<>4__this.idleOnly)
                            {
                                if (this.<>4__this.enemyPoint != 0)
                                {
                                }
                                goto Label_04F7;
                            }
                            if (this.<>4__this.attackPatternLoopDictionary[this.<>4__this.currentActionPatternId].Count != 0)
                            {
                                goto Label_0183;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 3;
                            return true;
                        Label_0177:
                            this.<>1__state = -1;
                            continue;
                        Label_0183:
                            if (this.<>4__this.actionState == eActionState.IDLE)
                            {
                            }
                            if (this.<>4__this.actionState > eActionState.IDLE)
                            {
                                return false;
                            }
                            if (!BattleManager.Instance.BlackOutUnitList.Contains(this.<>4__this))
                            {
                                goto Label_01E8;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 4;
                            return true;
                        Label_01DC:
                            this.<>1__state = -1;
                            continue;
                        Label_01E8:
                            if (this.<>4__this.isConfusionOrConvert)
                            {
                                if (!this.<>4__this.targetPlayerList.Contains(this.<>4__this) && (this.<>4__this.targetPlayerList.Count != 1))
                                {
                                    this.<>4__this.SetState(eActionState.WALK, 0, 0, false);
                                    return false;
                                }
                            }
                            else if (this.<>4__this.targetEnemyList.Count <= 0)
                            {
                                this.<>4__this.isRunForCatchUp = true;
                                this.<>4__this.SetState(eActionState.WALK, 0, 0, false);
                                return false;
                            }
                            if (((this.<>4__this.actionStartTimeCounter <= 0f) || this.<>4__this.isOther) && !this.<>4__this.toadRelease)
                            {
                                this.<>4__this.m_fCastTimer -= this.<>4__this.DeltaTimeForPause;
                            }
                            if (!this.<>4__this.toadRelease)
                            {
                                goto Label_0312;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 5;
                            return true;
                        Label_0306:
                            this.<>1__state = -1;
                            continue;
                        Label_0312:
                            if ((this.<>4__this.m_fCastTimer <= 0f) && (this.<>4__this.attackPatternDictionary != null))
                            {
                                goto Label_0383;
                            }
                            if (this.<>4__this.Hp <= 0f)
                            {
                                goto Label_0371;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 6;
                            return true;
                        Label_0365:
                            this.<>1__state = -1;
                            continue;
                        Label_0371:
                            this.<>4__this.SetState(eActionState.DIE, 0, 0, false);
                            return false;
                        Label_0383:
                            this.<list>5__2 = this.<>4__this.attackPatternIsLoop ? this.<>4__this.attackPatternLoopDictionary[this.<>4__this.currentActionPatternId] : this.<>4__this.attackPatternDictionary[this.<>4__this.currentActionPatternId];
                            this.<currentAction>5__3 = this.<list>5__2[this.<>4__this.attackPatternIndex];
                            this.<currentSkillid>5__4 = this.<list>5__2[this.<>4__this.attackPatternIndex];
                            if (this.<>4__this.attackPatternIndex == (this.<list>5__2.Count - 1))
                            {
                                this.<>4__this.attackPatternIndex = 0;
                                this.<>4__this.attackPatternIsLoop = true;
                                this.<actionid>5__5 = this.<>4__this.attackPatternLoopDictionary[this.<>4__this.currentActionPatternId][0];
                            }
                            else
                            {
                                this.<>4__this.attackPatternIndex++;
                                this.<actionid>5__5 = this.<list>5__2[this.<>4__this.attackPatternIndex];
                            }
                            this.<>4__this.cancalByAwake = false;
                            if (this.<currentAction>5__3 <= 1)
                            {
                                this.<>4__this.SetState(eActionState.ATK, this.<actionid>5__5, this.<currentSkillid>5__4, false);
                                return false;
                            }
                            this.<>4__this.SetState(eActionState.SKILL, this.<actionid>5__5, this.<currentSkillid>5__4, false);
                            return false;
                        Label_04F7:
                            if (this.<>4__this.isSummonOrPhantom)
                            {
                                goto Label_0525;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 7;
                            return true;
                        Label_051C:
                            this.<>1__state = -1;
                            continue;
                        Label_0525:
                            Debug.LogError("咕咕咕！");
                            break;
                        }
                        break;

                    case 1:
                        goto Label_00B1;

                    case 2:
                        goto Label_00E4;

                    case 3:
                        goto Label_0177;

                    case 4:
                        goto Label_01DC;

                    case 5:
                        goto Label_0306;

                    case 6:
                        goto Label_0365;

                    case 7:
                        goto Label_051C;

                    default:
                        return false;
                }
                return false;
            }
            
            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            object IEnumerator<object>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }*/
        /*
        [CompilerGenerated]
        private sealed class <_UpdateSkill>d__164 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private object <>2__current;
            public int skillId;
            public UnitCtrl <>4__this;

            [DebuggerHidden]
            public <_UpdateSkill>d__164(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        break;

                    case 1:
                        this.<>1__state = -1;
                        break;

                    default:
                        return false;
                }
                if (this.<>4__this.cancalByCovert || this.<>4__this.cancalByToad)
                {
                    this.<>4__this.actionController.CancalAction(this.skillId);
                    this.<>4__this.cancalByToad = false;
                    this.<>4__this.cancalByCovert = false;
                    this.<>4__this.cancalByAwake = false;
                    return false;
                }
                if (((this.<>4__this.actionState == eActionState.SKILL1) || (this.<>4__this.actionState == eActionState.DAMAGE)) || (this.<>4__this.actionState == eActionState.DIE))
                {
                    this.<>4__this.actionController.CancalAction(this.skillId);
                    this.<>4__this.cancalByCovert = false;
                    this.<>4__this.cancalByAwake = false;
                    return false;
                }
                if (this.<>4__this.cancalByAwake && (this.skillId != this.<>4__this.currentTriggerSkillId))
                {
                    this.<>4__this.actionController.CancalAction(this.skillId);
                    this.<>4__this.cancalByAwake = false;
                    this.<>4__this.cancalByCovert = false;
                    this.<>4__this.cancalByToad = false;
                    return false;
                }
                if (this.<>4__this.actionController.HasNextAnime(this.skillId) && this.<>4__this.actionController.IsLoopMotionPlaying(this.skillId))
                {
                    return false;
                }
                if (this.<>4__this.spineController.IsPlayingAnimeBattleComplete || (this.<>4__this.actionController.GetSkillMotionType(this.skillId) == eSkillMotionType.NONE))
                {
                    if (this.<>4__this.actionController.IsModeChange(this.skillId))
                    {
                        Debug.LogError("咕咕咕！");
                    }
                    if (!this.<>4__this.actionController.HasNextAnime(this.skillId))
                    {
                        this.<>4__this.SetState(eActionState.IDLE, 0, 0, false);
                        this.<>4__this.cancalByAwake = false;
                        this.<>4__this.cancalByCovert = false;
                        this.<>4__this.cancalByToad = false;
                    }
                    return false;
                }
                this.<>2__current = null;
                this.<>1__state = 1;
                return true;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            object IEnumerator<object>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }*/
        /*
        [CompilerGenerated]
        private sealed class <_UpdateStandBy>d__160 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private object <>2__current;
            public UnitCtrl <>4__this;

            [DebuggerHidden]
            public <_UpdateStandBy>d__160(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        while (!this.<>4__this.spineController.IsPlayingAnimeBattleComplete)
                        {
                            if (this.<>4__this.actionState != eActionState.IDLE)
                            {
                                goto Label_005A;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 1;
                            return true;
                        Label_0051:
                            this.<>1__state = -1;
                            continue;
                        Label_005A:
                            this.<>4__this.standByDone = true;
                            this.<>2__current = null;
                            this.<>1__state = 2;
                            return true;
                        Label_0076:
                            this.<>1__state = -1;
                        }
                        this.<>4__this.standByDone = true;
                        if (this.<>4__this.actionState == eActionState.IDLE)
                        {
                            this.<>4__this.spineController.SetAnimaton(eAnimationType.idle, true, 1f);
                        }
                        return false;

                    case 1:
                        goto Label_0051;

                    case 2:
                        goto Label_0076;
                }
                return false;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            object IEnumerator<object>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }*/
        /*
        [CompilerGenerated]
        private sealed class <_UpdateTargets>d__166 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private object <>2__current;
            public UnitCtrl <>4__this;
            private List<UnitCtrl> <list>5__1;
            private List<UnitCtrl>.Enumerator <>s__2;
            private UnitCtrl <a>5__3;

            [DebuggerHidden]
            public <_UpdateTargets>d__166(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        this.<>2__current = null;
                        this.<>1__state = 1;
                        return true;

                    case 1:
                        this.<>1__state = -1;
                        while (this.<>4__this.actionState != eActionState.DIE)
                        {
                            if (!this.<>4__this.isPause)
                            {
                                if (this.<>4__this.abnomeralStateDic[eAbnomeralState.CONFUSION] ^ this.<>4__this.isOther)
                                {
                                    this.<list>5__1 = this.<>4__this.staticBattleManager.PlayersList;
                                }
                                else
                                {
                                    this.<list>5__1 = this.<>4__this.staticBattleManager.EnemiesList;
                                }
                                this.<>4__this.targetList.Clear();
                                this.<>s__2 = this.<list>5__1.GetEnumerator();
                                try
                                {
                                    while (this.<>s__2.MoveNext())
                                    {
                                        this.<a>5__3 = this.<>s__2.Current;
                                        if (this.<>4__this.GetDistance(this.<a>5__3, true) < (this.<>4__this.SearchAreaWidth + 100))
                                        {
                                            this.<>4__this.targetList.Add(this.<a>5__3);
                                        }
                                        this.<a>5__3 = null;
                                    }
                                }
                                finally
                                {
                                    this.<>s__2.Dispose();
                                }
                                this.<>s__2 = new List<UnitCtrl>.Enumerator();
                                this.<>4__this.targetList.Sort(new Comparison<UnitCtrl>(this.<>4__this.<_UpdateTargets>b__166_0));
                                this.<>4__this.targetEnemyList = this.<>4__this.targetList;
                                this.<list>5__1 = null;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 2;
                            return true;
                        Label_01B2:
                            this.<>1__state = -1;
                        }
                        return false;

                    case 2:
                        goto Label_01B2;
                }
                return false;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            object IEnumerator<object>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }*/
        /*
        [CompilerGenerated]
        private sealed class <_UpdateWalk>d__162 : IEnumerator<object>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private object <>2__current;
            public int corId;
            public UnitCtrl <>4__this;
            private List<UnitCtrl> <targetList>5__1;
            private bool <k1>5__2;
            private bool <k2>5__3;
            private bool <k3>5__4;
            private float <speedRate_k>5__5;
            private float <deltaX>5__6;

            [DebuggerHidden]
            public <_UpdateWalk>d__162(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        while (this.<>4__this.actionState == eActionState.WALK)
                        {
                            if (this.corId != this.<>4__this.walkCoroutineId)
                            {
                                return false;
                            }
                            if (((this.<>4__this.actionState == eActionState.SKILL1) || (this.<>4__this.actionState == eActionState.DAMAGE)) || (this.<>4__this.actionState == eActionState.DIE))
                            {
                                this.<>4__this.isRunForCatchUp = false;
                                return false;
                            }
                            if (!this.<>4__this.isPause)
                            {
                                goto Label_00D2;
                            }
                            this.<>2__current = null;
                            this.<>1__state = 1;
                            return true;
                        Label_00C6:
                            this.<>1__state = -1;
                            continue;
                        Label_00D2:
                            this.<targetList>5__1 = this.<>4__this.isOther ? this.<>4__this.staticBattleManager.EnemiesList : this.<>4__this.staticBattleManager.PlayersList;
                            if (this.<>4__this.isConfusionOrConvert)
                            {
                                if (this.<targetList>5__1.FindAll(UnitCtrl.<>c.<>9__162_2 ?? (UnitCtrl.<>c.<>9__162_2 = new Predicate<UnitCtrl>(UnitCtrl.<>c.<>9.<_UpdateWalk>b__162_2))).Count == 1)
                                {
                                    this.<>4__this.isRunForCatchUp = false;
                                    this.<>4__this.SetState(eActionState.IDLE, 0, 0, false);
                                    return false;
                                }
                                if ((this.<>4__this.targetPlayerList.Count > 1) || this.<>4__this.idleOnly)
                                {
                                    this.<>4__this.SetState(eActionState.IDLE, 0, 0, false);
                                    return false;
                                }
                            }
                            else if (this.<>4__this.idleOnly || (this.<>4__this.targetEnemyList.Count > 0))
                            {
                                this.<>4__this.isRunForCatchUp = false;
                                this.<>4__this.SetState(eActionState.IDLE, 0, 0, false);
                                return false;
                            }
                            this.<k1>5__2 = this.<targetList>5__1.FindIndex(UnitCtrl.<>c.<>9__162_0 ?? (UnitCtrl.<>c.<>9__162_0 = new Predicate<UnitCtrl>(UnitCtrl.<>c.<>9.<_UpdateWalk>b__162_0))) == -1;
                            this.<k2>5__3 = this.<targetList>5__1.TrueForAll(UnitCtrl.<>c.<>9__162_1 ?? (UnitCtrl.<>c.<>9__162_1 = new Predicate<UnitCtrl>(UnitCtrl.<>c.<>9.<_UpdateWalk>b__162_1)));
                            this.<k3>5__4 = this.<>4__this.staticBattleManager.GameState == eGameBattleState.FIGHTING;
                            this.<speedRate_k>5__5 = this.<>4__this.isRunForCatchUp ? 1f : 1.6f;
                            this.<deltaX>5__6 = (this.<>4__this.moveRate * this.<speedRate_k>5__5) * this.<>4__this.DeltaTimeForPause;
                            if ((this.<k1>5__2 | this.<k2>5__3) & this.<k3>5__4)
                            {
                                goto Label_036C;
                            }
                            if ((this.<>4__this.spineController.GetCurrentAnimationName() == eAnimationType.idle) && (this.<>4__this.moveRate != 0f))
                            {
                                this.<>4__this.spineController.SetAnimaton(eAnimationType.run, true, 1f);
                            }
                            this.<>4__this.SetPosition(this.<deltaX>5__6);
                            this.<>4__this.SetLeftDirection(this.<deltaX>5__6 > 0f);
                            this.<>2__current = null;
                            this.<>1__state = 2;
                            return true;
                        Label_0360:
                            this.<>1__state = -1;
                            continue;
                        Label_036C:
                            if (this.<>4__this.spineController.GetCurrentAnimationName() == eAnimationType.run)
                            {
                                goto Label_03BB;
                            }
                            this.<>4__this.SetLeftDirection(this.<deltaX>5__6 > 0f);
                            this.<>2__current = null;
                            this.<>1__state = 3;
                            return true;
                        Label_03B2:
                            this.<>1__state = -1;
                            continue;
                        Label_03BB:
                            this.<>4__this.spineController.SetAnimaton(eAnimationType.idle, true, 1f);
                            this.<>2__current = null;
                            this.<>1__state = 4;
                            return true;
                        Label_03E3:
                            this.<>1__state = -1;
                            this.<targetList>5__1 = null;
                        }
                        return false;

                    case 1:
                        goto Label_00C6;

                    case 2:
                        goto Label_0360;

                    case 3:
                        goto Label_03B2;

                    case 4:
                        goto Label_03E3;
                }
                return false;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            object IEnumerator<object>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }*/
        /*
        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly UnitCtrl.<>c <>9 = new UnitCtrl.<>c();
            public static Predicate<UnitCtrl> <>9__162_2;
            public static Predicate<UnitCtrl> <>9__162_0;
            public static Predicate<UnitCtrl> <>9__162_1;

            internal bool <_UpdateWalk>b__162_0(UnitCtrl a) => 
                (a.Hp > 0f);

            internal bool <_UpdateWalk>b__162_1(UnitCtrl a) => 
                (a.isStealth || (a.Hp == 0f));

            internal bool <_UpdateWalk>b__162_2(UnitCtrl a) => 
                !a.isDead;
        }*/
    }
}