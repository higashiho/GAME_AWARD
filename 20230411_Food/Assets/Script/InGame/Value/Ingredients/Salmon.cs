using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サーモンのクラス
/// </summary>
public class Salmon : MonoBehaviour
{
    [Header("サーモンのデータ"), SerializeField]
    private IngredientData data;
    public int Point{get; private set;}
    public string Type{get; private set;}

    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }
}
