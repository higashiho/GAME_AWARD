using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class SoyPoint
    {
        private int soyPointAmount;

        public SoyPoint(int amount)
        {
            //�l�̏����l
            soyPointAmount = amount;
        }

        //�l�𑝂₷���\�b�h
        public SoyPoint Add(SoyPoint addAmount)
        {
            //�C���X�^���X����
            return new SoyPoint(soyPointAmount + addAmount.soyPointAmount);
        }
    }
}
