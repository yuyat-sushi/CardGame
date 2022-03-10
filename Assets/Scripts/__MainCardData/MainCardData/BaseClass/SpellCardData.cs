using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellCardData", menuName = "CardGame/MainCard/SpellCardData", order = 0)]
public class SpellCardData : MainCardData {
        [field: SerializeField]
        [field: RenameField("Ability")]
        public BaseAbility Ability{get; private set;}
}