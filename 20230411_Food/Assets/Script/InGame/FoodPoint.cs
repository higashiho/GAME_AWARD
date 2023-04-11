using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class FoodPoint
    {
        public MeatPoint Mep;//肉ポイント
        public FishPoint Fp;//魚ポイント
        public VegPoint Vep;//野菜ポイント
        public SugarPoint Sup;//砂糖ポイント
        public SaltPoint Sap;//塩ポイント
        public VinePoint Vip;//酢ポイント
        public SoyPoint Sop;//醤油ポイント
        public MisoPoint Mip;//味噌ポイント

        public FoodPoint()
        {
            //ポイント初期値
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
