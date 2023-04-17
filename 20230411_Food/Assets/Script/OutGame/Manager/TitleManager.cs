using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx.Triggers;
using UniRx;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Constants;


namespace Title
{
    /// <summary>
    /// タイトル管理クラス
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        [SerializeField, Header("メインカメラ")]
        private GameObject mainCamera;
        public GameObject MainCamera{get{return mainCamera;}}

        [SerializeField, Header("フェイドパネルイメージ")]
        private Image fadePanel;
        public Image FadePanel{get{return fadePanel;}}

        [SerializeField, Header("タイトルカメラのデータ")]
        private TitleCameraData cameraData;
        public TitleCameraData CameraData{get{return cameraData;}}

        [SerializeField, Header("テキストイメージ")]
        private Image[] textImageCanvas = new Image[3];
        public Image[] TextImageCanvas{get{return textImageCanvas;}}
       
        /// <summary>入力イベントインスタンス化</summary>
        public TitleInputEvent InputEvent{get; private set;}

        /// <summary>イベント管理クラスインスタンス化</summary>
        private InputEventManager eventSetting;
        /// <summary>
        /// テキスト接近イベント
        /// </summary>
        private TextApproachEventManager textApproachEvent;

        /// <summary>シーン挙動クラスインスタンス化</summary>
        public MakeTweenMovengs Move{get; private set;}

        /// <summary>何かのイベントが処理されているか</summary>
        public bool NowPlayeEvents{get{return nowPlayEvents;} set{nowPlayEvents = value;}}
        private bool nowPlayEvents = false;

        public CancellationTokenSource Cts{get; private set;} = new CancellationTokenSource();
        void Awake()
        {
            // インスタンス化
            ObjectManager.TitleScene = this;
            InputEvent  = new TitleInputEvent(this.gameObject);
            ObjectManager.Player = new PlayerManager(PlayerManager.PlayerState.MAIN);
            ObjectManager.SubPlayer = new PlayerManager(PlayerManager.PlayerState.SUB);
            eventSetting = new InputEventManager();
            Move = new MakeTweenMovengs();
            textApproachEvent = new TextApproachEventManager();

            // タイトル入力確認ループ
            this.UpdateAsObservable()
                .Subscribe(_ => InputEvent.Update())
                .AddTo(this);

            // 入力イベント設定
            eventSetting.SetInputEvents();

            // テキストイベント設定
            textApproachEvent.TextApproachEvents();
        }

        private void OnDestroy() 
        {
            DOTween.KillAll();
            Cts.Cancel();
        }

