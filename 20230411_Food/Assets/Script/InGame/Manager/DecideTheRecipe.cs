using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    /// <summary>
    /// 料理を決めるクラス
    /// </summary>
    public class DecideTheRecipe 
    {
        // 食材データを取ってきて料理、量を決める
        // 
        public static int RecipeIndex{get; private set;}
        // コンストラクタ
        public DecideTheRecipe(FoodThemeDataList dataList)
        {
            RecipeIndex = UnityEngine.Random.Range(0, dataList.FoodThemes.Count);
        }


    }
}

