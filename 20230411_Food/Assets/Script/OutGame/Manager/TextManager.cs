using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


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
        private TextApproachValue approachValue = new TextApproachValue(10);
        private TextApproachMovementTime approachMovementTime = new TextApproachMovementTime(1);
        
        /// <summary>
        /// 食材一覧表示テキスト挙動処理
        /// </summary>
        public void FoodNicknamesTextMovement()
        {
            Debug.Log("Play");
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(0));

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(0).DOMoveY(
                approachValue.TextApproachAmount,
                approachMovementTime.TextApproachMovementTimeAmount
            ).SetEase(Ease.Linear);

            sequence.Play();
        }

        /// <summary>
        /// 食材相性表示テキスト挙動処理
        /// </summary>
        public void DisplayIngredientsListTextMovement()
        {
            Debug.Log("Play");
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(1));

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(1).DOMoveY(
                approachValue.TextApproachAmount,
                approachMovementTime.TextApproachMovementTimeAmount
            ).SetEase(Ease.Linear);

            sequence.Play();
        }

        /// <summary>
        /// ゲームスタートテキストイメージ挙動処理
        /// </summary>
        public void GameStartTextMovement()
        {
            Debug.Log("Play");
            // ImageのTweenを削除
            DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(2));

            // Sequenceのインスタンスを作成
            var sequence = DOTween.Sequence();

            ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(2).DOMoveY(
                approachValue.TextApproachAmount,
                approachMovementTime.TextApproachMovementTimeAmount
            ).SetEase(Ease.Linear);

            sequence.Play();
        }
        /// <summary>
        /// テキストイメージ初期化挙動処理
        /// </summary>
        /// <param name="tmpPlayerObject">Playerオブジェクト</param>
        public void ResetTextMovement(GameObject tmpPlayerObject)
        {
            // TODO : 引数オブジェクトの座標で元に戻すテキストイメージを判断して実行

            // // ImageのTweenを削除
            // DOTween.Kill(ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(0));

            // // Sequenceのインスタンスを作成
            // var sequence = DOTween.Sequence();

            // ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(0).DOMoveY(
            //     ObjectManager.TitleScene.TextImageCanvas.transform.GetChild(0).position.y + approachValue.TextApproachAmount,
            //     approachMovementTime.TextApproachMovementTimeAmount
            // ).SetEase(Ease.Linear);

            // sequence.Play();
        }
    }
}