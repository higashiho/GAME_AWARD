using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class FoodPoint
    {
        public MeatPoint Mep;//���|�C���g
        public FishPoint Fp;//���|�C���g
        public VegPoint Vep;//��؃|�C���g
        public SugarPoint Sup;//�����|�C���g
        public SaltPoint Sap;//���|�C���g
        public VinePoint Vip;//�|�|�C���g
        public SoyPoint Sop;//�ݖ��|�C���g
        public MisoPoint Mip;//���X�|�C���g

        public FoodPoint()
        {
            //�|�C���g�����l
            Mep = new MeatPoint(0);
            Fp = new FishPoint(0);
            Vep = new VegPoint(0);
            Sup = new SugarPoint(0);
            Sap = new SaltPoint(0);
            Vip = new VinePoint(0);
            Sop = new SoyPoint(0);
            Mip = new MisoPoint(0);
        }
    }
}
