using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class MeatPoint
    {
        private int meatPointAmount;

        public MeatPoint(int amount)
        {
            //値の初期値
            meatPointAmount = amount;
        }

        //値を増やすメソッド
        public MeatPoint Add(MeatPoint addAmount)
        {
            //インスタンス生成
            return new MeatPoint(meatPointAmount + addAmount.meatPointAmount);
        }
    }
}
