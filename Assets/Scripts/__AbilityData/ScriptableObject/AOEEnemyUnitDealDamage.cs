using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOEEnemyUnitDealDamage", menuName = "CardGame/Ability/AOEEnemyUnitDealDamage", order = 0)]
public class AOEEnemyUnitDealDamage : BaseAbility {
    [field: SerializeField]
    public int Damage{get; private set;}
    //SelectDealDamage
    //相手ユニット・デッキマスターにダメージを与える
    public override void Ability(int actplayer){
        int opponentplayer = actplayer == 0 ? 1 : 0;
        Debug.Log("SelectDealDamage");
        for(int i = 0; i < 5; i++){
            if(BattleField.Unit[opponentplayer,i].CardID != -1){
                BattleField.Unit[opponentplayer,i].Damage(Damage);
            }
        }
    }

    public override void Ability(bool[,] select, int actplayer){
        Ability(actplayer);
    }
}