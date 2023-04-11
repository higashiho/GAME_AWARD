using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class SugarPoint
    {
        private int sugarPointAmount;

        public SugarPoint(int amount)
        {
            //�l�̏����l
            sugarPointAmount = amount;
        }

        //�l�𑝂₷���\�b�h
        public SugarPoint Add(SugarPoint addAmount)
        {
            //�C���X�^���X����
            return new SugarPoint(sugarPointAmount + addAmount.sugarPointAmount);
        }
    }
}
