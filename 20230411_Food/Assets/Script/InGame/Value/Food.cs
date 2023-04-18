using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;
namespace Food
{
    public class Food
    {
        public MeatPoint MeatPoint;

        public FishPoint FishPoint;

        public VegPoint VegPoint;

        public SugarPoint SugarPoint;

        public SaltPoint SaltPoint;

        public VinePoint VinePoint;

        public SoyPoint SoyPoint;

        public MisoPoint MisoPoint;


        public Food (
            MeatPoint tmpMeatPoint,
            FishPoint tmpFishPoint,
            VegPoint tmpVegPoint,
            SugarPoint tmpSugarPoint,
            SaltPoint tmpSaltPoint,
            VinePoint tmpVinePoint,
            SoyPoint tmpSoyPoint,
            MisoPoint tmpMisoPoint
        )
        {
            MeatPoint = tmpMeatPoint;
            FishPoint = tmpFishPoint;
            VegPoint = tmpVegPoint;
            SugarPoint = tmpSugarPoint;
            SaltPoint = tmpSaltPoint;
            VinePoint = tmpVinePoint;
            SoyPoint = tmpSoyPoint;
            MisoPoint = tmpMisoPoint;
        }
    }
}
