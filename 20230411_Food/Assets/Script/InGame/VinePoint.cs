using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class VinePoint
    {
        private int vinePointAmount;

        public VinePoint(int amount)
        {
            //値の初期値
            vinePointAmount = amount;
        }

        //値を増やすメソッド
        public VinePoint Add(VinePoint addAmount)
        {
            //インスタンス生成
            return new VinePoint(vinePointAmount + addAmount.vinePointAmount);
        }
    }
}