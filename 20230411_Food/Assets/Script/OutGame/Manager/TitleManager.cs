using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx.Triggers;
using UniRx;
using DG.Tweening;
using Cysharp.Threading.Tasks;


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

        [SerializeField, Header("Fade Panel")]
        private Image fadePanel;
        public Image FadePanel{get{return fadePanel;}}

        [SerializeField, Header("Title Camera Data")]
        private TitleCameraData cameraData;
        public TitleCameraData CameraData{get{return cameraData;}}
       
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
        public MakeTweenMovengs Move{get; private set;}

        /// <summary> 何かのイベントが処理されているか </summary>
        public bool NowPlayeEvents{get{return nowPlayEvents;} set{nowPlayEvents = value;}}
        private bool nowPlayEvents = false;
        
        void Awake()
        {
            // インスタンス化
            InputEvent  = new TitleInputEvent(this.gameObject);
            ObjectManager.Player = new PlayerManager();
            eventSetting = new EventManager();
            Move = new MakeTweenMovengs();
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
        public IReadOnlyReactiveProperty<bool> ResetCameraToStart => keyBackInput;

        // =================================================================


        
        // 以下入力イベント実装===============================================
        private readonly ReactiveProperty<bool> keyReturnInput = new BoolReactiveProperty();
        private readonly ReactiveProperty<bool> keyBackInput = new BoolReactiveProperty();
        // =================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tmpObj">イベント対象オブジェクト</param>
        public TitleInputEvent(GameObject tmpObj)
        {
            // 以下指定オブジェクトDestroy時にイベント破棄設定=========
            keyReturnInput.AddTo(tmpObj);
            keyBackInput.AddTo(tmpObj);
            // =====================================================
        }

        /// <summary>
        /// 入力更新関数
        /// </summary>
        public void Update()
        {
            // 以下各種入力をReactivePropertyに反映=========================
            keyReturnInput.Value = Input.GetKeyDown(KeyCode.Return);
            keyBackInput.Value = Input.GetKeyDown(KeyCode.Backspace);
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
    /// Tween挙動作成クラス
    /// </summary>
    public sealed class MakeTweenMovengs
    {
        private SceneMoveTime moveTime = new SceneMoveTime(4);
        /// <summary>
        /// シーン挙動
        /// </summary>
        public void GameStartMovement()
        {
            
            // カメラのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.MainCamera.transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            // Tween目標座標座標
            var targetCoordinates = new Vector3(
                ObjectManager.Player.HitObject.transform.position.x,
                ObjectManager.Player.HitObject.transform.position.y + (ObjectManager.Player.HitObject.transform.localScale.y / 2),
                ObjectManager.Player.HitObject.transform.position.z - (ObjectManager.Player.HitObject.transform.localScale.z / 2)
            );

            // Appendで動作を追加
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DOMove(
                targetCoordinates,
                moveTime.MoveTimeAmount
            ).SetEase(Ease.Linear)
            .OnComplete(() => SceneManager.LoadScene("InGameScene")));

            // Appendで動作を追加
            sequence.Join(ObjectManager.TitleScene.FadePanel.rectTransform.DOScale(
                Vector3.zero,
                moveTime.MoveTimeAmount
            ).SetEase(Ease.InCirc));

            // Playで実行
            sequence.Play();
        }

        /// <summary>
        /// レシピ本開ける動作
        /// </summary>
        public async UniTask OpenRecipeBook()
        {
            
            // カメラのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.MainCamera.transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            // Tween目標座標座標
            var targetCoordinates = new Vector3(
                ObjectManager.Player.HitObject.transform.position.x,
                ObjectManager.Player.HitObject.transform.position.y + (ObjectManager.Player.HitObject.transform.localScale.y * 2),
                ObjectManager.Player.HitObject.transform.position.z - (ObjectManager.Player.HitObject.transform.localScale.z)
            );

            // Appendで動作を追加
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DOMove(
                targetCoordinates,
                moveTime.MoveTimeAmount / 2
            ).SetEase(Ease.Linear));
            
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DORotate(
                Vector3.right * 45,
                moveTime.MoveTimeAmount / 2
            ).SetEase(Ease.Linear)
            .OnComplete(() => Debug.Log("OpenRecipeBook")));

            
            // Playで実行
            sequence.Play();
            // 終了するまで待つ
            await sequence.AsyncWaitForCompletion();
        }

        /// <summary>
        /// 冷蔵庫開ける動作
        /// </summary>
        public async UniTask OpenRefrugerator()
        {
            // カメラのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.MainCamera.transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            // Tween目標座標座標
            var targetCoordinates = new Vector3(
                ObjectManager.Player.HitObject.transform.position.x,
                ObjectManager.Player.HitObject.transform.position.y + (ObjectManager.Player.HitObject.transform.localScale.y / 2.5f),
                ObjectManager.Player.HitObject.transform.position.z - (ObjectManager.Player.HitObject.transform.localScale.z * 2)
            );

            // Appendで動作を追加
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DOMove(
                targetCoordinates,
                moveTime.MoveTimeAmount / 2
            ).SetEase(Ease.Linear)
            .OnComplete(() => Debug.Log("OpenRefrugerator")));

            
            // Playで実行
            sequence.Play();
            // 完了するまで待つ
            await sequence.AsyncWaitForCompletion();
        }

        /// <summary>
        /// 初期位置に戻る
        /// </summary>
        public async UniTask ResetCamera()
        {
            // カメラのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.MainCamera.transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();


            // Appendで動作を追加
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DOMove(
                ObjectManager.TitleScene.CameraData.StartPos,
                moveTime.MoveTimeAmount / 2
            ).SetEase(Ease.Linear)
            .OnComplete(() => Debug.Log("ResetCamera")));

            sequence.Join(ObjectManager.TitleScene.MainCamera.transform.DORotate(
                ObjectManager.TitleScene.CameraData.StartAngle,
                moveTime.MoveTimeAmount / 2
            ).SetEase(Ease.Linear));

            // Playで実行
            sequence.Play();
            // 完了するまで待つ
            await sequence.AsyncWaitForCompletion();
        }
    }

}

