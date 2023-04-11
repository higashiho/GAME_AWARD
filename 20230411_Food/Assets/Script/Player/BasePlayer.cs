using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterface;

namespace player
{
    public class BasePlayer : AdjustPlayer, IActor
    {
        public void Initialization()
        {
            Data = new DataPlayer();
        }

        public void Update()
        {
            
        }
    }

    public class GetPlayerObject : AdjustPlayer
    {
        public void Instance()
        {
            // 生成座標を設定   
            InstancePosOne = new PlayerInstancePos(Data.FirstPlayerCreatePos);

            // プレイヤー生成
            PlayerObject = MonoBehaviour.Instantiate(PlayerPref
            , InstancePosOne.GetInitPos()
            , Quaternion.identity);
        }
    }

    public class PlayerMove
    {
        public void Move()
        {
            
        }
    }
}