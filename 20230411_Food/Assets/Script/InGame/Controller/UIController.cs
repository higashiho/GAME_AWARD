using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;

using GameManager;

namespace InGame
{
    public class UIController : MonoBehaviour
    {
        private bool cutInFlag = true;
        private UIMove uiMove;

        [SerializeField]
        private Canvas cutInCanvas = default;

        
        // Start is called before the first frame update
        void Start()
        {
            uiMove = new UIMove(this.transform.GetChild(1));
            this.UpdateAsObservable()
                .Where(_ => cutInFlag && this.transform.GetChild(1).gameObject.activeSelf)
                .Subscribe(_ =>{
                    cutInFlag = false;
                    
                    var panelImage = cutInCanvas.transform.GetChild(0).GetComponent<RawImage>();
                    panelImage.DOFade(0,1).SetEase(Ease.Linear)
                        .SetLink(this.gameObject).OnComplete(() => {
                            uiMove.Movement();
                            panelImage.gameObject.SetActive(false);
                            }).OnUpdate(() => {
                            panelImage.transform.GetChild(0).GetComponent<Image>().color = panelImage.color;
                            panelImage.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = panelImage.color;
                        });
                }).AddTo(this.gameObject);
        }

    }

    public class UIMove
    {
        private TextMeshProUGUI countdownText;
        public UIMove(Transform obj)
        {
            countdownText = obj.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// UI全体挙動メソッド
        /// </summary>
        /// <returns></returns>
        public void Movement()
        {
            countdownAsync();
        }

        /// <summary>
        /// Countdown挙動メソッド
        /// </summary>
        /// <returns></returns>
        private async void countdownAsync()
        {
            await changeText("3");

            await changeText("2");

            await changeText("1");

            await changeText("go!!");
            ObjectManager.GameManager.ChangeState();

            countdownText.transform.parent.gameObject.SetActive(false);
        }

        /// <summary>
        /// テキスト変更メソッド
        /// </summary>
        /// <param name="str">変更したいstring</param>
        /// <returns></returns>
        private async UniTask changeText(string str)
        {
            countdownText.text = str;
            await UniTask.Delay(1000);
        }
    }
}

