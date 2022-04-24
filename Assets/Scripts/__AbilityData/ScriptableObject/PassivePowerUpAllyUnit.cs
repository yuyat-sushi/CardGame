using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassivePowerUpAllyUnit", menuName = "CardGame/Ability/PassivePowerUpAllyUnit", order = 0)]
public class PassivePowerUpAllyUnit : BaseAbility {

    [field: SerializeField]
    public int Power{get; private set;}

    //PassivePowerUpAllyUnit
    //Passive効果
    //CurrentPowerに増加パワーを足した値を設定する
    public override void Ability(int actplayer){
        //パワーの計算
        for(int i = 0; i < 5; i++){
            BattleField.Unit[actplayer,i].PowerSet(BattleField.Unit[actplayer,i].CurrentPower + Power);
        }
    }

    public override void Ability(bool[,] selected, int actplayer){
        Ability(actplayer);
    }
}