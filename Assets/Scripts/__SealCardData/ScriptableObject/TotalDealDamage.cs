using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TotalDealDamege", menuName = "CardGame/SealCardData/TotalDealDamage", order = 0)]
public class TotalDealDamage : SealCardData {
        [field: SerializeField]
        public int RequireDamage{get; private set;}
        
        //相手プレイヤーにゲーム開始時からRequireDamage以上のダメージを与えたかの判定
        public override bool ConditionCheck(bool player){
                int enemyplayer = player ? 1 : 0;
                //Debug.Log("OneTurnDealDamage.ConditionCheck()");
                if(BattleField.TotalDealDamage[enemyplayer] >= RequireDamage){
                        return true;
                }else{
                        return false;
                }
        }
}