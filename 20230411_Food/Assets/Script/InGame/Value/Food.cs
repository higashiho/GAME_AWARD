using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;
using System;
namespace Food
{
    public class FoodData
    {
        private DishData dishData;

        public enum DataIndex{
            DishName = 0,
            ExplanatoryText = 1,
            MeatPoint = 2,
            FishPoint = 3,
            VegPoint = 4,
            LevelOfSatietyPoint = 5
        }

        public Dictionary<DataIndex, string> DishPoints{get; private set;} = new Dictionary<DataIndex,string>(6);
        private string[] getData = new string[6]; 
        public FoodData (DishData Data, int dishId)
        {
            dishData = Data;

            GetDishData(dishId);
        }

        private void GetDishData(int dishId)
        {
            getData = DishData.DishPointData[dishId];

            for(int i = 0;i < Enum.GetValues(typeof(DataIndex)).Length;i++)
            {
                DishPoints.Add((DataIndex)Enum.ToObject(typeof(DataIndex),i), getData[i]);
            }
        }
    }
}