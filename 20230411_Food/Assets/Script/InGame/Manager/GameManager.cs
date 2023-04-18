using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;
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

        private ObjectManager objectManager;

        void Start()
        {
            objectManager = new ObjectManager();
            ObjectManager.Player = new PlayerManager();
        }
        
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

