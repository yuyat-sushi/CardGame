using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DeckMasterInfo : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Name;
    [SerializeField]
    TextMeshProUGUI PowerValue;
    [SerializeField]
    TextMeshProUGUI Rank;
    [SerializeField]
    TextMeshProUGUI SealValue;
    [SerializeField]
    TextMeshProUGUI Type;
    [SerializeField]
    Image Color1;
    [SerializeField]
    Image Color2;

    [SerializeField]
    TextMeshProUGUI AbilityText;

    [SerializeField]
    TextMeshProUGUI SealText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Instantiate(int playernum){
        Name.text = BattleField.DeckMaster[playernum].Name;
        Rank.text = BattleField.DeckMaster[playernum].SealRank.ToString();
        PowerValue.text = BattleField.DeckMaster[playernum].CurrentPower.ToString();
        SealValue.text = BattleField.DeckMaster[playernum].SealMana + "/" + BattleField.DeckMaster[playernum].SealRank;
        Type.text = BattleField.DeckMaster[playernum].Types[0];
        if(BattleField.DeckMaster[playernum].Types[1] != "") Type.text += "/" + BattleField.DeckMaster[playernum].Types[1];
        switch(BattleField.DeckMaster[playernum].DeckMasterColor[0]){
            case Element.Black:
                Color1.color = Color.black;
                break;
            case Element.Blue:
                Color1.color = Color.blue;
                break;
            case Element.Green:
                Color1.color = Color.green;
                break;
            case Element.Red:
                Color1.color = Color.red;
                break;
            case Element.White:
                Color1.color = Color.yellow;
                break;
            case Element.None:
                Color1.color = Color.clear;
                break;
        }
        switch(BattleField.DeckMaster[playernum].DeckMasterColor[1]){
            case Element.Black:
                Color2.color = Color.black;
                break;
            case Element.Blue:
                Color2.color = Color.blue;
                break;
            case Element.Green:
                Color2.color = Color.green;
                break;
            case Element.Red:
                Color2.color = Color.red;
                break;
            case Element.White:
                Color2.color = Color.yellow;
                break;
            case Element.None:
                Color2.color = Color.clear;
                break;
        }
        AbilityText.text = "";
        for(int i = 0; i < 2; i++){
            switch(BattleField.DeckMaster[playernum].AbilityCard[i].Trigger){
                case Trigger.Passive:
                    AbilityText.text += "【常時】\n";
                    break;
                case Trigger.entered:
                    AbilityText.text += "【登場時】\n";
                    break;
                case Trigger.destroyed:
                    AbilityText.text += "【破壊時】\n";
                    break;
                case Trigger.TurnStart:
                    AbilityText.text += "【自分ターン開始時】\n";
                    break;
                case Trigger.TurnEnd:
                    AbilityText.text += "【自分ターン終了時】\n";
                    break;
                case Trigger.Attacked:
                    AbilityText.text += "【攻撃時】\n";
                    break;
                case Trigger.Active:
                    AbilityText.text += "【シールコスト"+ BattleField.DeckMaster[playernum].AbilityCard[i].SealCost + " ";
                    if(BattleField.DeckMaster[playernum].AbilityCard[i].ActiveTurnOnce) AbilityText.text += " ターン1回";
                    AbilityText.text += "】\n";
                    break;
            }
            AbilityText.text += BattleField.DeckMaster[playernum].AbilityCard[i].Ability.Text + "\n";
        }
        SealText.text = "";
        for(int i = 0; i < 4; i++){
            if(i < BattleField.DeckMaster[playernum].SealRank){
                SealText.text += "Level "+ (i+1) + ":\n" + BattleField.DeckMaster[playernum].SealCard[i].Text + "\n";
            }
        }
    }
}