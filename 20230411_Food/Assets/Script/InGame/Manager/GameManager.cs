using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using FoodPoint;

using Item;
using Player;
using FollowCamera;

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
        private PointManager[] pointManager = new PointManager[2];
        
        void Start()
        {
            objectManager = new ObjectManager();
        }


        private ObjectManager objectManager;
        private FollowingCameraManager followingCameraManager;

        [SerializeField]
        private DataPlayer main;
        
        [SerializeField]
        private DataPlayer sub;

        /// <summary>
        /// InGameの初期化はこのメソッド内で行う
        /// </summary>
        /// <returns></returns>
        private async UniTask InitGame()
        {
            ObjectManager.PlayerManagers.Add(new PlayerManager(main));
            ObjectManager.PlayerManagers.Add(new PlayerManager(sub));
            followingCameraManager = new FollowingCameraManager();

            ObjectManager.ItemManager = new ItemManager();
            // ポイントマネージャー作成
            for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
            {
                pointManager[i] = new PointManager(ObjectManager.PlayerManagers[i]);
            }
            
            // アイテム関係初期化
            ObjectManager.ItemManager.Init();
            // 追従カメラクラスがプレイヤーマネージャーを取得
            followingCameraManager.SetFollowingPlayer(ObjectManager.PlayerManagers);
            // 追従カメラ初期化
            followingCameraManager.Init();
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
                    // 追従カメラ更新
                    followingCameraManager.Update();
                        
                    
                    break;
                
                case gameState.COOKING:

                    // プレイヤーが集めたポイント達を配列に入れていく
                    //pointManager.GetPlayerFoodPoint(ObjectManager.Player);
                    for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
                    {
                        pointManager[i].GetPlayerPoint(ObjectManager.PlayerManagers[i]);
                    }
                    phase = gameState.END;
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
            //ItemManager.Update();
        }

        private static List<PlayerManager> playerManagers = new List<PlayerManager>(2);

        public static List<PlayerManager> PlayerManagers
        {
            get{return playerManagers;}
        }

        // インゲーム全体統括メソッド
        public void PlayerUpdate()
        {
            for(int i = 0; i < PlayerManagers.Count; i++)
            {
                ObjectManager.PlayerManagers[i].Update();
            }
        }
    }
}

