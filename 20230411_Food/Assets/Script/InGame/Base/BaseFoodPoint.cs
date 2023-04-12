using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class BaseFoodPoint 
    {
        // ポイントの量
        public int Amount{get; protected set;}

        public enum FoodPointName
        {
            MEAT,   FISH,   VEG,    SUGAR,  SALT,   VINE,   SOY,    MISO
        }
        public FoodPointName PointName;
    }
}

