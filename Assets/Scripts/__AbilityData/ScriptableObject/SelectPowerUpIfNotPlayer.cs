using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectPowerUpIfNotPlayer", menuName = "CardGame/Ability/SelectPowerUpIfNotPlayer", order = 0)]
public class SelectPowerUpIfNotPlayer : BaseAbility {
    [field: SerializeField]
    public int Power{get; private set;}

    //SelectPowerUp
    //もし相手プレイヤーが選択されていなかった場合、Powerの値分、selectedにあるユニットのパワーを増減する
    public override void Ability(bool[,] selected, int actplayer){
        Debug.Log("SelectPowerUp");
        int opponentplayer = actplayer == 0 ? 1 : 0;
        if(!selected[opponentplayer,6]){
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
}