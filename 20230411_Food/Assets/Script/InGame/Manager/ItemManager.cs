using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Item
{
    /// <summary>
    /// アイテムの管理を担当するクラス
    /// </summary>
    public class ItemManager : MonoBehaviour
    {   
        private ItemFactory itemFactory;

        // 取得されたアイテムの座標を一時的に保管しておくQueue
        private Queue<Vector3> emptyItemPos;
        
        // コンストラクタ
        public ItemManager()
        {
            
            itemFactory = new ItemFactory();
        }

        /// <summary>
        /// アイテムが取得された時にそのアイテムの座標を保管Queueに返すメソッド
        /// </summary>
        /// <param name="pos">アイテムの座標</param>
        public void ReturnEmptyItemPos(Vector3 pos)
        {
            // 座標保管Queueにいれていく
            emptyItemPos.Enqueue(pos);
        }
       
        
        

        

        
    }
}


