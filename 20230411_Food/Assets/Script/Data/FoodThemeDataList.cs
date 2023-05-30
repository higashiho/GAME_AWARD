using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(fileName = "FoodThemeDataList", menuName = "ScriptableObjects/FoodThemeAsset")]
    public class FoodThemeDataList : ScriptableObject
    {
        public List<FoodThemeData> FoodThemes = new List<FoodThemeData>(10);
    }
}
