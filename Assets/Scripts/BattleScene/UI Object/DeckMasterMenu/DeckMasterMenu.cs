using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DeckMasterMenu : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Name;
    [SerializeField]
    TextMeshProUGUI Rank;
    [SerializeField]
    TextMeshProUGUI PowerValue;
    [SerializeField]
    TextMeshProUGUI SealValue;
    [SerializeField]
    TextMeshProUGUI Type;
    [SerializeField]
    TextMeshProUGUI KeyWord;
    [SerializeField]
    Image Color1;
    [SerializeField]
    Image Color2;

    [SerializeField]
    TextMeshProUGUI[] AbilityText = new TextMeshProUGUI[2];
    [SerializeField]
    Button[] AbilityButton = new Button[2];

    [SerializeField]
    TextMeshProUGUI[] SealText = new TextMeshProUGUI[4];

    [SerializeField]
    AttackMenu AttackMenu;
    [SerializeField]
    AbilityMenu AbilityMenu;

    [SerializeField]
    MainCardDataBase CardDataBase;

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
    public void Instantiate(){
        Name.text = BattleField.DeckMaster[0].Name;
        Rank.text = BattleField.DeckMaster[0].SealRank.ToString();
        PowerValue.text = BattleField.DeckMaster[0].CurrentPower.ToString();
        SealValue.text = BattleField.DeckMaster[0].SealMana + "/" + BattleField.DeckMaster[0].SealRank;
        Type.text = BattleField.DeckMaster[0].Types[0];
        if(BattleField.DeckMaster[0].Types[1] != "") Type.text += "/" + BattleField.DeckMaster[0].Types[1];
        switch(BattleField.DeckMaster[0].DeckMasterColor[0]){
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
        switch(BattleField.DeckMaster[0].DeckMasterColor[0]){
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
        KeyWord.text = "";
        for(int i = 0; i < 2; i++){
            //!(card.ActiveTurnOnce[0] && BattleField.Unit[0, fieldnum].ActiveThisTurn[0]) =
            //カードデータ側のActiveTurnOnceとフィールド側のActiveThisTurnが両方共trueでなかったらtrueと返す
            AbilityButton[i].interactable = (BattleField.DeckMaster[0].IsLiberation)&&
                                            (BattleField.DeckMaster[0].AbilityCard[i].Trigger == Trigger.Active)&&
                                            (BattleField.DeckMaster[0].AbilityCard[i].SealCost <= BattleField.DeckMaster[0].SealMana)&&
                                           !(BattleField.DeckMaster[0].AbilityCard[i].ActiveTurnOnce && BattleField.DeckMaster[0].ActiveThisTurn[i]);
            AbilityText[i].text = "";
            switch(BattleField.DeckMaster[0].AbilityCard[i].Trigger){
                case Trigger.Passive:
                    AbilityText[i].text += "【常時】\n";
                    break;
                case Trigger.entered:
                    AbilityText[i].text += "【登場時】\n";
                    break;
                case Trigger.destroyed:
                    AbilityText[i].text += "【破壊時】\n";
                    break;
                case Trigger.TurnStart:
                    AbilityText[i].text += "【自分ターン開始時】\n";
                    break;
                case Trigger.TurnEnd:
                    AbilityText[i].text += "【自分ターン終了時】\n";
                    break;
                case Trigger.Attacked:
                    AbilityText[i].text += "【攻撃時】\n";
                    break;
                case Trigger.Active:
                    AbilityText[i].text += "【シールコスト"+ BattleField.DeckMaster[0].AbilityCard[i].SealCost + " ";
                    if(BattleField.DeckMaster[0].AbilityCard[i].ActiveTurnOnce) AbilityText[i].text += " ターン1回";
                    AbilityText[i].text += "】\n";
                    break;
            }
            AbilityText[i].text += BattleField.DeckMaster[0].AbilityCard[i].Ability.Text + "\n";
        }
        for(int i = 0; i < 4; i++){
            if(i < BattleField.DeckMaster[0].SealRank){
                SealText[i].text = "Level "+ (i+1) + ":\n" + BattleField.DeckMaster[0].SealCard[i].Text;
                if(i < BattleField.DeckMaster[0].LiberationLevel){
                    SealText[i].color = Color.green;
                }else if(i == BattleField.DeckMaster[0].LiberationLevel){
                    SealText[i].color = Color.white;
                }else if(i > BattleField.DeckMaster[0].LiberationLevel){
                    SealText[i].color = Color.gray;
                }
            }else{
                SealText[i].text = "";
            }
        }
    }

    public void AttackCommandOnClick()
    {
        gameObject.SetActive(false);
        AttackMenu.Instantiate(5);
        AttackMenu.gameObject.SetActive(true);
    }

    public void Ability1CommandOnClick(){
        SelectAbility = 0;
        BattleField.DeckMasterActiveAbiliy();
        gameObject.SetActive(false);
        if(BattleField.DeckMaster[0].AbilityCard[SelectAbility].Ability.SelfCast){
            //自分自身を選択する
            AbilityMenu.SelectSelf(0,5);
            BattleField.SelectAbility();
        }else if(BattleField.DeckMaster[0].AbilityCard[SelectAbility].Ability.Selectable){
            //選択メニューに遷移
            AbilityMenu.Instantiate(BattleField.DeckMaster[0].AbilityCard[SelectAbility].Ability);
            AbilityMenu.gameObject.SetActive(true);
        }else{
            //そのまま実行する
            BattleField.SelectAbility();
        }
    }

    public void Ability2CommandOnClick(){
        SelectAbility = 1;
        BattleField.DeckMasterActiveAbiliy();
        gameObject.SetActive(false);
        if(BattleField.DeckMaster[0].AbilityCard[SelectAbility].Ability.SelfCast){
            //自分自身を選択する
            AbilityMenu.SelectSelf(0,5);
            BattleField.SelectAbility();
        }else if(BattleField.DeckMaster[0].AbilityCard[SelectAbility].Ability.Selectable){
            //選択メニューに遷移
            AbilityMenu.Instantiate(BattleField.DeckMaster[0].AbilityCard[SelectAbility].Ability);
            AbilityMenu.gameObject.SetActive(true);
        }else{
            //そのまま実行する
            BattleField.SelectAbility();
        }
    }
}