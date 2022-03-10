using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGenButton : MonoBehaviour
{
    [SerializeField]
    HandCard HandCardPrefab = null;
    [SerializeField]
    GameObject PlayerHandGrid = null;
    [SerializeField]
    GameObject OpponentHandGrid = null;

    public void OnClick(){
        HandCard card = Instantiate(HandCardPrefab) as HandCard;
        if(BattleField.OperationPlayerCurrentTurn){
            card.transform.SetParent(PlayerHandGrid.transform, false);
        }else{
            card.transform.SetParent(OpponentHandGrid.transform, false);
        }
        int cardnum = Random.Range(0, 5);
        card.Instantiate(cardnum);
        BattleField.AddHandCard(BattleField.OperationPlayerCurrentTurn, cardnum);
    }
}
