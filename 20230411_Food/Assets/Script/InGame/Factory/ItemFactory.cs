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
        private ItemData itemData;
        
        /// <summary>
        /// アイテム出現座標リスト
        /// </summary>
        /// <typeparam name="ItemPosData">アイテムの座標データ</typeparam>
        /// <returns></returns>
        private List<ItemPosData> itemPos = new List<ItemPosData>(16);

        // ロードしたプレファブを入れるList
        // アイテムをプールしておく
        private List<GameObject> poolList = new List<GameObject>(16);

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
                await UniTask.WhenAll((UniTask)loadTask);
            
                
                // アイテムを生成
                for(int i = 0; i < itemData.ItemPopNum; i++)
                {
                    Create();
                }
                    
                // プレイヤーに食べ物を取得したときのイベントを登録
                for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
                {
                    ObjectManager.PlayerManagers[i].FoodPoint.ReturnPresentItemPos += ReturnEmptyItemPos;
                }
            }
        }

        /// <summary>
        /// アイテムをリポップさせるメソッド
        /// </summary>
        public void Create()
        {
            
            
            GameObject obj = poolList[0];
            poolList.RemoveAt(0);
            
            // アイテム座標リストシャッフル
            itemPos = itemPos.OrderBy(a => Guid.NewGuid()).ToList();

            // 座標リストから空座標のデータを取得
            ItemPosData data = itemPos.Find(item => !item.GetAttend());

            int index = itemPos.IndexOf(data);
            // 生成座標設定
            obj.transform.position = itemPos[index].Pos;

            // アイテム生成フラグON
            itemPos[index].SetAttend(true);
            

            // アクティブ化
            obj.SetActive(true);
            
            
            
        }

        /// <summary>
        /// アイテムをプーリングするメソッド
        /// </summary>
        public void Storing(GameObject obj)
        {
            // プールのQueueに入れる
            poolList.Add(obj);
            obj.SetActive(false);
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
                var obj = MonoBehaviour.Instantiate(item);
                obj.SetActive(false);
                poolList.Add(obj);
            }

            var dataHandle = Addressables.LoadAssetAsync<ItemData>("ItemData");
            await dataHandle.Task;

            itemData = dataHandle.Result;
        }

        /// <summary>
        /// アイテムが取得された時にそのアイテムの座標を保管Queueに返すメソッド
        /// </summary>
        /// <param name="pos">アイテムの座標</param>
        public void ReturnEmptyItemPos(object sender, ReturnPresentPosEventArgs e)
        {   
            
            // プレイヤーが取得したアイテムの座標データ取得
            var data = itemPos.Find(item => item.Pos == e.presentPos);
            int index = itemPos.IndexOf(data);
            
            // アイテム出現フラグOFF
            data.SetAttend(false);
            itemPos[index] = data;
            CreateItem();
        }

        /// <summary>
        /// アイテムのリポップメソッド
        /// </summary>
        /// <returns></returns>
        public async void CreateItem()
        {
            
            // 表示されているアイテムが8個未満の場合
            int num = itemPos.Count(item => item.GetAttend());
            if(num >= itemData.ItemPopNum) return;
            
            await UniTask.Delay(itemData.RipopInterval);
            Create();
            
        }
        public void DebugText()
        {
            if(Input.GetKeyDown("space"))
            {
                for(int i = 0; i < itemPos.Count; i++)
                {
                    if(itemPos[i].GetAttend())
                    {
                        Debug.Log(itemPos[i].Pos + " : アイテム生成座標");
                    }

                }
            }
        }


        /// <summary>
        /// 生成座標を作るメソッド
        /// </summary>
        private void makePopPosArr()
        {   
            // 座標を入れていく
            for(int i = 0; i < posRow; i++)
            {
                for(int j = 0; j < posCol; j++)
                {
                    // アイテム座標管理リストに追加
                    // 生成座標, アイテムが生成されているかのフラグ
                    itemPos.Add(new ItemPosData(new Vector3(baseLineX + i * spaceX, 1.0f, baseLineZ + j * spaceZ), false));
                }
            }   
        }
        
    }

    /// <summary>
        /// アイテムの座標データ
        /// </summary>
    class ItemPosData
    {
        public ItemPosData(Vector3 pos, bool attend)
        {
            this.Pos = pos;
            this.attend = attend; 
        }
        // 座標
        public Vector3 Pos{get; private set;}
        // アイテムが出現しているか
        private bool attend;
        public bool GetAttend(){return attend;}
        public void SetAttend(bool value){attend = value;}

    }

}

