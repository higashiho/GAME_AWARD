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
    /// <summary>
    /// プレイヤーが取得したポイントを管理するクラス
    /// </summary>
    public class PointManager 
    {
        // フードデータ
        private FoodData foodData;

        // 満腹度ポイント
        public LevelOfSatiety Amount;
        // 食料ポイント
        public FoodPoint Points;
        // 調味料ポイント
        public SeasousingPoint SeasPoint;

        // プレイヤー
        private PlayerManager player;


        // 指定された料理のポイント配列
        private BaseFoodPoint[] specifiedDishPoint = new BaseFoodPoint[5];


        static public int[] playerPercentageArr = new int[5];


        static public int[] arr = new int[3];

    
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
        public Point PointType;
        /// <summary>
        /// ポイントマネージャーのコンストラクタ
        /// </summary>
        /// <param name="data">料理の数値データ</param>
        public PointManager(PlayerManager tmpPlayer)
        {
            player = tmpPlayer;
        }

        /// <summary>
        /// プレイヤーのポイントを取得するメソッド
        /// </summary>
        /// <returns></returns>
        public void GetPlayerPoint(PlayerManager player)
        {
            int vegetablePoint = 0;
            int meatPoint = 0;
            int fishPoint = 0;
            int amount = 0;
            int seasousing = 0;
            
            int[] val = player.FoodPoint.Array["VEGETABLE"];
            vegetablePoint = val[0];
            amount += val[1];

            val = player.FoodPoint.Array["MEAT"];
            meatPoint = val[0];
            amount += val[1];

            val = player.FoodPoint.Array["FISH"];
            fishPoint = val[0];
            amount += val[1];

            val = player.FoodPoint.Array["SEASOUSING"];
            seasousing = val[0];
            amount += val[1];

            playerPercentageArr[0] = meatPoint;
            playerPercentageArr[1] = vegetablePoint;
            playerPercentageArr[2] = fishPoint;
            playerPercentageArr[3] = seasousing;
            playerPercentageArr[4] = amount;
            
            Debug.Log("MEAT" + playerPercentageArr[0]);
            Debug.Log("VEG" + playerPercentageArr[1]);
            Debug.Log("FISH" + playerPercentageArr[2]);
            Debug.Log("SEAS" + playerPercentageArr[3]);
            Debug.Log("AMOUNT" + playerPercentageArr[4]);


            

        }

        /// <summary>
        /// 割合計算メソッド
        /// </summary>
        /// <param name="getPoint">プレイヤーが取得したポイント</param>
        /// <param name="targetPoint">目標のポイント</param>
        /// <returns>得点率</returns>
        public int CalcThePercentage(int getPoint, int targetPoint)
        {
            int percent = 0;
            // 割合を計算
            int rate = getPoint / targetPoint;

            // 割合が1より大きい場合
            if(rate > 1)
            {
                percent = (1 - (rate % 1) ) * 100;
            }
            else if(rate >= 0)
            {
                percent = rate * 100;
            }
            else
            {
                Debug.LogError("結果が負になっています");
            }

            return percent;
        }

    }
}

