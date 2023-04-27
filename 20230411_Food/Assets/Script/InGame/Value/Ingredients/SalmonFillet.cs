using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サーモンの切り身クラス
/// </summary>
public class SalmonFillet : MonoBehaviour
{
    [Header("サーモンフィレットのデータ"), SerializeField]
    private IngredientData data;

    public int Point{get; private set;}
    public string Type{get; private set;}

    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }
}
