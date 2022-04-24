using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectUntap", menuName = "CardGame/Ability/SelectUntap", order = 0)]
public class SelectUntap : BaseAbility {

    //SelectUnitUntap
    //選択したユニット・デッキマスターをアンタップする
    public override void Ability(bool[,] selected, int actplayer){
        Debug.Log("SelectUnitUntap");
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 5; i++){
                if(selected[j,i]){
                    BattleField.Unit[j,i].UnTap();
                }
            }
            if(selected[j,5]){
                BattleField.DeckMaster[j].UnTap();
            }
        }
    }
}