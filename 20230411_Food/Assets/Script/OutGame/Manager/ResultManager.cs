using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx.Triggers;
using UniRx;

namespace Result
{
    public class ResultManager : MonoBehaviour
    {
        private ResultMove move = new ResultMove();
        // Start is called before the first frame update
        void Start()
        {
            // インスタンス化
            ObjectManager.Result = this;
            ObjectManager.Events = new EventsManager(this.gameObject);

            // ループイベント設定
            move.SetEvents();
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
            resultEvent[1] = startEvenet;
            resultEvent[2] = startEvenet;
            resultEvent[3] = startEvenet;
            resultEvent[4] = startEvenet;
            resultEvent[5] = startEvenet;
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
                            resultEvent[i]();
                            break;
                        }
                    }
                }
                ).AddTo(ObjectManager.Result);
        }

        private async void startEvenet()
        {
            onEventMountingFlag[0] = false;


            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.FOOD_RATE);
        }
    }
}

