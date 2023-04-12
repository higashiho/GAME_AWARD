using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "SriptableObjects/PlayerParamAsset")]
    public class DataPlayer : ScriptableObject
    {
        [SerializeField, Header("1P生成座標")]
        private Vector3 firstPlayerCreatePos;
        public Vector3 FirstPlayerCreatePos{get{return firstPlayerCreatePos;}}

        [SerializeField, Header("プレイヤー移動速度")]
        private float moveSpeed;
        public float MoveSpeed{get{return moveSpeed;}}

        [SerializeField, Header("プレイヤーオブジェクト")]
        private GameObject playerPrefab;
        public GameObject PlayerPrefab{get{return playerPrefab;}}
    }
}
