using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;



using Player;
using Food;


namespace FoodPoint
{
    // プレイヤー => MeatPoint, FishPoint, VegetablePoint => 三点の比を計算する      =>５割
    //           => 調味料ポイントを計算する                                         =>４割
    //           => 量の割合を計算する                                               =>１割


    // プレイヤーのDictionaryを参照
    // FoodDataと比較
    // 計算


    /// <summary>
    /// プレイヤーが取得したポイントを管理するクラス
    /// </summary>
    public class PointManager 
    {
        // フードデータ
        private FoodData foodData;

        // プレイヤー
        private PlayerManager player;

        private FoodPoint[] pointArr = new FoodPoint[5];

        // 指定された料理のポイント配列
        private BaseFoodPoint[] specifiedDishPoint = new BaseFoodPoint[5];

        // FoodPoint
        private Dictionary<Point, BaseFoodPoint> pointMap = new Dictionary<Point, BaseFoodPoint>(5);

    
        public enum Point
        {
            // 肉ポイント
            MEATPOINT = 0,
            // 魚ポイント
            FISHPOINT = 1,
            // 野菜ポイント
            VEGPOINT = 2,
            // 調味料ポイント
            SEASOUSINGPOINT = 3,
            // 満腹度ポイント
            LEVELOFSATIETY = 4
        }

        /// <summary>
        /// ポイントマネージャーのコンストラクタ
        /// </summary>
        /// <param name="data">料理の数値データ</param>
        public PointManager()
        {
            pointMap.Add(Point.MEATPOINT, new MeatPoint(0));
            pointMap.Add(Point.FISHPOINT, new FishPoint(0));
            pointMap.Add(Point.VEGPOINT, new VegPoint(0));
            pointMap.Add(Point.SEASOUSINGPOINT, new SeasousingPoint(0));
            pointMap.Add(Point.LEVELOFSATIETY, new LevelOfSatiety(0));
        }

        // ポイントの取得メソッド
        public void GetPoint(Point tmpPoint)
        {
            BaseFoodPoint foodPoint = pointMap[tmpPoint];
        }

        //private 

        

       

        public void DetermineDish()
        {
            int rand = UnityEngine.Random.Range(0, DishData.DishPointData.Count);

            // 料理を設定
            foodData = new FoodData(rand);

        }
        /// <summary>
        /// プレイヤーのFOODPOINTを取得するメソッド
        /// </summary>
        public void GetPlayerFoodPoint(PlayerManager tmpPlayer)
        {
            // 一旦羅列してます
            // DishData設定時に値型を決定 => プレイヤーが取得時に型を判定するように変更する

            
            //pointArr[0] = CalcFoodPoint(new MeatPoint(tmpPlayer.FoodPoint.Array["MeatPoint"]), new MeatPoint(int.Parse(foodData.DishPoints[FoodData.DataIndex.MeatPoint])));
            Debug.Log(pointArr[0]);
            
        }

        /// <summary>
        /// 料理を指定するメソッド
        /// Openingなどで呼び出す
        /// </summary>
        /// <param name="id">料理ID</param>
        public void SetFood(int id)
        {
            if(id < 0)
            {
                Debug.LogError("指定された料理のIDは負の値で存在しません");
                return;
            }

            // 料理を生成
            //foodData = new FoodData(dishData, id);
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

