using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase{
    GameStart,      //ゲーム開始時
    DrawPhase,      //ドローフェイズ
    MainPhase,      //メインフェイズ
    EndPhase,       //エンドフェイズ
    PlayerChange,   //プレイヤー交代
    GameSet         //ゲーム終了時
}

public enum Element{
    None,           //無
    Red,            //赤
    Blue,           //青
    Green,          //緑
    White,          //白
    Black           //黒
}

public enum Trigger {
    none,           //条件無し
    Passive,        //常時効果
    entered,        //登場時
    destroyed,      //退場時
    TurnStart,      //ターン開始時
    TurnEnd,        //ターン終了時
    Attacked,       //攻撃時効果
    Active,         //アクティブ効果
}

public enum MenuStatus{
    NoMenu,
    ChangeMenu,
    AbilityMenu,
    AttackMenu
}

public enum Category{
    Unit,
    Enchant
}

public enum SelectCategory{
    Unit,
    UnitDeckMaster,
    UnitDeckMasterPlayer,
    Enchant
}

public static class BattleField
{
    //カードデータベース
    public static MainCardDataBase MainCardDB{get; private set;} = null;

    //プレイヤー
    public static bool OperationPlayerFirstPlay{get; private set;} = false;  //操作プレイヤーの先攻後攻 trueで先攻
    public static bool OperationPlayerCurrentTurn{get; private set;} = false;    //操作プレイヤーは現在自分のターンか　trueで自分のターン

    public static int TurnCount{get; private set;} = 0;  //ターン数

    public static GamePhase CurrentGamePhase{get; private set;} = GamePhase.GameStart;   //ゲームフェイズ変数

    public static MenuStatus CurrentMenuStatus{get; private set;} = MenuStatus.NoMenu;   //メニュー状態変数

    public static bool Winner{get; private set;} = false; //ゲームの勝者　trueで操作プレイヤーが勝者となる

    public static bool Dropped{get; private set;} = false; //カードがドロップされたか
    public static bool UnitActivedAbility{get; private set;} = false; //ユニットカードのアクティブボタンが押されたか
    public static bool EnchantActivedAbility{get; private set;} = false; //エンチャントカードのアクティブボタンが押されたか
    public static bool DeckMasterActivedAbiliy{get; private set;} = false; //デッキマスターカードのアクティブボタンが押されたか
    public static bool SelectedSummonSpace{get; private set;} = false; //埋まった場合の配置場所は決まったか
    public static bool SelectedAbility{get; private set;} = false; //アビリティの対象選択は決まったか

    //盤面
    //[0]が操作プレイヤー　[1]が相手プレイヤー
    public static int[] MaxMana{get; private set;} = new int[2];     //最大マナ
    public static int[] Mana{get; private set;} = new int[2];        //現在マナ
    public static int[] Hp{get; private set;} = new int[2];          //HP
    public static List<int>[] HandList{get; private set;} = new List<int>[2];    //手札リスト
    public static List<int>[] DeckList{get; private set;} = new List<int>[2];    //山札リスト
    public static List<int>[] TrushList{get; private set;} = new List<int>[2];    //捨て場リスト
    public static List<int>[] ExileList{get; private set;} = new List<int>[2];    //除外場リスト
    //ユニットカード置き場
    public static UnitCardObject[,] Unit{get; private set;} = new UnitCardObject[2,5];
    //エンチャントカード置き場
    public static EnchantCardObject[,] Enchant{get; private set;} = new EnchantCardObject[2,2];
    //デッキマスター置き場
    public static DeckMasterObject[] DeckMaster{get; private set;} = new DeckMasterObject[2];

    //解放条件監視用変数
    //プレイヤー用と相手用で分ける
    //ターン開始時にリセットされるもの
    //このターンに与えたダメージ
    public static int[] ThisTurnDealDamage{get; private set;} = new int[2];
    //これまでのゲームで与えたダメージ
    public static int[] TotalDealDamage{get; private set;} = new int[2];



    //最初の手札の枚数
    public const int FirstHandNumber = 5;

