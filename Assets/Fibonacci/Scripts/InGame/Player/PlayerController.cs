using UnityEngine;
using Fibonacci.InGame.Core;
using Fibonacci.Event;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの全体制御と状態管理を司る司令塔クラス。
    /// ゲームフェーズに応じた物理演算の切り替えや、エリア移動に伴う重力変化の実行、
    /// 移動入力の仲介などを一括して管理します。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private PlayerAnimationController animationController;

        private PlayerMove playerMove;
        private Rigidbody2D rb;
        private PlayerCheck playerCheck;
        private readonly PlayerGravityLogic gravityLogic = new PlayerGravityLogic();

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerCheck = GetComponent<PlayerCheck>();
            playerMove = new PlayerMove(rb, transform, animationController, moveSpeed);
        }

        private void OnEnable()
        {
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        /// <summary>
        /// ゲームフェーズの変更通知を受け取り、物理シミュレーションの有効・無効を切り替えます。
        /// プレイ開始時には位置判定を強制更新し、最新のエリア効果を即座に反映させます。
        /// </summary>
        /// <param name="newPhase">遷移後のゲームフェーズ</param>
        private void HandlePhaseChanged(GamePhase newPhase)
        {
            if (newPhase == GamePhase.Playing)
            {
                rb.simulated = true;

                if (playerCheck != null)
                {
                    playerCheck.ForceCheck(); 
                }
            }
            else if (newPhase == GamePhase.Drawing)
            {
                rb.simulated = false;
            }
        }

        /// <summary>
        /// 所属エリアのインデックスに基づいて、プレイヤーに適用される重力反転などの能力を実行します。
        /// </summary>
        /// <param name="areaIndex">現在のプレイヤー位置に対応するエリア番号</param>
        public void ChangeAreaEffect(int areaIndex)
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                return;
            }

            AbilityType ability = AbilityManager.Instance.GetAbilityAt(areaIndex);
            int gravityDir = (ability == AbilityType.GravityInvert) ? 1 : 0;
            gravityLogic.Execute(rb, this.transform, gravityDir);
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance == null) return;

            bool isPlaying = GameManager.Instance.CurrentPhase == GamePhase.Playing;
            rb.simulated = isPlaying;

            if (!isPlaying) return;

            playerMove.ExecutePhysicsUpdate();
        }

        /// <summary>
        /// 外部の入力管理クラスから移動ベクトルを受け取り、移動ロジックとアニメーションに反映させます。
        /// </summary>
        /// <param name="input">移動方向と強さを示すベクトル</param>
        public void SetMoveInput(Vector2 input)
        {
            playerMove.MoveInput = input;
            playerMove.UpdateAnimation();
        }

        /// <summary>
        /// プレイヤーの位置、速度、重力方向、および入力状態を初期状態にリセットします。
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
                rb.simulated = true;
            }

            gravityLogic.Execute(rb, this.transform, 0);

            if (GameManager.Instance != null && GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                rb.simulated = false;
            }
        }
    }
}