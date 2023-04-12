using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManager;

namespace player
{
    public class ValueObjectPlayer
    {
        
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
    public sealed class PlayerMoveSpeed
    {
        public float moveSpeed{get; private set;}

        // コンストラクタ
        public PlayerMoveSpeed()
        {
            if(moveSpeed <= 0)
            {
                Debug.LogError("プレイヤーの移動速度がマイナスだよ");
            }
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
            if(Rotate == null)
            {
                Debug.LogError("プレイヤーの回転座標がNULLだよ");
            }

            Rotate = ObjectManager.Player.DataPlayer.RotatePos;
        }
    }
}

