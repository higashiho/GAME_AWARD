using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            textImageObject = ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(0);
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(0));

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            textImageObject.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            ObjectManager.TitleScene.InputEvent.FoodNicknamesTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y));

            sequence.Play();
        }

        /// <summary>
        /// 食材相性表示テキスト挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void DisplayIngredientsListTextMovement(float distination)
        {
            textImageObject = ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(1);
            Debug.Log("Play");
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(1));

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            textImageObject.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            ObjectManager.TitleScene.InputEvent.DisplayIngredientsListTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y));

            sequence.Play();
        }

        /// <summary>
        /// ゲームスタートテキストイメージ挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void GameStartTextMovement(float distination)
        {
            textImageObject = ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(2);

            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(2));

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            textImageObject.DOLocalMoveY(
                distination,
                approachMovementTime.Amount
            ).SetEase(Ease.Linear).OnComplete(() => 
            ObjectManager.TitleScene.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_LEAVE_POS_Y));

            sequence.Play();
        }
        /// <summary>
        /// テキストイメージ初期化挙動処理
        /// </summary>
        /// <param name="distination">テキストイメージ目標座標</param>
        public void ResetTextMovement(float distination)
        {
            // TODO : 引数オブジェクトの座標で元に戻すテキストイメージを判断して実行
            // 目標座標更新
            ObjectManager.TitleScene.InputEvent.GameStartTextPoint.OnNext(OutGameConstants.TEXT_IMAGE_APPROACH_POS_Y);
        }
    }
}