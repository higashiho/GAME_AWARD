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


        // プレイヤー
        private PlayerManager player;
        

        /// <summary>
        /// プレイヤーの取得したポイント群
        /// </summary>
        /// <value>
        /// ０: VegetablePoint
        /// １: MeatPoint
        /// ２: FishPoint
        /// ３: SeasousingPoint
        /// ４: AmountRate
        /// </value>
        public static int[,] PlayerPercentageArr{get; private set;} = new int[2,5];
 
        public static int[,] FoodScoreValues{get; private set;} = new int[2,3];

        public enum Point
        {
            // 肉ポイント
            MEATPOINT = 0,
            // 魚ポイント
            FISHPOINT = 1,
            // 野菜ポイント
            VEGPOINT = 2,
            // 満腹度ポイント
            AMOUNTPOINT = 3,
            // 調味料ポイント
            SEASOUSINGPOINT = 4,
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
        public void GetPlayerPoint(int num ,PlayerManager player)
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

            PlayerPercentageArr[num, 0] = meatPoint;
            PlayerPercentageArr[num, 1] = vegetablePoint;
            PlayerPercentageArr[num, 2] = fishPoint;
            PlayerPercentageArr[num, 3] = seasousing;
            PlayerPercentageArr[num, 4] = amount;

            setScoreToArray();
        }

        /// <summary>
        /// 割合計算メソッド
        /// </summary>
        /// <param name="getPoint">プレイヤーが取得したポイント</param>
        /// <param name="targetPoint">目標のポイント</param>
        /// <returns>得点率(100点中?)</returns>
        public static int CalcThePercentage(float getPoint, float targetPoint)
        {
            int percent = 0;
            // 割合を計算
            float rate = getPoint / targetPoint;

            Debug.Log("rate :" + rate);
            // 割合が1より大きい場合
            if(rate > 1)
            {
                percent = (int)((1 - (rate % 1) ) * 100);
            }
            else if(rate <= 1)
            {
                percent = (int)(rate * 100);
            }
            else
            {
                Debug.LogError("結果が負になっています");
            }

            return percent;
        }

        /// <summary>
        /// フードポイントの平均値を計算するメソッド
        /// </summary>
        /// <returns>
        /// リザルト表示用のFoodPointの得点率
        /// </returns>
        private FoodPoint calcFoodPoint(int num)
        {
            int sumPoint = 0;
            for(int i = 0; i < 3; i++)
            {
                sumPoint += PlayerPercentageArr[num, i];
            }

            return new FoodPoint(sumPoint / 3);
        }

        /// <summary>
        /// リザルト
        /// </summary>
        private void setScoreToArray()
        {
            for(int i = 0; i < GameManager.ObjectManager.PlayerManagers.Count; i++)
            {
                FoodScoreValues[i,0] = calcFoodPoint(i).Point;

                //*****************要修正
                // 量ポイントを追加
                FoodScoreValues[i,1] = PlayerPercentageArr[i, 4];
                // 調味料ポイントを追加
                FoodScoreValues[i,2] = PlayerPercentageArr[i, 3];
            }
            
        }
       
    }
}

