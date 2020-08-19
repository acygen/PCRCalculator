using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PCRCaculator
{


    public class SkillDetailButton : MonoBehaviour
    {
        public Image smallIcon;
        public Image bigIcon;
        public Text nameText;
        public Text describeText;
        public Text detailText;
        public List<Vector3> IconPositions;
        /// <summary>
        /// 设置技能详情面板
        /// </summary>
        /// <param name="sprite">技能图标</param>
        /// <param name="skillid">技能id</param>
        /// <param name="skilllevel">技能等级</param>
        /// <param name="pos">技能位置0-UB；1-sk1；2-sk2；3-EX</param>
        public void SetDetails(Sprite sprite, int skillid, UnitData unitData, int pos)
        {
            SkillData sk = MainManager.Instance.SkillDataDic[skillid];
            smallIcon.sprite = sprite;
            bigIcon.sprite = sprite;
            if (MainManager.Instance.SkillNameAndDescribe_cn.ContainsKey(skillid))
            {
                string[] str = MainManager.Instance.SkillNameAndDescribe_cn[skillid];
                nameText.text = str[0];
                describeText.text = str[1];
            }
            else
            {
                nameText.text = sk.name;
                describeText.text = sk.describes;
            }
            detailText.text = sk.GetSkillDetails(pos, unitData);
            bigIcon.gameObject.transform.localPosition = IconPositions[pos];

        }
        /// <summary>
        /// 退出
        /// </summary>
        public void Exit()
        {
            Destroy(gameObject);
        }
    }
}