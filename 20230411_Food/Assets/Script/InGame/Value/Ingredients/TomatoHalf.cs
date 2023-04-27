using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 半分トマトのクラス
/// </summary>
public class TomatoHalf : MonoBehaviour
{
    [Header("トマト半分のデータ"), SerializeField]
    private IngredientData data;

    public int Point{get; private set;}
    public string Type{get; private set;}


    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }
}
