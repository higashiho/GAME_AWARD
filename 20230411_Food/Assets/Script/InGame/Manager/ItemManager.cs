using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

        // アイテム生成クラス
        public ItemFactory itemFactory{get; private set;}
        // プレイヤーマネージャー
        private PlayerManager playerManager;
        private int counter;
        private int ripopItemCounter;
        private int deleteTime = 600;
        private int ripopTime = 600;
        // アイテムリポップタスク
        private UniTask? repopItemTask = null;

        
        
        
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
            counter = 0;
            ripopItemCounter = 0;
        }

        /// <summary>
        /// アイテム関連の更新メソッド
        /// </summary>
        public void Update()
        {
            //itemFactory.CreateItem();
            counter++;
            ripopItemCounter++;
            if(counter == deleteTime)
            {
                itemFactory.RandomEraseItem();
                counter = 0;
            }
            if(ripopItemCounter > ripopTime)
            {
                itemFactory.CreateItem();
                ripopItemCounter = 0;
            }
        }

        
        


        
        
        

        

        
    }
}


// Queueが空じゃないとき && 


