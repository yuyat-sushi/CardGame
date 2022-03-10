using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OneTurnDealDamege", menuName = "CardGame/SealCardData/OneTurnDealDamage", order = 0)]
public class OneTurnDealDamage : SealCardData {
        [field: SerializeField]
        public int RequireDamage{get; private set;}
        
        //相手プレイヤーに1ターンの間RequireDamage以上のダメージを与えたかの判定
        public override bool ConditionCheck(bool player){
                int enemyplayer = player ? 1 : 0;
                //Debug.Log("OneTurnDealDamage.ConditionCheck()");
                if(BattleField.ThisTurnDealDamage[enemyplayer] >= RequireDamage){
                        return true;
                }else{
                        return false;
                }
        }
}