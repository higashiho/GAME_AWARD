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
        // 調味料
        public SeasousingPoint SeasousingPoint{get; protected set;}
    }
}

