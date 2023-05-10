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
    public sealed class PlayerInstancePos
    {
        public Vector3 MainPos{get; private set;}

        // コンストラクタ
        public PlayerInstancePos(Vector3 tmpPos)
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

    public sealed class PlayerRotateForwardPos
    {
        public Vector3 Amount{get; private set;}

        // コンストラクタ
        public PlayerRotateForwardPos(Vector3 tmpPos)
        {
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

    // プレイヤーから出るカプセル型のレイの距離
    public sealed class PlayerBoxRayDistance
    {
        public float Amount{get; private set;}

        // コンストラクタ
        public PlayerBoxRayDistance(float tmpDirection)
        {
            Amount = tmpDirection;
        }
    }

    // プレイヤーから出る、矩形のレイの半径
    public sealed class PlayerBoxRayHalfExtents
    {
        public Vector3 Amount{get; private set;}

        // コンストラクタ
        public PlayerBoxRayHalfExtents(Vector3 tmpPos)
        {
            Amount = tmpPos;
        }
    }

    // ステージの大きさクラス
    public sealed class Stage
    {
        public Vector3[] Scale{get; private set;}

        public Stage(Vector3[] tmpScale)
        {
            Scale = tmpScale;
        }
    }

    // 机オブジェクトの外枠の座標
    public sealed class OutSide
    {
        public Vector3 Pos{get; private set;}

        public OutSide(Vector3 tmpPos)
        {
            Pos = tmpPos;
        }
    }

    public sealed class PlayerNumber
    {
        public int Index{get; private set;}

        public PlayerNumber(int tmpNum)
        {
            Index = tmpNum;
        }
    }
}

