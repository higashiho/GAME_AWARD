using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Constants;
using DG.Tweening;
using TMPro;
using OutGame;

namespace Title
{
    /// <summary>
    /// UI挙動管理クラス
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [SerializeField, Header("レシピブックのキャンバス")]
        private Canvas recipeBookChanvas;
        public Canvas RecipeBookChanvas{get{return recipeBookChanvas;}}
        [SerializeField, Header("冷蔵庫のキャンバス")]
        private Canvas refrigeratorCanvas;
        public Canvas RefrigeratorCanvas{get{return refrigeratorCanvas;}}
        [SerializeField, Header("肉イメージSprite")]
        private Sprite[] meatImage = new Sprite[3];
        public Sprite[] MeatImage{get{return meatImage;}}
        [SerializeField, Header("魚イメージSprite")]
        private Sprite[] fishImage = new Sprite[3];
        public Sprite[] FishImage{get{return fishImage;}}
        [SerializeField, Header("野菜イメージSprite")]
        private Sprite[] vagImage = new Sprite[3];
        public Sprite[] VagImage{get{return vagImage;}}
        [SerializeField, Header("Player達の補助用UIキャンバス")]
        private Canvas assistCanvas;
        public Canvas AssistCanvas{get{return assistCanvas;}}


        // インスタンス化
        private RefrigeratorUIMove refrigeratorUIMove;
        private PlayerAssistUIMove assistUIMove;
        private TitlePlayerDataList playerDataList;

        // Start is called before the first frame update
        async void Start()
        {
            // インスタンス化
            refrigeratorUIMove = new RefrigeratorUIMove(RefrigeratorCanvas);
            ObjectManager.Ui = this;
            assistUIMove = new PlayerAssistUIMove(AssistCanvas);


            // インプットイベントがインスタンス化されるまで待つ
            await UniTask.WaitWhile(() => ObjectManager.Events == null);

            playerDataList = ObjectManager.TitleScene.PlayerData;
            // イベント設定
            refrigeratorUIMove.InputUISubscribe();

            // ループ設定
            setRoop();
        }

        /// <summary>
        /// ループ設定関数
        /// </summary>
        private void setRoop()
        {
            // アシストUI挙動設定
            this.UpdateAsObservable()
                .Where(_ => ObjectManager.Player[0].HitObject || ObjectManager.Player[1].HitObject)
                .Subscribe(_ => {
                    assistUIMove.Display(playerDataList.PlayerDatas[0].Id);
                    assistUIMove.Display(playerDataList.PlayerDatas[1].Id);
                    for(int i = 0; i < playerDataList.PlayerDatas.Count; i++)
                        assistUIMove.MainMovement(i);
                }).AddTo(this);
        }


        /// <summary>
        /// アクティブ化設定関数
        /// </summary>
        /// <param name="value">true : アクティブ false : 非アクティブ</param>
        public void SetRefrigeratorCanvasActive(bool value)
        {
            RefrigeratorCanvas.gameObject.SetActive(value);
        }

        /// <summary>
        /// アクティブ化設定関数
        /// </summary>
        /// <param name="value">true : アクティブ false : 非アクティブ</param>
        public void SetRecipeBookChanvasActive(bool value)
        {
            RecipeBookChanvas.gameObject.SetActive(value);
        }
        /// <summary>
        /// アクティブ化設定関数
        /// </summary>
        /// <param name="num">child要素数</param>
        /// <param name="value">true : アクティブ false : 非アクティブ</param>
        public void SetAssistPlayerUIActive(int num, bool value)
        {
            AssistCanvas.transform.GetChild(num).gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// Player達のアシストUI挙動クラス
    /// </summary>
    public class PlayerAssistUIMove
    {
        private Canvas canvas;

        public PlayerAssistUIMove(Canvas parentCanvas)
        {
            canvas = parentCanvas;
        }

        private Tween mainMoveTween = null;
        private Tween subMoveTween = null;

        private UIMoveTile moveTile = new UIMoveTile(1);

        /// <summary>
        /// アシストUI挙動関数
        /// </summary>
        /// <param name="num">表示したい対象子オブジェクトの要素数</param>
        public void Display(int num)
        {
            if(canvas.transform.GetChild(num).gameObject.activeSelf)
            {
                if(num == ObjectManager.TitleScene.PlayerData.PlayerDatas[0].Id)
                {
                    var tmpPos = new Vector3(
                        ObjectManager.Player[num].Object.transform.position.x,
                        canvas.transform.GetChild(num).position.y,
                        canvas.transform.GetChild(num).position.z
                    );

                    canvas.transform.GetChild(num).position = tmpPos;
                }
                else if(num == ObjectManager.TitleScene.PlayerData.PlayerDatas[1].Id)
                {
                    var tmpPos = new Vector3(
                        ObjectManager.Player[num].Object.transform.position.x,
                        canvas.transform.GetChild(num).position.y,
                        canvas.transform.GetChild(num).position.z
                    );

                    canvas.transform.GetChild(num).position = tmpPos;
                }
            }
        }

        /// <summary>
        /// アシスト挙動関数
        /// </summary>
        public void MainMovement(int num)
        {
            if(mainMoveTween == null && canvas.transform.GetChild(num).gameObject.activeSelf)
            {
                mainMoveTween = canvas.transform.GetChild(num).DOLocalMoveY(
                    canvas.transform.GetChild(num).localPosition.y + TitleConstants.ASSISTUI_MOVE_Y,
                    moveTile.Amount
                ).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).OnKill(() => mainMoveTween = null);
            }
            else if(!canvas.transform.GetChild(num).gameObject.activeSelf) 
                DOTween.Kill(canvas.transform.GetChild(num));
        }
    }

    
    /// <summary>
    /// 冷蔵庫UIの挙動クラス
    /// </summary>
    public class RefrigeratorUIMove
    {
        // キャンバス
        private Canvas canvas;
        // 種類テキストまとめオブジェクト
        private GameObject texts;
        private GameObject foodTexts;
        // カーソル
        private Image cursor;
        // 表示イメージまとめオブジェクト
        private GameObject images;

