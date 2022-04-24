using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityMenu : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TargetText;
    [SerializeField]
    TextMeshProUGUI AbilityText;
    [SerializeField]
    MainCardDataBase CardDataBase;
    
    [field: SerializeField]
    public bool[,] Selected{get; private set;} = new bool[2, 7];
    
    public SelectCategory PlannedCategory{get; private set;}
    public int PlannedNum{get; private set;} = -1;
    public MainCardData PlannedCard{get; private set;}

    public BaseAbility Ability{get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Ability.Category == SelectCategory.Unit){
            TargetText.text =  "ユニット・デッキマスター";
        }else{
            TargetText.text =  "エンチャント";
        }
        TargetText.text += "\n" + Ability.SelectCount + "枚まで";
        AbilityText.text = Ability.Text;
    }

    public void Instantiate(BaseAbility ability, SelectCategory category, int num, MainCardData card){
        BattleField.MenuChange(MenuStatus.AbilityMenu);
        Ability = ability;
        PlannedCategory = category;
        PlannedNum = num;
        PlannedCard = card;
        ResetSelect();
    }

    public void Instantiate(BaseAbility ability){
        BattleField.MenuChange(MenuStatus.AbilityMenu);
        Ability = ability;
        PlannedNum = -1;
        ResetSelect();
    }

    public void ResetSelect(){
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 7; i++){
                Selected[j, i] = false;
            }
        }
    }

    public void Select(int player, int select){
        Selected[player, select] = !Selected[player, select];
        int counter = 0;
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 7; i++){
                counter += Selected[j,i] ? 1 : 0;
                if(counter > Ability.SelectCount){
                    Selected[player, select] = false;
                }
            }
        }
    }

    public void SelectSelf(int player, int select){
        for(int j = 0; j < 2; j++){
            for(int i = 0; i < 7; i++){
                Selected[j, i] = j == player && i == select;
            }
        }
    }

    public void AcceptButtonOnClick(){
        BattleField.MenuChange(MenuStatus.NoMenu);
        BattleField.SelectAbility();
        gameObject.SetActive(false);
    }

    public void CancelButtonOnClick(){
        BattleField.MenuChange(MenuStatus.NoMenu);
        BattleField.ResetAbilityFlag();
        gameObject.SetActive(false);
    }

}
