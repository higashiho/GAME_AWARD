using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

using FoodPoint;

using Item;
using Player;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        private UniTask? initTask = null;
        // ステート
        [SerializeField]
        private enum gameState
        {
            OPENING,    // 開幕演出
            COUNTDOWM,  // ゲーム開始カウントダウン
            GAME,       // ゲーム
            COOKING,    // 集めた食材で料理
            END         // リザルトへ
        }

        // ゲームステート管理変数
        [SerializeField]
        private gameState phase;

        // ポイント管理クラス
        private PointManager pointManager;
        

        void Start()
        {
            
            
            objectManager = new ObjectManager();

            
        }


        private ObjectManager objectManager;

        /// <summary>
        /// InGameの初期化はこのメソッド内で行う
        /// </summary>
        /// <returns></returns>
        private async UniTask InitGame()
        {
            
            ObjectManager.Player = new PlayerManager();
            ObjectManager.ItemManager = new ItemManager();
            // アイテム関係初期化
            ObjectManager.ItemManager.Init();
            // 仮
            await UniTask.Delay(5);
        }
       
        
        async void Update()
        {
            switch(phase)
            {
                case gameState.OPENING:
                    // スタートアニメーション
                    // ロードなどの処理

                    // ゲームシーン初期化処理
                    if(initTask == null)
                    {
                        initTask = InitGame();

                        await (UniTask)initTask;
                    }

                    break;
                
                case gameState.COUNTDOWM:
                    // ゲームスタート時のカウントダウン処理
                    break;

                case gameState.GAME:
                    
                    objectManager.PlayerUpdate();
                    objectManager.ItemUpdate();
                        
                    
                    break;
                
                case gameState.COOKING:

                    // プレイヤーが集めたポイント達を配列に入れていく
                    
                    break;
                
                case gameState.END:

                    // リザルトシーンへ
                    SceneManager.LoadScene("ResultScene");
                    phase = gameState.OPENING;
                    break;

            }
        }
    }

    public class ObjectManager
    {
        
        // アイテム管理クラス
        private static ItemManager itemManager;

        public static ItemManager ItemManager
        {
            get{return itemManager;}
            set{itemManager = value;}
        }

        public void ItemUpdate()
        {
            ItemManager.Update();
        }

        // プレイヤー
        private static PlayerManager player;

        public static PlayerManager Player
        {
            get{return player;}
            set{player = value;}
        }

        // インゲーム全体統括メソッド
        public void PlayerUpdate()
        {
            Player.Update();
        }
    }
}

