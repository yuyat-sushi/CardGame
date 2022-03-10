using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "MainCardData", menuName = "CardGame/MainCardData", order = 0)]
public abstract class MainCardData : ScriptableObject {
        [field: SerializeField]
        [field: RenameField("Cost")]
        public int Cost{get; private set;}
        [field: SerializeField]
        [field: RenameField("CardName")]
        public string CardName{get; private set;}
        [field: SerializeField]
        public Element[] Elements{get; private set;} = new Element[2];
        [field: SerializeField]
        public string[] Types{get; private set;} = new string[2];
}