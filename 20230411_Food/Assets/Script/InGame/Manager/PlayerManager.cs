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
        /// 食べ物を取ってポイントを獲得するクラス
        /// </summary>
        public FoodPoint FoodPoint{get{return foodPoint;} set{foodPoint = value;}}
        private FoodPoint foodPoint;

        public PlayerNumber PlayerNumber{get{return playerNumber;} set{playerNumber = value;}}
        private PlayerNumber playerNumber;

        // コンストラクタ
        public PlayerManager(DataPlayer tmpData)
        {
            // 初期化
            Initialization(tmpData);
        }

        public async void Initialization(DataPlayer tmpData)
        {

            FoodPoint = new FoodPoint(tmpData);
            
            // 取得するまで待つ
            DataHandle = Addressables.LoadAssetAsync<GameObject>(tmpData.AdressKey);
            await DataHandle.Task;

            // 生成座標を設定   
            PlayerInstancePos InstancePos = new PlayerInstancePos(tmpData.PlayerCreatePos);


            // 1Pプレイヤー生成
            var tmpObj = (GameObject)DataHandle.Result;
            FoodPoint.Move.RayController.Object = MonoBehaviour.Instantiate(tmpObj
            , InstancePos.MainPos
            , Quaternion.identity);

            PlayerNumber = new PlayerNumber(tmpData.Number);
        }

        public void Update()
        {
            // Rayで当たり判定を得る
            FoodPoint.Move.RayController.RayUpdate();
            
            // 各移動メソッド
            FoodPoint.Move.MovementActor();

            // 食べ物を獲得してポイントゲット
            FoodPoint.addPointUpdate();

        }

        public void Initialization()
        {
            throw new NotImplementedException();
        }
    }


    // プレイヤーが獲得した得点を管理するクラス
    public class FoodPoint
    {
        // プレイヤーが取得したポイントを保管しておく配列
        public Dictionary<string, int[]> Array{get; private set;} = new Dictionary<string, int[]>(4);
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


        /// <summary>
        /// プレイヤー移動クラス
        /// </summary>
        public PlayerMove Move{get{return move;} set{move = value;}}
        private PlayerMove move;

        /// <summary>
        /// 効果音クラス
        /// </summary>
        /// <value></value>
        public PlayerAudio PlayerAudio{get{return playerAudio;} set{playerAudio = value;}}
        private PlayerAudio playerAudio;

        public FoodPoint(DataPlayer tmpData)
        {
            Move = new PlayerMove(tmpData);
            int[] val = {0,0};
            Array.Add("MEAT", val);
            Array.Add("FISH", val);
            Array.Add("VEGETABLE", val);
            Array.Add("SEASOUSING", val);

            PlayerAudio = new PlayerAudio();
        }

        public void addPointUpdate()
        {
            AddPoint(Move.RayController.Data);
        }

        // ポイント獲得メソッド
        public void AddPoint(DataPlayer data)
        {
            // 何にもあたっていなければメソッドから抜ける
            if(!Move.RayController.RayHitObject) return;

            if(Move.RayController.RayHitObject.tag != "Food") return;

            // 初めてその種類の食材を獲得
            // if(!Array.ContainsKey(getFoodName(data))
            // && Input.GetKeyDown(KeyCode.LeftShift))
            // {
            //     // アイテムの座標を取得するイベントインスタンス化
            //     ReturnPresentPosEventArgs args = new ReturnPresentPosEventArgs();
            //     // 座標設定
            //     args.presentPos = Move.RayController.RayHitObject.transform.position;
            //     // 座標を返す
            //     ReturnPresentPos(args);

            //     // 目の前の食材をキューに追加
            //     ObjectManager.ItemManager.itemFactory.Storing(Move.RayController.RayHitObject);

            //     // １回しか取得できない
            //     deleteFood(data);

            //     // Dictionaryに肉１点追加
            //     Array.Add(getFoodName(data), 1);
            //     //Debug.Log(Array.FirstOrDefault());

                
            //     return;
            // }

            // 2回目以降の食材獲得
            if(Array.ContainsKey(getFoodName(data))
            && Input.GetKeyDown(KeyCode.LeftShift))
            {
                // アイテムの座標を取得するイベントインスタンス化
                ReturnPresentPosEventArgs args = new ReturnPresentPosEventArgs();
                // 座標設定
                args.presentPos = Move.RayController.RayHitObject.transform.position;
                // 座標を返す
                ReturnPresentPos(args);

                // 目の前の食材をキューに追加
                ObjectManager.ItemManager.itemFactory.Storing(Move.RayController.RayHitObject);
                
                // １回取得すると消える
                deleteFood(data);

                // 肉に１点加算
                incrimentDictionary(getFoodName(data), getFoodPoint(data));
                //Debug.Log(Array.FirstOrDefault());

                // 獲得音を鳴らす
                PlayerAudio.GetFood(data);
                
                return;
            }
        }

        // １回取得すると消える
        private void deleteFood(DataPlayer data)
        {
            if(Move.RayController.RayHitObject.tag == "Food")
            {
                Move.RayController.RayHitObject.SetActive(false);
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
        private void incrimentDictionary(string type, int[] pointValue)
        {
            int[] tmpCount;

            // tmpCountにfoodの値を代入
            Array.TryGetValue(type, out tmpCount);

            // 追加するポイントの配列をつくる
            int[] val = 
            {
                tmpCount[0] + pointValue[0],
                tmpCount[1] + pointValue[1]
            };

            // 獲得ポイントを加算する
            Array[type] = val;
        }


        // 獲得した食材の名前を獲得する
        private string getFoodName(DataPlayer data)
        {
            // 目の前にある食材の名前を返す
            return  Move.RayController.RayHitObject.GetComponent<GetValue>().Type;
        }

        // 獲得した食材のポイントと量を取得する
        private int[] getFoodPoint(DataPlayer data)
        {
            int[] val = 
            {
                Move.RayController.RayHitObject.GetComponent<GetValue>().Point,
                Move.RayController.RayHitObject.GetComponent<GetValue>().Amount
            };

            return val;
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
        

        /// <summary>
        /// Rayを操作するクラス
        /// </summary>
        public RayController RayController{get{return rayController;} set{rayController = value;}}
        private RayController rayController;


        // コンストラクタ
        public PlayerMove(DataPlayer tmpData)
        {

            RayController = new RayController(tmpData);
            
            moveSpeed = new PlayerMoveSpeed(tmpData.MoveSpeed);
            playerRotateRightPos = new PlayerRotateRightPos(tmpData.RotateRightPos);
            playerRotateLeftPos = new PlayerRotateLeftPos(tmpData.RotateLeftPos);
            playerRotateBackPos = new PlayerRotateBackPos(tmpData.RotateBackPos);
            playerRotateForwardPos = new PlayerRotateForwardPos(tmpData.RotateForWardPos);
            
        }

        public void MovementActor()
        {
            action(RayController.Object, RayController.Data);
        }

        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        private void action(GameObject players, DataPlayer tmpData)
        {//todo:移動は前後のみ左右は回転のみ
            Vector3 playerMovePos = Vector3.zero;
            Vector3 playerRotateAmount = players.transform.eulerAngles;

            if(Input.GetKey(tmpData.ControlleKey[0]))
            {
                playerMovePos = players.transform.forward;
            }
            else if(Input.GetKey(tmpData.ControlleKey[1]))
            {
                playerRotateAmount = -players.transform.up;
            }
            else if(Input.GetKey(tmpData.ControlleKey[2]))
            {
                playerMovePos -= players.transform.forward;
                playerRotateAmount = -players.transform.forward;
            }
            else if(Input.GetKey(tmpData.ControlleKey[3]))
            {
                playerRotateAmount = players.transform.up;
            }

            if(Input.GetKey(tmpData.ControlleKey[1])
            || Input.GetKey(tmpData.ControlleKey[3]))
            {
                // 回転を計算する
                players.transform.eulerAngles += playerRotateAmount;
            }
            

            if(RayController.PlayerCapsuleRay == null) return;

            // プレイヤーが何かと当たっていなければ移動できる
            // プレイヤー自身とレイに必ず当たるので2以下になっている
            if(!RayController.RayHitObject
            && RayController.PlayerOverlapCapsule.Amount.Length <= 2)
            {
                // 移動を計算する
                players.transform.localPosition +=
                playerMovePos * moveSpeed.Amount * Time.deltaTime;
            }
            else
            {
                // 後ろに移動する
                players.transform.localPosition -=
                playerMovePos * moveSpeed.Amount * Time.deltaTime;
            }
            
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
        /// <summary>
        /// 実際に操作するプレイヤーのオブジェクト
        /// </summary>
        /// <value></value>
        public GameObject Object{get{return playerobj;} set{playerobj = value;}}
        private GameObject playerobj;

        public PlayerCapsuleRayStartPos PlayerCapsuleRayStartPos{get{return playerCapsuleRayStartPos;} set{playerCapsuleRayStartPos = value;}}
        private PlayerCapsuleRayStartPos playerCapsuleRayStartPos;

        public PlayerCapsuleRayEndPos PlayerCapsuleRayEndPos{get{return playerCapsuleRayEndPos;} set{playerCapsuleRayEndPos = value;}}
        private PlayerCapsuleRayEndPos playerCapsuleRayEndPos;

        public PlayerCapsuleRayRadiuse PlayerCapsuleRayRadiuse{get{return playerCapsuleRayRadiuse;} set{playerCapsuleRayRadiuse = value;}}
        private PlayerCapsuleRayRadiuse playerCapsuleRayRadiuse;

        public PlayerCapsuleRayDistance PlayerCapsuleRayDistance{get{return playerCapsuleRayDistance;} set{playerCapsuleRayDistance = value;}}
        private PlayerCapsuleRayDistance playerCapsuleRayDistance;

        public PlayerOverlapCapsule PlayerOverlapCapsule{get{return playerOverlapCapsule;} set{playerOverlapCapsule = value;}}
        private PlayerOverlapCapsule playerOverlapCapsule;
        /// <summary>
        /// Rayが当たったオブジェクト
        /// </summary>
        public GameObject RayHitObject{get{return rayHitObject;} set{rayHitObject = value;}}
        private GameObject rayHitObject;

        public PlayerCapsuleRay PlayerCapsuleRay{get{return playerCapsuleRay;} set{playerCapsuleRay = value;}}
        private PlayerCapsuleRay playerCapsuleRay;

        public DataPlayer Data{get{return data;} set{data  = value;}}
        private DataPlayer data;

        public RayController(DataPlayer data)
        {
            // カプセル型のレイの距離
            PlayerCapsuleRayDistance = new PlayerCapsuleRayDistance(data.RayDirection);

            // カプセル型のレイの半径
            PlayerCapsuleRayRadiuse = new PlayerCapsuleRayRadiuse(data.RayRadiuse);

            Data = data;
        }

        public void RayUpdate()
        {
            CapsuleRayCast(Object);
        }

        // 筒型のレイを飛ばす
        private void CapsuleRayCast(GameObject players)
        {
            // プレイヤーから出る、カプセル型のレイの端の球の中心の座標
            PlayerCapsuleRayStartPos = new PlayerCapsuleRayStartPos(new Vector3(
                players.transform.localPosition.x + players.transform.forward.x / 10,
                0.5f,
                players.transform.localPosition.z + players.transform.forward.z / 10
            ));

            // プレイヤーから出る、カプセル型のレイの端の球の中心の座標
            PlayerCapsuleRayEndPos = new PlayerCapsuleRayEndPos(new Vector3(
                players.transform.localPosition.x + players.transform.forward.x / 10,
                3,
                players.transform.localPosition.z + players.transform.forward.z / 10
            ));

            Vector3 overlapStartPos = new Vector3(
                players.transform.localPosition.x + players.transform.forward.x / 5,
                0.5f,
                players.transform.localPosition.z + players.transform.forward.z / 5
            );

            Vector3 overlapEndPos = new Vector3(
                players.transform.localPosition.x + players.transform.forward.x / 5,
                3,
                players.transform.localPosition.z + players.transform.forward.z / 5
            );

            RaycastHit hit;

            // カプセル型のレイを飛ばす
            PlayerCapsuleRay = new PlayerCapsuleRay(Physics.CapsuleCast(
                PlayerCapsuleRayStartPos.Amount,
                PlayerCapsuleRayEndPos.Amount,
                PlayerCapsuleRayRadiuse.Amount,
                players.transform.forward,
                out hit,
                PlayerCapsuleRayDistance.Amount));

            // レイの内側を格納する
            PlayerOverlapCapsule = new PlayerOverlapCapsule(Physics.OverlapCapsule(
                overlapStartPos,
                overlapEndPos,
                PlayerCapsuleRayRadiuse.Amount
            ));


            // レイを見えるようにする
            VisualPhysics.CapsuleCast(
                overlapStartPos,
                overlapEndPos,
                PlayerCapsuleRayRadiuse.Amount,
                players.transform.forward,
                out hit,
                PlayerCapsuleRayDistance.Amount);

            // レイが当たったら
            if(PlayerCapsuleRay.Cast)
            {
                RayHitObject = hit.collider.gameObject;
            }
            else
            {
                // レイが外れたらnullにする
                RayHitObject = null;
            }
        }
    }


    /// <summary>
    /// プレイヤーが発する効果音
    /// </summary>
    public class PlayerAudio
    {
        public void GetFood(DataPlayer data)
        {
            if(data.PlayerAudioSource.isPlaying)
            {
                data.PlayerAudioSource.PlayOneShot(data.ActionSE[0]);
            }
        }
    }
    
}