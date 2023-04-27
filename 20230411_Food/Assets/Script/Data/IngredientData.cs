using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;

[CreateAssetMenu(fileName = "IngredientData", menuName = "ScriptableObjects/IngredientParamAsset")]
public class IngredientData : ScriptableObject
{
    [Header("ポイントの量"), SerializeField]
    private int pointAmount;
    public int PointAmount{get;}

    [Header("ポイントの種類"), SerializeField]
    private string pointType;
    public string PointType{get;}
}


