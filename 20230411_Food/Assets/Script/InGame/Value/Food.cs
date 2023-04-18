using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;
namespace Food
{
    public class Food
    {
        private DishData dishData;

        private BaseFoodPoint[] FoodPoints = new BaseFoodPoint[4];

        public Food (DishData Data, int dishId)
        {
            dishData = Data;

            GetDishData(dishId);
        }

        public void GetDishData(int dishId)
        {
            getData = dishData.DishPointData[dishId];

            FoodPoints[0] = new MeatPoint(int.Parse(getData[2]));
            FoodPoints[1] = new FishPoint(int.Parse(getData[3]));
            FoodPoints[2] = new VegPoint(int.Parse(getData[4]));
            FoodPoints[3] = new LebelOfSatiety(int.Parse(getData[5]));
        }
    }
}