using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyWord {
        //鈍重
        //ドローフェイズにアンタップしない
        [field: SerializeField]
        public bool Dullness{get; set;}
        //無防備
        //アタックされた時、反撃することが出来ない
        [field: SerializeField]
        public bool Defenseless{get; set;}
        //脆弱
        //ターン終了時に破壊される
        [field: SerializeField]
        public bool Vulnerable{get; set;}
        //硬直
        //アタックできない
        [field: SerializeField]
        public bool Immobile{get; set;}
        //近接
        //プレイヤーにはアタックできない
        [field: SerializeField]
        public bool Melee{get; set;}
        //疾風
        //1ターンに1回アタック後にアンタップする
        [field: SerializeField]
        public bool DoubleAttack{get; set;}
        //貫通
        //相手ユニットを破壊した場合、超過したパワー分のダメージを与える
        [field: SerializeField]
        public bool Trumple{get; set;}
        //先制
        //ユニット・デッキマスター同士でバトルを行う際、先制を持っていない側からダメージを受ける
        [field: SerializeField]
        public bool FirstStrike{get; set;}
        //通過
        //相手ユニットを無視してアタックできる
        [field: SerializeField]
        public bool Passing{get; set;}
        //結界
        //相手はこのカードを効果で選べない
        [field: SerializeField]
        public bool Proof{get; set;}
        //防弾
        //カードの効果によるダメージを受けない
        [field: SerializeField]
        public bool Protect{get; set;}
        //守護
        //相手は守護を持ったカード以外をアタックする事が出来ない（デッキマスターが持った場合は自ユニットが優先され、通過で無視される）
        [field: SerializeField]
        public bool GuardMan{get; set;}
        //接死
        //このカードとのバトルでダメージを受けたカードはバトル終了後破壊される
        [field: SerializeField]
        public bool Slayer{get; set;}
        //威迫
        //コスト3以下のユニットにアタックされず、コスト3以下のユニットを無視してアタックできる。
        [field: SerializeField]
        public bool Menace{get; set;}

        public KeyWord(){
                Dullness = false;
                Defenseless = false;
                Vulnerable = false;
                Immobile = false;
                Melee = false;
                DoubleAttack = false;
                Trumple = false;
                FirstStrike = false;
                Passing = false;
                Proof = false;
                Protect = false;
                GuardMan = false;
                Slayer = false;
                Menace = false;
        }

        public KeyWord(KeyWord keyword){
                Dullness = keyword.Dullness;
                Defenseless = keyword.Defenseless;
                Vulnerable = keyword.Vulnerable;
                Immobile = keyword.Immobile;
                Melee = keyword.Melee;
                DoubleAttack = keyword.DoubleAttack;
                Trumple = keyword.Trumple;
                FirstStrike = keyword.FirstStrike;
                Passing = keyword.Passing;
                Proof = keyword.Proof;
                Protect = false;
                GuardMan = keyword.GuardMan;
                Slayer = keyword.Slayer;
                Menace = keyword.Menace;
        }

        //キーワードを追加する
        //論理和によって、追加したいキーワードをそのまま追加する
        public void AddKeyword(KeyWord keyword){
                Dullness |= keyword.Dullness;
                Defenseless |= keyword.Defenseless;
                Vulnerable |= keyword.Vulnerable;
                Immobile |= keyword.Immobile;
                Melee |= keyword.Melee;
                DoubleAttack |= keyword.DoubleAttack;
                Trumple |= keyword.Trumple;
                FirstStrike |= keyword.FirstStrike;
                Passing |= keyword.Passing;
                Proof |= keyword.Proof;
                Protect |= keyword.Protect;
                GuardMan |= keyword.GuardMan;
                Slayer |= keyword.Slayer;
                Menace |= keyword.Menace;
        }

        //キーワードを表示する
        public override string ToString(){
                string text = "";
                if(Dullness){
                        text += "◆鈍重 ";
                }if(Defenseless){
                        text += "◆無防備 ";
                }if(Vulnerable){
                        text += "◆脆弱 ";
                }if(Immobile){
                        text += "◆硬直 ";
                }if(Melee){
                        text += "◆近接 ";
                }if(DoubleAttack){
                        text += "◆疾風 ";
                }if(Trumple){
                        text += "◆貫通 ";
                }if(FirstStrike){
                        text += "◆先制 ";
                }if(Passing){
                        text += "◆通過 ";
                }if(Proof){
                        text += "◆結界 ";
                }if(Protect){
                        text += "◆防弾 ";
                }if(GuardMan){
                        text += "◆守護 ";
                }if(Slayer){
                        text += "◆接死 ";
                }if(Menace){
                        text += "◆威迫 ";
                }
                if(text != "") text += "\n";
                return text;
        }
}