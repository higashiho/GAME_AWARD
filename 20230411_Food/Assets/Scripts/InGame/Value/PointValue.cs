using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    /// <summary>
    /// 肉ポイントのVlueObject
    /// </summary>
    public class MeatPoint :  BaseFoodPoint
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpAmount">インスタンス化するポイント</param>
        public MeatPoint(int tmpAmount)
        {
            if(tmpAmount < 0)
            {
                Debug.LogError("渡された値は負の値です。");
                return;
            }
            //値の初期値
            Amount = tmpAmount;
        }

        /// <summary>
        /// 肉ポイントの値を加算するメソッド
        /// </summary>
        /// <param name="addAmount">追加するポイント</param>
        /// <returns>ポイント加算後の値を持った肉ポイントのインスタンス</returns>
        public MeatPoint Add(MeatPoint addAmount)
        {
            //インスタンス生成
            return new MeatPoint(Amount + addAmount.Amount);
        }

        
    }

    /// <summary>
    /// 魚ポイントのValueObject
    /// </summary>
    public class FishPoint : BaseFoodPoint
    {
        /// <summary>
        /// 魚ポイントのコンストラクタ
        /// </summary>
        /// <param name="tmpAmount">インスタンス化するポイント</param>
        public FishPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.FISH;
        }

        /// <summary>
        /// 魚ポイントの値を加算するメソッド
        /// </summary>
        /// <param name="addAmount">追加するポイント</param>
        /// <returns>ポイント加算後の値を持った魚ポイントのインスタンス</returns>
        public FishPoint Add(FishPoint addAmount)
        {
            //インスタンス生成
            return new FishPoint(Amount + addAmount.Amount);
        }
    }

    /// <summary>
    /// 野菜ポイントのValueObject
    /// </summary>
    public class VegPoint : BaseFoodPoint
    {
        /// <summary>
        /// 野菜ポイントのコンストラクタ
        /// </summary>
        /// <param name="tmpAmount">インスタンス化するポイント</param>
        public VegPoint(int tmpAmount)
        {
            //値の初期値
            Amount = tmpAmount;
            PointName = FoodPointName.VEG;
        }

        /// <summary>
        /// 魚ポイントの値を加算するメソッド
        /// </summary>
        /// <param name="addAmount">追加する値</param>
        /// <returns>ポイント加算後の値を持った野菜ポイントのインスタンス</returns>
        public VegPoint Add(VegPoint addAmount)
        {
            //インスタンス生成
            return new VegPoint(Amount + addAmount.Amount);
        }
    }

    /// <summary>
    /// 調味料ポイントのValueObject
    /// </summary>
    public class SeasousingPoint : BaseFoodPoint
    {
        // ポイント
        //public int Amount{get; private set;}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpAmount">インスタンス化するポイント</param>
        public SeasousingPoint(int tmpAmount)
        {
            if(tmpAmount < 0)
            {
                Debug.LogError("Seasousingに渡された値が負です");
                return;
            }

            Amount = tmpAmount;
        }

        /// <summary>
        /// 調味料ポイントを追加するメソッド
        /// 調味料ポイントは１０より大きな値は返さない
        /// </summary>
        /// <param name="addAmount">追加するポイントの量</param>
        /// <returns>加算後の値を持った調味料ポイントのインスタンス</returns>
        public SeasousingPoint Add(SeasousingPoint addAmount)
        {
            // 現在のポイントと追加するポイントの合計値を計算
            int val = this.Amount + addAmount.Amount;

            // 値が１０より大きい場合
            if(val > 10)
            {
                return new SeasousingPoint(10);
            }
            else
            {
                return new SeasousingPoint(val);
            }
            
        }
    }

    /// <summary>
    /// 満腹度ポイントのValueObject
    /// </summary>
    public class LevelOfSatiety : BaseFoodPoint
    {
        /// <summary>
        /// 満腹度ポイントのコンストラクタ
        /// </summary>
        /// <param name="tmpAmount"></param>
        public LevelOfSatiety(int tmpAmount)
        {
            if(tmpAmount < 0)
            {
                Debug.LogError("LebelOfSatietyポイントに渡された値が負です");
                return;
            }

            this.Amount = tmpAmount;
        }

        /// <summary>
        /// 満腹度ポイントを追加するメソッド
        /// </summary>
        /// <param name="addAmount">追加する満腹度ポイント</param>
        /// <returns>加算後の値を持った満腹度ポイントのインスタンス</returns>
        public LevelOfSatiety Add(LevelOfSatiety addAmount)
        {
            return new LevelOfSatiety(this.Amount + addAmount.Amount);
        }
    }
}