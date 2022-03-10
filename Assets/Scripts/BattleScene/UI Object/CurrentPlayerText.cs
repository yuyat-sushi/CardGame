using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentPlayerText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Text = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = "Turn:" + BattleField.TurnCount + "\n" + (BattleField.OperationPlayerCurrentTurn ? "Player Turn" : "Opponent Turn");
    }
}
