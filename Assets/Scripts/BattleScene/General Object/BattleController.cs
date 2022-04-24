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
                    //最初のターンでなかった場合、ターンプレイヤーの山札から手札をドローする
                    if(BattleField.TurnCount != 1) {
                        BattleField.DrawCard(BattleField.OperationPlayerCurrentTurn);
                        //もしもプレイヤーだった場合、手札オブジェクトを追加する
                        if(BattleField.OperationPlayerCurrentTurn){
                            HandCard card = Instantiate(HandCardPrefab) as HandCard;
                            card.transform.SetParent(PlayerHandGrid.transform, false);
                            card.Instantiate(BattleField.HandList[BattleField.OperationPlayerCurrentTurn ? 0 : 1][BattleField.HandList[(BattleField.OperationPlayerCurrentTurn ? 0 : 1)].Count - 1]);
                        }
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
                                    unit.Ability[j].Ability(player);
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
                                enchant.Ability.Ability(player);
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
                    EnemyAI();
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
                                            unit.Ability[j].Ability(self, playerloop);
                                        }else{
                                            unit.Ability[j].Ability(playerloop);
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
                                        enchant.Ability.Ability(self, playerloop);
                                    }else{
                                        enchant.Ability.Ability(playerloop);
                                    }
                                }
                            }
                            //そのターンに出したかどうかのフラグをオフにする
                            BattleField.Enchant[playerloop, j].OffSummonThisTurn();
                            //アクティブ使用のフラグをオフにする
                            BattleField.Enchant[playerloop, j].OffActiveThisTurn();
                        }
                        //ターンプレイヤーのデッキマスターの開放判定を行う
                        if((playerloop == 0)==BattleField.OperationPlayerCurrentTurn){
                            if(BattleField.DeckMaster[playerloop].Libelation()){
                                LevelUp.gameObject.SetActive(true);
                                LevelUp.Instantiate(playerloop);
                            }
                        }
                        //お互いのデッキマスターのスタンカウントを減らす
                        BattleField.DeckMaster[playerloop].DownStanCount();
                        //お互いのデッキマスターを回復する
                        BattleField.DeckMaster[playerloop].Recovery();
                        //アクティブ使用のフラグをオフにする
                        BattleField.DeckMaster[playerloop].OffActiveThisTurn();
                    }
                    WaitCount = 1.5f;
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
                    //常時効果適用前にパワーのセットを行う
                    BattleField.Unit[player,unit].PowerSet(BattleField.Unit[player,unit].BasePower - BattleField.Unit[player,unit].RecievedDamage + BattleField.Unit[player,unit].RecievedBuff);
                    UnitCardData card = (UnitCardData)MainCardDataBase.Cards[BattleField.Unit[player, unit].CardID];
                    for(int i = 0; i < 2; i++){
                        if(card.Ability[i].Selectable){
                            //Selectableだった場合、エラーを表示しアビリティを実行しない
                            Debug.LogError("Unit["+player+","+unit+"] Ability"+i+" is Selectable!");
                        }else if((card.Trigger[i] == Trigger.Passive)&&card.Ability[i].SelfCast){
                            //自分自身を指定してアビリティ実行
                            bool[,] selfselect = new bool[2,7];
                            selfselect[player,unit] = true;
                            card.Ability[i].Ability(selfselect, player);
                        }else if(card.Trigger[i] == Trigger.Passive){
                            //何も指定せずアビリティ実行
                            card.Ability[i].Ability(player);
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
                        card.Ability.Ability(selfselect, player);
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
            card.Ability[CommandMenu.SelectAbility].Ability(AbilityMenu.Selected, 0);
        }
        //エンチャントがアクティブアビリティを使用した時、アクティブアビリティを発動する
        if(BattleField.EnchantActivedAbility && BattleField.SelectedAbility){
            Debug.Log("EnchantActiveAbility");
            BattleField.ResetAbilityFlag();
            EnchantCardData card = (EnchantCardData)MainCardDataBase.Cards[BattleField.Enchant[0, EnchantMenu.SelectNumber].CardID];
            BattleField.Enchant[0,EnchantMenu.SelectNumber].OnActiveThisTurn();
            BattleField.ManaDecrease(true, card.ActiveManaCost);
            card.Ability.Ability(AbilityMenu.Selected, 0);
        }
        //デッキマスターがアクティブアビリティを使用した時、アクティブアビリティを発動する
        if(BattleField.DeckMasterActivedAbiliy && BattleField.SelectedAbility){
            Debug.Log("DeckMasterActiveAbility");
            BattleField.ResetAbilityFlag();
            BattleField.DeckMaster[0].OnActiveThisTurn(DeckMasterMenu.SelectAbility);
            BattleField.DeckMaster[0].SealManaDecrease(BattleField.DeckMaster[0].AbilityCard[DeckMasterMenu.SelectAbility].SealCost);
            BattleField.DeckMaster[0].AbilityCard[DeckMasterMenu.SelectAbility].Ability.Ability(AbilityMenu.Selected, 0);
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

    void EnemyAI(){
        //新AI
        //判断基準を求めるための変数を用意する
        int playerHighestPower = 0;           //プレイヤーの盤面で最もパワーの高いユニットのパワー
        int playerHighestPowerUnitNum = -1;   //プレイヤーの盤面で最もパワーの高いユニットの番号
        bool playerHighestPowerFS = false;    //プレイヤーの盤面で最もパワーの高いユニットの先制の有無
        for (int i = 0; i < 5; i++){
            if(BattleField.Unit[0,i].CardID != -1 && playerHighestPower < BattleField.Unit[0,i].CurrentPower){
                playerHighestPower = BattleField.Unit[0,i].CurrentPower;
                playerHighestPowerUnitNum = i;
                playerHighestPowerFS = BattleField.Unit[0,i].CurrentKeyWord.FirstStrike;
            }
        }
        int opponentHighestPower = 0;         //相手の盤面で攻撃可能な最もパワーの高いユニットのパワー
        int opponentHighestPowerUnitNum = -1; //相手の盤面で攻撃可能な最もパワーの高いユニットの番号
        bool opponentHighestPowerFS = false;   //プレイヤーの盤面で最もパワーの高いユニットの先制の有無
        for (int i = 0; i < 5; i++){
            if(BattleField.Unit[1,i].CardID != -1 && !BattleField.Unit[1,i].TapMode && opponentHighestPower < BattleField.Unit[1,i].CurrentPower){
                opponentHighestPower = BattleField.Unit[1,i].CurrentPower;
                opponentHighestPowerUnitNum = i;
                opponentHighestPowerFS = BattleField.Unit[1,i].CurrentKeyWord.FirstStrike;
            }
        }
        int opponentHandHighestPower = 0;         //相手の手札で最もパワーの高いユニットのパワー
        int opponentHandHighestPowerHandNum = -1; //相手の手札で最もパワーの高いユニットの番号
        bool opponentHandHighestPowerFS = false;    //プレイヤーの盤面で最もパワーの高いユニットの先制の有無
        bool opponentHandHeatBlade = false;       //相手の手札に15(ヒートブレード)は存在するか
        int opponentHandHeatBladeHandNum = -1;    //相手の手札の15(ヒートブレード)の位置
        for (int i = 0; i < BattleField.HandList[1].Count; i++){
            if(MainCardDataBase.Cards[BattleField.HandList[1][i]] is UnitCardData){
                UnitCardData Unit = (UnitCardData)MainCardDataBase.Cards[BattleField.HandList[1][i]];
                if(BattleField.Mana[1] >= Unit.Cost && opponentHighestPower < Unit.Power){
                    opponentHandHighestPower = Unit.Power;
                    opponentHandHighestPowerHandNum = i;
                    opponentHandHighestPowerFS = Unit.KeyWord.FirstStrike;
                }
            }else if(BattleField.HandList[1][i] == 15){
                opponentHandHeatBlade = true;
                opponentHandHeatBladeHandNum = i;
            }
        }
        Debug.Log("playerHighestPower:"+playerHighestPower+" playerHighestPowerUnitNum:"+playerHighestPowerUnitNum);
        Debug.Log("opponentHighestPower:"+opponentHighestPower+" opponentHighestPowerUnitNum:"+opponentHighestPowerUnitNum);
        Debug.Log("opponentHandHighestPower:"+opponentHandHighestPower+" opponentHandHighestPowerHandNum:"+opponentHandHighestPowerHandNum);
        bool opponentcanattack = false;      //相手の盤面で攻撃可能なユニットは存在するか
        for(int i = 0; i < 5; i++){
            if(BattleField.Unit[1,i].CardID != -1 && !BattleField.Unit[1,i].TapMode) opponentcanattack = true;
        }
        //盤面の優先順位によって分岐させる
        if(BattleField.DeckMaster[1].IsLiberation && BattleField.DeckMaster[1].StanCount == 0 && !BattleField.DeckMaster[1].ActiveThisTurn[0]){
            //デッキマスターが開放されており、ターン1回の効果を使用してなかった場合、
            bool[,] selected = new bool[2, 7];
            BattleField.DeckMaster[1].OnActiveThisTurn(0);
            BattleField.DeckMaster[1].SealManaDecrease(BattleField.DeckMaster[1].AbilityCard[0].SealCost);
            BattleField.DeckMaster[1].AbilityCard[0].Ability.Ability(selected, 1);
        }
        if(BattleField.DeckMaster[1].IsLiberation && BattleField.DeckMaster[1].StanCount == 0 && BattleField.DeckMaster[1].SealMana >= 1 && playerHighestPowerUnitNum != -1 && 5000 >= playerHighestPower && playerHighestPower >= 4000){
            //デッキマスターが開放されており、シールコストが不足しておらず、パワーが4000~5000のユニットが存在した場合アクティブ効果を使用する
            bool[,] selected = new bool[2, 7];
            selected[0,playerHighestPowerUnitNum] = true;
            BattleField.DeckMaster[1].OnActiveThisTurn(1);
            BattleField.DeckMaster[1].SealManaDecrease(BattleField.DeckMaster[1].AbilityCard[1].SealCost);
            BattleField.DeckMaster[1].AbilityCard[1].Ability.Ability(selected, 1);
        }
        if(opponentHighestPowerUnitNum != -1 && playerHighestPowerUnitNum != -1 && opponentHighestPower > playerHighestPower){
            //もしもお互いの場にユニットが存在し、相手ユニットのパワーがプレイヤーのユニットの最大パワーよりも高かった場合、攻撃を行わせる
            BattleField.Attack(opponentHighestPowerUnitNum,playerHighestPowerUnitNum);
        }else if(opponentHandHighestPowerHandNum != -1 && playerHighestPowerUnitNum != -1 && opponentHandHighestPower > playerHighestPower){
            //もし手札にあるカードのパワーが相手のユニットのパワーよりも高かった場合、そのカードを出す
            bool[,] selected = new bool[2, 7];
            if(!BattleField.PrayCard(false, opponentHandHighestPowerHandNum, selected)){
                //なんらかの理由で出せなかった場合、エンドフェイズに移行する
                BattleField.PhaseChange(GamePhase.EndPhase);
            }
        }else if(opponentHandHeatBlade && playerHighestPowerUnitNum != -1 &&  playerHighestPower <= 3000 && BattleField.Mana[1] >= MainCardDataBase.Cards[15].Cost){
            //カード番号15（ヒートブレード）が手札に存在し、撃つ対象も存在する場合、そのカードを使用する
            //出すカードが選択されていた場合、カードを出す
            bool[,] selected = new bool[2, 7];
            //パワー3000以下のユニットを指定する
            selected[0,playerHighestPowerUnitNum] = true;
            if(!BattleField.PrayCard(false, opponentHandHighestPowerHandNum, selected)){
                //なんらかの理由で出せなかった場合、エンドフェイズに移行する
                BattleField.PhaseChange(GamePhase.EndPhase);
            }
        }else if(BattleField.DeckMaster[1].IsLiberation && BattleField.DeckMaster[1].StanCount == 0 && playerHighestPowerUnitNum != -1 &&  !BattleField.DeckMaster[1].TapMode && BattleField.DeckMaster[1].CurrentPower > playerHighestPower){
            //もし相手フィールドにユニットが存在し、デッキマスターが攻撃可能だった場合デッキマスターとプレイヤーが攻撃を行う
            BattleField.Attack(5,playerHighestPowerUnitNum);
        }else if(opponentHighestPowerUnitNum != -1 && playerHighestPowerUnitNum != -1 && opponentHighestPower == playerHighestPower &&(playerHighestPowerFS == opponentHandHighestPowerFS||opponentHandHighestPowerFS)){
            //もしもお互いの場にユニットが存在し、相手ユニットのパワーがプレイヤーのユニットの最大パワーと同じで相手だけが先制持ち以外だった場合、攻撃を行わせる
            BattleField.Attack(opponentHighestPowerUnitNum,playerHighestPowerUnitNum);
        }else if(opponentHandHighestPowerHandNum != -1 && playerHighestPowerUnitNum != -1 && opponentHandHighestPower == playerHighestPower &&(playerHighestPowerFS == opponentHandHighestPowerFS||opponentHandHighestPowerFS)){
            //もし手札にあるカードのパワーが相手のユニットのパワーと同じで相手だけが先制持ち以外だった場合、カードを出す
            bool[,] selected = new bool[2, 7];
            if(!BattleField.PrayCard(false, opponentHandHighestPowerHandNum, selected)){
                //なんらかの理由で出せなかった場合、エンドフェイズに移行する
                BattleField.PhaseChange(GamePhase.EndPhase);
            }
        }else if(BattleField.Mana[1] > 0){
            //マナが残っていた場合、残ったマナで一番コストの高いカードを出す
            int prayCardNumber = -1;
            int prayCardMaxCost = -1;
            //出すカードを選択する
            //マナカーブ通りのカードを使う
            for(int i = 0; i < BattleField.HandList[1].Count; i++){
                //もし15番(ヒートブレード)だった場合、使用せずに温存する
                if(BattleField.HandList[1][i] == 15) continue;
                //マナコストの比較を行う
                int manaCost = MainCardDataBase.Cards[BattleField.HandList[1][i]].Cost;
                if(manaCost <= BattleField.Mana[1] && prayCardMaxCost < manaCost){
                    prayCardNumber = i;
                    prayCardMaxCost = manaCost;
                }
            }
            Debug.Log("PrayCard:" + prayCardNumber);
            if(prayCardNumber != -1){
                //出すカードが選択されている場合、カードを出す
                bool[,] selected = new bool[2, 7];
                //カードごとのselectedの指定
                //カード番号8（ファイアーボール）だった場合、プレイヤーを指定する
                if(BattleField.HandList[1][prayCardNumber] == 8) selected[0,6] = true;
                if(!BattleField.PrayCard(false, prayCardNumber, selected)){
                    //なんらかの理由で出せなかった場合、エンドフェイズに移行する
                    BattleField.PhaseChange(GamePhase.EndPhase);
                }
            }else{
                //出せるカードが無かった場合、マナを-999してマナ残量を0にする(暫定)
                BattleField.ManaDecrease(false, 999);
            }
        }else if(opponentcanattack){
            //プレイヤーに攻撃可能なユニットが存在する場合、プレイヤーに攻撃する
            for(int unit = 0; unit < 5; unit++){
                if(BattleField.Unit[1,unit].CardID != -1 && !BattleField.Unit[1,unit].TapMode){
                    bool canattack = false;
                    if(BattleField.Unit[1,unit].CurrentKeyWord.Passing) {
                        //キーワード通過を持っていた場合、無条件でプレイヤーにアタックする
                        canattack = true;
                    }else{
                        //盤面に誰も残っていない場合
                        int counter = 0;
                        for(int punit = 0; punit < 5; punit++){
                            if(BattleField.Unit[0,punit].CardID != -1) counter++;
                        }
                        if(counter == 0) canattack = true;
                    }
                    if(canattack){
                        //攻撃可能だった場合、攻撃を行う
                        BattleField.Attack(unit,6);
                    }else{
                        //攻撃できなかった場合、攻撃可能ユニットをタップさせる(暫定)
                        BattleField.Unit[1,unit].Tap();
                    }
                }
            }
        }else if(BattleField.DeckMaster[1].IsLiberation && BattleField.DeckMaster[1].StanCount == 0 && !BattleField.DeckMaster[1].TapMode){
            //デッキマスターがプレイヤーに攻撃可能だった場合、プレイヤーに攻撃する
            bool canattack = false;
            if(BattleField.DeckMaster[1].CurrentKeyWord.Passing) {
                //キーワード通過を持っていた場合、無条件でプレイヤーにアタックする
                canattack = true;
            }else{
                //盤面に誰も残っていない場合
                int counter = 0;
                for(int punit = 0; punit < 5; punit++){
                    if(BattleField.Unit[0,punit].CardID != -1) counter++;
                }
                if(counter == 0) canattack = true;
            }
            if(canattack){
                //攻撃可能だった場合、攻撃を行う
                BattleField.Attack(5,6);
            }else{
                //攻撃できなかった場合、攻撃可能ユニットをタップさせる(暫定)
                BattleField.DeckMaster[1].Tap();
            }
        }else{
            //何も出来なかった場合、ターンエンドを選択する
            BattleField.PhaseChange(GamePhase.EndPhase);
        }
        /* 旧AI
        int prayCardNumber = -1;
        int prayCardMaxCost = -1;
        //出すカードを選択する
        //マナカーブ通りのカードを使う
        for(int i = 0; i < BattleField.HandList[1].Count; i++){
            int manaCost = MainCardDataBase.Cards[BattleField.HandList[1][i]].Cost;
            if(manaCost <= BattleField.Mana[1] && prayCardMaxCost < manaCost){
                prayCardNumber = i;
                prayCardMaxCost = manaCost;
            }
        }
        //出すカードが選択されていた場合、カードを出す
        Debug.Log("PrayCard:" + prayCardNumber);
        if(prayCardNumber != -1){
            bool[,] selected = new bool[2, 5];
            if(!BattleField.PrayCard(false, prayCardNumber, selected)){
                BattleField.PhaseChange(GamePhase.EndPhase);
            }
        }else{
            BattleField.PhaseChange(GamePhase.EndPhase);
        }
        */
    }
}