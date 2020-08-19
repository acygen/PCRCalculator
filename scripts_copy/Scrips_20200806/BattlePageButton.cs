using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PCRCaculator
{


    public class BattlePageButton : MonoBehaviour
    {
        public TextMeshProUGUI positionText;//名次
        public Text nameText;
        public Text levelText;
        public Text totalPointText;//总战力
        public List<CharacterPageButton> characters;

        private int pos = 0;
        public void SetButton(AddedPlayerData player, int pos)
        {
            this.pos = pos;
            positionText.text = pos + "";
            nameText.text = player.playerName;
            levelText.text = player.playerLevel + "";
            totalPointText.text = player.totalpoint + "";
            for (int i = 0; i < 5; i++)
            {
                if (player.playrCharacters.Count > i)
                {
                    characters[i].SetButton(player.playrCharacters[i]);
                }
                else
                {
                    characters[i].SetButton(-1);
                }
            }
        }
        public void BattleButton()
        {
            JJCManager.Instance.AttackButton(pos);
        }
        public void DeleteButton()
        {
            JJCManager.Instance.DeleteButton(pos);
        }
    }
}