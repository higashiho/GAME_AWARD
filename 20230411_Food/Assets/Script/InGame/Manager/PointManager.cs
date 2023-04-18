using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;



using Player;


namespace FoodPoint
{
    // プレイヤー => MeatPoint, FishPoint, VegetablePoint => 三点の比を計算する      =>５割
    //           => 調味料ポイントを計算する                                         =>４割
    //           => 量の割合を計算する                                               =>１割

    /// <summary>
    /// プレイヤーが取得したポイントを管理するクラス
    /// </summary>
    public class PointManager 
    {
        // 料理データ
        private DishData dishData;

        private enum DataIndex
        {
            DishName = 0,
            ExplanatoryText = 1,
            MeatPoint = 2,
            FishPoint = 3,
            VegPoint = 4,
            LevelOfSatietyPoint = 5
        }


        /// <summary>
        /// 指定された料理のポイントを保管しておく配列
        /// Meatpoint, FishPoint, VegPoint, LevelOfSatietyPoint 
        /// </summary>
        // private BaseFoodPoint[] specifiedFoodPoints = new BaseFoodPoint[4];
        private Dictionary<DataIndex, string> specifiedFoodPoints = new Dictionary<DataIndex, string>(6);
        private string[] getData = new string[16];
         
        
        public PointManager(DishData data, int dishId)
        {
            dishData = data;
            
            GetDishData(dishId);
            
        }

        /// <summary>
        /// int型の値を渡すとその数値に応じたインデックスの料理データをstringで取得するメソッド
        /// </summary>
        public void GetDishData(int dishId)
        {
            getData = dishData.DishPointData[dishId];
            
            for(int i = 0; i < Enum.GetValues(typeof(DataIndex)).Length; i++)
            {
                specifiedFoodPoints.Add((DataIndex)Enum.ToObject(typeof(DataIndex), i), getData[i]);
            }
            
        }

        /// <summary>
        /// 調味料ポイントを計算するメソッド
        /// プレイヤーが取得した調味料ポイントを計算して
        /// 得点化する
        /// 10点中何点みたいな。。。
        /// </summary>
        public void CalcSeasoningPoint()
        {

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

