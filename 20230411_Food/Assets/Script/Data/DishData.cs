using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace FoodPoint
{
    public class DishData 
    {
        // 料理データを保存する配列
        public static List<string[]> DishPointData{get;} = new List<string[]>();
        
        // CSVデータアセットを保管する変数
        public TextAsset LoadedAsset{get; private set;}

        private GetData getData;

        public DishData(ref GetData data)
        {
            getData = data;
        }

        /// <summary>
        /// CSVデータをロード
        /// </summary>
        /// <param name="getData"></param>
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
        public void GetDishData(GetData getData)
        {
            getData.ReadData(LoadedAsset, DishPointData);
        }
    }
}

