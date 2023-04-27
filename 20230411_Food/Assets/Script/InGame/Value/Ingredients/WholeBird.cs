using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鳥一羽のクラス
/// </summary>
public class WholeBird : MonoBehaviour
{
    [Header("鳥一羽のデータ"), SerializeField]
    private IngredientData data;

    public int Point{get; private set;}
    public string Type{get; private set;}

    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }
}
