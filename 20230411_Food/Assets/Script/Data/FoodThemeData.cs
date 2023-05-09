using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    [System.Serializable]
    public class FoodThemeData
    {
        // PlayerのID
        [Tooltip("食材の目標割合")]
        public int[] TargetRate = new int[3];
        [Tooltip("食材のテキスト"), MultilineAttribute(3)]
        public string InstanceAddress;
    }
}
