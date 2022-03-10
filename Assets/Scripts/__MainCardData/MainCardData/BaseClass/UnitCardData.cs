using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitCardData", menuName = "CardGame/MainCard/UnitCardData", order = 0)]
public class UnitCardData : MainCardData {
        [field: SerializeField]
        [field: RenameField("Sprite")]
        public Sprite Sprite{get; private set;}
        
        [field: SerializeField]
        [field: RenameField("Power")]
        public int Power{get; private set;}
        [field: SerializeField]
        public KeyWord KeyWord{get; private set;}
        [field: SerializeField]
        public Trigger[] Trigger{get; private set;} = new Trigger[2];
        [field: SerializeField]
        public int[] ActiveManaCost{get; private set;} = new int[2];
        [field: SerializeField]
        public bool[] ActiveTurnOnce{get; private set;} = new bool[2];
        [field: SerializeField]
        public bool[] ActiveDontSummonTurn{get; private set;} = new bool[2];
        [field: SerializeField]
        public BaseAbility[] Ability{get; private set;} = new BaseAbility[2];
}