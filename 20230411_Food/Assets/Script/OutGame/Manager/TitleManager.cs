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
        private TitlePlayerData playerData;
        public TitlePlayerData PlayerData{get{return playerData;}}
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
        async void Awake()
        {
            // インスタンス化
            getData = new GetData();

            // リストを取得出来ていなかったら取得する
            if(DishData.DishPointData.Count == 0)
            {    
                dishData = new DishData(getData);
                await dishData.LoadData();
                dishData.GetDishData();
                Debug.Log(DishData.DishPointData.Count);
            }
            if(TitleTextData.TextData.Count == 0)
            {
                textData = new TitleTextData(getData);
                await textData.LoadData();
                textData.GetTextData();
                Debug.Log(TitleTextData.TextData.Count);
            }
            // インスタンス化
            ObjectManager.TitleScene = this;
            ObjectManager.InputEvent = new InputEvent(this.gameObject);
            ObjectManager.Player = new PlayerManager(PlayerManager.PlayerState.MAIN);
            ObjectManager.SubPlayer = new PlayerManager(PlayerManager.PlayerState.SUB);
            textApproachEvent = new TextApproachEventManager();

            // テキストイベント設定
            textApproachEvent.TextApproachEvents();
            // イベント設定
            ObjectManager.InputEvent.InputSetting.SetInputEvents();
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
    /// オブジェクト管理クラス
    /// </summary>
    public abstract class ObjectManager
    {
        // プレイヤー
        private static PlayerManager player;
        public static PlayerManager Player{
            get{return player;} 
            set{player = value;}
        }
        // サブプレイヤー
        private static PlayerManager subPlayer;
        public static PlayerManager SubPlayer{
            get{return subPlayer;}
            set{subPlayer = value;}
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
        private static InputEvent inputEvent;
        public static InputEvent InputEvent{
            get{return inputEvent;} 
            set{inputEvent = value;} 
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
                // UI表示
                ObjectManager.Ui.SetRecipeBookChanvasActive(true)
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

