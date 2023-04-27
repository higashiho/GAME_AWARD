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
using Item;
using Nomnom.RaycastVisualization;

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
        public GameObject Object{get; private set;}
        
        /// <summary>
        /// プレイヤーのデータ
        /// </summary>
        public DataPlayer Data{get; private set;}

        /// <summary>
        /// プレイヤー移動クラス
        /// </summary>
        private PlayerMove Move;

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

        public PlayerCapsuleRay PlayerCapsuleRay{get{return playerCapsuleRay;} set{playerCapsuleRay = value;}}
        private PlayerCapsuleRay playerCapsuleRay;

        public PlayerCapsuleRayStartPos PlayerCapsuleRayStartPos{get{return playerCapsuleRayStartPos;} set{playerCapsuleRayStartPos = value;}}
        private PlayerCapsuleRayStartPos playerCapsuleRayStartPos;

        public PlayerCapsuleRayDistance PlayerCapsuleRayDistance{get{return playerCapsuleRayDistance;} set{playerCapsuleRayDistance = value;}}
        private PlayerCapsuleRayDistance playerCapsuleRayDistance;

        public PlayerCapsuleRayEndPos PlayerCapsuleRayEndPos{get{return playerCapsuleRayEndPos;} set{playerCapsuleRayEndPos = value;}}
        private PlayerCapsuleRayEndPos playerCapsuleRayEndPos;

        public PlayerCapsuleRayRadiuse PlayerCapsuleRayRadiuse{get{return playerCapsuleRayRadiuse;} set{playerCapsuleRayRadiuse = value;}}
        private PlayerCapsuleRayRadiuse playerCapsuleRayRadiuse;

        public PlayerOverlapCapsule PlayerOverlapCapsule{get{return playerOverlapCapsule;} set{playerOverlapCapsule = value;}}
        private PlayerOverlapCapsule playerOverlapCapsule;

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
            DataHandle = Addressables.LoadAssetAsync<GameObject>("InPlayer");
            await DataHandle.Task;

            // 生成座標を設定   
            FirstPlayerInstancePos InstancePosOne = new FirstPlayerInstancePos(Data.FirstPlayerCreatePos);


            // 1Pプレイヤー生成
            var tmpObj = (GameObject)DataHandle.Result;
            Object = MonoBehaviour.Instantiate(tmpObj
            , InstancePosOne.MainPos
            , Quaternion.identity);


            Move = new PlayerMove();
            
            RayController = new RayController();

            // カプセル型のレイの距離
            PlayerCapsuleRayDistance = new PlayerCapsuleRayDistance(ObjectManager.Player.Data.RayDirection);

            // カプセル型のレイの半径
            PlayerCapsuleRayRadiuse = new PlayerCapsuleRayRadiuse(0.5f);
        }

        public void Update()
        {
            // Rayで当たり判定を得る
            RayController.RayUpdate();
            
            // 各移動メソッド
            Move.MovementActor();

            // 食べ物を獲得してポイントゲット
            FoodPoint.AddPoint();
        }
    }

    /// <summary>
    /// プレイヤーの移動 と 回転クラス
    /// </summary>
    public class PlayerMove
    {
        private PlayerMoveSpeed moveSpeed;
        private PlayerRotateRightPos playerRotateRightPos;
        private PlayerRotateLeftPos playerRotateLeftPos;
        private PlayerRotateForwardPos playerRotateForwardPos;
        private PlayerRotateBackPos playerRotateBackPos;

        // コンストラクタ
        public PlayerMove()
        {
            moveSpeed = new PlayerMoveSpeed(ObjectManager.Player.Data.MoveSpeed);
            playerRotateRightPos = new PlayerRotateRightPos(ObjectManager.Player.Data.RotateRightPos);
            playerRotateLeftPos = new PlayerRotateLeftPos(ObjectManager.Player.Data.RotateLeftPos);
            playerRotateBackPos = new PlayerRotateBackPos(ObjectManager.Player.Data.RotateBackPos);
            playerRotateForwardPos = new PlayerRotateForwardPos(ObjectManager.Player.Data.RotateForWardPos);
        }

        public void MovementActor()
        {
            action(KeyCode.W,KeyCode.S,KeyCode.A,KeyCode.D, ObjectManager.Player.Object);
        }

        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        private void action(KeyCode upKey,KeyCode downKey,KeyCode leftKey,KeyCode rightKey, GameObject player)
        {
            Vector3 playerMovePos = Vector3.zero;
            Vector3 playerRotateAmountLeftRight = player.transform.eulerAngles;
            Vector3 playerRotateAmountForwardBack = player.transform.eulerAngles;

            if(Input.GetKey(leftKey))
            {
                playerMovePos += Vector3.left;
                playerRotateAmountLeftRight = playerRotateLeftPos.Amount;
                //playerRotateForwardPos = new PlayerRotateForwardPos(ObjectManager.Player.Data.RotateForWardPos);
            }
            else if(Input.GetKey(rightKey))
            {
                playerMovePos += Vector3.right;
                playerRotateAmountLeftRight = playerRotateRightPos.Amount;
                //playerRotateForwardPos = new PlayerRotateForwardPos(Vector3.zero);
            }
            else if(Input.GetKey(upKey))
            {
                playerMovePos += Vector3.forward;
                playerRotateAmountForwardBack = playerRotateForwardPos.Amount;
            }
            else if(Input.GetKey(downKey))
            {
                playerMovePos += Vector3.back;
                playerRotateAmountForwardBack = playerRotateBackPos.Amount;
            }

            // 移動中なら
            // if(playerMovePos.x != 0 && playerMovePos.z != 0)
            // {
            //     // 斜め移動を単位ベクトル化
            //     playerMovePos = normalization(playerMovePos);
            // }

            // 回転を計算する
            player.transform.eulerAngles = (playerRotateAmountLeftRight + playerRotateAmountForwardBack) / 2;

            if(ObjectManager.Player.PlayerCapsuleRay == null) return;

            // プレイヤーが何かと当たっていなければ移動できる
            if(!ObjectManager.Player.RayHitObject
             && ObjectManager.Player.PlayerOverlapCapsule.Amount.Length <= 2)
            {
                // 移動を計算する
                player.transform.localPosition +=
                playerMovePos * moveSpeed.Amount * Time.deltaTime;
            }
            // 振り向きで壁にめり込むのを防ぐ
            else if(ObjectManager.Player.PlayerOverlapCapsule.Amount.Length > 2)
            {
                if(Input.GetKeyDown(leftKey) || Input.GetKeyDown(rightKey) || Input.GetKeyDown(upKey) || Input.GetKeyDown(downKey))
                {
                    player.transform.localPosition -=
                    playerMovePos * moveSpeed.Amount * Time.deltaTime;
                }
            }
        }

        // 正規化関数
        private Vector3 normalization(Vector3 tmpPos)
        {

            float tmpNum = Mathf.Sqrt(tmpPos.x * tmpPos.x
                                    + tmpPos.y * tmpPos.y
                                    + tmpPos.z * tmpPos.z);
            
            Vector3 normalizePos = new Vector3(tmpPos.x / tmpNum
                                             , tmpPos.y / tmpNum
                                             , tmpPos.z / tmpNum);
            return normalizePos;
        }
    }
    
    /// <summary>
    /// アイテムを取得した位置を返すクラス
    /// </summary>
    public class ReturnPresentPosEventArgs : EventArgs
    {
        public Vector3 presentPos;
    }

    // Rayで当たり判定を得ているクラス
    public class RayController
    {
        public void RayUpdate()
        {
            CapsuleRayCast();
        }

        // 筒型のレイを飛ばす
        private void CapsuleRayCast()
        {
            // プレイヤーから出る、カプセル型のレイの端の球の中心の座標
            ObjectManager.Player.PlayerCapsuleRayStartPos = new PlayerCapsuleRayStartPos(new Vector3(
                ObjectManager.Player.Object.transform.localPosition.x + ObjectManager.Player.Object.transform.forward.x / 10,
                0.5f,
                ObjectManager.Player.Object.transform.localPosition.z + ObjectManager.Player.Object.transform.forward.z / 10
            ));

            // プレイヤーから出る、カプセル型のレイの端の球の中心の座標
            ObjectManager.Player.PlayerCapsuleRayEndPos = new PlayerCapsuleRayEndPos(new Vector3(
                ObjectManager.Player.Object.transform.localPosition.x + ObjectManager.Player.Object.transform.forward.x / 10,
                3,
                ObjectManager.Player.Object.transform.localPosition.z + ObjectManager.Player.Object.transform.forward.z / 10
            ));

            Vector3 overlapStartPos = new Vector3(
                ObjectManager.Player.Object.transform.localPosition.x + ObjectManager.Player.Object.transform.forward.x / 5,
                0.5f,
                ObjectManager.Player.Object.transform.localPosition.z + ObjectManager.Player.Object.transform.forward.z / 5
            );

            Vector3 overlapEndPos = new Vector3(
                ObjectManager.Player.Object.transform.localPosition.x + ObjectManager.Player.Object.transform.forward.x / 5,
                3,
                ObjectManager.Player.Object.transform.localPosition.z + ObjectManager.Player.Object.transform.forward.z / 5
            );

            RaycastHit hit;

            // カプセル型のレイを飛ばす
            ObjectManager.Player.PlayerCapsuleRay = new PlayerCapsuleRay(Physics.CapsuleCast(
                ObjectManager.Player.PlayerCapsuleRayStartPos.Amount,
                ObjectManager.Player.PlayerCapsuleRayEndPos.Amount,
                ObjectManager.Player.PlayerCapsuleRayRadiuse.Amount,
                ObjectManager.Player.Object.transform.forward,
                out hit,
                ObjectManager.Player.PlayerCapsuleRayDistance.Amount));

            // レイの内側を格納する
            ObjectManager.Player.PlayerOverlapCapsule = new PlayerOverlapCapsule(Physics.OverlapCapsule(
                overlapStartPos,
                overlapEndPos,
                ObjectManager.Player.PlayerCapsuleRayRadiuse.Amount
            ));


            // レイを見えるようにする
            VisualPhysics.CapsuleCast(
                ObjectManager.Player.PlayerCapsuleRayStartPos.Amount,
                ObjectManager.Player.PlayerCapsuleRayEndPos.Amount,
                ObjectManager.Player.PlayerCapsuleRayRadiuse.Amount,
                ObjectManager.Player.Object.transform.forward,
                out hit,
                ObjectManager.Player.PlayerCapsuleRayDistance.Amount);

            // レイが当たったら
            if(ObjectManager.Player.PlayerCapsuleRay.Cast)
            {
                ObjectManager.Player.RayHitObject = hit.collider.gameObject;
            }
            else
            {
                // レイが外れたらnullにする
                ObjectManager.Player.RayHitObject = null;
            }
        }
    }


    

    // プレイヤーが獲得した得点を管理するクラス
    public class FoodPoint
    {
        // プレイヤーが取得したポイントを保管しておく配列
        public Dictionary<string, int[]> Array{get; private set;} = new Dictionary<string, int[]>(4);
        
        //public BaseFoodPoint[] HavePointArr{get; private set;} = new BaseFoodPoint[5];
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

        public FoodPoint()
        {
            int[] val = {0,0};
            // Array.Add("MEAT", val);
            // Array.Add("FISH", val);
            // Array.Add("VEGETABLE", val);
            // Array.Add("SEASOUSING", val);

            // 取得ポイント配列初期化
            for(int i = 0; i < InGameConst.PointName.Length - 1; i++)
            {
                Array.Add(InGameConst.PointName[i], val);
            }
        }


        // ポイント獲得メソッド
        public void AddPoint()
        {
            // 何にもあたっていなければメソッドから抜ける
            if(!ObjectManager.Player.RayHitObject) return;

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

                // 目の前の食材をキューに追加
                ObjectManager.ItemManager.itemFactory.Storing(ObjectManager.Player.RayHitObject);
                
                // １回取得すると消える
                deleteFood();

                // 肉に１点加算
                incrimentDictionary(getFoodName(), getFoodPoint());
                Debug.Log(Array.FirstOrDefault());

                
                return;
            }
        }

        // １回取得すると消える
        private void deleteFood()
        {
            if(ObjectManager.Player.RayHitObject.tag == "Food")
            {
                ObjectManager.Player.RayHitObject.SetActive(false);
            }
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
        private void incrimentDictionary(string food, int[] pointValue)
        {
            int[] tmpCount;

            // tmpCountにfoodの値を代入
            Array.TryGetValue(food, out tmpCount);

            int[] val = 
            {
                tmpCount[0] + pointValue[0],
                tmpCount[0] + pointValue[1]
            };

            // 獲得ポイントを加算する
            Array[food] = val;
        }


        // 獲得した食材の名前を獲得する
        private string getFoodName()
        {
            // 目の前にある食材の名前を返す
            //return  ObjectManager.Player.RayHitObject.tag;
            return ObjectManager.Player.RayHitObject.GetComponent<GetValue>().Type;
        }

        private int[] getFoodPoint()
        {
            int[] point = new int[2];
            point[0] = ObjectManager.Player.RayHitObject.GetComponent<GetValue>().Point;
            point[1] = ObjectManager.Player.RayHitObject.GetComponent<GetValue>().Amount;
            return point;
        }

       

        
    }
}