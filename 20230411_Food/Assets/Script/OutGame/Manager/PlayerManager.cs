using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using ObjectInterface;

namespace Title
{
    public class PlayerManager : IActor
    {
        private PlayerInputEvent inputEvent;

        public GameObject Object{get; private set;}
        public PlayerManager()
        {
            inputEvent = new PlayerInputEvent(Object);

            // 初期化
            Initialization();

            // 更新
            Update();
       }

        /// <summary>
        /// 初期化関数
        /// </summary>
        public void Initialization()
        {

        }

        /// <summary>
        /// 更新関数
        /// </summary>
        public void Update()
        {

        }
    }

    

    /// <summary>
    /// タイトル入力イベント管理クラス
    /// </summary>
    public sealed class PlayerInputEvent
    {
        // 以下入力イベント==================================================
        public IReadOnlyReactiveProperty<bool> LeftMove => leftMove;
        public IReadOnlyReactiveProperty<bool> RightMove => rightMove;
        public IReadOnlyReactiveProperty<bool> ForwartMove => forwartMove;
        public IReadOnlyReactiveProperty<bool> BakeMove => bakeMove;

        // =================================================================


        
        // 以下入力イベント実装===============================================
        private readonly ReactiveProperty<bool> leftMove = new BoolReactiveProperty();
        private readonly ReactiveProperty<bool> rightMove = new BoolReactiveProperty();
        private readonly ReactiveProperty<bool> forwartMove = new BoolReactiveProperty();
        private readonly ReactiveProperty<bool> bakeMove = new BoolReactiveProperty();
        // =================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpObj">イベント対象オブジェクト</param>
        public PlayerInputEvent(GameObject tmpObj)
        {
            // 以下指定オブジェクトDestroy時にイベント破棄設定=========
            leftMove.AddTo(tmpObj);
            rightMove.AddTo(tmpObj);
            forwartMove.AddTo(tmpObj);
            bakeMove.AddTo(tmpObj);
            // =====================================================
        }

        /// <summary>
        /// 入力更新関数
        /// </summary>
        public void Update()
        {
            // 以下各種入力をReactivePropertyに反映=========================
            leftMove.Value = Input.GetKeyDown(KeyCode.A);
            rightMove.Value = Input.GetKeyDown(KeyCode.D);
            forwartMove.Value = Input.GetKeyDown(KeyCode.W);

            // ===========================================================
        }
    }
}

