using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;

namespace player
{
    public class BasePlayer
    {
        // プレイヤーが取得したポイントを保管しておく配列
        public Dictionary<string, int> PointArr = new Dictionary<string, int>();

        /// <summary>
        /// 取得ポイント
        /// </summary>
        // お肉
        public MeatPoint MeatPoint{get; protected set;}
        // お魚
        public FishPoint FishPoint{get; protected set;}
        // お野菜
        public VegPoint VegPoint{get; protected set;}
        // お砂糖
        public SugarPoint SugarPoint{get; protected set;}
        // お塩
        public SaltPoint SaltPoint{get; protected set;}
        // お酢
        public VinePoint VinePoint{get; protected set;}
        // お醤油
        public SoyPoint SoyPoint{get; protected set;}
        // お味噌
        public MisoPoint MisoPoint{get; protected set;}
    }
}

