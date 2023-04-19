using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Title
{
    public class TitleTextData
    {
            // 料理データを保存する配列
            public static List<string[]> TextData{get;} = new List<string[]>();
            
            // CSVデータアセットを保管する変数
            public TextAsset LoadedAsset{get; private set;}

            // データ取得クラス
            private GetData getData;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="data">データ取得クラスのインスタンス</param>
            public TitleTextData(GetData data)
            {
                getData = data;
            }

            /// <summary>
            /// CSVデータをロード
            /// </summary>
            /// <returns></returns>
            public async UniTask LoadData()
            {
                LoadedAsset = await getData.LoadAsset("TextData");
                await UniTask.WaitWhile(() => LoadedAsset == null);
            }

            /// <summary>
            /// CSVデータからリストに読み込むメソッド
            /// </summary>
            /// <param name="getData"></param>
            public void GetDishData()
            {
                getData.ReadData(LoadedAsset, TextData);
            }
        
    }
}

