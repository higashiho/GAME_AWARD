using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class MisoPoint
    {
        private int misoPointAmount;

        public MisoPoint(int amount)
        {
            //�l�̏����l
            misoPointAmount = amount;
        }

        //�l�𑝂₷���\�b�h
        public MisoPoint Add(MisoPoint addAmount)
        {
            //�C���X�^���X����
            return new MisoPoint(misoPointAmount + addAmount.misoPointAmount);
        }
    }
}
