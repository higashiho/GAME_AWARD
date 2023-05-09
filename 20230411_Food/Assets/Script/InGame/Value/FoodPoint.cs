using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class FoodPoint
    {
        // FoodPoint
        public int Point{get; private set;}

        // コンストラクタ => FoodPointの初期化
        public FoodPoint(int point)
        {
            if(point < 0)
            {
                Debug.LogError("FoodPointに渡された数値が負の値です。");
                return;
            }
            this.Point = point;
        }
    }
}