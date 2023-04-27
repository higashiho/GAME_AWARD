using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// トマトクラス
/// </summary>
public class Tomato : MonoBehaviour
{
    [Header("トマトのデータ"), SerializeField]
    private IngredientData data;

    public int Point{get; private set;}
    public string Type{get; private set;}

    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }

}
