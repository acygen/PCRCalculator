using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PCRCaculator.Battle
{
    public enum eLogMessageType 
    { 
        ERROR=0,
        BATTLE_READY = 1,
        READY_ACTION = 2,
        EXEC_ACTION = 3,
        CANCEL_ACTION = 4,

        GET_DAMAGE = 10,
        MISS_ACTION = 11,
        DIE=12,
        HP_RECOVERY = 15,
        CHANGE_TP = 20,
    }
    public class BattleUIManager : MonoBehaviour
    {
        public static BattleUIManager Instance;
        public Text debugText1;
        public ScrollRect scrollRect1;
        public Text debugText2;
        public ScrollRect scrollRect2;
        public GameObject skillNamePerfab;
        public Transform parent;
        public GameObject buffUIPrefab;
        public List<Sprite> rularSprites;
        public Slider timeScaleSlider;
        public Text timeScaleText;
        public Text FPSText;
        public List<CharacterPageButton> PlayerUI;
        public List<CharacterPageButton> EnemyUI;

        private long fpsCount = 0;
        private float timeCount = 0;
        private BattleManager battleManager;

        private float m_LastUpdateShowTime = 0f;    //上一次更新帧率的时间;
        private float m_UpdateShowDeltaTime = 0.05f;//更新帧率的时间间隔;
        private int m_FrameUpdate = 0;//帧数;
        private int m_FPS = 0;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            battleManager = BattleManager.Instance;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;
        }
        private void Update()
        {
            if(battleManager.GameState == eGameBattleState.FIGHTING && !battleManager.IsPause)
            {
                fpsCount++;
                timeCount += battleManager.DeltaTimeForPause;
            }
            m_FrameUpdate++;
            if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
            {
                m_FPS = Mathf.RoundToInt(m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime));
                m_FrameUpdate = 0;
                m_LastUpdateShowTime = Time.realtimeSinceStartup;
            }
            FPSText.text = "" + m_FPS;
        }

        public void LogMessage(string word,eLogMessageType logMessageType,bool isOther)
        {
            string logtext = "(" + fpsCount + ")" + word + "\n";
            if (isOther)
            {
                debugText2.text += logtext;
                scrollRect2.verticalNormalizedPosition = 0;
            }
            else
            {
                debugText1.text += logtext;
                scrollRect1.verticalNormalizedPosition = 0;
            }
            Debug.Log(timeCount +"-" +  word);
            //Canvas.ForceUpdateCanvases();
        }
        public void ShowSkillName(string skillName,Transform transform)
        {
            GameObject a = Instantiate(skillNamePerfab);
            a.transform.SetParent(parent, false);
            a.GetComponent<SkillNameImage>().SetName(skillName, transform);

        }
        public void SetUI()
        {
            for(int i=0;i<PlayerUI.Count;i++)
            {
                if (BattleManager.Instance.PlayersList.Count > i)
                {
                    GameObject a = Instantiate(buffUIPrefab);
                    CharacterBuffUIController b = a.GetComponent<CharacterBuffUIController>();
                    BattleManager.Instance.PlayersList[i].SetUI(PlayerUI[i],b);
                    b.SetBuffUI(rularSprites[i], BattleManager.Instance.PlayersList[i]);
                }
                else
                {
                    PlayerUI[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < EnemyUI.Count; i++)
            {
                if (BattleManager.Instance.EnemiesList.Count > i)
                {
                    GameObject a = Instantiate(buffUIPrefab);
                    CharacterBuffUIController b = a.GetComponent<CharacterBuffUIController>();
                    BattleManager.Instance.EnemiesList[i].SetUI(EnemyUI[i],b);
                    b.SetBuffUI(rularSprites[i+5], BattleManager.Instance.EnemiesList[i]);
                }
                else
                {
                    EnemyUI[i].gameObject.SetActive(false);
                }
            }

        }
        public void OnTimeScaleSliderDraged()
        {
            int scaleRate = Mathf.RoundToInt(timeScaleSlider.value);
            float timeScale = Mathf.Pow(2, (scaleRate - 3));
            timeScaleText.text = "x" + timeScale;
            //Time.timeScale = timeScale;
            SetTimeScale(timeScale);
        }
        public void SetTimeScale(float scale)
        {
            foreach(UnitCtrl a in BattleManager.Instance.PlayersList)
            {
                a.SetTimeScale(scale);
            }
            foreach(UnitCtrl b in BattleManager.Instance.EnemiesList)
            {
                b.SetTimeScale(scale);
            }
        }
    }
}