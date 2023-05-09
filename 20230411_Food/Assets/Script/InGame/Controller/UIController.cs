using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using TMPro;

using GameManager;

namespace InGame
{
    public class UIController : MonoBehaviour
    {
        private bool cutInFlag = true;
        private CountDownMove countDownMove;

        
        // Start is called before the first frame update
        void Start()
        {
            countDownMove = new CountDownMove(this.gameObject);
            this.UpdateAsObservable()
                .Where(_ => cutInFlag && this.gameObject.activeSelf)
                .Subscribe(_ =>{
                    cutInFlag = false;
                    countDownMove.Movement();
                }).AddTo(this.gameObject);
        }

    }

    public class CountDownMove
    {
        private TextMeshProUGUI countdownText;
        public CountDownMove(GameObject obj)
        {
            countdownText = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// Countdown挙動メソッド
        /// </summary>
        /// <returns></returns>
        public async void Movement()
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

