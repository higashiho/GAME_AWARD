using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace player
{
    public class AdjustPlayer
    {
        // プレイヤーオブジェクト
        public GameObject PlayerObject;

        // プレイヤーのプレハブ
        public GameObject PlayerPref;

        // プレイヤーの生成座標
        public PlayerInstancePos InstancePosOne;

        private DataPlayer data;
        public DataPlayer Data{get{return data;}protected set{data = value;}}
    }
}

