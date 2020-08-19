using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PCRCaculator.Battle;

namespace PCRCaculator
{


    public class CharacterPageButton : MonoBehaviour
    {
        public enum ButtonType { typeA = 0, typeB = 1 ,typeC}//按钮样式，A-长条，B-图标
        public ButtonType buttonType = ButtonType.typeA;
        public bool showPosition;//是否显示位置图标
        public Stars stars;
        public Text levelText;
        public TextMeshProUGUI levelText_B;
        public Image characterImage;
        public List<Sprite> backgrounds;//0-4
        public Image characterPositionImage_B;
        public List<Sprite> positionSprites_B;//0-2
        public Image backImage;
        public Slider HPSlider;
        public Slider TPSlider;
        public List<Image> abnormalIcons;
        /// <summary>
        /// 设置组件
        /// </summary>
        /// <param name="unitid">角色序号</param>
        public void SetButton(int unitid, Sprite sprite = null)
        {
            if (unitid >= 200000 || unitid <= 0)
            {
                stars.SetStars(-1);
                if (buttonType == ButtonType.typeA)
                {
                    levelText.text = "";
                }
                else if (buttonType == ButtonType.typeB)
                {
                    levelText_B.text = "";
                    if (characterPositionImage_B != null)
                    {
                        characterPositionImage_B.gameObject.SetActive(false);
                    }
                }
                backImage.sprite = backgrounds[0];
                characterImage.sprite = sprite;
                return;
            }
            UnitData data = MainManager.Instance.unitDataDic[unitid];
            SetButton(data);
        }
        /// <summary>
        /// 设置组件
        /// </summary>
        /// <param name="data">角色详情</param>
        public void SetButton(UnitData data)
        {
            int unitid = data.unitId;
            stars.SetStars(data.rarity);
            int charid = data.rarity >= 3 ? unitid + 30 : unitid + 10;
            string path = "";
            if (buttonType == ButtonType.typeA)
            {
                path = "pictures/unit_plate_" + charid;
            }
            else if (buttonType == ButtonType.typeB || buttonType == ButtonType.typeC)
            {
                path = "charicons/fav_push_notif_" + unitid;
            }
            Sprite sprite = MainManager.LoadSourceSprite(path);
            if (sprite != null)
            {
                characterImage.sprite = MainManager.LoadSourceSprite(path);
            }
            int backtype = 0;
            if (data.rank <= 1)
            {
                backtype = 0;
            }
            else if (data.rank <= 3)
            {
                backtype = 1;
            }
            else if (data.rank <= 6)
            {
                backtype = 2;
            }
            else if (data.rank <= 12)
            {
                backtype = 3;
            }
            else
            {
                backtype = 4;
            }
            backImage.sprite = backgrounds[backtype];
            if (buttonType == ButtonType.typeA)
            {
                levelText.text = "等级" + data.level;
            }
            else if (buttonType == ButtonType.typeB)
            {
                levelText_B.text = "lv:" + data.level;
                if (showPosition)
                {
                    PositionType k = MainManager.Instance.UnitRarityDic[unitid].unitPositionType;
                    characterPositionImage_B.sprite = positionSprites_B[(int)k];
                }
            }
        }
        public void SetButton(UnitCtrl unitCtrl)
        {
            if(buttonType!= ButtonType.typeC)
            {
                Debug.LogError("按钮样式错误！");
                return;
            }
            SetButton(unitCtrl.UnitData);
            SetHPAndTP(1, 1);
            SetAbnormalIcons();
        }
        public void SetHPAndTP(float normalizedHPRate,float normalizedTPRate)
        {
            HPSlider.value = normalizedHPRate;
            TPSlider.value = normalizedTPRate;
        }
        public void SetAbnormalIcons()
        {
            foreach(Image a in abnormalIcons)
            {
                a.gameObject.SetActive(false);
            }
        }
    }
}