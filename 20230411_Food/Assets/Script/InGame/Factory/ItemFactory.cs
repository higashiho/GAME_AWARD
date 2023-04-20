using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;

namespace Item
{
    /// <summary>
    /// アイテムの生成を担当するクラス
    /// </summary>
    public class ItemFactory 
    {
        // ロードしたプレファブを入れるList
        // アイテムをプールしておく
        private List<GameObject> loadPrefab = new List<GameObject>(16);

        // プレファブをロードするタスク
        private UniTask? loadTask = null;

        // アイテムの生成座標を保管する配列
        // -4.5, -1.5, 1.5, 4.5
        private Vector3[,] itemPopPos;

        // アイテムを生成する間隔
        private float spaceX = 10.0f;
        private float spaceZ = 3.0f;

        // アイテムを生成する基準ライン
        private float baseLineX = -15.0f;
        private float baseLineZ = -4.5f;
        
        
        /// <summary>
        /// ItemFactoryのコンストラクタ
        /// </summary>
        public ItemFactory()
        {
            // 生成座標配列作成
            makePopPosArr();
        }

        /// <summary>
        /// ステージのアイテムをセットするメソッド
        /// ゲーム開始時の一度だけ呼ばれる
        /// </summary>
        public async void InitItem()
        {
            // アイテムをロード
            if(loadTask == null)
            {
                // アイテムロードタスクにロード処理を入れる
                loadTask = load();
                // ロードタスクが終わるのを待つ
                await UniTask.WhenAny((UniTask)loadTask);
            
                // => ロードより先に呼ばれてる問題
                // シャッフル
                loadPrefab = loadPrefab.OrderBy(a => Guid.NewGuid()).ToList();

                // ステージにアイテムを生成
                for(int i = 0; i < itemPopPos.GetLength(0); i++)
                {
                    for(int j = 0; j < itemPopPos.GetLength(1); j++)
                    {
                        setItem(itemPopPos[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// アイテムをリポップさせるメソッド
        /// </summary>
        public void Create(Vector3 pos)
        {
            // プールからDequeue
            Debug.Log(loadPrefab[0]);
            GameObject obj = loadPrefab[0];//pool.Dequeue();
            //loadPrefab.RemoveAt(0);
            // 生成座標設定
            obj.transform.position = pos;

            // アクティブ化
            obj.SetActive(true);
        }

        private void setItem(Vector3 pos)
        {
            GameObject obj = loadPrefab[0];
            //loadPrefab.RemoveAt(0);

            // 生成座標設定
            obj.transform.position = pos;
            // FoodPoint設定

            // 生成
            MonoBehaviour.Instantiate(obj);
                
            
        }


        /// <summary>
        /// アイテムをプーリングするメソッド
        /// </summary>
        public void Storing(GameObject obj)
        {
            // プールのQueueに入れる
            loadPrefab.Add(obj);
        }


        /// <summary>
        /// アイテムを生成する座標を作る
        /// </summary>
        private void makePopPosArr()
        {
            // アイテムの生成座標配列インスタンス化
            itemPopPos = new Vector3[4,4];
            
            // 座標を入れていく
            for(int i = 0; i < itemPopPos.GetLength(0); i++)
            {
                for(int j = 0; j < itemPopPos.GetLength(1); j++)
                {
                    itemPopPos[i,j] = new Vector3(baseLineX + i * spaceX, 1.0f, baseLineZ + j * spaceZ);
                }
            }
        }

        /// <summary>
        /// アイテムをロードするメソッド
        /// </summary>
        private async UniTask load()
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>("Ingredient", null);
            await handle.Task;
            foreach(var item in handle.Result)
            { 
                loadPrefab.Add(item);
            }

        }

    }
}

