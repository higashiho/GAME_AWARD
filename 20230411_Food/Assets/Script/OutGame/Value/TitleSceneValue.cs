using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    /// <summary>
    /// Rayの長さ
    /// </summary>
    public sealed class RayDistance
    {
        public float Amount{get; private set;}
        
        public RayDistance(float tmpAmount)
        {
            Amount = tmpAmount;
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
        public float Amount{get; private set;}
        public SceneMoveTime(float tmpAmount)
        {
            Amount = tmpAmount;
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
        public float Amount{get; private set;}
        public TextApproachValue(float tmpAmount)
        {
            Amount = tmpAmount;
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
        public float Amount{get; private set;}
        public TextApproachMovementTime(float tmpAmount)
        {
            Amount = tmpAmount;
        }
    }

    /// <summary>
    /// UI挙動時間設定クラス
    /// </summary>
    public sealed class UIMoveTile
    {
        /// <summary>
        /// 時間
        /// </summary>
        public float Amount{get; private set;}
        public UIMoveTile(float tmpAmount)
        {
            Amount = tmpAmount;
        }
    }
}

