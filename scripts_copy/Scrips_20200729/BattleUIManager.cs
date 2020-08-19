using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PCRCaculator.Battle
{
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
        public List<CharacterPageButton> PlayerUI;
        public List<CharacterPageButton> EnemyUI;

        private void Awake()
        {
            Instance = this;
        }

        public void LogMessage(string word,bool isOther)
        {
            if (isOther)
            {
                debugText2.text += word+"\n";
                scrollRect2.verticalNormalizedPosition = 0;
            }
            else
            {
                debugText1.text += word + "\n";
                scrollRect1.verticalNormalizedPosition = 0;
            }
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