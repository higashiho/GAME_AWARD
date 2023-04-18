using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterface;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GameManager;
using System.Linq;
using System;

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
        public GameObject ObjectOne{get; private set;}
        public GameObject ObjectTwo{get; private set;}
        
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
            FirstPlayerInstancePos InstancePosOne = new FirstPlayerInstancePos();
            SecondPlayerInstancePos InstancePosTwo = new SecondPlayerInstancePos();

            // 1Pプレイヤー生成
            var tmpObj = (GameObject)DataHandle.Result;
            ObjectOne = MonoBehaviour.Instantiate(tmpObj
            , InstancePosOne.onePos
            , Quaternion.identity);

            // 2Pプレイヤー生成
            var tmpobj = (GameObject)DataHandle.Result;
            ObjectTwo = MonoBehaviour.Instantiate(tmpobj
            , InstancePosTwo.twoPos
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
            PlayerMove.Move();

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
        public void Move()
        {
            if(Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D))
            {
                ObjectManager.Player.ObjectOne.transform.position +=
                ObjectManager.Player.ObjectOne.transform.forward * MoveSpeed.moveSpeed * Time.deltaTime;
            }

            //2P--------------------------------
            if(Input.GetKey(KeyCode.UpArrow)
            || Input.GetKey(KeyCode.LeftArrow)
            || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.RightArrow))
            {
                ObjectManager.Player.ObjectTwo.transform.position +=
                ObjectManager.Player.ObjectTwo.transform.forward * MoveSpeed.moveSpeed * Time.deltaTime;
            }
        }        
    }

    /// <summary>
    /// プレイヤー回転クラス
    /// </summary>
    public class PlayerRotate
    {
        public PlayerRotateRightPos PlayerRotateRightPos{get; private set;}

        public PlayerRotateLeftPos PlayerRotateLeftPos{get; private set;}

        public PlayerRotateBackPos PlayerRotateBackPos{get; private set;}

        // コンストラクタ
        public PlayerRotate()
        {
            PlayerRotateRightPos = new PlayerRotateRightPos();
            PlayerRotateLeftPos = new PlayerRotateLeftPos();
            PlayerRotateBackPos = new PlayerRotateBackPos();
        }

        // 回転させる
        public void Rotate()
        {
            if(Input.GetKey(KeyCode.D))
            {
                ObjectManager.Player.ObjectOne.transform.eulerAngles = PlayerRotateRightPos.RightRotate;
            }

            if(Input.GetKey(KeyCode.A))
            {
                ObjectManager.Player.ObjectOne.transform.eulerAngles = PlayerRotateLeftPos.LeftRotate;
            }

            if(Input.GetKey(KeyCode.W))
            {
                ObjectManager.Player.ObjectOne.transform.eulerAngles = Vector3.zero;
            }

            if(Input.GetKey(KeyCode.S))
            {
                ObjectManager.Player.ObjectOne.transform.eulerAngles = PlayerRotateBackPos.BackRotate;
            }

            //2P--------------------------------
            if(Input.GetKey(KeyCode.RightArrow))
            {
                ObjectManager.Player.ObjectTwo.transform.eulerAngles = PlayerRotateRightPos.RightRotate;
            }

            if(Input.GetKey(KeyCode.LeftArrow))
            {
                ObjectManager.Player.ObjectTwo.transform.eulerAngles = PlayerRotateLeftPos.LeftRotate;
            }

            if(Input.GetKey(KeyCode.UpArrow))
            {
                ObjectManager.Player.ObjectTwo.transform.eulerAngles = Vector3.zero;
            }

            if(Input.GetKey(KeyCode.DownArrow))
            {
                ObjectManager.Player.ObjectTwo.transform.eulerAngles = PlayerRotateBackPos.BackRotate;
            }
        }
    }

    // 食べ物を取得して得点に変換するクラス
    public class TakeFood
    {
        public BasePlayer BasePlayer{get; private set;}
        
        // 取得したアイテムの座標を返すイベント
        public event EventHandler<ReturnPresentPosEventArgs> ReturnPresentItemPos;

        // コンストラクタ
        public TakeFood()
        {
            BasePlayer = new BasePlayer();
        }

        // ポイント獲得メソッド
        public void AddPoint()
        {
            if(!ObjectManager.Player.RayHitObject) return;

            // 初めてその種類の食材を獲得
            if(!BasePlayer.PointArr.ContainsKey(getFoodName())
            && Input.GetKey(KeyCode.LeftShift))
            {
                // アイテムの座標を取得するイベントインスタンス化
                ReturnPresentPosEventArgs args = new ReturnPresentPosEventArgs();
                // 座標設定
                args.presentPos = ObjectManager.Player.RayHitObject.transform.position;
                // 座標を返す
                ReturnPresentPos(args);

                // １回しか取得できない
                ObjectManager.Player.RayHitObject.SetActive(false);

                // Dictionaryに肉１点追加
                BasePlayer.PointArr.Add(getFoodName(), 1);
                Debug.Log(BasePlayer.PointArr.FirstOrDefault());
                return;
            }

            // 2回目以降の食材獲得
            if(BasePlayer.PointArr.ContainsKey(getFoodName())
            && Input.GetKey(KeyCode.LeftShift))
            {
                // アイテムの座標を取得するイベントインスタンス化
                ReturnPresentPosEventArgs args = new ReturnPresentPosEventArgs();
                // 座標設定
                args.presentPos = ObjectManager.Player.RayHitObject.transform.position;
                // 座標を返す
                ReturnPresentPos(args);

                // １回取得すると消える
                ObjectManager.Player.RayHitObject.SetActive(false);

                // 肉に１点加算
                incrimentDictionary(getFoodName(), 1);
                Debug.Log(BasePlayer.PointArr.FirstOrDefault());
                return;
            }
        }


        // Dictionaryの特定のキーの値を加算する
        private void incrimentDictionary(string food, int pointValue)
        {
            int tmpCount;

            // tmpCountにfoodの値を代入
            BasePlayer.PointArr.TryGetValue(food, out tmpCount);

            // 獲得ポイントを加算する
            BasePlayer.PointArr[food] = tmpCount + pointValue;
        }


        // 獲得した食材の名前を獲得する
        private string getFoodName()
        {
            // 目の前にある食材の名前を返す
            return  ObjectManager.Player.RayHitObject.tag;
        }

        private void ReturnPresentPos(ReturnPresentPosEventArgs e)
        {
            EventHandler<ReturnPresentPosEventArgs> handler = ReturnPresentItemPos;

            if(handler != null)
            {
                handler(this, e);
            }
        }
    }
    
    public class ReturnPresentPosEventArgs : EventArgs
    {
        public Vector3 presentPos;
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
            var checkFoodRay = new Ray(ObjectManager.Player.ObjectOne.transform.position,
                                       ObjectManager.Player.ObjectOne.transform.forward * PlayerRayDirection.RayDirection);

            // Rayを可視化する
            Debug.DrawRay(ObjectManager.Player.ObjectOne.transform.position,
                          ObjectManager.Player.ObjectOne.transform.forward * PlayerRayDirection.RayDirection, Color.red);

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