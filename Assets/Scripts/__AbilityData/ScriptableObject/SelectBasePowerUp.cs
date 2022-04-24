using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectBasePowerUp", menuName = "CardGame/Ability/SelectBasePowerUp", order = 0)]
public class SelectBasePowerUp : BaseAbility {
    [field: SerializeField]
    public int Power{get; private set;}

    //SelectBasePowerUp
    //Powerの値分、selectedにあるユニットのパワーを増減する
    public override void Ability(bool[,] selected, int actplayer){
        Debug.Log("SelectBasePowerUp");
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