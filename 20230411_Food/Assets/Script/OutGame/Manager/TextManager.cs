using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Constants;
using OutGame;


namespace Title
{
    /// <summary>
    /// テキスト管理クラス
    /// </summary>
    public class TextManager
    {
        public TextMove Move{get; private set;}

        /// <summary>
        /// テキストイメージオブジェクトのRay
        /// </summary>
        public RaycastHit[] HitPlayerRay{get{return hitPlayerRay;} set{hitPlayerRay = value;}}
        private RaycastHit[] hitPlayerRay = new RaycastHit[3];

        public Vector3 StartScale{get; private set;}

        // コンストラクタ
        public TextManager(Vector3 objectScale)
        {
            Move = new TextMove();

            StartScale = objectScale;
        }

    }

    /// <summary>
    /// テキスト挙動管理クラス
    /// </summary>
    public class TextMove
    {
        
        // 接近移動時間
        private TextApproachMovementTime approachMovementTime = new TextApproachMovementTime(1);

        // テキストイメージのトランスフォーム
        private Image textImageObject;

        
        /// <summary>
        /// 食材一覧表示テキスト挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void FoodNicknamesTextMovement(float distination)
        {
            textImageObject = ObjectManager.TitleScene.TextImageCanvas[0];
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas[0].transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            // 上挙動
            sequence.Append(textImageObject.transform.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            {
                // 座標更新
                var tmpPos = textImageObject.transform.localPosition;
                tmpPos.y = TitleConstants.TEXT_IMAGE_APPROACH_POS_Y;
                textImageObject.transform.localPosition = tmpPos;

            }));

            // 拡大
            sequence.Join(textImageObject.transform.DOScale(
                Vector3.one,
                approachMovementTime.Amount
            )).SetEase(Ease.Linear);

            // アルファ値変更
            sequence.Join(textImageObject.DOFade(1, approachMovementTime.Amount)
            .SetEase(Ease.Linear));


            sequence.Play();
            
            //　座標目標値設定
            ObjectManager.Events.GameStartTextPoint.OnNext(TitleConstants.TEXT_IMAGE_LEAVE_POS_Y);
        }

        /// <summary>
        /// ゲームスタートテキストイメージ挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void GameStartTextMovement(float distination)
        {
            textImageObject = ObjectManager.TitleScene.TextImageCanvas[1];

            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas[1].transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            // 上挙動
            sequence.Append(textImageObject.transform.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            {
                // 座標更新
                var tmpPos = textImageObject.transform.localPosition;
                tmpPos.y = TitleConstants.TEXT_IMAGE_APPROACH_POS_Y;
                textImageObject.transform.localPosition = tmpPos;
            }));

            // 拡大
            sequence.Join(textImageObject.transform.DOScale(
                Vector3.one,
                approachMovementTime.Amount
            )).SetEase(Ease.Linear);

            // アルファ値変更
            sequence.Join(textImageObject.DOFade(1, approachMovementTime.Amount)
            .SetEase(Ease.Linear));

            sequence.Play();
            
        }
        /// <summary>
        /// 食材相性表示テキスト挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void DisplayIngredientsListTextMovement(float distination)
        {
            textImageObject = ObjectManager.TitleScene.TextImageCanvas[2];
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas[2].transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            // 上挙動
            sequence.Append(textImageObject.transform.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            {
                // 座標更新
                var tmpPos = textImageObject.transform.localPosition;
                tmpPos.y = TitleConstants.TEXT_IMAGE_APPROACH_POS_Y;
                textImageObject.transform.localPosition = tmpPos;
            }));

            // 拡大
            sequence.Join(textImageObject.transform.DOScale(
                Vector3.one,
                approachMovementTime.Amount
            )).SetEase(Ease.Linear);

            // アルファ値変更
            sequence.Join(textImageObject.DOFade(1, approachMovementTime.Amount)
            .SetEase(Ease.Linear));


            sequence.Play();
            
        }
        /// <summary>
        /// テキストイメージ初期化挙動処理
        /// </summary>
        /// <param name="resetImage">テキストイメージ</param>
        public void ResetTextMovement(Image resetImage)
        {
            // Posが指定の座標と同じじゃなければ移動処理
            if(resetImage.transform.localPosition.y == TitleConstants.TEXT_IMAGE_APPROACH_POS_Y)
            {
                 // Sequenceのインスタンスを作成
                    var sequence = DOTween.Sequence();

                    // 下挙動
                    sequence.Append(
                        resetImage.transform.DOLocalMoveY(
                            TitleConstants.TEXT_IMAGE_LEAVE_POS_Y,
                            approachMovementTime.Amount
                        ).SetEase(Ease.Linear).OnStart(() => {
                            Debug.Log("ResetTextMovement");
                            }).OnComplete(() => {
                            // 座標調整
                            var tmpPos = resetImage.transform.localPosition;
                            tmpPos.y = TitleConstants.TEXT_IMAGE_LEAVE_POS_Y;
                            resetImage.transform.localPosition = tmpPos;
                        }));

                    // 縮小
                    sequence.Join(resetImage.transform.DOScale(
                        ObjectManager.Text.StartScale,
                        approachMovementTime.Amount
                    )).SetEase(Ease.Linear);

                    // アルファ値変更
                    sequence.Join(resetImage.DOFade(0.5f, approachMovementTime.Amount)
                    .SetEase(Ease.Linear));

                    sequence.Play();
            }
        }
    }
}