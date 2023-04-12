using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace player
{
    public class ValueObjectPlayer
    {
        
    }

    // プレイヤーオブジェクトそのもののクラス
    public sealed class PlayerObject
    {
        public GameObject identity{get; private set;}

        // コンストラクタ
        public PlayerObject(GameObject player)
        {
            if(identity == null)
            {
                Debug.LogError("プレイヤーがNULLだよ");
            }
            // 初期化
            identity = player;
        }
    }

    // プレイヤー生成座標クラス
    public sealed class PlayerInstancePos
    {
        public Vector3 pos{get; private set;}

        // コンストラクタ
        public PlayerInstancePos(Vector3 playerPos)
        {
            if(pos == null)
            {
                Debug.LogError("プレイヤーの初期位置がNULLだよ");
            }
            // 初期化
            pos = playerPos;
        }
    }

    // プレイヤーの速度クラス
    public sealed class MoveSpeed
    {
        public float moveSpeed{get; private set;}

        // コンストラクタ
        public MoveSpeed(float speed)
        {
            if(moveSpeed <= 0)
            {
                Debug.LogError("プレイヤーの移動速度がマイナスだよ");
            }
            // 初期化
            moveSpeed = speed;
        }
    }

    // プレイヤーの前に食べ物があるか判定するためのRayの座標
    public sealed class PlayerRay : BasePlayer
    {
        public Vector3 RayStartPos{get; private set;}

        // コンストラクタ
        public PlayerRay(Vector3 ray)
        {
            if(RayStartPos == null)
            {
                Debug.LogError("Rayの座標がnullです");
            }

            RayStartPos = ray;
        }

        public PlayerRay Move()
        {
            return new PlayerRay(PlayerObject.transform.position);
        }
    }
}

