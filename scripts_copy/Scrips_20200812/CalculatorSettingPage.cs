using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PCRCaculator
{
    public class CalculatorSettingPage : MonoBehaviour
    {
        public Dropdown dropdown;
        public List<Toggle> toggles;
        public delegate void Config(int dropdownChoose, int toggleChoose);
        public Config config;
        public System.Action cancel;
        public void SetButton(Config config,System.Action action = null)
        {
            this.config = config;
            cancel = action;
        }
        public void ConfigButton()
        {
            int i = 1;
            int choose = 0;
            foreach(Toggle toggle in toggles)
            {
                if (toggle.isOn)
                {
                    choose = i;
                }
                i++;
            }
            config?.Invoke(dropdown.value,choose);
            Destroy(gameObject);
        }
        public void CancelButton()
        {
            cancel?.Invoke();
            Destroy(gameObject);
        }
    }
}