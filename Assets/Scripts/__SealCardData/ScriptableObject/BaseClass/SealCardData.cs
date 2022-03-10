using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SealCardData", menuName = "CardGame/SealCardData", order = 0)]
public abstract class SealCardData : ScriptableObject {
        [field: SerializeField]
        public string Text{get; private set;}
        public virtual bool ConditionCheck(bool player){
                Debug.LogError("No Define ConditionCheck!");
                return false;
        }
}