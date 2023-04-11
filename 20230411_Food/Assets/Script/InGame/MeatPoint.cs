using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class MeatPoint
    {
        private int meatPointAmount;

        public MeatPoint(int amount)
        {
            //�l�̏����l
            meatPointAmount = amount;
        }

        //�l�𑝂₷���\�b�h
        public MeatPoint Add(MeatPoint addAmount)
        {
            //�C���X�^���X����
            return new MeatPoint(meatPointAmount + addAmount.meatPointAmount);
        }
    }
}
