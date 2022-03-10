using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelUp : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI LevelText;

    [SerializeField]
    TextMeshProUGUI NextConditionText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Instantiate(int player){
        int level = BattleField.DeckMaster[player].LiberationLevel;
        if(!BattleField.DeckMaster[player].IsLiberation){
            LevelText.text = level + " ⇒ " + (level + 1);
            NextConditionText.text = "Next...\n" + BattleField.DeckMaster[player].SealCard[level].Text;
        }else{
            LevelText.text = "Rank" + level;
            NextConditionText.text = "Libelation...Now!";
        }
    }
}
