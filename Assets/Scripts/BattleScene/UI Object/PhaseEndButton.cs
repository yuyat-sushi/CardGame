using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseEndButton : MonoBehaviour
{
    [SerializeField]
    Button Button;
    void Update() {
        Button.interactable = BattleField.CurrentMenuStatus == MenuStatus.NoMenu;
    }

    public void OnClick(){
        BattleField.PhaseChange(GamePhase.EndPhase);
    }
}
