using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
namespace PCRCaculator
{


    public class CalculatorManager : MonoBehaviour
    {
        public static CalculatorManager Instance;
        public TextAsset db;
        public GameObject backGround;
        public GameObject equipmentCalPage_back;
        public GameObject equipmentCalPage_A;
        public GameObject equipmentCalPage_B;
        public GameObject equipmentCalPage_C;
        public GameObject calculatorSettingPrefab;
        public Text equipmentCalPage_calChar_count;
        public Text equipmentCalPage_calEquipment_count;
        public Text equipmentCalPage_megText;
        public Text equipmentCalPage_calAP;
        public Button backButton;
        public Button stepButton_1;
        public Button stepButton_2;
        public Button stepButton_3;
        public RectTransform parent_A;
        public RectTransform parent_B;
        public RectTransform parent_C;
        public GameObject unitPrefab;
        public GameObject equipPrefab;
        public GameObject questPrefab;
        public int original_hight;
        public Vector2 baseRange;//第一个按钮的位置
        public Vector2 range;//相邻按钮之间的距离
        public int original_hight_quest;
        public Vector2 baseRange_quest;//第一个按钮的位置
        public Vector2 range_quest;//相邻按钮之间的距离


        public Dictionary<int, UnitData> unitDataDicForCal_perverous;
        public Dictionary<int, UnitData> unitDataDicForCal_now;
        private List<UnitData> unitDataListForCal = new List<UnitData>();
        private List<GameObject> unitPrefabs = new List<GameObject>();
        private List<GameObject> equipPrefabs = new List<GameObject>();
        private List<GameObject> questPrefabs = new List<GameObject>();
        private Dictionary<int, int> equipentRequireDic;
        private List<int> equipmentCraftList = new List<int>();
        private Dictionary<int, int> equipmentRequireDic_2;
        private Dictionary<QuestRewardData, int> equipmentQuestDic = new Dictionary<QuestRewardData, int>();
        private Dictionary<int, int> equipmentGetedDic_Quest = new Dictionary<int, int>();


        private Dictionary<int, EquipmentCraft> equipmentCraftDic;//装备制造数据
        private Dictionary<int, QuestRewardData> questRewardDic;//地图掉落数据
        private Dictionary<int, EquipmentGet> equipmentGetDic;//装备获得数据
        private List<int> expCost;
        private List<int> skillCost;

        private int cost_nama_total = 0;
        private int cost_mana_craftEquipment = 0;//制造装备消耗的玛娜
        private int cost_mana_leveUp_equipment = 0;
        private int cost_mana_leveUp_skill = 0;

        private enum CurrentStep { step1 = 1, step2 = 2, step3 = 3 }
        private CurrentStep equipmentCalPage_step;
        [TextArea]
        public string stepmsg_1;
        [TextArea]
        public string stepmsg_2;
        [TextArea]
        public string stepmsg_3;

