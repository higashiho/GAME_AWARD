using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Constants;


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

        // コンストラクタ
        public TextManager()
        {
            Move = new TextMove();
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
        private Transform textImageObject;
        
        /// <summary>
        /// 食材一覧表示テキスト挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void FoodNicknamesTextMovement(float distination)
        {
            textImageObject = ObjectManager.TitleScene.TextImageCanvas[0].transform;
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas[0].transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            textImageObject.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            {
                // 座標更新
                var tmpPos = textImageObject.localPosition;
                tmpPos.y = OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y;
                textImageObject.localPosition = tmpPos;
            });

            sequence.Play();
            
            //　座標目標値設定
            ObjectManager.TitleScene.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y);
        }

        /// <summary>
        /// 食材相性表示テキスト挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void DisplayIngredientsListTextMovement(float distination)
        {
            textImageObject = ObjectManager.TitleScene.TextImageCanvas[1].transform;
            Debug.Log("Play");
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas[1].transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            textImageObject.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            {
                // 座標更新
                var tmpPos = textImageObject.localPosition;
                tmpPos.y = OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y;
                textImageObject.localPosition = tmpPos;
            });

            sequence.Play();
            
            //　座標目標値設定
            ObjectManager.TitleScene.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y);
        }

        /// <summary>
        /// ゲームスタートテキストイメージ挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void GameStartTextMovement(float distination)
        {
            textImageObject = ObjectManager.TitleScene.TextImageCanvas[2].transform;

            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas[2].transform);

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            textImageObject.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            {
                // 座標更新
                var tmpPos = textImageObject.localPosition;
                tmpPos.y = OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y;
                textImageObject.localPosition = tmpPos;
            });

            sequence.Play();
            
            //　座標目標値設定
            ObjectManager.TitleScene.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y);
        }
        /// <summary>
        /// テキストイメージ初期化挙動処理
        /// </summary>
        /// <param name="resetImage">テキストイメージ</param>
        public void ResetTextMovement(Image resetImage)
        {
            // TODO : 引数オブジェクトの座標で元に戻すテキストイメージを判断して実行
            // Posが指定の座標と同じじゃなければ移動処理
            if(resetImage.transform.localPosition.y != OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y)
            {
                // 挙動
                resetImage.transform.DOLocalMoveY(
                    OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y,
                    approachMovementTime.Amount
                ).SetEase(Ease.Linear).OnStart(() => Debug.Log("ResetTextMovement")).OnComplete(() => {
                    // 座標調整
                    var tmpPos = resetImage.transform.localPosition;
                    tmpPos.y = OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y;
                    resetImage.transform.localPosition = tmpPos;
                });
            }
            // 目標座標更新
            ObjectManager.TitleScene.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
        }
    }
}