using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using Constants;
using OutGame;

namespace Title
{
    /// <summary>
    /// タイトル入力イベント管理クラス
    /// </summary>
    public sealed class EventsManager
    {
           
        /// <summary>インプットイベント管理クラスインスタンス化</summary>
        public InputManager InputSetting{get; private set;}

        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manageObject">イベント対象オブジェクト</param>
        /// <param name="tmpDataList">playerのデータリスト</param>
        public EventsManager(GameObject manageObject, TitlePlayerDataList tmpDataList)
        {
            InputSetting = new InputManager(tmpDataList);
            // 以下指定オブジェクトDestroy時にイベント破棄設定=========
            keyResetInput.AddTo(manageObject);
            FoodNicknamesTextPoint.AddTo(manageObject);
            DisplayIngredientsListTextPoint.AddTo(manageObject);
            GameStartTextPoint.AddTo(manageObject);
            // =====================================================

            
            // タイトル入力確認ループ
            manageObject.UpdateAsObservable()
                .Subscribe(_ => Update())
                .AddTo(manageObject);
        }

        // 以下入力イベント取得用変数==================================================
        // オブジェクト取得イベント
        public IObservable<GameObject>[] HoldPlayerObject => HavePlayerObject;
        // カメラリセットイベント
        public IReadOnlyReactiveProperty<bool> ResetCameraToStart => keyResetInput;
        public IObservable<uint> KeyPressed => keyPressed;

        // =================================================================


        
        // 以下入力イベント実装===============================================
        // リセットボタン判断リアクティブプロパティ
        private readonly ReactiveProperty<bool> keyResetInput = new BoolReactiveProperty();
        // 所持オブジェクト判断サブジェクト
        public Subject<GameObject>[] HavePlayerObject{get;} = {
            new Subject<GameObject>(),
            new Subject<GameObject>()
            };
        // キー入力判断サブジェクト
        private Subject<uint> keyPressed = new Subject<uint>();

        // テキストイメージ移動座標設定サブジェクト
        public Subject<float> FoodNicknamesTextPoint{get;} = new Subject<float>();
        public Subject<float> DisplayIngredientsListTextPoint{get;} = new Subject<float>();
        public Subject<float> GameStartTextPoint{get;} = new Subject<float>();
        // =================================================================

        
        /// <summary>
        /// 入力更新関数
        /// </summary>
        public void Update()
        {
            // 以下各種入力をReactivePropertyに反映=========================
            keyResetInput.Value = Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Tab);

            // ===========================================================
            
            currentPressed();
        }

