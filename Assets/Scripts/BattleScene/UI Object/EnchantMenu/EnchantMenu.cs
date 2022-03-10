using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnchantMenu : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Name;
    [SerializeField]
    TextMeshProUGUI CostValue;
    [SerializeField]
    TextMeshProUGUI Type;
    [SerializeField]
    Image Color1;
    [SerializeField]
    Image Color2;
    [SerializeField]
    TextMeshProUGUI AbilityText;
    [SerializeField]
    Button AbilityButton;

    [SerializeField]
    AbilityMenu AbilityMenu;

    [SerializeField]
    MainCardDataBase CardDataBase;

    [field: SerializeField]
    public int SelectNumber{get; private set;} = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Instantiate(int fieldnum){
        SelectNumber = fieldnum;
        EnchantCardData card = (EnchantCardData)CardDataBase.Cards[BattleField.Enchant[0, fieldnum].CardID];
        Name.text = card.CardName;
        CostValue.text = card.Cost.ToString();
        Type.text = card.Types[0];
        if(card.Types[1] != "") Type.text += "/" + card.Types[1];
        switch(card.Elements[0]){
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
        switch(card.Elements[1]){
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
        //!(card.ActiveTurnOnce[0] && BattleField.Unit[0, fieldnum].ActiveThisTurn[0]) =
        //カードデータ側のActiveTurnOnceとフィールド側のActiveThisTurnが両方共trueでなかったらtrueと返す
        AbilityButton.interactable = (card.Trigger == Trigger.Active)&&
                                     (card.ActiveManaCost <= BattleField.Mana[0])&&
                                    !(card.ActiveTurnOnce && BattleField.Enchant[0, fieldnum].ActiveThisTurn)&&
                                    !(card.ActiveDontSummonTurn && BattleField.Enchant[0, fieldnum].SummonThisTurn);
        AbilityText.text = "";
        switch(card.Trigger){
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
            case Trigger.Active:
                AbilityText.text += "【"+ card.ActiveManaCost +"マナ";
                if(card.ActiveTurnOnce) AbilityText.text += " ターン1回";
                if(card.ActiveDontSummonTurn) AbilityText.text += " 召喚ターン不可";
                AbilityText.text += "】\n";
                break;
        }
        AbilityText.text += card.Ability.Text + "\n";
    }

    public void AbilityCommandOnClick(){
        BattleField.EnchantActiveAbility();
        EnchantCardData card = (EnchantCardData)CardDataBase.Cards[BattleField.Enchant[0, SelectNumber].CardID];
        gameObject.SetActive(false);
        if(card.Ability.SelfCast){
            //自分自身を選択する
            AbilityMenu.SelectSelf(0,SelectNumber);
            BattleField.SelectAbility();
        }else if(card.Ability.Selectable){
            //選択メニューに遷移
            AbilityMenu.Instantiate(card.Ability);
            AbilityMenu.gameObject.SetActive(true);
        }else{
            //そのまま実行する
            BattleField.SelectAbility();
        }
    }
}
