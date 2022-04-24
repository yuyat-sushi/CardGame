using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "BaseAbility", menuName = "CardGame/BaseAbility", order = 0)]
public abstract class BaseAbility : ScriptableObject {
        [field: SerializeField]
        public string Text{get; private set;}
        [field: SerializeField]
        public bool Selectable{get; private set;} = false;
        [field: SerializeField]
        public bool SelfCast{get; private set;} = false;
        [field: SerializeField]
        public bool AttackOpponentSelect{get; private set;} = false;
        [field: SerializeField]
        public SelectCategory Category{get; private set;} = SelectCategory.Unit;
        [field: SerializeField]
        public int SelectCount{get; private set;} = 0;
        [field: SerializeField]
        public int PowerLimit{get; private set;} = -1;
        [field: SerializeField]
        public int CostLimit{get; private set;} = -1;

        public virtual void Ability(int actplayer){
                Debug.LogError("This is Select Only!");
        }
        public virtual void Ability(bool[,] selected, int actplayer){
                Debug.LogError("No Define Ability(bool[,] Selected)!");
        }
}