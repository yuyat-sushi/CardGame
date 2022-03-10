using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinButton : MonoBehaviour
{
    public void OnClick(){
        BattleField.GameSet(BattleField.OperationPlayerCurrentTurn);
    }
}
