using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllAllyPowerUp", menuName = "CardGame/Ability/AllAllyPowerUp", order = 0)]
public class AllAllyPowerUp : BaseAbility {
    [field: SerializeField]
    public int Power{get; private set;}

    //SelectPowerUp
    //Powerの値分、selectedにあるユニットのパワーを増減する
    public override void Ability(bool[,] selected, int actplayer){
        Debug.Log("AllAllyPowerUp");
        for(int i = 0; i < 5; i++){
            if(selected[actplayer,i]){
                BattleField.Unit[actplayer,i].BasePowerUpDown(Power);
            }
        }
        if(selected[actplayer,5]){
            BattleField.DeckMaster[actplayer].BasePowerUpDown(Power);
        }
    }
}