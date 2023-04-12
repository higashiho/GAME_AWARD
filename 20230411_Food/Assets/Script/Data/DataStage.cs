using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stage
{
    [CreateAssetMenu(fileName = "StageData", menuName = "SriptableObjects/StageParamAsset")]
    public class DataStage : ScriptableObject
    {
        [SerializeField, Header("ステージの横の長さ")]
        private float stageWidth;
        public float StageWidth{get;}

        [SerializeField, Header("ステージの縦の長さ")]
        private float stageHeight;
        public float StageHeight{get;}
    }
}

