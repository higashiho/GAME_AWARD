using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    /// <summary>
    /// アイテムの生成を担当するクラス
    /// </summary>
    public class ItemFactory 
    {
        // プールQueue
        private Queue<GameObject> pool = new Queue<GameObject>(8);

        // アイテムの生成座標を保管する配列
        // -4.5, -1.5, 1.5, 4.5
        private Vector3[,] itemPopPos;

        // アイテムを生成する間隔
        private float spaceX = 3.0f;
        private float spaceZ = 9.0f;

        // アイテムを生成する基準ライン
        private float baseLineX = -7.5f;
        private float baseLineZ = -22.5f;
        
        
        /// <summary>
        /// ItemFactoryのコンストラクタ
        /// </summary>
        public ItemFactory()
        {
            // 生成座標配列作成
            makePopPosArr();
        }


        /// <summary>
        /// アイテムを生成するメソッド
        /// </summary>
        public void Create(Vector3 pos)
        {
            // プールからDequeue
            GameObject obj = pool.Dequeue();
            
            // 生成座標設定
            obj.transform.position = pos;

            // アクティブ化
            obj.SetActive(true);
        }


        /// <summary>
        /// アイテムをプーリングするメソッド
        /// </summary>
        public void Storing(GameObject obj)
        {
            // プールのQueueに入れる
            pool.Enqueue(obj);
        }


        /// <summary>
        /// アイテムを生成する座標を作る
        /// </summary>
        private void makePopPosArr()
        {
            // アイテムの生成座標配列インスタンス化
            itemPopPos = new Vector3[5,5];
            
            // 座標を入れていく
            for(int i = 1; i < itemPopPos.GetLength(0); i++)
            {
                for(int j = 1; j < itemPopPos.GetLength(1); j++)
                {
                    itemPopPos[i,j] = new Vector3(baseLineX + i * spaceX, 0f, baseLineZ * j * spaceZ);
                }
            }
        }

    }
}

