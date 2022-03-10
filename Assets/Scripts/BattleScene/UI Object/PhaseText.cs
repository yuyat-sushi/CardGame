using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhaseText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(BattleField.CurrentGamePhase){
            case GamePhase.GameStart:
                Text.text = "GameStart";
                break;
            case GamePhase.DrawPhase:
                Text.text = "DrawPhase";
                break;
            case GamePhase.MainPhase:
                Text.text = "MainPhase";
                break;
            case GamePhase.EndPhase:
                Text.text = "EndPhase";
                break;
            case GamePhase.PlayerChange:
                Text.text = "PlayerChange";
                break;
        }
    }
}
