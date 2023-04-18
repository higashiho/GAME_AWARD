using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Title
{
    /// <summary>
    /// UI挙動管理クラス
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [SerializeField, Header("レシピブックのキャンバス")]
        private Canvas recipeBookChanvas;
        [SerializeField, Header("冷蔵庫のキャンバス")]
        private Canvas refrigeratorCanvas;
        
        private RecipeBookUIMove recipeBookUIMove = new RecipeBookUIMove();
        private RefrigeratorUIMove refrigeratorUIMove = new RefrigeratorUIMove();
        // Start is called before the first frame update
        async void Start()
        {
            await UniTask.WaitWhile(() => ObjectManager.InputEvent == null);

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
    }

    /// <summary>
    /// 冷蔵庫UIの挙動クラス
    /// </summary>
    public class RefrigeratorUIMove
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
    }

}

