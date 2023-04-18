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
}

