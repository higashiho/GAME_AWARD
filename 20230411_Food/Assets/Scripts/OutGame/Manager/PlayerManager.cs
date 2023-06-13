using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using ObjectInterface;
using Constants;
using OutGame;

namespace Title
{
    /// <summary>
    /// player管理クラス
    /// </summary>
    public class PlayerManager : IActor
    {
        
        /// <summary>
        /// プレイヤーが動いている向き
        /// </summary>
        public Vector3 MoveDis{get{return moveDis;} set{moveDis = value;}}
        private Vector3 moveDis = Vector3.zero;


        /// <summary>
        /// 挙動管理クラス
        /// </summary>
        public PlayerMove Move;


        /// <summary>
        /// プレイヤーオブジェクト
        /// </summary>
        public GameObject Object{get; private set;}

        /// <summary>
        /// プレイヤーのハンドル
        /// </summary>
        /// <value></value>
        public AsyncOperationHandle Handle{get;private set;}

        /// <summary>
        /// playerの挙動アニメーション
        /// </summary>
        //public Animator MoveAnimator{get;private set;}

        private TitlePlayerData data;

        /// <summary>
        /// rayが当たった対象オブジェクト
        /// </summary>
        /// <value></value>
        public GameObject HitObject{get{return hitObject;} set{hitObject = value;}}
        private GameObject hitObject = null;
        /// <summary>
        /// オブジェクトにRayが当たった時のプレイヤーの向き
        /// </summary>
        /// <value></value>
        public Vector3? HitDistance{get{return hitDistance;} set{hitDistance = value;}}
        private Vector3? hitDistance = null;

        /// ray処理
        private PlayerRayProcessing rayProcessing;

        
        /// <summary>
        /// Playerイベント管理クラス
        /// </summary>
        public InputMovementManager MoveEvents{get; private set;}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlayerManager(TitlePlayerData playerData)
        {
            data = playerData;
            // 初期化
            Initialization();
        
            // 更新ループ設定
            Update();
       }

        /// <summary>
        /// 初期化関数
        /// </summary>
        public async void Initialization()
        {
            GameObject playerObj = null;
            // 取得し完了するまで待つ
            Handle = Addressables.LoadAssetAsync<GameObject>(data.InstanceAddress);
            await Handle.Task;

            // プレイヤー生成
            // ゲームオブジェクト型にcastし生成
            playerObj = (GameObject)Handle.Result;

            Object = MonoBehaviour.Instantiate(playerObj, data.InstancePos, Quaternion.identity);

            rayProcessing = new PlayerRayProcessing(data);
            Move = new PlayerMove(data);
            MoveEvents = new InputMovementManager(data);
            //MoveAnimator = Object.transform.GetChild(0).GetComponent<Animator>();
        
        }

        /// <summary>
        /// 更新関数
        /// </summary>
        public async void Update()
        {
            // オブジェクトが生成されるまで待つ
            await UniTask.WaitWhile(() => !Object);


            // Ray用ループ
            Object.UpdateAsObservable()
                .Subscribe(_ => {
                    rayProcessing.Processing();
                })
                .AddTo(Object);


            MoveEvents.SetMovementLoops();
        }   

    }

    /// <summary>
    /// Player挙動管理クラス
    /// </summary>
    public sealed class PlayerMove
    {
        private TitlePlayerData data;
        public PlayerMove(TitlePlayerData tmpData)
        {
            data = tmpData;
            moveSpeed = new Player.PlayerMoveSpeed(data.MoveSpeed);
        }
        private Player.PlayerMoveSpeed moveSpeed;
        /// <summary>
        /// 左移動処理
        /// </summary>
        public void LeftMovement()
        {
            // どの向きに歩いているか設定
            ObjectManager.Player[data.Id].MoveDis = Vector3.left;
            
            // 移動アニメーション再生
            //tmpPlayer.MoveAnimator.SetBool("Move", true);

            // ９０度左を向く
            ObjectManager.Player[data.Id].Object.transform.eulerAngles = TitleConstants.PLAYER_DIRECTION_LEFT;

            // 移動
            ObjectManager.Player[data.Id].Object.transform.position += Vector3.left * moveSpeed.Amount * Time.deltaTime;
        }
        /// <summary>
        /// 右移動処理
        /// </summary>
        public void RightMovement()
        {
            // どの向きに歩いているか設定
            ObjectManager.Player[data.Id].MoveDis = Vector3.right;
            
            // 移動アニメーション再生
            //tmpPlayer.MoveAnimator.SetBool("Move", true);

            // ９０度右を向く
            ObjectManager.Player[data.Id].Object.transform.eulerAngles = TitleConstants.PLAYER_DIRECTION_RIGHT;

            // 移動
            ObjectManager.Player[data.Id].Object.transform.position += Vector3.right * moveSpeed.Amount * Time.deltaTime;
            
        }
        /// <summary>
        /// 前移動処理
        /// </summary>
        public void ForwardMovement()
        {
            // どの向きに歩いているか設定
            ObjectManager.Player[data.Id].MoveDis = Vector3.forward;
            
            // 移動アニメーション再生
            //tmpPlayer.MoveAnimator.SetBool("Move", true);

            // 前を向く
            ObjectManager.Player[data.Id].Object.transform.eulerAngles = Vector3.zero;

            // 移動
            ObjectManager.Player[data.Id].Object.transform.position += Vector3.forward * moveSpeed.Amount * Time.deltaTime;
        }
        /// <summary>
        /// 後ろ移動処理
        /// </summary>
        public void BackMovement()
        {
            // どの向きに歩いているか設定
            ObjectManager.Player[data.Id].MoveDis = Vector3.back;

            // 移動アニメーション再生
            //tmpPlayer.MoveAnimator.SetBool("Move", true);

            // 後ろを向く
            ObjectManager.Player[data.Id].Object.transform.eulerAngles = TitleConstants.PLAYER_DIRECTION_BACK;

            // 移動
            ObjectManager.Player[data.Id].Object.transform.position += Vector3.back * moveSpeed.Amount * Time.deltaTime;
        }
        
