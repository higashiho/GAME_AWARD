using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Linq;
using System;
using Player;
using GameManager;

namespace Item
{   
    /// <summary>
    /// アイテムの生成を担当するクラス
    /// </summary>
    public class ItemFactory 
    {
        /// <summary>
        /// アイテムの座標データ
        /// </summary>
        struct ItemPosData
        {
            // 座標
            public Vector3 pos;
            // アイテムが出現しているか
            public bool attend;
        }
        
        /// <summary>
        /// アイテム出現座標リスト
        /// </summary>
        /// <typeparam name="ItemPosData">アイテムの座標データ</typeparam>
        /// <returns></returns>
        private List<ItemPosData> itemPos = new List<ItemPosData>(16);

        // ロードしたプレファブを入れるList
        // アイテムをプールしておく
        private List<GameObject> loadPrefab = new List<GameObject>(16);

        // プレファブをロードするタスク
        private UniTask? loadTask = null;

        // アイテムを生成する間隔
        private float spaceX = 8.0f;
        private float spaceZ = 3.0f;

        // 11.5, 4.0, 4.0, 11.5
        // アイテムを生成する基準ライン
        private float baseLineX = -12.0f;
        private float baseLineZ = -4.5f;

        private int posRow = 4;
        private int posCol = 4;

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

                // 生成座標リストをシャッフル
                itemPos = itemPos.OrderBy(a => Guid.NewGuid()).ToList();

                
                for(int i = 0; i < 8; i++)
                {
                    Create();
                }
                    
                
                for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
                {
                    ObjectManager.PlayerManagers[i].FoodPoint.ReturnPresentItemPos += ReturnEmptyItemPos;
                }
            }
        }

        /// <summary>
        /// アイテムをリポップさせるメソッド
        /// ステージのアイテムが８個未満になった時呼ばれるメソッド
        /// </summary>
        public void Create()
        {
            //loadPrefab = loadPrefab.OrderBy(a => Guid.NewGuid()).ToList();
            // 生成アイテムをアイテムリストから取得
            GameObject obj = loadPrefab[0];
            loadPrefab.RemoveAt(0);

            itemPos = itemPos.OrderBy(a => Guid.NewGuid()).ToList();
            // 座標リストから空座標のデータを取得
            ItemPosData data = itemPos.Find(item => !item.attend);

            // 生成座標設定
            obj.transform.position = data.pos;

            // アイテム生成フラグON
            data.attend = true;

            // アクティブ化
            obj.SetActive(true);
            
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
            // 座標を入れていく
            for(int i = 0; i < posRow; i++)
            {
                for(int j = 0; j < posCol; j++)
                {
                    // インスタンス化
                    ItemPosData data = new ItemPosData();
                    // 座標設定
                    data.pos = new Vector3(baseLineX + i * spaceX, 1.0f, baseLineZ + j * spaceZ);
                    
                    // アイテム生成フラグOFF
                    data.attend = false;
                    // アイテム座標管理リストに追加
                    itemPos.Add(data);
                }
            }   
        }

        /// <summary>
        /// アイテムをロードするメソッド
        /// </summary>
        private async UniTask load()
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>("Ingredients", null);
            
            await handle.Task;
            foreach(var item in handle.Result)
            {   
                MonoBehaviour.Instantiate(item);
                item.SetActive(false);
                loadPrefab.Add(item);
            }
        }

        /// <summary>
        /// アイテムが取得された時にそのアイテムの座標を保管Queueに返すメソッド
        /// </summary>
        /// <param name="pos">アイテムの座標</param>
        public void ReturnEmptyItemPos(object sender, ReturnPresentPosEventArgs e)
        {
            
            // プレイヤーが取得したアイテムの座標データ取得
            var index = itemPos.Find(item => item.pos == e.presentPos);

            // アイテム出現フラグOFF
            index.attend = false;

            CreateItem();
        }


        /// <summary>
        /// アイテムのリポップメソッド
        /// </summary>
        /// <returns></returns>
        public void CreateItem()
        {
            // 表示されているアイテムが8個未満の場合
            int num = itemPos.Count(item => item.attend);
            if(num < 8)
            {
                // アイテム生成
                Create();
            }
            
            
        }

        
    }
}