    //BattleControllerのAwakeの時に実行する処理
    public static void BattleFieldAwake(MainCardDataBase database){
        MainCardDB = database;
        Debug.Log("MainCardDataBase Loaded");
    }

    //GameStartフェイズに実行する処理
    public static void GameStart(DeckData playerDeck, DeckData opponentDeck){
        for (int i = 0; i < 2; i++){
            //リスト配列の初期化
            HandList[i] = new List<int>();
            DeckList[i] = new List<int>();
            TrushList[i] = new List<int>();
            ExileList[i] = new List<int>();
            //マナのリセット
            MaxMana[i] = 0;
            Mana[i] = 0;
            //HPのセット
            Hp[i] = 30000;
            //フィールドの初期化
            for (int j = 0; j < 5; j++){
                Unit[i,j] = new UnitCardObject();
            }
            for (int j = 0; j < 2; j++){
                Enchant[i,j] = new EnchantCardObject();
            }
        }
        //デッキデータ読み込み（DeckData内のList<int>をコピーする JSON利用予定）
        DeckList[0] = new List<int>(playerDeck.Deck);
        DeckMaster[0] = new DeckMasterObject(playerDeck);
        DeckList[1] = new List<int>(opponentDeck.Deck);
        DeckMaster[1] = new DeckMasterObject(opponentDeck);

        //シャッフル
        DeckShuffle(true);
        DeckShuffle(false);

        //初手のドローを行う
        //デッキからのドローを初手枚数分行う
        for (int i = 0; i < 2; i++){
            for (int j = 0; j < FirstHandNumber; j++){
                bool player = i == 0;
                DrawCard(player);
            }
        }
        //先攻後攻決定
        bool rand = Random.Range(0, 2) == 0;
        OperationPlayerFirstPlay = rand;            //先攻後攻プレイヤーの決定　操作プレイヤーが先攻だった場合true
        OperationPlayerCurrentTurn = rand;          //操作プレイヤーがターンプレイヤーかどうかを代入　そうだった場合true
        BattleField.ManaIncrease(!OperationPlayerCurrentTurn); //後攻プレイヤーはマナを追加で得る
        //ターン数初期化
        TurnCount = 1;
        //シールカード用のカウントをリセットする
        //ゲーム開始時から数えるカウントをリセットする
        ResetGameStartCount();
        //ターン一回あたりのカウントをリセットする
        ResetThisTurnCount();
    }

    //ゲーム開始時から数えるカウントをリセットする
    public static void ResetGameStartCount(){
        for(int i = 0; i < 2; i++){
            TotalDealDamage[i] = 0;
        }
    }

    //ターン一回あたりのカウントをリセットする
    public static void ResetThisTurnCount(){
        for(int i = 0; i < 2; i++){
            ThisTurnDealDamage[i] = 0;
        }
    }

    //プレイヤー交代
    public static void ChangeTurnPlayer(){
        TurnCount++;
        OperationPlayerCurrentTurn = !OperationPlayerCurrentTurn;
    }

    //ゲーム終了
    //操作プレイヤーか否かで判定する
    public static void GameSet(bool winPlayer){
        CurrentGamePhase = GamePhase.GameSet;
        Winner = winPlayer;
    }

    //フェイズ変更を行う
    public static void PhaseChange(GamePhase phase){
        CurrentGamePhase = phase;
    }

    //メニュー変更を行う
    public static void MenuChange(MenuStatus menu){
        CurrentMenuStatus = menu;
    }

    //カードがドロップされた時、Droppedをオンにし、メニューフラグをリセットする
    public static void Drop(){
        Dropped = true;
        SelectedSummonSpace = false;
        SelectedAbility = false;
    }

    //アクティブアビリティを発動した際、UnitActivedAbilityをtrueにする
    public static void UnitActiveAbility(){
        UnitActivedAbility = true;
    }

    //アクティブアビリティを発動した際、UnitActivedAbilityをtrueにする
    public static void EnchantActiveAbility(){
        EnchantActivedAbility = true;
    }

