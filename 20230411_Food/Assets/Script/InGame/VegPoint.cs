using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class VegPoint
    {
        private int vegPointAmount;

        public VegPoint(int amount)
        {
            //�l�̏����l
            vegPointAmount = amount;
        }

        //�l�𑝂₷���\�b�h
        public VegPoint Add(VegPoint addAmount)
        {
            //�C���X�^���X����
            return new VegPoint(vegPointAmount + addAmount.vegPointAmount);
        }
    }
}
