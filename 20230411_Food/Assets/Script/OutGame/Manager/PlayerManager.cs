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
using Constants;

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
        public PlayerMove Move;

        /// <summary>
        /// イベント管理クラス
        /// </summary>
        public EventManager Events{get{return events;}}
        private EventManager events;

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
        /// playerの挙動アニメーション
        /// </summary>
        public Animator MoveAnimator{get;private set;}

        /// <summary>
        /// rayが当たった対象オブジェクト
        /// </summary>
        /// <value></value>
        public GameObject HitObject{get{return hitObject;} set{hitObject = value;}}
        private GameObject hitObject = null;
        /// <summary>
        /// オブジェクトにRayが当たった時のプレイヤーの向き
        /// </summary>
        /// <value></value>
        public Vector3? HitDistance{get{return hitDistance;} set{hitDistance = value;}}
        private Vector3? hitDistance = null;

        /// <summary>
        /// ray処理
        /// </summary>
        private PlayerRayProcessing rayProcessing;

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

            MoveAnimator = Object.transform.GetChild(0).GetComponent<Animator>();
        
            // 入力イベントインスタンス化
            inputEvent = new PlayerInputEvent(Object);
            Move = new PlayerMove();
            events = new EventManager();
            rayProcessing = new PlayerRayProcessing();
        }

        /// <summary>
        /// 更新関数
        /// </summary>
        public async void Update()
        {
            
            // インプットイベントが代入されるまで一旦待つ
            await UniTask.WaitWhile(() => inputEvent == null);


            // Ray用ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => {
                    rayProcessing.Processing();
                })
                .AddTo(Object);

            // タイトルPlayer入力確認ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => inputEvent.Update())
                .AddTo(Object);

            
            // イベント処理生成
            //setInputEvent();
            events.SetMovementLoops();
        }   

        
        /// <summary>
        /// 入力イベント処理代入関数
        /// </summary>
        private void setInputEvent()
        {
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
            // どの向きに歩いているか設定
            ObjectManager.Player.Events.PlayerMoveDis = Vector3.left;
            
            // 移動アニメーション再生
            ObjectManager.Player.MoveAnimator.SetBool("Move", true);

            // ９０度左を向く
            ObjectManager.Player.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_LEFT;

            // 移動
            ObjectManager.Player.Object.transform.position += Vector3.left * Time.deltaTime;
        }
        /// <summary>
        /// 右移動処理
        /// </summary>
        public void RightMovement()
        {
            // どの向きに歩いているか設定
            ObjectManager.Player.Events.PlayerMoveDis = Vector3.right;
            
            // 移動アニメーション再生
            ObjectManager.Player.MoveAnimator.SetBool("Move", true);

            // ９０度右を向く
            ObjectManager.Player.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_RIGHT;

            // 移動
            ObjectManager.Player.Object.transform.position += Vector3.right * Time.deltaTime;
            
        }
        /// <summary>
        /// 前移動処理
        /// </summary>
        public void ForwardMovement()
        {
            // どの向きに歩いているか設定
            ObjectManager.Player.Events.PlayerMoveDis = Vector3.forward;
            
            // 移動アニメーション再生
            ObjectManager.Player.MoveAnimator.SetBool("Move", true);

            // 前を向く
            ObjectManager.Player.Object.transform.eulerAngles = Vector3.zero;

            // 移動
            ObjectManager.Player.Object.transform.position += Vector3.forward * Time.deltaTime;
        }
        /// <summary>
        /// 後ろ移動処理
        /// </summary>
        public void BackMovement()
        {
            // どの向きに歩いているか設定
            ObjectManager.Player.Events.PlayerMoveDis = Vector3.back;

            // 移動アニメーション再生
            ObjectManager.Player.MoveAnimator.SetBool("Move", true);

            // 後ろを向く
            ObjectManager.Player.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_BACK;

            // 移動
            ObjectManager.Player.Object.transform.position += Vector3.back * Time.deltaTime;
        }
        
        /// <summary>
        /// アニメーションリセット関数
        /// </summary>
        public void ResetAnim()
        {
            // 初期化
            ObjectManager.Player.MoveAnimator.SetBool("Move", false);
        }

        /// <summary>
        /// 挙動リセット関数
        /// </summary>
        public void ResetMovement()
        {
            // 初期化
            ObjectManager.Player.Events.PlayerMoveDis = Vector3.zero;
        }
    }

    public class PlayerRayProcessing
    {
        
        /// <summary>
        /// PlayerのRayの長さ
        /// </summary>
        private RayDistance rayDistance = new RayDistance(OutGameConstants.PLAYER_RAY_DISTANCE);

        /// <summary>
        /// Ray処理
        /// </summary>
        public void Processing()
        {
            var forwardRay = new Ray(ObjectManager.Player.Object.transform.position, ObjectManager.Player.Object.transform.forward);
            Debug.DrawRay(ObjectManager.Player.Object.transform.position, ObjectManager.Player.Object.transform.forward * rayDistance.DistanceAmount, Color.blue);
        
            RaycastHit hit;
            // rayの当たり判定を確認
            if(Physics.Raycast(forwardRay, out hit, rayDistance.DistanceAmount))
            {
                // 当たっていたら向き格納
                ObjectManager.Player.HitDistance = ObjectManager.Player.Object.transform.eulerAngles;
                // 当たっていたらオブジェクト格納
                ObjectManager.Player.HitObject = hit.collider.gameObject;
            }
            else 
            {
                // 当たっていなかったらnullに変換
                ObjectManager.Player.HitDistance = null;
                ObjectManager.Player.HitObject = null;
            }
        }
    }
}

