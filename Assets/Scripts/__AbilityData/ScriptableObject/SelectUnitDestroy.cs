using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectUnitDestroy", menuName = "CardGame/Ability/SelectUnitDestory", order = 0)]
public class SelectUnitDestroy : BaseAbility {

    //SelectUnitDestroy
    //選択したユニットを破壊する
    public override void Ability(bool[,] selected){
        Debug.Log("SelectUnitDestroy");
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 5; i++){
                if(selected[j,i]){
                    BattleField.ToTrushFieldUnitCard(j == 0, i);
                }
            }
        }
    }
}