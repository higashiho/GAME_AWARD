using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManager;

namespace Player
{
    public class PlayerValue
    {
        
    }

    // 1Pプレイヤー生成座標クラス
    public sealed class FirstPlayerInstancePos
    {
        public Vector3 MainPos{get; private set;}

        // コンストラクタ
        public FirstPlayerInstancePos(Vector3 tmpPos)
        {
            // 初期化
            MainPos = tmpPos;
        }
    }

    // 2Pプレイヤー生成座標クラス
    public sealed class SecondPlayerInstancePos
    {
        public Vector3 SubPos{get; private set;}

        // コンストラクタ
        public SecondPlayerInstancePos(Vector3 tmpPos)
        {
            // 初期化
            SubPos = tmpPos;
        }
    }

    // プレイヤーの速度クラス
    public sealed class PlayerMoveSpeed
    {
        public float Amount{get; private set;}

        // コンストラクタ
        public PlayerMoveSpeed(float tmpSpeed)
        {
            // 初期化
            Amount = tmpSpeed;
        }
    }


    // プレイヤー右回転座標クラス
    public sealed class PlayerRotateRightPos
    {
        public Vector3 Amount{get; private set;}

        // コンストラクタ
        public PlayerRotateRightPos(Vector3 tmpPos)
        {
            // 初期化
            Amount = tmpPos;
        }
    }

    // プレイヤー左回転座標クラス
    public sealed class PlayerRotateLeftPos
    {
        public Vector3 Amount{get; private set;}

        // コンストラクタ
        public PlayerRotateLeftPos(Vector3 tmpPos)
        {
            // 初期化
            Amount = tmpPos;
        }
    }

    // プレイヤー後ろ回転座標クラス
    public sealed class PlayerRotateBackPos
    {
        public Vector3 Amount{get; private set;}

        // コンストラクタ
        public PlayerRotateBackPos(Vector3 tmpPos)
        {
            // 初期化
            Amount = tmpPos;
        }
    }

    // プレイヤーから出るRayの長さの最大値のクラス
    public sealed class PlayerRayDirection
    {
        public float Amount{get; private set;}

        // コンストラクタ
        public PlayerRayDirection(float tmpDirection)
        {
            Amount = tmpDirection;
        }
    }
}

