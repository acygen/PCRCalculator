using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

namespace PCRCaculator
{


    public class JJCManager : MonoBehaviour
    {
        public static JJCManager Instance;
        public Text playerLevelText;
        //public GameObject battleReadyPage;

        public GameObject buttonPerferb_JJC;
        public RectTransform parent;
        public int original_hight;
        public Vector3 startPosition;
        public Vector3 addPosition;
        public Vector3 perferbScale;

        private List<AddedPlayerData> players = new List<AddedPlayerData>();
        private List<GameObject> selectButtons = new List<GameObject>();
        private int waitDeletePlayerPos = 0;//等待删除的玩家
        private int attackplayerPos = 0;//战斗对手的序号
        private void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 开始战斗
        /// </summary>
        public void ReadyAttack(List<int> myCharid)
        {
            Debug.Log("开始战斗！");
            MainManager.Instance.ChangeSceneToBalttle(myCharid, players[attackplayerPos - 2]);
        }
        public void Reflash(bool reload = true)
        {
            playerLevelText.text = MainManager.Instance.PlayerSetting.playerLevel + "";
            if (reload) { LoadAddedPlayerData(); }
            foreach (GameObject a in selectButtons)
            {
                Destroy(a);
            }
            selectButtons.Clear();
            parent.localPosition = new Vector3();
            parent.sizeDelta = new Vector2(100, original_hight);
            int i = 0;
            foreach (AddedPlayerData player in players)
            {
                GameObject b = Instantiate(buttonPerferb_JJC);
                b.transform.SetParent(parent);
                b.transform.localScale = perferbScale;
                b.GetComponent<BattlePageButton>().SetButton(player, i + 2);
                b.transform.localPosition = startPosition + i * addPosition;
                selectButtons.Add(b);
                i++;
            }
            if (parent.sizeDelta.y < i * Mathf.Abs(addPosition.y) + 10)
            {
                parent.sizeDelta = new Vector2(100, i * Mathf.Abs(addPosition.y) + 10);
            }
        }
        /// <summary>
        /// 添加对手按钮
        /// </summary>
        public void AddPlayerButton()
        {
            ChoosePannelManager.Instance.CallChooseBack(2);
        }
        /// <summary>
        /// 对战记录按钮
        /// </summary>
        public void ReviewButton()
        {
            MainManager.Instance.WindowMessage("咕咕咕！");
        }
        /// <summary>
        /// 开始战斗按钮
        /// </summary>
        /// <param name="id">玩家序号(名次，从2开始)</param>
        public void AttackButton(int id)
        {
            attackplayerPos = id;
            ChoosePannelManager.Instance.CallChooseBack(1, players[id - 2]);
            //Debug.LogError("收到战斗请求：id=" + id);
        }
        private void LoadAddedPlayerData()
        {
            string filePath = Application.persistentDataPath + "/Battle/AddedPlayerList.json";
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                string jsonStr = sr.ReadToEnd();
                sr.Close();
                if (jsonStr != "")
                {
                    players = JsonConvert.DeserializeObject<List<AddedPlayerData>>(jsonStr);
                }
            }

        }
        /// <summary>
        /// 编辑完敌方阵容后调用
        /// </summary>
        /// <param name="addedPlayerData"></param>
        public void FinishAddingNewPlayer(AddedPlayerData addedPlayerData)
        {
            players.Add(addedPlayerData);
            Reflash(false);
            SaveDataToJson();
        }
        public void DeleteButton(int pos)
        {
            waitDeletePlayerPos = pos;
            MainManager.Instance.WindowConfigMessage("是否删除该玩家？(此操作无法恢复)", DeleteAddedPlayer);
        }
        public void DeleteAddedPlayer()
        {
            if (waitDeletePlayerPos >= 2 && waitDeletePlayerPos < players.Count + 2)
            {
                players.RemoveAt(waitDeletePlayerPos - 2);
                Reflash(false);
                SaveDataToJson();
                MainManager.Instance.WindowMessage("删除成功！");
            }
            else
            {
                MainManager.Instance.WindowMessage("删除失败！");
            }
        }
        private void SaveDataToJson()
        {
            string filePath = Application.persistentDataPath + "/Battle/AddedPlayerList.json";
            if (!Directory.Exists(Application.persistentDataPath + "/Battle"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Battle");
            }
            string saveJsonStr = JsonConvert.SerializeObject(players);
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(saveJsonStr);
            sw.Close();
        }
    }

    [System.Serializable]
    public class AddedPlayerData
    {
        public string playerName = "佑树";
        public int playerLevel = 100;
        public int totalpoint = 0;//队伍总战力
        public List<UnitData> playrCharacters = new List<UnitData>();//至少为1，最多为5
    }
}