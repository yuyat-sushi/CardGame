using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaBoost", menuName = "CardGame/Ability/ManaBoost", order = 0)]
public class ManaBoost : BaseAbility {
    public override void Ability(int actplayer){
        bool player = actplayer == 0;
        BattleField.ManaIncrease(player);
    }
    public override void Ability(bool[,] selected, int actplayer){
        Ability(actplayer);
    }
}