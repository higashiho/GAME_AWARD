using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class VegPoint
    {
        private int vegPointAmount;

        public VegPoint(int amount)
        {
            //値の初期値
            vegPointAmount = amount;
        }

        //値を増やすメソッド
        public VegPoint Add(VegPoint addAmount)
        {
            //インスタンス生成
            return new VegPoint(vegPointAmount + addAmount.vegPointAmount);
        }
    }
}