    public static void DeckMasterActiveAbiliy(){
        DeckMasterActivedAbiliy = true;
    }

    //召喚場所を選択し終えた時、SelectedSummonSpaceをtrueにする
    public static void SelectSummonSpace(){
        SelectedSummonSpace = true;
    }

    //アビリティ対象を選択し終えた時、SelectedAbilityをtrueにする
    public static void SelectAbility(){
        SelectedAbility = true;
    }

    //キャンセルボタンを押した時、メニューを閉じてドロップ状態を解除する
    public static void ResetAbilityFlag(){
        UnitActivedAbility = false;
        EnchantActivedAbility = false;
        DeckMasterActivedAbiliy = false;
        Dropped = false;
        SelectedSummonSpace = false;
        SelectedAbility = false;
    }

    //最大マナを増やす
    //最大マナは11以上にはならない
    public static void ManaIncrease(bool player){
        int setplayer = player ? 0 : 1;
        MaxMana[setplayer]++;
        if(MaxMana[setplayer] > 10) MaxMana[setplayer] = 10;
    }

    //マナを減らす
    //結果をboolの返り値にして、0未満になってしまう場合マナの引き算を行わずfalseにする
    public static bool ManaDecrease(bool player, int manaCost){
        int setplayer = player ? 0 : 1;
        //マナが0未満になってしまう場合、マナの引き算を行わずfalseにする
        if((Mana[setplayer] - manaCost) < 0) return false;
        Mana[setplayer] -= manaCost;
        return true;
    }
    
    //マナを回復する
    public static void ManaRegeneration(bool player){
        int setplayer = player ? 0 : 1;
        Mana[setplayer] = MaxMana[setplayer];
    }

    //山札をシャッフルする
    public static void DeckShuffle(bool player){
        int setplayer = player ? 0 : 1;
        DeckList[setplayer].Shuffle();
        DeckListLog(player);
    }

    //手札にカードを追加する
    public static void AddHandCard(bool player, int cardNum){
        int setplayer = player ? 0 : 1;
        HandList[player ? 0 : 1].Add(cardNum);
        Debug.Log("HandList[" + setplayer + "]["+ (HandList[setplayer].Count - 1) +"]:"+HandList[setplayer][HandList[setplayer].Count - 1]);
    }

    //山札から手札に加える
    public static void DrawCard(bool player){
        int setplayer = player ? 0 : 1;
        Debug.Log("DrawCard!");
        Debug.Log("DeckList[" + setplayer + "]["+ (DeckList[setplayer].Count - 1) +"]:"+DeckList[setplayer][DeckList[setplayer].Count - 1]);
        HandList[setplayer].Add(DeckList[setplayer].Pop());
        Debug.Log("HandList[" + setplayer + "]["+ (HandList[setplayer].Count - 1) +"]:"+HandList[setplayer][HandList[setplayer].Count - 1]);
        Debug.Log("DeckList[" + setplayer + "]["+ (DeckList[setplayer].Count - 1) +"]:"+DeckList[setplayer][DeckList[setplayer].Count - 1]);
    }


    //手札を捨て場に送る
    public static void ToTrushHandCard(bool player, int number){
        int setplayer = player ? 0 : 1;
        int cardnum = HandList[setplayer][number];
        HandList[setplayer].RemoveAt(number);
        TrushList[setplayer].Add(cardnum);
        HandListLog(player);
        TrushListLog(player);
    }

    //手札同士で入れ替える
    public static void ExchangeHandCard(int firstnumber, int secondnumber){
        int cardnum = HandList[0][firstnumber];
        secondnumber = secondnumber >= HandList[0].Count ? HandList[0].Count - 1 : secondnumber;
        HandList[0].RemoveAt(firstnumber);
        HandList[0].Insert(secondnumber, cardnum);
        HandListLog(true);
    }

    //フィールドのユニットカードを捨て場に送る
    public static void ToTrushFieldUnitCard(bool player, int number){
        int setplayer = player ? 0 : 1;
        TrushList[setplayer].Add(Unit[setplayer, number].CardID);
        Unit[setplayer, number].Destroy();
        TrushListLog(player);
    }

