using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using Constants;

namespace Title
{
    /// <summary>
    /// イベント管理クラス
    /// </summary>
    public sealed class InputEventManager
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
            ObjectManager.TitleScene.InputEvent.GameStart
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerのRayにスタート用のオブジェクトが入っているか
                .Where(_ => ObjectManager.Player.HitObject != null &&
                ObjectManager.Player.HitObject.name == "GasBurner")
                // シーン転移が可能か
                .Where(_ => OnSceneMoveFlag)
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // 実施
                .Subscribe(_ =>
                {
                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // フラグを折る
                    OnSceneMoveFlag = false;
                    // シーン転移
                    ObjectManager.TitleScene.Move.GameStartMovement();
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
            ObjectManager.TitleScene.InputEvent.FoodNicknames
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerかSubPlayerのRayにスタート用のオブジェクトが入っているか
                .Where(_ =>ObjectManager.Player.HitObject?.name == "RecipeBook" || ObjectManager.SubPlayer.HitObject?.name == "RecipeBook")
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // 押されて指定秒経っていたら
                // レシピ本は挙動が二つある為２倍する
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount * 2))
                // 実施
                .Subscribe(async _ =>
                {
                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // UI表示
                    await ObjectManager.TitleScene.Move.OpenRecipeBook();
                    
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
            ObjectManager.TitleScene.InputEvent.DisplayIngredientsList
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerのRayにスタート用のオブジェクトが入っているか
                .Where(_ => ObjectManager.Player.HitObject?.name == "Refrugerator" || ObjectManager.SubPlayer.HitObject?.name == "Refrugerator")
                // イベントが処理されていなければ
                .Where(_ => !ObjectManager.TitleScene.NowPlayeEvents)
                // 押されて指定秒経っていたら
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.Amount))
                // 実施
                .Subscribe(async _ =>
                {
                    // イベント実行フラグを立てる
                    ObjectManager.TitleScene.NowPlayeEvents = true;
                    // UI表示関数実行
                    await ObjectManager.TitleScene.Move.OpenRefrugerator();
               
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
            ObjectManager.TitleScene.InputEvent.ResetCameraToStart
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
        public  void SetInputEvents()
        {
            gameStartEvents();
            
            foodNicknamesEvents();

            displayIngredientsListEvents();
            
            resetCameraToStartEvenets();
        } 
    
    }

    /// <summary>
    /// 移動入力イベント管理クラス
    /// </summary>
    public class InputMovementEventManager
    {

        /// <summary>
        /// プレイヤーが動いている向き
        /// </summary>
        public Vector3 PlayerMoveDis{get{return playerMoveDis;} set{playerMoveDis = value;}}
        private Vector3 playerMoveDis = Vector3.zero;

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
                .Where(_ => PlayerMoveDis == Vector3.zero ||
                            PlayerMoveDis == Vector3.left)
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
                .Where(_ => PlayerMoveDis == Vector3.zero ||
                            PlayerMoveDis == Vector3.right)
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
                .Where(_ => PlayerMoveDis == Vector3.zero ||
                            PlayerMoveDis == Vector3.forward)
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
                .Where(_ => PlayerMoveDis == Vector3.zero ||
                            PlayerMoveDis == Vector3.back)
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

    public class TextApproachEventManager
    {
        private TextManager text;
        public TextApproachEventManager()
        {
            text = new TextManager();
        }

        /// <summary>
        /// 食材一覧表示テキスト接近処理イベント
        /// </summary>
        private void foodNicknamesTextApproachEvents()
        {
            // 食材一覧表示テキスト接近処理イベント設定
            ObjectManager.TitleScene.InputEvent.FoodNicknamesTextPoint
                // どちらかの取得オブジェクトが冷蔵庫の場合
                .Where(_ => ObjectManager.Player.HitObject?.name == ("Refrugerator") ||
                            ObjectManager.SubPlayer.HitObject?.name == ("Refrugerator"))
                // 目標座標が接近座標の場合
                .Where(x => x == OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x => text.Move.FoodNicknamesTextMovement(x))
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.TitleScene);
        }

        /// <summary>
        /// 食材相性表示テキスト接近処理イベント
        /// </summary>
        private void displayIngredientsListTextApproachEvents()
        {
            // 食材相性表示テキスト接近処理イベント設定
            ObjectManager.TitleScene.InputEvent.DisplayIngredientsListTextPoint
                // どちらかの取得オブジェクトがレシピブックの場合
                .Where(_ => ObjectManager.Player.HitObject?.name == ("RecipeBook") ||
                            ObjectManager.SubPlayer.HitObject?.name == ("RecipeBook"))
                // 目標座標が接近座標の場合
                .Where(x => x == OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x => text.Move.DisplayIngredientsListTextMovement(x))
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.TitleScene);
        }

        /// <summary>
        /// ゲームスタートテキスト接近処理イベント
        /// </summary>
        private void gameStartTextApproachEvents()
        {
            // ゲームスタートテキスト接近処理イベント設定
            ObjectManager.TitleScene.InputEvent.GameStartTextPoint
                // どちらかの取得オブジェクトがガスバーナーの場合
                .Where(_ => ObjectManager.Player.HitObject?.name == ("GasBurner") ||
                            ObjectManager.SubPlayer.HitObject?.name == ("GasBurner"))
                // 目標座標が接近座標の場合
                .Where(x => x == OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y)
                // 実施
                .Subscribe(x => text.Move.GameStartTextMovement(x))
                // 指定のオブジェクトが消えるまで
                .AddTo(ObjectManager.TitleScene);
        }

        /// <summary>
        /// テキストイメージリセットイベント
        /// </summary>
        private void resetTextImageEvents()
        {
            
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
            resetTextImageEvents();
        }
    }
}

