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
                // 目の前の食材をキューに追加
                ObjectManager.ItemManager.itemFactory.Storing(Move.RayController.RayHitObject);
                // 座標を返す
                ReturnPresentPos(args);

                
                
                // １回取得すると消える
                //deleteFood(data);

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
            return Move.RayController.RayHitObject.GetComponent<GetValue>().Type;
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
        

        /// <summary>
        /// Rayを操作するクラス
        /// </summary>
        public RayController RayController{get{return rayController;} set{rayController = value;}}
        private RayController rayController;


        public Stage Stage{get{return stage;} set{stage = value;}}
        private Stage stage;

        public OutSide OutSide{get{return outSide;} set{outSide = value;}}
        private OutSide outSide;

        // コンストラクタ
        public PlayerMove(DataPlayer tmpData)
        {
            Vector3[] tmpVector3 = {};

            Vector3 tmpPos = new Vector3(0, 0, 0);

            RayController = new RayController(tmpData);
            
            moveSpeed = new PlayerMoveSpeed(tmpData.MoveSpeed);

            for(int i = 0; i < tmpData.Table.Length; i++)
            {
                tmpVector3[i] = tmpData.Table[i].transform.localScale;
                
                // オブジェクトの外側の座標を求める
                tmpPos = tmpData.Table[i].transform.position - getTableScale(tmpData)[i];
            }
            
            Stage = new Stage(tmpVector3);

            OutSide = new OutSide(tmpPos);
            
        }

        public void MovementActor()
        {
            action(RayController.Object, RayController.Data);
        }// todo:移動を壁にめり込まないような処理にする

        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        private void action(GameObject players, DataPlayer tmpData)
        {
            Vector3 playerMovePos = Vector3.zero;
            Vector3 playerRotateAmount = Vector3.zero;

            if(Input.GetKey(tmpData.ControlleKey[0]))
            {
                playerMovePos = players.transform.forward;
            }
            if(Input.GetKey(tmpData.ControlleKey[1]))
            {
                playerRotateAmount = -players.transform.up;
            }
            if(Input.GetKey(tmpData.ControlleKey[2]))
            {
                playerMovePos -= players.transform.forward;
            }
            if(Input.GetKey(tmpData.ControlleKey[3]))
            {
                playerRotateAmount = players.transform.up;
            }

            if(playerRotateAmount != Vector3.zero)
            {
                // 回転を計算する
                players.transform.eulerAngles += playerRotateAmount;
            }
            

            // プレイヤーが何かと当たっていなければ移動できる
            if(RayController.RayHitObject == null)
            {
                // 移動を計算する
                players.transform.localPosition +=
                playerMovePos * moveSpeed.Amount * Time.deltaTime;
            }
            else
            {

            }
        }

        // 机オブジェクトを取得
        private Vector3[] getTableScale(DataPlayer tmpData)
        {
            Vector3[] tmpPlayerPos;

            for(int i = 0; i < tmpData.Table.Length; i++)
            {
                // 机の半径を取得
                Stage.Scale[i] /= 2;
            }

            return tmpPlayerPos = Stage.Scale;
        }

        // プレイヤーが壁にめり込んだら横にスライドする
        private void returnPlayer(DataPlayer tmpData, GameObject tmpPlayer)
        {
            for(int i = 0; i < tmpData.Table.Length; i++)
            {
                if(RayController.RayHitObject == tmpData.Table[i])
                {
                    // プレイヤーは当たったオブジェクトより奥か手前か
                    // 右上
                    if(tmpPlayer.transform.position.x > RayController.RayHitObject.transform.position.x
                    && tmpPlayer.transform.position.z > RayController.RayHitObject.transform.position.z)
                    {
                        // 右移動か左移動か
                        
                    }

                    // 左上
                    if(tmpPlayer.transform.position.x < RayController.RayHitObject.transform.position.x
                    && tmpPlayer.transform.position.z > RayController.RayHitObject.transform.position.z)
                    {

                    }

                    // 右下
                    if(tmpPlayer.transform.position.x > RayController.RayHitObject.transform.position.x
                    && tmpPlayer.transform.position.z < RayController.RayHitObject.transform.position.z)
                    {
                        
                    }

                    // 左下
                    if(tmpPlayer.transform.position.x < RayController.RayHitObject.transform.position.x
                    && tmpPlayer.transform.position.z < RayController.RayHitObject.transform.position.z)
                    {
                        
                    }


                    // オブジェクトの内側か判断
                    if(tmpPlayer.transform.position.x > OutSide.Pos.x
                    || tmpPlayer.transform.position.z > OutSide.Pos.z)
                    {
                        tmpPlayer.transform.position = OutSide.Pos;
                        break;
                    }
                    
                }
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

        public PlayerBoxRayHalfExtents PlayerBoxRayHalfExtents{get{return playerBoxRayHalfExtents;} set{playerBoxRayHalfExtents = value;}}
        private PlayerBoxRayHalfExtents playerBoxRayHalfExtents;

        public PlayerBoxRayDistance PlayerBoxRayDistance{get{return playerBoxRayDistance;} set{playerBoxRayDistance = value;}}
        private PlayerBoxRayDistance playerBoxRayDistance;


        /// <summary>
        /// Rayが当たったオブジェクト
        /// </summary>
        public GameObject RayHitObject{get{return rayHitObject;} set{rayHitObject = value;}}
        private GameObject rayHitObject;

        public DataPlayer Data{get{return data;} set{data  = value;}}
        private DataPlayer data;

        public RayController(DataPlayer data)
        {
            // カプセル型のレイの距離
            PlayerBoxRayDistance = new PlayerBoxRayDistance(data.RayDirection);

            // カプセル型のレイの半径
            PlayerBoxRayHalfExtents = new PlayerBoxRayHalfExtents(data.RayRadiuse);

            Data = data;
        }

        public void RayUpdate()
        {
            BoxCast(Object);
        }

        // 筒型のレイを飛ばす
        private void BoxCast(GameObject players)
        {

            RaycastHit hit;

            Vector3 BoxCenter = new Vector3(
                players.transform.localPosition.x,
                1,
                players.transform.localPosition.z
            );


            // レイを見えるようにする
            VisualPhysics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                Quaternion.identity,
                data.RayDirection);


            // レイを飛ばしてレイが当たったら
            if(Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                Quaternion.identity,
                data.RayDirection))
            {
                if(hit.collider != null)
                {
                    RayHitObject = hit.collider.gameObject;
                }

            }
            else
            {
                // 埋もれている
                if(Physics.CheckBox(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                Quaternion.identity))
                {
                    if(hit.collider != null)
                    {
                        RayHitObject = hit.collider.gameObject;
                    }
                }

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