    //フィールドのエンチャントカードを捨て場に送る
    public static void ToTrushFieldEnchantCard(bool player, int number){
        int setplayer = player ? 0 : 1;
        TrushList[setplayer].Add(Enchant[setplayer, number].CardID);
        Enchant[setplayer, number].Destory();
        TrushListLog(player);
    }

    //手札からフィールドにカードを出す
    //マナが足りない、フィールドが埋まっているで成功しなかった場合、falseを返す
    //Enteredトリガーが存在する場合、それを実行する
    public static bool PrayCard(bool player, int number, bool[,] select){
        int setplayer = player ? 0 : 1;
        int cardnum = HandList[setplayer][number];
        MainCardData card = MainCardDB.Cards[cardnum];
        //マナが足りない場合、falseにする
        if(!ManaDecrease(player, card.Cost)) return false;
        if(card is UnitCardData){
            UnitCardData unit = (UnitCardData)card;
            for(int i = 0; i < 5; i++){
                if(Unit[setplayer,i].CardID == -1){
                    Unit[setplayer,i].Set(cardnum, unit.Power, unit.KeyWord);
                    HandList[setplayer].RemoveAt(number);
                    //登場時トリガーのアビリティを発動する
                    for(int j = 0; j < 2; j++){
                        if(unit.Trigger[j] == Trigger.entered){
                            Debug.Log("Triggered Enter");
                            unit.Ability[j].Ability(select);
                            break;
                        }
                    }
                    break;
                }
                if(i == 4) return false;
            }
        }else if(card is EnchantCardData){
            EnchantCardData enchant = (EnchantCardData)card;
            for(int i = 0; i < 2; i++){
                if(Enchant[setplayer,i].CardID == -1){
                    Enchant[setplayer,i].SetFieldEnchant(cardnum);
                    HandList[setplayer].RemoveAt(number);  
                    if(enchant.Trigger == Trigger.entered){
                        Debug.Log("Triggered Enter");
                        enchant.Ability.Ability(select);
                        break;
                    }                  
                    break;
                }
                if(i == 1) return false;
            }
        }else if(card is SpellCardData){
            SpellCardData spell = (SpellCardData)card;
            spell.Ability.Ability(select);
            ToTrushHandCard(player, number);
        }
        return true;
    }

