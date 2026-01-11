using UnityEngine;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの全体制御と状態管理を司る司令塔（Controller）クラス。
    /// ゲームフェーズに応じた動作の許可判定、PlayerMove（移動ロジック）のインスタンス管理、
    /// エリア変更に伴う重力操作やUIとの連携など、プレイヤーに関する各コンポーネントの仲介役として機能します。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private PlayerGravity playerGravity;
        [SerializeField] private PlayerAnimationController animationController;


        private PlayerMove playerMove;
        private Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (playerGravity == null) playerGravity = GetComponent<PlayerGravity>();

            playerMove = new PlayerMove(rb, transform, animationController, moveSpeed);
        }

        void FixedUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }

            playerMove.ExecutePhysicsUpdate();
        }

        public void SetMoveInput(Vector2 input)
        {
            playerMove.MoveInput = input;
            playerMove.UpdateAnimation();
        }

        #region GameLogic (Gravity/Effects)

        /// <summary>
        /// 【追加】プレイヤーの状態を一括リセットします。
        /// PlayerInputManagerのOnGameRestartから呼ばれます。
        /// </summary>
        public void ResetPlayerState()
        {
            // 移動ロジック側の座標・速度リセットを実行
            if (playerMove != null)
            {
                playerMove.ResetPosition();
            }

            // 重力状態を通常に戻す
            if (playerGravity != null)
            {
                playerGravity.SetNormalGravity();
            }

        #endregion
        }
    }
}