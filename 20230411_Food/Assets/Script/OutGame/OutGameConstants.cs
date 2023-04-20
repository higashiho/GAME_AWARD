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
        /// <summary>テキストイメージ接近目標座標</summary>
        public const float TEXT_IMAGE_APPROACH_POS_Y = -95f;
        /// <summary>テキストイメージ離反目標座標</summary>
        public const float TEXT_IMAGE_LEAVE_POS_Y = -235f;

        /// <summary>
        /// 冷蔵庫UIテキストのy座標
        /// </summary>
        public static readonly float[] TEXT_CHILD_POS_Y = {250, 0, -250};

        /// <summary>
        /// UIの画面外座標
        /// </summary>
        public const float UI_OUTCAMERA_POS_X = 1800f;
    }
}

