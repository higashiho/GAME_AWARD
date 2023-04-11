using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class SaltPoint
    {
        private int saltPointAmount;

        public SaltPoint(int amount)
        {
            //値の初期値
            saltPointAmount = amount;
        }

        //値を増やすメソッド
        public SaltPoint Add(SaltPoint addAmount)
        {
            //インスタンス生成
            return new SaltPoint(saltPointAmount + addAmount.saltPointAmount);
        }
    }
}