    //攻撃の処理
    public static void Attack(int attacker, int defencer){
        int attackPlayer = OperationPlayerCurrentTurn ? 0 : 1;
        int defencePlayer = OperationPlayerCurrentTurn ? 1 : 0;
        //アタッカーのタップによって処理を変更する
        if(attacker < 5){
            //ユニットがアタックする場合
            Unit[attackPlayer, attacker].Tap();
            //疾風を持っていてこのターンまだ未発動だった場合、アンタップして再攻撃可能にする
            if(Unit[attackPlayer, attacker].CurrentKeyWord.DoubleAttack&&!Unit[attackPlayer, attacker].DoubleAttacked){
                Unit[attackPlayer, attacker].EnableDoubleAttack();
            }
            //攻撃時効果
            UnitCardData unit = (UnitCardData)MainCardDB.Cards[Unit[attackPlayer, attacker].CardID];
            for(int i = 0; i < 2; i++){
                if(unit.Trigger[i] == Trigger.Attacked){
                    bool[,] select = new bool[2,7];
                    if(unit.Ability[i].SelfCast) select[attackPlayer, attacker] = true;
                    if(unit.Ability[i].AttackOpponentSelect) select[defencePlayer, defencer] = true;
                    unit.Ability[i].Ability(select);
                }
            }
            if(defencer < 5){
                //ユニットvsユニット
                int atkpower = Unit[attackPlayer,attacker].CurrentPower;
                int defpower = Unit[defencePlayer, defencer].CurrentPower;
                if(Unit[attackPlayer,attacker].CurrentKeyWord.FirstStrike &&
                  !Unit[defencePlayer,defencer].CurrentKeyWord.FirstStrike){
                    //攻撃側だけが先制を持っていた場合、防御側からダメージを受ける
                    Unit[defencePlayer,defencer].Damage(atkpower);
                    if(Unit[defencePlayer,defencer].CurrentPower > 0) Unit[attackPlayer, attacker].Damage(defpower);
                }else if(!Unit[attackPlayer,attacker].CurrentKeyWord.FirstStrike &&
                          Unit[defencePlayer,defencer].CurrentKeyWord.FirstStrike){
                    //守備側だけが先制を持っていた場合、攻撃側からダメージを受ける
                    Unit[attackPlayer,attacker].Damage(defpower);
                    if(Unit[attackPlayer,attacker].CurrentPower > 0) Unit[defencePlayer,defencer].Damage(atkpower);
                }else{
                    //両方共先制だった場合、または両方共先制を持っていなかった場合、両者ダメージを受ける
                    Unit[defencePlayer,defencer].Damage(atkpower);
                    Unit[attackPlayer,attacker].Damage(defpower);
                }
                //貫通を持っていてパワーを上回っていた場合、相手プレイヤーにダメージを与える
                if(Unit[attackPlayer,attacker].CurrentKeyWord.Trumple && (atkpower - defpower) > 0){
                    DealDamage(!OperationPlayerCurrentTurn, atkpower - defpower);
                }
            }else if(defencer == 5){
                //ユニットvsデッキマスター
                int atkpower = Unit[attackPlayer,attacker].CurrentPower;
                int defpower = DeckMaster[defencePlayer].CurrentPower;
                if(Unit[attackPlayer,attacker].CurrentKeyWord.FirstStrike &&
                  !DeckMaster[defencePlayer].CurrentKeyWord.FirstStrike){
                    //攻撃側だけが先制を持っていた場合、防御側からダメージを受ける
                    DeckMaster[defencePlayer].Damage(atkpower);
                    if(DeckMaster[defencePlayer].CurrentPower > 0) Unit[attackPlayer, attacker].Damage(defpower);
                }else if(!Unit[attackPlayer,attacker].CurrentKeyWord.FirstStrike &&
                          DeckMaster[defencePlayer].CurrentKeyWord.FirstStrike){
                    //守備側だけが先制を持っていた場合、攻撃側からダメージを受ける
                    Unit[attackPlayer,attacker].Damage(defpower);
                    if(Unit[attackPlayer,attacker].CurrentPower > 0) DeckMaster[defencePlayer].Damage(atkpower);
                }else{
                    //両方共先制だった場合、または両方共先制を持っていなかった場合、両者ダメージを受ける
                    DeckMaster[defencePlayer].Damage(atkpower);
                    Unit[attackPlayer,attacker].Damage(defpower);
                }
                //貫通を持っていてパワーを上回っていた場合、相手プレイヤーにダメージを与える
                if(Unit[attackPlayer,attacker].CurrentKeyWord.Trumple && (atkpower - defpower) > 0){
                    DealDamage(!OperationPlayerCurrentTurn, atkpower - defpower);
                }
            }else if(defencer == 6){
                //ユニットvsプレイヤー
                DealDamage(!OperationPlayerCurrentTurn, Unit[attackPlayer, attacker].CurrentPower);
            }
        }else if(attacker == 5){
            //デッキマスターがアタックする場合
            DeckMaster[attackPlayer].Tap();
            if(defencer < 5){
                //デッキマスターvsユニット
                int atkpower = DeckMaster[attackPlayer].CurrentPower;
                int defpower = Unit[defencePlayer, defencer].CurrentPower;
                if(DeckMaster[attackPlayer].CurrentKeyWord.FirstStrike &&
                  !Unit[defencePlayer,defencer].CurrentKeyWord.FirstStrike){
                    //攻撃側だけが先制を持っていた場合、防御側からダメージを受ける
                    Unit[defencePlayer,defencer].Damage(atkpower);
                    if(Unit[defencePlayer,defencer].CurrentPower > 0) DeckMaster[attackPlayer].Damage(defpower);
                }else if(!DeckMaster[attackPlayer].CurrentKeyWord.FirstStrike &&
                          Unit[defencePlayer,defencer].CurrentKeyWord.FirstStrike){
                    //守備側だけが先制を持っていた場合、攻撃側からダメージを受ける
                    DeckMaster[attackPlayer].Damage(defpower);
                    if(DeckMaster[attackPlayer].CurrentPower > 0) Unit[defencePlayer,defencer].Damage(atkpower);
                }else{
                    //両方共先制だった場合、または両方共先制を持っていなかった場合、両者ダメージを受ける
                    Unit[defencePlayer,defencer].Damage(atkpower);
                    DeckMaster[attackPlayer].Damage(defpower);
                }
                //貫通を持っていてパワーを上回っていた場合、相手プレイヤーにダメージを与える
                if(DeckMaster[attackPlayer].CurrentKeyWord.Trumple && (atkpower - defpower) > 0){
                    DealDamage(!OperationPlayerCurrentTurn, atkpower - defpower);
                }
            }else if(defencer == 5){
                //デッキマスターvsデッキマスター
                int atkpower = DeckMaster[attackPlayer].CurrentPower;
                int defpower = DeckMaster[defencePlayer].CurrentPower;
                if(DeckMaster[attackPlayer].CurrentKeyWord.FirstStrike &&
                  !DeckMaster[defencePlayer].CurrentKeyWord.FirstStrike){
                    //攻撃側だけが先制を持っていた場合、防御側からダメージを受ける
                    DeckMaster[defencePlayer].Damage(atkpower);
                    if(DeckMaster[defencePlayer].CurrentPower > 0) DeckMaster[attackPlayer].Damage(defpower);
                }else if(!DeckMaster[attackPlayer].CurrentKeyWord.FirstStrike &&
                          DeckMaster[defencePlayer].CurrentKeyWord.FirstStrike){
                    //守備側だけが先制を持っていた場合、攻撃側からダメージを受ける
                    DeckMaster[attackPlayer].Damage(defpower);
                    if(DeckMaster[attackPlayer].CurrentPower > 0) Unit[defencePlayer,defencer].Damage(atkpower);
                }else{
                    //両方共先制だった場合、または両方共先制を持っていなかった場合、両者ダメージを受ける
                    DeckMaster[defencePlayer].Damage(atkpower);
                    DeckMaster[attackPlayer].Damage(defpower);
                }
                //貫通を持っていてパワーを上回っていた場合、相手プレイヤーにダメージを与える
                if(DeckMaster[attackPlayer].CurrentKeyWord.Trumple && (atkpower - defpower) > 0){
                    DealDamage(!OperationPlayerCurrentTurn, atkpower - defpower);
                }
            }else if(defencer == 6){
                //デッキマスターvsプレイヤー
                DealDamage(!OperationPlayerCurrentTurn, DeckMaster[attackPlayer].CurrentPower);
            }
        }
    }

    //プレイヤーに直接ダメージを与える
    public static void DealDamage(bool player, int damage){
        int setplayer = player ? 0 : 1;
        Hp[setplayer] -= damage;
        TotalDealDamage[setplayer] += damage;
        ThisTurnDealDamage[setplayer] += damage;
    }

    //山札のログを出す
    public static void DeckListLog(bool player){
        int setplayer = player ? 0 : 1;
        for (int i = 0; i < DeckList[setplayer].Count; i++) Debug.Log("DeckList["+setplayer+"]["+ i +"]:" + DeckList[setplayer][i]);
    }

    //手札のログを出す
    public static void HandListLog(bool player){
        int setplayer = player ? 0 : 1;
        for (int i = 0; i < HandList[setplayer].Count; i++) Debug.Log("HandList["+setplayer+"]["+ i +"]:" + HandList[setplayer][i]);
    }

    //捨て場のログを出す
    public static void TrushListLog(bool player){
        int setplayer = player ? 0 : 1;
        for (int i = 0; i < TrushList[setplayer].Count; i++) Debug.Log("TrushList["+setplayer+"]["+ i +"]:" + TrushList[setplayer][i]);
    }





}
