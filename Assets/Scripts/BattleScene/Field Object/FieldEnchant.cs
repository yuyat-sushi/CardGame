using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FieldEnchant : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer Sprite;

    [SerializeField]
    MainCardDataBase CardDataBase = null;
    [SerializeField]
    EnchantMenu EnchantMenu;
    [SerializeField]
    ChangeMenu ChangeMenu;
    [SerializeField]
    AbilityMenu AbilityMenu;
    [SerializeField]
    Info Info;
    [SerializeField]
    DeckMasterInfo DeckMasterInfo;

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
        int cardnum = BattleField.Enchant[playernum, fieldnum].CardID;
        if( (BattleField.CurrentMenuStatus == MenuStatus.ChangeMenu && playernum == 0 && ChangeMenu.SelectCategory == SelectCategory.Enchant && ChangeMenu.Selected == fieldnum)||
            (BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && AbilityMenu.Ability.Category == SelectCategory.Enchant && AbilityMenu.Selected[playernum, fieldnum])){
            if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && AbilityMenu.PlannedCategory == SelectCategory.Enchant && playernum == 0 && AbilityMenu.PlannedNum == fieldnum){
                float blue = Mathf.PingPong(Time.time, 1);
                Sprite.color = new Color(blue, blue, 1);
            }else{
                float gray = Mathf.PingPong(Time.time, 1);
                Sprite.color = new Color(gray, gray, gray);
            }
        }else if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && playernum == 0 && AbilityMenu.PlannedNum == fieldnum && AbilityMenu.PlannedCategory == SelectCategory.Enchant){
            Sprite.color = Color.blue;
        }else{
            Sprite.color = Color.white;
        }
        
        if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && playernum == 0 && AbilityMenu.PlannedNum == fieldnum && AbilityMenu.PlannedCategory == SelectCategory.Enchant){
            EnchantCardData enchant = (EnchantCardData)AbilityMenu.PlannedCard;
            Sprite.sprite = enchant.Sprite;
        }else if(cardnum >= 0){
            if(CardDataBase.Cards[cardnum] is EnchantCardData){
                EnchantCardData enchant = (EnchantCardData)CardDataBase.Cards[cardnum];
                Sprite.sprite = enchant.Sprite;
            }
        }else{
            Sprite.sprite = null;
        }
    }

    public void OnClick(){
        int cardnum = BattleField.Enchant[playernum, fieldnum].CardID;
        if(cardnum >= 0){
            Debug.Log("Clicked! playernum: " + playernum + "   fieldnum: " + fieldnum);
            if(playernum == 0 && BattleField.CurrentGamePhase == GamePhase.MainPhase && BattleField.CurrentMenuStatus == MenuStatus.NoMenu) {
                DeckMasterInfo.gameObject.SetActive(false);
                Info.gameObject.SetActive(false);
                EnchantMenu.Instantiate(fieldnum);
                EnchantMenu.gameObject.SetActive(true);
            }else{
                DeckMasterInfo.gameObject.SetActive(false);
                Info.gameObject.SetActive(true);
                Info.Instantiate(cardnum);
            }
            if(BattleField.CurrentMenuStatus == MenuStatus.ChangeMenu){
                if(playernum == 0 && ChangeMenu.SelectCategory == SelectCategory.Enchant){
                    ChangeMenu.Select(fieldnum);
                }
            }else if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu){
                EnchantCardData enchant = (EnchantCardData)CardDataBase.Cards[cardnum];
                if( AbilityMenu.Ability.Category == SelectCategory.Enchant&&
                    (AbilityMenu.Ability.CostLimit == -1||AbilityMenu.Ability.CostLimit <= enchant.Cost)){
                    AbilityMenu.Select(playernum, fieldnum);
                }
            }
        }else if(playernum == 0 && AbilityMenu.PlannedNum == fieldnum && BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu){
            if( AbilityMenu.Ability.Category == SelectCategory.Enchant){
                EnchantCardData enchant = (EnchantCardData)AbilityMenu.PlannedCard;
                if(AbilityMenu.Ability.CostLimit == -1||AbilityMenu.Ability.CostLimit <= enchant.Cost){
                    AbilityMenu.Select(playernum, fieldnum);
                }
            }
        }
    }
}
