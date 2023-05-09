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

        
        public ItemFactory itemFactory{get; private set;}
        private PlayerManager playerManager;
        
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
        }

        /// <summary>
        /// アイテム関連の更新メソッド
        /// </summary>
        public async void Update()
        {
            
            //itemFactory.CreateItem();
        }

        
        


        
        
        

        

        
    }
}


// Queueが空じゃないとき && 


