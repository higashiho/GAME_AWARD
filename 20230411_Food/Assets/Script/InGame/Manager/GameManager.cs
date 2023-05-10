using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using FoodPoint;
using System.Linq;
using UniRx;


using Item;
using Player;
using InGame;

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
        
        async void Start()
        {
            // if(!cutInCanvas.gameObject.activeSelf)
            //     cutInCanvas.gameObject.SetActive(true);
            // スタートアニメーション
            // ロードなどの処理

            // ゲームシーン初期化処理
            if(initTask == null)
            {
                initTask = InitGame();

                await (UniTask)initTask;
            }

            await UniTask.WaitWhile(() => !Input.GetKeyDown(KeyCode.Return));
            ChangeState();
               

        }

        [SerializeField]
        private DataPlayer mainPlayerData;
        
        [SerializeField]
        private DataPlayer subPlayerData;

        [SerializeField]
        private FoodThemeDataList foodThemeData;

        [SerializeField]
        private Canvas cutInCanvas;

        public Subject<bool> NowCountDownFlag{get;} = new Subject<bool>();
        [SerializeField]
        private UIController uiController;

        /// <summary>
        /// InGameの初期化はこのメソッド内で行う
        /// </summary>
        /// <returns></returns>
        private async UniTask InitGame()
        {
            ObjectManager.GameManager = this;
            ObjectManager.PlayerManagers.Add(new PlayerManager(mainPlayerData));
            ObjectManager.PlayerManagers.Add(new PlayerManager(subPlayerData));

            ObjectManager.ItemManager = new ItemManager();
            // ポイントマネージャー作成
            for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
            {
                pointManager[i] = new PointManager(ObjectManager.PlayerManagers[i]);
            }
            
            // アイテム関係初期化
            ObjectManager.ItemManager.Init();
            // 仮
            await UniTask.Delay(5);

            setSubscribe();
        }

        private void setSubscribe()
        {
            NowCountDownFlag
                .Where(x => x)
                .Subscribe(_ =>
                    {
                        if(!uiController.transform.GetChild(1).gameObject.activeSelf)
                            uiController.transform.GetChild(1).gameObject.SetActive(true);
                    }
                ).AddTo(this.gameObject);
        }
       
        
        void Update()
        {
            switch(phase)
            {
                
                case gameState.COUNTDOWM:
                    // ゲームスタート時のカウントダウン処理
                    NowCountDownFlag.OnNext(true);
                    break;
                case gameState.GAME:

                    ObjectManager.PlayerUpdate();
                    ObjectManager.ItemUpdate();
                        
                    
                    break;
                
                case gameState.COOKING:

                    // プレイヤーが集めたポイント達を配列に入れていく
                    //pointManager.GetPlayerFoodPoint(ObjectManager.Player);
                    for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
                    {
                        pointManager[i].GetPlayerPoint(ObjectManager.PlayerManagers[i]);
                    }
                    ChangeState();
                    break;
                
                case gameState.END:

                    // リザルトシーンへ
                    SceneManager.LoadScene("ResultScene");
                    ChangeState();
                    break;

            }
        }

        /// <summary>
        /// 次のステートへ変更メソッド
        /// </summary>
        public void ChangeState()
        {
            if((int)phase <= Enum.GetValues(typeof(gameState)).Cast<int>().Max())
                phase++;
            else
                phase = (gameState)Enum.GetValues(typeof(gameState)).Cast<int>().Min();
        }
    }

    public class ObjectManager
    {
        private static GameManager gameManager;
        public static GameManager GameManager
        {
            get => gameManager;
            set => gameManager = value;
        }
        
        // アイテム管理クラス
        private static ItemManager itemManager;

        public static ItemManager ItemManager
        {
            get{return itemManager;}
            set{itemManager = value;}
        }

        public static void ItemUpdate()
        {
            //ItemManager.Update();
        }

        private static List<PlayerManager> playerManagers = new List<PlayerManager>(2);

        public static List<PlayerManager> PlayerManagers
        {
            get{return playerManagers;}
        }

        // インゲーム全体統括メソッド
        public static void PlayerUpdate()
        {
            for(int i = 0; i < PlayerManagers.Count; i++)
            {
                ObjectManager.PlayerManagers[i].Update();
            }
        }
    }
}

