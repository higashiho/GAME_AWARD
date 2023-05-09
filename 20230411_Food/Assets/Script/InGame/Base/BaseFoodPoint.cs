using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FoodPoint
{
    public abstract class BaseFoodPoint 
    {
        

        
        

        // ポイントの量
        public int Amount{get; protected set;}

        public enum FoodPointName
        {
            MEAT,   FISH,   VEG,   SEASOUSING,   SATIETY
        }
        public FoodPointName PointName;
    }
}

