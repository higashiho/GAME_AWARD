using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{

    
    [CreateAssetMenu(fileName = "TitlePlayerDataList", menuName = "ScriptableObjects/TitlePlayerParamAsset")]
    public class TitlePlayerDataList : ScriptableObject
    {
        public List<TitlePlayerData> PlayerDatas = new List<TitlePlayerData>(2);
    }
}