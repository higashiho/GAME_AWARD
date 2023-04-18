using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;

namespace player
{
    public class BasePlayer
    {
        // プレイヤーの生成座標
        public PlayerInstancePos InstancePosOne{get; protected set;}

        // プレイヤーが取得したポイントを保管しておく配列
        public Dictionary<BaseFoodPoint, int> PointArr = new Dictionary<BaseFoodPoint, int>();

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

