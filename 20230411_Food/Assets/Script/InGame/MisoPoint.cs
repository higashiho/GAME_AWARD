using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class MisoPoint
    {
        private int misoPointAmount;

        public MisoPoint(int amount)
        {
            //値の初期値
            misoPointAmount = amount;
        }

        //値を増やすメソッド
        public MisoPoint Add(MisoPoint addAmount)
        {
            //インスタンス生成
            return new MisoPoint(misoPointAmount + addAmount.misoPointAmount);
        }
    }
}