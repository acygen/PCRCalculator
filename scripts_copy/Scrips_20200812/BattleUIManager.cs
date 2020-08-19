using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PCRCaculator.Battle
{
    public enum eLogMessageType
    {
        ERROR = 0,
        BATTLE_READY = 1,
        READY_ACTION = 2,
        EXEC_ACTION = 3,
        CANCEL_ACTION = 4,

        GET_DAMAGE = 10,
        MISS_ACTION = 11,
        DIE = 12,
        HP_RECOVERY = 15,
        CHANGE_TP = 20,
        BUFF_DEBUFF = 30,
        MISS_DAMAGE_BY_NO_DAMAGE = 40

    }
    public class BattleUIManager : MonoBehaviour
    {
        public static BattleUIManager Instance;
        public Text debugText1;
        public ScrollRect scrollRect1;
        public Text debugText2;
        public ScrollRect scrollRect2;
        public int maxTextLength;
        public GameObject skillNamePerfab;
        public Transform parent;
        public GameObject buffUIPrefab;
        public List<Sprite> rularSprites;
        public Slider timeScaleSlider;
        public Text timeScaleText;
        public Text FPSText;
        public List<CharacterPageButton> PlayerUI;
        public List<CharacterPageButton> EnemyUI;
        public List<Sprite> buffDebuffIcons;

        public GameObject numberPrefab;
        public GameObject missPrefab;
        public Vector3 numberPosFix;
        public List<Sprite> number_physical_large;
        public List<Sprite> number_magical_large;
        public List<Sprite> number_physical_small;
        public List<Sprite> number_heal_large;
        public List<Sprite> number_energy_large;
        public Sprite sprite_total_physical;
        public Sprite sprite_total_magical;
        public Sprite sprite_critical_physical;
        public Sprite sprite_critical_magical;

        private long fpsCount = 0;
        private float timeCount = 0;
        private BattleManager battleManager;

        private float m_LastUpdateShowTime = 0f;    //上一次更新帧率的时间;
        private float m_UpdateShowDeltaTime = 0.05f;//更新帧率的时间间隔;
        private int m_FrameUpdate = 0;//帧数;
        private int m_FPS = 0;

        static string[] SPLIT = new string[] { "\n" };

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
            if (battleManager.GameState == eGameBattleState.FIGHTING && !battleManager.IsPause)
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

        public void LogMessage(string word, eLogMessageType logMessageType, bool isOther)
        {
            string logtext = "(" + fpsCount + ")" + word + "\n";
            Text debugText = isOther ? debugText2 : debugText1;
            ScrollRect scrollRect = isOther ? scrollRect2 : scrollRect1;
            int overText = debugText.text.Length - maxTextLength;
            if (overText > 0)
            {
                string[] messages = debugText.text.Split(SPLIT, System.StringSplitOptions.None);
                Stack<string> messageStack = new Stack<string>();
                int lengthCount = 0;
                for (int i = messages.Length - 1; i >= 0; i--)
                {
                    lengthCount += messages[i].Length;
                    if (lengthCount < maxTextLength)
                    {
                        messageStack.Push(messages[i]);
                    }
                    else
                    {
                        break;
                    }
                }
                debugText.text = "";
                foreach (string str in messageStack)
                {
                    debugText.text += str;
                }
                //debugText2.text = debugText2.text.Substring(overText+5);
                //debugText2.text = "";
            }
            debugText.text += logtext;
            scrollRect.verticalNormalizedPosition = 0;
            if (logMessageType == eLogMessageType.ERROR)
            {
                Debug.LogError(timeCount + "-" + word);
                if (BattleManager.Instance.ShowErrorMessage)
                {
                    MainManager.Instance.WindowConfigMessage("<color=#FF0000>" + word + "</color>", null, null);
                }
            }
            else
            {
                Debug.Log(timeCount + "-" + word);
            }
            //Canvas.ForceUpdateCanvases();
        }
        public void ShowSkillName(string skillName, Transform transform)
        {
            GameObject a = Instantiate(skillNamePerfab);
            a.transform.SetParent(parent, false);
            a.GetComponent<SkillNameImage>().SetName(skillName, transform);

        }
        public void SetUI()
        {
            for (int i = 0; i < PlayerUI.Count; i++)
            {
                if (BattleManager.Instance.PlayersList.Count > i)
                {
                    GameObject a = Instantiate(buffUIPrefab);
                    CharacterBuffUIController b = a.GetComponent<CharacterBuffUIController>();
                    BattleManager.Instance.PlayersList[i].SetUI(PlayerUI[i], b);
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
                    BattleManager.Instance.EnemiesList[i].SetUI(EnemyUI[i], b);
                    b.SetBuffUI(rularSprites[i + 5], BattleManager.Instance.EnemiesList[i]);
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
            foreach (UnitCtrl a in BattleManager.Instance.PlayersList)
            {
                a.SetTimeScale(scale);
            }
            foreach (UnitCtrl b in BattleManager.Instance.EnemiesList)
            {
                b.SetTimeScale(scale);
            }
        }
        public Sprite GetAbnormalIconSprite(eStateIconType stateIconType)
        {
            int index = 40;
            try
            {
                index = int.Parse(stateIconType.GetDescription());
            }
            catch
            {
                LogMessage("未设置" + stateIconType.GetDescription() + "的技能图标！", eLogMessageType.ERROR, false);
            }
            return buffDebuffIcons[index];
        }
        public void SetDamageNumber(UnitCtrl source, UnitCtrl target, int value, eDamageType damageType, eDamageEffectType effectType, bool isCritical, bool isTotal)
        {
            string valuestr = value.ToString();
            List<Sprite> numbers = new List<Sprite>();
            List<Sprite> sprites = number_physical_large;
            if (damageType == eDamageType.MGC)
            {
                sprites = number_magical_large;
            }
            for (int i = 0; i < valuestr.Length; i++)
            {
                //int num = (int)valuestr[i];
                int num = (int)valuestr[i] - 48;
                numbers.Add(sprites[num]);
            }
            Sprite head = null;
            if (isCritical)
            {
                if (damageType == eDamageType.MGC)
                {
                    head = sprite_critical_magical;
                }
                else
                {
                    head = sprite_critical_physical;
                }
            }
            if (isTotal)
            {
                head = damageType == eDamageType.MGC ? sprite_total_magical : sprite_total_physical;
            }
            float scale = 1;
            if (effectType == eDamageEffectType.LARGE)
            {
                scale = 2;
            }
            Vector3 pos = target.transform.position + numberPosFix;
            SetPrefabNumber(source, pos, numbers, head, scale);
        }
        public void SetHealNumber(UnitCtrl source, UnitCtrl target, int value, float scale = 1)
        {
            string valuestr = value.ToString();
            List<Sprite> numbers = new List<Sprite>();
            List<Sprite> sprites = number_heal_large;
            for (int i = 0; i < valuestr.Length; i++)
            {
                //int num = (int)valuestr[i];
                int num = (int)valuestr[i] - 48;
                numbers.Add(sprites[num]);
            }
            Vector3 pos = target.transform.position + numberPosFix;
            SetPrefabNumber(source, pos, numbers, null, scale);
        }
        public void SetEnergyNumber(UnitCtrl source, UnitCtrl target, int value, float scale = 1)
        {
            string valuestr = value.ToString();
            List<Sprite> numbers = new List<Sprite>();
            List<Sprite> sprites = number_energy_large;
            for (int i = 0; i < valuestr.Length; i++)
            {
                //int num = (int)valuestr[i];
                int num = (int)valuestr[i] - 48;
                if (num >= 0 && num <= 9)
                {
                    numbers.Add(sprites[num]);
                }
                else
                {
                    Debug.LogError("伤害数字错误！错误数字：" + num);
                }
            }
            Vector3 pos = target.transform.position + numberPosFix;
            SetPrefabNumber(source, pos, numbers, null, scale);
        }
        public void SetMissEffect(UnitCtrl source, UnitCtrl target, float scale = 1)
        {
            Vector3 randomPos = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.25f), 0);
            Vector3 pos = target.transform.position + numberPosFix + randomPos;
            GameObject a = Instantiate(missPrefab);
            a.transform.position = pos;
            a.GetComponent<DamageNumbers>().SetMiss(source, scale);
        }

        private void SetPrefabNumber(UnitCtrl source, Vector3 pos, List<Sprite> numbers, Sprite head, float scale)
        {
            GameObject a = Instantiate(numberPrefab);
            Vector3 randomPos = new Vector3(Random.Range(-0.65f, 0.65f), Random.Range(-0.35f, 0.35f), 0);
            a.transform.position = pos + randomPos;
            a.GetComponent<DamageNumbers>().SetDamageNumber(source, numbers, head, scale);

        }
    }
}