using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx.Triggers;
using UniRx;
using DG.Tweening;


namespace Title
{
    /// <summary>
    /// タイトル管理クラス
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        [SerializeField, Header("Main Camera")]
        private GameObject mainCamera;
        public GameObject MainCamera{get{return mainCamera;}}
       
        /// <summary>
        /// 入力イベントインスタンス化
        /// </summary>
        public TitleInputEvent InputEvent{get; private set;}

        /// <summary>
        /// イベント管理クラスインスタンス化
        /// </summary>
        private EventManager eventSetting;

        /// <summary>
        /// シーン挙動クラスインスタンス化
        /// </summary>
        /// <value></value>
        public SceneMove Move{get; private set;}
        
        void Awake()
        {
            // インスタンス化
            InputEvent  = new TitleInputEvent(this.gameObject);
            ObjectManager.Player = new PlayerManager();
            eventSetting = new EventManager();
            Move = new SceneMove();
            ObjectManager.TitleScene = this;

            // タイトル入力確認ループ
            this.UpdateAsObservable()
                .Subscribe(_ => InputEvent.Update())
                .AddTo(this);

            // 入力イベント設定
            eventSetting.SetInputEvents();
        }

        private void OnDestroy() 
        {
            DOTween.KillAll();
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

        // タイトルマネージャー
        private static TitleManager titleScene;
        public static TitleManager TitleScene{
            get{return titleScene;} 
            set{titleScene = value; Debug.LogWarning("Assigned to titleScene.");} 
            }
    }

    /// <summary>
    /// シーン挙動クラス
    /// </summary>
    public sealed class SceneMove
    {
        private SceneMoveTime moveTime = new SceneMoveTime(4);
        /// <summary>
        /// シーン挙動
        /// </summary>
        public void Movements()
        {
            //Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            //Appendで動作を追加
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DOMove(
                ObjectManager.Player.HitObject.transform.position,
                moveTime.MoveTimeAmount
            ).SetEase(Ease.Linear)
            .OnComplete(() => SceneManager.LoadScene("InGameScene")));

            //Appendで動作を追加
            sequence.Join(ObjectManager.TitleScene.MainCamera.transform.DORotate(
                Vector3.forward * 360,
                // 移動終了までに４回転させるために回転させる数で割る
                moveTime.MoveTimeAmount / 4,
                RotateMode.LocalAxisAdd
            ).SetEase(Ease.Linear)
            .SetLoops(-1,LoopType.Restart));

            //Playで実行
            sequence.Play();
        }

    }

    /// <summary>
    /// シーン挙動までにかかる時間設定クラス
    /// </summary>
    public sealed class SceneMoveTime
    {
        /// <summary>
        /// シーン挙動までにかかる時間
        /// </summary>
        public float MoveTimeAmount{get; private set;}
        public SceneMoveTime(float tmpAmount)
        {
            MoveTimeAmount = tmpAmount;
        }
    }
}

