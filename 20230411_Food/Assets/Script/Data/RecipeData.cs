using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InGame
{
    /// <summary>
    /// レシピデータクラス
    /// </summary>
    [System.Serializable]
    public class RecipeData 
    {
        public enum PointEnum
        {
            MEAT, VEG, FISH, AMOUNT, SEASOUSING
        }
        /// <summary>
        /// 料理データ
        /// </summary>
        [Tooltip("ポイント配列"), EnumIndex(typeof(PointEnum))]
        public int[] Points = new int[5];

        /// <summary>
        /// 料理の名前
        /// </summary>
        [Tooltip("料理の名前")]
        public string Name;
    }
    
    
  
}