        // UIが動き終わるまでの時間
        private UIMoveTile moveTile = new UIMoveTile(1);

        // Tweenが動いているか
        private bool onPlayTween = false;

        // コンストラクタ
        public RefrigeratorUIMove(Canvas tmpCanvas)
        {
            canvas = tmpCanvas;
            texts = canvas.transform.GetChild(1).gameObject;
            cursor = canvas.transform.GetChild(2).GetComponent<Image>();
            images = canvas.transform.GetChild(3).gameObject;
            foodTexts = canvas.transform.GetChild(4).gameObject;
        }
        /// <summary>
        /// 各インプットイベント設定関数
        /// </summary>
        public void InputUISubscribe()
        {    
            cursorUp();
            cursorDown();
            decision();
        }

        /// <summary>
        /// カーソル上移動設定
        /// </summary>
        private void cursorUp()
        {
            ObjectManager.Events.KeyPressed
                // カーソルが表示されているときのみ判断
                .Where(_ => cursor.transform.parent.gameObject.activeSelf)
                .Where(x => (KeyCode)x == KeyCode.W || (KeyCode)x == KeyCode.UpArrow)
                .Subscribe(_ => {
                    upMove();
                });
        }


        /// <summary>
        /// カーソル上挙動実装関数
        /// </summary>
        private void upMove()
        {
            // カーソル音再生
            var sound = ObjectManager.TitleScene.AudioController.GetComponent<TitleSoundController>();
            sound.CursorSource.PlayOneShot(sound.AudioClipsList[(int)TitleSoundController.SoundPatternEnum.CURSOR_SE]);
            
            // 移動pos
            Vector3 movePos = Vector3.zero;
            // カーソルがどこにいるか判断して次のポイントに移動
            switch(cursor.transform.localPosition.y)
            {
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[0]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        TitleConstants.TEXT_CHILD_POS_Y[2],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[1]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        TitleConstants.TEXT_CHILD_POS_Y[0],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[2]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        TitleConstants.TEXT_CHILD_POS_Y[1],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                default:
                    break;
            }
            
        }


        /// <summary>
        /// カーソル下移動設定
        /// </summary>
        private void cursorDown()
        {
             ObjectManager.Events.KeyPressed
                // カーソルが表示されているときのみ判断
                .Where(_ => cursor.transform.parent.gameObject.activeSelf)
                .Where(x => (KeyCode)x == KeyCode.S || (KeyCode)x == KeyCode.DownArrow)
                .Subscribe(_ => {
                    downMove();
                });
        }

