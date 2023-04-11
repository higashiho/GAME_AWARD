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
        public static TitleInputEvent InputEvent{get; private set;}

        private EventManager eventSetting;

        
        void Awake()
        {
            // インスタンス化
            InputEvent  = new TitleInputEvent(this.gameObject);
            ObjectManager.Player = new PlayerManager();
            eventSetting = new EventManager();

            // タイトル入力確認ループ
            this.UpdateAsObservable()
                .Subscribe(_ => InputEvent.Update())
                .AddTo(this);

            // 入力イベント設定
            eventSetting.SetInputEvents();
        }

    }
    /// <summary>
    /// タイトル入力イベント管理クラス
    /// </summary>
    public sealed class TitleInputEvent
    {
        // 以下入力イベント==================================================
        public IReadOnlyReactiveProperty<bool> GameStart => keyReturnInput;
        public IReadOnlyReactiveProperty<bool> FoodNicknames => keyReturnInput;
        public IReadOnlyReactiveProperty<bool> DisplayIngredientsList => keyReturnInput;

        // =================================================================


        
        // 以下入力イベント実装===============================================
        private readonly ReactiveProperty<bool> keyReturnInput = new BoolReactiveProperty();
        // =================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpObj">イベント対象オブジェクト</param>
        public TitleInputEvent(GameObject tmpObj)
        {
            // 以下指定オブジェクトDestroy時にイベント破棄設定=========
            keyReturnInput.AddTo(tmpObj);
            // =====================================================
        }

        /// <summary>
        /// 入力更新関数
        /// </summary>
        public void Update()
        {
            // 以下各種入力をReactivePropertyに反映=========================
            keyReturnInput.Value = Input.GetKeyDown(KeyCode.Return);
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

