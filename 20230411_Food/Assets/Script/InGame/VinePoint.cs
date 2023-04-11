using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class VinePoint
    {
        private int vinePointAmount;

        public VinePoint(int amount)
        {
            //�l�̏����l
            vinePointAmount = amount;
        }

        //�l�𑝂₷���\�b�h
        public VinePoint Add(VinePoint addAmount)
        {
            //�C���X�^���X����
            return new VinePoint(vinePointAmount + addAmount.vinePointAmount);
        }
    }
}
