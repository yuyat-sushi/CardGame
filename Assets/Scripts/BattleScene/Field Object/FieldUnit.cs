using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldUnit : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer Sprite;
    [SerializeField]
    TextMeshPro Text;
    [SerializeField]
    Info Info;
    [SerializeField]
    DeckMasterInfo DeckMasterInfo;
    [SerializeField]
    CommandMenu CommandMenu;
    [SerializeField]
    AttackMenu AttackMenu;
    [SerializeField]
    ChangeMenu ChangeMenu;
    [SerializeField]
    AbilityMenu AbilityMenu;

    [SerializeField]
    MainCardDataBase CardDataBase = null;

    [SerializeField]
    int playernum;
    [SerializeField]
    int fieldnum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int cardnum = BattleField.Unit[playernum, fieldnum].CardID;
        if( (BattleField.CurrentMenuStatus == MenuStatus.AttackMenu && playernum == 1 && AttackMenu.Selected == fieldnum)||
            (BattleField.CurrentMenuStatus == MenuStatus.ChangeMenu && playernum == 0 && ChangeMenu.SelectCategory == SelectCategory.Unit && ChangeMenu.Selected == fieldnum)||
            (BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && 
             AbilityMenu.Ability.Category != SelectCategory.Enchant &&
             AbilityMenu.Selected[playernum, fieldnum])){
            if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && AbilityMenu.PlannedCategory == SelectCategory.Unit &&  playernum == 0 && AbilityMenu.PlannedNum == fieldnum){
                float blue = Mathf.PingPong(Time.time, 1);
                Sprite.color = new Color(blue, blue, 1);
            }else{
                float gray = Mathf.PingPong(Time.time, 1);
                Sprite.color = new Color(gray, gray, gray);
            }
        }else if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && playernum == 0 && AbilityMenu.PlannedNum == fieldnum && AbilityMenu.PlannedCategory == SelectCategory.Unit){
            Sprite.color = Color.blue;
        }else{
            Sprite.color = Color.white;
        }
        if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && playernum == 0 && AbilityMenu.PlannedNum == fieldnum && AbilityMenu.PlannedCategory == SelectCategory.Unit){
            UnitCardData unit = (UnitCardData)AbilityMenu.PlannedCard;
            Sprite.sprite = unit.Sprite;
            Text.text = "";
        }else if(cardnum >= 0){
            if(CardDataBase.Cards[cardnum] is UnitCardData){
                UnitCardData unit = (UnitCardData)CardDataBase.Cards[cardnum];
                Sprite.sprite = unit.Sprite;
                Text.text = BattleField.Unit[playernum, fieldnum].CurrentPower.ToString();
            }
        }else{
            Sprite.sprite = null;
            Text.text = "";
        }
    }

    public void OnClick()
    {
        int cardnum = BattleField.Unit[playernum, fieldnum].CardID;
        if(cardnum >= 0){
            Debug.Log("Clicked! playernum: " + playernum + "   fieldnum: " + fieldnum);
            if(playernum == 0 && BattleField.CurrentGamePhase == GamePhase.MainPhase && BattleField.CurrentMenuStatus == MenuStatus.NoMenu) {
                DeckMasterInfo.gameObject.SetActive(false);
                Info.gameObject.SetActive(false);
                CommandMenu.gameObject.SetActive(true);
                CommandMenu.Instantiate(fieldnum);
            }else{
                DeckMasterInfo.gameObject.SetActive(false);
                Info.gameObject.SetActive(true);
                Info.Instantiate(cardnum);
            }
            if(BattleField.CurrentMenuStatus == MenuStatus.ChangeMenu){
                if(playernum == 0 && ChangeMenu.SelectCategory == SelectCategory.Unit){
                    ChangeMenu.Select(fieldnum);
                }
            }else if(BattleField.CurrentMenuStatus == MenuStatus.AttackMenu){
                if(playernum == 1){
                    AttackMenu.Select(fieldnum);
                }
            }else if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu){
                UnitCardData unit = (UnitCardData)CardDataBase.Cards[cardnum];
                if( AbilityMenu.Ability.Category == SelectCategory.Unit ||
                    AbilityMenu.Ability.Category == SelectCategory.UnitDeckMaster ||
                    AbilityMenu.Ability.Category == SelectCategory.UnitDeckMasterPlayer ){
                    if((AbilityMenu.Ability.PowerLimit == -1 || AbilityMenu.Ability.PowerLimit >= BattleField.Unit[playernum, fieldnum].CurrentPower)&&
                       (AbilityMenu.Ability.CostLimit == -1 || AbilityMenu.Ability.CostLimit >= unit.Cost)){
                            AbilityMenu.Select(playernum, fieldnum);
                    }
                }
            }
        }else if(playernum == 0 && AbilityMenu.PlannedNum == fieldnum && BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu){
            if(AbilityMenu.Ability.Category == SelectCategory.Unit ||
               AbilityMenu.Ability.Category == SelectCategory.UnitDeckMaster ||
               AbilityMenu.Ability.Category == SelectCategory.UnitDeckMasterPlayer){
                UnitCardData unit = (UnitCardData)AbilityMenu.PlannedCard;
                if( (AbilityMenu.Ability.PowerLimit == -1||AbilityMenu.Ability.PowerLimit >= unit.Power)&&
                    (AbilityMenu.Ability.CostLimit == -1||AbilityMenu.Ability.CostLimit >= unit.Cost)){
                    AbilityMenu.Select(playernum, fieldnum);
                }
            }
        }
    }
}
