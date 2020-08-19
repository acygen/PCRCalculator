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
        public void SetBuffUI(Sprite sprite,UnitCtrl owner)
        {
            rular.sprite = sprite;
            this.owner = owner;
            transform.SetParent(owner.transform, false);
            SetAbnormalIcons();
            StartCoroutine(ShowPosition());
        }
        public void SetAbnormalIcons()
        {
            foreach (SpriteRenderer a in abnormalIcons)
            {
                a.gameObject.SetActive(false);
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