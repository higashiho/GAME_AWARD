using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class FishPoint
    {
        private int fishPointAmount;

        public FishPoint(int amount)
        {
            //�l�̏����l
            fishPointAmount = amount;
        }

        //�l�𑝂₷���\�b�h
        public FishPoint Add(FishPoint addAmount)
        {
            //�C���X�^���X����
            return new FishPoint(fishPointAmount + addAmount.fishPointAmount);
        }
    }
}
