using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PCRCaculator.Battle
{
    public class DamageNumbers : MonoBehaviour
    {
        public SpriteRenderer headSprite;
        public List<SpriteRenderer> numbers;
        public float countTime;
        public float speed;
        /// <summary>
        /// 设置伤害特效显示
        /// </summary>
        /// <param name="source">伤害源，用于获取deltatimeforpause</param>
        /// <param name="numberSprites">数字图片列表，最多6个</param>
        /// <param name="head">Total、Critical等，没有就写Null</param>
        /// <param name="scale"></param>
        public void SetDamageNumber(UnitCtrl source,List<Sprite> numberSprites,Sprite head,float scale = 1)
        {
            for(int i = 0; i < numbers.Count; i++)
            {
                if (i < numberSprites.Count)
                {
                    numbers[i].sprite = numberSprites[i];
                }
                else
                {
                    numbers[i].sprite = null;
                }
            }
            headSprite.sprite = head;
            transform.localScale = Vector3.one * scale;
            StartCoroutine(_UpdateNumber(source));
        }
        public void SetMiss(UnitCtrl source,float scale)
        {
            transform.localScale = Vector3.one * scale;
            StartCoroutine(_UpdateNumber(source));
        }
        private IEnumerator _UpdateNumber(UnitCtrl source)
        {
            float time_0 = countTime;
            float deltaTime = 0.0167f;
            while (time_0 >= 0)
            {
                deltaTime = source.DeltaTimeForPause;
                transform.Translate(Vector3.up * speed * deltaTime);
                time_0 -= deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}