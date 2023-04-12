using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;

namespace player
{
    public class BasePlayer
    {
        // プレイヤーオブジェクト
        public GameObject PlayerObject;

        // プレイヤーの生成座標
        public PlayerInstancePos InstancePosOne;

        // プレイヤーの速度
        public MoveSpeed Speed;

        // 取得ポイント
        public MeatPoint MeatPoint;
        public FishPoint FishPoint;
        public VegPoint VegPoint;
        public SugarPoint SugarPoint;
        public SaltPoint SaltPoint;
        public VinePoint VinePoint;
        public SoyPoint SoyPoint;
        public MisoPoint MisoPoint;

        private DataPlayer data;
        public DataPlayer Data{get{return data;}protected set{data = value;}}

        private PlayerInstance playerInstance;
        public PlayerInstance PlayerInstance{get{return playerInstance;}protected set{playerInstance = value;}}
    }
}

