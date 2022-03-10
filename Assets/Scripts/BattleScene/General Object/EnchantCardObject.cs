using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantCardObject
{
        //元となったカードのID
        public int CardID{get; private set;}

        //そのターンに出したかどうか
        public bool SummonThisTurn{get; private set;}

        //そのターンにアクティブ効果を一度使用したか
        public bool ActiveThisTurn{get; private set;}

        public EnchantCardObject(){
                CardID = -1;
                SummonThisTurn = false;
                ActiveThisTurn = false;
        }

        public void SetFieldEnchant(int id){
                CardID = id;
                SummonThisTurn = true;
                ActiveThisTurn = false;
        }

        public void Destory()
        {
                CardID = -1;
                SummonThisTurn = false;
                ActiveThisTurn = false;
        }

        public void OffSummonThisTurn(){
                SummonThisTurn = false;
        }

        public void OnActiveThisTurn(){
                ActiveThisTurn = true;
        }

        public void OffActiveThisTurn(){
                ActiveThisTurn = false;
        }
}
