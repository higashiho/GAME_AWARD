using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
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
        /// 挙動管理クラス
        /// </summary>
        private PlayerMove move;

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
            move = new PlayerMove();
        }

        /// <summary>
        /// 更新関数
        /// </summary>
        public async void Update()
        {
            
            // インプットイベントが代入されるまで一旦待つ
            await UniTask.WaitWhile(() => inputEvent == null);


            // TODO : Reyを飛ばして1つ隣のオブジェクトを判断
            // オブジェクトに対して格納されたオブジェクトが指定のものの場合指定ごとの処理


            // タイトルPlayer入力確認ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => inputEvent.Update())
                .AddTo(Object);

            
            // イベント処理生成
            //setInputEvent();
            setMovementLoops();
        }   

        /// <summary>
        /// 移動更新ループ設定関数
        /// </summary>
        private void setMovementLoops()
        {
            // 前方移動
            Object.UpdateAsObservable()
                .Where(_ => Input.GetKey(KeyCode.W))
                .Subscribe(_ => move.ForwardMovement())
                .AddTo(Object);

            // 左移動
            Object.UpdateAsObservable()
                .Where(_ => Input.GetKey(KeyCode.A))
                .Subscribe(_ => move.LeftMovement())
                .AddTo(Object);

            // 後方移動
            Object.UpdateAsObservable()
                .Where(_ => Input.GetKey(KeyCode.S))
                .Subscribe(_ => move.BackMovement())
                .AddTo(Object);

            // 右移動
            Object.UpdateAsObservable()
                .Where(_ => Input.GetKey(KeyCode.D))
                .Subscribe(_ => move.RightMovement())
                .AddTo(Object);
        }

        /// <summary>
        /// 入力イベント処理代入関数
        /// </summary>
        private void setInputEvent()
        {
            // 以下移動入力イベント処理=========================
           
            // ===============================================
        }
    }

    

    /// <summary>
    /// タイトル入力イベント管理クラス
    /// </summary>
    public sealed class PlayerInputEvent
    {
        // 以下入力イベント==================================================
        
        // =================================================================


        
        // 以下入力イベント実装===============================================
       
        // =================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpObj">イベント対象オブジェクト</param>
        public PlayerInputEvent(GameObject tmpObj)
        {
            // 以下指定オブジェクトDestroy時にイベント破棄設定=========
           
            // =====================================================
        }

        /// <summary>
        /// 入力更新関数
        /// </summary>
        public void Update()
        {
            // 以下各種入力をReactivePropertyに反映=========================
            
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
            ObjectManager.Player.Object.transform.position += Vector3.left * Time.deltaTime;
        }
        /// <summary>
        /// 右移動処理
        /// </summary>
        public void RightMovement()
        {
            ObjectManager.Player.Object.transform.position += Vector3.right * Time.deltaTime;
            
        }
        /// <summary>
        /// 前移動処理
        /// </summary>
        public void ForwardMovement()
        {
            ObjectManager.Player.Object.transform.position += Vector3.forward * Time.deltaTime;
        }
        /// <summary>
        /// 後ろ移動処理
        /// </summary>
        public void BackMovement()
        {
            ObjectManager.Player.Object.transform.position += Vector3.back * Time.deltaTime;
        }
    }
}

