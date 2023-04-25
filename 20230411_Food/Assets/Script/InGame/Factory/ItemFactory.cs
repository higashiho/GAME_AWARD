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
    // 座標配列 => ランダム座標に〇個生成
    // 出現アイテムが取得される => 空座標からランダムに生成座標を決定
    // 生成座標にランダムなアイテムを表示

    // アイテム全部をインスタンス化してない問題


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

        // 出現しているアイテムのリスト
        private List<GameObject> dispObj = new List<GameObject>(8);
        
        /// <summary>
        /// アイテム出現座標リスト
        /// </summary>
        /// <typeparam name="ItemPosData"></typeparam>
        /// <returns></returns>
        private List<ItemPosData> itemPos = new List<ItemPosData>(16);

        // ロードしたプレファブを入れるList
        // アイテムをプールしておく
        private List<GameObject> loadPrefab = new List<GameObject>(16);

        // プレファブをロードするタスク
        private UniTask? loadTask = null;

        // アイテムを生成する間隔
        private float spaceX = 10.0f;
        private float spaceZ = 3.0f;

        // アイテムを生成する基準ライン
        private float baseLineX = -15.0f;
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

                // for(int i = 0; i < 8; i++)
                // {
                //     // 生成座標データ取得
                //     ItemPosData data = itemPos[i];
        
                //     // アイテム生成フラグON
                //     data.attend = true;
                //     // アイテム生成
                //     setItem(data.pos);
                // }
                for(int i = 0; i < 8; i++)
                {
                    Create();
                }
                    
                
                        
                    
                ObjectManager.Player.FoodPoint.ReturnPresentItemPos += ReturnEmptyItemPos;

            }
        }

        /// <summary>
        /// アイテムをリポップさせるメソッド
        /// ステージのアイテムが８個未満になった時呼ばれるメソッド
        /// </summary>
        public void Create()
        {
            loadPrefab = loadPrefab.OrderBy(a => Guid.NewGuid()).ToList();
            // 生成アイテムをアイテムリストから取得
            GameObject obj = loadPrefab[0];
            loadPrefab.RemoveAt(0);

            itemPos = itemPos.OrderBy(a => Guid.NewGuid()).ToList();
            // 座標リストから空座標のデータを取得
            ItemPosData data = itemPos.Find(item => !item.attend);
            Debug.Log(data.pos);
            // 生成座標設定
            obj.transform.position = data.pos;

            // アイテム生成フラグON
            data.attend = true;

            // アクティブ化
            obj.SetActive(true);
            
        }

        /// <summary>
        /// ゲーム開始時にアイテムをセットするメソッド
        /// </summary>
        /// <param name="pos">アイテムを生成する座標</param>
        private void setItem(Vector3 pos)
        {
            // アイテムプレファブの先頭の要素を抽出
            GameObject obj = loadPrefab[0];

            // 生成座標設定
            obj.transform.position = pos;
            // FoodPoint設定

            // 生成
            obj.SetActive(true);
            
                
        }


        /// <summary>
        /// アイテムをプーリングするメソッド
        /// </summary>
        public void Storing(GameObject obj)
        {
            // プールのQueueに入れる
            loadPrefab.Add(obj);

            // プレイヤーがアイテムを取得したときのイベントを登録
            //ObjectManager.Player.FoodPoint.ReturnPresentItemPos += ReturnEmptyItemPos;
            
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
        public async void ReturnEmptyItemPos(object sender, ReturnPresentPosEventArgs e)
        {
            
            // プレイヤーが取得したアイテムの座標データ取得
            var index = itemPos.Find(item => item.pos == e.presentPos);

            // アイテム出現フラグOFF
            index.attend = false;

            //await Task.Delay(5 * 1000);
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
            // Debug.Log(num);
            if(num < 8)
            {
                // アイテム生成
                Create();
            }
            
            
        }

        
    }
}

