using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterface;

namespace player
{
    public class PlayerManager : BasePlayer, IActor
    {
        public void Initialization()
        {
            Data = new DataPlayer();
            PlayerInstance = new PlayerInstance();

            PlayerInstance.Instance();
        }

        public void Update()
        {
            
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

    public class PlayerMove
    {
        /// <summary>
        /// プレイヤーを移動させる
        /// </summary>
        public void Move()
        {
            
        }
    }
}