using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassivePowerDownIfOpoTurn", menuName = "CardGame/Ability/PassivePowerDownIfOpoTurn", order = 0)]
public class PassivePowerDownIfOpoTurn : BaseAbility {
    [field: SerializeField]
    public int DownPower{get; private set;}

    //PassivePowerDownIfOpoTurn
    //Passive効果のself前提
    //パワーを基本パワーに戻した後、もし相手ターン中だった場合パワーをダウンさせる
    public override void Ability(bool[,] selected, int actplayer){
        bool player = actplayer == 0;
        //パワーの計算
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 5; i++){
                if(selected[j,i]&&(player != BattleField.OperationPlayerCurrentTurn)){
                    BattleField.Unit[j,i].PowerSet(BattleField.Unit[j,i].BasePower - DownPower - BattleField.Unit[j,i].RecievedDamage + BattleField.Unit[j,i].RecievedBuff);
                }
            }
        }
    }
}