        /// <summary>
        /// 描画関数
        /// </summary>
        void OnDrawGizmos()
        {
            // Ray描画
            //　Cubeのレイを疑似的に視覚化
            for(int i = 0; i < TextImageCanvas.Length; i++)
            {    
                var drawRayScale = new Vector3(
                    TextImageCanvas[i].transform.localScale.x * 3,
                    TextImageCanvas[i].transform.localScale.y * 2,
                    TextImageCanvas[i].transform.localScale.z * 0.5f
                );
                var drawRayPos = new Vector3(
                    TextImageCanvas[i].transform.position.x,
                    // 中央に表示しないように少し下げる
                    TextImageCanvas[i].transform.position.y * 0.9f,
                    TextImageCanvas[i].transform.position.z
                );
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(drawRayPos + TextImageCanvas[i].transform.forward, drawRayScale);
            }        
        }
    }
    /// <summary>
    /// タイトル入力イベント管理クラス
    /// </summary>
    public sealed class TitleInputEvent
    {
        // 以下入力イベント取得用変数==================================================
        // ゲームスタートイベント
        public IReadOnlyReactiveProperty<bool> GameStart => keyReturnInput;
        // 食材一覧表示イベント
        public IReadOnlyReactiveProperty<bool> FoodNicknames => keyReturnInput;
        // 食材相性表示イベント
        public IReadOnlyReactiveProperty<bool> DisplayIngredientsList => keyReturnInput;
        // カメラリセットイベント
        public IReadOnlyReactiveProperty<bool> ResetCameraToStart => keyBackInput;

        // =================================================================


        
        // 以下入力イベント実装===============================================
        private readonly ReactiveProperty<bool> keyReturnInput = new BoolReactiveProperty();
        private readonly ReactiveProperty<bool> keyBackInput = new BoolReactiveProperty();

        private Subject<float> foodNicknamesTextPoint = new Subject<float>();
        public Subject<float> FoodNicknamesTextPoint
        {get{return foodNicknamesTextPoint;} set{foodNicknamesTextPoint = value;}}
        private Subject<float> displayIngredientsListTextPoint = new Subject<float>();
        public Subject<float> DisplayIngredientsListTextPoint
        {get{return displayIngredientsListTextPoint;} set{displayIngredientsListTextPoint = value;}}
        private Subject<float> gameStartTextPoint = new Subject<float>();
        public Subject<float> GameStartTextPoint
        {get{return gameStartTextPoint;} set{gameStartTextPoint = value;}}
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
            FoodNicknamesTextPoint.AddTo(tmpObj);
            DisplayIngredientsListTextPoint.AddTo(tmpObj);
            GameStartTextPoint.AddTo(tmpObj);
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

    /// <summary>
    /// オブジェクト管理クラス
    /// </summary>
    public abstract class ObjectManager
    {
        // プレイヤー
        private static PlayerManager player;
        public static PlayerManager Player{
            get{return player;} 
            set{player = value; Debug.LogWarning("Assigned to player.");}
        }
        // サブプレイヤー
        private static PlayerManager subPlayer;
        public static PlayerManager SubPlayer{
            get{return subPlayer;}
            set{subPlayer = value; Debug.LogWarning("Assigned to subPlayer");}
        }

        // タイトルマネージャー
        private static TitleManager titleScene;
        public static TitleManager TitleScene{
            get{return titleScene;} 
            set{titleScene = value; Debug.LogWarning("Assigned to titleScene.");} 
            }

        // テキストマネージャー
        private static TextManager text;
        public static TextManager Text{
            get{return text;}
            set{text = value; Debug.LogWarning("Assigned to text");}
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
                moveTime.Amount
            ).SetEase(Ease.Linear)
            .OnComplete(() => SceneManager.LoadScene("InGameScene")));

            // Appendで動作を追加
            sequence.Join(ObjectManager.TitleScene.FadePanel.rectTransform.DOScale(
                Vector3.zero,
                moveTime.Amount
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
            var targetCoordinates = Vector3.zero;

            if(ObjectManager.Player.HitObject)
                // Tween目標座標座標
                targetCoordinates = new Vector3(
                    ObjectManager.Player.HitObject.transform.position.x,
                    ObjectManager.Player.HitObject.transform.position.y + (ObjectManager.Player.HitObject.transform.localScale.y * 2),
                    ObjectManager.Player.HitObject.transform.position.z - (ObjectManager.Player.HitObject.transform.localScale.z)
                );
            else if(ObjectManager.SubPlayer.HitObject)
                // Tween目標座標座標
                targetCoordinates = new Vector3(
                    ObjectManager.SubPlayer.HitObject.transform.position.x,
                    ObjectManager.SubPlayer.HitObject.transform.position.y + (ObjectManager.SubPlayer.HitObject.transform.localScale.y * 2),
                    ObjectManager.SubPlayer.HitObject.transform.position.z - (ObjectManager.SubPlayer.HitObject.transform.localScale.z)
                );

            // Appendで動作を追加
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DOMove(
                targetCoordinates,
                moveTime.Amount / 2
            ).SetEase(Ease.Linear));
            
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DORotate(
                Vector3.right * 45,
                moveTime.Amount / 2
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
                moveTime.Amount / 2
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
                moveTime.Amount / 2
            ).SetEase(Ease.Linear)
            .OnComplete(() => Debug.Log("ResetCamera")));

            sequence.Join(ObjectManager.TitleScene.MainCamera.transform.DORotate(
                ObjectManager.TitleScene.CameraData.StartAngle,
                moveTime.Amount / 2
            ).SetEase(Ease.Linear));

            // Playで実行
            sequence.Play();
            // 完了するまで待つ
            await sequence.AsyncWaitForCompletion();
        }
    }

}

