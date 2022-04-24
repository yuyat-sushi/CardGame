using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectDealDamage", menuName = "CardGame/Ability/SelectDealDamage", order = 0)]
public class SelectDealDamage : BaseAbility {
    [field: SerializeField]
    public int Damage{get; private set;}
    //SelectDealDamage
    //選択したユニット・デッキマスターをアンタップする
    public override void Ability(bool[,] selected, int actplayer){
        Debug.Log("SelectDealDamage");
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 5; i++){
                if(selected[j,i]){
                    BattleField.Unit[j,i].Damage(Damage);
                }
            }
            if(selected[j,5]){
                BattleField.DeckMaster[j].Damage(Damage);
            }
            if(selected[j,6]){
                BattleField.DealDamage(!BattleField.OperationPlayerCurrentTurn, Damage);
            }
        }
    }
}