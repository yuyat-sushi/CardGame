using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldDeckMaster : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer Sprite;
    [SerializeField]
    TextMeshPro Text;
    [SerializeField]
    int playernum;
    [SerializeField]
    Button AttackCommand;
    [SerializeField]
    AttackMenu AttackMenu;
    [SerializeField]
    AbilityMenu AbilityMenu;
    [SerializeField]
    DeckMasterMenu DeckMasterMenu;
    [SerializeField]
    Info Info;
    [SerializeField]
    DeckMasterInfo DeckMasterInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = BattleField.DeckMaster[playernum].CurrentPower.ToString();
        if(BattleField.DeckMaster[playernum].IsLiberation){
            if((BattleField.CurrentMenuStatus == MenuStatus.AttackMenu && playernum == 1 && AttackMenu.Selected == 5) ||
               ((BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && 
                AbilityMenu.Ability.Category != SelectCategory.Enchant &&
                AbilityMenu.Selected[playernum, 5]))){
                float gray = Mathf.PingPong(Time.time, 1);
                Sprite.color = new Color(gray, gray, gray);
            }else{
                Sprite.color = Color.white;
            }
        }else{
            Sprite.color = Color.black;
        }
    }

    public void OnClick(){
        if(playernum == 0 && BattleField.CurrentGamePhase == GamePhase.MainPhase && BattleField.CurrentMenuStatus == MenuStatus.NoMenu) {
            AttackCommand.interactable = !BattleField.DeckMaster[0].TapMode && BattleField.DeckMaster[0].IsLiberation && (BattleField.DeckMaster[0].StanCount == 0);
            Info.gameObject.SetActive(false);
            DeckMasterInfo.gameObject.SetActive(false);
            DeckMasterMenu.Instantiate();
            DeckMasterMenu.gameObject.SetActive(true);
        }else{
            Info.gameObject.SetActive(false);
            DeckMasterInfo.Instantiate(playernum);
            DeckMasterInfo.gameObject.SetActive(true);
        }
        if(BattleField.CurrentMenuStatus == MenuStatus.AttackMenu){
            if(playernum == 1){
                AttackMenu.Select(5);
            }
        }
        if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu){
            if( AbilityMenu.Ability.Category == SelectCategory.UnitDeckMaster ||
                AbilityMenu.Ability.Category == SelectCategory.UnitDeckMasterPlayer ){
                if((AbilityMenu.Ability.PowerLimit == -1 || AbilityMenu.Ability.PowerLimit >= BattleField.DeckMaster[playernum].CurrentPower)){
                        AbilityMenu.Select(playernum, 5);
                }
            }
        }
    }
}
