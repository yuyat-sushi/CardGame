using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectPowerUp", menuName = "CardGame/Ability/SelectPowerUp", order = 0)]
public class SelectPowerUp : BaseAbility {
    [field: SerializeField]
    public int Power{get; private set;}

    //SelectPowerUp
    //Powerの値分、selectedにあるユニットのパワーを増減する
    public override void Ability(bool[,] selected, int actplayer){
        Debug.Log("SelectPowerUp");
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 5; i++){
                if(selected[j,i]){
                    BattleField.Unit[j,i].BasePowerUpDown(Power);
                }
            }
            if(selected[j,5]){
                BattleField.DeckMaster[j].BasePowerUpDown(Power);
            }
        }
    }
}