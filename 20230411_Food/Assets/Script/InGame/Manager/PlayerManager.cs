using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterface;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GameManager;
using System;
using FoodPoint;
using Nomnom.RaycastVisualization;
using UnityEngine.Events;

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

        /// <summary>
        /// プレイヤー移動・回転クラス
        /// </summary>
        public PlayerMove PlayerMove{get{return playerMove;} set{playerMove = value;}}
        private PlayerMove playerMove;

        /// <summary>
        /// レイで当たり判定を得ているクラス
        /// </summary>
        /// <value></value>
        public RayController RayController{get{return rayController;} set{rayController = value;}}
        private RayController rayController;

        public PlayerNumber PlayerNumber{get{return playerNumber;} set{playerNumber = value;}}
        private PlayerNumber playerNumber;

        /// <summary>
        /// プレイヤー
        /// </summary>
        public GameObject Object{get; private set;}
        public Umbrella.UmbrellaController[] Umbrella{get;private set;} = new Umbrella.UmbrellaController[2];
        

        /// <summary>
        /// レイが食べ物に当たったものを取得
        /// </summary>
        public GameObject RayHitFoodObject{get; private set;}
        public GameObject RayHitNonFoodObject{get; private set;}
        public GameObject RayHitFloorObject{get; private set;}

        /// <summary>
        /// プレイヤー周りの箱のレイ
        /// </summary>
        public bool FrontBoxCast{get; private set;}
        public bool BackBoxCast{get; private set;}

        /// <summary>
        /// プレイヤーのレイの半径
        /// </summary>
        /// <value></value>
        public PlayerBoxRayHalfExtents PlayerRayRadiuse{get; private set;}

        // プレイヤーのパラメータ
        public DataPlayer DataPlayer{get{return dataPlayer;} set{dataPlayer = value;}}
        private DataPlayer dataPlayer;

        /// <summary>
        /// レイの当たったオブジェクト
        /// </summary>
        /// <param name="tmpRayHitObj"></param>
        private void setRayHitFoodObject(GameObject tmpRayHitObj)
        {
            RayHitFoodObject = tmpRayHitObj;
        }

        private void setRayHitNonFoodObject(GameObject tmpRayHitObj)
        {
            RayHitNonFoodObject = tmpRayHitObj;
        }
        private void setRayHitFloorObject(GameObject tmpRayHitObject)
        {
            RayHitFloorObject = tmpRayHitObject;
        }

        /// <summary>
        /// プレイヤーの周りのレイ
        /// </summary>
        /// <param name="tmpFrontRay"></param>
        private void setPlayerFrontRay(bool tmpFrontRay)
        {
            FrontBoxCast = tmpFrontRay;
        }
        private void setPlayerBackRay(bool tmpBackRay)
        {
            BackBoxCast = tmpBackRay;
        }
        
        /// <summary>
        /// レイの半径
        /// </summary>
        /// <param name="tmpRayRadiuse"></param>
        private void setPlayerRayRadiuse(PlayerBoxRayHalfExtents tmpRayRadiuse)
        {
            PlayerRayRadiuse = tmpRayRadiuse;
        }
        private void setPlayerData(DataPlayer tmpData)
        {
            DataPlayer = tmpData;
        }


        private UnityAction<GameObject> rayHitObjecFood = null;
        private UnityAction<GameObject> rayHitObjectNonFood = null;
        private UnityAction<GameObject> rayHitObjectFloor = null;
        private UnityEvent[] getHitObject = new UnityEvent[2];
        private UnityAction<DataPlayer> dataMethod = null;
        private UnityAction<bool> frontBoxCast = null;
        private UnityAction<bool> backBoxCast = null;
        private UnityAction<PlayerBoxRayHalfExtents> rayRadiuse = null;


        // コンストラクタ
        public PlayerManager(DataPlayer tmpData)
        {
            rayHitObjecFood = setRayHitFoodObject;
            rayHitObjectNonFood += setRayHitNonFoodObject;
            rayHitObjectFloor += setRayHitFloorObject;
            dataMethod = setPlayerData;
            frontBoxCast = setPlayerFrontRay;
            backBoxCast = setPlayerBackRay;
            rayRadiuse = setPlayerRayRadiuse;

            // 初期化
            Initialization(tmpData);
        }

        public async void Initialization(DataPlayer tmpData)
        {

            FoodPoint = new FoodPoint(tmpData);
            
            RayController = new RayController(rayHitObjecFood, rayHitObjectNonFood, rayHitObjectFloor, tmpData);
            
            // 取得するまで待つ
            DataHandle = Addressables.LoadAssetAsync<GameObject>(tmpData.AdressKey);
            await DataHandle.Task;

            // 生成座標を設定   
            PlayerInstancePos InstancePos = new PlayerInstancePos(tmpData.PlayerCreatePos);


            // 1Pプレイヤー生成
            var tmpObj = (GameObject)DataHandle.Result;
            RayController.Object = Object = MonoBehaviour.Instantiate(tmpObj
            , InstancePos.MainPos
            , tmpObj.transform.rotation);

            PlayerNumber = new PlayerNumber(tmpData.Number);

            PlayerMove = new PlayerMove(tmpData, 
                                        frontBoxCast, 
                                        backBoxCast,     
                                        rayHitObjecFood, 
                                        rayHitObjectNonFood, 
                                        rayHitObjectFloor,
                                        rayRadiuse);
            

            PlayerMove.InstanceAction();

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
            RayController.RayUpdate();
            
            // 各移動メソッド
            PlayerMove.MovementActor();

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
        /// 効果音クラス
        /// </summary>
        /// <value></value>
        public PlayerAudio PlayerAudio{get{return playerAudio;} set{playerAudio = value;}}
        private PlayerAudio playerAudio;

        private DataPlayer data;

        public FoodPoint(DataPlayer tmpData)
        {
            data = tmpData;
            int[] val = {0,0};
            Array.Add("MEAT", val);
            Array.Add("FISH", val);
            Array.Add("VEGETABLE", val);
            Array.Add("SEASOUSING", val);

            PlayerAudio = new PlayerAudio();
        }

        public void addPointUpdate()
        {
            AddPoint();
        }

        // ポイント獲得メソッド
        public void AddPoint()
        {
            // 何にもあたっていなければメソッドから抜ける
            if(!ObjectManager.PlayerManagers[data.Number].RayHitFoodObject) return;

            if(ObjectManager.PlayerManagers[data.Number].RayHitFoodObject.tag != "Food") return;

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
                args.presentPos = ObjectManager.PlayerManagers[data.Number].RayHitFoodObject.transform.position;
                // 座標を返す
                ReturnPresentPos(args);

                // 目の前の食材をキューに追加
                ObjectManager.ItemManager.itemFactory.Storing(ObjectManager.PlayerManagers[data.Number].RayHitFoodObject);
                
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
            
            if(ObjectManager.PlayerManagers[data.Number].RayHitFoodObject.tag == "Food")
            {
                ObjectManager.PlayerManagers[data.Number].RayHitFoodObject.SetActive(false);
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
            return  ObjectManager.PlayerManagers[data.Number].RayHitFoodObject.GetComponent<GetValue>().Type;
        }

        // 獲得した食材のポイントと量を取得する
        private int[] getFoodPoint(DataPlayer data)
        {
            int[] val = 
            {
                ObjectManager.PlayerManagers[data.Number].RayHitFoodObject.GetComponent<GetValue>().Point,
                ObjectManager.PlayerManagers[data.Number].RayHitFoodObject.GetComponent<GetValue>().Amount
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
        /// レイが当たったオブジェクトの辺の中点
        /// </summary>
        /// <value></value>
        public OutSide OutSide{get{return outSide;} set{outSide = value;}}
        private OutSide outSide;

        /// <summary>
        /// プレイヤーの半径・縦
        /// </summary>
        /// <value></value>
        public PlayerRadiuse PlayerRadiuse{get{return playerRadiuse;} set{playerRadiuse = value;}}
        private PlayerRadiuse playerRadiuse;

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
        
        private DataPlayer data;

        private UnityAction<bool> frontBoxRay;
        private UnityAction<bool> backBoxRay;
        private UnityAction<GameObject> rayHitFloorObject;
        private UnityAction<PlayerBoxRayHalfExtents> rayRadiuse;
        private UnityAction<GameObject> rayHitNonFoodObject;
        private UnityAction<GameObject> rayHitFoodObject;
        

        // コンストラクタ
        public PlayerMove(DataPlayer tmpData, 
                          UnityAction<bool> tmpFrontRay, 
                          UnityAction<bool> tmpBackRay, 
                          UnityAction<GameObject> tmpFoodObj,
                          UnityAction<GameObject> tmpNonFoodObj,
                          UnityAction<GameObject> tmpFloorObject,
                          UnityAction<PlayerBoxRayHalfExtents> tmpRadiuse)
        {
            frontBoxRay = tmpFrontRay;
            backBoxRay = tmpBackRay;
            rayHitFloorObject = tmpFloorObject;
            rayRadiuse = tmpRadiuse;
            rayHitNonFoodObject = tmpNonFoodObj;
            rayHitFoodObject = tmpFoodObj;

            data = tmpData;


            moveSpeed = new PlayerMoveSpeed(tmpData.MoveSpeed);    
        }

        public void InstanceAction()
        {
            PlayerRadiuse = new PlayerRadiuse(ObjectManager.PlayerManagers[data.Number].Object.transform.localScale.z / 2);
            Debug.Log(PlayerRadiuse.Value);
        }

        public void MovementActor()
        {
            action(ObjectManager.PlayerManagers[data.Number].Object, data); 
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
            
            
            // if(ObjectManager.PlayerManagers[tmpData.Number].RayController.PlayerFloorRayCast)
            // {
            //     // プレイヤーがステージの内側に収める
            //     insideStage(players, playerMovePos);
            // }
            // else
            // {

            // }
                


            // プレイヤーが食べ物以外と当たっていなければ移動できる
            if(!ObjectManager.PlayerManagers[data.Number].RayHitNonFoodObject
            && !ObjectManager.PlayerManagers[data.Number].RayHitFoodObject)
            {
                // 移動を計算する
                move(players, playerMovePos);
            }

            // レイが何かと当たったら
            else if(ObjectManager.PlayerManagers[data.Number].RayHitNonFoodObject
                 || ObjectManager.PlayerManagers[data.Number].RayHitFoodObject)
            {
                // 前方のレイに何かが当たったら
                if(ObjectManager.PlayerManagers[data.Number].FrontBoxCast)
                {
                    // プレイヤーのいる方向を決める
                    checkPlayerRayHitObjectSideFlag(players);
                    
                    // 当たったオブジェクトと平行に移動する
                    if(Input.GetKey(tmpData.ControlleKey[0]))
                    {

                        players.transform.position += scratchWall(players) * moveSpeed.Amount * Time.deltaTime / 2;

                        // 角から内側に侵入するのを防ぐ
                        if(Input.GetKey(tmpData.ControlleKey[1])
                        || Input.GetKey(tmpData.ControlleKey[3]))
                        {
                            // 後ろに下がる
                            playerMovePos -= players.transform.forward;

                            move(players, playerMovePos);
                        }
                    }


                    // 振り向き時のめり込み防止
                    if(Input.GetKey(tmpData.ControlleKey[1])
                    || Input.GetKey(tmpData.ControlleKey[3]))
                    {
                        // 後ろに下がる
                        playerMovePos -= players.transform.forward;

                        move(players, playerMovePos);
                    }

                    if(Input.GetKey(tmpData.ControlleKey[2]))
                    {
                        playerMovePos -= players.transform.forward;

                        move(players, playerMovePos);
                    }

                }
                // 後ろ向きで当たっている
                else if(ObjectManager.PlayerManagers[data.Number].BackBoxCast)
                {
                    
                    // プレイヤーのいる方向を決める
                    checkPlayerRayHitObjectSideFlag(players);

                    // 当たったオブジェクトと平行に移動する
                    if(Input.GetKey(tmpData.ControlleKey[2]))
                    {
                        players.transform.position += scratchWallMoonWalk(players) * moveSpeed.Amount * Time.deltaTime / 2;

                        // 角から内側に侵入するのを防ぐ
                        if(Input.GetKey(tmpData.ControlleKey[1])
                        || Input.GetKey(tmpData.ControlleKey[3]))
                        {
                            // 壁の外にずれる
                            playerMovePos += players.transform.forward;

                            move(players, playerMovePos);
                        }
                    }

                    // 振り向き時のめり込み防止
                    if(Input.GetKey(tmpData.ControlleKey[1])
                    || Input.GetKey(tmpData.ControlleKey[3]))
                    {
                        // 前に下がる
                        playerMovePos += players.transform.forward;

                        move(players, playerMovePos);
                    }

                    if(Input.GetKey(tmpData.ControlleKey[0]))
                    {
                        playerMovePos += players.transform.forward;

                        move(players, playerMovePos);
                    }
                }
            }
        }


        /// <summary>
        /// プレイヤーをステージの内側に収める
        /// </summary>
        private void insideStage(GameObject tmpPlayer, Vector3 tmpPlayerMovePos)
        {
            
            // プレイヤーが踏み込める外側の座標・上
            float playerPositiveBorderPosZ = ObjectManager.PlayerManagers[data.Number].RayHitFloorObject.transform.position.z + 
                                             getTableHalfScale(ObjectManager.PlayerManagers[data.Number].RayHitFloorObject).z -
                                             PlayerRadiuse.Value;

            // プレイヤーが踏み込める外側の座標・下
            float playerNegativeBorderPosZ = ObjectManager.PlayerManagers[data.Number].RayHitFloorObject.transform.position.z - 
                                             getTableHalfScale(ObjectManager.PlayerManagers[data.Number].RayHitFloorObject).z +
                                             PlayerRadiuse.Value;

            // プレイヤーが踏み込める外側の座標・左
            float playerPositiveBorderPosX = ObjectManager.PlayerManagers[data.Number].RayHitFloorObject.transform.position.x - 
                                             getTableHalfScale(ObjectManager.PlayerManagers[data.Number].RayHitFloorObject).x +
                                             PlayerRadiuse.Value;

            // プレイヤーが踏み込める外側の座標・右
            float playerNegativeBorderPosX = ObjectManager.PlayerManagers[data.Number].RayHitFloorObject.transform.position.x + 
                                             getTableHalfScale(ObjectManager.PlayerManagers[data.Number].RayHitFloorObject).x -
                                             PlayerRadiuse.Value;

            // プレイヤーが上へはみ出そう
            if(tmpPlayer.transform.position.z > playerPositiveBorderPosZ)
            {
                // 後ろへ下げる
                backwardPlayer(tmpPlayer, tmpPlayerMovePos);
            }

            // プレイヤーが下へはみ出そう
            if(tmpPlayer.transform.position.z < playerNegativeBorderPosZ)
            {
                // 後ろへ下げる
                backwardPlayer(tmpPlayer, tmpPlayerMovePos);
            }

            // プレイヤーが左へはみ出そう
            if(tmpPlayer.transform.position.x > playerNegativeBorderPosX)
            {
                // 後ろへ下げる
                backwardPlayer(tmpPlayer, tmpPlayerMovePos);
            }

            // プレイヤーが右へはみ出そう
            if(tmpPlayer.transform.position.x > playerPositiveBorderPosX)
            {
                // 後ろへ下げる
                backwardPlayer(tmpPlayer, tmpPlayerMovePos);
            }
        }


        /// <summary>
        /// プレイヤーを後ろに下げる
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <param name="tmpPlayerMovePos">移動量</param>
        private void backwardPlayer(GameObject tmpPlayer, Vector3 tmpPlayerMovePos)
        {
            tmpPlayerMovePos -= tmpPlayer.transform.forward;
        }
        

        /// <summary>
        /// プレイヤーの移動最終計算
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <param name="tmpPlayerMovePos">移動量</param>
        private void move(GameObject tmpPlayer, Vector3 tmpPlayerMovePos)
        {
            tmpPlayer.transform.localPosition +=
            tmpPlayerMovePos * moveSpeed.Amount * Time.deltaTime;
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
        /// プレイヤーがどの方向を向いているかによって移動方向を決める
        /// </summary>
        /// <param name="tmpPlayer">プレイヤー</param>
        /// <returns>当たったオブジェクトに対して平行方向の座標</returns>
        private Vector3 scratchWallMoonWalk(GameObject tmpPlayer)
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

                // プレイヤーは右から上向きの間を向いている
                if(judgeTopToRight(tmpPlayer))
                {
                    // 左にスライドして移動
                    playerMoveDirection = playerMovePos[3];
                }

                // 左から上
                else if(judgeLeftToTop(tmpPlayer))
                {
                    // 右にスライドして移動
                    playerMoveDirection = playerMovePos[2];
                }
                break;


                case playerLocation.LEFT:

                // 左上
                if(judgeLeftToTop(tmpPlayer))
                {
                    // 下にスライドして移動
                    playerMoveDirection = playerMovePos[1];
                }

                // 下から左
                else if(judgeBottomToLeft(tmpPlayer))
                {
                    // 上にスライドして移動
                    playerMoveDirection = playerMovePos[0];
                }
                break;


                case playerLocation.DOWN:

                if(judgeRightToBottom(tmpPlayer))
                {
                    playerMoveDirection = playerMovePos[3];
                }

                else if(judgeBottomToLeft(tmpPlayer))
                {
                    playerMoveDirection = playerMovePos[2];
                }
                break;


                case playerLocation.RIGHT:

                if(judgeRightToBottom(tmpPlayer))
                {
                    playerMoveDirection = playerMovePos[0];
                    
                }

                else if(judgeTopToRight(tmpPlayer))
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
        private void checkPlayerRayHitObjectSideFlag(GameObject tmpPlayer)
        {
            // 当たったオブジェクトの右側にいるなら
            if(tmpPlayer.transform.position.x > getRayHitObjectOutSidePos().Pos[0].x + ObjectManager.PlayerManagers[data.Number].PlayerRayRadiuse.Amount.x)
            {
                Position = playerLocation.RIGHT;
            }

            // 左
            else if(tmpPlayer.transform.position.x < getRayHitObjectOutSidePos().Pos[1].x - ObjectManager.PlayerManagers[data.Number].PlayerRayRadiuse.Amount.x)
            {
                Position = playerLocation.LEFT;
            }

            // 上
            else if(tmpPlayer.transform.position.z >= getRayHitObjectOutSidePos().Pos[0].z)
            {
                Position = playerLocation.UP;
            }

            // 下
            else if(tmpPlayer.transform.position.z <= getRayHitObjectOutSidePos().Pos[1].z)
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
            Vector3 rayHitObjectPositivePos = ObjectManager.PlayerManagers[data.Number].RayHitNonFoodObject.transform.position 
                                            + getTableHalfScale(ObjectManager.PlayerManagers[data.Number].RayHitNonFoodObject);

            Vector3 rayHitObjectNegativePos = ObjectManager.PlayerManagers[data.Number].RayHitNonFoodObject.transform.position 
                                            - getTableHalfScale(ObjectManager.PlayerManagers[data.Number].RayHitNonFoodObject);

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
        private Vector3 getTableHalfScale(GameObject tmpRayHitObject)
        {

            // 机の半径を取得
            return tmpRayHitObject.transform.localScale / 2;
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
        /// Rayが当たったオブジェクト・食べ物
        /// </summary>
        public GameObject RayHitObjectFood{get{return rayHitObjectFood;} set{rayHitObjectFood = value;}}
        private GameObject rayHitObjectFood;

        /// <summary>
        /// Rayが当たったオブジェクト・食べ物と床以外
        /// </summary>
        /// <value></value>
        public GameObject RayHitObjectNonFood{get{return rayHitObjectNonFood;} set{rayHitObjectNonFood = value;}}
        private GameObject rayHitObjectNonFood;

        /// <summary>
        /// Rayが当たったオブジェクト・床
        /// </summary>
        /// <value></value>
        public GameObject RayHitFloorObject{get{return rayHitFloorObject;} set{rayHitFloorObject = value;}}
        private GameObject rayHitFloorObject;

        private DataPlayer dataPlayer;
        public bool PlayerFrontBoxCast{get; protected set;}
        public bool PlayerBackBoxCast{get; protected set;}
        public bool PlayerFoodBoxCast{get; protected set;}
        public bool PlayerFloorRayCast{get; protected set;}


        private UnityAction<GameObject> rayHitFoodObject;
        private UnityAction<GameObject> rayHitNonFoodObject;
        private UnityAction<GameObject> rayHitObjectFloor;

        public RayController(UnityAction<GameObject> tmpRayHitObj, 
                             UnityAction<GameObject> tmpNonFoodObj, 
                             UnityAction<GameObject> tmpFloorObj, 
                             DataPlayer tmpData)
        {
            rayHitFoodObject = tmpRayHitObj;
            rayHitNonFoodObject = tmpNonFoodObj;
            rayHitObjectFloor = tmpFloorObj;

            dataPlayer = tmpData;
            // レイの半径
            PlayerBoxRayHalfExtents = new PlayerBoxRayHalfExtents(tmpData.RayRadiuse);
        }

        public void RayUpdate()
        {
            BoxCast(Object, dataPlayer);
        }

        // 筒型のレイを飛ばす
        private void BoxCast(GameObject players, DataPlayer tmpData)
        {

            RaycastHit hit;
            // Foodのみ当たる
            int foodOnlyLayer = 1 << 7;
            // Food以外に当たる
            int nonFoodLayer = ~(1 << 7);
            // 床にのみ当たる
            int floorLayer = 1 << 9;

            Vector3 BoxCenter = new Vector3(
                players.transform.localPosition.x,
                1,
                players.transform.localPosition.z
            );
            
            Vector3 FloorRayDirection = new Vector3(
                0, -Vector3.up.y, 0
            );
            

            // 正面のレイ
            PlayerFrontBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                players.transform.rotation,
                dataPlayer.RayDirection,
                nonFoodLayer);

            // 背面のレイ
            PlayerBackBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                -players.transform.forward,
                out hit,
                players.transform.rotation,
                dataPlayer.RayDirection,
                nonFoodLayer);


            // 真下に床用のレイを飛ばす
            PlayerFloorRayCast = Physics.Raycast(
                players.transform.position,
                FloorRayDirection,
                out hit,
                players.transform.localScale.y,
                floorLayer
            );

            


            // 食材用のレイ
            PlayerFoodBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                players.transform.rotation,
                dataPlayer.RayDirection,
                foodOnlyLayer);


            // レイを見えるようにする
            VisualPhysics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                players.transform.rotation,
                dataPlayer.RayDirection);

            VisualPhysics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                -players.transform.forward,
                players.transform.rotation,
                dataPlayer.RayDirection);

            VisualPhysics.Raycast(
                players.transform.position,
                FloorRayDirection,
                players.transform.localScale.y
            );


            // レイを飛ばして当たったら
            if(PlayerFoodBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                players.transform.rotation,
                dataPlayer.RayDirection,
                foodOnlyLayer))
            {
                
                if(hit.collider != null)
                {
                    ObjectManager.PlayerManagers[dataPlayer.Number].Umbrella[dataPlayer.Number].
                        hitObjectSubject.OnNext(hit.collider.gameObject);
                        
                    RayHitObjectFood = hit.collider.gameObject;
                }  

            }
            else
            {
                ObjectManager.PlayerManagers[dataPlayer.Number].Umbrella[dataPlayer.Number].
                    hitObjectSubject.OnNext(null);

                RayHitObjectFood = null;
            }
            
            // 食べ物以外に当たった
            if((PlayerFrontBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                players.transform.forward,
                out hit,
                players.transform.rotation,
                dataPlayer.RayDirection,
                nonFoodLayer))

                ||
                
                (PlayerBackBoxCast = Physics.BoxCast(
                BoxCenter,
                PlayerBoxRayHalfExtents.Amount,
                -players.transform.forward,
                out hit,
                players.transform.rotation,
                dataPlayer.RayDirection,
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

            // 床を取得
            if(PlayerFloorRayCast = Physics.Raycast(
                players.transform.position,
                players.transform.position,
                out hit,
                players.transform.localScale.y,
                floorLayer
            ))
            {
                if(hit.collider != null)
                {
                    Debug.Log(hit.collider.name);
                    RayHitFloorObject = hit.collider.gameObject;
                }
            }
            else
            {
                RayHitFloorObject = null;
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