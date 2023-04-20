using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    [CreateAssetMenu(fileName = "TitlePlayerData", menuName = "SriptableObjects/TitlePlayerData")]
    public class TitlePlayerData : ScriptableObject
    {
        [SerializeField, Header("Player生成座標")]
        private Vector3 instancePos;
        public Vector3 InstancePos{get{return instancePos;}}

        [SerializeField, Header("移動スピード")]
        private float moveSpeed;
        public float MoveSpeed{get{return moveSpeed;}}

        [SerializeField, Header("Rayの長さ")]
        private float rayDistance;
        public float RayDistance{get{return rayDistance;}}
    }

}