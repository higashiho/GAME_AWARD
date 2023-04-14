using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterface;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GameManager;

namespace player
{
    public class PlayerManager : IActor
    {
        /// <summary>
        /// DataPlayerのハンドル
        /// </summary>
        public AsyncOperationHandle DataHandle{get; private set;}

        /// <summary>
        /// プレイヤーオブジェクト
        /// </summary>
        public GameObject Object{get; private set;}
        
        /// <summary>
        /// プレイヤーのデータ
        /// </summary>
        public DataPlayer DataPlayer{get; private set;}

        /// <summary>
        /// プレイヤー移動クラス
        /// </summary>
        public PlayerMove PlayerMove{get; private set;}

        /// <summary>
        /// プレイヤー回転クラス
        /// </summary>
        public PlayerRotate PlayerRotate{get; private set;}

        /// <summary>
        /// ベースクラス
        /// </summary>
        public BasePlayer BasePlayer{get; private set;}

        /// <summary>
        /// 食べ物を取ってポイントを獲得するクラス
        /// </summary>
        public TakeFood TakeFood{get; private set;}

        /// <summary>
        /// Rayが当たったオブジェクト
        /// </summary>
        public GameObject RayHitObject{get{return rayHitObject;} set{rayHitObject = value;}}
        private GameObject rayHitObject;

        /// <summary>
        /// Rayを操作するクラス
        /// </summary>
        /// <value></value>
        public RayController RayController{get; private set;}

        // コンストラクタ
        public PlayerManager()
        {
            // 初期化
            Initialization();
        }

        public async void Initialization()
        {

            // プレイヤーデータ取得
            DataHandle = Addressables.LoadAssetAsync<DataPlayer>("nGame/PlayerData.assetyerData.asset");
            // プレイヤーデータハンドルを非同期にする
            await DataHandle.Task;
            DataPlayer = (DataPlayer)DataHandle.Result;
            // 解放
            Addressables.Release(DataHandle);
            
            // 取得するまで待つ
            DataHandle = Addressables.LoadAssetAsync<GameObject>("Player");
            await DataHandle.Task;

            // 生成座標を設定   
            PlayerInstancePos InstancePosOne = new PlayerInstancePos(DataPlayer.FirstPlayerCreatePos);

            // プレイヤー生成
            var tmpObj = (GameObject)DataHandle.Result;
            Object = MonoBehaviour.Instantiate(tmpObj
            , InstancePosOne.pos
            , Quaternion.identity);

            PlayerMove = new PlayerMove();
            PlayerRotate = new PlayerRotate();
            BasePlayer = new BasePlayer();
            TakeFood = new TakeFood();
            RayController = new RayController();
        }

        public void Update()
        {
            // 各移動メソッド
            PlayerMove.ForwardMove();
            PlayerMove.BackMove();
            PlayerMove.RightMove();
            PlayerMove.LeftMove();

            // 回転メソッド
            PlayerRotate.Rotate();
            
            // Rayで当たり判定を得る
            RayController.RayCast();

            // 食べ物を獲得してポイントゲット
            TakeFood.AddPoint();

        }
    }

    /// <summary>
    /// プレイヤーの移動クラス
    /// </summary>
    public class PlayerMove
    {
        
        public PlayerMoveSpeed MoveSpeed{get; private set;}

        // コンストラクタ
        public PlayerMove()
        {
            MoveSpeed = new PlayerMoveSpeed();
        }

        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        public void ForwardMove()
        {
            if(Input.GetKey(KeyCode.W))
            {
                ObjectManager.Player.Object.transform.position +=
                ObjectManager.Player.Object.transform.forward * MoveSpeed.moveSpeed * Time.deltaTime;
            }
        }

        public void BackMove()
        {
            if(Input.GetKey(KeyCode.S))
            {
                ObjectManager.Player.Object.transform.position +=
                -ObjectManager.Player.Object.transform.forward * MoveSpeed.moveSpeed * Time.deltaTime;
            }
        }

        public void RightMove()
        {
            if(Input.GetKey(KeyCode.D))
            {
                ObjectManager.Player.Object.transform.position +=
                ObjectManager.Player.Object.transform.right * MoveSpeed.moveSpeed * Time.deltaTime;
            }
        }

        public void LeftMove()
        {
            if(Input.GetKey(KeyCode.A))
            {
                ObjectManager.Player.Object.transform.position +=
                -ObjectManager.Player.Object.transform.right * MoveSpeed.moveSpeed * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// プレイヤー回転クラス
    /// </summary>
    public class PlayerRotate
    {
        public PlayerRotatePos PlayerRotatePos{get; private set;}

        // コンストラクタ
        public PlayerRotate()
        {
            PlayerRotatePos = new PlayerRotatePos();
        }

        // 回転させる
        public void Rotate()
        {
            if(Input.GetKey(KeyCode.D))
            {
                ObjectManager.Player.Object.transform.eulerAngles = PlayerRotatePos.Rotate * Time.deltaTime;
            }

            if(Input.GetKey(KeyCode.A))
            {
                ObjectManager.Player.Object.transform.eulerAngles = -PlayerRotatePos.Rotate * Time.deltaTime;
            }
        }
    }

    // 食べ物を取得して得点に変換するクラス
    public class TakeFood
    {
        public BasePlayer BasePlayer{get; private set;}

        // コンストラクタ
        public TakeFood()
        {
            BasePlayer = new BasePlayer();
        }

        // ポイント獲得メソッド
        public void AddPoint()
        {
            // XXX:NULL-RayHitObject
            if(ObjectManager.Player.RayHitObject.tag == ("Food"))
            {
                // TODO:RayHitObjectの持っている値を入れる
                //BasePlayer.PointArr.Add(ObjectManager.Player.RayHitObject., 1);
                //Debug.Log(BasePlayer.PointArr[BasePlayer.MeatPoint]);
            }
            else if(ObjectManager.Player.RayHitObject == null)
            {
                return;
            }
        }
    }


    // Rayで当たり判定を得ているクラス
    public class RayController
    {
        public PlayerRayDirection PlayerRayDirection{get; private set;}

        // コンストラクタ
        public RayController()
        {
            PlayerRayDirection = new PlayerRayDirection();
        }

        public void RayCast()
        {
            // プレイヤーから出るRayの設定
            var checkFoodRay = new Ray(ObjectManager.Player.Object.transform.position,
                                       ObjectManager.Player.Object.transform.forward * PlayerRayDirection.RayDirection);
            // 色がついていない
            Debug.DrawRay(ObjectManager.Player.Object.transform.position,
            ObjectManager.Player.Object.transform.forward * PlayerRayDirection.RayDirection, Color.red);

            RaycastHit hit;

            // Rayが当たったら
            if(Physics.Raycast(checkFoodRay, out hit, PlayerRayDirection.RayDirection))
            {
                // Rayが当たったオブジェクトを格納
                ObjectManager.Player.RayHitObject = hit.collider.gameObject;
            }
            else
            {
                ObjectManager.Player.RayHitObject = null;
            }
        }
    }
}