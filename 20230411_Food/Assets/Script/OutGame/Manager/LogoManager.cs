using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace Logo
{
    public class LogoManager : MonoBehaviour
    {
        [SerializeField]
        private Canvas logoCanvas;
        
        private LogoMove move;
        // Start is called before the first frame update
        void Start()
        {
            move = new LogoMove(logoCanvas.transform.GetChild(1).GetComponent<Image>());
            move.Movement();
        }
    }

    public class LogoMove
    {
        private Image logo;
        private Vector3 startScale;
        public LogoMove(Image logoImg)
        {
            logo = logoImg;
            startScale = logo.transform.localScale;
        }
        public void Movement()
        {
            var sequence = DOTween.Sequence();
            sequence.SetLink(logo.gameObject);

            sequence.Append(logo.transform.DOLocalJump(Vector3.zero, 100, 5, 2).
                SetEase(Ease.Linear).OnComplete(() =>{
                //logo.transform.parent.GetChild(3).gameObject.SetActive(true);
            }));

            sequence.AppendInterval(0.3f);
            sequence.Append(logo.transform.DOScale(Vector3.one,1).
                SetEase(Ease.Linear).OnComplete(() =>
                logo.transform.DOScale(startScale,1).
                SetEase(Ease.Linear)));

            // sequence.Append(logo.transform.DOScale(Vector3.one * 2.3f,2).SetEase(Ease.Linear).OnStart(() =>{
            //     logo.transform.parent.GetChild(3).gameObject.SetActive(false);
            //     logo.transform.parent.GetChild(2).gameObject.SetActive(true);

            // }));
            
            sequence.Join(logo.transform.DORotate(new Vector3(0, 0, 360), 1, RotateMode.FastBeyond360)
            .SetLoops(2, LoopType.Restart).SetEase(Ease.Linear));

            sequence.Append(logo.transform.parent.GetChild(4).GetComponent<Image>().DOFade(1,2).
                SetEase(Ease.Linear).OnComplete(() => SceneManager.LoadScene("TitleScene")));
        }
    }
}

