using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using Constants;

namespace Title
{
    /// <summary>
    /// タイトル入力イベント管理クラス
    /// </summary>
    public sealed class EventsManager
    {
           
        /// <summary>インプットイベント管理クラスインスタンス化</summary>
        public InputManager InputSetting{get;} = new InputManager();

        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpObj">イベント対象オブジェクト</param>
        public EventsManager(GameObject manageObject)
        {
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
        public IObservable<GameObject> HoldPlayerObject => HavePlayerObject;
        public IObservable<GameObject> HoldSubPlayerObject => HaveSubPlayerObject;
        // カメラリセットイベント
        public IReadOnlyReactiveProperty<bool> ResetCameraToStart => keyResetInput;
        public IObservable<uint> KeyPressed => keyPressed;

        // =================================================================


        
        // 以下入力イベント実装===============================================
        // リセットボタン判断リアクティブプロパティ
        private readonly ReactiveProperty<bool> keyResetInput = new BoolReactiveProperty();
        // 所持オブジェクト判断サブジェクト
        public Subject<GameObject> HavePlayerObject{get;} = new Subject<GameObject>();
        public Subject<GameObject> HaveSubPlayerObject{get;} = new Subject<GameObject>();
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
                    Debug.Log((uint)keyCode);
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


        /// <summary>
        /// ゲームスタートイベント処理
        /// </summary>
        private void gameStartEvents()
        {
            // ゲームスタートイベント============================================
            // メインプレイヤー用イベント
            ObjectManager.Events.HoldPlayerObject
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトを持っているか
                .Where(x => x)
                // スタート用のオブジェクトが入っているか
                .Where(x => x.name == "GasBurner")
                // シーン転移が可能か
                .Where(_ => OnSceneMoveFlag)
                // 実施
                .Subscribe(x =>
                {
                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // フラグを折る
                    OnSceneMoveFlag = false;
                    // シーン転移
                    ObjectManager.TitleScene.Move.GameStartMovement(x);
                    Debug.Log("SceneMove");
                });

            // サブプレイヤー用イベント
            ObjectManager.Events.HaveSubPlayerObject
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトを持っているか
                .Where(x => x)
                // スタート用のオブジェクトが入っているか
                .Where(x => x.name == "GasBurner")
                // シーン転移が可能か
                .Where(_ => OnSceneMoveFlag)
                // 実施
                .Subscribe(x =>
                {
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
            ObjectManager.Events.HoldPlayerObject
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerかSubPlayerのRayにスタート用のオブジェクトが入っているか
                .Where(x => x.name == "RecipeBook")
                // 押されて指定秒経っていたら
                // レシピ本は挙動が二つある為２倍する
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount * 2))
                // 実施
                .Subscribe(async x =>
                {
                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // UI表示
                    await ObjectManager.TitleScene.Move.OpenRecipeBook(x);
                    
                    // フラグを立てる
                    nowOpenRecipeBook = true;
                });

            // サブプレイヤー用イベント
            ObjectManager.Events.HaveSubPlayerObject
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerかSubPlayerのRayにスタート用のオブジェクトが入っているか
                .Where(x => x.name == "RecipeBook")
                // 押されて指定秒経っていたら
                // レシピ本は挙動が二つある為２倍する
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount * 2))
                // 実施
                .Subscribe(async x =>
                {
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
            ObjectManager.Events.HoldPlayerObject
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerのRayにスタート用のオブジェクトが入っているか
                .Where(x => x.name == "Refrugerator")
                // 押されて指定秒経っていたら
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount))
                // 実施
                .Subscribe(async x =>
                {
                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // UI表示関数実行
                    await ObjectManager.TitleScene.Move.OpenRefrugerator(x);
               
                    // フラグを立てる
                    nowOpenRefrugerator = true;
                });

