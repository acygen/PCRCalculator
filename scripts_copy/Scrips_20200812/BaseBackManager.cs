using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace PCRCaculator
{


    public class BaseBackManager : MonoBehaviour
    {
        public static BaseBackManager Instance;

        public GameObject latestUIback;//最上方ui底板，用于动态添加UIperferb
        public Text debugtext;
        public TextMeshProUGUI playerLevelText;
        public List<Slider> sliders;
        public List<TextMeshProUGUI> texts;
        public GameObject SettingBack;
        public PlayerSetting PlayerSetting { get => MainManager.Instance.PlayerSetting; }

        private void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 主界面设置按钮
        /// </summary>
        public void SettingButton()
        {
            SettingBack.SetActive(true);
            sliders[0].value = PlayerSetting.playerLevel;
            sliders[1].value = PlayerSetting.playerProcess;
            texts[0].text = PlayerSetting.playerLevel + "";
            texts[1].text = PlayerSetting.playerProcess + "";
        }
        /// <summary>
        /// 滑动条函数
        /// </summary>
        public void OnSliderDraged()
        {
            PlayerSetting.playerLevel = (int)sliders[0].value;
            PlayerSetting.playerProcess = (int)sliders[1].value;
            texts[0].text = PlayerSetting.playerLevel + "";
            texts[1].text = PlayerSetting.playerProcess + "";
            playerLevelText.text = PlayerSetting.playerLevel + "";

        }
        /// <summary>
        /// 设置界面保存按钮
        /// </summary>
        public void SaveButton()
        {
            MainManager.Instance.SavePlayerSetting();
            MainManager.Instance.WindowMessage("保存成功！");
        }
        public void DeleteButton()
        {
            MainManager.Instance.DeletePlayerData();
        }
        public void AddButton(int id)
        {
            if (sliders[id].value < sliders[id].maxValue)
            {
                sliders[id].value++;
            }
        }
        public void MinusButton(int id)
        {
            if (sliders[id].value > sliders[id].minValue)
            {
                sliders[id].value--;
            }
        }
        public void MainUnderButton(int id)
        {
            switch (id)
            {
                case 1:
                    MainManager.Instance.HomeButton();
                    break;
                case 2:
                    MainManager.Instance.CharacterButton();
                    break;
                case 3:
                    MainManager.Instance.BattleButton();
                    break;
                case 4:
                    MainManager.Instance.GambleButton();
                    break;
                case 5:
                    MainManager.Instance.CalculatorButton();
                    break;
            }
        }
    }
}