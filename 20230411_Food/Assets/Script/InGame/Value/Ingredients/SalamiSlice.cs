using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スライスサラミのクラス
/// </summary>
public class SalamiSlice : MonoBehaviour
{
    [Header("スライスサラミのデータ"), SerializeField]
    private IngredientData data;

    public int Point{get; private set;}
    public string Type{get; private set;}

   
    public void Awake()
    {
        Point = data.PointAmount;
        Type = data.PointType;
    }
}
