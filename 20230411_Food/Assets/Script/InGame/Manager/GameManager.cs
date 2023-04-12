using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoodPoint;

namespace GameManager
    {
    public class GameManager : MonoBehaviour
    {

        // ステート
        private enum gameState
        {
            OPENING,    // 開幕演出
            COUNTDOWM,  // ゲーム開始カウントダウン
            GAME,       // ゲーム
            COOKING,    // 集めた食材で料理
            END         // リザルトへ
        }

        // ゲームステート管理変数
        private gameState phase;

        private PointManager pointManager;
        
        void Update()
        {
            switch(phase)
            {
                case gameState.OPENING:
                    break;
                
                case gameState.COUNTDOWM:
                    break;

                case gameState.GAME:
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
}

