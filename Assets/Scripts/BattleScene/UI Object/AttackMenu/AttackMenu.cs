using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackMenu : MonoBehaviour
{
    [SerializeField]
    CommandMenu CommandMenu;
    [SerializeField]
    TextMeshProUGUI OpponentText;
    [SerializeField]
    TextMeshProUGUI PlayerText;
    [SerializeField]
    MainCardDataBase CardDataBase;
    
    [field: SerializeField]
    public int Selected{get; private set;} = 0;

    int Fieldnum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string AttackerName = "";
        string DefenserName = "";
        int AttackerPower = 0;
        int DefenserPower = 0;

        if(Fieldnum < 5){
            UnitCardData playerCard = (UnitCardData)CardDataBase.Cards[BattleField.Unit[0, Fieldnum].CardID];
            AttackerName = playerCard.CardName;
            AttackerPower = BattleField.Unit[0, Fieldnum].CurrentPower;
        }else if(Fieldnum == 5){
            AttackerName = BattleField.DeckMaster[0].Name;
            AttackerPower = BattleField.DeckMaster[0].CurrentPower;
        }
        if(Selected < 5){
            UnitCardData opponentCard = (UnitCardData)CardDataBase.Cards[BattleField.Unit[1, Selected].CardID];
            DefenserName = opponentCard.CardName;
            DefenserPower = BattleField.Unit[1, Selected].CurrentPower;
        }else if(Selected == 5){
            DefenserName = BattleField.DeckMaster[1].Name;
            DefenserPower = BattleField.DeckMaster[1].CurrentPower;
        }else if(Selected == 6){
            DefenserName = "相手プレイヤー";
            DefenserPower = BattleField.Hp[1];
        }

        OpponentText.text = DefenserName + "\n" + DefenserPower + " ⇒ " + (DefenserPower - AttackerPower);
        if(Selected != 6){
            PlayerText.text = AttackerName + "\n" + AttackerPower + " ⇒ " + (AttackerPower - DefenserPower);
        }else{
            PlayerText.text = AttackerName + "\n" + AttackerPower;
        }
    }

    public void Instantiate(int fieldnum){
        BattleField.MenuChange(MenuStatus.AttackMenu);
        Fieldnum = fieldnum;
        bool FieldCheck = false;
        for(int i = 0; i < 5; i++){
            FieldCheck = BattleField.Unit[1, i].CardID == -1;
            if(!FieldCheck) break;
        }
        if(FieldCheck){
            Selected = 6;
        }else{
            for(int i = 0; i < 5; i++){
                if(BattleField.Unit[1, i].CardID >= 0){
                    Selected = i;
                    break;
                }
            }
        }
    }

    public void Select(int select){
        if(select >= 5){
            //デッキマスターやプレイヤーを選択した場合、相手フィールドのユニットがいるかどうかを判定する
            //キーワード「無防備」ユニットがいる場合はその時点で選択出来るようになる
            int FieldCheck = 0;
            for(int i = 0; i < 5; i++){
                if(BattleField.Unit[1,i].CardID != -1) FieldCheck++;
                if(BattleField.Unit[0,Fieldnum].CurrentKeyWord.Passing || BattleField.Unit[1,i].CurrentKeyWord.Defenseless){
                    FieldCheck = 0;
                    break;
                }
            }
            if(FieldCheck == 0){
                //フィールドチェックに成功した場合の処理
                //デッキマスターの場合は攻撃相手のデッキマスターが開放されており、スタンしていない場合のみ選択可能
                //プレイヤーはキーワード「近接」がついていなければ可能
                if((select == 5 && BattleField.DeckMaster[1].IsLiberation && BattleField.DeckMaster[1].StanCount <= 0)||(select == 6 && !BattleField.Unit[0,Fieldnum].CurrentKeyWord.Melee)){
                    Selected = select;
                }
            }
        }else{
            Selected = select;
        }
    }

    public void AttackButtonOnClick(){
        BattleField.MenuChange(MenuStatus.NoMenu);
        gameObject.SetActive(false);
        BattleField.Attack(Fieldnum, Selected);
    }

    public void CancelButtonOnClick(){
        BattleField.MenuChange(MenuStatus.NoMenu);
        gameObject.SetActive(false);
    }
}
