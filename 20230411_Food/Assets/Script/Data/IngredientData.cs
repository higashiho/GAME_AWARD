using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;

[CreateAssetMenu(fileName = "IngredientData", menuName = "ScriptableObjects/IngredientParamAsset")]
public class IngredientData : ScriptableObject
{
    [Header("ポイントの量"), SerializeField]
    private int point;
    public int Point{get => point;}

    [Header("量ポイントの量"), SerializeField]
    private int amount;
    public int Amount{get => amount;}

    [Header("ポイントの種類"), SerializeField]
    private string type;
    public string Type{get => type;}
}


