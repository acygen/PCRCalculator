using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using Mono.Data.Sqlite;
using TMPro;

namespace PCRCaculator
{


    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance;
        public GameObject MassagePerferb;//弹出消息条底板
        public GameObject SystemWindowMessagePerferb;//二次确认底板
        public GameObject LoadingPagePerferb;//加载面板
        public TextAsset db;
        public enum StayPage { home = 0, character = 1, battle = 2, gamble = 3 }
        public StayPage stayPage;
        public int loadCharacterMax;//最多加载到的角色序号
        public int levelMax { get => playerSetting.playerLevel; }
        public int rankMax { get => playerSetting.maxRank; }

        private Dictionary<string, Sprite> spriteCacheDic = new Dictionary<string, Sprite>();//图片缓存
        private Dictionary<int, EquipmentData> equipmentDic = new Dictionary<int, EquipmentData>();//装备类与装备id的对应字典
        public Dictionary<int, UnitData> unitDataDic = new Dictionary<int, UnitData>();//角色类可更改数据与角色id的对应字典(临时)
        public Dictionary<int, UnitData> unitDataDic_save = new Dictionary<int, UnitData>();//角色类可更改数据与角色id的对应字典(已保存)
        private Dictionary<int, UnitRarityData> unitRarityDic = new Dictionary<int, UnitRarityData>();//角色基础数据与角色id的对应字典 
        private Dictionary<int, UnitStoryData> unitStoryDic = new Dictionary<int, UnitStoryData>();//角色羁绊奖励列表
        private Dictionary<int, List<int>> unitStoryEffectDic = new Dictionary<int, List<int>>();//角色的马甲列表
        private Dictionary<int, SkillData> skillDataDic = new Dictionary<int, SkillData>();//所有的技能列表
        private Dictionary<int, SkillAction> skillActionDic = new Dictionary<int, SkillAction>();//所有小技能列表
        private Dictionary<int, string> unitName_cn = new Dictionary<int, string>();//角色中文名字
        private Dictionary<int, string[]> skillNameAndDescribe_cn = new Dictionary<int, string[]>();//技能中文名字和描述
        private Dictionary<int, string> skillActionDescribe_cn = new Dictionary<int, string>();//技能片段中文描述


        private CharacterManager characterManager;
        private AdventureManager battleManager;
        //private SQLiteHelper sql;

        private Coroutine windowmassageIE;
        private PlayerSetting playerSetting;

        private List<UnitData> playerDataForBattle;
        private List<UnitData> enemyDataForBattle;

        public Dictionary<int, EquipmentData> EquipmentDic { get => equipmentDic; }
        public Dictionary<int, UnitRarityData> UnitRarityDic { get => unitRarityDic; }
        public Dictionary<int, UnitStoryData> UnitStoryDic { get => unitStoryDic; }
        public Dictionary<int, List<int>> UnitStoryEffectDic { get => unitStoryEffectDic; }
        public Dictionary<int, SkillData> SkillDataDic { get => skillDataDic; }
        public Dictionary<int, SkillAction> SkillActionDic { get => skillActionDic; }
        public Dictionary<int, string> UnitName_cn { get => unitName_cn; }
        public Dictionary<int, string[]> SkillNameAndDescribe_cn { get => skillNameAndDescribe_cn; }
        public Dictionary<int, string> SkillActionDescribe_cn { get => skillActionDescribe_cn; }
        public PlayerSetting PlayerSetting { get => playerSetting; }
        public GameObject LatestUIback
        {
            get
            {
                if (BaseBackManager.Instance != null) { return BaseBackManager.Instance.latestUIback; }
                else { return null; }
            }
        }

