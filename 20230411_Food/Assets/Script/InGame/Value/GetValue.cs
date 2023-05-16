using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ポイント取得クラス
public class GetValue : MonoBehaviour
{
    [Header("データ"), SerializeField]
    private IngredientData data;
    
    // ポイント
    [SerializeField]
    public int Point;//{get; private set;}

    public void SetPoint(int value)
    {
        this.Point = value;
        Debug.Log(Point);
    }

    // 量
    public int Amount{get; private set;}

    // ポイントタイプ
    public string Type{get; private set;}
    
    void Awake()
    {
        
        Amount = data.Amount;
        Type = data.Type;
        // 要修正
        if(Type == "SEASOUSING")
        {
            this.gameObject.GetComponent<SetSeasPoint>().SetPoint();
            return;
        }
        Point = data.Point;
    }

    

}
