using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterface;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GameManager;
using System.Linq;
using System;
using FoodPoint;

namespace Player
{
    public class PlayerManager : IActor
    {
        /// <summary>
        /// DataPlayerのハンドル
        /// </summary>
        private AsyncOperationHandle DataHandle;

        /// <summary>
        /// プレイヤーオブジェクト
        /// </summary>
        public GameObject ObjectOne{get; private set;}
        public GameObject ObjectTwo{get; private set;}
        
        /// <summary>
        /// プレイヤーのデータ
        /// </summary>
        public DataPlayer Data{get; private set;}

        /// <summary>
        /// プレイヤー移動クラス
        /// </summary>
        private PlayerMove Move;

        /// <summary>
        /// プレイヤー回転クラス
        /// </summary>
        private PlayerRotate Rotate;

        /// <summary>
        /// 食べ物を取ってポイントを獲得するクラス
        /// </summary>
        public FoodPoint FoodPoint{get;} = new FoodPoint();

        /// <summary>
        /// Rayが当たったオブジェクト
        /// </summary>
        public GameObject RayHitObject{get{return rayHitObject;} set{rayHitObject = value;}}
        private GameObject rayHitObject;

        /// <summary>
        /// Rayを操作するクラス
        /// </summary>
        private RayController RayController;

        // コンストラクタ
        public PlayerManager()
        {
            // 初期化
            Initialization();
        }

        public async void Initialization()
        {

            // プレイヤーデータ取得
            DataHandle = Addressables.LoadAssetAsync<DataPlayer>("Assets/Data/InGame/MainPlayerData.asset");
            // プレイヤーデータハンドルを非同期にする
            await DataHandle.Task;
            Data = (DataPlayer)DataHandle.Result;
            // 解放
            Addressables.Release(DataHandle);
            
            // 取得するまで待つ
            DataHandle = Addressables.LoadAssetAsync<GameObject>("Player");
            await DataHandle.Task;

            // 生成座標を設定   
            FirstPlayerInstancePos InstancePosOne = new FirstPlayerInstancePos(Data.FirstPlayerCreatePos);


            // 1Pプレイヤー生成
            var tmpObj = (GameObject)DataHandle.Result;
            ObjectOne = MonoBehaviour.Instantiate(tmpObj
            , InstancePosOne.MainPos
            , Quaternion.identity);

            Move = new PlayerMove();

            Rotate = new PlayerRotate();
            
            RayController = new RayController();
        }

        public void Update()
        {
            // 各移動メソッド
            Move.MovementActor();

            // 回転メソッド
            Rotate.Rotate();
            
            // Rayで当たり判定を得る
            RayController.RayCast();

            // 食べ物を獲得してポイントゲット
            FoodPoint.AddPoint();
        }
    }

    /// <summary>
    /// プレイヤーの移動クラス
    /// </summary>
    public class PlayerMove
    {
        private PlayerMoveSpeed moveSpeed;

        // コンストラクタ
        public PlayerMove()
        {
            moveSpeed = new PlayerMoveSpeed(ObjectManager.Player.Data.MoveSpeed);
        }

        // 移動しているのがどのプレイヤーか
        public void MovementActor()
        {
            if(Input.GetKey(KeyCode.A))
            {
                move(ObjectManager.Player.ObjectOne);
            }
            else if(Input.GetKey(KeyCode.D))
            {
                move(ObjectManager.Player.ObjectOne);
            }

            if(Input.GetKey(KeyCode.W))
            {
                move(ObjectManager.Player.ObjectOne);
            }
            else if(Input.GetKey(KeyCode.S))
            {
                move(ObjectManager.Player.ObjectOne);
            }

            // if(Input.GetKey(KeyCode.LeftArrow))
            // {
            //     move(ObjectManager.Player.ObjectOne);
            // }
            // else if(Input.GetKey(KeyCode.RightArrow))
            // {
            //     move(ObjectManager.Player.ObjectTwo);
            // }

            // if(Input.GetKey(KeyCode.UpArrow))
            // {
            //     move(ObjectManager.Player.ObjectOne);
            // }
            // else if(Input.GetKey(KeyCode.DownArrow))
            // {
            //     move(ObjectManager.Player.ObjectTwo);
            // }
        }

        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        private void move(GameObject player)
        {
            player.transform.position +=
            player.transform.forward * moveSpeed.Amount * Time.deltaTime;
        }

