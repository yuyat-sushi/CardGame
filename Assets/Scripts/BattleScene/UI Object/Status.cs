using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Status : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Text = null;
    [SerializeField]
    int playernum = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string playerOrOpponent = (playernum == 0) ? "Player" : "Opponent";
        Text.text = playerOrOpponent + " HP: " + BattleField.Hp[playernum] + "\n" +
                    playerOrOpponent + " Mana: " + BattleField.Mana[playernum] + "/" + BattleField.MaxMana[playernum] + "\n" +
                    "Hand:" + BattleField.HandList[playernum].Count + " Deck:" + BattleField.DeckList[playernum].Count + "Trush:" + BattleField.TrushList[playernum].Count; 
    }
}
