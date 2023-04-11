using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;


namespace Title
{
    /// <summary>
    /// タイトル管理クラス
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary>
        /// 入力イベントインスタンス
        /// </summary>
        private TitleInputEvent inputEvent;

        // Scene転移が可能かどうか
        private bool OnSceneMoveFlag = true;
        
        void Awake()
        {
            // インスタンス化
            inputEvent  = new TitleInputEvent(this.gameObject);
            ObjectManager.Player = new PlayerManager();

            // タイトル入力確認ループ
            this.UpdateAsObservable()
                .Subscribe(_ => inputEvent.Update())
                .AddTo(this);

            // ゲームスタートイベントループ
            inputEvent.GameStart
                // イベント指定した入力がされているか
                .Where(x => x)
                // シーン転移が可能か
                .Where(_ => OnSceneMoveFlag)
                // 実施
                .Subscribe(_ =>
                {
                    // フラグを折る
                    OnSceneMoveFlag = false;
                    // シーン転移
                    Debug.Log("SceneMove");
                });
        }

    }

    /// <summary>
    /// タイトル入力イベント管理クラス
    /// </summary>
    public sealed class TitleInputEvent
    {
        // 以下入力イベント==================================================
        public IReadOnlyReactiveProperty<bool> GameStart => gameStart;

        // =================================================================


        
        // 以下入力イベント実装===============================================
        private readonly ReactiveProperty<bool> gameStart = new BoolReactiveProperty();
        // =================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpObj">イベント対象オブジェクト</param>
        public TitleInputEvent(GameObject tmpObj)
        {
            // 以下指定オブジェクトDestroy時にイベント破棄設定=========
            gameStart.AddTo(tmpObj);
            // =====================================================
        }

        /// <summary>
        /// 入力更新関数
        /// </summary>
        public void Update()
        {
            // 以下各種入力をReactivePropertyに反映=========================
            gameStart.Value = Input.GetKeyDown(KeyCode.Return);

            // ===========================================================
        }
    }

    public abstract class ObjectManager
    {
        // プレイヤー
        private static PlayerManager player;
        public static PlayerManager Player{
            get{return player;} 
            set{player = value; Debug.LogWarning("Assigned to player.");}
        }
    }
}

