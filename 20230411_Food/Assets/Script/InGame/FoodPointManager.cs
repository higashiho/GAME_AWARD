using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class FoodPointManager 
    {
        // プレイヤー１のFoodPoint取得用
        private FoodPoint sumPlayer1FoodPoint;

        // プレイヤー２のFoodPoint取得用
        private FoodPoint sumPlayer2FoodPoint;

        // 指定された料理のパラメータ取得用変数
        


        // FoodPointの多いプレイヤーを取得するメソッド
        public void GetWinner()
        {
            // PlayerのSumFoodpointを比較して
            // 大きい方のプレイヤーを返す
            // Mathクラス
        }

        // 例)  fishPoint
        //
        // 指定された料理のfishPointと比較して割合を取得する
        // 割合 < 1　指定された料理のfishPoint * 割合  => FoodPointに加算
        // 割合 > 1  指定された料理のfishpoint * (割合 - 1)  => FoodPointに加算

    }
}

