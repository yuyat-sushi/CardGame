using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    bool  GameStart = false;            //ゲームスタート処理を行ったかどうか
    bool  DrawPhaseFirst = false;       //プレイヤーはそのターンドローを行ったかどうか
    bool  MainPhaseFirst = false;       //メインフェイズ最初の処理を行ったかどうか
    bool  EndPhaseFirst = false;        //エンドフェイズ最初の処理を行ったかどうか
    bool  PlayerChangeFirst = false;    //プレイヤー交代最初の処理を行ったどうか
    float WaitCount = 0.0f;     //次のシーンに移行するまでのウェイト
    float LiberationWaitCount = 0.0f;    //解放時のウェイト


    [SerializeField]
    MainCardDataBase MainCardDataBase = null;

    [SerializeField]
    DeckData PlayerDeck = null;
    [SerializeField]
    DeckData OpponentDeck = null;


    [SerializeField]
    GameObject PhaseEndButton = null;
    [SerializeField]
    GameObject WinLose = null;
    [SerializeField]
    LevelUp LevelUp = null;

    [SerializeField]
    HandCard HandCardPrefab = null;
    [SerializeField]
    GameObject PlayerHandGrid = null;

    [SerializeField]
    CommandMenu CommandMenu = null;
    [SerializeField]
    EnchantMenu EnchantMenu = null;
    [SerializeField]
    AbilityMenu AbilityMenu = null;
    [SerializeField]
    DeckMasterMenu DeckMasterMenu = null;
    

    void Awake() {
        BattleField.BattleFieldAwake(MainCardDataBase);
        BattleField.GameStart(PlayerDeck, OpponentDeck);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //フェイズごとの処理
        switch(BattleField.CurrentGamePhase){
            case GamePhase.GameStart:
                //ゲームスタート処理
                //最初のフレームだった場合、ゲーム開始処理を行う
                if(!GameStart){
                    Debug.Log("GameStart");
                    //プレイヤーの手札
                    for (int j = 0; j < BattleField.FirstHandNumber; j++){
                        HandCard card = Instantiate(HandCardPrefab) as HandCard;
                        card.transform.SetParent(PlayerHandGrid.transform, false);
                        card.Instantiate(BattleField.HandList[0][j]);
                    }
                    GameStart = true;
                    WaitCount = 2.0f;
                }
                if(WaitCount <= 0.0f){
                    //ドローフェイズに移行
                    BattleField.PhaseChange(GamePhase.DrawPhase);
                }else{
                    WaitCount -= Time.deltaTime;
                    //Debug.Log("WaitCount:" + WaitCount);
                }
                break;
            case GamePhase.DrawPhase:
                //ドローフェイズ
                //最初のフレームだった場合、ターンプレイヤーはカードを一枚ドローする
                if(!DrawPhaseFirst){
                    Debug.Log("Draw!");
                    //ターン一回あたりのカウントをリセットする
                    BattleField.ResetThisTurnCount();
                    //最大マナを回復し、増加させる
                    BattleField.ManaIncrease(BattleField.OperationPlayerCurrentTurn);
                    BattleField.ManaRegeneration(BattleField.OperationPlayerCurrentTurn);
                    //ターンプレイヤーの山札から手札をドローする
                    BattleField.DrawCard(BattleField.OperationPlayerCurrentTurn);
                    //もしもプレイヤーだった場合、手札に追加する
                    if(BattleField.OperationPlayerCurrentTurn){
                        HandCard card = Instantiate(HandCardPrefab) as HandCard;
                        card.transform.SetParent(PlayerHandGrid.transform, false);
                        card.Instantiate(BattleField.HandList[BattleField.OperationPlayerCurrentTurn ? 0 : 1][BattleField.HandList[(BattleField.OperationPlayerCurrentTurn ? 0 : 1)].Count - 1]);
                    }
                    //ターン所有者を求める
                    int player = BattleField.OperationPlayerCurrentTurn ? 0 : 1;
                    for(int i = 0; i < 5; i++){
                        if(BattleField.Unit[player, i].CardID >= 0){
                            //鈍重を持っていない場合、ターン所有者のユニットをアンタップする
                            if(!BattleField.Unit[player, i].CurrentKeyWord.Dullness) BattleField.Unit[player, i].UnTap();
                            //ターン所有者のユニットのターン開始時アビリティを発動する
                            UnitCardData unit = (UnitCardData)MainCardDataBase.Cards[BattleField.Unit[player, i].CardID];
                            for(int j = 0; j < 2; j++){
                                if(unit.Trigger[j] == Trigger.TurnStart){
                                    Debug.Log("Triggered TurnStart");
                                    unit.Ability[j].Ability();
                                    break;
                                }
                            }
                        }
                    }
                    for(int i = 0; i < 2; i++){
                        if(BattleField.Enchant[player, i].CardID >= 0){
                            //ターン所有者のエンチャントのターン開始時アビリティを発動する
                            EnchantCardData enchant = (EnchantCardData)MainCardDataBase.Cards[BattleField.Enchant[player, i].CardID];
                            if(enchant.Trigger == Trigger.TurnStart){
                                Debug.Log("Triggered TurnStart");
                                enchant.Ability.Ability();
                            }
                        }
                    }
                    //ターン所有者のデッキマスターをアンタップする
                    BattleField.DeckMaster[player].UnTap();
                    DrawPhaseFirst = true;
                    WaitCount = 1.0f;
                }
                if(WaitCount <= 0.0f){
                    //メインフェイズに移行
                    DrawPhaseFirst = false;
                    BattleField.PhaseChange(GamePhase.MainPhase);
                }else{
                    WaitCount -= Time.deltaTime;
                    //Debug.Log("WaitCount:" + WaitCount);
                }
                break;
            case GamePhase.MainPhase:
                //メインフェイズ
                if(!MainPhaseFirst){
                    Debug.Log("Main!");
                    if(BattleField.OperationPlayerCurrentTurn){
                        PhaseEndButton.SetActive(true);
                    }
                    MainPhaseFirst = true;
                }
                //もしもプレイヤーではなかった場合、相手はカードを出す動きをする
                //出せるカードが無かった場合
                //行動可能なカードがなければターンエンドする
                if(!BattleField.OperationPlayerCurrentTurn){
                    int prayCardNumber = -1;
                    int prayCardMaxCost = -1;
                    for(int i = 0; i < BattleField.HandList[1].Count; i++){
                        int manaCost = MainCardDataBase.Cards[BattleField.HandList[1][i]].Cost;
                        if(manaCost <= BattleField.Mana[1] && prayCardMaxCost < manaCost){
                            prayCardNumber = i;
                            prayCardMaxCost = manaCost;
                        }
                    }
                    Debug.Log("PrayCard:" + prayCardNumber);
                    if(prayCardNumber != -1){
                        bool[,] selected = new bool[2, 5];
                        if(!BattleField.PrayCard(false, prayCardNumber, selected)){
                            BattleField.PhaseChange(GamePhase.EndPhase);
                        }
                    }else{
                        /*
                        //全てのカードを出し終わった場合、カードを攻撃させる
                        bool UntapFlag = false;
                        //アンタップしているカードを確認するループ
                        //アンタップしていなかった場合UntapFlagはTrueになる
                        for(int i = 0; i < 5; i++){
                            UntapFlag = BattleField.Unit[1, i].TapMode;
                            if(!UntapFlag) break; 
                        }
                        if(UntapFlag) {
                            BattleField.PhaseChange(GamePhase.EndPhase);
                        } else　{
                           //アンタップしているカードを動かす処理 
                        }
                        */
                        BattleField.PhaseChange(GamePhase.EndPhase);
                    }
                }
                break;
            case GamePhase.EndPhase:
                //エンドフェイズ
                if(!EndPhaseFirst){
                    Debug.Log("End!");
                    PhaseEndButton.SetActive(false);
                    MainPhaseFirst = false;
                    EndPhaseFirst = true;
                    for(int playerloop = 0; playerloop < 2; playerloop++){
                        for(int i = 0; i < 5; i++){
                            if(BattleField.Unit[playerloop, i].CardID >= 0){
                                //ユニットカードを回復する
                                BattleField.Unit[playerloop, i].Recovery();
                                //そのターンに出したかどうかのフラグをオフにする
                                BattleField.Unit[playerloop, i].OffSummonThisTurn();
                                //アクティブ使用のフラグをオフにする
                                BattleField.Unit[playerloop, i].OffActiveThisTurn();
                                //ターン所有者のユニットのエンドフェイズ時アビリティを発動する
                                UnitCardData unit = (UnitCardData)MainCardDataBase.Cards[BattleField.Unit[playerloop, i].CardID];
                                for(int j = 0; j < 2; j++){
                                    if((unit.Trigger[j] == Trigger.TurnEnd) && ((playerloop == 0)==BattleField.OperationPlayerCurrentTurn)){
                                        Debug.Log("Triggered TurnEnd");
                                        if(unit.Ability[j].SelfCast){
                                            bool[,] self = new bool[2,7];
                                            self[playerloop,i] = true;
                                            unit.Ability[j].Ability(self);
                                        }else{
                                            unit.Ability[j].Ability();
                                        }
                                        break;
                                    }
                                }
                                //キーワード「脆弱」がついていた場合、破壊する
                                if(BattleField.Unit[playerloop,i].CurrentKeyWord.Vulnerable){
                                    BattleField.ToTrushFieldUnitCard(playerloop == 0, i);
                                }
                            }
                        }
                        for(int j = 0; j < 2; j++){
                            if(BattleField.Enchant[playerloop, j].CardID >= 0){
                                //ターン所有者のエンチャントのエンドフェイズ時アビリティを発動する
                                EnchantCardData enchant = (EnchantCardData)MainCardDataBase.Cards[BattleField.Enchant[playerloop, j].CardID];
                                if(enchant.Trigger == Trigger.TurnEnd && ((playerloop == 0)==BattleField.OperationPlayerCurrentTurn)){
                                    Debug.Log("Triggered TurnEnd");
                                    if(enchant.Ability.SelfCast){
                                        bool[,] self = new bool[2,2];
                                        self[playerloop,j] = true;
                                        enchant.Ability.Ability(self);
                                    }else{
                                        enchant.Ability.Ability();
                                    }
                                    enchant.Ability.Ability();
                                }
                            }
                            //そのターンに出したかどうかのフラグをオフにする
                            BattleField.Enchant[playerloop, j].OffSummonThisTurn();
                            //アクティブ使用のフラグをオフにする
                            BattleField.Enchant[playerloop, j].OffActiveThisTurn();
                        }
                        //デッキマスターのスタンカウントを減らす
                        BattleField.DeckMaster[playerloop].DownStanCount();
                        //デッキマスターを回復する
                        BattleField.DeckMaster[playerloop].Recovery();
                        //アクティブ使用のフラグをオフにする
                        BattleField.DeckMaster[playerloop].OffActiveThisTurn();
                    }
                    WaitCount = 1.0f;
                }
                if(WaitCount <= 0.0f){
                    //プレイヤーチェンジフェイズに移行
                    EndPhaseFirst = false;
                    BattleField.PhaseChange(GamePhase.PlayerChange);
                }else{
                    WaitCount -= Time.deltaTime;
                    //Debug.Log("WaitCount:" + WaitCount);
                }
                break;
            case GamePhase.PlayerChange:
                //プレイヤーチェンジ
                if(!PlayerChangeFirst){
                    Debug.Log("PlayerChange!");
                    BattleField.ChangeTurnPlayer();
                    PlayerChangeFirst = true;
                    WaitCount = 0.5f;
                }
                if(WaitCount <= 0.0f){
                    //ドローフェイズに移行
                    PlayerChangeFirst = false;
                    BattleField.PhaseChange(GamePhase.DrawPhase);
                }else{
                    WaitCount -= Time.deltaTime;
                    //Debug.Log("WaitCount:" + WaitCount);
                }
                break;
            case GamePhase.GameSet:
                //ゲーム終了
                PhaseEndButton.SetActive(false);
                WinLose.SetActive(true);
                break;
        }
        //フェイズに無関係な処理
        //常時効果の実行
        for(int player = 0; player < 2; player++){
            for(int unit = 0; unit < 5; unit++){
                if(BattleField.Unit[player, unit].CardID != -1){
                    UnitCardData card = (UnitCardData)MainCardDataBase.Cards[BattleField.Unit[player, unit].CardID];
                    for(int i = 0; i < 2; i++){
                        if(card.Ability[i].Selectable){
                            //Selectableだった場合、エラーを表示しアビリティを実行しない
                            Debug.LogError("Unit["+player+","+unit+"] Ability"+i+" is Selectable!");
                        }else if((card.Trigger[i] == Trigger.Passive)&&card.Ability[i].SelfCast){
                            //自分自身を指定してアビリティ実行
                            bool[,] selfselect = new bool[2,7];
                            selfselect[player,unit] = true;
                            card.Ability[i].Ability(selfselect);
                        }else if(card.Trigger[i] == Trigger.Passive){
                            //何も指定せずアビリティ実行
                            card.Ability[i].Ability();
                        }
                    }
                }
            }
            for(int enchant = 0; enchant < 2; enchant++){
                if(BattleField.Enchant[player, enchant].CardID != -1){
                    EnchantCardData card = (EnchantCardData)MainCardDataBase.Cards[BattleField.Enchant[player, enchant].CardID];
                    if(card.Trigger == Trigger.Passive&&!card.Ability.Selectable){
                        //自分自身を指定してアビリティ実行
                        bool[,] selfselect = new bool[2,2];
                        selfselect[player,enchant] = true;
                        card.Ability.Ability(selfselect);
                    }else if(card.Ability.Selectable){
                        //Selectableだった場合、エラーを表示しアビリティを実行しない
                        Debug.LogError("Unit["+player+","+enchant+"] Ability is Selectable!");
                    }
                }
            }
        }
        //ユニットがアクティブアビリティを使用した時、アクティブアビリティを発動する
        if(BattleField.UnitActivedAbility && BattleField.SelectedAbility){
            Debug.Log("UnitActiveAbility");
            BattleField.ResetAbilityFlag();
            UnitCardData card = (UnitCardData)MainCardDataBase.Cards[BattleField.Unit[0, CommandMenu.SelectNumber].CardID];
            BattleField.Unit[0,CommandMenu.SelectNumber].OnActiveThisTurn(CommandMenu.SelectAbility);
            BattleField.ManaDecrease(true, card.ActiveManaCost[CommandMenu.SelectAbility]);
            card.Ability[CommandMenu.SelectAbility].Ability(AbilityMenu.Selected);
        }
        //エンチャントがアクティブアビリティを使用した時、アクティブアビリティを発動する
        if(BattleField.EnchantActivedAbility && BattleField.SelectedAbility){
            Debug.Log("EnchantActiveAbility");
            BattleField.ResetAbilityFlag();
            EnchantCardData card = (EnchantCardData)MainCardDataBase.Cards[BattleField.Enchant[0, EnchantMenu.SelectNumber].CardID];
            BattleField.Enchant[0,EnchantMenu.SelectNumber].OnActiveThisTurn();
            BattleField.ManaDecrease(true, card.ActiveManaCost);
            card.Ability.Ability(AbilityMenu.Selected);
        }
        //デッキマスターがアクティブアビリティを使用した時、アクティブアビリティを発動する
        if(BattleField.DeckMasterActivedAbiliy && BattleField.SelectedAbility){
            Debug.Log("DeckMasterActiveAbility");
            BattleField.ResetAbilityFlag();
            BattleField.DeckMaster[0].OnActiveThisTurn(DeckMasterMenu.SelectAbility);
            BattleField.DeckMaster[0].SealManaDecrease(BattleField.DeckMaster[0].AbilityCard[DeckMasterMenu.SelectAbility].SealCost);
            BattleField.DeckMaster[0].AbilityCard[DeckMasterMenu.SelectAbility].Ability.Ability(AbilityMenu.Selected);
        }
        //シールカードの条件監視処理を行う
        if(LiberationWaitCount <= 0){
            //各デッキマスターの解放レベルに応じた解放条件を監視し、条件を達成した場合、開放処理を行う
            if(!BattleField.DeckMaster[0].IsLiberation && BattleField.DeckMaster[0].SealCard[BattleField.DeckMaster[0].LiberationLevel].ConditionCheck(true)){
                Debug.Log("Player Level Up!");
                LiberationWaitCount = 2;
                BattleField.DeckMaster[0].LiberationLevelUp();
                Debug.Log("LiberationLevel:" + BattleField.DeckMaster[0].LiberationLevel);
                LevelUp.Instantiate(0);
                LevelUp.gameObject.SetActive(true);
            }else if(!BattleField.DeckMaster[1].IsLiberation && BattleField.DeckMaster[1].SealCard[BattleField.DeckMaster[1].LiberationLevel].ConditionCheck(false)){
                Debug.Log("Opponent Level Up!");
                LiberationWaitCount = 2;
                BattleField.DeckMaster[1].LiberationLevelUp();
                Debug.Log("LiberationLevel:" + BattleField.DeckMaster[0].LiberationLevel);
                LevelUp.Instantiate(1);
                LevelUp.gameObject.SetActive(true);
            }
        }else{
            LiberationWaitCount -= Time.deltaTime;
            if(LiberationWaitCount <= 0){
                LevelUp.gameObject.SetActive(false);
            }
        }
        //状況起因処理の実行
        //カードプレイによるアビリティ発動よりも早く発動する
        //何らかの要員でどちらかのHPが0になった場合ゲームが終了する
        if(BattleField.Hp[0] <= 0){
            BattleField.GameSet(!BattleField.OperationPlayerCurrentTurn);
        }else if(BattleField.Hp[1] <= 0){
            BattleField.GameSet(BattleField.OperationPlayerCurrentTurn);
        }
        //ユニットのパワーが0になった時、捨て場に送られる
        for(int player = 0; player < 2; player++){
            for(int unit = 0; unit < 5; unit++){
                if((BattleField.Unit[player, unit].CardID != -1)&&(BattleField.Unit[player, unit].CurrentPower <= 0)){
                    bool playerflag = player == 0;
                    BattleField.ToTrushFieldUnitCard(playerflag, unit);
                }
            }
        }
        //スタンカウントが0の時にデッキマスターのパワーが0になった時、スタンカウントが2付与される
        for(int player = 0; player < 2; player++){
            if((BattleField.DeckMaster[player].CurrentPower <= 0)&&(BattleField.DeckMaster[player].StanCount == 0)){
                BattleField.DeckMaster[player].Stan();
            }
        }
    }
}