using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeMenu : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI BeforeText;
    [SerializeField]
    TextMeshProUGUI AfterText;
    [SerializeField]
    MainCardDataBase CardDataBase;
    
    [field: SerializeField]
    public int Selected{get; private set;} = 0;

    [field: SerializeField]
    public SelectCategory SelectCategory{get; private set;} = SelectCategory.Unit;
    int CardNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MainCardData PrayCard = CardDataBase.Cards[CardNumber];
        MainCardData TargetCard;
        if(SelectCategory == SelectCategory.Unit){
            TargetCard = CardDataBase.Cards[BattleField.Unit[0, Selected].CardID];
        }else{
            TargetCard = CardDataBase.Cards[BattleField.Enchant[0, Selected].CardID];
        }

        BeforeText.text = TargetCard.Cost + "コスト\n" + TargetCard.CardName;
        AfterText.text = PrayCard.Cost + "コスト\n" + PrayCard.CardName;
    }

    public void Instantiate(SelectCategory select, int cardnum){
        Debug.Log("Instantiate");
        BattleField.MenuChange(MenuStatus.ChangeMenu);
        SelectCategory = select;
        CardNumber = cardnum;
        Selected = 0;
    }

    public void Select(int select){
        Selected = select;
    }

    public void ChangeButtonOnClick(){
        Debug.Log("ChangeButton");
        BattleField.MenuChange(MenuStatus.NoMenu);
        BattleField.SelectSummonSpace();
        gameObject.SetActive(false);
    }

    public void CancelButtonOnClick(){
        BattleField.MenuChange(MenuStatus.NoMenu);
        BattleField.ResetAbilityFlag();
        gameObject.SetActive(false);
    }

}
