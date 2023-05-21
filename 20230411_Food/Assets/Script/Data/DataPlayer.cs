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

        [SerializeField, Header("プレイヤーから出るRayの最大の長さ")]
        private float rayDistance;
        public float RayDistance{get{return rayDistance;}}

        [SerializeField, Header("レイの半径")]
        private Vector3 rayRadiuse;
        public Vector3 RayRadiuse{get{return rayRadiuse;}}


        [SerializeField, Header("効果音まとめ")]
        private AudioClip[] actionSE;
        public AudioClip[] ActionSE{get{return actionSE;}}

        [SerializeField, Header("音源を流すもの")]
        private AudioSource playerAudioSource;
        public AudioSource PlayerAudioSource{get{return playerAudioSource;}}

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
