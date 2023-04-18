using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManager;

namespace player
{
    public class PlayerValue
    {
        
    }

    // 1Pプレイヤー生成座標クラス
    public sealed class FirstPlayerInstancePos
    {
        public Vector3 onePos{get; private set;}

        // コンストラクタ
        public FirstPlayerInstancePos()
        {
            // 初期化
            onePos = ObjectManager.Player.DataPlayer.FirstPlayerCreatePos;
        }
    }

    // 2Pプレイヤー生成座標クラス
    public sealed class SecondPlayerInstancePos
    {
        public Vector3 twoPos{get; private set;}

        // コンストラクタ
        public SecondPlayerInstancePos()
        {
            // 初期化
            twoPos = ObjectManager.Player.DataPlayer.SecondPlayerCreatePos;
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


    // プレイヤー右回転座標クラス
    public sealed class PlayerRotateRightPos
    {
        public Vector3 RightRotate{get; private set;}

        // コンストラクタ
        public PlayerRotateRightPos()
        {
            // 初期化
            RightRotate = ObjectManager.Player.DataPlayer.RotateRightPos;
        }
    }

    // プレイヤー左回転座標クラス
    public sealed class PlayerRotateLeftPos
    {
        public Vector3 LeftRotate{get; private set;}

        // コンストラクタ
        public PlayerRotateLeftPos()
        {
            // 初期化
            LeftRotate = ObjectManager.Player.DataPlayer.RotateLeftPos;
        }
    }

    // プレイヤー後ろ回転座標クラス
    public sealed class PlayerRotateBackPos
    {
        public Vector3 BackRotate{get; private set;}

        // コンストラクタ
        public PlayerRotateBackPos()
        {
            // 初期化
            BackRotate = ObjectManager.Player.DataPlayer.RotateBackPos;
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

