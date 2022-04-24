using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectPowerUpWithKeyword", menuName = "CardGame/Ability/SelectPowerUpWithKeyword", order = 0)]
public class SelectPowerUpWithKeyword : BaseAbility {
    [field: SerializeField]
    public int Power{get; private set;}
    [field: SerializeField]
    public KeyWord KeyWord{get; private set;}

    //SelectPowerUp
    //Powerの値分、selectedにあるユニットのパワーを増減する
    public override void Ability(bool[,] selected, int actplayer){
        Debug.Log("SelectPowerUp");
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 5; i++){
                if(selected[j,i]){
                    BattleField.Unit[j,i].BasePowerUpDown(Power);
                    BattleField.Unit[j,i].CurrentKeyWord.AddKeyword(KeyWord);
                }
            }
            if(selected[j,5]){
                BattleField.DeckMaster[j].BasePowerUpDown(Power);
                BattleField.DeckMaster[j].CurrentKeyWord.AddKeyword(KeyWord);
            }
        }
    }
}