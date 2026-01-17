using UnityEngine;
using Fibonacci.InGame.Core;

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
        [SerializeField] private PlayerGravityLogic playerGravity;
        [SerializeField] private PlayerAnimationController animationController;


        private PlayerMove playerMove;
        private Rigidbody2D rb;
        private readonly PlayerGravityLogic gravityLogic = new PlayerGravityLogic();

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerMove = new PlayerMove(rb, transform, animationController, moveSpeed);
        }

        /// <summary>
        /// 司令塔への「エリアが変わったぞ」という報告窓口
        /// </summary>
        public void ChangeAreaEffect(int areaIndex)
        {
            AbilityType ability = AbilityManager.Instance.GetAbilityAt(areaIndex);
            int gravityDir = (ability == AbilityType.GravityInvert) ? 1 : 0;
            gravityLogic.Execute(rb, this.transform, gravityDir);
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
        /// プレイヤーの全状態（座標、速度、重力、向き）を初期化します。
        /// </summary>
        public void ResetPlayerState()
        {
            if (playerMove != null)
            {
                playerMove.ResetPosition();
                playerMove.MoveInput = Vector2.zero;
                playerMove.UpdateAnimation();
            }

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            ChangeAreaEffect(0);

            Debug.Log("<color=green>【PlayerController】</color> プレイヤーの全状態を正常にリセットしました。");
        }

        #endregion 
    }
}