        /// <summary>
        /// アニメーションリセット関数
        /// </summary>
        public void ResetAnim()
        {
            // 初期化
            //mpPlayer.MoveAnimator.SetBool("Move", false);
        }

        /// <summary>
        /// 挙動リセット関数
        /// </summary>
        public void ResetMovement()
        {
            // 初期化
            ObjectManager.Player[data.Id].MoveDis = Vector3.zero;
        }
    }

    /// <summary>
    /// PlayerRay管理クラス
    /// </summary>
    public sealed class PlayerRayProcessing
    {
        private TitlePlayerData data;
        private GameObject obj;
        public PlayerRayProcessing(TitlePlayerData tmpData)
        {
            data = tmpData;
            obj = ObjectManager.Player[data.Id].Object;

            rayDistance = new RayDistance(data.RayDistance);
        }
        /// <summary>
        /// PlayerのRayの長さ
        /// </summary>
        private RayDistance rayDistance;

        /// <summary>
        /// Ray処理
        /// </summary>
        public void Processing()
        {
            var forwardRay = new Ray(obj.transform.position, obj.transform.forward);
            Debug.DrawRay(obj.transform.position, obj.transform.forward * rayDistance.Amount, Color.blue);
        
            RaycastHit hit;
            // rayの当たり判定を確認
            if(Physics.Raycast(forwardRay, out hit, rayDistance.Amount))
            {
                // サブジェクトに代入
                ObjectManager.Events.HavePlayerObject[data.Id].OnNext(hit.collider.gameObject);

                // オブジェクトが変わっていなかったら処理中断
                if(ObjectManager.Player[data.Id].HitObject == hit.collider.gameObject) return;

                
                // 当たっていたら向き格納
                ObjectManager.Player[data.Id].HitDistance = obj.transform.eulerAngles;
                // オブジェクト格納
                ObjectManager.Player[data.Id].HitObject = hit.collider.gameObject;

                // 表示できるか
                bool displayableFlag = false;
                
                // サブジェクトに代入
                switch(ObjectManager.Player[data.Id].HitObject.name)
                {
                    case "Refrugerator":
                        ObjectManager.Events.FoodNicknamesTextPoint.OnNext(TitleConstants.TEXT_IMAGE_APPROACH_POS_Y);
                        displayableFlag = true;
                        break;
                    case "RecipeBook": 
                        ObjectManager.Events.DisplayIngredientsListTextPoint.OnNext(TitleConstants.TEXT_IMAGE_APPROACH_POS_Y);
                        displayableFlag = true;
                        break;
                    case "GasBurner":  
                        ObjectManager.Events.GameStartTextPoint.OnNext(TitleConstants.TEXT_IMAGE_APPROACH_POS_Y);
                        displayableFlag = true;
                        break;
                    default:    
                        break;
                }

                    
                // アシストUI表示
                if( displayableFlag &&
                    !ObjectManager.Ui.AssistCanvas.transform.GetChild(data.Id).gameObject.activeSelf)
                    ObjectManager.Ui.SetAssistPlayerUIActive(data.Id, true);

            }
            else 
            {
                // イメージUIが表示されていたら非表示にする
                if(ObjectManager.Ui.AssistCanvas.transform.GetChild(data.Id).gameObject.activeSelf)
                    ObjectManager.Ui.SetAssistPlayerUIActive(data.Id, false);
                    
                // 当たっていなかったらnullに変換
                ObjectManager.Player[data.Id].HitDistance = null;
                ObjectManager.Player[data.Id].HitObject = null;
            }
        }
    }
}