        /// <summary>
        /// カーソル下挙動実装関数
        /// </summary>
        private void downMove()
        {
            // カーソル音再生
            var sound = ObjectManager.TitleScene.AudioController.GetComponent<TitleSoundController>();
            sound.CursorSource.PlayOneShot(sound.AudioClipsList[(int)TitleSoundController.SoundPatternEnum.CURSOR_SE]);
            // 移動pos
            Vector3 movePos = Vector3.zero;
            // カーソルがどこにいるか判断して次のポイントに移動
            switch(cursor.transform.localPosition.y)
            {
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[0]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        TitleConstants.TEXT_CHILD_POS_Y[1],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[1]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        TitleConstants.TEXT_CHILD_POS_Y[2],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[2]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        TitleConstants.TEXT_CHILD_POS_Y[0],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// 決定挙動
        /// </summary>
        private void decision()
        {
            ObjectManager.Events.KeyPressed
                // キャンバスが表示されているときのみ判断
                .Where(_ => canvas.gameObject.activeSelf)
                .Where(x => (KeyCode)x == KeyCode.LeftShift || (KeyCode)x == KeyCode.RightShift)
                .Subscribe(_ => {
                    // カーソルが表示されていたら決定挙動
                    if(cursor.gameObject.activeSelf)
                        decisionMove();
                    // 非表示は初期化
                    else
                        resetUI();
                });
        }

        /// <summary>
        /// 決定挙動
        /// </summary>
        private void decisionMove()
        {
            // 決定音再生
            var sound = ObjectManager.TitleScene.AudioController.GetComponent<TitleSoundController>();
            sound.SelectSource.PlayOneShot(sound.AudioClipsList[(int)TitleSoundController.SoundPatternEnum.SELECT_SE]);

            // カーソルがどこにいるか判断して次のポイントに移動
            switch(cursor.transform.localPosition.y)
            {
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[0]:
                    changeUI("Meat");
                    break;
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[1]:
                    changeUI("Fish");
                    break;
                case float i when i == TitleConstants.TEXT_CHILD_POS_Y[2]:
                    changeUI("Veg");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 表示UI変更
        /// </summary>
        private void changeUI(string name)
        {
            // Tweenが動いていなかったら処理実行
            if(!onPlayTween)
            {    
                // Sequenceのインスタンスを作成
                var sequence = DOTween.Sequence();

                // カーソル非表示
                cursor.gameObject.SetActive(false);
                
                // Tween設定
                sequence.Append(texts.transform.DOLocalMoveX(-TitleConstants.UI_OUTCAMERA_POS_X, moveTile.Amount).SetEase(Ease.Linear));
                sequence.Join(images.transform.DOLocalMoveX(0, moveTile.Amount).SetEase(Ease.Linear));

                // 再生
                sequence.Play().OnStart(() => onPlayTween = true).OnComplete(() => 
                {
                    foodTexts.SetActive(true);
                    onPlayTween = false;
                });


                if(name == "Meat")
                    meatUISetting();
                else if(name == "Fish")
                    fishUISetting();
                else if(name == "Veg")
                    vagUISetting();
            }
        }

        /// <summary>
        /// UIリセット挙動
        /// </summary>
        private void resetUI()
        {
            // Tweenが動いていなかったら処理実行
            if(!onPlayTween)
            {    
                Debug.Log("Reset");

                // Sequenceのインスタンスを作成
                var sequence = DOTween.Sequence();

                
                // Tween設定
                sequence.Append(texts.transform.DOLocalMoveX(0, moveTile.Amount).SetEase(Ease.Linear)
                .OnComplete(() => cursor.gameObject.SetActive(true)));
                sequence.Join(images.transform.DOLocalMoveX(TitleConstants.UI_OUTCAMERA_POS_X, moveTile.Amount).SetEase(Ease.Linear));

                // 再生
                sequence.Play().OnStart(() => {
                    onPlayTween = true;
                    foodTexts.SetActive(false);
                    }).OnComplete(() => onPlayTween = false);
            }
        }
        /// <summary>
        /// 肉UI設定関数
        /// </summary>
        private void meatUISetting()
        {
            // イメージ設定
            setImage(ObjectManager.Ui.MeatImage);

            Debug.Log(TitleTextData.TextData[1][0]);

            // テキスト設定
            var textNum = 1;
            var tildNum = 0;
            setUIText(tildNum++, textNum++);
            setUIText(tildNum++, textNum++);
            setUIText(tildNum, textNum);
        }
        /// <summary>
        /// 魚UI設定関数
        /// </summary>
        private void fishUISetting()
        {
            // イメージ設定            
            setImage(ObjectManager.Ui.FishImage);

            
            // テキスト設定
            var textNum = 5;
            var tildNum = 0;
            setUIText(tildNum++, textNum++);
            setUIText(tildNum++, textNum++);
            setUIText(tildNum, textNum);
        }
        /// <summary>
        /// 野菜UI設定関数
        /// </summary>
        private void vagUISetting()
        {
            // イメージ設定
            setImage(ObjectManager.Ui.VagImage);

            // テキスト設定
            var textNum = 9;
            var tildNum = 0;
            setUIText(tildNum++, textNum++);
            setUIText(tildNum++, textNum++);
            setUIText(tildNum, textNum);
        }

        /// <summary>
        /// テキスト代入関数
        /// </summary>
        /// <param name="childIndex">代入テキストイメージの場所インデックス</param>
        /// <param name="index">取得行数</param>
        private void setUIText(int childIndex,int index)
        {
            // テキスト初期化
            foodTexts.transform.GetChild(childIndex).GetComponent<TextMeshProUGUI >().text = "";
            // テキスト代入処理
            foreach(string text in TitleTextData.TextData[index])
            {
                foodTexts.transform.GetChild(childIndex).GetComponent<TextMeshProUGUI >().text += text + "\n";
            }
        }

        /// <summary>
        /// イメージ差し替え関数
        /// </summary>
        /// <param name="spriteImages">セットするイメージのスプライト配列</param>
        private void setImage(Sprite[] spriteImages)
        {
            images.transform.GetChild(0).GetComponent<Image>().sprite = spriteImages[0];
            images.transform.GetChild(1).GetComponent<Image>().sprite = spriteImages[1];
            images.transform.GetChild(2).GetComponent<Image>().sprite = spriteImages[2];
        }
    }

}

