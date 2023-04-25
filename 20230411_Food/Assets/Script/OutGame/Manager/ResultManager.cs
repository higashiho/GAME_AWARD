using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx.Triggers;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Result
{
    public class ResultManager : MonoBehaviour
    {
        private ResultMove move;

        [SerializeField, Header("Gageイメージのマスク")]
        private GameObject[] gageImages = new GameObject[2];
        public GameObject[] GageImages{get => gageImages;}

        [SerializeField, Header("割合管理データ")]
        private FoodPointRateData foodPointRate;
        public FoodPointRateData FoodPointRate{get => foodPointRate;} 
        public CancellationTokenSource Cts{get;} = new CancellationTokenSource();
        // Start is called before the first frame update
        void Start()
        {
            // インスタンス化
            ObjectManager.Result = this;
            ObjectManager.Events = new EventsManager(this.gameObject);
            move = new ResultMove();

            // ループイベント設定
            move.SetEvents();
            move.SetLoops();

            
            // 初期サブジェクト代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.START);
        }
    }
    public sealed class ObjectManager
    {
        // リザルトマネージャー
        private static ResultManager result;
        public static ResultManager Result{
            get => result;
            set{result = value;}
        }

        // イベントマネージャー
        private static EventsManager events;
        public static EventsManager Events{
            get => events;
            set{events = value;}
        }
    }

    /// <summary>
    /// リザルトの挙動クラス
    /// </summary>
    public class ResultMove
    {
        // どのイベントが実装可能か配列
        private bool[] onEventMountingFlag = new bool[6]
        {
            false, false, false, false, false, false
        };

        private UnityAction[] resultEvent = new UnityAction[6];

        // コンストラクタ
        public ResultMove()
        {
            // イベント設定
            resultEvent[0] = startEvenet;
            resultEvent[1] = foodRateEvenet;
            resultEvent[2] = foodAmountEvenet;
            resultEvent[3] = seasoningEvenet;
            resultEvent[4] = judgmentEvenet;
            resultEvent[5] = endEvenet;

        }


        /// <summary>
        /// イベント設定関数
        /// </summary>
        public void SetEvents()
        {
            ObjectManager.Events.ResultPattern
                .Where(x => x == EventsManager.ResultPatternEnum.START)
                .Subscribe(_ => onEventMountingFlag[0] = true);

            ObjectManager.Events.ResultPattern
                .Where(x => x == EventsManager.ResultPatternEnum.FOOD_RATE)
                .Subscribe(_ => onEventMountingFlag[1] = true);

            ObjectManager.Events.ResultPattern
                .Where(x => x == EventsManager.ResultPatternEnum.FOOD_AMOUNT)
                .Subscribe(_ => onEventMountingFlag[2] = true);

            ObjectManager.Events.ResultPattern
                .Where(x => x == EventsManager.ResultPatternEnum.SEASONING)
                .Subscribe(_ => onEventMountingFlag[3] = true);

            ObjectManager.Events.ResultPattern
                .Where(x => x == EventsManager.ResultPatternEnum.JUDGMENT)
                .Subscribe(_ => onEventMountingFlag[4] = true);
                
            ObjectManager.Events.ResultPattern
                .Where(x => x == EventsManager.ResultPatternEnum.END)
                .Subscribe(_ => onEventMountingFlag[5] = true);
        }

        /// <summary>
        /// イベント判断ループ
        /// </summary>
        public void SetLoops()
        {
            ObjectManager.Result.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    for(int i = 0; i < onEventMountingFlag.Length; i++)
                    {
                        // フラグがたっていたら実行
                        if(onEventMountingFlag[i])
                        {
                            // フラグを折って実行
                            onEventMountingFlag[i] = false;
                            resultEvent[i]();
                            break;
                        }
                    }
                }
                ).AddTo(ObjectManager.Result);
        }

        /// <summary>
        /// リザルトパターンがスタート時処理
        /// </summary>
        private async void startEvenet()
        {

            await UniTask.Delay(1000);
            Debug.Log("Start");
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.FOOD_RATE);
        }
        /// <summary>
        /// リザルトパターンが食材割合確認時処理
        /// </summary>
        private async void foodRateEvenet()
        {
            Debug.Log("foodRate");

            await gageUp(ObjectManager.Result.FoodPointRate.Rate[0]);
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.FOOD_AMOUNT);
        }
        /// <summary>
        /// リザルトパターンが食材量確認時処理
        /// </summary>
        private async void foodAmountEvenet()
        {
            Debug.Log("foodAmount");

            await gageUp(ObjectManager.Result.FoodPointRate.Rate[1]);
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.SEASONING);
        }
        /// <summary>
        /// リザルトパターンが調味料確認時処理
        /// </summary>
        private async void seasoningEvenet()
        {
            Debug.Log("seasoning");

            await gageUp(ObjectManager.Result.FoodPointRate.Rate[2]);
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.JUDGMENT);
        }
        /// <summary>
        /// リザルトパターンがゲージ確認時処理
        /// </summary>
        private async void judgmentEvenet()
        {
            Debug.Log("judgment");

            await UniTask.Delay(1000);
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.END);
        }
        /// <summary>
        /// リザルトパターンがエンド時処理
        /// </summary>
        private async void endEvenet()
        {
            Debug.Log("end");

            await UniTask.Delay(1000);
        }

        /// <summary>
        /// ゲージアップ処理
        /// </summary>
        /// <param name="rate">ゲージアップする割合</param>
        private async UniTask gageUp(int rate)
        {
            // ゲージ減算最大値                 // 割合を取りたいため0.1積算
            var gageAmount = ObjectManager.Result.GageImages[0].GetComponent<RectTransform>().sizeDelta.y * (rate * 0.1f);

            
            // ゲージイメージパディング取得
            var playerGagePadding = ObjectManager.Result.GageImages[0].GetComponent<RectMask2D>().padding;
            var subPlayerGagePadding = ObjectManager.Result.GageImages[1].GetComponent<RectMask2D>().padding;

            // プレイヤーとサブプレイヤーのゲージ減算量
            var playerGageAmount = gageAmount * (100f / 100f);
            var subPlayerGageAmount = gageAmount * (80f / 100f);

            
            // ゲージ目標paddingのtop
            var playerTargetPadding = playerGagePadding.w - playerGageAmount;
            var subPlayerTargetPadding = subPlayerGagePadding.w - subPlayerGageAmount;

            // ０．１秒に対して減算する量(20%ずつ減算するため２０で除算)
            var playerDecrement = playerGageAmount / 20f;
            var subPlayerDecrement = subPlayerGageAmount / 20f;
            Debug.Log("gageAmount : " + gageAmount);
            Debug.Log("playerTargetPadding : " + playerTargetPadding);
            Debug.Log("subPlayerTargetPadding : " + subPlayerTargetPadding);
            // ゲージ減算ループ
            while(!ObjectManager.Result.Cts.Token.IsCancellationRequested)
            {
                // TODO　ゲージが指定一以下になったらではなく２秒経ったら処理終了
                // またはどちらのゲージも伸びなくなったら終了処理。
                // ゲージ増加量は規定値。
                
                // 指定値以下の場合は
                playerGagePadding.w -= playerDecrement;
                subPlayerGagePadding.w -= subPlayerDecrement;
                
                // 代入
                ObjectManager.Result.GageImages[0].GetComponent<RectMask2D>().padding = playerGagePadding;
                ObjectManager.Result.GageImages[1].GetComponent<RectMask2D>().padding = subPlayerGagePadding;

                // ゲージが指定値以下になったら処理終了
                if(playerGagePadding.w <= playerTargetPadding && subPlayerGagePadding.w <= subPlayerTargetPadding)
                {
                    playerGagePadding.w = playerTargetPadding;
                    subPlayerGagePadding.w = subPlayerTargetPadding;
                    ObjectManager.Result.GageImages[0].GetComponent<RectMask2D>().padding = playerGagePadding;
                    ObjectManager.Result.GageImages[1].GetComponent<RectMask2D>().padding = subPlayerGagePadding;

                    // ３秒待つ
                    await UniTask.Delay(3000);
                    break;
                }

                await UniTask.Delay(100);
            }
        }
    }
}

