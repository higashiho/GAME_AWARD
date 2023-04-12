using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class MeatPoint : BaseFoodPoint
    {
        public MeatPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.MEAT;
        }

        //値を増やすメソッド
        public MeatPoint Add(MeatPoint addAmount)
        {
            //インスタンス生成
            return new MeatPoint(Amount + addAmount.Amount);
        }

        
    }


    public class FishPoint : BaseFoodPoint
    {

        public FishPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.FISH;
        }

        //値を増やすメソッド
        public FishPoint Add(FishPoint addAmount)
        {
            //インスタンス生成
            return new FishPoint(Amount + addAmount.Amount);
        }
    }


    public class VegPoint : BaseFoodPoint
    {
        public VegPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.VEG;
        }

        //値を増やすメソッド
        public VegPoint Add(VegPoint addAmount)
        {
            //インスタンス生成
            return new VegPoint(Amount + addAmount.Amount);
        }
    }


    public class SugarPoint : BaseFoodPoint
    {

        public SugarPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.SUGAR;
        }

        //値を増やすメソッド
        public SugarPoint Add(SugarPoint addAmount)
        {
            //インスタンス生成
            return new SugarPoint(Amount + addAmount.Amount);
        }
    }


    public class SaltPoint : BaseFoodPoint
    {
        public SaltPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.SALT;
        }

        //値を増やすメソッド
        public SaltPoint Add(SaltPoint addAmount)
        {
            //インスタンス生成
            return new SaltPoint(Amount + addAmount.Amount);
        }
    }


    public class VinePoint : BaseFoodPoint
    {
        public VinePoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.VINE;
        }

        //値を増やすメソッド
        public VinePoint Add(VinePoint addAmount)
        {
            //インスタンス生成
            return new VinePoint(Amount + addAmount.Amount);
        }
    }


    public class SoyPoint : BaseFoodPoint
    {
        public SoyPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.SOY;
        }

        //値を増やすメソッド
        public SoyPoint Add(SoyPoint addAmount)
        {
            //インスタンス生成
            return new SoyPoint(Amount + addAmount.Amount);
        }
    }


    public class MisoPoint : BaseFoodPoint
    {
        public MisoPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.MISO;
        }

        //値を増やすメソッド
        public MisoPoint Add(MisoPoint addAmount)
        {
            //インスタンス生成
            return new MisoPoint(Amount + addAmount.Amount);
        }
    }
}