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
        private List<Vector3> emptyItemPos = new List<Vector3>(16);
        
        
        // コンストラクタ
        public ItemManager()
        {
            
            itemFactory = new ItemFactory();
            
        }

        /// <summary>
        /// アイテム関連の初期化メソッド
        /// </summary>
        public void Init()
        {
            // アイテムをステージにセット
            itemFactory.InitItem();
        }

        /// <summary>
        /// アイテム関連の更新メソッド
        /// </summary>
        public void Update()
        {
            // プレイヤーがアイテムを取得したときのイベントを登録
            ObjectManager.Player.FoodPoint.ReturnPresentItemPos += ReturnEmptyItemPos;
            // アイテムリポップ
            CreateItem();
        }
        /// <summary>
        /// アイテムが取得された時にそのアイテムの座標を保管Queueに返すメソッド
        /// </summary>
        /// <param name="pos">アイテムの座標</param>
        public void ReturnEmptyItemPos(object sender, ReturnPresentPosEventArgs e)
        {
            // 座標保管Queueにいれていく
            emptyItemPos.Add(e.presentPos);
        }


        /// <summary>
        /// アイテムのリポップメソッド
        /// </summary>
        /// <returns></returns>
        public async void CreateItem()
        {
            if(emptyItemPos.Count == 0)
                return;

            // アイテムの空き座標をQueueから取得
            Vector3? createPos = emptyItemPos[0];
            emptyItemPos.RemoveAt(0);

            
            
            // 5秒待機
            await Task.Delay(5 * 1000);

            // アイテムリポップ
            //itemFactory.Create((Vector3)createPos);

        }
        
        

        

        
    }
}


// Queueが空じゃないとき && 