        // 1Pか2Pかを調べる
        // public bool checkActor(GameObject player)
        // {
        //     if(player == ObjectManager.Player.ObjectOne)
        //     {
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }   
    }

    /// <summary>
    /// プレイヤー回転クラス
    /// </summary>
    public class PlayerRotate
    {
        private PlayerRotateRightPos PlayerRotateRightPos;

        private PlayerRotateLeftPos PlayerRotateLeftPos;

        private PlayerRotateBackPos PlayerRotateBackPos;

        // コンストラクタ
        public PlayerRotate()
        {
            PlayerRotateRightPos = new PlayerRotateRightPos(ObjectManager.Player.Data.RotateRightPos);
            PlayerRotateLeftPos = new PlayerRotateLeftPos(ObjectManager.Player.Data.RotateLeftPos);
            PlayerRotateBackPos = new PlayerRotateBackPos(ObjectManager.Player.Data.RotateBackPos);
        }

        // 回転させる
        public void Rotate()
        {
            if(Input.GetKey(KeyCode.D))
            {
                ObjectManager.Player.ObjectOne.transform.eulerAngles = PlayerRotateRightPos.Amount;
            }
            else if(Input.GetKey(KeyCode.A))
            {
                ObjectManager.Player.ObjectOne.transform.eulerAngles = PlayerRotateLeftPos.Amount;
            }

            if(Input.GetKey(KeyCode.W))
            {
                ObjectManager.Player.ObjectOne.transform.eulerAngles = Vector3.zero;
            }
            else if(Input.GetKey(KeyCode.S))
            {
                ObjectManager.Player.ObjectOne.transform.eulerAngles = PlayerRotateBackPos.Amount;
            }

            //2P--------------------------------
            // if(Input.GetKey(KeyCode.RightArrow))
            // {
            //     ObjectManager.Player.ObjectTwo.transform.eulerAngles = PlayerRotateRightPos.Amount;
            // }
            // else if(Input.GetKey(KeyCode.LeftArrow))
            // {
            //     ObjectManager.Player.ObjectTwo.transform.eulerAngles = PlayerRotateLeftPos.Amount;
            // }
            // else if(Input.GetKey(KeyCode.UpArrow))
            // {
            //     ObjectManager.Player.ObjectTwo.transform.eulerAngles = Vector3.zero;
            // }
            // else if(Input.GetKey(KeyCode.DownArrow))
            // {
            //     ObjectManager.Player.ObjectTwo.transform.eulerAngles = PlayerRotateBackPos.Amount;
            // }
        }
    }
    
    /// <summary>
    /// アイテムを取得した位置を返すクラス
    /// </summary>
    public class ReturnPresentPosEventArgs : EventArgs
    {
        public Vector3 presentPos;
    }

    // todo:プレイヤーの内側にも当たり判定をつける
    // Rayで当たり判定を得ているクラス
    public class RayController
    {
        private PlayerRayDirection playerRayDirection;

        // コンストラクタ
        public RayController()
        {
            playerRayDirection = new PlayerRayDirection(ObjectManager.Player.Data.RayDirection);
        }

        public void RayCast()
        {
            // プレイヤーから出るRayの設定
            var checkFoodRay = new Ray(-ObjectManager.Player.ObjectOne.transform.forward / 2
                                    , ObjectManager.Player.ObjectOne.transform.forward);

            // Rayを可視化する
            Debug.DrawRay(ObjectManager.Player.ObjectOne.transform.position,
                          ObjectManager.Player.ObjectOne.transform.forward, Color.red);

            RaycastHit hit;

            // Rayが当たったら
            if(Physics.Raycast(checkFoodRay, out hit, playerRayDirection.Amount))
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


    

    // プレイヤーが獲得した得点を管理するクラス
    public class FoodPoint
    {
        // プレイヤーが取得したポイントを保管しておく配列
        public Dictionary<string, int> Array{get; private set;} = new Dictionary<string, int>();
        // 取得したアイテムの座標を返すイベント
        public event EventHandler<ReturnPresentPosEventArgs> ReturnPresentItemPos;
        /// <summary>
        /// 取得ポイント
        /// </summary>
        // お肉
        public MeatPoint MeatPoint{get; protected set;}
        // お魚
        public FishPoint FishPoint{get; protected set;}
        // お野菜
        public VegPoint VegPoint{get; protected set;}



        // ポイント獲得メソッド
        public void AddPoint()
        {
            // 何にもあたっていなければメソッドから抜ける
            if(!ObjectManager.Player.RayHitObject) return;

            // 初めてその種類の食材を獲得
            if(!Array.ContainsKey(getFoodName())
            && Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("当たっている");
                // アイテムの座標を取得するイベントインスタンス化
                ReturnPresentPosEventArgs args = new ReturnPresentPosEventArgs();
                // 座標設定
                args.presentPos = ObjectManager.Player.RayHitObject.transform.position;
                // 座標を返す
                ReturnPresentPos(args);

                // １回しか取得できない
                deleteFood();

                // Dictionaryに肉１点追加
                Array.Add(getFoodName(), 1);
                Debug.Log(Array.FirstOrDefault());
                return;
            }

            // 2回目以降の食材獲得
            if(Array.ContainsKey(getFoodName())
            && Input.GetKeyDown(KeyCode.LeftShift))
            {
                // アイテムの座標を取得するイベントインスタンス化
                ReturnPresentPosEventArgs args = new ReturnPresentPosEventArgs();
                // 座標設定
                args.presentPos = ObjectManager.Player.RayHitObject.transform.position;
                // 座標を返す
                ReturnPresentPos(args);

                // １回取得すると消える
                deleteFood();

                // 肉に１点加算
                incrimentDictionary(getFoodName(), 1);
                Debug.Log(Array.FirstOrDefault());
                return;
            }
        }

        // １回取得すると消える
        private void deleteFood()
        {
            ObjectManager.Player.RayHitObject.SetActive(false);
        }

        private void ReturnPresentPos(ReturnPresentPosEventArgs e)
        {
            EventHandler<ReturnPresentPosEventArgs> handler = ReturnPresentItemPos;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        // Dictionaryの特定のキーの値を加算する
        private void incrimentDictionary(string food, int pointValue)
        {
            int tmpCount;

            // tmpCountにfoodの値を代入
            Array.TryGetValue(food, out tmpCount);

            // 獲得ポイントを加算する
            Array[food] = tmpCount + pointValue;
        }


        // 獲得した食材の名前を獲得する
        private string getFoodName()
        {
            // 目の前にある食材の名前を返す
            return  ObjectManager.Player.RayHitObject.tag;
        }
    }
}