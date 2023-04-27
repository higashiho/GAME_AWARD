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

    public sealed class PlayerRotateForwardPos
    {
        public Vector3 Amount{get; private set;}

        // コンストラクタ
        public PlayerRotateForwardPos(Vector3 tmpPos)
        {
            Amount = tmpPos;
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

    // プレイヤーから出るカプセル型のレイの距離
    public sealed class PlayerCapsuleRayDistance
    {
        public float Amount{get; private set;}

        // コンストラクタ
        public PlayerCapsuleRayDistance(float tmpDirection)
        {
            Amount = tmpDirection;
        }
    }

    // 
    public sealed class PlayerCapsuleRay
    {
        public bool Cast{get; private set;}

        // コンストラクタ
        public PlayerCapsuleRay(bool tmpCount)
        {
            Cast = tmpCount;
        }
    }

    // プレイヤーから出る、カプセル型のレイの端の球の中心の座標
    public sealed class PlayerCapsuleRayStartPos
    {
        public Vector3 Amount{get; private set;}

        // コンストラクタ
        public PlayerCapsuleRayStartPos(Vector3 tmpPos)
        {
            Amount = tmpPos;
        }
    }

    // プレイヤーから出る、カプセル型のレイの端の球の中心の座標
    public sealed class PlayerCapsuleRayEndPos
    {
        public Vector3 Amount{get; private set;}

        // コンストラクタ
        public PlayerCapsuleRayEndPos(Vector3 tmpPos)
        {
            Amount = tmpPos;
        }
    }

    // プレイヤーから出る、カプセル型のレイの半径・太さ
    public sealed class PlayerCapsuleRayRadiuse
    {
        public float Amount{get; private set;}

        // コンストラクタ
        public PlayerCapsuleRayRadiuse(float tmpValue)
        {
            Amount = tmpValue;
        }
    }

    public sealed class RayCastHit
    {
        public RaycastHit hit{get; private set;}

        // コンストラクタ
        public RayCastHit(RaycastHit tmpHit)
        {
            hit = tmpHit;
        }
    }

    public sealed class PlayerOverlapCapsule
    {
        public Collider[] Amount{get; private set;}

        public PlayerOverlapCapsule(Collider[] tmpAmount)
        {
            Amount = tmpAmount;
        }
    }
}

