using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldPlayer : MonoBehaviour
{
    [SerializeField]
    TextMeshPro Text;
    [SerializeField]
    int playernum;

    [SerializeField]
    SpriteRenderer Sprite;

    [SerializeField]
    AttackMenu AttackMenu;

    [SerializeField]
    AbilityMenu AbilityMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if((BattleField.CurrentMenuStatus == MenuStatus.AttackMenu && playernum == 1 && AttackMenu.Selected == 6) ||
           ((BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu && 
            AbilityMenu.Ability.Category != SelectCategory.Enchant &&
            AbilityMenu.Selected[playernum, 6]))){
            float gray = Mathf.PingPong(Time.time, 1);
            Sprite.color = new Color(gray, gray, gray);
        }else{
            Sprite.color = Color.white;
        }
        Text.text = BattleField.Hp[playernum].ToString();
    }

    public void OnClick(){
        if(BattleField.CurrentMenuStatus == MenuStatus.AttackMenu){
            if(playernum == 1){
                AttackMenu.Select(6);
            }
        }else if(BattleField.CurrentMenuStatus == MenuStatus.AbilityMenu){
            if( AbilityMenu.Ability.Category == SelectCategory.UnitDeckMasterPlayer ){
                    AbilityMenu.Select(playernum, 6);
            }
        }
    } 
}