        public Text Debugtext { get => BaseBackManager.Instance.debugtext; }
        public TextMeshProUGUI PlayerLevelText { get => BaseBackManager.Instance.playerLevelText; }
        public CharacterManager CharacterManager { get => CharacterManager.Instance; set => characterManager = value; }
        public AdventureManager BattleManager { get => AdventureManager.Instance; set => battleManager = value; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Application.targetFrameRate = 60;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            try
            {
                Load();

            }
            catch (System.Exception e)
            {
                Debugtext.text += e.Message;
            }

            //CharacterManager = CharacterManager.Instance;
            //BattleManager = AdventureManager.Instance;
        }
        private void Load()
        {

            string jsonStr = db.text;
            if (jsonStr != "")
            {
                AllData allData = JsonConvert.DeserializeObject<AllData>(jsonStr);
                foreach (string a in allData.equipmentDic)
                {
                    EquipmentData equipmentData = new EquipmentData(a);
                    equipmentDic.Add(equipmentData.equipment_id, equipmentData);
                }
                Debugtext.text += "\n成功加载" + EquipmentDic.Count + "个装备数据！";

                foreach (string a in allData.unitRarityDic)
                {
                    UnitRarityData unitRarityData = new UnitRarityData(a);
                    unitRarityDic.Add(unitRarityData.unitId, unitRarityData);
                }
                Debugtext.text += "\n成功加载" + UnitRarityDic.Count + "个角色数据！";

                foreach (string a in allData.unitStoryDic)
                {
                    UnitStoryData unitStoryData = new UnitStoryData(a);
                    unitStoryDic.Add(unitStoryData.unitid, unitStoryData);
                }
                unitStoryEffectDic = allData.unitStoryEffectDic;
                foreach (string a in allData.skillDataDic)
                {
                    SkillData skillData = new SkillData(a);
                    skillDataDic.Add(skillData.skillid, skillData);
                }
                Debugtext.text += "\n成功加载" + SkillDataDic.Count + "个技能数据！";

                foreach (string a in allData.skillActionDic)
                {
                    SkillAction skillAction = new SkillAction(a);
                    skillActionDic.Add(skillAction.actionid, skillAction);
                }
                unitName_cn = allData.unitName_cn;
                Debugtext.text += "\n成功加载" + unitName_cn.Count + "个角色数据(中文)！";

                skillNameAndDescribe_cn = allData.skillNameAndDescribe_cn;
                Debugtext.text += "\n成功加载" + skillNameAndDescribe_cn.Count + "个技能数据(中文)！";

                skillActionDescribe_cn = allData.skillActionDescribe_cn;
            }
            LoadUnitData();
            LoadPlayerSettings();
            Debugtext.text += "\n数据加载完毕！";

            return;
        }
        private void LoadUnitData()
        {
            string filePath = Application.persistentDataPath + "/SaveData.json";
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                string jsonStr = sr.ReadToEnd();
                sr.Close();
                if (jsonStr != "")
                {
                    unitDataDic_save = JsonConvert.DeserializeObject<Dictionary<int, UnitData>>(jsonStr);
                    unitDataDic = JsonConvert.DeserializeObject<Dictionary<int, UnitData>>(jsonStr);
                    return;
                }

            }
            foreach (int id in UnitRarityDic.Keys)
            {
                unitDataDic.Add(id, new UnitData(id, UnitRarityDic[id].detailData.minrarity));
                unitDataDic_save.Add(id, new UnitData(id, UnitRarityDic[id].detailData.minrarity));
            }

        }
        private void LoadPlayerSettings()
        {
            string filePath = Application.persistentDataPath + "/PlayerData.json";
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                string jsonStr = sr.ReadToEnd();
                sr.Close();
                if (jsonStr != "")
                {
                    playerSetting = JsonConvert.DeserializeObject<PlayerSetting>(jsonStr);
                    PlayerLevelText.text = playerSetting.playerLevel + "";
                    return;
                }

            }
            playerSetting = new PlayerSetting();
            playerSetting.maxRank = 9;
            playerSetting.playerLevel = 95;
            PlayerLevelText.text = playerSetting.playerLevel + "";

        }

        /// <summary>
        /// 保存玩家设置到json
        /// </summary>
        public void SaveUnitData()
        {
            string filePath = Application.persistentDataPath + "/SaveData.json";
            string saveJsonStr = JsonConvert.SerializeObject(unitDataDic);
            unitDataDic_save = JsonConvert.DeserializeObject<Dictionary<int, UnitData>>(saveJsonStr);
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(saveJsonStr);
            sw.Close();
        }
        /// <summary>
        /// 从json读取玩家设置
        /// </summary>
        public void ReLoad()
        {
            string filePath = Application.persistentDataPath + "/SaveData.json";
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                string jsonStr = sr.ReadToEnd();
                sr.Close();
                if (jsonStr != "")
                {
                    unitDataDic_save = JsonConvert.DeserializeObject<Dictionary<int, UnitData>>(jsonStr);
                    unitDataDic = JsonConvert.DeserializeObject<Dictionary<int, UnitData>>(jsonStr);
                }

            }
        }
        public void SavePlayerSetting()
        {
            string filePath = Application.persistentDataPath + "/PlayerData.json";
            string saveJsonStr = JsonConvert.SerializeObject(playerSetting);
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(saveJsonStr);
            sw.Close();
        }
        public void DeletePlayerData()
        {
            if (File.Exists(Application.persistentDataPath + "/SaveData.json"))
            {
                File.Delete(Application.persistentDataPath + "/SaveData.json");
                unitDataDic = new Dictionary<int, UnitData>();
                unitDataDic_save = new Dictionary<int, UnitData>();
                WindowMessage("删除成功！");
                LoadUnitData();
                return;
            }
            WindowMessage("删除失败！");
        }
        public void WindowMessage(string word)
        {
            if (windowmassageIE == null)
            {
                windowmassageIE = StartCoroutine(WindowMessage_start(word));
            }
        }
        public void WindowConfigMessage(string word, SystemWindowMessage.configDelegate configDelegate)
        {
            GameObject a = Instantiate(SystemWindowMessagePerferb);
            a.transform.SetParent(LatestUIback.transform);
            a.transform.localPosition = new Vector3();
            a.transform.localScale = new Vector3(1, 1, 1);
            a.GetComponent<SystemWindowMessage>().SetWindowMassage(word, configDelegate);
        }
        private IEnumerator WindowMessage_start(string word)
        {
            GameObject a = Instantiate(MassagePerferb);
            a.transform.SetParent(LatestUIback.transform);
            a.transform.localPosition = new Vector3();
            a.transform.localScale = new Vector3(1, 1, 1);
            a.GetComponentInChildren<Text>().text = word;
            yield return new WaitForSecondsRealtime(1.5f);
            Destroy(a);
            windowmassageIE = null;
        }
        public void HomeButton()
        {
            CharacterManager.SwitchPage(0);
            BattleManager.SwitchPage(0);
            stayPage = StayPage.home;
        }

        public void CharacterButton()
        {
            CharacterManager.SwitchPage(1);
            BattleManager.SwitchPage(0);
            stayPage = StayPage.character;
        }

        public void BattleButton()
        {
            CharacterManager.SwitchPage(0);
            BattleManager.SwitchPage(1);
            stayPage = StayPage.battle;
        }
        public void GambleButton()
        {
            WindowMessage("咕咕咕！");
        }
        public void GetBattleData(out List<UnitData> playerdata, out List<UnitData> enemydata)
        {
            playerdata = playerDataForBattle;
            enemydata = enemyDataForBattle;
        }
        public void ChangeSceneToBalttle(List<int> my, AddedPlayerData other)
        {
            List<UnitData> d1 = new List<UnitData>();
            foreach (int id in my)
            {
                d1.Add(unitDataDic[id]);
            }
            playerDataForBattle = d1;
            enemyDataForBattle = other.playrCharacters;
            StartCoroutine(LoadScene());
        }
        IEnumerator LoadScene()
        {
            GameObject a = Instantiate(LoadingPagePerferb);
            a.transform.SetParent(LatestUIback.transform);
            a.transform.localScale = new Vector3(1, 1, 1);
            a.GetComponent<RectTransform>().offsetMax = new Vector2(5, 5);
            a.GetComponent<RectTransform>().offsetMin = new Vector2(-5, -5);

            yield return new WaitForSeconds(1.5f);
            var async = SceneManager.LoadSceneAsync("BattleScene");
            async.allowSceneActivation = true;
            while (!async.isDone)
            {
                yield return null;
            }
        }

        public static Sprite LoadSourceSprite(string relativePath)
        {
            if (Instance.spriteCacheDic.ContainsKey(relativePath))
            {
                return Instance.spriteCacheDic[relativePath];
            }

            Object Preb = Resources.Load(relativePath, typeof(Sprite));
            Sprite tmpsprite = null;
            if (Preb != null)
            {
                try
                {
                    tmpsprite = Instantiate(Preb) as Sprite;
                    Instance.spriteCacheDic.Add(relativePath, tmpsprite);
                }
                catch (System.Exception ex)
                {
                    Debug.Log(relativePath + "图片加载失败，原因：" + ex.Message);
                }

            }

            //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
            return tmpsprite;
        }

    }

}