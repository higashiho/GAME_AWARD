using System.Collections;
using System.Collections.Generic;
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

            await UniTask.Delay(1000);
            Debug.Log("foodRate");
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.FOOD_AMOUNT);
        }
        /// <summary>
        /// リザルトパターンが食材量確認時処理
        /// </summary>
        private async void foodAmountEvenet()
        {

            await UniTask.Delay(1000);
            Debug.Log("foodAmount");
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.SEASONING);
        }
        /// <summary>
        /// リザルトパターンが調味料確認時処理
        /// </summary>
        private async void seasoningEvenet()
        {

            await UniTask.Delay(1000);
            Debug.Log("seasoning");
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.JUDGMENT);
        }
        /// <summary>
        /// リザルトパターンがゲージ確認時処理
        /// </summary>
        private async void judgmentEvenet()
        {

            await UniTask.Delay(1000);
            Debug.Log("judgment");
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.END);
        }
        /// <summary>
        /// リザルトパターンがエンド時処理
        /// </summary>
        private async void endEvenet()
        {

            await UniTask.Delay(1000);
            Debug.Log("end");
        }

        /// <summary>
        /// ゲージアップ処理
        /// </summary>
        private async void gageUp()
        {

        }
    }
}

