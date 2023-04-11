using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class SugarPoint
    {
        private int sugarPointAmount;

        public SugarPoint(int amount)
        {
            //値の初期値
            sugarPointAmount = amount;
        }

        //値を増やすメソッド
        public SugarPoint Add(SugarPoint addAmount)
        {
            //インスタンス生成
            return new SugarPoint(sugarPointAmount + addAmount.sugarPointAmount);
        }
    }
}
