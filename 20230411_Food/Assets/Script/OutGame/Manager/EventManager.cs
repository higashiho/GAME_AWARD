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
    public sealed class EventManager
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
        /// プレイヤーが動いている向き
        /// </summary>
        public Vector3 PlayerMoveDis{get{return playerMoveDis;} set{playerMoveDis = value;}}
        private Vector3 playerMoveDis = Vector3.zero;

        /// <summary>
        /// タイトル入力イベント設定関数
        /// </summary>
        public  void SetInputEvents()
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

            // 食材相性UI表示イベント==============================================
            ObjectManager.TitleScene.InputEvent.FoodNicknames
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerのRayにスタート用のオブジェクトが入っているか
                .Where(_ => ObjectManager.Player.HitObject != null &&
                ObjectManager.Player.HitObject.name == "RecipeBook")
                // イベントが処理されていなければ
                .Where(_ => !!ObjectManager.TitleScene.NowPlayeEvents)
                // 押されて指定秒経っていたら
                // レシピ本は挙動が二つある為２倍する
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.MoveTimeAmount * 2))
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

            // 食材一覧表示イベント================================================
            ObjectManager.TitleScene.InputEvent.DisplayIngredientsList
                // イベント指定した入力がされているか
                .Where(x => x)
                // playerのRayにスタート用のオブジェクトが入っているか
                .Where(_ => ObjectManager.Player.HitObject != null &&
                ObjectManager.Player.HitObject.name == "Refrugerator")
                // イベントが処理されていなければ
                .Where(_ => !!ObjectManager.TitleScene.NowPlayeEvents)
                // 押されて指定秒経っていたら
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.MoveTimeAmount))
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

            // カメラリセットイベント==============================================
            ObjectManager.TitleScene.InputEvent.ResetCameraToStart
                // どれかのフラグがたっているか
                .Where(_ => nowOpenRecipeBook ||
                            nowOpenRefrugerator)
                // イベント指定した入力がされているか
                .Where(x => x)
                // 押されて指定秒経っていたら
                .ThrottleFirst(TimeSpan.FromSeconds(moveTime.MoveTimeAmount))
                // 実施
                .Subscribe(async _ =>
                {
                    // イベント実行フラグを立てる
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
        /// プレイヤー移動更新ループ設定関数
        /// </summary>
        public void SetMovementLoops()
        {
            // 前方移動
            ObjectManager.Player.Object.UpdateAsObservable()
                // wが押されているか
                .Where(_ => Input.GetKey(KeyCode.W))
                // イベントが実行中でないか
                .Where(_ => !!ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => Vector3.zero != ObjectManager.Player.HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => PlayerMoveDis == Vector3.zero ||
                            PlayerMoveDis == Vector3.forward)
                // 実行
                .Subscribe(_ => ObjectManager.Player.Move.ForwardMovement())
                .AddTo(ObjectManager.Player.Object);

            // 左移動
            ObjectManager.Player.Object.UpdateAsObservable()
                // aが押されているか
                .Where(_ => Input.GetKey(KeyCode.A))
                // イベントが実行中でないか
                .Where(_ => !!ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => OutGameConstants.PLAYER_DIRECTION_LEFT  != ObjectManager.Player.HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => PlayerMoveDis == Vector3.zero ||
                            PlayerMoveDis == Vector3.left)
                // 実行
                .Subscribe(_ => ObjectManager.Player.Move.LeftMovement())
                .AddTo(ObjectManager.Player.Object);

            // 後方移動
            ObjectManager.Player.Object.UpdateAsObservable()
                // sを押しているか
                .Where(_ => Input.GetKey(KeyCode.S))
                // イベントが実行中でないか
                .Where(_ => !!ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => OutGameConstants.PLAYER_DIRECTION_BACK != ObjectManager.Player.HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => PlayerMoveDis == Vector3.zero ||
                            PlayerMoveDis == Vector3.back)
                // 実行
                .Subscribe(_ => ObjectManager.Player.Move.BackMovement())
                .AddTo(ObjectManager.Player.Object);

            // 右移動
            ObjectManager.Player.Object.UpdateAsObservable()
                // dを押しているか
                .Where(_ => Input.GetKey(KeyCode.D))
                // イベントが実行中でないか
                .Where(_ => !!ObjectManager.TitleScene.NowPlayeEvents)
                // オブジェクトに当たっている場合当たった向きが指定の向きじゃないか
                .Where(_ => OutGameConstants.PLAYER_DIRECTION_RIGHT != ObjectManager.Player.HitDistance)
                // playerが待機中か同じ向きに動いているか
                .Where(_ => PlayerMoveDis == Vector3.zero ||
                            PlayerMoveDis == Vector3.right)
                // 実行
                .Subscribe(_ => ObjectManager.Player.Move.RightMovement())
                .AddTo(ObjectManager.Player.Object);

            // アニメーション初期化
            ObjectManager.Player.Object.UpdateAsObservable()
                // どの移動ボタンも押されていないとき
                .Where(_ => !Input.GetKey(KeyCode.W)&& 
                            !Input.GetKey(KeyCode.A)&&
                            !Input.GetKey(KeyCode.S)&&
                            !Input.GetKey(KeyCode.D))
                // 実行
                .Subscribe(_ => ObjectManager.Player.Move.ResetAnim())
                .AddTo(ObjectManager.Player.Object);

            // 挙動初期化
            ObjectManager.Player.Object.UpdateAsObservable()
                // どの移動ボタンも押されていないとき
                .Where(_ => Input.GetKeyUp(KeyCode.W)|| 
                            Input.GetKeyUp(KeyCode.A)||
                            Input.GetKeyUp(KeyCode.S)||
                            Input.GetKeyUp(KeyCode.D))
                // 実行
                .Subscribe(_ => ObjectManager.Player.Move.ResetMovement())
                .AddTo(ObjectManager.Player.Object);
        }

    }
}