        public Dictionary<int, EquipmentCraft> EquipmentCraftDic { get => equipmentCraftDic; set => equipmentCraftDic = value; }
        public Dictionary<int, QuestRewardData> QuestRewardDic { get => questRewardDic; set => questRewardDic = value; }
        public Dictionary<int, EquipmentGet> EquipmentGetDic { get => equipmentGetDic; set => equipmentGetDic = value; }

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            LoadEquipmentCraftDic();
        }
        public void SwitchPage(int i)
        {
            if (i == 1)
            {
                backGround.SetActive(true);
            }
            else
            {
                backGround.SetActive(false);
            }
        }
        public void CalculateEquipButton()
        {
            equipmentCalPage_back.SetActive(true);
            equipmentCalPage_A.SetActive(true);
            equipmentCalPage_B.SetActive(false);
            CalculateEquip_SetFirst();
        }
        private void CalculateEquip_SetFirst()
        {
            equipmentCalPage_calChar_count.text = "" + 0;
            equipmentCalPage_megText.text = stepmsg_1;
            stepButton_1.interactable = true;
            stepButton_2.interactable = false;
            stepButton_3.interactable = false;
            backButton.interactable = false;
            equipmentCalPage_step = CurrentStep.step1;
            ClearUnitIcons(parent_A, unitPrefabs);
        }
        public void StepButton_1_OnClick()
        {
            equipmentCalPage_megText.text = stepmsg_2;
            stepButton_1.interactable = false;
            stepButton_2.interactable = true;
            stepButton_3.interactable = false;
            backButton.interactable = true;
            equipmentCalPage_step = CurrentStep.step2;
            if (unitDataDicForCal_perverous != null && unitDataDicForCal_perverous.Count > 0)
            {
                MainManager.Instance.WindowConfigMessage("已经设置过初始状态，是否覆盖？", Step1_A, null);
            }
            else
            {
                Step1_A();
            }
        }
        /// <summary>
        /// 覆盖初始状态
        /// </summary>
        public void Step1_A()
        {
            unitDataDicForCal_perverous = new Dictionary<int, UnitData>();
            foreach (UnitData unitData in MainManager.Instance.unitDataDic.Values)
            {
                unitDataDicForCal_perverous.Add(unitData.unitId, unitData.Copy());
            }
        }
        public void StepButton_2_OnClick()
        {
            equipmentCalPage_megText.text = stepmsg_3;
            stepButton_1.interactable = false;
            stepButton_2.interactable = false;
            stepButton_3.interactable = true;
            backButton.interactable = true;
            equipmentCalPage_step = CurrentStep.step3;
            if (unitDataDicForCal_now != null && unitDataDicForCal_now.Count > 0)
            {
                MainManager.Instance.WindowConfigMessage("已经设置过最终状态，是否覆盖？", Step2_A, Step2_B);
            }
            else
            {
                Step2_A();
            }
        }
        public void Step2_A()
        {
            unitDataDicForCal_now = new Dictionary<int, UnitData>();
            foreach (UnitData unitData in MainManager.Instance.unitDataDic.Values)
            {
                unitDataDicForCal_now.Add(unitData.unitId, unitData.Copy());
            }
            Step2_B();
        }
        public void Step2_B()
        {
            unitDataListForCal.Clear();
            foreach (UnitData a in unitDataDicForCal_now.Values)
            {
                if (a.CompairIsUp(unitDataDicForCal_perverous[a.unitId]))
                {
                    unitDataListForCal.Add(a);
                }
            }
            Step2_C();
        }
        public void Step2_C()
        {
            ClearUnitIcons(parent_A, unitPrefabs);
            int count = 1;
            foreach (UnitData unitData in unitDataListForCal)
            {

                GameObject b = Instantiate(unitPrefab);
                b.transform.SetParent(parent_A);
                b.transform.localScale = new Vector3(1, 1, 1);
                b.transform.localPosition = new Vector3(baseRange.x + range.x * ((count - 1) % 8), -1 * (baseRange.y + range.y * (Mathf.FloorToInt((count - 1) / 8))), 0);
                int id0 = unitData.unitId;
                b.GetComponent<CharacterPageButton>().SetButton(unitData.unitId);
                unitPrefabs.Add(b);
                count++;
                //showUnitIDs.Add(id);
            }
            if (parent_A.sizeDelta.y <= Mathf.CeilToInt(count / 8) * 95 + 5)
            {
                parent_A.sizeDelta = new Vector2(100, Mathf.CeilToInt(count / 8) * 95 + 5);
            }
            equipmentCalPage_calChar_count.text = "" + (count - 1);
        }
        private void ClearUnitIcons(RectTransform parent, List<GameObject> gameObjects)
        {
            foreach (GameObject a in gameObjects)
            {
                Destroy(a);
            }
            gameObjects.Clear();
            parent.localPosition = new Vector3();
            parent.sizeDelta = new Vector2(100, original_hight);

        }
        public void StepButton_3_OnClick()
        {
            equipmentCalPage_B.SetActive(true);
            Step3_A();
        }
        private void Step3_A()
        {
            equipentRequireDic = new Dictionary<int, int>();
            foreach (UnitData a in unitDataListForCal)
            {
                foreach (int equipID in a.GetRequiredEquipment(unitDataDicForCal_perverous[a.unitId]))
                {
                    if (equipentRequireDic.ContainsKey(equipID))
                    {
                        equipentRequireDic[equipID]++;
                    }
                    else
                    {
                        equipentRequireDic.Add(equipID, 1);
                    }
                }
            }
            Step3_B();
        }
        private void Step3_B()
        {
            ClearUnitIcons(parent_B, equipPrefabs);
            int count = 1;
            int requireNum = 0;
            foreach (int equipId in equipentRequireDic.Keys)
            {

                GameObject b = Instantiate(equipPrefab);
                b.transform.SetParent(parent_B);
                b.transform.localScale = new Vector3(1, 1, 1);
                b.transform.localPosition = new Vector3(baseRange.x + range.x * ((count - 1) % 8), -1 * (baseRange.y + range.y * (Mathf.FloorToInt((count - 1) / 8))), 0);
                int num = equipentRequireDic[equipId];
                b.GetComponent<EquipmentPageIcon>().SetEquipmentIcon(equipId, num);
                equipPrefabs.Add(b);
                count++;
                requireNum += num;
                //showUnitIDs.Add(id);
            }
            if (parent_B.sizeDelta.y <= Mathf.CeilToInt(count / 8) * 95 + 105)
            {
                parent_B.sizeDelta = new Vector2(100, Mathf.CeilToInt(count / 8) * 95 + 105);
            }
            equipmentCalPage_calEquipment_count.text = "" + requireNum;

        }
        public void BackButton_OnClick()
        {
            switch (equipmentCalPage_step)
            {
                case CurrentStep.step2:
                    CalculateEquip_SetFirst();
                    break;
                case CurrentStep.step3:
                    equipmentCalPage_megText.text = stepmsg_2;
                    stepButton_1.interactable = false;
                    stepButton_2.interactable = true;
                    stepButton_3.interactable = false;
                    backButton.interactable = true;
                    equipmentCalPage_step = CurrentStep.step2;
                    break;
            }
        }
        public void CancelButton_OnClick()
        {
            equipmentCalPage_back.SetActive(false);
            equipmentCalPage_A.SetActive(false);
            equipmentCalPage_B.SetActive(false);
        }
        public void CalculateEquipCraftButton()
        {
            cost_mana_craftEquipment = 0;
            equipmentCraftList.Clear();
            equipmentRequireDic_2 = new Dictionary<int, int>();
            foreach(int equipid in equipentRequireDic.Keys)
            {
                int num_A = equipentRequireDic[equipid];
                if (!equipmentGetDic[equipid].isCraft)
                {
                    if (equipmentRequireDic_2.ContainsKey(equipid))
                    {
                        equipmentRequireDic_2[equipid] += num_A;
                    }
                    else
                    {
                        equipmentRequireDic_2.Add(equipid, num_A);
                        equipmentCraftList.Add(equipid);
                    }
                }
                else
                {
                    equipmentCraftDic[equipid].GetCraftIdTotal(equipmentCraftDic, out List<int> eq, out List<int> num,out int cost, num_A);
                    for(int i = 0; i < eq.Count; i++)
                    {
                        if (eq[i] != 0)
                        {
                            if (equipmentRequireDic_2.ContainsKey(eq[i]))
                            {
                                equipmentRequireDic_2[eq[i]] += num[i];
                            }
                            else
                            {
                                equipmentRequireDic_2.Add(eq[i], num[i]);
                                equipmentCraftList.Add(eq[i]);
                            }
                            cost_mana_craftEquipment += cost;
                        }
                    }
                }
            }
            equipmentCraftList.Sort((a,b)=>b-a);
            ClearUnitIcons(parent_B, equipPrefabs);
            int count = 1;
            int requireNum = 0;
            foreach (int equipId in equipmentCraftList)
            {

                GameObject b = Instantiate(equipPrefab);
                b.transform.SetParent(parent_B);
                b.transform.localScale = new Vector3(1, 1, 1);
                b.transform.localPosition = new Vector3(baseRange.x + range.x * ((count - 1) % 8), -1 * (baseRange.y + range.y * (Mathf.FloorToInt((count - 1) / 8))), 0);
                int num = equipmentRequireDic_2[equipId];
                b.GetComponent<EquipmentPageIcon>().SetEquipmentIcon(equipId, num);
                equipPrefabs.Add(b);
                count++;
                requireNum += num;
                //showUnitIDs.Add(id);
            }
            if (parent_B.sizeDelta.y <= Mathf.CeilToInt(count / 8) * 95 + 105)
            {
                parent_B.sizeDelta = new Vector2(100, Mathf.CeilToInt(count / 8) * 95 + 105);
            }
            equipmentCalPage_calEquipment_count.text = "" + requireNum;

        }
        public void CalculateManaButton()
        {
            if(equipmentRequireDic_2 == null)
            {
                MainManager.Instance.WindowMessage("请先计算装备碎片需求！");
                return;
            }
            cost_nama_total = 0;
            cost_mana_leveUp_equipment = 0;
            cost_mana_leveUp_skill = 0;
            foreach(UnitData unitData in unitDataListForCal)
            {
                int[] rankequips = MainManager.Instance.UnitRarityDic[unitData.unitId].GetRankEquipments(unitData.rank);
                UnitData unitdata_old = unitDataDicForCal_perverous[unitData.unitId];
                if (unitData.rank > unitdata_old.rank)
                {
                    for(int i = 0; i < 6; i++)
                    {
                        cost_mana_leveUp_equipment += CalcManaForEquipEnhance(MainManager.Instance.EquipmentDic[rankequips[i]].promotion_level, 0, unitData.equipLevel[i]);
                    }
                }
                else if(unitData.rank == unitdata_old.rank)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        cost_mana_leveUp_equipment += CalcManaForEquipEnhance(MainManager.Instance.EquipmentDic[rankequips[i]].promotion_level,
                           unitdata_old.equipLevel[i] , unitData.equipLevel[i]);
                    }
                }
                for(int i = 0; i < unitData.skillLevel.Length; i++)
                {
                    if (unitData.skillLevel[i] > unitdata_old.skillLevel[i])
                    {
                        if (unitData.skillLevel[i] > skillCost.Count)
                        {
                            MainManager.Instance.WindowConfigMessage("等级超过可计算的范围！\n可计算的最大等级：" + skillCost.Count + "\n当前技能等级：" + unitData.skillLevel[i], null, null);
                            return;
                        }
                        cost_mana_leveUp_skill += skillCost[unitData.skillLevel[i]] - skillCost[unitdata_old.skillLevel[i]];
                    }
                }
            }
            cost_nama_total += cost_mana_craftEquipment + cost_mana_leveUp_equipment + cost_mana_leveUp_skill;
            string word = "总共消耗玛娜：" + cost_nama_total + "\n制造装备消耗：" + cost_mana_craftEquipment + "\n升级装备消耗：" + cost_mana_leveUp_equipment +
                "\n升级技能消耗：" + cost_mana_leveUp_skill;
            MainManager.Instance.WindowConfigMessage(word, null, null);
        }
        private int CalcManaForEquipEnhance(int prolevel,int oldlevel,int newlevel)
        {
            if (oldlevel < 0) { oldlevel = 0; }
            if (newlevel < 0) { newlevel = 0; }
            int[] point;
            switch (prolevel)
            {
                case 2:
                    return 120 * 20 * (newlevel - oldlevel);
                case 3:
                    point = new int[]{ 0,30, 80, 160};
                    return 150 * (point[newlevel] - point[oldlevel]);
                case 4:
                    point = new int[] { 0, 60, 160, 340,700,1200 };
                    return 200 * (point[newlevel] - point[oldlevel]);
                case 5:
                    point = new int[] { 0,100,260,540,1020,1800 };
                    return 250 * (point[newlevel] - point[oldlevel]);
            }
            return 0;
        }
        public void CalculateEXPButton()
        {
            long exp_total = 0;
            foreach(UnitData a in unitDataListForCal)
            {
                int perLevel = unitDataDicForCal_perverous[a.unitId].level;
                if (a.level > perLevel)
                {
                    if (a.level < expCost.Count)
                    {
                        exp_total += expCost[a.level] - expCost[perLevel];
                    }
                    else
                    {
                        MainManager.Instance.WindowConfigMessage("等级超过可计算的范围！\n可计算的最大等级：" + expCost.Count + "\n当前等级：" + a.level, null, null);
                        return;
                    }
                }
            }
            int bottle = Mathf.CeilToInt(exp_total / 7500);
            MainManager.Instance.WindowConfigMessage("所需经验：" + exp_total + "\n需要超级经验药剂：" + bottle, null, null);
        }
        public void CalculateAPButton()
        {
            if (equipmentRequireDic_2 == null)
            {
                MainManager.Instance.WindowMessage("请先计算装备碎片需求！");
                return;
            }
            GameObject a = Instantiate(calculatorSettingPrefab);
            a.transform.SetParent(BaseBackManager.Instance.latestUIback.transform, false);
            a.transform.localPosition = new Vector3();
            a.transform.localScale = new Vector3(1, 1, 1);
            a.GetComponent<CalculatorSettingPage>().SetButton(CalcAP_A);
        }
        public void CalcAP_A(int ignorenum,int rate)
        {
            equipmentGetedDic_Quest.Clear();
            equipmentQuestDic.Clear();
            foreach(int equipment in equipmentRequireDic_2.Keys)
            {
                EquipmentGet equipmentGet = equipmentGetDic[equipment];
                if(equipmentGet.first_appear_quest_id > 11000 + ignorenum)
                {
                    AppendEquipmentToDic(equipmentGet,equipmentRequireDic_2[equipment],rate);
                }
            }
            CalcAP_B();
        }
        private void CalcAP_B()
        {
            //var dicSort = from objDic in equipmentQuestDic orderby objDic.Value descending select objDic;
            List<QuestRewardData> rewardDatas = new List<QuestRewardData>();
            foreach(QuestRewardData data in equipmentQuestDic.Keys)
            {
                rewardDatas.Add(data);
            }
            rewardDatas.Sort((a, b) => b.quest_id - a.quest_id);
            equipmentCalPage_C.SetActive(true);
            ClearUnitIcons(parent_C, questPrefabs);
            int count = 1;
            int requireNum = 0;
            foreach (QuestRewardData questRewardData in rewardDatas)
            {

                GameObject b = Instantiate(questPrefab);
                b.transform.SetParent(parent_C,false);
                b.transform.localScale = new Vector3(1, 1, 1);
                b.transform.localPosition = new Vector3(baseRange_quest.x, -1 * (baseRange_quest.y + range_quest.y * count), 0);
                int num = equipmentQuestDic[questRewardData];
                b.GetComponent<CalculatePageButton>().SetButton(questRewardData, num, num*10);
                questPrefabs.Add(b);
                count++;
                requireNum += num;
                //showUnitIDs.Add(id);
            }
            if (parent_C.sizeDelta.y <= range_quest.y*count + 2*baseRange_quest.y)
            {
                parent_C.sizeDelta = new Vector2(100, range_quest.y * count + 2 * baseRange_quest.y);
            }
            equipmentCalPage_calAP.text = "" + requireNum*10;

        }
        private void AppendEquipmentToDic(EquipmentGet equipmentGet,int num,int rate)
        {
            int need = num;
            if(equipmentGetedDic_Quest.TryGetValue(equipmentGet.equipment_id,out int value))
            {
                if (value >= num)
                {
                    return;
                }
                need -= value;
            }
            equipmentGet.GetBestWays(true, out List<int> ways, out List<int> odds);
            if (ways.Count >= 1)
            {
                int count = Mathf.RoundToInt(100.0f * need / (float)(odds[0] * rate));
                QuestRewardData questRewardData = questRewardDic[ways[0]];
                if (equipmentQuestDic.ContainsKey(questRewardData))
                {
                    equipmentQuestDic[questRewardData] += count;
                }
                else
                {
                    equipmentQuestDic.Add(questRewardData, count);
                }
                AppendQuestDatas(questRewardData, count, rate);
            }
        }
        private void AppendQuestDatas(QuestRewardData questRewardData,int count,int rate)
        {
            for(int i = 0; i < questRewardData.rewardEquips.Count; i++)
            {
                int equipment = questRewardData.rewardEquips[i];
                int odd = questRewardData.odds[i];
                if(equipment != 0 && odd != 0)
                {
                    int num_0 = Mathf.RoundToInt(odd * count * rate / 100.0f);
                    if (equipmentGetedDic_Quest.ContainsKey(equipment))
                    {
                        equipmentGetedDic_Quest[equipment] += num_0;
                    }
                    else
                    {
                        equipmentGetedDic_Quest.Add(equipment, num_0);
                    }
                }
            }
        }
        private void LoadEquipmentCraftDic()
        {
            string jsonStr = db.text;
            if (jsonStr != "")
            {
                CalcDics dic = JsonConvert.DeserializeObject<CalcDics>(jsonStr);
                equipmentCraftDic = dic.equipmentCraftDic;
                equipmentGetDic = dic.equipmentGetDic;
                questRewardDic = dic.questRewardDic;
                expCost = dic.exp_cost;
                skillCost = dic.skill_cost;
            }
            else
            {
                MainManager.Instance.WindowConfigMessage("加载装备掉落数据失败！", null, null);
            }
        }

    }
}