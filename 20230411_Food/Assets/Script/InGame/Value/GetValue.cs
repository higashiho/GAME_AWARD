using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetValue : MonoBehaviour
{
    [Header("データ"), SerializeField]
    private IngredientData data;
    
    // ポイント
    public int Point{get; private set;}

    public void SetPoint(int value)
    {
        this.Point = value;
    }

    // 量
    public int Amount{get; private set;}

    // ポイントタイプ
    public string Type{get; private set;}
    
    void Start()
    {
        Point = data.Point;
        Amount = data.Amount;
        Type = data.Type;
    }

}
