using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainCardDataBase", menuName = "CardGame/MainCardDataBase", order = 0)]
public class MainCardDataBase : ScriptableObject {
        [field: SerializeField]
        public MainCardData[] Cards{get; private set;} = new MainCardData[0];
}