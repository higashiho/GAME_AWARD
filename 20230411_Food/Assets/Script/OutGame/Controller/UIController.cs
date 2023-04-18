using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using Constants;

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
        
        // インスタンス化
        private RecipeBookUIMove recipeBookUIMove;
        private RefrigeratorUIMove refrigeratorUIMove;
        // Start is called before the first frame update
        async void Start()
        {
            await UniTask.WaitWhile(() => ObjectManager.InputEvent == null);
            // インスタンス化
            recipeBookUIMove = new RecipeBookUIMove();
            refrigeratorUIMove = new RefrigeratorUIMove(RefrigeratorCanvas);
            // イベント設定
            recipeBookUIMove.InputUIEvent();
            refrigeratorUIMove.InputUIEvent();
        }

    }

    /// <summary>
    /// レシピブックUIの挙動クラス
    /// </summary>
    public class RecipeBookUIMove
    {

        /// <summary>
        /// 各インプットイベント設定関数
        /// </summary>
        public void InputUIEvent()
        {
           cursorUp();
           cursorLeft();
           cursorDown();
           cursorRight();
           
        }

        /// <summary>
        /// カーソル上移動
        /// </summary>
        private void cursorUp()
        {
            ObjectManager.InputEvent.KeyPressed
                .Where(x => (KeyCode)x == KeyCode.W || (KeyCode)x == KeyCode.UpArrow)
                .DistinctUntilChanged()
                .Subscribe(_ => {
                    
                });
        }

        /// <summary>
        /// カーソル左移動
        /// </summary>
        private void cursorLeft()
        {
            ObjectManager.InputEvent.KeyPressed
                .Where(x => (KeyCode)x == KeyCode.A || (KeyCode)x == KeyCode.LeftArrow)
                .DistinctUntilChanged()
                .Subscribe(_ => {
                    
                });
        }

        /// <summary>
        /// カーソル下移動
        /// </summary>
        private void cursorDown()
        {
             ObjectManager.InputEvent.KeyPressed
                .Where(x => (KeyCode)x == KeyCode.S || (KeyCode)x == KeyCode.DownArrow)
                .DistinctUntilChanged()
                .Subscribe(_ => {
                    
                });
        }

        /// <summary>
        /// カーソル右移動
        /// </summary>
        private void cursorRight()
        {
            ObjectManager.InputEvent.KeyPressed
                .Where(x => (KeyCode)x == KeyCode.D || (KeyCode)x == KeyCode.RightArrow)
                .DistinctUntilChanged()
                .Subscribe(_ => {
                    
                });
        }

        /// <summary>
        /// 決定挙動
        /// </summary>
        private void decision()
        {
            ObjectManager.InputEvent.KeyPressed
                .Where(x => (KeyCode)x == KeyCode.LeftShift || (KeyCode)x == KeyCode.RightShift)
                .DistinctUntilChanged()
                .Subscribe(_ => {

                });
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
        // カーソル
        private Image cursor;
        // 表示イメージまとめオブジェクト
        private GameObject images;

        // コンストラクタ
        public RefrigeratorUIMove(Canvas tmpCanvas)
        {
            canvas = tmpCanvas;
            texts = canvas.transform.GetChild(1).gameObject;
            cursor = canvas.transform.GetChild(2).GetComponent<Image>();
            images = canvas.transform.GetChild(3).gameObject;
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
            texts.SetActive(false);
            cursor.gameObject.SetActive(false);
            images.SetActive(true);

            if(name == "Meat")
                Debug.Log("Meatを表示");
            else if(name == "Fish")
                Debug.Log("Fishを表示");
            else if(name == "Veg")
                Debug.Log("Vegを表示");
        }

        private void resetUI()
        {
            Debug.Log("Reset");
            texts.SetActive(true);
            cursor.gameObject.SetActive(true);
            images.SetActive(false);
        }
    }

}

