using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerParamAsset")]
    public class DataPlayer : ScriptableObject
    {
        [SerializeField, Header("1P生成座標")]
        private Vector3 firstPlayerCreatePos;
        public Vector3 FirstPlayerCreatePos{get{return firstPlayerCreatePos;}}

        [SerializeField, Header("2P生成座標")]
        private Vector3 secondPlayerCreatePos;
        public Vector3 SecondPlayerCreatePos{get{return secondPlayerCreatePos;}}

        [SerializeField, Header("プレイヤー移動速度")]
        private float moveSpeed;
        public float MoveSpeed{get{return moveSpeed;}}

        [SerializeField, Header("プレイヤーの右回転座標")]
        private Vector3 rotateRightPos;
        public Vector3 RotateRightPos{get{return rotateRightPos;}}

        [SerializeField, Header("プレイヤーの左回転座標")]
        private Vector3 rotateLeftPos;
        public Vector3 RotateLeftPos{get{return rotateLeftPos;}}

        [SerializeField, Header("プレイヤーの後回転座標")]
        private Vector3 rotateBackPos;
        public Vector3 RotateBackPos{get{return rotateBackPos;}}

        [SerializeField, Header("プレイヤーから出るRayの最大の長さ")]
        private float rayDirection;
        public float RayDirection{get{return rayDirection;}}
    }
}
