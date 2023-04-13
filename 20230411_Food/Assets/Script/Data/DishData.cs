using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodPoint
{
    public class DishData 
    {
        // 料理データを保存する配列
        public List<string[]> DishPointData = new List<string[]>();
        
        // CSVデータアセットを保管する変数
        public TextAsset LoadedAsset{get; private set;}

        public DishData()
        {
            
        }

        /// <summary>
        /// CSVデータをロード
        /// </summary>
        /// <param name="getData"></param>
        /// <returns></returns>
        public async void LoadData(GetData getData)
        {
            await getData.LoadAsset("FishDataPoint", LoadedAsset);
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

