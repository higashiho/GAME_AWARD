using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;

public class IngredientData : MonoBehaviour
{
    // FoodPoint
    public BaseFoodPoint FoodPoint{get; private set;}

    /// <summary>
    /// 食材データのコンストラクタ
    /// ポイントをコンストラクタで設定
    /// </summary>
    /// <param name="tmpFoodPoint">ポイント</param>
    public void SetIngredientData(BaseFoodPoint tmpFoodPoint)
    {
        FoodPoint = tmpFoodPoint;
    }
}
