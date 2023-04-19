using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Player;
using GameManager;

namespace Item
{
    /// <summary>
    /// アイテムの管理を担当するクラス
    /// </summary>
    public class ItemManager 
    {   
        public ItemFactory itemFactory{get; private set;}
        private PlayerManager playerManager;
        

        // 取得されたアイテムの座標を一時的に保管しておくQueue
        private Queue<Vector3> emptyItemPos = new Queue<Vector3>(8);
        private Queue<GameObject> storingItem = new Queue<GameObject>();
        
        // コンストラクタ
        public ItemManager()
        {
            
            itemFactory = new ItemFactory();
            
        }

        public void Init()
        {
            // アイテムをステージにセット
            itemFactory.InitItem();

            
        }

        public void Update()
        {
            
            // イベントを登録
            ObjectManager.Player.FoodPoint.ReturnPresentItemPos += ReturnEmptyItemPos;
            // アイテムリポップ
            //CreateItem();
        }
        /// <summary>
        /// アイテムが取得された時にそのアイテムの座標を保管Queueに返すメソッド
        /// </summary>
        /// <param name="pos">アイテムの座標</param>
        public void ReturnEmptyItemPos(object sender, ReturnPresentPosEventArgs e)
        {
            // 座標保管Queueにいれていく
            emptyItemPos.Enqueue(e.presentPos);
        }


        /// <summary>
        /// アイテムのリポップメソッド
        /// </summary>
        /// <returns></returns>
        public async void CreateItem()
        {
            // アイテムの空き座標をQueueから取得
            Vector3? createPos = emptyItemPos.Dequeue();

            // アイテム生成座標がQueueに無かった場合処理を抜ける
            if(createPos == null)   return;
            
            // 5秒待機
            await Task.Delay(5 * 1000);

            // アイテムリポップ
            itemFactory.Create((Vector3)createPos);

        }
        
        

        

        
    }
}


// Queueが空じゃないとき && 


