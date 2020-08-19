using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace PCRCaculator.Battle
{
    public class UnitActionController : MonoBehaviour,ISingletonField
    {
        public ActionParameterOnPerferbDetail AttackDetail;
        public bool UseDefaultDelay;
        public Skill Attack;
        public List<Skill> UnionBurstList = new List<Skill>();
        public List<Skill> MainSkillList = new List<Skill>();
        public List<Skill> SpecialSkillList = new List<Skill>();
        public List<Skill> UnionBurstEvolutionList = new List<Skill>();
        public List<Skill> MainSkillEvolutionList = new List<Skill>();
        public Skill Annihilation;

        private UnitCtrl Owner;
        private bool skill1IsChargeTime;
        private bool skill1Charging;
        private bool disableUBByModeChange;
        private bool modeChanging;
        private bool moveEnd;
        private Dictionary<int, Skill> skillDictionary = new Dictionary<int, Skill>();
        private bool continuousActionEndDone;
        private bool isUnionBurstOnlyOwner;//初始化时判断UBid是否为0，以此判断是否有独立UB
        private bool updateBrachMotionRunning;
        private static BattleManager staticBattleManager;

        private UnitSkillData unitSkillData;
        private UnitActionController4Json actionData_json;

        public bool MoveEnd { get => moveEnd; set => moveEnd = value; }

        public delegate int SortFunction(BasePartsData a, BasePartsData b);

        public void InterInitialize(UnitCtrl owner,bool initializeAttackOnly,UnitCtrl seOwner,int unitid)
        {
            LoadUnitActionController(unitid);
            Owner = owner;
            staticBattleManager = BattleManager.Instance;
            unitSkillData = MainManager.Instance.UnitRarityDic[unitid].skillData;
            Initialize(owner, unitid, initializeAttackOnly, seOwner);
        }
        private void Initialize(UnitCtrl owner,int unitid, bool initializeAttackOnly, UnitCtrl seOwner)
        {
            //Attack = new Skill();
            Attack.animationId = eAnimationType.attack;
            if (initializeAttackOnly)
            {
                Attack.WeaponType = seOwner.WeaponSeType;
            }
            else
            {
                Attack.WeaponType = owner.WeaponSeType;
            }
            Attack.skillAreaWidth = Owner.SearchAreaWidth;
            Attack.SkillId = 1;
            Attack.SkillName = "普攻";
            AttackAction attackAction = new AttackAction();
            attackAction.TargetAssignment = eTargetAssignment.OTHER_SIDE;
            attackAction.TargetSort = PirorityPattern.NEAR;
            attackAction.TargetNth = 0;
            attackAction.TargetNum = 1;
            attackAction.TargetWidth = Owner.SearchAreaWidth;
            attackAction.Direction = DirectionType.FRONT;
            attackAction.ActionType = eActionType.ATTACK;
            attackAction.TargetList = new List<BasePartsData>();
            attackAction.Value = new Dictionary<int, float>
            {
                { 1, 0 },{3,1}
            };
            attackAction.ActionDetail1 = owner.AtkType;
            if (UseDefaultDelay)
            {
                if (!initializeAttackOnly) { seOwner = owner; }
                if (BattleDefine.WEAPON_HIT_DELAY_DIC.TryGetValue(seOwner.WeaponMotionType, out float value))
                {
                    List<ActionExecTime> execTimes = new List<ActionExecTime>();
                    ActionExecTime execTime = new ActionExecTime();
                    execTime.Weight = 1;
                    execTime.DamageNumType = eDamageEffectType.NORMAL;
                    execTimes.Add(execTime);
                    attackAction.ExecTime = new float[1] { value };
                    attackAction.ActionWeightSum = 1;
                    attackAction.ActionExecTimeList = execTimes;
                }
            }
            else
            {
                attackAction.ExecTime = new float[1] { AttackDetail.ExecTimeForPrefab[0].Time};
                attackAction.ActionWeightSum = 1;
                attackAction.ActionExecTimeList = AttackDetail.ExecTimeForPrefab;
            }
            attackAction.ActionChildrenIndexes = new List<int>();
            //attackAction.ActionId = 
            Attack.ActionParmeters = new List<ActionParameter>();
            Attack.ActionParmeters.Add(attackAction);
            skillDictionary.Add(1, Attack);
            
            DependActionSolve(Attack);
            ExecActionOnStart(Attack);
            if (!initializeAttackOnly)
            {
                isUnionBurstOnlyOwner = unitSkillData.UB != 0;
                UnionBurstList[0].SkillNum = 0;
                UnionBurstList[0].SkillId = unitSkillData.UB;
                skillDictionary.Add(unitSkillData.UB, UnionBurstList[0]);
                //skillDictionary.Add(unitSkillData.UB_ev, UnionBurstEvolutionList[0]);
                //UnionBurstEvolutionList[0].SkillNum = 0;
                /*if (MainSkillList.Count >= 1)
                {
                    for(int i = 0; i < MainSkillList.Count; i++)
                    {
                        skillDictionary .Add(unitSkillData.)
                    }
                }*/
                skillDictionary.Add(owner.Skill_1Id, MainSkillList[0]);
                MainSkillList[0].SkillNum = 0;
                MainSkillList[0].SkillId = unitSkillData.skill_1;
                skillDictionary.Add(owner.Skill_2Id, MainSkillList[1]);
                MainSkillList[1].SkillNum = 1;
                MainSkillList[1].SkillId = unitSkillData.skill_2;
            }
            
            foreach(Skill skill in skillDictionary.Values)
            {
                SetSkillParameter(skill);
                ExecActionOnStart(skill);
                ExecActionOnWaveStart(skill);
            }
            
        }
        private void LoadUnitActionController(int unitid)
        {
            TextAsset text = Resources.Load<TextAsset>("unit_jsons/UNIT_" + unitid);
            string json = text.text;
            UnitActionController4Json ac = JsonConvert.DeserializeObject<UnitActionController4Json>(json);
            UseDefaultDelay = ac.UseDefaultDelay;
            actionData_json = ac;
            AttackDetail = ac.AttackDetail.CopyThis();
            Attack = ac.Attack.CopyThis();
            UnionBurstList = ac.GetUBList();
            UnionBurstEvolutionList = ac.GetUBEvList();
            MainSkillList = ac.GetMainSkillList();
            MainSkillEvolutionList = ac.GetMainSkillEvList();
        }

        public bool StartAction(int skillId)
        {
            if (!skillDictionary.ContainsKey(skillId))
            {
                Debug.LogError("未找到对应技能！");
                return false;
            }
            Skill skill = skillDictionary[skillId];
            skill.DefeatEnemycount = 0;
            skill.DefeatByThisSkill = false;
            skill.AlreadyAddAttackSelfSeal = false;
            skill.LifeSteal = 0;
            skill.Cancel = false;
            skill.OwnerReturnPosition = transform.position;
            skill.CountBlind = false;
            if (skill.HasAttack)
            {
                if (skill.IsLifeStealEnabled)
                {
                    for (int i = Owner.LifeStealQueueList.Count - 1; i >= 0; i--)
                    {
                        int val = Owner.LifeStealQueueList[i].Dequeue();
                        skill.LifeSteal += val;
                        if (Owner.LifeStealQueueList[i].Count <= 0)
                        {
                            Owner.LifeStealQueueList.RemoveAt(i);
                            //移除吸血图标
                        }
                    }
                }
            }
            bool isTargetExit = true;
            List<int> hasParentIndex = skill.HasParentIndexes ?? new List<int>();
            foreach (ActionParameter actionParmeter in skill.ActionParmeters)
            {
                actionParmeter.IdOffsetDictionary = new Dictionary<BasePartsData, long>();
                actionParmeter.CancelByIfForAll = false;
                actionParmeter.AdditionalValue = new Dictionary<int, float>();
                actionParmeter.MultipleValue = new Dictionary<int, float>();
                actionParmeter.DivideValue = new Dictionary<int, float>();
                if (!actionParmeter.ReferencedByReflection)
                {
                    if (!actionParmeter.isSearchAndSorted)
                    {
                        SearchAndSortTarget(skill, actionParmeter, Owner.FixedPosition, false, true);
                    }
                    actionParmeter.isSearchAndSorted = false;
                    if (actionParmeter.ActionType == eActionType.REFLEXIVE)
                    {
                        Debug.LogError("咕咕咕！");
                    }
                    isTargetExit &= (actionParmeter.TargetList.Count == 0);
                }
            }
            int k = 0;
            if (skill.BlackOutTime > 0)
            {
                foreach (ActionParameter actionParameter in skill.ActionParmeters)
                {
                    if (!hasParentIndex.Contains(k) || actionParameter.ReferencedByReflection)
                    {
                        if (actionParameter.TargetList.Count >= 1)
                        {
                            foreach (BasePartsData basePartsData in actionParameter.TargetList)
                            {
                                var targetOwner = basePartsData.Owner;
                                if (actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE)
                                {
                                    targetOwner.IsScaleChangeTarget = true;
                                }
                                //处理特效
                                //AddBlackOutTarget(...)
                            }
                        }
                    }
                    k++;
                }
            }
            if (Owner.UBSkillId == skillId)
            {
                //显示UB名字
            }
            if ((isTargetExit && !skill.ForcePlayNoTarget) || (Owner.abnomeralStateDic[eAbnormalState.SLIENCE] && !Owner.AttackWhenSilence))
            {
                //如果是boss……这段代码不可能运行到，因为目前没有己方角色有沉默，所以咕了
                BattleUIManager.Instance.LogMessage("技能出现问题！",eLogMessageType.ERROR,Owner.IsOther);
                return false;
            }
            else
            {
                //显示技能名字
                if(skillId != 1)
                {
                    BattleUIManager.Instance.ShowSkillName(skill.SkillName, this.transform);
                }
                //添加特效
                Owner.PlaySkillAnimation(skillId);
                skill.ReadySkill();
                skill.AweValue = Owner.CalcAweValue(Owner.UBSkillId == skillId, skillId == 1);
                int m = 0;
                foreach (ActionParameter actionParameter in skill.ActionParmeters)
                {
                    if ((!hasParentIndex.Contains(m) || actionParameter.ReferencedByReflection) && !actionParameter.ReferencedByEffect)
                    {
                        ExecUnitActionWithDelay(actionParameter, skill, true, true, false);
                    }

                    m++;
                }
            }
            //特效


            return true;
        }
        public void ExecUnitActionWithDelay(ActionParameter action,Skill skill,bool first,bool boneCount,bool ignoreCancel)
        {
            if (action.TargetList.Count == 0)
            {
                action.OnInitWhenNoTarget1?.Invoke();
            }
            if(action.ActionType == eActionType.CONTINUOUS_ATTACK)
            {
                Debug.LogError("咕咕咕！");
            }
            for(int i=0;i<action.TargetList.Count;i++)
            {
                if (i > action.TargetNum-1)//i从0计数，所以要补偿1
                {
                    break;
                }
                StartCoroutine(ExecActionWithDelayAndTarget(action, skill, action.TargetList[i], 0, first, boneCount, ignoreCancel));
            }
        }
        public void CancalAction(int skillId)
        {
            Debug.Log("技能" + skillId + "被取消！");
        }
        public void ExecAction(ActionParameter action,Skill skill,BasePartsData target,int num,float starttime)
        {
            //int index = skill.ActionParmeters.FindIndex(a => a == action)+1;
            //int index2 = skill.ActionParmeters.Count;
            //string loging = Owner.UnitName + "执行技能" + skill.SkillName  + "(" + index + "/" + index2 + ")"
            //    + ",目标"  + target.Owner.UnitName;
            //Debug.Log(loging);
            //BattleUIManager.Instance.LogMessage(loging, Owner.IsOther);
            Dictionary<int, float> valueDic = new Dictionary<int, float>
            {
                {1,0},{2,0},{3,0},{4,0},{5,0},{6,0},{7,0}
            };
            for(int i=1;i<8;i++)
            {
                float addValue = 0;
                float multValue = 1;
                float divVale = 1;
                if(action.AdditionalValue.TryGetValue(i,out float add))
                {
                    addValue = add;
                }
                if(action.MultipleValue.TryGetValue(i,out float mult))
                {
                    multValue = mult;
                }
                if(action.DivideValue.TryGetValue(i,out float div))
                {
                    divVale = div == 0 ? 1 : div;
                }
                valueDic[i] = (((action.Value.TryGetValue(i,out float value)?value:0) + addValue) * multValue) / divVale;
            }
            bool v52 = action.JudgeLastAndNotExeced(target.Owner, num);
            int v34 = action.AlreadyExecedData[target.Owner][num].TargetPartsNumber;
            Dictionary<int, bool> enabledChildaction = new Dictionary<int, bool>();
            if (!target.ResistStatus(action.ActionType,action.ActionDetail1, this.Owner, v52, v34 == 1, target))
            {
                if (action.JudgeIsAlreadyExeced(target.Owner, num))
                {
                    //暂时忽略子技能
                    action.ExecAction(Owner, target, num, this, skill, 0, enabledChildaction, valueDic);
                }
            }
            if(action.ActionType != eActionType.REFLEXIVE)
            {
                if (action.HitOnceDic.ContainsKey(target))
                {
                    if (action.ExecTime.Length - 1 != num)
                    {
                        return;//技能还没执行完毕，返回
                    }
                    if (!action.HitOnceDic[target])
                    {
                        foreach(int i in action.ActionChildrenIndexes)
                        {
                            skill.ActionParmeters[action.ActionChildrenIndexes[i]].OnActionEnd?.Invoke();
                        }
                        return;
                    }
                }
                ExecChildrenAction(action, skill, target, num, starttime, enabledChildaction);
            }

        }
        public void ExecChildrenAction(ActionParameter action,Skill skill,BasePartsData target,int num,float starttime,Dictionary<int,bool> enabledChildAction)
        {
            foreach(int idx in action.ActionChildrenIndexes)
            {
                ActionParameter childAction = skill.ActionParmeters[idx];
                if(childAction.ActionType != eActionType.MODE_CHANGE || childAction.ActionDetail1 != 3)
                {
                    if (!enabledChildAction.ContainsKey(childAction.ActionId) || enabledChildAction[childAction.ActionId])
                    {
                        if(action.ActionType != eActionType.ATTACK || action.ExecTime.Length - 1 == num)
                        {
                            //简化调用协程的流程
                            StartCoroutine(ExecActionWithDelayAndTarget(childAction, skill, target, starttime, false, true, false));
                        }
                    }
                }
            }
        }
        public bool HasNextAnime(int skillId)
        {
            Skill skill = skillDictionary[skillId];
            if (skill.IsPrincessForm)
            {
                return true;
            }
            //if (skill.animationId != eAnimationType.skill1)
            //{
            //    return false;
            //}
            return Owner.SpineController.HasNextAnimation();
        }
        public bool IsLoopMotionPlaying(int skillId)
        {
            //Debug.LogError("咕咕咕！");
            Skill skill = skillDictionary[skillId];
            return skill.animationId == Owner.SpineController.GetCurrentAnimationName();
        }
        public eSkillMotionType GetSkillMotionType(int skillId)
        {
            return eSkillMotionType.ATTACK;
        }
        public bool IsModeChange(int skillId)
        {
            return false;
        }
        private void SearchAndSortTarget(Skill skill,ActionParameter action,Vector3 basePosition,bool quiet,bool considerBodyWidth)
        {
            SearchTargetUnit(action, basePosition, skill, considerBodyWidth);
            SortTargetListByTargetPattern(action, Owner.transform, action.Value[1], false);
            int v19 = 0;
            int i;
            for (i=0; ; i = 1)
            {
                if (v19 >= action.TargetList.Count - action.TargetNth)
                {
                    break;
                }
                action.TargetList[v19] = action.TargetList[action.TargetNth + v19];
                v19++;
            }
            if (i==1)
            {
                int v28 = action.TargetList.Count;
                for(int j=v28;j>v28-action.TargetNth ; j--)
                {
                    action.TargetList.RemoveAt(j-1);//移除重复目标
                }
            }
            else
            {
                if (action.TargetNth > 0)
                {
                    if (action.TargetList.Count > 0)//如果目标数量小于最小优先度，选取最接近的目标代替
                    {
                        BasePartsData target = action.TargetList[action.TargetList.Count - 1];
                        action.TargetList.Clear();
                        action.TargetList.Add(target);
                    }
                }
            }
            /*if(action.TargetAssignment == eTargetAssignment.OWNER_SIDE && action.TargetNum == 1&&action.TargetList.Count>0)
            {//在ExecUnitActionWithDelay里会判定目标数量
                BasePartsData target = action.TargetList[0];
                action.TargetList.Clear();
                action.TargetList.Add(target);
            }*/
            //如果不是对敌方的单目标技能，结束
            if (action.TargetAssignment != eTargetAssignment.OTHER_SIDE || action.TargetNum != 1)
            {
                action.isSearchAndSorted = true;
                return;
            }
            //对于对敌单目标技能，判断距离
            if (action.TargetList.Count > 1)
            {
                BasePartsData target = action.TargetList[0];
                action.TargetList.Clear();
                action.TargetList.Add(target);
            }
            /*if (!Owner.IsOther)
            {
                if (Owner.IsConfusionOrConvert)
                {
                    //
                }
            }*/
            action.isSearchAndSorted = true;
        }
        /// <summary>
        /// 搜索目标单位（根据技能的assisment,area,range，考虑召唤物类型的sort)，添加所有符合条件的战斗单位为目标）
        /// </summary>
        /// <param name="actionParameter"></param>
        /// <param name="basePosition"></param>
        /// <param name="skill"></param>
        /// <param name="considerBodyWidth"></param>
        private void SearchTargetUnit(ActionParameter actionParameter,Vector3 basePosition,Skill skill,bool considerBodyWidth)
        {
            actionParameter.TargetList.Clear();
            List<UnitCtrl> targetEnemyList = (Owner.IsOther ^ Owner.IsConfusionOrConvert) ? staticBattleManager.PlayersList : staticBattleManager.EnemiesList;
            List<UnitCtrl> targetPlayetList = Owner.IsOther ? staticBattleManager.EnemiesList : staticBattleManager.PlayersList;
            List<UnitCtrl> targetList_default;
            if(actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE)
            {
                targetList_default = targetEnemyList;
            }
            else
            {
                targetList_default = targetPlayetList;
            }
            //根据优先度先筛选一部分目标
            switch (actionParameter.TargetSort)
            {
                case PirorityPattern.ALL_SUMMON_RANDOM:
                case PirorityPattern.SUMMON:
                    targetList_default=targetList_default.FindAll(a => a.SummonType == eSummonType.SUMMON || a.SummonType == eSummonType.PHANTOM);
                    break;
                case PirorityPattern.ATK_PHYSICS:
                    targetList_default = targetList_default.FindAll(a => a.AtkType == 1);
                    break;
                case PirorityPattern.ATK_MAGIC:
                    targetList_default = targetList_default.FindAll(a => a.AtkType == 2);
                    break;
                case PirorityPattern.OWN_SUMMON_RANDOM:
                    targetList_default.Clear();
                    foreach(UnitCtrl unitCtrl in Owner.SummonUnitDictionary.Values)
                    {
                        targetList_default.Add(unitCtrl);
                    }
                    targetList_default = targetList_default.FindAll(a => !(a.IdleOnly || a.IsDead));
                    break;
                default:
                    break;
            }
            if(actionParameter.TargetSort == PirorityPattern.FORWARD||actionParameter.TargetSort == PirorityPattern.BACK)
            {
                foreach(UnitCtrl unit in targetList_default)
                {
                    if (JudgeIsTarget(unit, actionParameter))
                    {
                        if (unit.IsPartsBoss)
                        {
                            Debug.LogError("咕咕咕！");
                        }
                        else
                        {
                            actionParameter.TargetList.Add(unit.GetFirstParts(false, 0));
                        }
                    }
                }
                return;
            }
            bool dir = actionParameter.Direction == DirectionType.FRONT ? !Owner.IsLeftDir : false;
            float searchTargetWidth = actionParameter.TargetWidth;
            if (searchTargetWidth <= 0)
            {
                if (skill.SkillId == 1)
                {
                    searchTargetWidth = Owner.SearchAreaWidth;
                }
                else
                {
                    searchTargetWidth = skill.skillAreaWidth;
                }
            }
            float start = dir ? 0 : -1 * searchTargetWidth;
            BasePartsData item = null;
            foreach(UnitCtrl unit1 in targetList_default)
            {
                if (unit1.IsPartsBoss)
                {
                    Debug.LogError("咕咕咕！");
                }
                else
                {
                    if(JudgeIsInTargetArea(actionParameter,basePosition,considerBodyWidth,
                        1, start, actionParameter.TargetWidth, unit1.GetFirstParts(false, 0))){
                        if (JudgeIsTarget(unit1, actionParameter))
                        {
                            actionParameter.TargetList.Add(unit1.GetFirstParts(false, 0));
                        }
                        else
                        {
                            if (unit1.Hp > 0)
                            {
                                item = unit1.GetFirstParts(false, 0);
                            }
                        }
                    }
                }
            }
            if(actionParameter.TargetSort == PirorityPattern.NEAR)
            {
                if (item != null && actionParameter.TargetList.Count == 0)
                {
                    if (!Owner.IsConfusionOrConvert)
                    {
                        actionParameter.TargetList.Add(item);
                    }
                }
            }
        }
        /// <summary>
        /// 按有限度要求对目标列表进行排序
        /// </summary>
        /// <param name="actionParameter"></param>
        /// <param name="baaseTransform"></param>
        /// <param name="value"></param>
        /// <param name="quiet"></param>
        private void SortTargetListByTargetPattern(ActionParameter actionParameter,Transform baaseTransform,float value,bool quiet)
        {
            List<BasePartsData> targetList = actionParameter.TargetList;
            SortFunction sort = (a, b) => a.GetButtontransformPosition().x > b.GetButtontransformPosition().x ? 1 :
            (a.GetButtontransformPosition().x < b.GetButtontransformPosition().x ?-1:0);
            switch (actionParameter.TargetSort)
            {
                case PirorityPattern.RANDOM:
                case PirorityPattern.RANDOM_ONCE:
                case PirorityPattern.ALL_SUMMON_RANDOM:
                case PirorityPattern.OWN_SUMMON_RANDOM:
                    if (targetList.Count >= 2)
                    {
                        for(int i = targetList.Count-1; i > 0; i--)
                        {
                            int random = staticBattleManager.Random(0, i);
                            BasePartsData a = targetList[random];
                            BasePartsData b = targetList[i];
                            targetList[random] = b;
                            targetList[i] = a;
                        }
                    }
                    return;
                case PirorityPattern.NEAR:
                    sort = (a, b) => a.GetButtontransformPosition().x > b.GetButtontransformPosition().x ? 1 :
                                (a.GetButtontransformPosition().x < b.GetButtontransformPosition().x ? -1 : 0); 
                    break;
                case PirorityPattern.FAR:
                    sort = (a, b) => a.GetButtontransformPosition().x < b.GetButtontransformPosition().x ? 1 :
                                (a.GetButtontransformPosition().x > b.GetButtontransformPosition().x ? -1 : 0);
                    break;
                case PirorityPattern.HP_ASC:
                    sort = ((a, b) => a.Owner.Hp > b.Owner.Hp ? 1 : (a.Owner.Hp < b.Owner.Hp ? -1 : 0));
                    break;
                case PirorityPattern.HP_DEC:
                    sort = ((a, b) => a.Owner.Hp < b.Owner.Hp ? 1 : (a.Owner.Hp > b.Owner.Hp ? -1 : 0));
                    break;
                case PirorityPattern.OWNER:
                    targetList.Clear();
                    targetList.Add(Owner.GetFirstParts(Owner, -1024));
                    return;
                case PirorityPattern.FORWARD:
                case PirorityPattern.BACK:
                    if((actionParameter.TargetSort == PirorityPattern.FORWARD ^ Owner.IsOther) ^ Owner.IsConfusionOrConvert)
                    {
                        sort = ((a, b) => a.Owner.FixedPosition.x < b.Owner.FixedPosition.x ? 1 : 
                        (a.Owner.FixedPosition.x > b.Owner.FixedPosition.x ? -1 :0));
                    }
                    else
                    {
                        sort = ((a, b) => a.Owner.FixedPosition.x > b.Owner.FixedPosition.x ? 1 :
                        (a.Owner.FixedPosition.x < b.Owner.FixedPosition.x ? -1 : 0));
                    }
                    break;
                case PirorityPattern.ABSOLUTE_POSITION:
                    targetList.Clear();
                    targetList.Add(Owner.GetFirstParts(true, 0));
                    return;
                case PirorityPattern.ENERGY_DEC_NEAR:
                case PirorityPattern.ENERGY_DEC:                
                    sort = ((a, b) => a.Owner.Energy < b.Owner.Energy ? 1 : (a.Owner.Energy > b.Owner.Energy ? -1 :0));
                    QuichSortTargetList(targetList, sort);
                    List<BasePartsData> full = new List<BasePartsData>();
                    List<BasePartsData> notfull = new List<BasePartsData>();
                    foreach(BasePartsData a in targetList)
                    {
                        if (a.Owner.Energy == 1000)
                        {
                            full.Add(a);
                        }
                        else
                        {
                            notfull.Add(a);
                        }
                    }
                    notfull.AddRange(full);
                    targetList = notfull;
                    return;
                case PirorityPattern.ENERGY_REDUCING:
                case PirorityPattern.ENERGY_ASC:
                    sort = ((a, b) => a.Owner.Energy > b.Owner.Energy ? 1 : (a.Owner.Energy < b.Owner.Energy ? -1 : 0));
                    break;
                case PirorityPattern.ATK_DEC:
                    sort = ((a, b) => a.Owner.BaseValues.Atk < b.Owner.BaseValues.Atk ? 1 : 
                    (a.Owner.BaseValues.Atk > b.Owner.BaseValues.Atk ? -1:0));
                    break;
                case PirorityPattern.ATK_ASC:
                    sort = ((a, b) => a.Owner.BaseValues.Atk > b.Owner.BaseValues.Atk ? 1 :
                    (a.Owner.BaseValues.Atk < b.Owner.BaseValues.Atk ? -1 : 0));
                    break;
                case PirorityPattern.MAGIC_STR_DEC:
                    sort = ((a, b) => 
                    a.Owner.BaseValues.Magic_str < b.Owner.BaseValues.Magic_str ? 1 : 
                    (a.Owner.BaseValues.Magic_str > b.Owner.BaseValues.Magic_str ?-1:0));
                    break;
                case PirorityPattern.MAGIC_STR_ASC:
                    sort = ((a, b) => 
                    a.Owner.BaseValues.Magic_str > b.Owner.BaseValues.Magic_str ? 1 :
                    (a.Owner.BaseValues.Magic_str < b.Owner.BaseValues.Magic_str ? -1:0));
                    break;
                case PirorityPattern.BOSS:
                    Debug.LogError("以BOSS为优先度的排序鸽了！");
                    return;
                case PirorityPattern.HP_ASC_NEAR:
                    sort = ((a, b) => a.Owner.Hp > b.Owner.Hp ? 1 : (a.Owner.Hp < b.Owner.Hp ? -1 : 0));
                    Debug.LogError("该排序暂时不考虑距离！");
                    break;
                case PirorityPattern.HP_DEC_NEAR:
                    sort = ((a, b) => a.Owner.Hp < b.Owner.Hp ? 1 : (a.Owner.Hp > b.Owner.Hp ? -1 : 0));
                    break;
                case PirorityPattern.MAGIC_STR_DEC_NEAR:
                    sort = ((a, b) =>
                    a.Owner.BaseValues.Magic_str < b.Owner.BaseValues.Magic_str ? 1 :
                    (a.Owner.BaseValues.Magic_str > b.Owner.BaseValues.Magic_str ? -1 : 0));
                    Debug.LogError("该排序暂时不考虑距离！");
                    break;
                case PirorityPattern.SHADOW:
                    Debug.LogError("以SHADOW为优先度的排序鸽了！");
                    return;
                case PirorityPattern.ATK_MAGIC:
                    //也就新年u1有这个还是群体，不排序也无所谓
                    break;







                default:
                    //sort = ((a, b) => a.GetButtontransformPosition().x < b.GetButtontransformPosition().x ? 1 : -1);

                    //Debug.LogError("以" + actionParameter.TargetSort.ToString() + "为优先度的排序鸽了！");
                    break;


            }
            QuichSortTargetList(targetList, sort);
        }
        /// <summary>
        /// 判断目标是否活着，是否是特殊召唤物或隐身状态
        /// </summary>
        /// <param name="unitCtrl"></param>
        /// <param name="actionParameter"></param>
        /// <returns></returns>
        private bool JudgeIsTarget(UnitCtrl unitCtrl,ActionParameter actionParameter)
        {
            if(unitCtrl.isStealth||(unitCtrl.SummonType == eSummonType.PHANTOM&&actionParameter.TargetSort != PirorityPattern.OWN_SUMMON_RANDOM)){
                return false;
            }
            if ((!unitCtrl.IsDead && unitCtrl.Hp>0)||unitCtrl.IsDead&&unitCtrl.HasUnDeadTime)
            {
                if (actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE)
                {
                    return Owner != unitCtrl;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断目标是否在范围内（未完成）
        /// </summary>
        /// <param name="actionParameter"></param>
        /// <param name="basePosition">FIXEDPOSITION</param>
        /// <param name="considerbodywidth"></param>
        /// <param name="parentLossyScale"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="unitCtrl"></param>
        /// <returns></returns>
        private bool JudgeIsInTargetArea(ActionParameter actionParameter,Vector3 basePosition,bool considerbodywidth,
            float parentLossyScale,float start,float end,BasePartsData unitCtrl)
        {
            if (!unitCtrl.GetTargetable())
            {
                return false;
            }
            float distance = Mathf.Abs((unitCtrl.GetPosition().x - basePosition.x) / parentLossyScale);
            float dis2 = unitCtrl.GetBodyWidth();
            if (considerbodywidth)
            {
                dis2 += Owner.BodyWidth;
            }
            if (distance >= start - 0.5f * dis2 && distance <= dis2 * 0.5f + end)
            {
                return true;
            }
            return actionParameter.Direction == DirectionType.ALL;
        }
        private void DependActionSolve(Skill skill)
        {
            int i = 0;
            foreach(ActionParameter action in skill.ActionParmeters)
            {
                if (action.DependActionId != 0)
                {
                    foreach(ActionParameter action1 in skill.ActionParmeters)
                    {
                        if (action.DependActionId == action1.ActionId)
                        {
                            action1.ActionChildrenIndexes.Add(i);
                        }
                    }
                }
                i++;
            }
            skill.HasParentIndexes = new List<int>();
            foreach(ActionParameter action2 in skill.ActionParmeters)
            {
                if (action2.ActionChildrenIndexes.Count > 0)
                {
                    for(int j =0;j< action2.ActionChildrenIndexes.Count;j++)
                    {
                        skill.HasParentIndexes.Add(action2.ActionChildrenIndexes[j]);
                    }
                }
                if(action2.ActionType == eActionType.REFLEXIVE)
                {
                    foreach(int id in action2.ActionChildrenIndexes)
                    {
                        skill.ActionParmeters[id].ReferencedByReflection = true;
                    }
                }
            }
            //UpdateEffectRunTimeData(skill,skill.skilleffects,true);
        }
        private void UpdateEffectRunTimeData(Skill skill,List<NormalSkillEffect> _skillList,bool _resetflag)
        {

        }
        private void ExecActionOnStart(Skill skill)
        {
            foreach (ActionParameter actionParameter in skill.ActionParmeters)
            {
                actionParameter.ExecActionOnStart(skill, Owner, this);
            }

        }
        private void ExecActionOnWaveStart(Skill skill)
        {
            StartCoroutine(ExecActionOnWaveStart_0(skill));
        }
        private IEnumerator ExecActionOnWaveStart_0(Skill skill)
        {
            yield return null;
            foreach (ActionParameter actionParameter in skill.ActionParmeters)
            {
                actionParameter.ExecActionOnWaveStart(skill, Owner, this);
            }
        }
        /// <summary>
        /// 设置（添加）技能片段（不会设置普攻！）
        /// </summary>
        /// <param name="skill">要设置的技能</param>
        private void SetSkillParameter(Skill skill)
        {
            //忽略普攻，因为普攻已经设置过了
            if (skill.SkillId == 1) 
            {
                InitializeAction(skill);
                return; 
            }
            if(MainManager.Instance.SkillDataDic.TryGetValue(skill.SkillId,out SkillData skillData))
            {
                //如果是UB，设置UB音效
                skill.skillAreaWidth = skillData.areawidth == 0 ? Owner.SearchAreaWidth : skillData.areawidth;
                for(int i = 0; i < skillData.skillactions.Length; i++)
                {
                    int actionid = skillData.skillactions[i];
                    if (actionid == 0) { break; }
                    SkillAction actionData = MainManager.Instance.SkillActionDic[actionid];
                    //构造与actionType对应的派生类
                    ActionParameter action;
                    eActionType actionType = (eActionType)actionData.type;                    
                    action = BattleDefine.GetActionParameterByActionType(actionType,out bool success);
                    if (!success)
                    {
                        string word = Owner.UnitName + "的技能" + skillData.name +"的初始化<color=#660000>失败</color>，原因：技能片段种类" + actionType.ToString() + "鸽了！";
                        BattleUIManager.Instance.LogMessage(word,eLogMessageType.ERROR, Owner.IsOther);
                    }
                    //ActionParameter action = new ActionParameter();
                    action.ActionType = actionType;
                    action.TargetSort = (PirorityPattern)actionData.target_type;
                    action.TargetNth = actionData.target_number;
                    action.TargetNum = actionData.target_count;
                    action.TargetWidth = actionData.target_range < 1 ? Owner.SearchAreaWidth : actionData.target_range;
                    action.ActionDetail1 = actionData.details[0];
                    action.ActionDetail2 = actionData.details[1];
                    action.ActionDetail3 = actionData.details[2];
                    action.Value = new Dictionary<int, float>();
                    for(int j = 0; j < actionData.values.Length; j++)
                    {
                        action.Value.Add(j +1, actionData.values[j]);
                    }
                    action.MasterData_mine = actionData;
                    //维护actiontype字典
                    ActionParameterOnPrefab ac_pr = skill.ActionParametersOnPrefab.Find(a => (int)a.ActionType == actionData.type);
                    if(ac_pr == null)
                    {
                        skill.ActionParametersOnPrefab.Add(new ActionParameterOnPrefab { ActionType = (eActionType)actionData.type });
                    }
                    List<ActionParameterOnPerferbDetail> p = ac_pr.Details.FindAll(a => a.ActionId == actionid);
                    action.ActionExecTimeList = new List<ActionExecTime>();
                    float weightsum = 0;
                    foreach(ActionParameterOnPerferbDetail a in p)
                    {
                        foreach(ActionExecTime b in a.ExecTimeForPrefab)
                        {
                            action.ActionExecTimeList.Add(b);
                            weightsum += b.Weight;
                        }
                    }
                    List<float> exTimeList = new List<float>();
                    foreach(ActionExecTime actionExecTime in action.ActionExecTimeList)
                    {
                        exTimeList.Add(actionExecTime.Time);
                    }
                    action.ExecTime = exTimeList.ToArray();
                    action.ActionWeightSum = weightsum;
                    action.DependActionId = skillData.dependactions[i];
                    action.ActionId = actionid;
                    action.TargetList = new List<BasePartsData>();
                    action.ActionChildrenIndexes = new List<int>();
                    action.TargetAssignment = (eTargetAssignment)actionData.target_assigment;
                    action.Direction = (DirectionType)actionData.target_area;
                    skill.ActionParmeters.Add(action);
                }
                skill.CastTime = skillData.casttime;
                if(MainManager.Instance.SkillNameAndDescribe_cn.TryGetValue(skill.SkillId,out string[] str))
                {
                    skill.SkillName = str[0];
                }
                else
                {
                    skill.SkillName = skillData.name;
                }
                switch (skill.SkillMotionType)
                {
                    case eSkillMotionType.DEFAULT:
                        //Debug.LogError(Owner.UnitName + "的技能" + skill.SkillId + "的动画id还没有设置！");
                        break;
                    case eSkillMotionType.AWAKE:
                        //skill.animationId = eAnimationType.awake;
                        break;
                    case eSkillMotionType.ATTACK:
                        skill.animationId = eAnimationType.attack;
                        break;
                    case eSkillMotionType.EVOLUTION:
                        //skill.animationId = eAnimationType.evolution;
                        break;
                }
                DependActionSolve(skill);
                InitializeAction(skill);
                if(skill.SkillId == Owner.UBSkillId)
                {
                    skill.SetLevel(Owner.UnitData.skillLevel[0]);
                    skill.animationId = eAnimationType.skill0;
                }
                else if(skill.SkillId == Owner.Skill_1Id)
                {
                    skill.SetLevel(Owner.UnitData.skillLevel[1]);
                    skill.animationId = eAnimationType.skill1;
                }
                else if(skill.SkillId == Owner.Skill_2Id)
                {
                    skill.SetLevel(Owner.UnitData.skillLevel[2]);
                    skill.animationId = eAnimationType.skill2;
                }
                else
                {
                    Debug.LogError("技能等级初始化出错！");
                }
            }
            else
            {
                Debug.LogError(Owner.UnitName + " 的技能" + skill.SkillId + "初始化失败！");
            }
        }
        /// <summary>
        /// 初始化技能（会遍历技能的所有片段并初始化）
        /// </summary>
        /// <param name="skill">要初始化的技能</param>
        private void InitializeAction(Skill skill)
        {
            foreach(ActionParameter action in skill.ActionParmeters)
            {
                action.Initialize();
            }
        }

        public IEnumerator ExecActionWithDelayAndTarget(ActionParameter action,Skill skill,BasePartsData target,float starttime,bool first,bool boneCount,bool ignoreCancel)
        {
            //Debug.Log(Owner.UnitName +  "开始执行技能" + skill.SkillName);
            for(int i = 0; i < action.ExecTime.Length; i++)
            {
                if (boneCount)
                {
                    action.AppendTargetNum(target.Owner, i);
                }
            }
            float time0 = starttime;
            if (first)
            {
                time0 += skill.CutInSkipTime;
            }
            int i_5 = 0;
            /*float[] ids = action.ExecTime;
            for(int i = 0; i < action.ExecTime.Length; i++)
            {
                ids[i] = 0;

            }*/
            //target.Owner.ActionTargetOnMe.Add(actionin)
            if (action.ExecTime.Length >= 1)
            {
                time0 += action.ExecTime[0];
            }
            else
            {
                Debug.LogError("技能持续时间错误！");
            }
            time0 += action.ExecTime[0];
            while (i_5 <= action.ExecTime.Length - 1)
            {
                if (!ignoreCancel)
                {
                    if (skill.Cancel || action.CancelByIfForAll)
                    {
                        action.OnActionEnd?.Invoke();

                        //target.Owner.ActionTargetOnMe.Remove()
                        Debug.Log("技能被取消！");
                        yield break;
                    }
                }
                time0 -= Owner.DeltaTimeForPause;
                if (time0 >= 0)
                {
                    if(action.ActionType!= eActionType.TRIGER || action.ActionDetail1 != 4)
                    {
                        yield return null;
                        continue;
                    }
                }
                ExecAction(action, skill, target, i_5, 0);
                //持续攻击……
                if(action.ActionType == eActionType.MODE_CHANGE&&action.ActionDetail1!=3)
                {
                    //咕咕咕
                }
                else
                {
                    //target.Owner.ActionTargetOnMe.Remove()
                }
                if (i_5 == action.ExecTime.Length - 1)
                {
                    if(action.ActionType!= eActionType.CONTINUOUS_ATTACK&&action.ActionType!= eActionType.MOVE)
                    {
                        action.OnActionEnd?.Invoke();
                    }
                }
                time0 += action.ExecTime[i_5];
                i_5++;
                yield return null;
            }
        }

        private static void QuichSortTargetList(List<BasePartsData> targetList,SortFunction sort,bool isAsc = true)
        {
            QuickSort(targetList, 0, targetList.Count - 1, sort);
            if (!isAsc)
            {
                targetList.Reverse();
            }
        }
        //快速排序（目标数组，数组的起始位置，数组的终止位置）

        private static void QuickSort(List<BasePartsData> numbers, int left, int right,SortFunction sort)
        {
            if (left < right)
            {
                BasePartsData middle = numbers[Mathf.FloorToInt((left + right) / 2)];
                int i = left - 1;
                int j = right + 1;
                while (true)
                {
                    while (sort(numbers[++i], middle) < 0) ;

                    while (sort(numbers[--j], middle) > 0) ;

                    if (i >= j)
                        break;

                    Swap(numbers, i, j);
                }

                QuickSort(numbers, left, i - 1,sort);
                QuickSort(numbers, j + 1, right,sort);
            }
        }

        private static void Swap(List<BasePartsData> numbers, int i, int j)
        {
            BasePartsData number = numbers[i];
            numbers[i] = numbers[j];
            numbers[j] = number;
        }
    }
}