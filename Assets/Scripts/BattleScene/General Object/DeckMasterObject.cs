using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckMasterObject
{
        //名前
        public String Name{get; private set;}
        //色
        public Element[] DeckMasterColor{get; private set;} = new Element[2];
        //タイプ
        public String[] Types{get; private set;} = new String[2];
        //シールランク
        public int SealRank{get; private set;}
        //シールカード
        public SealCardData[] SealCard{get; private set;} = new SealCardData[4];
        //開放状態か
        public bool IsLiberation{get; private set;}
        //解放レベル
        public int LiberationLevel{get; private set;}
        //シールマナ
        public int SealMana{get; private set;}
        //基礎パワー
        public int BasePower{get; private set;}
        //現在パワー
        public int CurrentPower{get; private set;}

        //元のキーワード
        public KeyWord BaseKeyWord{get; private set;}
        //そのターン中有効なキーワード(こちらを優先して参照する)
        public KeyWord CurrentKeyWord{get; private set;}
        //相手ターンに有効になるキーワード
        public KeyWord OpponentTurnKeyWord{get; private set;}

        //現在受けているダメージ
        public int RecievedDamage{get; private set;}
        //現在受けているパワーバフ
        public int RecievedBuff{get; private set;}

        //タップ状態かどうか
        public bool TapMode{get; private set;}
        //疾風を発動したか
        public bool DoubleAttacked{get; private set;}
        //
        public bool[] ActiveThisTurn{get; private set;} = new bool[2];

        //スタン状態カウント
        //ダメージを受けてCurrentPowerが0以下になるとスタン状態となり、スタンカウントが+2される
        //お互いのエンドフェイズに-1し、0になった場合復活する
        public int StanCount{get; private set;}
        //アビリティカード
        public AbilityCardData[] AbilityCard{get; private set;} = new AbilityCardData[2];

        public DeckMasterObject(DeckData deck){
                Name = deck.MasterName;
                for(int i = 0; i < 2; i++){
                        DeckMasterColor[i] = deck.Color[i];
                }
                SealRank = deck.SealRank;
                Array.Copy(deck.SealCard, SealCard, SealRank);
                Types[0] = deck.Types[0];
                Types[1] = deck.Types[1];
                IsLiberation = false;
                LiberationLevel = 0;
                SealMana = 0;
                BasePower = SealRank * 2000 + 1000;
                if(DeckMasterColor[0]==Element.None||DeckMasterColor[1]==Element.None){
                        BasePower += 1000;
                }
                CurrentPower = BasePower;
                RecievedDamage = 0;  
                RecievedBuff = 0;
                BaseKeyWord = new KeyWord(deck.KeyWord);
                CurrentKeyWord = new KeyWord(BaseKeyWord);
                OpponentTurnKeyWord = new KeyWord();
                TapMode = false;
                DoubleAttacked = false;
                for(int i = 0; i < 2; i++){
                        AbilityCard[i] = deck.AbilityCard[i];
                }
        }

        public void LiberationLevelUp(){
                LiberationLevel++;
                if(LiberationLevel == SealRank){
                        IsLiberation = true;
                        SealMana = SealRank;
                }
        }

        //SealManaDecrease
        //シールマナを減少させる
        public void SealManaDecrease(int sealCost){
                SealMana -= sealCost;
        }

        //Damage
        //パワーを指定した数減らし、受けたダメージを記録する
        public void Damage(int damage){
                CurrentPower -= damage;
                RecievedDamage += damage;
        }

        //PowerUpDown
        //パワーを増減させ、増減値を記録する
        public void PowerUpDown(int power){
                CurrentPower += power;
                RecievedBuff += power;
        }

        //パワーを特定の値にする
        //UpDownと違い、記録されない
        public void PowerSet(int power){
                CurrentPower = power;
        }

        //PowerUpDown
        //パワーを増減させ、増減値を記録する
        public void BasePowerUpDown(int power){
                BasePower += power;
                PowerUpDown(power);
        }

        //パワーを特定の値にする
        //UpDownと違い、記録されない
        public void BasePowerSet(int power){
                BasePower = power;
                PowerSet(power);
        }

        //Recovery
        //現在パワーを元のパワーに戻し、受けたダメージとバフとキーワードをリセットする
        //また、相手ターンに有効になるキーワードをこのタイミングで付与し、その後リセットする
        public void Recovery(){
                CurrentPower = BasePower;
                CurrentKeyWord = new KeyWord(BaseKeyWord);
                CurrentKeyWord.AddKeyword(OpponentTurnKeyWord);
                OpponentTurnKeyWord = new KeyWord();
                DoubleAttacked = false;
                RecievedDamage = 0;
                RecievedBuff = 0;
        }

        public void Tap(){
                TapMode = true;
        }

        public void UnTap(){
                TapMode = false;
        }

        public void EnableDoubleAttack(){
                TapMode = false;
                DoubleAttacked = true;
        }

        public void OnActiveThisTurn(int abilityNum){
                ActiveThisTurn[abilityNum] = true;
        }

        public void OffActiveThisTurn(){
                for(int i = 0; i < 2; i++){
                        ActiveThisTurn[i] = false;
                }
        }

        public void Stan(){
                StanCount = 2;                
        }

        public void DownStanCount(){
                if(StanCount > 0){
                        StanCount--;
                }
        }
}
