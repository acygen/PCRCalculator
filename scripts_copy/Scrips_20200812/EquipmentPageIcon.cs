using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PCRCaculator
{
    public class EquipmentPageIcon : MonoBehaviour
    {
        public Image equipmentIcon;
        public Text numText;
        
        public void SetEquipmentIcon(int equipmentid, int num)
        {
            string equip_path = "equipments/icon_equipment_" + equipmentid;
            Sprite im = MainManager.LoadSourceSprite(equip_path);
            if (im != null) 
            { 
                equipmentIcon.sprite = im;
            }
            else 
            { 
                equipmentIcon.sprite = MainManager.LoadSourceSprite("equipments/icon_equipment_999999");
            }

            if (num >= 2)
            {
                numText.text = "x" + num;
                numText.gameObject.SetActive(true);
            }
            else
            {
                numText.gameObject.SetActive(false);
            }
        }
    }
}