using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnchantCardData", menuName = "CardGame/MainCard/EnchantCardData", order = 0)]
public class EnchantCardData : MainCardData {
        [field: SerializeField]
        [field: RenameField("Sprite")]
        public Sprite Sprite{get; private set;}
        [field: SerializeField]
        [field: RenameField("Trigger")]
        public Trigger Trigger{get; private set;}
        [field: SerializeField]
        public int ActiveManaCost{get; private set;}
        [field: SerializeField]
        public bool ActiveTurnOnce{get; private set;}
        [field: SerializeField]
        public bool ActiveDontSummonTurn{get; private set;}
        [field: SerializeField]
        [field: RenameField("Ability")]
        public BaseAbility Ability{get; private set;}
}