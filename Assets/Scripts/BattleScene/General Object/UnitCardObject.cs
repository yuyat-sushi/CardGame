using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCardObject
{
        //元となったカードのID
        public int CardID{get; private set;}
        //元のパワー
        public int BasePower{get; private set;}
        //現在のパワー
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

        //そのターンに出したかどうか
        public bool SummonThisTurn{get; private set;}

        //そのターンにアクティブ効果を一度使用したか
        public bool[] ActiveThisTurn{get; private set;} = new bool[2];

        public UnitCardObject(){
                CardID = -1;
                BasePower = 0;
                CurrentPower = 0;
                BaseKeyWord = new KeyWord();
                CurrentKeyWord = new KeyWord();
                OpponentTurnKeyWord = new KeyWord();
                RecievedDamage = 0;
                RecievedBuff = 0;
                TapMode = false;
                SummonThisTurn = false;
                ActiveThisTurn = new bool[2];
        }

        //Set
        //カードIDとパワーを指定する
        public void Set(int id, int power, KeyWord keyword){
                CardID = id;
                BasePower = power;
                CurrentPower = power;
                BaseKeyWord = new KeyWord(keyword);
                CurrentKeyWord = new KeyWord(keyword);
                OpponentTurnKeyWord = new KeyWord();
                RecievedDamage = 0;  
                RecievedBuff = 0;
                TapMode = false;
                DoubleAttacked = false;
                SummonThisTurn = true;
                ActiveThisTurn = new bool[2];
        }

        //Destroy
        //カードIDを-1にして、フィールドに無いものとして扱う
        public void Destroy(){
                CardID = -1;
        }

        //Damage
        //パワーを指定した数減らし、受けたダメージを記録する
        //ダメージがマイナスだった場合スキップされる
        public void Damage(int damage){
                if(damage >= 0){
                        CurrentPower -= damage;
                        RecievedDamage += damage;
                }
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

        public void OffSummonThisTurn(){
                SummonThisTurn = false;
        }

        public void OnActiveThisTurn(int abilityNum){
                ActiveThisTurn[abilityNum] = true;
        }

        public void OffActiveThisTurn(){
                for(int i = 0; i < 2; i++){
                        ActiveThisTurn[i] = false;
                }
        }

}
