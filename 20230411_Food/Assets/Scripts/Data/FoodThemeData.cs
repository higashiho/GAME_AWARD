using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    [System.Serializable]
    public class FoodThemeData
    {
        private enum foodNameEnum
        { MEAT, FISH, VEGETABLE }
        [Tooltip("食材の名前")]
        public string FoodName;
        // PlayerのID
        [Tooltip("食材の目標割合"), EnumIndex(typeof(foodNameEnum))]
        public int[] TargetRate = new int[3];
        [Tooltip("食材のテキスト"), MultilineAttribute(3)]
        public string InstanceAddress;
    }
}
