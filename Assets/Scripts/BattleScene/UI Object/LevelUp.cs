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
    TextMeshProUGUI StatusText;

    float InvisibleCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InvisibleCount -= Time.deltaTime;
        if(InvisibleCount <= 0) gameObject.SetActive(false);
    }

    public void Instantiate(int player){
        int level = BattleField.DeckMaster[player].SealRank;
        LevelText.text = "Rank" + level;
        StatusText.text = "Power " + BattleField.DeckMaster[player].BasePower + "\n" + BattleField.DeckMaster[player].BaseKeyWord.ToString();
        InvisibleCount = 1.5f;
    }
}
