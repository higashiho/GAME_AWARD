using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject   
{
    [Header("アイテムがステージに出現する数"), SerializeField]
    private int itemPopNum;
    public int ItemPopNum{get{return itemPopNum;}}

    
}
