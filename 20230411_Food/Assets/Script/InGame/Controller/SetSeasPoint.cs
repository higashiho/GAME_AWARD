using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSeasPoint : MonoBehaviour
{
    void OnEnable()
    {
        var rand = UnityEngine.Random.Range(1,4);
        this.gameObject.GetComponent<GetValue>().SetPoint(rand * 3);
    }

    

    

    
}
