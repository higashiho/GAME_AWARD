using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class GetData 
{
    // ハンドル
    private AsyncOperationHandle handle = new AsyncOperationHandle();

    private AsyncOperationHandle<TextAsset> csvDataHandle = new AsyncOperationHandle<TextAsset>();


    // 読み込んだデータをいれとくリスト
    //private List<string[]> pointDataArr = new List<string[]>();

    public GetData()
    {
        
    }

    /// <summary>
    /// データハンドルをもとにアセットをロード
    /// </summary>
    /// <param name="dataName">ロードするデータの名前</param>
    /// <returns></returns>
    public async UniTask LoadAsset(string dataName, TextAsset data)
    {
        csvDataHandle = Addressables.LoadAssetAsync<TextAsset>(dataName);
        data = await csvDataHandle.Task;
    }

    /// <summary>
    /// CSVのデータを読むメソッド
    /// </summary>
    /// <param name="data"></param>
    public void ReadData(TextAsset data, List<string[]> dataArr)
    {
        // StringReaderインスタンス化
        StringReader reader = new StringReader(data.text);

        // , で分割して１行ずつ読み込み
        // リストに追加していく
        int maxX = -1;
        int maxY = -1;
        string line = "";

        while(reader.Peek() != -1)
        {
            // 一行ずつ読み込み
            line = reader.ReadLine();
            dataArr.Add(line.Split(','));
            maxY++;
        }
        maxX = CountChar(line, ',');
        
    }

    /// <summary>
    /// 文字数を数えるメソッド
    /// １行分のstringから","を引いた数を返す
    /// </summary>
    /// <param name="s">１行分のstring</param>
    /// <param name="c">,</param>
    /// <returns>１行分のstringから","を引いた数</returns>
    public int CountChar(string s, char c)
    {
        return s.Length - s.Replace(c.ToString(), "").Length;
    }




}
