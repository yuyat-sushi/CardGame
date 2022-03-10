using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOEEnemyUnitDealDamage", menuName = "CardGame/Ability/AOEEnemyUnitDealDamage", order = 0)]
public class AOEEnemyUnitDealDamage : BaseAbility {
    [field: SerializeField]
    public int Damage{get; private set;}
    //SelectDealDamage
    //選択したユニット・デッキマスターをアンタップする
    public override void Ability(){
        Debug.Log("SelectDealDamage");
        for(int i = 0; i < 5; i++){
            if(BattleField.Unit[1,i].CardID != -1){
                BattleField.Unit[1,i].Damage(Damage);
            }
        }
    }

    public override void Ability(bool[,] select){
        Ability();
    }
}