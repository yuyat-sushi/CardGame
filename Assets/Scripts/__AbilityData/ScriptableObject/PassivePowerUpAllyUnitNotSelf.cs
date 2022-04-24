using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassivePowerUpAllyUnitNotSelf", menuName = "CardGame/Ability/PassivePowerUpAllyUnitNotSelf", order = 0)]
public class PassivePowerUpAllyUnitNotSelf : BaseAbility {

    [field: SerializeField]
    public int Power{get; private set;}

    //PassivePowerUpAllyUnitNotSelf
    //Passive効果
    //Selfで選択されたであろうユニットを除いたユニットにCurrentPowerに増加パワーを足した値を設定する

    public override void Ability(bool[,] selected, int actplayer){
        //パワーの計算
        for(int i = 0; i < 5; i++){
            if(!selected[actplayer,i]) BattleField.Unit[actplayer,i].PowerSet(BattleField.Unit[actplayer,i].CurrentPower + Power);
        }
    }
}