using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

using Player;
using System;
using System.Linq;

namespace FollowCamera
{
    public class FollowingCameraManager 
    {
        private UniTask? loadTask = null;

        private List<PlayerManager> player;
        private GameObject followCamera;
        //private GameObject[] followCameras;
        private List<GameObject> followCameras;
        
        private float offsetZ = 4f;
        private float offsetX = 4f;

        public UnityAction ReleaseHandleEvent{get; private set;}

        
 

        /// <summary>
        /// カメラプレファブをロードするメソッド
        /// </summary>
        /// <returns></returns>
        private async UniTask load()
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>("FollowingCamera", null);
            
            await handle.Task;
            int count = GameManager.ObjectManager.PlayerManagers.Count;
            //followCameras = new GameObject[count];
            followCameras = new List<GameObject>(count);
            
            foreach(var item in handle.Result)
            {
                followCameras.Add(MonoBehaviour.Instantiate(item));
            }

            void releaseCameraHandle()
            {
                Addressables.Release(handle);
            }

            ReleaseHandleEvent = releaseCameraHandle;
        }

        /// <summary>
        /// 追従先を設定するメソッド
        /// </summary>
        /// <param name="player">追従するプレイヤー</param>
        public void SetFollowingPlayer(List<PlayerManager> player)
        {
            this.player = player;
            
        }

        /// <summary>
        /// 追従カメラ管理クラス初期化メソッド
        /// カメラの生成もここで行う
        /// </summary>
        public async void Init()
        {
            loadTask = load();
            await (UniTask)loadTask;

            int count = GameManager.ObjectManager.PlayerManagers.Count;

            for(int i = 0; i < count; i++)
            {
                // viewPort設定
                followCameras[i].GetComponent<Camera>().rect = new Rect(i * 0.5f, 0, (i + 1) * 0.5f, 1);
                
                
                // カメラをそれぞれ担当のプレイヤーの子にセット
                followCameras[i].transform.parent = GameManager.ObjectManager.PlayerManagers[i].Object.transform;


                // カメラの座標調整
                followCameras[i].transform.position = 
                GameManager.ObjectManager.PlayerManagers[i].Object.transform.position + 
                setOffset(followCameras[i].transform.parent.eulerAngles.y);

            }
        }
        
        /// <summary>
        /// カメラのオフセットを設定するメソッド
        /// プレイヤーが増えた場合カメラのオフセットの場合分けもここに増やす
        /// </summary>
        /// <param name="targetVectorY">プレイヤーのベクトル</param>
        /// <returns>オフセット</returns>
        private Vector3 setOffset(float targetVectorY)
        {
            var offset = new Vector3(0, 6f, 0);
            switch(targetVectorY)
            {
                // 0°
                case 0:
                    offset.z = -offsetZ;
                    break;
                
                // 90°
                case 90:
                    offset.x = offsetX;
                    break;

                // 180°
                case 180:
                    offset.z = offsetZ;
                    break;

                // 270°
                case 270:
                    offset.x = -offsetX;
                    break;
                
                default:
                    break;
                
            }
            return  offset;
        }

        
    }
}

