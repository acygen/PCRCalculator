using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

namespace PCRCaculator
{


    public class CharacterDetailManager : MonoBehaviour
    {
        public TextMeshProUGUI rankText;
        public Text nameText;
        public List<Image> equipmentImages;
        public List<Stars> equipmentStars;
        //public List<Toggle> toggles;
        public GameObject parent;
        public List<TextMeshProUGUI> detailSetTexts;
        public List<Slider> detailSetSliders;
        public List<TextMeshProUGUI> equipmentLevelTexts;
        public List<Slider> equipmentLevelSliders;
        public List<TextMeshProUGUI> unitDetailTexts;
        public List<Image> skillImages;
        public List<TextMeshProUGUI> skillDetailTexts;
        public List<Text> skillDetailTexts_2;
        public Vector3 spinePosition;
        public Vector3 spineScale;
        public GameObject skillDetailPerferb;
        public Text detailTitleText;

        private int unitId;
        private UnitData data;
        private UnitData data_save;
        UnitRarityData un;
        private int[] skilllist;
        private List<int> equipmentID = new List<int>();//当前RANK装备的六件套id缓存
        private GameObject spine;
        public void SetId(int id)
        {
            gameObject.SetActive(true);
            unitId = id;
            Reflash();
        }
        public void Next(bool next)
        {
            int nextid = CharacterManager.Instance.showUnitIDs.FindIndex(a => a.Equals(unitId));
            if (!next && nextid > 0)
            {
                SetId(CharacterManager.Instance.showUnitIDs[nextid - 1]);
            }
            else if (next && nextid < CharacterManager.Instance.showUnitIDs.Count - 1)
            {
                SetId(CharacterManager.Instance.showUnitIDs[nextid + 1]);
            }
        }
        private void Reflash()
        {
            if (!MainManager.Instance.unitDataDic.ContainsKey(unitId))
            {
                Debug.LogError("没有角色" + unitId + "的信息！");
                return;
            }
            data = MainManager.Instance.unitDataDic[unitId];
            if (spine != null) { Destroy(spine); spine = null; }
            LoadSpine(unitId);
            SetDetails();
            SetSilders(data);
            SetSkills();
        }
        private void LoadSpine(int unitid)
        {
            SkeletonDataAsset dataAsset = ScriptableObject.CreateInstance<SkeletonDataAsset>();
            dataAsset = Resources.Load<SkeletonDataAsset>("Unit/" + unitid + "/" + unitid + "_SkeletonData");
            int motiontype = MainManager.Instance.UnitRarityDic[unitid].detailData.motionType;
            string motiontype_str = motiontype < 10 ? "0" + motiontype : "" + motiontype;
            var spineAnimation = dataAsset.GetSkeletonData(false).FindAnimation(motiontype_str + "_idle");
            var sa = SkeletonGraphic.NewSkeletonGraphicGameObject(dataAsset, parent.transform);
            if (spineAnimation != null)
            {
                sa.Initialize(false);
                sa.AnimationState.SetAnimation(0, spineAnimation, true);
            }
            spine = sa.gameObject;
            spine.transform.localPosition = spinePosition;
            spine.transform.localScale = spineScale;

        }

