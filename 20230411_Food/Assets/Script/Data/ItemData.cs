using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Item
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
    public class ItemData : ScriptableObject   
    {
        [Header("アイテムがステージに出現する数"), SerializeField]
        private int itemPopNum;
        public int ItemPopNum{get{return itemPopNum;}}

        [Header("アイテムのリポップインターバル(秒)"), SerializeField]
        private int ripopInterval;

        // ミリ秒で使用するため1000倍する
        public int RipopInterval{get => ripopInterval * 1000;}

        
    }
}

