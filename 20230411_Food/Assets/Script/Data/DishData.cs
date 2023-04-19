using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace FoodPoint
{   
    /// <summary>
    /// 料理データを管理するクラス
    /// </summary>
    public class DishData 
    {
        // 料理データを保存する配列
        public static List<string[]> DishPointData{get;} = new List<string[]>();
        
        // CSVデータアセットを保管する変数
        public TextAsset LoadedAsset{get; private set;}

        // データ取得クラス
        private GetData getData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">データ取得クラスのインスタンス</param>
        public DishData(GetData data)
        {
            getData = data;
        }

        /// <summary>
        /// CSVデータをロード
        /// </summary>
        /// <returns></returns>
        public async UniTask LoadData()
        {
            
            LoadedAsset = await getData.LoadAsset("DishPointDatas");
            await UniTask.WaitWhile(() => LoadedAsset == null);
        }

        /// <summary>
        /// CSVデータからリストに読み込むメソッド
        /// </summary>
        /// <param name="getData"></param>
        public void GetDishData()
        {
            getData.ReadData(LoadedAsset, DishPointData);
        }
    }
}