            // サブプレイヤー用イベント
            ObjectManager.Events.HaveSubPlayerObject
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerのRayにスタート用のオブジェクトが入っているか
                .Where(x => x.name == "Refrugerator")
                // 押されて指定秒経っていたら
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount))
                // 実施
                .Subscribe(async x =>
                {
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
        public  void SetEvents()
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


        /// <summary>
        /// 左移動イベント処理
        /// </summary>
        /// <param name="leftKey">左移動Key</param>
        /// <param name="tmpPlayer">動かす対象マネージャー</param>
        private void leftMovementEvent(KeyCode leftKey, PlayerManager tmpPlayer)
        {
            // 前方移動
            tmpPlayer.Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(leftKey))
                // イベントが実行中でないか
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => OutGameConstants.PLAYER_DIRECTION_LEFT != tmpPlayer.HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => ObjectManager.Player.MoveDis == Vector3.zero ||
                            ObjectManager.Player.MoveDis == Vector3.left)
                // 実行
                .Subscribe(_ => tmpPlayer.Move.LeftMovement(tmpPlayer))
                // 指定のオブジェクトが消えるまで
                .AddTo(tmpPlayer.Object);

        }

        /// <summary>
        /// 右移動イベント処理
        /// </summary>
        /// <param name="rightKey">右移動Key</param>
        /// <param name="tmpPlayer">動かす対象マネージャー</param>
        private void rightMovementEvent(KeyCode rightKey, PlayerManager tmpPlayer)
        {
            // 前方移動
            tmpPlayer.Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(rightKey))
                // イベントが実行中でないか
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => OutGameConstants.PLAYER_DIRECTION_RIGHT != tmpPlayer.HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => ObjectManager.Player.MoveDis == Vector3.zero ||
                            ObjectManager.Player.MoveDis == Vector3.right)
                // 実行
                .Subscribe(_ => tmpPlayer.Move.RightMovement(tmpPlayer))
                // 指定のオブジェクトが消えるまで
                .AddTo(tmpPlayer.Object);

        }

        /// <summary>
        /// 前方移動イベント処理
        /// </summary>
        /// <param name="forwardKey">前方移動Key</param>
        /// <param name="tmpPlayer">動かす対象マネージャー</param>
        private void forwardMovementEvent(KeyCode forwardKey, PlayerManager tmpPlayer)
        {
            // 前方移動
            tmpPlayer.Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(forwardKey))
                // イベントが実行中でないか
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => Vector3.zero != tmpPlayer.HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => ObjectManager.Player.MoveDis == Vector3.zero ||
                            ObjectManager.Player.MoveDis == Vector3.forward)
                // 実行
                .Subscribe(_ => tmpPlayer.Move.ForwardMovement(tmpPlayer))
                // 指定のオブジェクトが消えるまで
                .AddTo(tmpPlayer.Object);

        }

        /// <summary>
        /// 後方移動イベント処理
        /// </summary>
        /// <param name="backkey">後方移動Key</param>
        /// <param name="tmpPlayer">動かす対象マネージャー</param>
        private void backMovementEvent(KeyCode backkey, PlayerManager tmpPlayer)
        {
            // 前方移動
            tmpPlayer.Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(backkey))
                // イベントが実行中でないか
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => OutGameConstants.PLAYER_DIRECTION_BACK != tmpPlayer.HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => ObjectManager.Player.MoveDis == Vector3.zero ||
                            ObjectManager.Player.MoveDis == Vector3.back)
                // 実行
                .Subscribe(_ => tmpPlayer.Move.BackMovement(tmpPlayer))
                // 指定のオブジェクトが消えるまで
                .AddTo(tmpPlayer.Object);

        }

        /// <summary>
        /// アニメーション初期化イベント処理
        /// </summary>
        /// <param name="forwardKey">前方移動Key</param>
        /// <param name="leftKey">左移動Key</param>
        /// <param name="backkey">後方移動Key</param>
        /// <param name="rightKey">右移動Key</param>
        /// <param name="tmpPlayer">動かす対象マネージャー</param>
        private void resetAnimEvents(KeyCode forwardKey, KeyCode leftKey, KeyCode backkey, KeyCode rightKey, PlayerManager tmpPlayer)
        {
            // アニメーション初期化
            tmpPlayer.Object.UpdateAsObservable()
                // どの移動ボタンも押されていないとき
                .Where(_ => !Input.GetKey(forwardKey)&& 
                            !Input.GetKey(leftKey)&&
                            !Input.GetKey(backkey)&&
                            !Input.GetKey(rightKey))
                // 実行
                .Subscribe(_ => tmpPlayer.Move.ResetAnim(tmpPlayer))
                // 指定のオブジェクトが消えるまで
                .AddTo(tmpPlayer.Object);
        }

        /// <summary>
        /// 挙動初期化イベント処理
        /// </summary>
        /// <param name="forwardKey">前方移動Key</param>
        /// <param name="leftKey">左移動Key</param>
        /// <param name="backkey">後方移動Key</param>
        /// <param name="rightKey">右移動Key</param>
        /// <param name="tmpPlayer">動かす対象マネージャー</param>
        private void resetMovementEvents(KeyCode forwardKey, KeyCode leftKey, KeyCode backkey, KeyCode rightKey, PlayerManager tmpPlayer)
        {
            // 挙動初期化
            tmpPlayer.Object.UpdateAsObservable()
                // どの移動ボタンも押されていないとき
                .Where(_ => Input.GetKeyUp(forwardKey)|| 
                            Input.GetKeyUp(leftKey)||
                            Input.GetKeyUp(backkey)||
                            Input.GetKeyUp(rightKey))
                // 実行
                .Subscribe(_ => tmpPlayer.Move.ResetMovement(tmpPlayer))
                // 指定のオブジェクトが消えるまで
                .AddTo(tmpPlayer.Object);
        }

        /// <summary>
        /// プレイヤー移動更新ループ設定関数
        /// </summary>
        public void SetMovementLoops()
        {
            // 前方移動
            forwardMovementEvent(KeyCode.W, ObjectManager.Player);
            
            // 左移動
            leftMovementEvent(KeyCode.A, ObjectManager.Player);

            // 後方移動
            backMovementEvent(KeyCode.S, ObjectManager.Player);

            // 右移動
            rightMovementEvent(KeyCode.D, ObjectManager.Player);

            // アニメーション初期化
            resetAnimEvents(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, ObjectManager.Player);

            // 挙動初期化
            resetMovementEvents(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, ObjectManager.Player);
        }

        /// <summary>
        /// ２P用移動関数
        /// </summary>
        public void SetSubPlayerMovementLoops()
        {
            // 前方移動
            forwardMovementEvent(KeyCode.UpArrow, ObjectManager.SubPlayer);
            
            // 左移動
            leftMovementEvent(KeyCode.LeftArrow, ObjectManager.SubPlayer);

            // 後方移動
            backMovementEvent(KeyCode.DownArrow, ObjectManager.SubPlayer);

            // 右移動
            rightMovementEvent(KeyCode.RightArrow, ObjectManager.SubPlayer);

            // アニメーション初期化
            resetAnimEvents(KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow, ObjectManager.SubPlayer);

            // 挙動初期化
            resetMovementEvents(KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow, ObjectManager.SubPlayer);
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
                .Where(_ => ObjectManager.Player.HitObject?.name == ("Refrugerator") ||
                            ObjectManager.SubPlayer.HitObject?.name == ("Refrugerator"))
                // 目標座標が接近座標の場合
                .Where(x => x == OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x => {
                    ObjectManager.Text.Move.FoodNicknamesTextMovement(x);
                    //　座標目標値設定
                    ObjectManager.Events.FoodNicknamesTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y);
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
                .Where(_ => ObjectManager.Player.HitObject?.name == ("RecipeBook") ||
                            ObjectManager.SubPlayer.HitObject?.name == ("RecipeBook"))
                // 目標座標が接近座標の場合
                .Where(x => x == OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x => {
                    ObjectManager.Text.Move.DisplayIngredientsListTextMovement(x);                    
                    //　座標目標値設定
                    ObjectManager.Events.DisplayIngredientsListTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y);
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
                .Where(_ => ObjectManager.Player.HitObject?.name == ("GasBurner") ||
                            ObjectManager.SubPlayer.HitObject?.name == ("GasBurner"))
                // 目標座標が接近座標の場合
                .Where(x => x == OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x =>{
                    ObjectManager.Text.Move.GameStartTextMovement(x);
                    //　座標目標値設定
                    ObjectManager.Events.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y);
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
                if(ObjectManager.TitleScene.TextImageCanvas[0].transform.localPosition.y == OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y &&
                ObjectManager.TitleScene.TextImageCanvas[1].transform.localPosition.y == OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y &&
                ObjectManager.TitleScene.TextImageCanvas[2].transform.localPosition.y == OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y)
                {
                    await UniTask.Yield();
                    continue;
                }


                
                for(int i = 0; i < ObjectManager.TitleScene.TextImageCanvas.Length; i++)
                {
                    // 自身が初期座標にいた場合処理を通過
                    if(ObjectManager.TitleScene.TextImageCanvas[i].transform.localPosition.y == OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y)
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
                        ObjectManager.TitleScene.TextImageCanvas[i].transform.forward, 
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

