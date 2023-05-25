using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        // アイテムを生成するときの親オブジェクト
        private GameObject parent;

        // 皿オブジェクト配列
        private Queue<GameObject> plateList = new Queue<GameObject>(5);
        // 皿オブジェクト
        private GameObject tmpPlate;
        /// <summary>
        /// アイテム出現座標リスト
        /// </summary>
        /// <typeparam name="ItemPosData">アイテムの座標データ</typeparam>
        /// <returns></returns>
        private List<ItemPosData> itemPos = new List<ItemPosData>(16);

        // ロードしたプレファブを入れるList
        // アイテムをプールしておく
        private List<GameObject> poolList = new List<GameObject>(10);

        // プレファブをロードするタスク
        private UniTask? loadTask = null;

        // アイテムを生成する間隔
        private float spaceX = 10.0f;
        private float spaceZ = 3.0f;

        // アイテムを生成する基準ライン
        private float baseLineX = -15.0f;
        private float baseLineZ = -4.5f;

        // アイテムの配置posの行、列
        private int posRow = 4;
        private int posCol = 4;

        // ハンドルリリースイベント
        public UnityAction ReleaseHandleEvent{get; private set;}


        /// <summary>
        /// ItemFactoryのコンストラクタ
        /// </summary>
        public ItemFactory()
        {
            // 生成座標配列作成
            makePopPosArr();

            // 親オブジェクトを設定
            parent = GameObject.Find("Item");
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

                for(int i = 0; i < itemPos.Count; i++)
                {
                    MonoBehaviour.Instantiate(tmpPlate, itemPos[i].Pos - new Vector3(0, 0.25f, 0), Quaternion.identity);
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
            // プールリストの中身が空の場合early return 
//            Debug.Log(poolList[0] + " : プールリストの先頭の要素");
            // プールリストの最初の要素を取得
            GameObject obj = poolList[0];
            
            // // その要素をリストから削除
            poolList.RemoveAt(0);
            
            
            // アイテム座標リストシャッフル
            itemPos = itemPos.OrderBy(a => Guid.NewGuid()).ToList();

            // 座標リストから空座標のデータを取得
            ItemPosData data = itemPos.Find(item => !item.GetAttend());

            // 座標リストのインデックスを取得
            int index = itemPos.IndexOf(data);

            // 生成座標設定
            obj.transform.position = itemPos[index].Pos;

            // アイテム生成フラグON
            itemPos[index].SetAttend(true);
            

            
            
            // 皿の座標調整
            
            
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
            // アイテムがあった座標のItemPosDataを取得
            var data = itemPos.Find(item => item.Pos == obj.transform.position);
            // 配列上のインデックスを取得
            int index = itemPos.IndexOf(data);
            // アイテム生成パラメータをオフ
            data.SetAttend(false);
            // 配列にそのデータを返す
            itemPos[index] = data;
            // アイテムオブジェクトを非アクティブにする
            obj.SetActive(false);
        }
        

        /// <summary>
        /// アイテムをロードするメソッド
        /// </summary>
        private async UniTask load()
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>("Ingredients", null);

            void releaseHandle()
            {
                for(int i = 0; i < handle.Result.Count; i++)
                {
                    Addressables.Release(handle);
                }
            }

            ReleaseHandleEvent = releaseHandle;
            
            await handle.Task;
            foreach(var item in handle.Result)
            {   
                var obj = MonoBehaviour.Instantiate(item);
                obj.transform.parent = parent.transform;
                obj.SetActive(false);
                poolList.Add(obj);
            }

            var dataHandle = Addressables.LoadAssetAsync<ItemData>("ItemData");
            await dataHandle.Task;
            
            itemData = dataHandle.Result;

            var plateHandle = Addressables.LoadAssetAsync<GameObject>("Plate");
            await plateHandle.Task;

            tmpPlate = plateHandle.Result;
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
        public void CreateItem()
        {
            // 表示されているアイテムが?個未満
            int num = 0;
            for(int i = 0; i < parent.transform.childCount; i++)
            {
                if(parent.transform.GetChild(i).gameObject.activeSelf)
                {
                    num++;
                }
            }

            if(num >= itemData.ItemPopNum)  return;
            
            Create();
            
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
                    itemPos.Add(new ItemPosData(new Vector3(baseLineX + i * spaceX, InGameConst.ITEMPOS_Y, baseLineZ + j * spaceZ), false));
                }
            }   
        }

        // アイテムリストの中から、アクティブな要素にランダムにアクセスして
        // 一定時間ごとにstoingする
        public void RandomEraseItem()
        {
            GameObject obj = null;
            int count = parent.transform.childCount;
            do
            {
                var rand = UnityEngine.Random.Range(0,count);
                obj = parent.transform.GetChild(rand).gameObject;
                
            }
            while(!obj.activeSelf);

            Storing(obj);
            
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

        /// <summary>
        /// attendを取得するメソッド
        /// </summary>
        /// <returns>attend</returns>
        public bool GetAttend(){return attend;}
        /// <summary>
        /// attendを登録するメソッド
        /// </summary>
        /// <param name="value">boolの値</param>
        public void SetAttend(bool value){attend = value;}

    }
}

