using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    /// <summary>
    /// アウトゲーム定数クラス
    /// </summary>
    public abstract class OutGameConstants
    {
        /// <summary>プレイヤー左向き</summary>
        public static readonly Vector3 PLAYER_DIRECTION_LEFT = new Vector3(0, 270, 0);
        /// <summary>プレイヤー右向き</summary>
        public static readonly Vector3 PLAYER_DIRECTION_RIGHT = new Vector3(0, 90, 0);
        /// <summary>プレイヤー後ろ向き</summary>
        public static readonly Vector3 PLAYER_DIRECTION_BACK = new Vector3(0, 180, 0);
        /// <summary>プレイヤーのrayの長さ</summary>
        public const float PLAYER_RAY_DISTANCE = 1.0f;
    }
}