        /// <summary>
        /// キーが押されたら押されているキーをSubjectに代入
        /// </summary>
        /// <returns><see cref="KeyCode"/></returns>
        private void currentPressed()
        {
            // 何もキーが押されていなかったら処理終了
            if(!Input.anyKey) return;
            
            foreach (KeyCode  keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                // 対象のキーの場合Subjectに代入
                if (Input.GetKeyDown(keyCode))
                {
                    keyPressed.OnNext((uint)keyCode);
                }
            }
        }
    }

    /// <summary>
    /// 入力判断クラス
    /// </summary>
    public sealed class InputManager
    {
       // Scene転移が可能かどうか
        private bool OnSceneMoveFlag = true;

        // レシピブックが開いているか
        private bool nowOpenRecipeBook = false;

        // 冷蔵庫が開いているか
        private bool nowOpenRefrugerator = false;

        // カメラ移動時間
        private SceneMoveTime moveTime = new SceneMoveTime(2);

        // Playerのデータ
        private TitlePlayerDataList playerDataList;

        public InputManager(TitlePlayerDataList tmpData)
        {
            playerDataList = tmpData;
        }

        /// <summary>
        /// ゲームスタートイベント処理
        /// </summary>
        private void gameStartEvents()
        {
            // ゲームスタートイベント============================================
            // メインプレイヤー用イベント
            ObjectManager.Events.HoldPlayerObject[0]
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトを持っているか
                .Where(x => x)
                // スタート用のオブジェクトが入っているか
                .Where(x => x.name == "GasBurner")
                .Where(_ => Input.GetKeyDown(playerDataList.PlayerDatas[0].DeterminationKey))
                // シーン転移が可能か
                .Where(_ => OnSceneMoveFlag)
                // 実施
                .Subscribe(x =>
                {
                    var sound = ObjectManager.TitleScene.AudioController.GetComponent<SoundController>();
                    // ゲームスタートBGM再生
                    sound.MainSource.
                        DOFade(0, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    sound.OnGamePlaySource.
                        PlayOneShot(sound.AudioClips[1]);
                    sound.OnGamePlaySource.
                        DOFade(1, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);

                    // アシストUI非表示
                    if(ObjectManager.Ui.AssistCanvas.transform.GetChild(playerDataList.PlayerDatas[0].Id).gameObject.activeSelf)
                        ObjectManager.Ui.SetAssistPlayerUIActive(playerDataList.PlayerDatas[0].Id, false);

                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // フラグを折る
                    OnSceneMoveFlag = false;
                    // シーン転移
                    ObjectManager.TitleScene.Move.GameStartMovement(x);
                    Debug.Log("SceneMove");
                });

            // サブプレイヤー用イベント
            ObjectManager.Events.HoldPlayerObject[1]
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトを持っているか
                .Where(x => x)
                // スタート用のオブジェクトが入っているか
                .Where(x => x.name == "GasBurner")
                .Where(_ => Input.GetKeyDown(playerDataList.PlayerDatas[1].DeterminationKey))
                // シーン転移が可能か
                .Where(_ => OnSceneMoveFlag)
                // 実施
                .Subscribe(x =>
                {          
                    // ゲームスタートBGM再生
                    var suond = ObjectManager.TitleScene.AudioController.GetComponent<SoundController>();
                    suond.MainSource.
                        DOFade(0, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    suond.OnGamePlaySource.
                        PlayOneShot(suond.AudioClips[1]);
                    suond.OnGamePlaySource.
                        DOFade(1, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                        
                    // アシストUI非表示
                    if(ObjectManager.Ui.AssistCanvas.transform.GetChild(playerDataList.PlayerDatas[1].Id).gameObject.activeSelf)
                        ObjectManager.Ui.SetAssistPlayerUIActive(playerDataList.PlayerDatas[1].Id, false);

                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // フラグを折る
                    OnSceneMoveFlag = false;
                    // シーン転移
                    ObjectManager.TitleScene.Move.GameStartMovement(x);
                    Debug.Log("SceneMove");
                });
            // ==================================================================
        }

        /// <summary>
        /// 食材相性イベント処理
        /// </summary>
        private void foodNicknamesEvents()
        {
            // 食材相性UI表示イベント==============================================
            // メインプレイヤー用イベント
            ObjectManager.Events.HoldPlayerObject[0]
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerかSubPlayerのRayにスタート用のオブジェクトが入っているか
                .Where(x => x.name == "RecipeBook")
                .Where(_ => Input.GetKeyDown(playerDataList.PlayerDatas[0].DeterminationKey))
                // 押されて指定秒経っていたら
                // レシピ本は挙動が二つある為２倍する
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount * 2))
                // 実施
                .Subscribe(async x =>
                {
                    // sound再生
                    var suond = ObjectManager.TitleScene.AudioController.GetComponent<SoundController>();
                    suond.MainSource.
                        DOFade(0.5f, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    suond.RecipeSource.
                        PlayOneShot(suond.AudioClips[2]);
                    suond.OnGamePlaySource.
                        DOFade(1, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);

                    // アシストUI非表示
                    if(ObjectManager.Ui.AssistCanvas.transform.GetChild(playerDataList.PlayerDatas[0].Id).gameObject.activeSelf)
                        ObjectManager.Ui.SetAssistPlayerUIActive(playerDataList.PlayerDatas[0].Id, false);

                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // UI表示
                    await ObjectManager.TitleScene.Move.OpenRecipeBook(x);
                    
                    // フラグを立てる
                    nowOpenRecipeBook = true;
                });

            // サブプレイヤー用イベント
            ObjectManager.Events.HoldPlayerObject[1]
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerかSubPlayerのRayにスタート用のオブジェクトが入っているか
                .Where(x => x.name == "RecipeBook")
                .Where(_ => Input.GetKeyDown(playerDataList.PlayerDatas[1].DeterminationKey))
                // 押されて指定秒経っていたら
                // レシピ本は挙動が二つある為２倍する
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount * 2))
                // 実施
                .Subscribe(async x =>
                {
                    
                    // sound再生
                    var suond = ObjectManager.TitleScene.AudioController.GetComponent<SoundController>();
                    suond.MainSource.
                        DOFade(0.5f, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    suond.RecipeSource.
                        PlayOneShot(suond.AudioClips[2]);
                    suond.OnGamePlaySource.
                        DOFade(1, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);

                    // アシストUI非表示
                    if(ObjectManager.Ui.AssistCanvas.transform.GetChild(playerDataList.PlayerDatas[1].Id).gameObject.activeSelf)
                        ObjectManager.Ui.SetAssistPlayerUIActive(playerDataList.PlayerDatas[1].Id, false);

                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // UI表示
                    await ObjectManager.TitleScene.Move.OpenRecipeBook(x);
                    
                    // フラグを立てる
                    nowOpenRecipeBook = true;
                });
            // ==================================================================

        }
        
        /// <summary>
        /// 食材一覧表示イベント処理
        /// </summary>
        private void displayIngredientsListEvents()
        {
            // 食材一覧表示イベント================================================
            // メインプレイヤー用イベント
            ObjectManager.Events.HoldPlayerObject[0]
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerのRayにスタート用のオブジェクトが入っているか
                .Where(x => x.name == "Refrugerator")
                .Where(_ => Input.GetKeyDown(playerDataList.PlayerDatas[0].DeterminationKey))
                // 押されて指定秒経っていたら
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount))
                // 実施
                .Subscribe(async x =>
                {
                    // sound再生
                    var suond = ObjectManager.TitleScene.AudioController.GetComponent<SoundController>();
                    suond.MainSource.
                        DOFade(0.5f, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    suond.FoodListSource.
                        PlayOneShot(suond.AudioClips[3]);
                    suond.OnGamePlaySource.
                        DOFade(1, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    
                    // アシストUI非表示
                    if(ObjectManager.Ui.AssistCanvas.transform.GetChild(playerDataList.PlayerDatas[0].Id).gameObject.activeSelf)
                        ObjectManager.Ui.SetAssistPlayerUIActive(playerDataList.PlayerDatas[0].Id, false);

                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // UI表示関数実行
                    await ObjectManager.TitleScene.Move.OpenRefrugerator(x);
               
                    // フラグを立てる
                    nowOpenRefrugerator = true;
                });

            // サブプレイヤー用イベント
            ObjectManager.Events.HoldPlayerObject[1]
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerのRayにスタート用のオブジェクトが入っているか
                .Where(x => x.name == "Refrugerator")
                .Where(_ => Input.GetKeyDown(playerDataList.PlayerDatas[1].DeterminationKey))
                // 押されて指定秒経っていたら
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount))
                // 実施
                .Subscribe(async x =>
                {
                    // sound再生
                    var suond = ObjectManager.TitleScene.AudioController.GetComponent<SoundController>();
                    suond.MainSource.
                        DOFade(0.5f, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    suond.FoodListSource.
                        PlayOneShot(suond.AudioClips[3]);
                    suond.OnGamePlaySource.
                        DOFade(1, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    // アシストUI非表示
                    if(ObjectManager.Ui.AssistCanvas.transform.GetChild(playerDataList.PlayerDatas[1].Id).gameObject.activeSelf)
                        ObjectManager.Ui.SetAssistPlayerUIActive(playerDataList.PlayerDatas[1].Id, false);
                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // UI表示関数実行
                    await ObjectManager.TitleScene.Move.OpenRefrugerator(x);
               
                    // フラグを立てる
                    nowOpenRefrugerator = true;
                });
            // ==================================================================
        }

        /// <summary>
        /// カメラリセットイベント処理
        /// </summary>
        private void resetCameraToStartEvenets()
        {
            // カメラリセットイベント==============================================
            ObjectManager.Events.ResetCameraToStart
                // どれかのフラグがたっているか
                .Where(_ => nowOpenRecipeBook ||
                            nowOpenRefrugerator)
                // イベント指定した入力がされているか
                .Where(x => x)
                // 押されて指定秒経っていたら
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount))
                // 実施
                .Subscribe(async _ =>
                {
                    var suond = ObjectManager.TitleScene.AudioController.GetComponent<SoundController>();
                    suond.MainSource.
                        DOFade(0.5f, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    if(nowOpenRefrugerator)
                    {
                        suond.FoodListSource.
                            DOFade(0, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    }
                    else if(nowOpenRecipeBook)
                    {
                        suond.RecipeSource.
                            DOFade(0, 1).SetEase(Ease.Linear).SetLink(ObjectManager.TitleScene.AudioController);
                    }
                    // イベント実行フラグを折る
                    ObjectManager.TitleScene.NowPlayeEvents = false;
                    // カメラリセット関数実行
                    await ObjectManager.TitleScene.Move.ResetCamera();
                    
                    // フラグ初期化
                    nowOpenRefrugerator = false;
                    nowOpenRecipeBook = false;
                    
                });
            // ==================================================================
        }
        
        /// <summary>
        /// タイトル入力イベント設定関数
        /// </summary>
        public void SetEvents()
        {
            gameStartEvents();
            
            foodNicknamesEvents();

            displayIngredientsListEvents();
            
            resetCameraToStartEvenets();
        } 
    
    }

    /// <summary>
    /// 移動入力管理クラス
    /// </summary>
    public class InputMovementManager
    {
        private TitlePlayerData playerData;
        public InputMovementManager(TitlePlayerData tmpData)
        {
            playerData = tmpData;
        }


        /// <summary>
        /// 前方移動イベント処理
        /// </summary>
        private void forwardMovementEvent()
        {
            // 前方移動
            ObjectManager.Player[playerData.Id].Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(playerData.MoveKey[0]))
                // イベントが実行中でないか
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => Vector3.zero != ObjectManager.Player[playerData.Id].HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => ObjectManager.Player[playerData.Id].MoveDis == Vector3.zero ||
                            ObjectManager.Player[playerData.Id].MoveDis == Vector3.forward)
                // 実行
                .Subscribe(_ => ObjectManager.Player[playerData.Id].Move.ForwardMovement())
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.Player[playerData.Id].Object);

        }
        /// <summary>
        /// 左移動イベント処理
        /// </summary>
        private void leftMovementEvent()
        {
            // 前方移動
            ObjectManager.Player[playerData.Id].Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(playerData.MoveKey[1]))
                // イベントが実行中でないか
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => TitleConstants.PLAYER_DIRECTION_LEFT != ObjectManager.Player[playerData.Id].HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => ObjectManager.Player[playerData.Id].MoveDis == Vector3.zero ||
                            ObjectManager.Player[playerData.Id].MoveDis == Vector3.left)
                // 実行
                .Subscribe(_ => ObjectManager.Player[playerData.Id].Move.LeftMovement())
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.Player[playerData.Id].Object);

        }
        /// <summary>
        /// 後方移動イベント処理
        /// </summary>
        private void backMovementEvent()
        {
            // 前方移動
            ObjectManager.Player[playerData.Id].Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(playerData.MoveKey[2]))
                // イベントが実行中でないか
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => TitleConstants.PLAYER_DIRECTION_BACK != ObjectManager.Player[playerData.Id].HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => ObjectManager.Player[playerData.Id].MoveDis == Vector3.zero ||
                            ObjectManager.Player[playerData.Id].MoveDis == Vector3.back)
                // 実行
                .Subscribe(_ => ObjectManager.Player[playerData.Id].Move.BackMovement())
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.Player[playerData.Id].Object);

        }

        /// <summary>
        /// 右移動イベント処理
        /// </summary>
        private void rightMovementEvent()
        {
            // 前方移動
            ObjectManager.Player[playerData.Id].Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(playerData.MoveKey[3]))
                // イベントが実行中でないか
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => TitleConstants.PLAYER_DIRECTION_RIGHT != ObjectManager.Player[playerData.Id].HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => ObjectManager.Player[playerData.Id].MoveDis == Vector3.zero ||
                            ObjectManager.Player[playerData.Id].MoveDis == Vector3.right)
                // 実行
                .Subscribe(_ => ObjectManager.Player[playerData.Id].Move.RightMovement())
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.Player[playerData.Id].Object);

        }



        /// <summary>
        /// アニメーション初期化イベント処理
        /// </summary>
        private void resetAnimEvents()
        {
            // アニメーション初期化
            ObjectManager.Player[playerData.Id].Object.UpdateAsObservable()
                // どの移動ボタンも押されていないとき
                .Where(_ => !Input.GetKey(playerData.MoveKey[0])&& 
                            !Input.GetKey(playerData.MoveKey[1])&&
                            !Input.GetKey(playerData.MoveKey[2])&&
                            !Input.GetKey(playerData.MoveKey[3]))
                // 実行
                .Subscribe(_ => ObjectManager.Player[playerData.Id].Move.ResetAnim())
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.Player[playerData.Id].Object);
        }

        /// <summary>
        /// 挙動初期化イベント処理
        /// </summary>
        private void resetMovementEvents()
        {
            // 挙動初期化
            ObjectManager.Player[playerData.Id].Object.UpdateAsObservable()
                // どれかのボタンが離された時
                .Where(_ => Input.GetKeyUp(playerData.MoveKey[0])|| 
                            Input.GetKeyUp(playerData.MoveKey[1])||
                            Input.GetKeyUp(playerData.MoveKey[2])||
                            Input.GetKeyUp(playerData.MoveKey[3]))
                // 実行
                .Subscribe(_ => ObjectManager.Player[playerData.Id].Move.ResetMovement())
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.Player[playerData.Id].Object);
        }

        /// <summary>
        /// プレイヤー移動更新ループ設定関数
        /// </summary>
        public void SetMovementLoops()
        {
            // 前方移動
            forwardMovementEvent();
            
            // 左移動
            leftMovementEvent();

            // 後方移動
            backMovementEvent();

            // 右移動
            rightMovementEvent();

            // アニメーション初期化
            resetAnimEvents();

            // 挙動初期化
            resetMovementEvents();
        }

    }

    /// <summary>
    /// テキスト拡大イベント処理
    /// </summary>
    public class TextApproachEventManager
    {
        
        /// <summary>
        /// Rayの長さ
        /// </summary>
        private RayDistance rayDistance = new RayDistance(1);
        public TextApproachEventManager()
        {
            // スケールはすべて一緒のため、０要素目を代入
            ObjectManager.Text = new TextManager(ObjectManager.TitleScene.TextImageCanvas[0].transform.localScale);
        }

        /// <summary>
        /// 食材一覧表示テキスト接近処理イベント
        /// </summary>
        private void foodNicknamesTextApproachEvents()
        {
            // 食材一覧表示テキスト接近処理イベント設定
            ObjectManager.Events.FoodNicknamesTextPoint
                // どちらかの取得オブジェクトが冷蔵庫の場合
                .Where(_ => ObjectManager.Player[0].HitObject?.name == ("Refrugerator") ||
                            ObjectManager.Player[1].HitObject?.name == ("Refrugerator"))
                // 目標座標が接近座標の場合
                .Where(x => x == TitleConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x => {
                    ObjectManager.Text.Move.FoodNicknamesTextMovement(x);
                    //　座標目標値設定
                    ObjectManager.Events.FoodNicknamesTextPoint.OnNext(TitleConstants.TEXT_IMAGE_LEAVE_POS_Y);
                    })
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.TitleScene);
        }

        /// <summary>
        /// 食材相性表示テキスト接近処理イベント
        /// </summary>
        private void displayIngredientsListTextApproachEvents()
        {
            // 食材相性表示テキスト接近処理イベント設定
            ObjectManager.Events.DisplayIngredientsListTextPoint
                // どちらかの取得オブジェクトがレシピブックの場合
                .Where(_ => ObjectManager.Player[0].HitObject?.name == ("RecipeBook") ||
                            ObjectManager.Player[1].HitObject?.name == ("RecipeBook"))
                // 目標座標が接近座標の場合
                .Where(x => x == TitleConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x => {
                    ObjectManager.Text.Move.DisplayIngredientsListTextMovement(x);                    
                    //　座標目標値設定
                    ObjectManager.Events.DisplayIngredientsListTextPoint.OnNext(TitleConstants.TEXT_IMAGE_LEAVE_POS_Y);
                    })
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.TitleScene);
        }

        /// <summary>
        /// ゲームスタートテキスト接近処理イベント
        /// </summary>
        private void gameStartTextApproachEvents()
        {
            // ゲームスタートテキスト接近処理イベント設定
            ObjectManager.Events.GameStartTextPoint
                // どちらかの取得オブジェクトがガスバーナーの場合
                .Where(_ => ObjectManager.Player[0].HitObject?.name == ("GasBurner") ||
                            ObjectManager.Player[1].HitObject?.name == ("GasBurner"))
                // 目標座標が接近座標の場合
                .Where(x => x == TitleConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x =>{
                    ObjectManager.Text.Move.GameStartTextMovement(x);
                    //　座標目標値設定
                    ObjectManager.Events.GameStartTextPoint.OnNext(TitleConstants.TEXT_IMAGE_LEAVE_POS_Y);
                    } )
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.TitleScene);
        }

        /// <summary>
        /// テキストイメージリセットイベント
        /// </summary>
        private async UniTaskVoid resetTextImageEvents()
        {
            while(!ObjectManager.TitleScene.Cts.Token.IsCancellationRequested)
            {
                // テキストイメージが全て初期位置にいた場合
                if(ObjectManager.TitleScene.TextImageCanvas[0].transform.localPosition.y == TitleConstants.TEXT_IMAGE_LEAVE_POS_Y &&
                ObjectManager.TitleScene.TextImageCanvas[1].transform.localPosition.y == TitleConstants.TEXT_IMAGE_LEAVE_POS_Y &&
                ObjectManager.TitleScene.TextImageCanvas[2].transform.localPosition.y == TitleConstants.TEXT_IMAGE_LEAVE_POS_Y)
                {
                    await UniTask.Yield();
                    continue;
                }


                
                for(int i = 0; i < ObjectManager.TitleScene.TextImageCanvas.Length; i++)
                {
                    // 自身が初期座標にいた場合処理を通過
                    if(ObjectManager.TitleScene.TextImageCanvas[i].transform.localPosition.y == TitleConstants.TEXT_IMAGE_LEAVE_POS_Y)
                        continue;
                    
                    // 判定サイズ設定
                    var drawRayScale = new Vector3(
                    ObjectManager.TitleScene.TextImageCanvas[i].transform.localScale.x * 3,
                    ObjectManager.TitleScene.TextImageCanvas[i].transform.localScale.y * 2,
                    ObjectManager.TitleScene.TextImageCanvas[i].transform.localScale.z * 0.5f
                    );
                    // 判定座標設定
                    var drawRayPos = new Vector3(
                        ObjectManager.TitleScene.TextImageCanvas[i].transform.position.x,
                        // 中央に表示しないように少し下げる
                        ObjectManager.TitleScene.TextImageCanvas[i].transform.position.y * 0.9f,
                        ObjectManager.TitleScene.TextImageCanvas[i].transform.position.z
                    );
                    // 当たっているか
                    if(!Physics.BoxCast(
                        drawRayPos, 
                        drawRayScale * 0.5f, 
                        -ObjectManager.TitleScene.TextImageCanvas[i].transform.forward, 
                        out ObjectManager.Text.HitPlayerRay[i], Quaternion.identity, 
                        rayDistance.Amount))
                        {
                            // イメージリセット関数呼び出し
                            ObjectManager.Text.Move.ResetTextMovement(ObjectManager.TitleScene.TextImageCanvas[i]);
                        }
                }
                await UniTask.Yield();

            }
        }

        /// <summary>
        /// テキスト接近イベント設定処理
        /// </summary>
        public void TextApproachEvents()
        {
            // 食材一覧表示テキスト接近処理イベント 
            foodNicknamesTextApproachEvents();

            // 食材一覧表示テキスト接近処理イベント
            displayIngredientsListTextApproachEvents();

            // ゲームスタートテキスト接近処理イベント
            gameStartTextApproachEvents();

            // リセットイベントループ
            resetTextImageEvents().Forget();
        }
    }
}

namespace Result
{
    /// <summary>
    /// リザルトイベント管理クラス
    /// </summary>
    public class EventsManager
    {
        // コンストラクタ
        public EventsManager(GameObject managerObject)
        {
            resultPatternSubject.AddTo(managerObject);
        }

        // リザルトのイベントパターン
        public enum ResultPatternEnum
        {
            START, FOOD_RATE, FOOD_AMOUNT, SEASONING, JUDGMENT, END
        }

        // 取得用イベント
        public IObservable<ResultPatternEnum> ResultPattern => resultPatternSubject;
        
        // リザルトの現在のパターン
        private Subject<ResultPatternEnum> resultPatternSubject{get;} = new Subject<ResultPatternEnum>();

        /// <summary>
        /// リザルトサブジェクトOnNext代入関数
        /// </summary>
        /// <param name="value">代入したいリザルトパターン</param>
        public void SetResultPatterunSubject(ResultPatternEnum value)
        {
            resultPatternSubject.OnNext(value);
        }
        
    }
}