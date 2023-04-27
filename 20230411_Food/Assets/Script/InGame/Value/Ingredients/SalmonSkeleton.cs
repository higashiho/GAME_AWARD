using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サーモンの骨クラス
/// </summary>
public class SalmonSkeleton : MonoBehaviour
{
    [Header("サーモンの骨のデータ"), SerializeField]
    private IngredientData data;

    public int Point{get; private set;}
    public string Type{get; private set;}

    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }
}
