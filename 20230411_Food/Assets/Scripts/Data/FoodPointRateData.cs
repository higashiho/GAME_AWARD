using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Result
{
    [CreateAssetMenu(fileName = "FoodPointRateData", menuName = "ScriptableObjects/FoodPointRateParamAsset")]
    public class FoodPointRateData : ScriptableObject
    {
        [SerializeField, Header("ジャッジ時用ポイント割合")]
        private List<int> rate = new List<int>(3);
        public List<int> Rate{ get => rate;}
        
    }
}

