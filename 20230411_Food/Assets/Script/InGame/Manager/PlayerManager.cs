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

        public GameObject Object{get; private set;}
        public Umbrella.UmbrellaController[] Umbrella{get;private set;} = new Umbrella.UmbrellaController[2];
        
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
            FoodPoint.Move.RayController.Object = Object =  MonoBehaviour.Instantiate(tmpObj
            , InstancePos.MainPos
            , tmpObj.transform.rotation);

            PlayerNumber = new PlayerNumber(tmpData.Number);
            // 笠は自身の子に1つしか存在しないため要素数０を取得する
            foreach(Transform cheld in GetChildrenRecursive(Object.transform))
            {
                var umburella = cheld.GetComponent<Umbrella.UmbrellaController>();

                if(umburella == null) continue;

                Umbrella[PlayerNumber.Index] = umburella;
                Debug.Log("in");
                break;
            }
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

        /// <summary>
        /// 親を含む子オブジェクトを再帰的に取得メソッド
        /// </summary>
        /// <param name="parent">取得する親オブジェクト</param>
        /// <returns>子オブジェクトの配列</returns>
        private Transform[] GetChildrenRecursive(Transform parent)
        {
            // trueを指定しないと非アクティブなオブジェクトを取得できないことに注意
            var parentAndChildren = parent.GetComponentsInChildren<Transform>(true);

            // 子オブジェクトの格納用配列作成
            var children = new Transform[parentAndChildren.Length - 1];

            // 親を除く子オブジェクトを結果にコピー
            Array.Copy(parentAndChildren, 1, children, 0, children.Length);

            // 子オブジェクトが再帰的に格納された配列
            return children;
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
            if(!Move.RayController.RayHitObjectFood) return;

            if(Move.RayController.RayHitObjectFood.tag != "Food") return;

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
                args.presentPos = Move.RayController.RayHitObjectFood.transform.position;
                // 座標を返す
                ReturnPresentPos(args);

                // 目の前の食材をキューに追加
                ObjectManager.ItemManager.itemFactory.Storing(Move.RayController.RayHitObjectFood);
                
                // 座標を返す
                ReturnPresentPos(args);

                
                
                // １回取得すると消える
                deleteFood();

                // 肉に１点加算
                incrimentDictionary(getFoodName(data), getFoodPoint(data));
                //Debug.Log(Array.FirstOrDefault());

                // 獲得音を鳴らす
                PlayerAudio.GetFood(data);
                
                return;
            }
        }

        // １回取得すると消える
        private void deleteFood()
        {
            
            if(Move.RayController.RayHitObjectFood.tag == "Food")
            {
                Move.RayController.RayHitObjectFood.SetActive(false);
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
            if(type == "SEASOUSING")
            {
                var result = Array[type];
                //  調味料ポイントが0じゃなっかったらreturn
                if(result[0] != 0)  return;
                
            } 
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
            return  Move.RayController.RayHitObjectFood.GetComponent<GetValue>().Type;
        }

        // 獲得した食材のポイントと量を取得する
        private int[] getFoodPoint(DataPlayer data)
        {
            int[] val = 
            {
                Move.RayController.RayHitObjectFood.GetComponent<GetValue>().Point,
                Move.RayController.RayHitObjectFood.GetComponent<GetValue>().Amount
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


        public OutSide OutSide{get{return outSide;} set{outSide = value;}}
        private OutSide outSide;

        private Vector3[] outSidePos = {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
        };


        private enum playerLocation
        {
            NOHIT,

            UP,     // 上にいる

            LEFT,   // 左

            DOWN,   // 下

            RIGHT,  // 右
        }
        private playerLocation Position;

        // コンストラクタ
        public PlayerMove(DataPlayer tmpData)
        {

            RayController = new RayController(tmpData);
            
            moveSpeed = new PlayerMoveSpeed(tmpData.MoveSpeed);            
        }

        public void MovementActor()
        {
            action(RayController.Object, RayController.Data);
        }

        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        private void action(GameObject players, DataPlayer tmpData)
        {
            Vector3 playerMovePos = Vector3.zero;
            Vector3 playerRotateAmount = Vector3.zero;

            // プレイヤーの今の座標
            Vector3 playerNowPos = players.transform.position;

           

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
            

            // プレイヤーが食べ物以外と当たっていなければ移動できる
            if(!RayController.RayHitObjectNonFood)
            {
                // 移動を計算する
                players.transform.localPosition +=
                playerMovePos * moveSpeed.Amount * Time.deltaTime;
            }

            else
            // レイが食べ物意外と当たったら
            {
                if(Input.GetKey(tmpData.ControlleKey[0]))
                {
                    // プレイヤーのいる方向を決める
                    checkPlayerRayHitObjectSideFlag(players);
                    

                    players.transform.position += scratchWall(players) * moveSpeed.Amount * Time.deltaTime / 2;
                    
                }
                else if(Input.GetKey(tmpData.ControlleKey[2]))
                {
                    // プレイヤーのいる方向を決める
                    checkPlayerRayHitObjectSideFlag(players);


                    players.transform.position += scratchWall(players) * moveSpeed.Amount * Time.deltaTime / 2;
                }
            }
        }

        /// <summary>
        /// プレイヤーがどの方向を向いているかによって移動方向を決める
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <returns>当たったオブジェクトに対して平行方向の座標</returns>
        private Vector3 scratchWall(GameObject tmpPlayer)
        {
            Vector3 playerMoveDirection = new Vector3(0, 0, 0);

            Vector3[] playerMovePos = {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
            };

            // プレイヤーが当たったオブジェクトに対してどこにいるか
            switch(Position)
            {
                // オブジェクトの上にいる
                case playerLocation.UP:

                // プレイヤーは右から下向きの間を向いている
                if(judgeRightToBottom(tmpPlayer))
                {
                    // 右にスライドして移動
                    playerMoveDirection = playerMovePos[2];
                }

                // 下から左
                else if(judgeBottomToLeft(tmpPlayer))
                {
                    // 左にスライドして移動
                    playerMoveDirection = playerMovePos[3];
                }
                break;


                case playerLocation.LEFT:

                // 右から下
                if(judgeRightToBottom(tmpPlayer))
                {
                    // 右にスライドして移動
                    playerMoveDirection = playerMovePos[1];
                }

                // 上から右
                else if(judgeTopToRight(tmpPlayer))
                {
                    // 左にスライドして移動
                    playerMoveDirection = playerMovePos[0];
                }
                break;


                case playerLocation.DOWN:

                if(judgeTopToRight(tmpPlayer))
                {
                    playerMoveDirection = playerMovePos[2];
                }

                else if(judgeLeftToTop(tmpPlayer))
                {
                    playerMoveDirection = playerMovePos[3];
                }
                break;


                case playerLocation.RIGHT:

                if(judgeLeftToTop(tmpPlayer))
                {
                    playerMoveDirection = playerMovePos[0];
                    
                }

                else if(judgeBottomToLeft(tmpPlayer))
                {
                    playerMoveDirection = playerMovePos[1];
                }
                break;
            }



            return playerMoveDirection;
            
        }

        /// <summary>
        /// プレイヤーがどの方向を向いているのか
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <returns>上から右の間の方向に向いている</returns>
        private bool judgeTopToRight(GameObject tmpPlayer)
        {
            // 上から右
            // 今回はy軸しか触らないのでこれでいいが、ジンバルロックを回避するならクオータニオンを使用すること
            if(tmpPlayer.transform.localEulerAngles.y > 0
            && tmpPlayer.transform.localEulerAngles.y < 90)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// プレイヤーがどの方向を向いているのか
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <returns>右から下の間の方向に向いている</returns>
        private bool judgeRightToBottom(GameObject tmpPlayer)
        {
            // 右から下
            if(tmpPlayer.transform.localEulerAngles.y > 90
            && tmpPlayer.transform.localEulerAngles.y < 180)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// プレイヤーがどの方向を向いているのか
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <returns>下から左の間の方向に向いている</returns>
        private bool judgeBottomToLeft(GameObject tmpPlayer)
        {
            // 下から左
            if(tmpPlayer.transform.localEulerAngles.y > 180
            && tmpPlayer.transform.localEulerAngles.y < 270)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// プレイヤーがどの方向を向いているのか
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <returns>左から上の間の方向に向いている</returns>
        private bool judgeLeftToTop(GameObject tmpPlayer)
        {
            // 左から上
            if(tmpPlayer.transform.localEulerAngles.y > 270
            && tmpPlayer.transform.localEulerAngles.y < 360)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// プレイヤーが当たったオブジェクトのどの方向にいるのか
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <returns>条件に当てはまった方向</returns>
        private void checkPlayerRayHitObjectSideFlag(GameObject tmpPlayer)
        {
            // 当たったオブジェクトの右側にいるなら
            if(tmpPlayer.transform.position.x > getRayHitObjectOutSidePos().Pos[0].x)
            {
                Position = playerLocation.RIGHT;
            }

            // 左
            else if(tmpPlayer.transform.position.x < getRayHitObjectOutSidePos().Pos[1].x)
            {
                Position = playerLocation.LEFT;
            }

            // 上
            else if(tmpPlayer.transform.position.z > getRayHitObjectOutSidePos().Pos[0].z)
            {
                Position = playerLocation.UP;
            }

            // 下
            else if(tmpPlayer.transform.position.z < getRayHitObjectOutSidePos().Pos[1].z)
            {
                Position = playerLocation.DOWN;
            }

            else
            {
                Position = playerLocation.NOHIT;
            }
            
        }

        

        /// <summary>
        /// プレイヤーの目の前の侵入できない座標の境界を配列化して取得する
        /// </summary>
        /// <returns>レイが当たったオブジェクトの外側の座標の配列・Vector3[]</returns>
        private OutSide getRayHitObjectOutSidePos()
        {
            // 当たったオブジェクトの中点の座標
            Vector3 rayHitObjectPositivePos = RayController.RayHitObjectNonFood.transform.position + getTableHalfScale();
            Vector3 rayHitObjectNegativePos = RayController.RayHitObjectNonFood.transform.position - getTableHalfScale();

            // 侵入できない座標配列
            Vector3[] rayHitObjectSidePoses = 
            {
                rayHitObjectPositivePos,
                rayHitObjectNegativePos
            };


            for(int i = 0; i < rayHitObjectSidePoses.Length; i++)
            {
                outSidePos[i] = rayHitObjectSidePoses[i];
            }
            
            return OutSide = new OutSide(outSidePos);
        }


        /// <summary>
        /// 机オブジェクトの半径を取得
        /// </summary>
        /// <param name="tmpData">プレイヤーのデータ</param>
        private Vector3 getTableHalfScale()
        {

            // 机の半径を取得
            return RayController.RayHitObjectNonFood.transform.localScale / 2;
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


        /// <summary>
        /// Rayが当たったオブジェクト
        /// </summary>
        public GameObject RayHitObjectFood{get{return rayHitObjectFood;} set{rayHitObjectFood = value;}}
        private GameObject rayHitObjectFood;

        public GameObject RayHitObjectNonFood{get{return rayHitObjectNonFood;} set{rayHitObjectNonFood = value;}}
        private GameObject rayHitObjectNonFood;

        public DataPlayer Data{get{return data;} set{data  = value;}}
        private DataPlayer data;

        public bool PlayerFrontBoxCast{get; protected set;}
        public bool PlayerBackBoxCast{get; protected set;}
        public RayController(DataPlayer data)
        {

            // レイの半径
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
            // Foodのみ当たる
            int foodOnlyLayer = 1 << 7;
            // Food以外に当たる
            int nonFoodLayer = ~(1 << 7);

            Vector3 BoxCenter = new Vector3(
                players.transform.localPosition.x,
                1,
                players.transform.localPosition.z
            );

            // 正面のレイ
            PlayerFrontBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                Quaternion.identity,
                data.RayDirection,
                nonFoodLayer);

            // 背面のレイ
            PlayerBackBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                -players.transform.forward,
                out hit,
                Quaternion.identity,
                data.RayDirection,
                nonFoodLayer);


            // レイを見えるようにする
            VisualPhysics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                Quaternion.identity,
                data.RayDirection);

            VisualPhysics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                -players.transform.forward,
                Quaternion.identity,
                data.RayDirection);


            // レイを飛ばして当たったら
            if(Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                Quaternion.identity,
                data.RayDirection,
                foodOnlyLayer))
            {
                
                if(hit.collider != null)
                {
                    ObjectManager.PlayerManagers[data.Number].Umbrella[data.Number].
                        hitObjectSubject.OnNext(hit.collider.gameObject);
                        
                    RayHitObjectFood = hit.collider.gameObject;
                }  

            }
            else
            {
                ObjectManager.PlayerManagers[data.Number].Umbrella[data.Number].
                    hitObjectSubject.OnNext(null);

                RayHitObjectFood = null;
            }
            
            // 食べ物以外に当たった
            if((PlayerFrontBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                Quaternion.identity,
                data.RayDirection,
                nonFoodLayer))

                ||
                
                (PlayerBackBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                -players.transform.forward,
                out hit,
                Quaternion.identity,
                data.RayDirection,
                nonFoodLayer)))
            {

                if(hit.collider != null)
                {

                    RayHitObjectNonFood = hit.collider.gameObject;
                }
                    
            }
            else
            {
                RayHitObjectNonFood = null;
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