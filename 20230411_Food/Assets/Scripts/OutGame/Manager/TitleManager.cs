using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Cysharp.Threading.Tasks;


using OutGame;
using FoodPoint;


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
        [SerializeField, Header("Playerのデータ")]
        private TitlePlayerDataList playerData;
        public TitlePlayerDataList PlayerData{get{return playerData;}}
        /// <summary>
        /// テキスト接近イベント
        /// </summary>
        private TextApproachEventManager textApproachEvent;

        /// <summary>シーン挙動クラスインスタンス化</summary>
        public MakeTweenMovengs Move{get;} = new MakeTweenMovengs();

        /// <summary>何かのイベントが処理されているか</summary>
        public bool NowPlayeEvents{get{return nowPlayEvents;} set{nowPlayEvents = value;}}
        private bool nowPlayEvents = false;


        // データ取得クラス
        private GetData getData;
        // 料理データクラス
        private DishData dishData;
        // テキストデータクラス
        private TitleTextData textData;

        public CancellationTokenSource Cts{get;} = new CancellationTokenSource();

        [SerializeField]
        private GameObject audioController;
        public GameObject AudioController{get => audioController;}
        async void Awake()
        {
            Application.targetFrameRate = 60;
            setSubscribe();
            // インスタンス化
            getData = new GetData();

            // リストを取得出来ていなかったら取得する
            if(DishData.DishPointData.Count == 0)
            {    
                dishData = new DishData(getData);
                await dishData.LoadData();
                dishData.GetDishData();
            }
            if(TitleTextData.TextData.Count == 0)
            {
                textData = new TitleTextData(getData);
                await textData.LoadData();
                textData.GetTextData();
            }
            // インスタンス化
            ObjectManager.TitleScene = this;
            ObjectManager.Events = new EventsManager(this.gameObject, PlayerData);
            // Playerの生成
            for(int i = 0; i < ObjectManager.Player.Length; i++)
            {
                ObjectManager.Player[i] = new PlayerManager(playerData.PlayerDatas[i]);
            }
            textApproachEvent = new TextApproachEventManager();

            // テキストイベント設定
            textApproachEvent.TextApproachEvents();
            // イベント設定
            ObjectManager.Events.InputSetting.SetEvents();
        }

        private void setSubscribe()
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => {     
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif                
                }).AddTo(this);
        }

        private void OnDestroy() 
        {
            for(int i = 0; i < ObjectManager.Player.Length; i++)
                Addressables.Release(ObjectManager.Player[i].Handle);

            
            DOTween.KillAll();
            Cts.Cancel();
        }

    }
   
    /// <summary>
    /// オブジェクト管理クラス
    /// </summary>
    public abstract class ObjectManager
    {
        // プレイヤー
        private static PlayerManager[] player = new PlayerManager[2];
        public static PlayerManager[] Player{
            get{return player;} 
            set{player = value;}
        }

        // タイトルマネージャー
        private static TitleManager titleScene;
        public static TitleManager TitleScene{
            get{return titleScene;} 
            set{titleScene = value;} 
        }

        // UIコントローラー
        private static UIController ui;
        public static UIController Ui{
            get{return ui;}
            set{ui = value;}
        }

        // テキストマネージャー
        private static TextManager text;
        public static TextManager Text{
            get{return text;}
            set{text = value;}
        }

        // インプットイベント
        private static EventsManager events;
        public static EventsManager Events{
            get{return events;} 
            set{events = value;} 
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
        /// <param name="hitObject">当たった対象オブジェクト</param>
        public void GameStartMovement(GameObject hitObject)
        {
            
            // カメラのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.MainCamera.transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            // Tween目標座標座標
            var targetCoordinates = new Vector3(
                hitObject.transform.position.x,
                hitObject.transform.position.y + (hitObject.transform.localScale.y / 2),
                hitObject.transform.position.z - (hitObject.transform.localScale.z / 2)
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
        /// <param name="hitObject">当たった対象オブジェクト</param>
        public async UniTask OpenRecipeBook(GameObject hitObject)
        {
            
            // カメラのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.MainCamera.transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();
                // Tween目標座標座標
            var targetCoordinates = new Vector3(
                    hitObject.transform.position.x,
                    hitObject.transform.position.y + (hitObject.transform.localScale.y * 2),
                    hitObject.transform.position.z - (hitObject.transform.localScale.z)
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
            .OnComplete(() => 
            {
                // スクロールされていたらリセット
                ObjectManager.Ui.TipUi.ScrollReset();
                // UI表示
                ObjectManager.Ui.SetRecipeBookChanvasActive(true);
            }
            ));

            
            // Playで実行
            sequence.Play();
            // 終了するまで待つ
            await sequence.AsyncWaitForCompletion();
        }

        /// <summary>
        /// 冷蔵庫開ける動作
        /// </summary>
        /// <param name="hitObject">当たった対象オブジェクト</param>
        public async UniTask OpenRefrugerator(GameObject hitObject)
        {
            // カメラのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.MainCamera.transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            // Tween目標座標座標
            var targetCoordinates = new Vector3(
                hitObject.transform.position.x,
                hitObject.transform.position.y + (hitObject.transform.localScale.y / 2.5f),
                hitObject.transform.position.z - (hitObject.transform.localScale.z * 2)
            );

            // Appendで動作を追加
            sequence.Append(ObjectManager.TitleScene.MainCamera.transform.DOMove(
                targetCoordinates,
                moveTime.Amount / 2
            ).SetEase(Ease.Linear)
            .OnComplete(() => 
                // UI表示
                ObjectManager.Ui.SetRefrigeratorCanvasActive(true)
            ));

            
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
            ).SetEase(Ease.Linear).OnStart(() => {
                // アクティブ状態なら非アクティブに変更
                if(ObjectManager.Ui.RecipeBookChanvas.gameObject.activeSelf)
                    ObjectManager.Ui.SetRecipeBookChanvasActive(false);
                // アクティブ状態なら非アクティブに変更
                if(ObjectManager.Ui.RefrigeratorCanvas.gameObject.activeSelf)    
                    ObjectManager.Ui.SetRefrigeratorCanvasActive(false);
            })
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

