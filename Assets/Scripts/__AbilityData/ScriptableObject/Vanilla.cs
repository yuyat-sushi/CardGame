using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vanilla", menuName = "CardGame/Ability/Vanilla", order = 0)]
public class Vanilla : BaseAbility {
    public override void Ability(){
        Debug.Log("Vanilla!");
    }
    public override void Ability(bool[,] selected){
        Ability();
    }
}