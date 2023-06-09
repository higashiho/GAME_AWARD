using System.Threading;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using FoodPoint;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using TMPro;
using DG.Tweening;

using Item;
using Player;
using InGame;
using FollowCamera;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        public CancellationTokenSource Cts{get;} = new CancellationTokenSource();
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
        [SerializeField]
        private FoodThemeDataList foodData;
        public FoodThemeDataList FoodData{get => foodData;}

        async void Start()
        {
            Application.targetFrameRate = 60;

            // if(!cutInCanvas.gameObject.activeSelf)
            //     cutInCanvas.gameObject.SetActive(true);
            // スタートアニメーション
            // ロードなどの処理

            // ゲームシーン初期化処理
            initTask = InitGame();

            await (UniTask)initTask;
        }

        [SerializeField]
        private DataPlayer firstPlayerData;
        
        [SerializeField]
        private DataPlayer secondPlayerData;

        [SerializeField]
        private FoodThemeDataList foodThemeData;
        public FoodThemeDataList FoodThemeData{get => foodThemeData;}

        [SerializeField]
        private Canvas cutInCanvas;

        public Subject<bool> NowCountDownFlag{get;} = new Subject<bool>();
        [SerializeField]
        private UIController uiController;
        public UIController UIController{get => uiController;}

        private GameTimer timer = null;
        [SerializeField]
        private Canvas timeUpCanvas;


        /// <summary>
        /// InGameの初期化はこのメソッド内で行う
        /// </summary>
        /// <returns></returns>
        private async UniTask InitGame()
        {
            ObjectManager.GameManager = this;
            ObjectManager.PlayerManagers.Clear();
            // 要素を増やすためにいったんnullを入れる
            ObjectManager.PlayerManagers.Add(null);
            ObjectManager.PlayerManagers.Add(null);
            // 要素書き換え
            ObjectManager.PlayerManagers[0] = new PlayerManager(firstPlayerData);
            ObjectManager.PlayerManagers[1] = new PlayerManager(secondPlayerData);

            // Playerの生成が終わるまで待つ
            for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
                await UniTask.WaitWhile(() => ObjectManager.PlayerManagers[i] == null);
                
            ObjectManager.ItemManager = new ItemManager();


            ObjectManager.FollowCamera = new FollowingCameraManager(ObjectManager.PlayerManagers);
            // レシピを決める
            ObjectManager.Recipe = new DecideTheRecipe(foodThemeData);

            // ポイントマネージャー作成
            for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
            {
                pointManager[i] = new PointManager(ObjectManager.PlayerManagers[i], FoodThemeData);
            }
            
            // アイテム関係初期化
            ObjectManager.ItemManager.Init();
            // 追従カメラ初期化
            ObjectManager.FollowCamera.Init();
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

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }).AddTo(this);
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
                    
                    if(timer == null)
                    {
                        timer = new GameTimer(timeUpCanvas);
                        timer.Timer();
                    }
                    
                    break;
                
                case gameState.COOKING:

                    // プレイヤーが集めたポイント達を配列に入れていく
                    //pointManager.GetPlayerFoodPoint(ObjectManager.Player);
                    for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
                    {
                        pointManager[i].GetPlayerPoint(i, ObjectManager.PlayerManagers[i]);
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

        private void OnDestroy()
        {
            ObjectManager.ItemManager.itemFactory.ReleaseHandleEvent();
            ObjectManager.FollowCamera.ReleaseHandleEvent();

            for(int i = 0; i < ObjectManager.PlayerManagers.Count; i++)
            {
                Addressables.Release(ObjectManager.PlayerManagers[i].DataHandle);
            }
            Cts.Cancel();
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
            ItemManager.Update();
        }

        private static List<PlayerManager> playerManagers = new List<PlayerManager>(2);

        public static List<PlayerManager> PlayerManagers
        {
            get{return playerManagers;}
            set{playerManagers = value;}
        }

        // インゲーム全体統括メソッド
        public static void PlayerUpdate()
        {
            for(int i = 0; i < PlayerManagers.Count; i++)
            {
                ObjectManager.PlayerManagers[i].Update();
            }
        }

        // 追従カメラ管理クラス
        private static FollowingCameraManager followCamera;
        public static FollowingCameraManager FollowCamera
        {
            get{return followCamera;}
            set{followCamera = value;}
        }

        // レシピを決定するクラス
        private static DecideTheRecipe recipe;
        public static DecideTheRecipe Recipe
        {
            get{return recipe;}
            set{recipe = value;}
        }

        
    }

    public class GameTimer
    {
        private Canvas timeUpCanvas;

        public GameTimer(Canvas tmpCanvas)
        {
            timeUpCanvas = tmpCanvas;
        }
        private float timeLimit = float.Parse(ObjectManager.GameManager.UIController.TitmerText.text);
        public async void Timer()
        {
            while(!ObjectManager.GameManager.Cts.IsCancellationRequested)
            {
                timeLimit--;
                await UniTask.Delay(1000);
                ObjectManager.GameManager.UIController.TitmerText.text = timeLimit.ToString();

                if(timeLimit <= 0)
                    break;
            }
            
            if(ObjectManager.GameManager.Cts.IsCancellationRequested) return;

            timeUpCanvas.transform.GetChild(0).gameObject.SetActive(true);
            timeUpCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Time UP !!";
            await UniTask.Delay(500);
            
            timeUpCanvas.transform.GetChild(1).GetComponent<Image>().
                DOFade(1, 1).SetLink(ObjectManager.GameManager.gameObject).SetEase(Ease.InSine).
                OnComplete(ObjectManager.GameManager.ChangeState);
        }
    }
}

