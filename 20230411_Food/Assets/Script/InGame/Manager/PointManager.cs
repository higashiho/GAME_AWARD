using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


using player;

namespace FoodPoint
{
    // プレイヤーの取得したポイントをゲームシーン中に
    // staticな配列に入れておく

    // 料理IDを作る
    // 指定された料理のポイントを把握しておく => Dataを取得
    // コンストラクタでプレイヤーのポイントを取得
    // 計算メソッドを呼んで割合を計算しておく
    // 得点に換算
    // ポイント毎に得点を配列に保存
    // => リザルトシーンで表示

    /// <summary>
    /// プレイヤーが取得したポイントを管理するクラス
    /// </summary>
    public class PointManager 
    {
        /// <summary>
        /// プレイヤー１のポイントを保管しておく配列
        /// </summary>
        // private static FoodPoint[] Player1FoodPoints;

        /// <summary>
        /// プレイヤー２のポイントを保管しておく配列
        /// </summary>
        // private static FoodPoint[] Player2FoodPoints;  
        private DishData dishData;
  

        /// <summary>
        /// 指定された料理のポイントを保管しておく配列
        /// </summary>
        private FoodPoint[] specifiedFoodPoints; 

         
        
        public PointManager(DishData data, int dishId)
        {
            dishData = data;
            GetDishData(dishId);
            // for(int i = 0; i < 8; i++)
            // {
            //     // Player1FoodPoints = CalcFoodPoint(player1.PointArr, 指定された料理のポイント配列);
            //     // Player2Foodpoints = CalcFoodPoint(player2.PointArr, 指定された料理のポイント配列);
            // }
        }

        /// <summary>
        /// int型の値を渡すとその数値に応じたインデックスの料理データを取得するメソッド
        /// </summary>
        public void GetDishData(int dishId)
        {
            string[] data = dishData.DishPointData[dishId];
            for(int i = 1; i < data.Length; i++)
            {
                specifiedFoodPoints[i] = new FoodPoint(int.Parse(data[i]));
                Debug.Log(specifiedFoodPoints[i]);
            }
        }

        /// <summary>
        /// FoodPointに換算するメソッド
        /// ポイントは同じ型のものを渡してください
        /// </summary>
        /// <param name="tmpPoint">プレイヤーが取得したポイント</param>
        /// <param name="perfectPoint">指定された料理のポイント</param>
        /// <returns>各パラメータポイントをFoodPointに変換した数値</returns>
       public FoodPoint CalcFoodPoint(BaseFoodPoint tmpPoint, BaseFoodPoint perfectPoint)
       {
            // 指定された料理のポイントが０だった場合
            if(perfectPoint.Amount == 0)
            {
                return new FoodPoint(0);
            }
            
            // 取得したポイントの名前に相違がある場合
            if(tmpPoint.PointName != perfectPoint.PointName)
            {
                Debug.LogError("渡されたポイントと指定された料理のポイントの型が一致していません");
                return new FoodPoint(0);
            }

            float resultRate;
            // ポイント取得
            int rate = tmpPoint.Amount / perfectPoint.Amount;

            if(rate > 1)
            {
                resultRate = rate % 1;
            }
            else
            {
                resultRate = rate;
            }

            return new FoodPoint(perfectPoint.Amount * rate);
       }

        // 例)  fishPoint
        //
        // 指定された料理のfishPointと比較して割合を取得する
        // 割合 < 1　指定された料理のfishPoint * 割合  => FoodPointに加算
        // 割合 > 1  指定された料理のfishpoint * (割合 - 1)  => FoodPointに加算

    }
}


// ポイントデータを取得
// スクリプタブルオブジェクトに流し込む