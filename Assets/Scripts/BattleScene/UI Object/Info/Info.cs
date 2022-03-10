using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Info : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Cost;
    [SerializeField]
    TextMeshProUGUI Name;
    [SerializeField]
    Image Color1;
    [SerializeField]
    Image Color2;
    [SerializeField]
    TextMeshProUGUI Power;
    [SerializeField]
    TextMeshProUGUI PowerValue;
    [SerializeField]
    TextMeshProUGUI Type;
    [SerializeField]
    TextMeshProUGUI CardText;
    [SerializeField]
    MainCardDataBase CardDataBase;

    MainCardData Card;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Instantiate(int cardnum){
        Card = CardDataBase.Cards[cardnum];
        Cost.text = Card.Cost.ToString();
        Name.text = Card.CardName;
        Type.text = Card.Types[0];
        if(Card.Types[1] != "") Type.text += "/" + Card.Types[1];
        switch(Card.Elements[0]){
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
        switch(Card.Elements[1]){
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
        //カードの種別をサブクラスの型によって判別する
        if(Card is UnitCardData){
            UnitCardData unit = (UnitCardData)Card;
            Power.text = "Power";
            CardText.text = unit.KeyWord.ToString();
            for(int i = 0; i < 2; i++){
                switch(unit.Trigger[i]){
                    case Trigger.Passive:
                        CardText.text += "【常時】\n";
                        break;
                    case Trigger.entered:
                        CardText.text += "【登場時】\n";
                        break;
                    case Trigger.destroyed:
                        CardText.text += "【破壊時】\n";
                        break;
                    case Trigger.TurnStart:
                        CardText.text += "【自分ターン開始時】\n";
                        break;
                    case Trigger.TurnEnd:
                        CardText.text += "【自分ターン終了時】\n";
                        break;
                    case Trigger.Attacked:
                        CardText.text += "【攻撃時】\n";
                        break;
                    case Trigger.Active:
                        CardText.text += "【"+ unit.ActiveManaCost[i] +"マナ";
                        if(unit.ActiveTurnOnce[i]) CardText.text += " ターン1回";
                        if(unit.ActiveDontSummonTurn[i]) CardText.text += " 召喚ターン不可";
                        CardText.text += "】\n";
                        break;
                }
                CardText.text += unit.Ability[i].Text + "\n";
            }
            PowerValue.text = unit.Power.ToString();
        }
        if(Card is SpellCardData){
            SpellCardData spell = (SpellCardData)Card;
            Power.text = "";
            CardText.text = spell.Ability.Text;
            PowerValue.text = "Spell";
        }
        if(Card is EnchantCardData){
            EnchantCardData enchant = (EnchantCardData)Card;
            Power.text = "";
            CardText.text = "";
                switch(enchant.Trigger){
                    case Trigger.Passive:
                        CardText.text += "【常時】\n";
                        break;
                    case Trigger.entered:
                        CardText.text += "【登場時】\n";
                        break;
                    case Trigger.destroyed:
                        CardText.text += "【破壊時】\n";
                        break;
                    case Trigger.TurnStart:
                        CardText.text += "【自分ターン開始時】\n";
                        break;
                    case Trigger.TurnEnd:
                        CardText.text += "【自分ターン終了時】\n";
                        break;
                    case Trigger.Active:
                        CardText.text += "【"+ enchant.ActiveManaCost +"マナ";
                        if(enchant.ActiveTurnOnce) CardText.text += " ターン1回";
                        if(enchant.ActiveDontSummonTurn) CardText.text += " 召喚ターン不可";
                        CardText.text += "】\n";
                        break;
                }
                CardText.text += enchant.Ability.Text + "\n";
            PowerValue.text = "Enchant";
        }
    }
}
