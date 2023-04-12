using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    [CreateAssetMenu(fileName = "TitleCameraData", menuName = "SriptableObjects/TitleCameraData")]
    public class TitleCameraData : ScriptableObject
    {
        [SerializeField, Header("初期座標")]
        private Vector3 startPos;
        public Vector3 StartPos{get{return startPos;}}

        [SerializeField, Header("初期アングル")]
        private Vector3 startAngle;
        public Vector3 StartAngle{get{return startAngle;}}
    }
}

