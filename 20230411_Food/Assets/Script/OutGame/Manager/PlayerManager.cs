using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UniRx;
using UniRx.Triggers;
using ObjectInterface;

namespace Title
{
    public class PlayerManager : IActor
    {
        /// <summary>
        /// 入力イベント
        /// </summary>
        private PlayerInputEvent inputEvent;

        /// <summary>
        /// プレイヤーオブジェクト
        /// </summary>
        public GameObject Object{get; private set;}

        /// <summary>
        /// プレイヤーのハンドル
        /// </summary>
        /// <value></value>
        public AsyncOperationHandle Handle{get;private set;}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlayerManager()
        {

            // 初期化
            Initialization();


            // 更新ループ設定
            Update();
       }

        /// <summary>
        /// 初期化関数
        /// </summary>
        public async void Initialization()
        {
            // 取得し完了するまで待つ
            Handle = Addressables.LoadAssetAsync<GameObject>("Player");
            await Handle.Task;

            // プレイヤー生成
            // ゲームオブジェクト型にcastし生成
            var tmpObj = (GameObject)Handle.Result;
            Object = MonoBehaviour.Instantiate(tmpObj, tmpObj.transform.position, Quaternion.identity);
            

            // 入力イベントインスタンス化
            inputEvent = new PlayerInputEvent(Object);
            
            // 以下移動入力イベント処理=========================
            // 左移動
            inputEvent.LeftMove
                // イベント指定した入力がされているか
                .Where(x => x)
                // 実施
                .Subscribe(_ =>
                {
                    // 移動処理
                    Debug.Log("LeftMove");
                });
            // 右移動
            inputEvent.RightMove
                // イベント指定した入力がされているか
                .Where(x => x)
                // 実施
                .Subscribe(_ =>
                {
                    // 移動処理
                    Debug.Log("RightMove");
                });
            // 前移動
            inputEvent.ForwardMove
                // イベント指定した入力がされているか
                .Where(x => x)
                // 実施
                .Subscribe(_ =>
                {
                    // 移動処理
                    Debug.Log("ForwardMove");
                });
            // 後ろ移動
            inputEvent.BakeMove
                // イベント指定した入力がされているか
                .Where(x => x)
                // 実施
                .Subscribe(_ =>
                {
                    // 移動処理
                    Debug.Log("BakeMove");
                });
            // ===============================================
        }

        /// <summary>
        /// 更新関数
        /// </summary>
        public void Update()
        {
            // TODO : Reyを飛ばして1つ隣のオブジェクトを判断
            // オブジェクトに対して格納されたオブジェクトが指定のものの場合指定ごとの処理

            // タイトルPlayer入力確認ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => inputEvent.Update())
                .AddTo(Object);
        }
    }

    

    /// <summary>
    /// タイトル入力イベント管理クラス
    /// </summary>
    public sealed class PlayerInputEvent
    {
        // 以下入力イベント==================================================
        public IReadOnlyReactiveProperty<bool> LeftMove => leftMove;
        public IReadOnlyReactiveProperty<bool> RightMove => rightMove;
        public IReadOnlyReactiveProperty<bool> ForwardMove => forwardMove;
        public IReadOnlyReactiveProperty<bool> BakeMove => bakeMove;

        // =================================================================


        
        // 以下入力イベント実装===============================================
        private readonly ReactiveProperty<bool> leftMove = new BoolReactiveProperty();
        private readonly ReactiveProperty<bool> rightMove = new BoolReactiveProperty();
        private readonly ReactiveProperty<bool> forwardMove = new BoolReactiveProperty();
        private readonly ReactiveProperty<bool> bakeMove = new BoolReactiveProperty();
        // =================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpObj">イベント対象オブジェクト</param>
        public PlayerInputEvent(GameObject tmpObj)
        {
            // 以下指定オブジェクトDestroy時にイベント破棄設定=========
            leftMove.AddTo(tmpObj);
            rightMove.AddTo(tmpObj);
            forwardMove.AddTo(tmpObj);
            bakeMove.AddTo(tmpObj);
            // =====================================================
        }

        /// <summary>
        /// 入力更新関数
        /// </summary>
        public void Update()
        {
            // 以下各種入力をReactivePropertyに反映=========================
            leftMove.Value = Input.GetKeyDown(KeyCode.A);
            rightMove.Value = Input.GetKeyDown(KeyCode.D);
            forwardMove.Value = Input.GetKeyDown(KeyCode.W);
            bakeMove.Value = Input.GetKeyDown(KeyCode.S);
            // ===========================================================
        }
    }

    public class PlayerMove
    {
        /// <summary>
        /// 左移動処理
        /// </summary>
        public void LeftMovement()
        {

        }
        /// <summary>
        /// 右移動処理
        /// </summary>
        public void RightMovement()
        {
            
        }
        /// <summary>
        /// 前移動処理
        /// </summary>
        public void ForwardMovement()
        {
            
        }
        /// <summary>
        /// 後ろ移動処理
        /// </summary>
        public void BackMovement()
        {
            
        }
    }
}

