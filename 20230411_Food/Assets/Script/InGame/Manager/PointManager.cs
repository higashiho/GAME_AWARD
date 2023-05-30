using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

using Player;
using InGame;
using Food;


namespace FoodPoint
{
    /// <summary>
    /// プレイヤーが取得したポイントを管理するクラス
    /// </summary>
    public class PointManager 
    {

        public enum Point
        {
            MEATPOINT, FISHPOINT, VEGPOINT, AMOUNTPOINT, SEASOUSINGPOINT
        }
        // プレイヤー
        private PlayerManager player;
        private FoodThemeDataList foodData;
        

        /// <summary>
        /// プレイヤーの取得したポイント群
        /// </summary>
        /// <value>
        /// ０: VegetablePoint
        /// １: MeatPoint
        /// ２: FishPoint
        /// ３: AmountRate
        /// ４: SeasousingPoint
        /// </value>
        public static int[,] PlayerPercentageArr{get; private set;} = new int[2,5];
 
        public static int[,] FoodScoreValues{get; private set;} = new int[2,3];

        
        /// <summary>
        /// ポイントマネージャーのコンストラクタ
        /// </summary>
        /// <param name="data">料理の数値データ</param>
        public PointManager(PlayerManager tmpPlayer, FoodThemeDataList dataList)
        {
            player = tmpPlayer;
            foodData = dataList;
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
            int sumFood = 0;
            
            // 野菜ポイント
            int[] val = player.FoodPoint.Array["VEGETABLE"];
            vegetablePoint = val[0];
            amount += val[1];
            // 肉ポイント
            val = player.FoodPoint.Array["MEAT"];
            meatPoint = val[0];
            amount += val[1];
            // 魚ポイント
            val = player.FoodPoint.Array["FISH"];
            fishPoint = val[0];
            amount += val[1];
            // 調味料ポイント
            val = player.FoodPoint.Array["SEASOUSING"];
            seasousing = val[0];
            
            amount += val[1];

            sumFood += meatPoint;
            sumFood += vegetablePoint;
            sumFood += fishPoint;
            
            PlayerPercentageArr[num, 0] = percentage(meatPoint, sumFood);//CalcThePercentage(meatPoint, foodData.FoodThemes[0].TargetRate[0]);
            PlayerPercentageArr[num, 1] = percentage(vegetablePoint, sumFood);//CalcThePercentage(vegetablePoint, foodData.FoodThemes[0].TargetRate[1]);
            PlayerPercentageArr[num, 2] = percentage(fishPoint, sumFood);//CalcThePercentage(fishPoint, foodData.FoodThemes[0].TargetRate[2]);
            PlayerPercentageArr[num, 3] = amount;
            PlayerPercentageArr[num, 4] = seasousing;
            
            setScoreToArray();
        }

        private int percentage(float point, float sum)
        {
            if(sum == 0)
            {
                return 0;
            }

            return (int)(10 * (point / sum));

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
            if(targetPoint == 0)
            {
                return percent;
            }
            // 割合を計算
            float rate = getPoint / targetPoint;

            // 割合が1より大きい場合
            if(rate > 1)
            {
                percent = (int)((1 - (rate % 1) ) * 100);
            }
            else if(rate <= 1)
            {
                percent = (int)(rate * 100);
            }
            else if(rate < 0)
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
                sumPoint += CalcThePercentage(PlayerPercentageArr[num, i], GameManager.ObjectManager.GameManager.FoodData.FoodThemes[InGame.DecideTheRecipe.RecipeIndex].TargetRate[i]);
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
                Debug.Log("Player" + i + "のFoodPoint" + FoodScoreValues[i,0]);
                // 量ポイントを追加
                FoodScoreValues[i,1] = PlayerPercentageArr[i, 3];
                // 調味料ポイントを追加
                FoodScoreValues[i,2] = PlayerPercentageArr[i, 4];

            }
            
        }
       
    }
}

