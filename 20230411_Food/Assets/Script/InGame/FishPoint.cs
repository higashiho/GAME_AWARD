using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class FishPoint
    {
        private int fishPointAmount;

        public FishPoint(int amount)
        {
            //値の初期値
            fishPointAmount = amount;
        }

        //値を増やすメソッド
        public FishPoint Add(FishPoint addAmount)
        {
            //インスタンス生成
            return new FishPoint(fishPointAmount + addAmount.fishPointAmount);
        }
    }
}
