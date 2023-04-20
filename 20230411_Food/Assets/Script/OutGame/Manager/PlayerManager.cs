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
        /// プレイヤーが動いている向き
        /// </summary>
        public Vector3 MoveDis{get{return moveDis;} set{moveDis = value;}}
        private Vector3 moveDis = Vector3.zero;



        /// <summary>
        /// playerステート
        /// </summary>
        public enum PlayerState
        {
            MAIN, SUB
        }
        public PlayerState State{get; private set;}


        /// <summary>
        /// 挙動管理クラス
        /// </summary>
        public PlayerMove Move = new PlayerMove();


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
        private PlayerRayProcessing rayProcessing = new PlayerRayProcessing();

        
        /// <summary>
        /// Playerイベント管理クラス
        /// </summary>
        public InputMovementManager MoveEvents{get;} = new InputMovementManager();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlayerManager(PlayerState tmpState)
        {
            
            State = tmpState;
            // 初期化
            Initialization();
            
            
            // 更新ループ設定
            if(State == PlayerState.MAIN)
                Update();
            // 更新ループ設定
            else if(State == PlayerState.SUB)
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
            if(State == PlayerState.MAIN)
                Object = MonoBehaviour.Instantiate(tmpObj, ObjectManager.TitleScene.PlayerData.InstancePos, Quaternion.identity);
            else if(State == PlayerState.SUB)
            {
                // Playerと反対に生成するためx座標のみ反転
                var instancePos = new Vector3(
                    -ObjectManager.TitleScene.PlayerData.InstancePos.x,
                    ObjectManager.TitleScene.PlayerData.InstancePos.y,
                    ObjectManager.TitleScene.PlayerData.InstancePos.z
                );
                Object = MonoBehaviour.Instantiate(tmpObj, instancePos, Quaternion.identity);
            }

            MoveAnimator = Object.transform.GetChild(0).GetComponent<Animator>();
        
        }

        /// <summary>
        /// 更新関数
        /// </summary>
        public async void Update()
        {
            // オブジェクトが生成されるまで待つ
            await UniTask.WaitWhile(() => !Object);


            // Ray用ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => {
                    rayProcessing.Processing();
                })
                .AddTo(Object);


            MoveEvents.SetMovementLoops();
        }   

        /// <summary>
        /// 2P用更新設定関数
        /// </summary>
        public async void SubUpdate()
        {
            // オブジェクトが生成されるまで待つ
            await UniTask.WaitWhile(() => !Object);

            // Ray用ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => {
                    rayProcessing.SubProcessing();
                })
                .AddTo(Object);
            
            // イベント処理生成
            //setInputEvent();
            MoveEvents.SetSubPlayerMovementLoops();
        }
    }

    /// <summary>
    /// Player挙動管理クラス
    /// </summary>
    public sealed class PlayerMove
    {
        private Player.PlayerMoveSpeed moveSpeed = new Player.PlayerMoveSpeed(ObjectManager.TitleScene.PlayerData.MoveSpeed);
        /// <summary>
        /// 左移動処理
        /// </summary>
        public void LeftMovement(PlayerManager tmpPlayer)
        {
            // どの向きに歩いているか設定
            tmpPlayer.MoveDis = Vector3.left;
            
            // 移動アニメーション再生
            tmpPlayer.MoveAnimator.SetBool("Move", true);

            // ９０度左を向く
            tmpPlayer.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_LEFT;

            // 移動
            tmpPlayer.Object.transform.position += Vector3.left * moveSpeed.Amount * Time.deltaTime;
        }
        /// <summary>
        /// 右移動処理
        /// </summary>
        public void RightMovement(PlayerManager tmpPlayer)
        {
            // どの向きに歩いているか設定
            tmpPlayer.MoveDis = Vector3.right;
            
            // 移動アニメーション再生
            tmpPlayer.MoveAnimator.SetBool("Move", true);

            // ９０度右を向く
            tmpPlayer.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_RIGHT;

            // 移動
            tmpPlayer.Object.transform.position += Vector3.right * moveSpeed.Amount * Time.deltaTime;
            
        }
        /// <summary>
        /// 前移動処理
        /// </summary>
        public void ForwardMovement(PlayerManager tmpPlayer)
        {
            // どの向きに歩いているか設定
            tmpPlayer.MoveDis = Vector3.forward;
            
            // 移動アニメーション再生
            tmpPlayer.MoveAnimator.SetBool("Move", true);

            // 前を向く
            tmpPlayer.Object.transform.eulerAngles = Vector3.zero;

            // 移動
            tmpPlayer.Object.transform.position += Vector3.forward * moveSpeed.Amount * Time.deltaTime;
        }
        /// <summary>
        /// 後ろ移動処理
        /// </summary>
        public void BackMovement(PlayerManager tmpPlayer)
        {
            // どの向きに歩いているか設定
            tmpPlayer.MoveDis = Vector3.back;

            // 移動アニメーション再生
            tmpPlayer.MoveAnimator.SetBool("Move", true);

            // 後ろを向く
            tmpPlayer.Object.transform.eulerAngles = OutGameConstants.PLAYER_DIRECTION_BACK;

            // 移動
            tmpPlayer.Object.transform.position += Vector3.back * moveSpeed.Amount * Time.deltaTime;
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
            tmpPlayer.MoveDis = Vector3.zero;
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
        private RayDistance rayDistance = new RayDistance(ObjectManager.TitleScene.PlayerData.RayDistance);

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
                    ObjectManager.InputEvent.FoodNicknamesTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
                else if(ObjectManager.Player.HitObject.name == "RecipeBook")
                    ObjectManager.InputEvent.DisplayIngredientsListTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
                else if(ObjectManager.Player.HitObject.name == "GasBurner")
                    ObjectManager.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
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
                    ObjectManager.InputEvent.FoodNicknamesTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
                else if(ObjectManager.SubPlayer.HitObject.name == "RecipeBook")
                    ObjectManager.InputEvent.DisplayIngredientsListTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
                else if(ObjectManager.SubPlayer.HitObject.name == "GasBurner")
                    ObjectManager.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);

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

