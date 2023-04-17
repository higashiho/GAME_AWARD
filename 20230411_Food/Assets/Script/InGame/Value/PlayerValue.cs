using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManager;

namespace player
{
    public class PlayerValue
    {
        
    }

    // プレイヤー生成座標クラス
    public sealed class PlayerInstancePos
    {
        public Vector3 pos{get; private set;}

        // コンストラクタ
        public PlayerInstancePos(Vector3 playerPos)
        {
            // 初期化
            pos = playerPos;
        }
    }

    // プレイヤーの速度クラス
    public sealed class PlayerMoveSpeed
    {
        public float moveSpeed{get; private set;}

        // コンストラクタ
        public PlayerMoveSpeed()
        {
            // 初期化
            moveSpeed = ObjectManager.Player.DataPlayer.MoveSpeed;
        }
    }


    // プレイヤー回転座標クラス
    public sealed class PlayerRotatePos
    {
        public Vector3 Rotate{get; private set;}

        // コンストラクタ
        public PlayerRotatePos()
        {
            // 初期化
            Rotate = ObjectManager.Player.DataPlayer.RotatePos;
        }
    }

    // プレイヤーから出るRayの長さの最大値のクラス
    public sealed class PlayerRayDirection
    {
        public float RayDirection{get; private set;}

        // コンストラクタ
        public PlayerRayDirection()
        {
            RayDirection = ObjectManager.Player.DataPlayer.RayDirection;
        }
    }
}

