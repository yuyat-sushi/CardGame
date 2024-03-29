using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDealDamage", menuName = "CardGame/Ability/EnemyDealDamage", order = 0)]
public class EnemyDealDamage : BaseAbility {
    [field: SerializeField]
    public int Damage{get; private set;}
    public override void Ability(int actplayer){
        bool opponentplayer = actplayer != 0;
        BattleField.DealDamage(opponentplayer, Damage);
    }
    public override void Ability(bool[,] selected, int actplayer){
        Ability(actplayer);
    }
}