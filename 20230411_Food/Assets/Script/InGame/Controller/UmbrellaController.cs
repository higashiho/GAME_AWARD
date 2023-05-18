using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Umbrella
{
    public class UmbrellaController : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            setSubscribe();
        }

        /// <summary>
        /// 購読設定
        /// </summary>
        private void setSubscribe()
        {
            this.UpdateAsObservable()
                .Where(_ => GameManager.ObjectManager.PlayerManagers[0].FoodPoint.Move.RayController.RayHitObjectFood)
                .Subscribe(_ =>
                {

                });
            this.UpdateAsObservable()
                .Where(_ => GameManager.ObjectManager.PlayerManagers[1].FoodPoint.Move.RayController.RayHitObjectFood)
                .Subscribe(_ =>
                {

                });
        }
    }
}

