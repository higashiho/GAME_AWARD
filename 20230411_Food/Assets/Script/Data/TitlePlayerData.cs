using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    [System.Serializable]
    public class TitlePlayerData
    {
        // PlayerのID
        [Tooltip("PlayerのID")]
        public int Id;
        [Tooltip("Playerのアドレスキー")]
        public string InstanceAddress;
        [Tooltip("移動方向キー")]
        public KeyCode[] MoveKey = new KeyCode[4];
        [Tooltip("決定キー")]
        public KeyCode DeterminationKey;
        // Player生成座標
        [Tooltip("Player生成座標")]
        public Vector3 InstancePos;
        // 移動スピード
        [Tooltip("移動スピード")]
        public float MoveSpeed;
        // Rayの長さ
        [Tooltip("Rayの長さ")]
        public float RayDistance;
    }
}