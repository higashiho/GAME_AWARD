using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class SaltPoint
    {
        private int saltPointAmount;

        public SaltPoint(int amount)
        {
            //�l�̏����l
            saltPointAmount = amount;
        }

        //�l�𑝂₷���\�b�h
        public SaltPoint Add(SaltPoint addAmount)
        {
            //�C���X�^���X����
            return new SaltPoint(saltPointAmount + addAmount.saltPointAmount);
        }
    }
}
