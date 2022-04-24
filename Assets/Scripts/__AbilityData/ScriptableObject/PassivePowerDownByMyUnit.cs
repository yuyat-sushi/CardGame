using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassivePowerDownByMyUnit", menuName = "CardGame/Ability/PassivePowerDownByMyUnit", order = 0)]
public class PassivePowerDownByMyUnit : BaseAbility {

    //PassivePowerDownByMyUnit
    //Passive効果のself前提
    //パワーを基本パワーに戻した後、バフ対象以外の自分ユニットの数だけパワーを-1000する
    public override void Ability(bool[,] selected, int actplayer){
        //Debug.Log("PassiveSelectPowerDown");
        //ユニット数の算出
        int[] unit = new int[2];
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 5; i++){
                if(BattleField.Unit[j,i].CardID != -1){
                    unit[j]++;
                }
            }
        }
        //パワーの計算
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 5; i++){
                if(selected[j,i]){
                    BattleField.Unit[j,i].PowerSet(BattleField.Unit[j,i].BasePower - (unit[j]-1)*1000 - BattleField.Unit[j,i].RecievedDamage + BattleField.Unit[j,i].RecievedBuff);
                }
            }
        }
    }
}