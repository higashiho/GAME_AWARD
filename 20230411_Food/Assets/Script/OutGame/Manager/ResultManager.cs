using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx.Triggers;
using UniRx;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;
using OutGame;
using Constants;

namespace Result
{
    public class ResultManager : MonoBehaviour
    {
        // インスタンス化
        private ResultUIMove move;

        [SerializeField, Header("Gageイメージのマスク")]
        private GameObject[] gageImages = new GameObject[2];
        public GameObject[] GageImages{get => gageImages;}

        [SerializeField, Header("割合管理データ")]
        private FoodPointRateData foodPointRate;
        public FoodPointRateData FoodPointRate{get => foodPointRate;} 
        public CancellationTokenSource Cts{get;} = new CancellationTokenSource();

        [SerializeField, Header("テキストの親オブジェクト")]
        private GameObject textsParent = default;
        public GameObject TextsParent{get => textsParent;}
        [SerializeField, Header("リザルトテキスト")]
        private GameObject[] resultText = new GameObject[2];
        public GameObject[] ResultText{get => resultText;}
        [SerializeField, Header("Player達のアニメーション配列")]
        private Animator[] playerAnim = new Animator[2];
        public Animator[] PlayerAnim{get => playerAnim;}
        

        // Start is called before the first frame update
        void Start()
        {
            // インスタンス化
            ObjectManager.Result = this;
            ObjectManager.Events = new EventsManager(this.gameObject);
            move = new ResultUIMove();

            // 購読設定
            move.SetSubscribe();

            
            // 初期サブジェクト代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.START);
        }

        void OnDestroy()
        {
            Cts.Cancel();
        }
    }

    /// <summary>
    /// オブジェクト管理クラス
    /// </summary>
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
    public class ResultUIMove
    {
        // インスタンス化
        private GageMove gage;
        private TextMove text;

        // どのイベントが実装可能か配列
        private bool[] onEventMountingFlag = new bool[6]
        {
            false, false, false, false, false, false
        };

        // リザルトが終われるか
        private bool nowFinishFlag = false;

        private UnityAction[] resultEvent = new UnityAction[6];

        // コンストラクタ
        public ResultUIMove()
        {
            gage = new GageMove();
            // イベント設定
            setEvenet();
        }

        /// <summary>
        /// イベント設定
        /// </summary>
        private void setEvenet()
        {
            resultEvent[0] = startEvenet;
            resultEvent[1] = foodRateEvenet;
            resultEvent[2] = foodAmountEvenet;
            resultEvent[3] = seasoningEvenet;
            resultEvent[4] = judgmentEvenet;
            resultEvent[5] = endEvenet;
        }

        /// <summary>
        /// 購読設定関数
        /// </summary>
        public void SetSubscribe()
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

                ObjectManager.Result.UpdateAsObservable()
                .Where(_ => ObjectManager.Result.ResultText[0].activeSelf)
                .Subscribe(_ =>
                {
                    ObjectManager.Result.ResultText[0].GetComponent<TextMeshProUGUI>().color = Color.HSVToRGB(Time.time % 1, 1, 1);
                
                    if(nowFinishFlag)
                    {
                        if(Input.GetKeyDown(KeyCode.Return))
                            SceneManager.LoadScene("InGameScene");

                        if(Input.GetKeyDown(KeyCode.Space))
                            SceneManager.LoadScene("TitleScene");
                    }
                }).AddTo(ObjectManager.Result);
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
            // インスタンス化
            text = new TextMove(ObjectManager.Result.TextsParent);

            await text.Movement();

