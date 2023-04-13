using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public sealed class RayDistance
    {
        public float DistanceAmount{get; private set;}
        
        public RayDistance(float tmpAmount)
        {
            DistanceAmount = tmpAmount;
        }
    }

    
    /// <summary>
    /// シーン挙動までにかかる時間設定クラス
    /// </summary>
    public sealed class SceneMoveTime
    {
        /// <summary>
        /// シーン挙動までにかかる時間
        /// </summary>
        public float MoveTimeAmount{get; private set;}
        public SceneMoveTime(float tmpAmount)
        {
            MoveTimeAmount = tmpAmount;
        }
    }

    /// <summary>
    /// テキストY座標接近加算距離設定クラス
    /// </summary>
    public sealed class TextApproachValue
    {
        /// <summary>
        /// テキストY座標接近加算距離
        /// </summary>
        public float TextApproachAmount{get; private set;}
        public TextApproachValue(float tmpAmount)
        {
            TextApproachAmount = tmpAmount;
        }
    }

    /// <summary>
    /// テキストY座標接近時間設定クラス
    /// </summary>
    public sealed class TextApproachMovementTime
    {
        /// <summary>
        /// テキストY座標接近時間
        /// </summary>
        public float TextApproachMovementTimeAmount{get; private set;}
        public TextApproachMovementTime(float tmpAmount)
        {
            TextApproachMovementTimeAmount = tmpAmount;
        }
    }
}

