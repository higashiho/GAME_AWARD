using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace player
{
    public class ValueObjectPlayer
    {
        
    }

    // プレイヤーオブジェクトそのもののクラス
    public class PlayerObject
    {
        public GameObject identity{get; private set;}

        // コンストラクタ
        public PlayerObject(GameObject player)
        {
            // 初期化
            identity = player;
        }
    }

    // プレイヤー生成座標クラス
    public class PlayerInstancePos
    {
        public Vector3 pos{get; private set;}

        // コンストラクタ
        public PlayerInstancePos(Vector3 playerPos)
        {
            // 初期化
            pos = playerPos;
        }

        // 座標を取得
        public Vector3 GetInitPos()
        {
            return this.pos;
        }
    }

    // プレイヤーの速度クラス
    public class MoveSpeed
    {
        private float moveSpeed;

        // コンストラクタ
        public MoveSpeed(float speed)
        {
            // 初期化
            moveSpeed = speed;
        }
    }
}

