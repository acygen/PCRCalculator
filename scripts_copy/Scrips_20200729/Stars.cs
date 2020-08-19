using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 控制星数显示
/// </summary>
public class Stars : MonoBehaviour
{
    public List<Image> stars;
    public Sprite bright;
    public Sprite dark;
    /// <summary>
    /// 设置星星数量
    /// </summary>
    /// <param name="i">1-5,-1隐藏</param>
    /// <param name="max">最大星数，默认5，有的装备是3</param>
    public void SetStars(int i,int max = 5)
    {
        if (i == -1)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        for(int k = 0; k < stars.Count; k++)
        {
            if (k < i)
            {
                stars[k].sprite = bright;
            }
            else
            {
                stars[k].sprite = dark;
            }
            if (k < max)
            {
                stars[k].gameObject.SetActive(true);
            }
            else
            {
                stars[k].gameObject.SetActive(false);
            }
        }
    }
}
