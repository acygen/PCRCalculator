using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PCRCaculator
{


    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance;
        public CharacterDetailManager characterDetailManager;
        public GameObject baseBack;
        public GameObject detailBack;
        public GameObject parent;
        public GameObject buttonPerferb;
        public Vector2 baseRange;//第一个按钮的位置
        public Vector2 range;//相邻按钮之间的距离
        public List<Toggle> toggles;

        public List<int> showUnitIDs = new List<int>();//展示出的人物id列表

        private List<GameObject> buttons = new List<GameObject>();

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 开启人物界面
        /// </summary>
        /// <param name="i">0-关闭所有界面，1-选择面板，2-角色详情面板</param>
        public void SwitchPage(int i)
        {
            if (i == 0)
            {
                baseBack.SetActive(false);
                detailBack.SetActive(false);
            }
            else if (i == 1)
            {
                baseBack.SetActive(true);
                detailBack.SetActive(false);
                ReflashBasePage(0);
            }
            else if (i == 2)
            {
                baseBack.SetActive(false);
                detailBack.SetActive(true);
            }
        }
        public void OnToggleSwitched(bool k)
        {
            if (k)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (toggles[i].isOn)
                    {
                        ReflashBasePage(i);
                    }
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
            foreach (GameObject a in buttons)
            {
                Destroy(a);
            }
            buttons.Clear();
            int count = 1;
            foreach (int id in MainManager.Instance.UnitRarityDic.Keys)
            {
                if (type == 0 || MainManager.Instance.UnitRarityDic[id].unitPositionType == positionType)
                {
                    if (id <= MainManager.Instance.loadCharacterMax)// || (id>=400000&&id<=499999))
                    {
                        GameObject b = Instantiate(buttonPerferb);
                        b.transform.SetParent(parent.transform);
                        b.transform.localScale = new Vector3(1, 1, 1);
                        b.transform.localPosition = new Vector3(baseRange.x + range.x * ((count - 1) % 3), -1 * (baseRange.y + range.y * (Mathf.FloorToInt((count - 1) / 3))), 0);
                        int id0 = id;
                        b.GetComponent<Button>().onClick.AddListener(delegate () { DetailButton(id0); });
                        b.GetComponent<CharacterPageButton>().SetButton(id0);
                        buttons.Add(b);
                        count++;
                        showUnitIDs.Add(id);                        
                    }
                }
            }
            parent.GetComponent<RectTransform>().sizeDelta = range * Mathf.CeilToInt(count/3.0f);

        }
        public void DetailButton(int id)
        {
            characterDetailManager.SetId(id);
        }

    }
}