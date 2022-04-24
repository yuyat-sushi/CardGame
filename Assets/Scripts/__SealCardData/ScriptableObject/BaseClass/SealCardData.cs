using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SealCardData", menuName = "CardGame/SealCardData", order = 0)]
public class SealCardData : ScriptableObject {
        [field: SerializeField]
        public string Text{get; private set;}
        [field: SerializeField]
        public Element Element{get; private set;}
        [field: SerializeField]
        public BaseAbility Ability{get; private set;}
        [field: SerializeField]
        public KeyWord KeyWord{get; private set;}
        [field: SerializeField]
        public int Power{get; private set;}
}