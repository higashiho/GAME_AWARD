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
        private float offsetY = 7f;
        //private float offsetZ = -3f;
        private float[] offsetZ;

        // カメラの回転状態
        public enum CameraRotate
        {
            NORMAL,
            REVERSE
        }
        public CameraRotate RotatePhase{get; private set;}

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
            // offset生成
            offsetZ = new float[count];
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
                    // x軸45°に設定する
                    followCameras[i].transform.localEulerAngles = new Vector3(45, 0, 0);
                    offsetZ[i] = -3f;
                    // カメラをそれぞれ担当のプレイヤーの子にセット
                    followCameras[i].transform.parent = player[i].FoodPoint.Move.RayController.Object.transform;
                }


            }  
        }
        
        // カメラ回転キーが入力されるとフェーズをひとつ進める
        // それに応じた位置にカメラが移動する
        private void input()
        {
            if(Input.GetKeyDown("q"))
            {
                changeState();
                cameraMove(followCameras[0]);
                offsetZ[0] *= -1f;
                
            }
            else if(Input.GetKeyDown("p"))
            {
                changeState();
                cameraMove(followCameras[1]);
                offsetZ[1] *= -1f;
            }
        }

        /// <summary>
        /// カメラの移動メソッド
        /// </summary>
        private void cameraMove(GameObject camera)
        {
            switch(RotatePhase)
            {
                case CameraRotate.NORMAL:
                    camera.transform.localEulerAngles = new Vector3(45, 0, 0);
                    break;

                case CameraRotate.REVERSE:
                    camera.transform.localEulerAngles = new Vector3(135, 0, 180);

                    break;
                
            }
        }

        /// <summary>
        /// ステートを次のフェーズに進めるメソッド
        /// </summary>
        private void changeState()
        {
            if((int)RotatePhase <= Enum.GetValues(typeof(CameraRotate)).Cast<int>().Max())
                RotatePhase++;
            else
                RotatePhase = (CameraRotate)Enum.GetValues(typeof(CameraRotate)).Cast<int>().Min();
        }

        private void followObject()
        {
            // X, Z 平面でプレイヤーを追従させる
            for(int i = 0; i < player.Count; i++)
            {
                // 各プレイヤーのX, Y座標取得
                float x = player[i].FoodPoint.Move.RayController.Object.transform.position.x;
                float z = player[i].FoodPoint.Move.RayController.Object.transform.position.z;
                

                // カメラの座標調整(各プレイヤーのX,Z座標を追従)
                followCameras[i].transform.position = new Vector3(x, offsetY, z + offsetZ[i]);
               
            }
        }

        public void Update()
        {
            input();
            followObject();
        }
    }
}

