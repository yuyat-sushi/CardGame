using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HandCard : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler,IDropHandler
{
    [SerializeField]
    TextMeshProUGUI Cost = null;
    [SerializeField]
    TextMeshProUGUI CardName = null;
    [SerializeField]
    TextMeshProUGUI CardType = null;
    [SerializeField]
    Image Color1 = null;
    [SerializeField]
    Image Color2 = null;
    [SerializeField]
    TextMeshProUGUI CardText = null;
    [SerializeField]
    TextMeshProUGUI Type = null;
    [SerializeField]
    TextMeshProUGUI Power = null;
    [SerializeField]
    MainCardDataBase CardDataBase = null;

    MainCardData Card;

    GameObject Canvas;
    GameObject Grid;
    GameObject DroppableObject;
    ChangeMenu ChangeMenu;
    AbilityMenu AbilityMenu;
    Info Info;
    DeckMasterInfo DeckMasterInfo;

    RectTransform Rect;
    int CardNum;
    int PlannedNum;
    bool Dropped = false;
    bool Filled = false;
    int StartSibling;
    static int DropSibling;

    // Start is called before the first frame update
    void Start()
    {
        //キャンバスの取得
        Canvas = transform.root.gameObject;
        //カードグリッドの取得
        Grid = transform.parent.gameObject;
        //DroppableFieldの取得
        //他のオブジェクトのRayCastの兼ね合いもあってドラッグ時のみ有効化する
        DroppableObject = Canvas.transform.Find("DroppableField").gameObject;
        Rect = transform as RectTransform;
        ChangeMenu = Canvas.transform.Find("ChangeMenu").GetComponent<ChangeMenu>();
        AbilityMenu = Canvas.transform.Find("AbilityMenu").GetComponent<AbilityMenu>();
        Info = Canvas.transform.Find("Info").GetComponent<Info>();
        DeckMasterInfo = Canvas.transform.Find("DeckMasterInfo").GetComponent<DeckMasterInfo>();
    }

    // Update is called once per frame
    void Update(){
        //Droppedフラグが立っていた場合、チェック処理を行う
        if(BattleField.Dropped && transform.GetSiblingIndex() == DropSibling){
            //マナが足りてなかった場合、カードプレイを拒否し終了する
            if((BattleField.Mana[0] - Card.Cost) < 0){
                BattleField.ResetAbilityFlag();
                return;
            }
            //入れ替えの必要があった場合、入れ替えメニューを表示する
            if(!BattleField.SelectedSummonSpace && BattleField.CurrentMenuStatus == MenuStatus.NoMenu){
                if(Card is UnitCardData){
                    UnitCardData unit = (UnitCardData)Card;
                    for(int i = 0; i < 5; i++){
                        if(BattleField.Unit[0,i].CardID == -1){
                            Filled = false;
                            PlannedNum = i;
                            BattleField.SelectSummonSpace();
                            break;
                        }
                        if(i == 4){
                            Filled = true;
                            ChangeMenu.Instantiate(SelectCategory.Unit, BattleField.HandList[0][DropSibling]);
                            ChangeMenu.gameObject.SetActive(true);
                        }
                    }
                }else if(Card is EnchantCardData){
                    EnchantCardData enchant = (EnchantCardData)Card;
                    for(int i = 0; i < 2; i++){
                        if(BattleField.Enchant[0,i].CardID == -1){
                            Filled = false;
                            PlannedNum = i;
                            BattleField.SelectSummonSpace();
                            break;
                        }
                        if(i == 1){
                            Filled = true;
                            ChangeMenu.Instantiate(SelectCategory.Enchant, BattleField.HandList[0][DropSibling]);
                            ChangeMenu.gameObject.SetActive(true);
                        }
                    }
                }else if(Card is SpellCardData){
                    Filled = false;
                    BattleField.SelectSummonSpace();
                }
            }
            //アビリティ対象選択の必要があった場合、アビリティメニューを表示する
            if(!BattleField.SelectedAbility && BattleField.CurrentMenuStatus == MenuStatus.NoMenu){
                if(Card is UnitCardData){
                    UnitCardData unit = (UnitCardData)Card;
                    for(int i = 0; i < 2; i++){
                        if(unit.Trigger[i] == Trigger.entered && unit.Ability[i].SelfCast){
                            AbilityMenu.SelectSelf(0, PlannedNum);
                            BattleField.SelectAbility();
                        }else if(unit.Trigger[i] == Trigger.entered && unit.Ability[i].Selectable){
                            if(Filled) PlannedNum = ChangeMenu.Selected;
                            AbilityMenu.Instantiate(unit.Ability[i], SelectCategory.Unit, PlannedNum, unit);
                            AbilityMenu.gameObject.SetActive(true);
                            break;
                        }
                        if(i == 1){
                            BattleField.SelectAbility();
                        }
                    }
                }else if(Card is EnchantCardData){
                    EnchantCardData enchant = (EnchantCardData)Card;
                    if(enchant.Trigger == Trigger.entered && enchant.Ability.SelfCast){
                        AbilityMenu.SelectSelf(0, PlannedNum);
                        BattleField.SelectAbility();
                    }else if(enchant.Trigger == Trigger.entered && enchant.Ability.Selectable){
                        if(Filled) PlannedNum = ChangeMenu.Selected;
                        AbilityMenu.Instantiate(enchant.Ability, SelectCategory.Enchant, PlannedNum, enchant);
                        AbilityMenu.gameObject.SetActive(true);
                    }else{
                        BattleField.SelectAbility();
                    }
                }else if(Card is SpellCardData){
                    SpellCardData spell = (SpellCardData)Card;
                    if(spell.Ability.Selectable){
                        AbilityMenu.gameObject.SetActive(true);
                        AbilityMenu.Instantiate(spell.Ability);
                    }else{
                        BattleField.SelectAbility();
                    }
                }
            }
            //もし入れ替えメニューとアビリティメニューがキャンセルされてなかった場合、カードをプレイする
            if(BattleField.SelectedSummonSpace && BattleField.SelectedAbility){
                if(Card is UnitCardData && Filled){
                    BattleField.ToTrushFieldUnitCard(true, ChangeMenu.Selected);
                }else if(Card is EnchantCardData && Filled){
                    BattleField.ToTrushFieldEnchantCard(true, ChangeMenu.Selected);
                }
                BattleField.ResetAbilityFlag();
                if(BattleField.PrayCard(true, DropSibling, AbilityMenu.Selected))
                    Destroy(gameObject);
            }
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData) {
        DeckMasterInfo.gameObject.SetActive(false);
        Info.gameObject.SetActive(true);
        Info.Instantiate(CardNum);
    }
    
    public void OnBeginDrag( PointerEventData eventData ){
        Debug.Log("OnBeginDrag");
        if(BattleField.CurrentMenuStatus == MenuStatus.NoMenu){
            StartSibling = transform.GetSiblingIndex();
            transform.SetParent(Canvas.transform);
            //DroppableFieldの有効化
            DroppableObject.SetActive(true);
        }    
    }

    public void OnDrag( PointerEventData eventData ){
        //Debug.Log("OnDrag");
        if(BattleField.CurrentMenuStatus == MenuStatus.NoMenu)
            transform.position = eventData.position;
    }

    public void OnDrop( PointerEventData eventData ){
        Debug.Log("OnDrop");
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll ( eventData, raycastResults );

        if(BattleField.OperationPlayerCurrentTurn && BattleField.CurrentGamePhase == GamePhase.MainPhase){
            foreach ( var hit in raycastResults )
            {
                // もしカードが自分のメインフェイズで何もメニューが出ておらずDroppableFieldの上なら、Droppedフラグを立てる
                /*if ( hit.gameObject.CompareTag( "DroppableField" ) )
                {
                    if(BattleField.PrayCard(true, StartSibling))
                        Destroy(gameObject);
                }*/
                Dropped = hit.gameObject.CompareTag( "DroppableField" );
                Debug.Log("Dropped:" + Dropped);
                if(Dropped) break;
            }
        }
        //ドラッグ処理が終了したのでDroppableFieldを無効化する
        DroppableObject.SetActive(false);
    }

    public void OnEndDrag( PointerEventData eventData ){
        Debug.Log("OnEndDrag");
        if(BattleField.CurrentMenuStatus == MenuStatus.NoMenu){
            transform.SetParent(Grid.transform);
            //UI上の基本解像度（1920x1080）の横幅と実際の解像度の幅に合わせて入れ替え後の位置を指定する
            int number = (int)(transform.position.x / (float)(Rect.rect.width * (Screen.width / 1920.0f)));
            transform.SetSiblingIndex(number);
            Debug.Log("Screen.width: " + Screen.width + "   StartSibling:" + StartSibling + "   number: " + number + "   transform.position.x: "+ transform.position.x + "   witdh: " + Rect.rect.width * (Screen.width / 1920.0f));
            BattleField.ExchangeHandCard(StartSibling, number);
            DropSibling = transform.GetSiblingIndex();
            if(Dropped) BattleField.Drop();
        }
    }


    public void Instantiate(int cardnum){
        //カードのUIを変更していく
        CardNum = cardnum;
        Card = CardDataBase.Cards[CardNum];
        Cost.text = Card.Cost.ToString();
        CardName.text = Card.CardName;
        Type.text = Card.Types[0];
        if(Card.Types[0] != "" && Card.Types[1] != "") Type.text += "/";
        Type.text += Card.Types[1];
        switch(Card.Elements[0]){
            case Element.Black:
                Color1.color = Color.black;
                break;
            case Element.Blue:
                Color1.color = Color.blue;
                break;
            case Element.Green:
                Color1.color = Color.green;
                break;
            case Element.Red:
                Color1.color = Color.red;
                break;
            case Element.White:
                Color1.color = Color.yellow;
                break;
            case Element.None:
                Color1.color = Color.clear;
                break;
        }
        switch(Card.Elements[1]){
            case Element.Black:
                Color2.color = Color.black;
                break;
            case Element.Blue:
                Color2.color = Color.blue;
                break;
            case Element.Green:
                Color2.color = Color.green;
                break;
            case Element.Red:
                Color2.color = Color.red;
                break;
            case Element.White:
                Color2.color = Color.yellow;
                break;
            case Element.None:
                Color2.color = Color.clear;
                break;
        }
        //カードの種別をサブクラスの型によって判別する
        if(Card is UnitCardData){
            UnitCardData unit = (UnitCardData)Card;
            Debug.Log("UnitCard!");
            CardType.text = "ユニットカード";
            CardText.text = unit.KeyWord.ToString();
            for(int i = 0; i < 2; i++){
                switch(unit.Trigger[i]){
                    case Trigger.Passive:
                        CardText.text += "【常時】\n";
                        break;
                    case Trigger.entered:
                        CardText.text += "【登場時】\n";
                        break;
                    case Trigger.destroyed:
                        CardText.text += "【破壊時】\n";
                        break;
                    case Trigger.TurnStart:
                        CardText.text += "【自分ターン開始時】\n";
                        break;
                    case Trigger.TurnEnd:
                        CardText.text += "【自分ターン終了時】\n";
                        break;
                    case Trigger.Attacked:
                        CardText.text += "【攻撃時】\n";
                        break;
                    case Trigger.Active:
                        CardText.text += "【"+ unit.ActiveManaCost[i] +"マナ";
                        if(unit.ActiveTurnOnce[i]) CardText.text += " ターン1回";
                        if(unit.ActiveDontSummonTurn[i]) CardText.text += " 召喚ターン不可";
                        CardText.text += "】\n";
                        break;
                }
                CardText.text += unit.Ability[i].Text + "\n";
            }
            Power.text = unit.Power.ToString();
        }
        if(Card is SpellCardData){
            SpellCardData spell = (SpellCardData)Card;
            Debug.Log("SpellCard!");
            CardType.text = "スペルカード";
            CardText.text = spell.Ability.Text;
            Power.text = "";
        }
        if(Card is EnchantCardData){
            EnchantCardData enchant = (EnchantCardData)Card;
            Debug.Log("EnchantCard!");
            CardType.text = "エンチャントカード";
            CardText.text = "";
                switch(enchant.Trigger){
                    case Trigger.Passive:
                        CardText.text += "【常時】\n";
                        break;
                    case Trigger.entered:
                        CardText.text += "【登場時】\n";
                        break;
                    case Trigger.destroyed:
                        CardText.text += "【破壊時】\n";
                        break;
                    case Trigger.TurnStart:
                        CardText.text += "【自分ターン開始時】\n";
                        break;
                    case Trigger.TurnEnd:
                        CardText.text += "【自分ターン終了時】\n";
                        break;
                    case Trigger.Active:
                        CardText.text += "【"+ enchant.ActiveManaCost +"マナ";
                        if(enchant.ActiveTurnOnce) CardText.text += " ターン1回";
                        if(enchant.ActiveDontSummonTurn) CardText.text += " 召喚ターン不可";
                        CardText.text += "】\n";
                        break;
                }
                CardText.text += enchant.Ability.Text + "\n";
            Power.text = "";
        }
    }
}
