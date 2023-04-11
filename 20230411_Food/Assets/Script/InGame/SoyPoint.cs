using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class SoyPoint
    {
        private int soyPointAmount;

        public SoyPoint(int amount)
        {
            //値の初期値
            soyPointAmount = amount;
        }

        //値を増やすメソッド
        public SoyPoint Add(SoyPoint addAmount)
        {
            //インスタンス生成
            return new SoyPoint(soyPointAmount + addAmount.soyPointAmount);
        }
    }
}
