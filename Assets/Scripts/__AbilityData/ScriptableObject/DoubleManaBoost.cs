using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoubleManaBoost", menuName = "CardGame/Ability/DoubleManaBoost", order = 0)]
public class DoubleManaBoost : BaseAbility {
    public override void Ability(int actplayer){
        bool player = actplayer == 0;
        BattleField.ManaIncrease(player);
        BattleField.ManaIncrease(player);
    }
    public override void Ability(bool[,] selected, int actplayer){
        Ability(actplayer);
    }
}