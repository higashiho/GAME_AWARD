using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterface;

namespace player
{
    public class PlayerManager : BasePlayer, IActor
    {
        // コンストラクタ
        public PlayerManager()
        {
            // 初期化
            Initialization();

            // 更新
            Update();
        }

        public void Initialization()
        {
            Data = new DataPlayer();
            PlayerInstance = new PlayerInstance();
            PlayerMove = new PlayerMove();

            PlayerInstance.Instance();
        }

        public void Update()
        {
            PlayerMove.ForwardMove();
            PlayerMove.BackMove();
            PlayerMove.RightMove();
            PlayerMove.LeftMove();
        }
    }

    public class PlayerInstance : BasePlayer
    {
        /// <summary>
        /// プレイヤーを生成
        /// </summary>
        public void Instance()
        {
            // 生成座標を設定   
            InstancePosOne = new PlayerInstancePos(Data.FirstPlayerCreatePos);

            // プレイヤー生成
            PlayerObject = MonoBehaviour.Instantiate(Data.PlayerPrefab
            , InstancePosOne.pos
            , Quaternion.identity);
        }
    }

    public class PlayerMove : BasePlayer
    {
        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        public void ForwardMove()
        {
            if(Input.GetKey("W"))
            {
                PlayerObject.transform.position += PlayerObject.transform.forward * Speed.moveSpeed * Time.deltaTime;
            }
        }

        public void BackMove()
        {
            if(Input.GetKey("S"))
            {
                PlayerObject.transform.position += -PlayerObject.transform.forward * Speed.moveSpeed * Time.deltaTime;
            }
        }

        public void RightMove()
        {
            if(Input.GetKey("D"))
            {
                PlayerObject.transform.position += PlayerObject.transform.right * Speed.moveSpeed * Time.deltaTime;
            }
        }

        public void LeftMove()
        {
            if(Input.GetKey("A"))
            {
                PlayerObject.transform.position += -PlayerObject.transform.right * Speed.moveSpeed * Time.deltaTime;
            }
        }
    }

    public class PlayerTakeFood : BasePlayer
    {
        // 目の前にある食べ物
        RaycastHit RayHitFood;

        // 食べ物を手に取る
        public void TakeFood()
        {
            //if()
        }
    }
}