using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Player;

namespace Item
{
    /// <summary>
    /// アイテムの管理を担当するクラス
    /// </summary>
    public class ItemManager : MonoBehaviour
    {   
        private ItemFactory itemFactory;
        private PlayerManager playerManager;

        // 取得されたアイテムの座標を一時的に保管しておくQueue
        private Queue<Vector3> emptyItemPos = new Queue<Vector3>(8);
        
        // コンストラクタ
        public ItemManager(PlayerManager tmpPlayerManager)
        {
            
            itemFactory = new ItemFactory();
            playerManager = tmpPlayerManager;
        }

        public void Update()
        {
            // イベントがうまくいってないため、一時的にコメントアウト
            //playerManager.TakeFood.ReturnPresentItemPos += ReturnEmptyItemPos;
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
       
        
        

        

        
    }
}


