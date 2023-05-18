using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


 

        /// <summary>
        /// カメラプレファブをロードするメソッド
        /// </summary>
        /// <returns></returns>
        private async UniTask load()
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>("FollowingCamera", null);
            
            await handle.Task;
            int count = player.Count;
            //followCameras = new GameObject[count];
            followCameras = new List<GameObject>(count);
            
            foreach(var item in handle.Result)
            {
                followCameras.Add(MonoBehaviour.Instantiate(item));
            }
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
            if(loadTask == null)
            {
                loadTask = load();
                await (UniTask)loadTask;

                int count = player.Count;

                for(int i = 0; i < count; i++)
                {
                    // viewPort設定
                    followCameras[i].GetComponent<Camera>().rect = new Rect(i * 0.5f, 0, (i + 1) * 0.5f, 1);
                    
                   
                    // カメラをそれぞれ担当のプレイヤーの子にセット
                    followCameras[i].transform.parent = player[i].FoodPoint.Move.RayController.Object.transform;

                    // ベクトルの判断、オフセット変更

                    // カメラの座標調整
                    followCameras[i].transform.position = 
                    player[i].FoodPoint.Move.RayController.Object.transform.position + 
                    setOffset(followCameras[i].transform.parent.eulerAngles.y);
                    
                    // 視野角調整
                    //followCameras[i].transform.localEulerAngles = new Vector3(30, 0, 0);
                }


            }  
        }

        // プレイヤーのベクトル取得
        
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

