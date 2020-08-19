using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace PCRCaculator
{

    public class ChoosePannelManager : MonoBehaviour
    {
        public static ChoosePannelManager Instance;
        public GameObject baseBack;
        public GameObject chooseBack_A;
        public GameObject settingBack;

        public RectTransform parent;
        public GameObject togglePerferb;
        public int original_hight;
        public Vector2 baseRange;//第一个按钮的位置
        public Vector2 range;//相邻按钮之间的距离
        public List<Toggle> switchToggles;

        public GameObject enemyGroups;
        public Text enemyTotalPointText;
        public List<CharacterPageButton> enemyChars;

        public List<CharacterPageButton> selectedChars;
        public Sprite defaultSprite;
        public Text nextButtonText;

        public Text totalPointText_setting;
        public List<CharacterPageButton> chars_setting;
        public List<Toggle> charToggles_setting;
        public List<TextMeshProUGUI> detailTexts_setting;
        public List<Slider> detailSliders_setting;

        //private List<Toggle> togglePerferbs = new List<Toggle>();
        private List<int> selectedCharId = new List<int>();
        private Dictionary<int, Toggle> togglePerferbs = new Dictionary<int, Toggle>();
        private bool togglesEnable;
        private AddedPlayerData playerData = new AddedPlayerData();
        private int selectedCharacterId_setting = 0;

        private bool isinstating = true;//是否正在初始化滑动条，是则忽略滑动条的回调事件
        private int type;
        private void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 调用选人面板的函数
        /// </summary>
        /// <param name="type">1-JJC开战选人，2-JJC编辑敌方队伍选人</param>
        /// <param name="playerData">敌人队伍，为空则不显示</param>
        public void CallChooseBack(int type, AddedPlayerData player = null)
        {
            this.type = type;
            baseBack.SetActive(true);
            chooseBack_A.SetActive(true);
            settingBack.SetActive(false);
            if (player == null)
            {
                enemyGroups.SetActive(false);
            }
            else
            {
                enemyGroups.SetActive(true);
                enemyTotalPointText.text = player.totalpoint + "";
                for (int i = 0; i < 5; i++)
                {
                    if (player.playrCharacters.Count > i)
                    {
                        enemyChars[i].SetButton(player.playrCharacters[i]);
                    }
                    else
                    {
                        enemyChars[i].SetButton(-1);
                    }
                }
            }
            //selectedCharId.Clear();
            if (PlayerPrefs.HasKey("selectedCharId"))
            {
                string id = PlayerPrefs.GetString("selectedCharId");
                try
                {
                    string[] ids = id.Split('-');
                    selectedCharId.Clear();
                    for (int i = 0; i < ids.Length; i++)
                    {
                        selectedCharId.Add(int.Parse(ids[i]));
                    }

                }
                catch
                {
                    Debug.LogError("读取预设阵容失败！");
                    selectedCharId.Clear();
                }
            }
            ReflashBasePage(0);
            switchToggles[0].isOn = true;
            ReflashSelectedButtons();
            if (type == 1)
            {
                nextButtonText.text = "战斗开始";
            }
            else if (type == 2)
            {
                nextButtonText.text = "下一步";
            }
        }
        public void OnToggleSwitched(bool k)
        {
            if (k)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (switchToggles[i].isOn)
                    {
                        ReflashBasePage(i);
                    }
                }
            }
        }
        public void CancalButton()
        {
            baseBack.SetActive(false);
            chooseBack_A.SetActive(false);
            settingBack.SetActive(false);
        }
        public void NextButton()
        {
            if (selectedCharId.Count == 0)
            {
                MainManager.Instance.WindowMessage("请选择至少一个角色！");
                return;
            }
            string saveID = "";
            for(int i =0;i<selectedCharId.Count;i++)
            {
                saveID += selectedCharId[i];
                if (i < selectedCharId.Count - 1)
                {
                    saveID += "-";
                }
            }
            PlayerPrefs.SetString("selectedCharId", saveID);
            if (type == 1)
            {
                //MainManager.Instance.WindowMessage("战斗系统还没做好！");
                CancalButton();
                JJCManager.Instance.ReadyAttack(selectedCharId);
            }
            else if (type == 2)
            {
                chooseBack_A.SetActive(false);
                settingBack.SetActive(true);
                playerData = new AddedPlayerData();
                isinstating = true;
                foreach (int unitid in selectedCharId)
                {
                    playerData.playrCharacters.Add(new UnitData(unitid));
                }
                for (int i = 0; i < 5; i++)
                {
                    charToggles_setting[i].interactable = i < selectedCharId.Count;
                    //charToggles_setting[i].isOn = i == 0;
                }
                ReflashSettingPage();
                ReflashSettingValues();
            }
        }
        public void FinishEditButton_setting()
        {
            CancalButton();
            JJCManager.Instance.FinishAddingNewPlayer(playerData);
        }
        public void OnToggleSwitched(bool k, int unitid)
        {
            if (selectedCharId.Contains(unitid))
            {
                selectedCharId.Remove(unitid);
                if (!togglesEnable)
                {
                    TurnAllToggles(true);
                }
            }
            else if (selectedCharId.Count < 5)
            {
                selectedCharId.Add(unitid);
                if (selectedCharId.Count == 5)
                {
                    TurnAllToggles(false);
                }
            }
            else
            {
                MainManager.Instance.WindowMessage("最多选5个！");
            }
            ReflashSelectedButtons();
        }
        public void OnToggleSwitched_setting(int id)
        {
            foreach (Toggle a in charToggles_setting)
            {
                if (a.interactable && a.isOn)
                {
                    selectedCharacterId_setting = id;
                    ReflashSettingValues();
                }
            }
        }
        public void OnButtonPressed(int buttonid)
        {
            if (selectedCharId.Count > buttonid)
            {
                togglePerferbs[selectedCharId[buttonid]].isOn = false;
            }
            else
            {
                MainManager.Instance.WindowMessage("请选择角色");
            }
        }
        public void OnSliderDraged()
        {
            if (isinstating) { return; }
            //UnitData data = playerData.playrCharacters[selectedCharacterId_setting];
            UnitData data = new UnitData(selectedCharId[selectedCharacterId_setting]);
            data.level = (int)detailSliders_setting[0].value;
            data.rarity = (int)detailSliders_setting[1].value;
            data.love = (int)detailSliders_setting[2].value;
            for (int i = 3; i < 7; i++)
            {
                data.skillLevel[i - 3] = (int)detailSliders_setting[i].value;
            }
            data.rank = (int)detailSliders_setting[7].value;
            for (int i = 8; i < 14; i++)
            {
                data.equipLevel[i - 8] = (int)detailSliders_setting[i].value;
            }
            playerData.playrCharacters[selectedCharacterId_setting] = data;
            ReflashSettingPage();
            ReflashSettingValues();
        }
        public void AddButton_setting(int buttonid)
        {
            if (detailSliders_setting[buttonid].value < detailSliders_setting[buttonid].maxValue)
            {
                detailSliders_setting[buttonid].value++;
            }
            OnSliderDraged();
        }
        public void MinusButton_setting(int buttonid)
        {
            if (detailSliders_setting[buttonid].value > detailSliders_setting[buttonid].minValue)
            {
                detailSliders_setting[buttonid].value--;
            }
            OnSliderDraged();
        }
        private void TurnAllToggles(bool k)
        {
            togglesEnable = k;
            foreach (Toggle toggle in togglePerferbs.Values)
            {
                if (!toggle.isOn)
                {
                    toggle.enabled = k;
                }
            }
        }
        private void ReflashBasePage(int type)
        {
            PositionType positionType = PositionType.frount;
            switch (type)
            {
                case 2:
                    positionType = PositionType.middle;
                    break;
                case 3:
                    positionType = PositionType.backword;
                    break;
            }
            foreach (Toggle a in togglePerferbs.Values)
            {
                Destroy(a.gameObject);
            }
            togglePerferbs.Clear();
            parent.localPosition = new Vector3();
            parent.sizeDelta = new Vector2(100, original_hight);
            int count = 1;
            foreach (int id in MainManager.Instance.UnitRarityDic.Keys)
            {
                if (type == 0 || MainManager.Instance.UnitRarityDic[id].unitPositionType == positionType)
                {
                    if (id <= MainManager.Instance.loadCharacterMax)// || (id>=400000&&id<=499999))
                    {
                        GameObject b = Instantiate(togglePerferb);
                        b.transform.SetParent(parent);
                        b.transform.localScale = new Vector3(1, 1, 1);
                        b.transform.localPosition = new Vector3(baseRange.x + range.x * ((count - 1) % 8), -1 * (baseRange.y + range.y * (Mathf.FloorToInt((count - 1) / 8))), 0);
                        int id0 = id;
                        if (selectedCharId.Contains(id))
                        {
                            b.GetComponent<Toggle>().isOn = true;
                        }
                        b.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => OnToggleSwitched(value, id0));
                        b.GetComponent<CharacterPageButton>().SetButton(id);
                        togglePerferbs.Add(id0, b.GetComponent<Toggle>());
                        count++;
                        //showUnitIDs.Add(id);

                    }
                }
            }
            if (parent.sizeDelta.y <= Mathf.CeilToInt(count / 8) * 95 + 5)
            {
                parent.sizeDelta = new Vector2(100, Mathf.CeilToInt(count / 8) * 95 + 5);
            }
        }
        private void ReflashSelectedButtons()
        {
            selectedCharId.Sort((x, y) => MainManager.Instance.UnitRarityDic[x].CompareTo(MainManager.Instance.UnitRarityDic[y]));
            for (int i = 0; i < 5; i++)
            {
                if (selectedCharId.Count > i)
                {
                    selectedChars[i].SetButton(selectedCharId[i]);
                }
                else
                {
                    selectedChars[i].SetButton(-1, defaultSprite);
                }
            }
        }
        private void ReflashSettingPage()
        {
            for (int i = 0; i < 5; i++)
            {
                if (playerData.playrCharacters.Count > i)
                {
                    chars_setting[i].SetButton(playerData.playrCharacters[i]);
                }
                else
                {
                    chars_setting[i].SetButton(-1);
                }
            }

        }
        private void ReflashSettingValues()
        {
            isinstating = true;
            UnitData data = playerData.playrCharacters[selectedCharacterId_setting];
            detailTexts_setting[0].text = data.level + "";
            detailSliders_setting[0].value = data.level;
            detailSliders_setting[0].maxValue = MainManager.Instance.levelMax;
            detailTexts_setting[1].text = data.rarity + "";
            detailSliders_setting[1].value = data.rarity;
            detailTexts_setting[2].text = data.love + "";
            detailSliders_setting[2].value = data.love;
            for (int i = 3; i < 7; i++)
            {
                detailTexts_setting[i].text = data.skillLevel[i - 3] + "";
                detailSliders_setting[i].value = data.skillLevel[i - 3];
                detailSliders_setting[i].maxValue = data.level;

            }
            detailTexts_setting[7].text = data.rank + "";
            detailSliders_setting[7].value = data.rank;
            detailSliders_setting[7].maxValue = MainManager.Instance.rankMax;
            for (int i = 8; i < 14; i++)
            {
                detailTexts_setting[i].text = data.equipLevel[i - 8] + "";
                detailSliders_setting[i].value = data.equipLevel[i - 8];

            }
            float to = 0;
            foreach (UnitData a in playerData.playrCharacters)
            {
                to += MainManager.Instance.UnitRarityDic[a.unitId].GetPowerValue(a);
            }
            totalPointText_setting.text = (int)to + "";
            playerData.totalpoint = (int)to;
            isinstating = false;
        }
    }
}