            await gage.Increase(ObjectManager.Result.FoodPointRate.Rate[0], 80f, 100f);
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.FOOD_AMOUNT);
        }
        /// <summary>
        /// リザルトパターンが食材量確認時処理
        /// </summary>
        private async void foodAmountEvenet()
        {
            Debug.Log("foodAmount");
            // インスタンス化
            text = new TextMove(ObjectManager.Result.TextsParent);

            await text.Movement();

            await gage.Increase(ObjectManager.Result.FoodPointRate.Rate[1], 80f, 100f);
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.SEASONING);
        }
        /// <summary>
        /// リザルトパターンが調味料確認時処理
        /// </summary>
        private async void seasoningEvenet()
        {
            Debug.Log("seasoning");
            // インスタンス化
            text = new TextMove(ObjectManager.Result.TextsParent);

            await text.Movement();

            await gage.Increase(ObjectManager.Result.FoodPointRate.Rate[2], 80f, 100f);
            // 次の状態を代入
            ObjectManager.Events.SetResultPatterunSubject(EventsManager.ResultPatternEnum.JUDGMENT);
        }
        /// <summary>
        /// リザルトパターンがゲージ確認時処理
        /// </summary>
        private async void judgmentEvenet()
        {
            Debug.Log("judgment");

            // Player勝利
            if(gage.GetPlayerGageMaskPadding().w <= gage.GetSubPlayerGageMaskPadding().w)
            {
                ObjectManager.Result.PlayerAnim[0].SetBool("Win", true);
                ObjectManager.Result.PlayerAnim[1].SetBool("Lose", true);
                ObjectManager.Result.ResultText[0].GetComponent<TextMeshProUGUI>().text = "1P Win !!!!!!!!!!!!!!";
            }
            // サブプレイヤー勝利
            else
            {
                ObjectManager.Result.PlayerAnim[0].SetBool("Lose", true);
                ObjectManager.Result.PlayerAnim[1].SetBool("Win", true);
                ObjectManager.Result.ResultText[0].GetComponent<TextMeshProUGUI>().text = "2P Win !!!!!!!!!!!!!!";
            }

            ObjectManager.Result.ResultText[0].SetActive(true);ObjectManager.Result.ResultText[0].transform.DOScale(Vector3.one, 2f).SetEase(Ease.Linear);
            ObjectManager.Result.ResultText[0].transform.DOLocalRotate(new Vector3(0,0,360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(4, LoopType.Restart);

  

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
            if(!ObjectManager.Result.ResultText[1].activeSelf)
            {
                ObjectManager.Result.ResultText[1].SetActive(true);
                ObjectManager.Result.ResultText[1].GetComponent<TextMeshProUGUI>().DOFade(1, 1).SetEase(Ease.Linear);
            }
            nowFinishFlag = true;
            await UniTask.Delay(1000);
        }  
    }

    /// <summary>
    /// ゲージ挙動クラス
    /// </summary>
    public class GageMove
    {
        private RectMask2D playerGageMask;
        private RectMask2D subPlayerGageMask;

        public GageMove()
        {
            playerGageMask = ObjectManager.Result.GageImages[0].GetComponent<RectMask2D>();
            subPlayerGageMask = ObjectManager.Result.GageImages[1].GetComponent<RectMask2D>();
        }

        /// <summary>
        /// ゲージアップ処理
        /// </summary>
        /// <param name="rate">ゲージアップする割合</param>
        /// <param name="playerPoint">プレイヤーの得点</param>
        /// <param name="subPlayerPoint">サブプレイヤーの得点</param>
        public async UniTask Increase(int rate, float playerPoint, float subPlayerPoint)
        {
            // ゲージ減算最大値     (割合を取る為10分の1)
            var gageAmount = ObjectManager.Result.GageImages[0].GetComponent<RectTransform>().sizeDelta.y * (rate * 0.1f);
            // 0.001秒に対して減算する量        (400%ずつ減算するため400で除算)
            var gameDecrement = gageAmount / 400f;
            
            // ゲージイメージパディング取得
            var playerGagePadding = playerGageMask.padding;
            var subPlayerGagePadding = subPlayerGageMask.padding;

            // プレイヤーとサブプレイヤーのゲージ減算量     (％を取る為100分の1)
            var playerGageAmount = gageAmount * (playerPoint / 100f);
            var subPlayerGageAmount = gageAmount * (subPlayerPoint / 100f);

            
            // ゲージパディングの目標Top値
            var playerTargetPadding = playerGagePadding.w - playerGageAmount;
            var subPlayerTargetPadding = subPlayerGagePadding.w - subPlayerGageAmount;

            // ゲージ減算ループ
            while(!ObjectManager.Result.Cts.Token.IsCancellationRequested)
            {
                
                // 指定値以下の場合は
                if(playerGagePadding.w > playerTargetPadding)
                    playerGagePadding.w -= gameDecrement;
                if(subPlayerGagePadding.w > subPlayerTargetPadding)
                    subPlayerGagePadding.w -= gameDecrement;
                
                // 代入
                playerGageMask.padding = playerGagePadding;
                subPlayerGageMask.padding = subPlayerGagePadding;

                // ゲージが指定値以下になったら処理終了
                if(playerGagePadding.w <= playerTargetPadding && subPlayerGagePadding.w <= subPlayerTargetPadding)
                {
                    // ずれ修正
                    playerGagePadding.w = playerTargetPadding;
                    subPlayerGagePadding.w = subPlayerTargetPadding;

                    // 代入
                    playerGageMask.padding = playerGagePadding;
                    subPlayerGageMask.padding = subPlayerGagePadding;

                    // 2000ミリ秒待つ
                    await UniTask.Delay(1000);
                    break;
                }

                // 1ミリ秒待つ
                await UniTask.Delay(1);
            }
        }

        /// <summary>
        /// マスクパディング取得
        /// </summary>
        /// <returns>パディングの値</returns>
        public Vector4 GetPlayerGageMaskPadding()
        {
            return playerGageMask.padding;
        }

        /// <summary>
        /// マスクパディング取得
        /// </summary>
        /// <returns>パディングの値</returns>
        public Vector4 GetSubPlayerGageMaskPadding()
        {
            return subPlayerGageMask.padding;
        }
    }

    /// <summary>
    /// テキスト挙動クラス
    /// </summary>
    public class TextMove
    {
        private TextMeshProUGUI[] texts = new TextMeshProUGUI[3];

        private UIMoveTile moveTile = new UIMoveTile(0.5f);

        private bool onMoveFlag = false;

        public TextMove(GameObject parent)
        {
            // 初期化
            for(int i = 0; i < texts.Length; i++)
            {
                texts[i] = parent.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
                texts[i].rectTransform.localPosition = ResultConstants.TEXT_START_POS[i];
                
                texts[i].color = new Color(1,1,1,1);
            }
        }

        /// <summary>
        /// テキスト挙動関数
        /// </summary>
        public async UniTask Movement()
        {
            await main();

            player();

            sub();

            // 挙動フラグがたっていたら待つ
            await UniTask.WaitWhile(() => !onMoveFlag);

        }

        /// <summary>
        /// メインテキスト挙動
        /// </summary>
        private async UniTask main()
        {
            changeText(texts[0], "resultName");

            var textTween = texts[0].transform.DOLocalMoveY(ResultConstants.TARGET_POS_Y[1] ,moveTile.Amount).SetEase(Ease.Linear);

            await textTween.AsyncWaitForCompletion();
        }

        /// <summary>
        /// メインプレイヤーテキスト挙動
        /// </summary>
        private void player()
        {
            var sequence = DOTween.Sequence();

            changeText(texts[1], "playerPoint");

            // 挙動追加
            sequence.Append(texts[1].transform.DOLocalMoveX(-ResultConstants.TARGET_POS_X ,moveTile.Amount).SetEase(Ease.Linear));

            // 一秒待機
            sequence.AppendInterval(moveTile.Amount);

            // 挙動追加
            sequence.Append(texts[1].transform.DOLocalMoveY(ResultConstants.TARGET_POS_Y[0] ,moveTile.Amount).SetEase(Ease.Linear));
            
            // 同時挙動追加
            sequence.Join(texts[1].DOFade(0 ,moveTile.Amount).SetEase(Ease.Linear));

            // 再生
            sequence.Play();
        }

        /// <summary>
        /// サブプレイヤーテキスト挙動
        /// </summary>
        private void sub()
        {
            var sequence = DOTween.Sequence();

            changeText(texts[2], "subPlayerPoint");
            
            // 挙動追加
            sequence.Append(texts[2].transform.DOLocalMoveX(ResultConstants.TARGET_POS_X ,moveTile.Amount).SetEase(Ease.Linear));

            // 一秒待機
            sequence.AppendInterval(moveTile.Amount);

            // 挙動追加
            sequence.Append(texts[2].transform.DOLocalMoveY(ResultConstants.TARGET_POS_Y[0] ,moveTile.Amount).SetEase(Ease.Linear));
            
            // 同時挙動追加
            sequence.Join(texts[2].DOFade(0 ,moveTile.Amount).SetEase(Ease.Linear));

            // 再生
            sequence.Play().OnComplete(() => 
            {
                onMoveFlag = true;
            });
        }

        private void changeText(TextMeshProUGUI text, string message)
        {
            text.text = message;
        }
    }
}