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
}

