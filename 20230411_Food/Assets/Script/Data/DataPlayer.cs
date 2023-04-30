using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerParamAsset")]
    public class DataPlayer : ScriptableObject
    {
        [SerializeField, Header("生成するプレイヤーのアドレスキー")]
        private string adressKey;
        public string AdressKey{get{return adressKey;}}

        [SerializeField, Header("生成座標")]
        private Vector3 playerCreatePos;
        public Vector3 PlayerCreatePos{get{return playerCreatePos;}}

        [SerializeField, Header("プレイヤー移動速度")]
        private float moveSpeed;
        public float MoveSpeed{get{return moveSpeed;}}

        [SerializeField, Header("操作キー")]
        private KeyCode[] controlleKey = new KeyCode[4];
        public KeyCode[] ControlleKey{get{return controlleKey;}}

        [SerializeField, Header("プレイヤーの右回転座標")]
        private Vector3 rotateRightPos;
        public Vector3 RotateRightPos{get{return rotateRightPos;}}

        [SerializeField, Header("プレイヤーの左回転座標")]
        private Vector3 rotateLeftPos;
        public Vector3 RotateLeftPos{get{return rotateLeftPos;}}

        [SerializeField, Header("プレイヤーの前向き座標")]
        private Vector3 rotateForwardPos;
        public Vector3 RotateForWardPos{get{return rotateForwardPos;}}

        [SerializeField, Header("プレイヤーの後回転座標")]
        private Vector3 rotateBackPos;
        public Vector3 RotateBackPos{get{return rotateBackPos;}}

        [SerializeField, Header("プレイヤーから出るRayの最大の長さ")]
        private float rayDirection;
        public float RayDirection{get{return rayDirection;}}

        [SerializeField, Header("レイの半径")]
        private float rayRadiuse;
        public float RayRadiuse{get{return rayRadiuse;}}

        [SerializeField, Header("何人目か")]
        private int number;
        public int Number{get{
            var tmpNum =  number - 1;
            if(tmpNum < 0)
            {
                Debug.LogError("Num Error");
                return 0;
            }
            else
                return tmpNum;
            }}
    }
}
