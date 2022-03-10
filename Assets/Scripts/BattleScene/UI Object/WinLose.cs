using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinLose : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Text = null;
    private void OnEnable() {
        Text.text = BattleField.Winner ? "YouWin!" : "YouLose";
        Text.color = BattleField.Winner ? Color.red : Color.blue;
    }
}
