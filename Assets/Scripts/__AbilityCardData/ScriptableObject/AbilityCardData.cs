using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityCardData", menuName = "CardGame/AbilityCardData", order = 0)]
public class AbilityCardData : ScriptableObject {
        [field: SerializeField]
        public string Name{get; private set;}
        [field: SerializeField]
        public Element[] RequireColor{get; private set;} = new Element[2];
        [field: SerializeField]
        public Trigger Trigger{get; private set;}
        [field: SerializeField]
        public int SealCost{get; private set;}
        [field: SerializeField]
        public bool ActiveTurnOnce{get; private set;}
        [field: SerializeField]
        public BaseAbility Ability{get; private set;}
}