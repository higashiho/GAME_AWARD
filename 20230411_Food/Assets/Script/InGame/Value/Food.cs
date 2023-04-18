using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;
namespace Food
{
    public class Food
    {
        private DishData dishData;

        private enum DataIndex{
            DishName = 0,
            ExplanatoryText = 1,
            MeatPoint = 2,
            FishPoint = 3,
            VegPoint = 4,
            LevelOfSatietyPoint = 5
        }
        private Dictionary <DataIndex,string> dishPoints = new Dictionary<DataIndex,string>(6);

        public Food (DishData Data, int dishId)
        {
            dishData = Data;

            GetDishData(dishId);
        }

        public void GetDishData(int dishId)
        {
            getData = dishData.DishPointData[dishId];

            for(int i = 0;i < Enum.GetValues(typeof(DataIndex)).Length;i++)
            {
                dishPoints.Add((DataIndex)Enum.ToObject(typeof(DataIndex),i),getData(i))
            }
        }
    }
}