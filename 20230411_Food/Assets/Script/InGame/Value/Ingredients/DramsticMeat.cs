using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 骨付き肉のクラス
/// </summary>
public class DramsticMeat : MonoBehaviour
{
    [Header("骨付き肉のデータ"), SerializeField]
    private IngredientData data;

    public int Point{get; private set;}
    public string Type{get; private set;}

    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }
}