        private void SetDetails()
        {
            un = MainManager.Instance.UnitRarityDic[unitId];
            equipmentID.Clear();
            for (int i = 0; i < 6; i++)
            {
                int equipmentid = un.GetRankEquipments(data.rank)[i];
                equipmentID.Add(equipmentid);
                string equip_path = "equipments/icon_equipment_" + equipmentid;
                if (data.equipLevel[i] == -1)
                {
                    equip_path = "equipments/icon_equipment_invalid_" + equipmentid;
                }
                Sprite im = MainManager.LoadSourceSprite(equip_path);
                if (im != null) { equipmentImages[i].sprite = im; }
                else { equipmentImages[i].sprite = MainManager.LoadSourceSprite("equipments/icon_equipment_999999"); }
                int maxStar = 0;
                if (MainManager.Instance.EquipmentDic.ContainsKey(equipmentid))
                {
                    maxStar = MainManager.Instance.EquipmentDic[equipmentid].GetMaxLevel();
                }
                equipmentStars[i].SetStars(data.equipLevel[i], maxStar);
            }
            if (MainManager.Instance.UnitName_cn.ContainsKey(unitId))
            {
                nameText.text = MainManager.Instance.UnitName_cn[unitId];
            }
            else
            {
                nameText.text = un.unitName;
            }
            rankText.text = "RANK " + data.rank;
            BaseData b = un.GetUnitData(data);
            data_save = MainManager.Instance.unitDataDic_save[unitId];
            BaseData c = un.GetUnitData(data_save, true);
            SetDetailTexts(b, c);
        }
        private void SetDetailTexts(BaseData b, BaseData c)
        {
            string[] compare = b.Compare(c);
            unitDetailTexts[0].text = b.Atk.ToString() + compare[1];
            unitDetailTexts[1].text = b.Magic_str.ToString() + compare[2];
            unitDetailTexts[2].text = b.Def.ToString() + compare[3];
            unitDetailTexts[3].text = b.Magic_def.ToString() + compare[4];
            unitDetailTexts[4].text = b.Hp.ToString() + compare[0];
            unitDetailTexts[5].text = b.Physical_critical.ToString() + compare[5];
            unitDetailTexts[6].text = b.Dodge.ToString() + compare[9];
            unitDetailTexts[7].text = b.Magic_critical.ToString() + compare[6];
            unitDetailTexts[8].text = b.Wave_hp_recovery.ToString() + compare[7];
            unitDetailTexts[9].text = b.Wave_energy_recovery.ToString() + compare[8];
            unitDetailTexts[10].text = b.Life_steal.ToString() + compare[12];
            unitDetailTexts[11].text = b.Hp_recovery_rate.ToString() + compare[13];
            unitDetailTexts[12].text = b.Energy_recovery_rate.ToString() + compare[14];
            unitDetailTexts[13].text = b.Enerey_reduce_rate.ToString() + compare[15];
            unitDetailTexts[14].text = b.Accuracy.ToString() + compare[16];
            unitDetailTexts[15].text = data.love.ToString() + BaseData.Compare_2(data.love,data_save.love);
            int x = Mathf.RoundToInt(b.GetPowerValue(data.skillLevel, data.rarity >= 5));
            int y = Mathf.RoundToInt(c.GetPowerValue(data_save.skillLevel, data_save.rarity >= 5));
            unitDetailTexts[16].text = x + BaseData.Compare_2(x, y);
        }
        public void SetSilders(UnitData unit)
        {
            detailSetSliders[0].maxValue = MainManager.Instance.levelMax;
            detailSetSliders[0].minValue = 1;
            detailSetSliders[0].value = unit.level;
            detailSetTexts[0].text = unit.level.ToString();
            detailSetSliders[1].value = unit.rarity;
            detailSetSliders[1].minValue = MainManager.Instance.UnitRarityDic[unitId].detailData.minrarity;
            detailSetTexts[1].text = unit.rarity.ToString();
            detailSetSliders[2].value = unit.love;
            detailSetTexts[2].text = unit.love.ToString();

            for (int i = 3; i < 7; i++)
            {
                detailSetSliders[i].maxValue = unit.level;
                detailSetSliders[i].minValue = 1;
                detailSetSliders[i].value = unit.skillLevel[i - 3];
                detailSetTexts[i].text = unit.skillLevel[i - 3].ToString();
            }

            equipmentLevelSliders[0].value = unit.rank;
            equipmentLevelSliders[0].maxValue = MainManager.Instance.rankMax;
            equipmentLevelTexts[0].text = unit.rank.ToString();
            for (int i = 0; i < 6; i++)
            {
                equipmentLevelSliders[i + 1].minValue = -1;
                int maxStar = 0;
                if (MainManager.Instance.EquipmentDic.ContainsKey(equipmentID[i]))
                {
                    maxStar = MainManager.Instance.EquipmentDic[equipmentID[i]].GetMaxLevel();
                }
                equipmentLevelSliders[i + 1].maxValue = maxStar;
                equipmentLevelSliders[i + 1].value = unit.equipLevel[i];
                if (unit.equipLevel[i] == -1)
                {
                    equipmentLevelTexts[i + 1].text = "null";
                }
                else
                {
                    equipmentLevelTexts[i + 1].text = unit.equipLevel[i].ToString();
                }
            }
        }
        private void SetSkills()
        {
            skilllist = un.GetSkillList(data);
            for (int i = 0; i < 4; i++)
            {
                string path = "skills/icon_skill_" + MainManager.Instance.SkillDataDic[skilllist[i]].icon;
                Sprite im = MainManager.LoadSourceSprite(path);
                if (im != null)
                {
                    skillImages[i].sprite = im;
                }
            }
            skillDetailTexts[0].text = "" + un.detailData.searchAreaWidth;
            skillDetailTexts[1].text = "" + un.detailData.normalAtkCastTime;
            string begin = "";
            string loop = "";
            for (int i = 0; i < un.skillData.loopStart - 1; i++)
            {
                begin += SkillInt2Str(un.skillData.atkPatterns[i]) + "-";
            }
            for (int i = un.skillData.loopStart - 1; i < un.skillData.loopEnd; i++)
            {
                loop += SkillInt2Str(un.skillData.atkPatterns[i]) + "-";
            }
            skillDetailTexts_2[0].text = begin.Substring(0, begin.Length - 1);
            skillDetailTexts_2[1].text = loop.Substring(0, loop.Length - 1);
        }
        private static string SkillInt2Str(int skillid)
        {
            if (skillid == 1) { return "普攻"; }
            if (skillid == 1001) { return "<color=#0422FF>技能1</color>"; }
            if (skillid == 1002) { return "<color=#6604FF>技能2</color>"; }
            return "ERROR!";
        }
        /// <summary>
        /// 滑动条用
        /// </summary>
        public void OnSliderDraged()
        {
            ResetUnitData();
            data = MainManager.Instance.unitDataDic[unitId];
            SetDetails();
            SetSilders(data);
            SetSkills();
        }
        /// <summary>
        /// 属性面板按钮+
        /// </summary>
        /// <param name="k"></param>
        public void DetailButton_Add(int k)
        {
            if (detailSetSliders[k].value < detailSetSliders[k].maxValue)
            {
                detailSetSliders[k].value++;
            }
            OnSliderDraged();
        }
        /// <summary>
        /// 属性面板按钮-
        /// </summary>
        /// <param name="k"></param>
        public void DetailButton_Minus(int k)
        {
            if (detailSetSliders[k].value > detailSetSliders[k].minValue)
            {
                detailSetSliders[k].value--;
            }
            OnSliderDraged();
        }
        /// <summary>
        /// 属性面板按钮+
        /// </summary>
        /// <param name="k"></param>
        public void EquipButton_Add(int k)
        {
            if (equipmentLevelSliders[k].value < equipmentLevelSliders[k].maxValue)
            {
                equipmentLevelSliders[k].value++;
            }
            OnSliderDraged();
        }
        /// <summary>
        /// 属性面板按钮-
        /// </summary>
        /// <param name="k"></param>
        public void EquipButton_Minus(int k)
        {
            if (equipmentLevelSliders[k].value > equipmentLevelSliders[k].minValue)
            {
                equipmentLevelSliders[k].value--;
            }
            OnSliderDraged();
        }
        /// <summary>
        /// 技能图标按钮
        /// </summary>
        /// <param name="buttonId">按钮序号</param>
        public void SkillButton(int buttonId)
        {
            GameObject a = Instantiate(skillDetailPerferb);
            a.transform.SetParent(BaseBackManager.Instance.latestUIback.transform);
            a.GetComponent<RectTransform>().offsetMax = new Vector2();
            a.GetComponent<RectTransform>().offsetMin = new Vector2();
            a.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
            a.GetComponent<SkillDetailButton>().SetDetails(skillImages[buttonId].sprite,
                skilllist[buttonId],
                data,
                buttonId);
        }
        public void LoadButton()
        {
            MainManager.Instance.ReLoad();
            Reflash();
            MainManager.Instance.WindowMessage("读取成功！");
        }
        public void SaveButton()
        {
            MainManager.Instance.SaveUnitData();
            SetDetails();
            MainManager.Instance.WindowMessage("保存成功！");
        }
        public void EquipmentButton()
        {
            MainManager.Instance.WindowMessage("这个按钮暂时没用！");
        }
        private void ResetUnitData()
        {
            int[] eqlv = new int[6] {(int)equipmentLevelSliders[1].value, (int)equipmentLevelSliders[2].value ,
            (int)equipmentLevelSliders[3].value , (int)equipmentLevelSliders[4].value,
        (int)equipmentLevelSliders[5].value,(int)equipmentLevelSliders[6].value};
            int[] sklv = new int[4] {(int)detailSetSliders[3].value, (int)detailSetSliders[4].value,
        (int)detailSetSliders[5].value,(int)detailSetSliders[6].value};
            UnitData newdata = new UnitData(unitId, (int)detailSetSliders[0].value, (int)detailSetSliders[1].value,
                (int)detailSetSliders[2].value, (int)equipmentLevelSliders[0].value, eqlv, sklv);
            MainManager.Instance.unitDataDic[unitId] = newdata;
        }
    }
}