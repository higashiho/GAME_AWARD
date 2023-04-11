using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class MeatPoint //肉ポイント
    {
        private int meatPointAmount;

        public MeatPoint(int amount)
        {
            //値の初期値
            meatPointAmount = amount;
        }

        //値を増やすメソッド
        public MeatPoint Add(MeatPoint addAmount)
        {
            //インスタンス生成
            return new MeatPoint(meatPointAmount + addAmount.meatPointAmount);
        }
    }


    public class FishPoint //魚ポイント
    {
        private int fishPointAmount;

        public FishPoint(int amount)
        {
            //値の初期値
            fishPointAmount = amount;
        }

        //値を増やすメソッド
        public FishPoint Add(FishPoint addAmount)
        {
            //インスタンス生成
            return new FishPoint(fishPointAmount + addAmount.fishPointAmount);
        }
    }


        public class VegPoint //野菜ポイント
    {
        private int vegPointAmount;

        public VegPoint(int amount)
        {
            //値の初期値
            vegPointAmount = amount;
        }

        //値を増やすメソッド
        public VegPoint Add(VegPoint addAmount)
        {
            //インスタンス生成
            return new VegPoint(vegPointAmount + addAmount.vegPointAmount);
        }
    }


    public class SugarPoint //砂糖ポイント
    {
        private int sugarPointAmount;

        public SugarPoint(int amount)
        {
            //値の初期値
            sugarPointAmount = amount;
        }

        //値を増やすメソッド
        public SugarPoint Add(SugarPoint addAmount)
        {
            //インスタンス生成
            return new SugarPoint(sugarPointAmount + addAmount.sugarPointAmount);
        }
    }


    public class SaltPoint //塩ポイント
    {
        private int saltPointAmount;

        public SaltPoint(int amount)
        {
            //値の初期値
            saltPointAmount = amount;
        }

        //値を増やすメソッド
        public SaltPoint Add(SaltPoint addAmount)
        {
            //インスタンス生成
            return new SaltPoint(saltPointAmount + addAmount.saltPointAmount);
        }
    }


    public class VinePoint //酢ポイント
    {
        private int vinePointAmount;

        public VinePoint(int amount)
        {
            //値の初期値
            vinePointAmount = amount;
        }

        //値を増やすメソッド
        public VinePoint Add(VinePoint addAmount)
        {
            //インスタンス生成
            return new VinePoint(vinePointAmount + addAmount.vinePointAmount);
        }
    }


    public class SoyPoint //醤油ポイント
    {
        private int soyPointAmount;

        public SoyPoint(int amount)
        {
            //値の初期値
            soyPointAmount = amount;
        }

        //値を増やすメソッド
        public SoyPoint Add(SoyPoint addAmount)
        {
            //インスタンス生成
            return new SoyPoint(soyPointAmount + addAmount.soyPointAmount);
        }
    }


        public class MisoPoint //味噌ポイント
    {
        private int misoPointAmount;

        public MisoPoint(int amount)
        {
            //値の初期値
            misoPointAmount = amount;
        }

        //値を増やすメソッド
        public MisoPoint Add(MisoPoint addAmount)
        {
            //インスタンス生成
            return new MisoPoint(misoPointAmount + addAmount.misoPointAmount);
        }
    }
}