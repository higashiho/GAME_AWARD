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

    // FoodDataからテキストメッセージを取得
    // UIで料理を指定するときにメッセージを呼び出せるようにしておく 

    /// <summary>
    /// プレイヤーが取得したポイントを管理するクラス
    /// </summary>
    public class PointManager 
    {
        // フードデータ
        private FoodData foodData;
        // 料理データ
        private DishData dishData;
        // プレイヤー
        private PlayerManager player;

        /// <summary>
        /// ポイントマネージャーのコンストラクタ
        /// </summary>
        /// <param name="data">料理の数値データ</param>
        public PointManager(DishData data)
        {
            dishData = data;
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
            foodData = new FoodData(dishData, id);
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

