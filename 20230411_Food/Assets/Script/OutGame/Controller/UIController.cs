using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using Constants;
using DG.Tweening;
using TMPro;

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
        // インスタンス化
        private RefrigeratorUIMove refrigeratorUIMove;
        // Start is called before the first frame update
        async void Start()
        {
            // インスタンス化
            refrigeratorUIMove = new RefrigeratorUIMove(RefrigeratorCanvas);
            ObjectManager.Ui = this;

            // インプットイベントがインスタンス化されるまで待つ
            await UniTask.WaitWhile(() => ObjectManager.InputEvent == null);
            // イベント設定
            refrigeratorUIMove.InputUIEvent();
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
        public void InputUIEvent()
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
            ObjectManager.InputEvent.KeyPressed
                // カーソルが表示されているときのみ判断
                .Where(_ => cursor.gameObject.activeSelf)
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
            // 移動pos
            Vector3 movePos = Vector3.zero;
            // カーソルがどこにいるか判断して次のポイントに移動
            switch(cursor.transform.localPosition.y)
            {
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[0]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        OutGameConstants.TEXT_CHILD_POS_Y[2],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[1]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        OutGameConstants.TEXT_CHILD_POS_Y[0],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[2]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        OutGameConstants.TEXT_CHILD_POS_Y[1],
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
             ObjectManager.InputEvent.KeyPressed
                // カーソルが表示されているときのみ判断
                .Where(_ => cursor.gameObject.activeSelf)
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
            // 移動pos
            Vector3 movePos = Vector3.zero;
            // カーソルがどこにいるか判断して次のポイントに移動
            switch(cursor.transform.localPosition.y)
            {
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[0]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        OutGameConstants.TEXT_CHILD_POS_Y[1],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[1]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        OutGameConstants.TEXT_CHILD_POS_Y[2],
                        cursor.transform.localPosition.z
                    );
                    cursor.transform.localPosition = movePos;
                    break;
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[2]:
                    movePos = new Vector3
                    (
                        cursor.transform.localPosition.x,
                        OutGameConstants.TEXT_CHILD_POS_Y[0],
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
            ObjectManager.InputEvent.KeyPressed
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

            // カーソルがどこにいるか判断して次のポイントに移動
            switch(cursor.transform.localPosition.y)
            {
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[0]:
                    changeUI("Meat");
                    break;
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[1]:
                    changeUI("Fish");
                    break;
                case float i when i == OutGameConstants.TEXT_CHILD_POS_Y[2]:
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
                sequence.Append(texts.transform.DOLocalMoveX(-OutGameConstants.UI_OUTCAMERA_POS_X, moveTile.Amount).SetEase(Ease.Linear));
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
                sequence.Join(images.transform.DOLocalMoveX(OutGameConstants.UI_OUTCAMERA_POS_X, moveTile.Amount).SetEase(Ease.Linear));

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
            images.transform.GetChild(0).GetComponent<Image>().sprite = ObjectManager.Ui.MeatImage[0];
            images.transform.GetChild(1).GetComponent<Image>().sprite = ObjectManager.Ui.MeatImage[1];
            images.transform.GetChild(2).GetComponent<Image>().sprite = ObjectManager.Ui.MeatImage[2];

            // テキスト設定
            foodTexts.transform.GetChild(0).GetComponent<TextMeshProUGUI >().text = "meat";
            foodTexts.transform.GetChild(1).GetComponent<TextMeshProUGUI >().text = "meat";
            foodTexts.transform.GetChild(2).GetComponent<TextMeshProUGUI >().text = "meat";
        }
        /// <summary>
        /// 魚UI設定関数
        /// </summary>
        private void fishUISetting()
        {
            // イメージ設定
            images.transform.GetChild(0).GetComponent<Image>().sprite = ObjectManager.Ui.FishImage[0];
            images.transform.GetChild(1).GetComponent<Image>().sprite = ObjectManager.Ui.FishImage[1];
            images.transform.GetChild(2).GetComponent<Image>().sprite = ObjectManager.Ui.FishImage[2];
            
            // テキスト設定
            foodTexts.transform.GetChild(0).GetComponent<TextMeshProUGUI >().text = "fish";
            foodTexts.transform.GetChild(1).GetComponent<TextMeshProUGUI >().text = "fhis";
            foodTexts.transform.GetChild(2).GetComponent<TextMeshProUGUI >().text = "fish";
        }
        /// <summary>
        /// 野菜UI設定関数
        /// </summary>
        private void vagUISetting()
        {
            // イメージ設定
            images.transform.GetChild(0).GetComponent<Image>().sprite = ObjectManager.Ui.VagImage[0];
            images.transform.GetChild(1).GetComponent<Image>().sprite = ObjectManager.Ui.VagImage[1];
            images.transform.GetChild(2).GetComponent<Image>().sprite = ObjectManager.Ui.VagImage[2];

            // テキスト設定
            foodTexts.transform.GetChild(0).GetComponent<TextMeshProUGUI >().text = "vag";
            foodTexts.transform.GetChild(1).GetComponent<TextMeshProUGUI >().text = "vag";
            foodTexts.transform.GetChild(2).GetComponent<TextMeshProUGUI >().text = "vag";
        }
    }

}

