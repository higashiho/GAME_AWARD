using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Umbrella
{
    public class UmbrellaController : MonoBehaviour
    {
        public Subject<GameObject> hitObjectSubject{get;} = new Subject<GameObject>();

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
            hitObjectSubject.Where(x => x)
                .Subscribe(x =>
                {
                    Debug.Log(x);
                    this.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(1,1,1,0.5f));
                }).AddTo(this.gameObject);

            hitObjectSubject.Where(x => !x)
                .Subscribe(x =>
                {
                    Debug.Log(x);
                    this.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(1,1,1,1));
                }).AddTo(this.gameObject);
        }
    }
}

