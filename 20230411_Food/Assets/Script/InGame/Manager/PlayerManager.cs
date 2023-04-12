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
        public DataPlayer DataPlayer{get; private set;} = null;

        /// <summary>
        /// プレイヤー移動クラス
        /// </summary>
        /// <value></value>
        public PlayerMove PlayerMove{get; private set;}

        /// <summary>
        /// プレイヤー回転クラス
        /// </summary>
        /// <value></value>
        public PlayerRotate PlayerRotate{get; private set;}

        // コンストラクタ
        public PlayerManager()
        {
            // 初期化
            Initialization();

            // 更新
            Update();
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
        }
    }

    /// <summary>
    /// プレイヤーの移動クラス
    /// </summary>
    public class PlayerMove
    {
        
        public PlayerMoveSpeed MoveSpeed{get; private set;}

        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        public void ForwardMove()
        {
            if(Input.GetKey("W"))
            {
                ObjectManager.Player.Object.transform.position +=
                ObjectManager.Player.Object.transform.forward * MoveSpeed.moveSpeed * Time.deltaTime;
            }
        }

        public void BackMove()
        {
            if(Input.GetKey("S"))
            {
                ObjectManager.Player.Object.transform.position +=
                -ObjectManager.Player.Object.transform.forward * MoveSpeed.moveSpeed * Time.deltaTime;
            }
        }

        public void RightMove()
        {
            if(Input.GetKey("D"))
            {
                ObjectManager.Player.Object.transform.position +=
                ObjectManager.Player.Object.transform.right * MoveSpeed.moveSpeed * Time.deltaTime;
            }
        }

        public void LeftMove()
        {
            if(Input.GetKey("A"))
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

        // 回転させる
        public void Rotate()
        {
            if(Input.GetKey("D"))
            {
                ObjectManager.Player.Object.transform.eulerAngles = PlayerRotatePos.Rotate * Time.deltaTime;
            }

            if(Input.GetKey("A"))
            {
                ObjectManager.Player.Object.transform.eulerAngles = -PlayerRotatePos.Rotate * Time.deltaTime;
            }
        }
    }
}