using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckData", menuName = "CardGame/DeckData", order = 0)]
public class DeckData : ScriptableObject {
    //デッキマスターネーム
    [field: SerializeField]
    public string MasterName{get; private set;}
    //デッキの色
    [field: SerializeField]
    public Element[] Color{get; private set;} = new Element[2];
    //タイプ
    [field: SerializeField]
    public string[] Types{get; private set;} = new string[2];
    //シールランク
    [field: SerializeField]
    public int SealRank{get; private set;}
    //シールカード
    [field: SerializeField]
    public SealCardData[] SealCard{get; private set;} = new SealCardData[4];
    //キーワード
    [field: SerializeField]
    public KeyWord KeyWord{get; private set;}
    //アビリティカード
    [field: SerializeField]
    public AbilityCardData[] AbilityCard{get; private set;} = new AbilityCardData[2];
    //メインデッキ
    [field: SerializeField]
    public List<int> Deck{get; private set;}
}
