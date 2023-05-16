using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(fileName = "RecipeDataList", menuName = "ScriptableObjects/RecipeDataListAsset")]
    public class RecipeDataList : ScriptableObject
    {
        public enum RecipeNameEnum
        {
            INOUE,
            HIGASIHO,
            YUKUMOTO,
            HUKUMOTO,
            NAKAI,
            SUGATYAN

        }
        [EnumIndex(typeof(RecipeNameEnum))]
        public List<RecipeData> RecipeDatas = new List<RecipeData>(10);
    }
}

