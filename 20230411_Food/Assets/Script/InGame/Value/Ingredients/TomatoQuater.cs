using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1/4トマトのデータ
/// </summary>
public class TomatoQuater : MonoBehaviour
{
    [Header("トマト1/4のデータ"), SerializeField]
    private IngredientData data;

    public int Point{get; private set;}
    public string Type{get; private set;}

    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }
}
