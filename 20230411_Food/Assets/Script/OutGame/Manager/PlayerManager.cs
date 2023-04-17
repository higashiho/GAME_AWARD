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
    /// <summary>
    /// player管理クラス
    /// </summary>
    public class PlayerManager : IActor
    {
        /// <summary>
        /// playerが1pか2pか判断ステート
        /// </summary>
        public enum PlayerState
        {
            MAIN, SUB
        }
        public PlayerState JudgeState;


        /// <summary>
        /// 挙動管理クラス
        /// </summary>
        public PlayerMove Move;

        /// <summary>
        /// イベント管理クラス
        /// </summary>
        public InputMovementEventManager Events{get{return events;}}
        private InputMovementEventManager events;

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

        /// ray処理
        private PlayerRayProcessing rayProcessing;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlayerManager(PlayerState tmpState)
        {
            JudgeState = tmpState;
            // 初期化
            Initialization();
            
            
            // 更新ループ設定
            if(JudgeState == PlayerState.MAIN)
                Update();
            // 更新ループ設定
            else if(JudgeState == PlayerState.SUB)
                SubUpdate();
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
            // 自身がどっちなのか判断して生成
            if(JudgeState == PlayerState.MAIN)
                Object = MonoBehaviour.Instantiate(tmpObj, tmpObj.transform.position, Quaternion.identity);
            else if(JudgeState == PlayerState.SUB)
            {
                // Playerと反対に生成するためx座標のみ反転
                var instancePos = new Vector3(
                    -tmpObj.transform.position.x,
                    tmpObj.transform.position.y,
                    tmpObj.transform.position.z
                );
                Object = MonoBehaviour.Instantiate(tmpObj, instancePos, Quaternion.identity);
            }

            MoveAnimator = Object.transform.GetChild(0).GetComponent<Animator>();
        
            // インスタンス化
            Move = new PlayerMove();
            events = new InputMovementEventManager();
            rayProcessing = new PlayerRayProcessing();
        }

        /// <summary>
        /// 更新関数
        /// </summary>
        public async void Update()
        {

            // イベントが代入されるまで一旦待つ
            await UniTask.WaitWhile(() => events == null);

            // Ray用ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => {
                    rayProcessing.Processing();
                })
                .AddTo(Object);


            
            // イベント処理生成
            //setInputEvent();
            events.SetMovementLoops();
        }   

        /// <summary>
        /// 2P用更新設定関数
        /// </summary>
        public async void SubUpdate()
        {
            
            // イベントが代入されるまで一旦待つ
            await UniTask.WaitWhile(() => events == null);

            // Ray用ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => {
                    rayProcessing.SubProcessing();
                })
                .AddTo(Object);
            
            // イベント処理生成
            //setInputEvent();
            events.SetSubPlayerMovementLoops();
        }

        
        /// <summary>
        /// 入力イベント処理代入関数
        /// </summary>
        private void setInputEvent()
        {
        }
    }

    /// <summary>
    /// Player挙動管理クラス
    /// </summary>
    public sealed class PlayerMove
    {
        /// <summary>
        /// 左移動処理
        /// </summary>
        public void LeftMovement(PlayerManager tmpPlayer)
        {
            // どの向きに歩いているか設定
            tmpPlayer.Events.PlayerMoveDis = Vector3.left;
            
            // 移動アニメーション再生
            tmpPlayer.MoveAnimator.SetBool("Move", true);

            // ９０度左を向く
            tmpPlayer.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_LEFT;

            // 移動
            tmpPlayer.Object.transform.position += Vector3.left * Time.deltaTime;
        }
        /// <summary>
        /// 右移動処理
        /// </summary>
        public void RightMovement(PlayerManager tmpPlayer)
        {
            // どの向きに歩いているか設定
            tmpPlayer.Events.PlayerMoveDis = Vector3.right;
            
            // 移動アニメーション再生
            tmpPlayer.MoveAnimator.SetBool("Move", true);

            // ９０度右を向く
            tmpPlayer.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_RIGHT;

            // 移動
            tmpPlayer.Object.transform.position += Vector3.right * Time.deltaTime;
            
        }
        /// <summary>
        /// 前移動処理
        /// </summary>
        public void ForwardMovement(PlayerManager tmpPlayer)
        {
            // どの向きに歩いているか設定
            tmpPlayer.Events.PlayerMoveDis = Vector3.forward;
            
            // 移動アニメーション再生
            tmpPlayer.MoveAnimator.SetBool("Move", true);

            // 前を向く
            tmpPlayer.Object.transform.eulerAngles = Vector3.zero;

            // 移動
            tmpPlayer.Object.transform.position += Vector3.forward * Time.deltaTime;
        }
        /// <summary>
        /// 後ろ移動処理
        /// </summary>
        public void BackMovement(PlayerManager tmpPlayer)
        {
            // どの向きに歩いているか設定
            tmpPlayer.Events.PlayerMoveDis = Vector3.back;

            // 移動アニメーション再生
            tmpPlayer.MoveAnimator.SetBool("Move", true);

            // 後ろを向く
            tmpPlayer.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_BACK;

            // 移動
            tmpPlayer.Object.transform.position += Vector3.back * Time.deltaTime;
        }
        
        /// <summary>
        /// アニメーションリセット関数
        /// </summary>
        public void ResetAnim(PlayerManager tmpPlayer)
        {
            // 初期化
            tmpPlayer.MoveAnimator.SetBool("Move", false);
        }

        /// <summary>
        /// 挙動リセット関数
        /// </summary>
        public void ResetMovement(PlayerManager tmpPlayer)
        {
            // 初期化
            tmpPlayer.Events.PlayerMoveDis = Vector3.zero;
        }
    }

    /// <summary>
    /// PlayerRay管理クラス
    /// </summary>
    public sealed class PlayerRayProcessing
    {
        
        /// <summary>
        /// PlayerのRayの長さ
        /// </summary>
        private RayDistance rayDistance = new RayDistance(1);

        /// <summary>
        /// Ray処理
        /// </summary>
        public void Processing()
        {
            var forwardRay = new Ray(ObjectManager.Player.Object.transform.position, ObjectManager.Player.Object.transform.forward);
            Debug.DrawRay(ObjectManager.Player.Object.transform.position, ObjectManager.Player.Object.transform.forward * rayDistance.Amount, Color.blue);
        
            RaycastHit hit;
            // rayの当たり判定を確認
            if(Physics.Raycast(forwardRay, out hit, rayDistance.Amount))
            {
                // オブジェクトが変わっていなかったら処理中断
                if(ObjectManager.Player.HitObject == hit.collider.gameObject) return;

                
                // 当たっていたら向き格納
                ObjectManager.Player.HitDistance = ObjectManager.Player.Object.transform.eulerAngles;
                // オブジェクト格納
                ObjectManager.Player.HitObject = hit.collider.gameObject;

                // サブジェクトに代入
                if(ObjectManager.Player.HitObject.name == "Refrugerator")
                    ObjectManager.TitleScene.InputEvent.FoodNicknamesTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
                else if(ObjectManager.Player.HitObject.name == "RecipeBook")
                    ObjectManager.TitleScene.InputEvent.DisplayIngredientsListTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
                else if(ObjectManager.Player.HitObject.name == "GasBurner")
                    ObjectManager.TitleScene.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
            }
            else 
            {
                // 当たっていなかったらnullに変換
                ObjectManager.Player.HitDistance = null;
                ObjectManager.Player.HitObject = null;
            }
        }

        /// <summary>
        /// サブプレイヤー用Ray処理
        /// </summary>
        public void SubProcessing()
        {
            var forwardRay = new Ray(ObjectManager.SubPlayer.Object.transform.position, ObjectManager.SubPlayer.Object.transform.forward);
            Debug.DrawRay(ObjectManager.SubPlayer.Object.transform.position, ObjectManager.SubPlayer.Object.transform.forward * rayDistance.Amount, Color.blue);
        
            RaycastHit hit;
            // rayの当たり判定を確認
            if(Physics.Raycast(forwardRay, out hit, rayDistance.Amount))
            {
                // オブジェクトが変わっていなかったら処理中断
                if(ObjectManager.SubPlayer.HitObject == hit.collider.gameObject) return;


               
                // 当たっていたら向き格納
                ObjectManager.SubPlayer.HitDistance = ObjectManager.SubPlayer.Object.transform.eulerAngles;
                // 当たっていたらオブジェクト格納
                ObjectManager.SubPlayer.HitObject = hit.collider.gameObject;

                // サブジェクトに代入
                if(ObjectManager.SubPlayer.HitObject.name == "Refrugerator")
                    ObjectManager.TitleScene.InputEvent.FoodNicknamesTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
                else if(ObjectManager.SubPlayer.HitObject.name == "RecipeBook")
                    ObjectManager.TitleScene.InputEvent.DisplayIngredientsListTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
                else if(ObjectManager.SubPlayer.HitObject.name == "GasBurner")
                    ObjectManager.TitleScene.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);

            }
            else 
            {
                // 当たっていなかったらnullに変換
                ObjectManager.SubPlayer.HitDistance = null;
                ObjectManager.SubPlayer.HitObject = null;
            }
        }
    }
}

