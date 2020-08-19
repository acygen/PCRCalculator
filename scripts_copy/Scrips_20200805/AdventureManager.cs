using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCRCaculator
{


    public class AdventureManager : MonoBehaviour
    {
        public static AdventureManager Instance;
        public GameObject backGround;//基础面板
        public GameObject JJCback;//jjc面板
        private void Awake()
        {
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }
        /// <summary>
        /// 开启战斗界面
        /// </summary>
        /// <param name="i">0-关闭所有界面，1-基础界面，2-JJC</param>
        public void SwitchPage(int i)
        {
            switch (i)
            {
                case 0:
                    backGround.SetActive(false);
                    JJCback.SetActive(false);
                    break;
                case 1:
                    backGround.SetActive(true);
                    JJCback.SetActive(false);
                    break;
                case 2:
                    backGround.SetActive(false);
                    JJCback.SetActive(true);
                    JJCback.GetComponent<JJCManager>().Reflash();
                    break;
            }
        }
    }
}