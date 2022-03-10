using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CommandMenu : MonoBehaviour
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
    TextMeshProUGUI PowerValue;
    [SerializeField]
    TextMeshProUGUI KeyWord;
    [SerializeField]
    TextMeshProUGUI Type;
    [SerializeField]
    Button AttackButton;
    [SerializeField]
    TextMeshProUGUI[] AbilityTextMeshProUGUI = new TextMeshProUGUI[2];
    [SerializeField]
    Button[] AbilityButton = new Button[2];

    [SerializeField]
    AttackMenu AttackMenu;

    [SerializeField]
    AbilityMenu AbilityMenu;

    [SerializeField]
    MainCardDataBase CardDataBase;

    [field: SerializeField]
    public int SelectNumber{get; private set;} = 0;

    [field: SerializeField]
    public int SelectAbility{get; private set;} = 0;

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
        UnitCardData card = (UnitCardData)CardDataBase.Cards[BattleField.Unit[0, fieldnum].CardID];
        Cost.text = card.Cost.ToString();
        Name.text = card.CardName;
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
        PowerValue.text = BattleField.Unit[0, fieldnum].CurrentPower.ToString();
        KeyWord.text = card.KeyWord.ToString();
        AttackButton.interactable = !BattleField.Unit[0,fieldnum].TapMode&&!BattleField.Unit[0,fieldnum].CurrentKeyWord.Immobile;
        for(int i = 0; i < 2; i++){
            //!(card.ActiveTurnOnce[0] && BattleField.Unit[0, fieldnum].ActiveThisTurn[0]) =
            //カードデータ側のActiveTurnOnceとフィールド側のActiveThisTurnが両方共trueでなかったらtrueと返す
            AbilityButton[i].interactable = (card.Trigger[i] == Trigger.Active)&&
                                            (card.ActiveManaCost[i] <= BattleField.Mana[0])&&
                                           !(card.ActiveTurnOnce[i] && BattleField.Unit[0, fieldnum].ActiveThisTurn[i])&&
                                           !(card.ActiveDontSummonTurn[i] && BattleField.Unit[0, fieldnum].SummonThisTurn);
            AbilityTextMeshProUGUI[i].text = "";
            switch(card.Trigger[i]){
                case Trigger.Passive:
                    AbilityTextMeshProUGUI[i].text += "【常時】\n";
                    break;
                case Trigger.entered:
                    AbilityTextMeshProUGUI[i].text += "【登場時】\n";
                    break;
                case Trigger.destroyed:
                    AbilityTextMeshProUGUI[i].text += "【破壊時】\n";
                    break;
                case Trigger.TurnStart:
                    AbilityTextMeshProUGUI[i].text += "【自分ターン開始時】\n";
                    break;
                case Trigger.TurnEnd:
                    AbilityTextMeshProUGUI[i].text += "【自分ターン終了時】\n";
                    break;
                case Trigger.Attacked:
                    AbilityTextMeshProUGUI[i].text += "【攻撃時】\n";
                    break;
                case Trigger.Active:
                    AbilityTextMeshProUGUI[i].text += "【"+ card.ActiveManaCost[i] +"マナ";
                    if(card.ActiveTurnOnce[i]) AbilityTextMeshProUGUI[i].text += " ターン1回";
                    if(card.ActiveDontSummonTurn[i]) AbilityTextMeshProUGUI[i].text += " 召喚ターン不可";
                    AbilityTextMeshProUGUI[i].text += "】\n";
                    break;
            }
            AbilityTextMeshProUGUI[i].text += card.Ability[i].Text + "\n";
        }
    }

    public void AttackCommandOnClick()
    {
        gameObject.SetActive(false);
        AttackMenu.Instantiate(SelectNumber);
        AttackMenu.gameObject.SetActive(true);
    }

    public void Ability1CommandOnClick(){
        SelectAbility = 0;
        BattleField.UnitActiveAbility();
        UnitCardData card = (UnitCardData)CardDataBase.Cards[BattleField.Unit[0, SelectNumber].CardID];
        gameObject.SetActive(false);
        if(card.Ability[0].SelfCast){
            //自分自身を選択する
            AbilityMenu.SelectSelf(0,SelectNumber);
            BattleField.SelectAbility();
        }else if(card.Ability[0].Selectable){
            //選択メニューに遷移
            AbilityMenu.Instantiate(card.Ability[SelectAbility]);
            AbilityMenu.gameObject.SetActive(true);
        }else{
            //そのまま実行する
            BattleField.SelectAbility();
        }
    }

    public void Ability2CommandOnClick(){
        SelectAbility = 1;
        BattleField.UnitActiveAbility();
        UnitCardData card = (UnitCardData)CardDataBase.Cards[BattleField.Unit[0, SelectNumber].CardID];
        gameObject.SetActive(false);
        if(card.Ability[1].SelfCast){
            //自分自身を選択する
            AbilityMenu.SelectSelf(0,SelectNumber);
            BattleField.SelectAbility();
        }else if(card.Ability[1].Selectable){
            //選択メニューに遷移
            AbilityMenu.Instantiate(card.Ability[SelectAbility]);
            AbilityMenu.gameObject.SetActive(true);
        }else{
            //そのまま実行する
            BattleField.SelectAbility();
        }
    }
}
