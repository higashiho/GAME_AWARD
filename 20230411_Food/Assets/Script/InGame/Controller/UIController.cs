using System.Collections;
using System.Threading;
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
        [SerializeField]
        private TextMeshProUGUI timerText;
        public TextMeshProUGUI TitmerText{get => timerText;}
        // Start is called before the first frame update
        async void Start()
        {
            await UniTask.WaitWhile(() => ObjectManager.GameManager == null);

            uiMove = new UIMove(this.transform);

            // 空腹量増加
            uiMove.FoodSaturation();
            // お題テキスト
            uiMove.FoodText();

            setSubscribe();
        }
        // 購読設定
        private void setSubscribe()
        {
            this.UpdateAsObservable()
                .Where(_ => cutInFlag && this.transform.GetChild(1).gameObject.activeSelf)
                .Subscribe(_ =>{
                    cutInFlag = false;
                    
                    var panelImage = cutInCanvas.transform.GetChild(0).GetComponent<RawImage>();
                    var gageObj = cutInCanvas.transform.GetChild(1);
                    panelImage.DOFade(0,1).SetEase(Ease.Linear)
                        .SetLink(this.gameObject).OnComplete(() => {
                            Destroy(GameObject.FindWithTag("GameMastar"));
                            uiMove.Countdown();
                            panelImage.gameObject.SetActive(false);
                            }).OnUpdate(() => {
                            fadeImages(panelImage, gageObj);
                        });
                }).AddTo(this.gameObject);
        }
        /// <summary>
        /// UIフェイドアウト処理
        /// </summary>
        /// <param name="panelImage">cut-inの全体イメージ親オブジェクト</param>
        /// <param name="gageObj">ゲージのイメージ親オブジェクト</param>
        private void fadeImages(RawImage panelImage, Transform gageObj)
        {
            panelImage.transform.GetChild(0).GetComponent<Image>().color = panelImage.color;
            panelImage.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = panelImage.color;
            gageObj.GetChild(0).GetComponent<Image>().color = panelImage.color;
            gageObj.GetChild(1).GetChild(0).GetComponent<RawImage>().color = panelImage.color;
            gageObj.GetChild(2).GetComponent<TextMeshProUGUI>().color = panelImage.color;
            gageObj.GetChild(3).GetComponent<TextMeshProUGUI>().color = panelImage.color;
        }
        private void OnDestroy()
        {
            uiMove.Cts.Cancel();
        }
    }

    public class UIMove
    {
        private GameObject foodSaturationsObj;
        private TextMeshProUGUI countdownText;
        private RectMask2D gageMask;
        private TextMeshProUGUI foodItemsText;

        public static int FoodSaturationAmount{get; private set;}
        public CancellationTokenSource Cts{get;} = new CancellationTokenSource();
        public UIMove(Transform obj)
        {
            foodSaturationsObj = obj.GetChild(0).GetChild(1).gameObject;
            countdownText = obj.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            gageMask = foodSaturationsObj.transform.GetChild(1).GetComponent<RectMask2D>();
            foodItemsText = obj.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// Countdown挙動管理メソッド
        /// </summary>
        /// <returns></returns>
        public void Countdown()
        {
            if(!ObjectManager.GameManager.UIController.TitmerText.transform.parent.gameObject.activeSelf)
            {
                ObjectManager.GameManager.UIController.TitmerText.transform.parent.parent.DOLocalMoveY(420,2).SetEase(Ease.Linear).
                    OnStart(() => {
                            var num = InGame.DecideTheRecipe.RecipeIndex;
    
                            var tmpText = ObjectManager.GameManager.UIController.TitmerText.transform.parent.parent.parent.GetChild(2).GetComponent<TextMeshProUGUI>();
                            
                            tmpText.text += "\n肉・野・魚\n " + ObjectManager.GameManager.FoodData.FoodThemes[num].TargetRate[0] + "・" 
                            + ObjectManager.GameManager.FoodData.FoodThemes[num].TargetRate[1] + "・"
                            + ObjectManager.GameManager.FoodData.FoodThemes[num].TargetRate[2];

                            tmpText.DOFade(1,2).SetEase(Ease.Linear).SetLink(tmpText.gameObject);
                            ObjectManager.GameManager.UIController.TitmerText.transform.parent.parent.parent.GetChild(1).GetComponent<Image>().DOFade(1,2).SetEase(Ease.Linear).SetLink(tmpText.gameObject);
                            
                            ObjectManager.GameManager.UIController.TitmerText.transform.parent.gameObject.SetActive(true);
                        });

            }
            countdownAsync();
        }

        /// <summary>
        /// 空腹ゲージ挙動管理メソッド
        /// </summary>
        /// <returns></returns>
        public async void FoodSaturation()
        {
            await foodSaturationAsync();
        }

        /// <summary>
        /// お題テキスト挙動管理メソッド
        /// </summary>
        /// <returns></returns>
        public async void FoodText()
        {
            await foodTextAsync();
        }

        /// <summary>
        /// Countdown挙動メソッド
        /// </summary>
        /// <returns></returns>
        private async void countdownAsync()
        {
            await changeCountdonwText("3");

            await changeCountdonwText("2");

            await changeCountdonwText("1");

            await changeCountdonwText("go!!");
            ObjectManager.GameManager.ChangeState();

            countdownText.transform.parent.gameObject.SetActive(false);
        }

        /// <summary>
        /// テキスト変更メソッド
        /// </summary>
        /// <param name="str">変更したいstring</param>
        /// <returns></returns>
        private async UniTask changeCountdonwText(string str)
        {
            countdownText.text = str;
            await UniTask.Delay(1000);
        }

        /// <summary>
        /// 空腹ゲージ増加メソッド
        /// </summary>
        /// <returns></returns>
        private async UniTask foodSaturationAsync()
        {
            var gagePadding = gageMask.padding;

            // Paddingが減算のため割合は逆値になる
            var targetRate = UnityEngine.Random.Range(0, 80);
            var targetPaddingAmount = (gageMask.padding.w / 100) * targetRate;

             // ゲージ減算ループ
            while(!Cts.Token.IsCancellationRequested || gageMask.transform.parent.gameObject.activeSelf)
            {
                
                // 指定値以下の場合は
                if(gagePadding.w > targetPaddingAmount)
                    gagePadding.w -= gageMask.rectTransform.sizeDelta.y / 200f;
                
                // 代入
                gageMask.padding = gagePadding;

                // ゲージが指定値以下になったら処理終了
                if(gagePadding.w <= targetPaddingAmount)
                {
                    // ずれ修正
                    gagePadding.w = targetPaddingAmount;

                    // 代入
                    gageMask.padding = gagePadding;

                    break;
                }

                // 1ミリ秒待つ
                await UniTask.Delay(1);
            }

            // テキスト変更
            var saturationText = foodSaturationsObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

            FoodSaturationAmount = (100 - (int)((targetPaddingAmount / gageMask.rectTransform.sizeDelta.y) * 100));
            saturationText.text = "空腹度 : " + FoodSaturationAmount;
            
            // 二秒待ってステート更新
            await UniTask.Delay(2000);
            ObjectManager.GameManager.ChangeState();
        }
        
        /// <summary>
        /// お題テキスト変更メソッド
        /// </summary>
        /// <returns></returns>
        private async UniTask foodTextAsync()
        {
            string[] wordArray; 
            wordArray = ObjectManager.GameManager.FoodThemeData.FoodThemes[DecideTheRecipe.RecipeIndex].InstanceAddress.Split(',');

            foreach(var tmpStr in wordArray)
            {
                foodItemsText.text += tmpStr;
                await UniTask.Delay(100);
            }
        }
    }
}

