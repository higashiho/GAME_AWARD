using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;

using Item;
using Player;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {

        // ステート
        [SerializeField]
        private enum gameState
        {
            OPENING,    // 開幕演出
            COUNTDOWM,  // ゲーム開始カウントダウン
            GAME,       // ゲーム
            COOKING,    // 集めた食材で料理
            END         // リザルトへ
        }

        // ゲームステート管理変数
        [SerializeField]
        private gameState phase;

        private PointManager pointManager;
        private ItemManager itemManager;

        private GetData getData;
        private DishData dishData;
        

        async void Start()
        {
            
            getData = new GetData();
            dishData = new DishData(ref getData);
            await dishData.LoadData();

            dishData.GetDishData(getData);

            pointManager = new PointManager(ref dishData, 1);
            
            
            objectManager = new ObjectManager();
            ObjectManager.Player = new PlayerManager();

            itemManager = new ItemManager(ObjectManager.Player);
        }


        private ObjectManager objectManager;

       
        
        void Update()
        {
            switch(phase)
            {
                case gameState.OPENING:
                    break;
                
                case gameState.COUNTDOWM:
                    break;

                case gameState.GAME:
                    
                    objectManager.Update();
                    itemManager.Update();
                        
                    
                    break;
                
                case gameState.COOKING:

                    // プレイヤーが集めたポイント達を配列に入れていく
                    
                    break;
                
                case gameState.END:

                    // リザルトシーンへ
                    break;

            }
        }
    }

    public class ObjectManager
    {
        // プレイヤー
        private static PlayerManager player;

        public static PlayerManager Player
        {
            get{return player;}
            set{player = value;}
        }

        // インゲーム全体統括メソッド
        public void Update()
        {
            Player.Update();
        }
    }
}

