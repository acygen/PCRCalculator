using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PCRCaculator.Battle
{

    public class CharacterBuffUIController : MonoBehaviour
    {
        public List<SpriteRenderer> abnormalIcons;
        public SpriteRenderer rular;
        public TextMesh posText;

        private UnitCtrl owner;
        private BattleUIManager uIManager;
        private List<eStateIconType> currentBuffs = new List<eStateIconType>();
        public void SetBuffUI(Sprite sprite,UnitCtrl owner)
        {
            rular.sprite = sprite;
            this.owner = owner;
            uIManager = BattleUIManager.Instance;
            transform.SetParent(owner.transform, false);
            SetAbnormalIcons(owner,eStateIconType.NONE,false);
            owner.OnChangeState += SetAbnormalIcons;
            StartCoroutine(ShowPosition());
        }
        public void SetAbnormalIcons(UnitCtrl unitCtrl,eStateIconType stateIconType,bool enable)
        {
            if(stateIconType == eStateIconType.NONE)
            {
                Reflash();
                return;
            }
            if (currentBuffs.Contains(stateIconType))
            {
                currentBuffs.Remove(stateIconType);
            }
            if (enable)
            {
                currentBuffs.Add(stateIconType);
            }
            Reflash();
        }
        private void Reflash()
        {
            for(int i = 0; i < 4; i++)
            {
                if (i < currentBuffs.Count)
                {
                    abnormalIcons[i].sprite = uIManager.GetAbnormalIconSprite(currentBuffs[i]);
                    abnormalIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    abnormalIcons[i].sprite = null;
                    abnormalIcons[i].gameObject.SetActive(false);
                }
            }
        }
        public void SetLeftDir(bool isReversed = false)
        {
            transform.localScale = new Vector3(isReversed ? -1 : 1, 1, 1);
        }

        private IEnumerator ShowPosition()
        {
            while (true)
            {
                posText.text = "" + Mathf.RoundToInt(owner.FixedPosition.x);
                yield return null;
            }
        }
    }
}